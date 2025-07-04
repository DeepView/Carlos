using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Carlos.Media.GoldenRecord
{
    public class ImageToMidiCoder
    {
        public static void EncodeDataToMidi(string data, string filePath)
        {
            // 将字符串转换为字节数组
            byte[] dataBytes = Encoding.ASCII.GetBytes(data);

            // 创建 MIDI 文件
            var midiFile = new MidiFile();
            var trackChunk = new TrackChunk();
            midiFile.Chunks.Add(trackChunk);

            // 设置时间格式 (500 ticks/quarter note)
            midiFile.TimeDivision = new TicksPerQuarterNoteTimeDivision(500);

            // 创建事件列表
            var events = new List<MidiEvent>();
            int currentTime = 0;
            const int deltaTime = 10; // 每个数据点之间的时间间隔 (ticks)

            // 添加音轨名称
            events.Add(new SequenceTrackNameEvent("Encoded Data Track") { DeltaTime = 0 });

            // 添加音色选择事件 (钢琴)
            events.Add(new ProgramChangeEvent((SevenBitNumber)0) { DeltaTime = 0 });

            foreach (byte value in dataBytes)
            {
                // 计算音符编号和力度
                byte noteNumber = (byte)(value / 128); // 0 或 1
                byte velocity = (byte)(value % 128);  // 0-127

                // 添加 NoteOn 事件
                events.Add(new NoteOnEvent((SevenBitNumber)noteNumber, (SevenBitNumber)velocity)
                {
                    DeltaTime = currentTime,
                    Channel = (FourBitNumber)0 // 通道 1
                });

                // 重置当前时间为 0，准备添加 NoteOff
                currentTime = 0;

                // 添加 NoteOff 事件 (在 NoteOn 后立即关闭)
                events.Add(new NoteOffEvent((SevenBitNumber)noteNumber, (SevenBitNumber)0)
                {
                    DeltaTime = 1, // 1 tick 后关闭
                    Channel = (FourBitNumber)0
                });
                // Fix for CS1729: “EndOfTrackEvent” does not contain a constructor that takes 1 argument.
                // The `EndOfTrackEvent` class does not accept any arguments in its constructor. 
                // The incorrect line is replaced with the correct instantiation of `EndOfTrackEvent`.

                //events.Add(new EndOfTrackEvent { DeltaTime = currentTime });
                // 设置下一个 NoteOn 的时间间隔
                currentTime = deltaTime - 1; // 减去 NoteOff 的 1 tick
            }

            // 添加结束事件
            //events.Add(new EndOfTrackEvent { DeltaTime = currentTime });

            // 将事件添加到音轨
            trackChunk.Events.AddRange(events);

            // 保存 MIDI 文件
            midiFile.Write(filePath, true);
        }
        public static string DecodeMidiToData(string filePath)
        {
            // 读取 MIDI 文件
            var midiFile = MidiFile.Read(filePath);

            // 收集所有 NoteOn 事件
            var noteOnEvents = new List<NoteOnEvent>();
            foreach (var chunk in midiFile.Chunks.OfType<TrackChunk>())
            {
                noteOnEvents.AddRange(chunk.Events.OfType<NoteOnEvent>());
            }

            // 按时间顺序排序
            noteOnEvents = noteOnEvents
                .OrderBy(e => e.Channel)
                .ThenBy(e => TimeConverter.ConvertTo<MetricTimeSpan>(e.DeltaTime, midiFile.GetTempoMap()))
                .ToList();

            // 解码数据
            var dataBytes = new List<byte>();
            foreach (var noteEvent in noteOnEvents)
            {
                // 跳过无效事件（力度为0的NoteOn事件实际上是NoteOff）
                if (noteEvent.Velocity == 0) continue;

                // 解码数据：原始值 = noteNumber * 128 + velocity
                byte value = (byte)((noteEvent.NoteNumber * 128) + noteEvent.Velocity);
                dataBytes.Add(value);
            }

            // 将字节数组转换为字符串
            return Encoding.ASCII.GetString(dataBytes.ToArray());
        }
    }
}
