using System.IO;
using System.Text;
namespace Carlos.Media
{
    public class MidiGenerator
    {
        public static void GenerateMidiFile(string fileName, (int noteNumber, int velocity, int duration)[] notes)
        {
            using var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            using var writer = new BinaryWriter(stream, Encoding.UTF8, true);
            // 写入 MIDI 文件头
            writer.Write("MThd");
            writer.Write(6);  // 头块长度
            writer.Write((short)1);  // 文件类型（1 表示多音轨）
            writer.Write((short)1);  // 音轨数
            writer.Write((short)960);  // 时间分割（每拍 960 刻度）

            // 写入音轨块头
            writer.Write("MTrk");
            writer.Write(0);  // 音轨块长度（暂时写 0，后续计算实际长度）

            // 记录音轨块的起始位置
            long trackBlockStartPosition = stream.Position;

            // 写入音轨名称
            byte[] trackNameBytes = Encoding.UTF8.GetBytes("");
            WriteVariableLength(writer, trackNameBytes.Length);
            writer.Write((byte)0xFF);  // 元事件类型
            writer.Write((byte)0x03);  // 音轨名称
            writer.Write(trackNameBytes);

            // 写入音符事件
            foreach (var note in notes)
            {
                int noteNumber = note.noteNumber;  // 音符编号 (0-127)
                int velocity = note.velocity;    // 音符强度 (0-127)
                int duration = note.duration;    // 音符持续时间 (以刻度为单位)

                // 音符开始事件
                WriteVariableLength(writer, 0);  // Delta 时间
                WriteNoteOnEvent(writer, noteNumber, velocity);

                // 音符结束事件
                WriteVariableLength(writer, duration);  // Delta 时间
                WriteNoteOffEvent(writer, noteNumber, 0);
            }

            // 写入结束事件
            WriteVariableLength(writer, 0);  // Delta 时间
            writer.Write((byte)0xFF);  // 元事件类型
            writer.Write((byte)0x2F);  // 结束事件
            writer.Write((byte)0x00);  // 数据长度

            // 记录实际音轨块长度
            long trackBlockEndPosition = stream.Position;
            long trackBlockLength = trackBlockEndPosition - trackBlockStartPosition;

            // 回到音轨块长度的位置，写入实际长度
            stream.Position = trackBlockStartPosition - 4;
            writer.Write((int)trackBlockLength);
        }
        private static void WriteVariableLength(BinaryWriter writer, int value)
        {
            do
            {
                int b = value & 0x7F;
                value >>= 7;
                if (value != 0)
                {
                    b |= 0x80;
                }
                writer.Write((byte)b);
            } while (value != 0);
        }
        private static void WriteNoteOnEvent(BinaryWriter writer, int noteNumber, int velocity)
        {
            writer.Write((byte)0x90);  // MIDI 通道 1 的音符开事件
            writer.Write((byte)noteNumber);
            writer.Write((byte)velocity);
        }
        private static void WriteNoteOffEvent(BinaryWriter writer, int noteNumber, int velocity)
        {
            writer.Write((byte)0x80);  // MIDI 通道 1 的音符关事件
            writer.Write((byte)noteNumber);
            writer.Write((byte)velocity);
        }
    }
}
