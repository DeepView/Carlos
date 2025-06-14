using System;
using System.IO;
using System.Text;
using System.Drawing;
using Carlos.Extends;
using Carlos.Enumerations;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace Carlos.Media.GoldenRecord
{
    /// <summary>
    /// 图像声音化的帮助类。
    /// </summary>
    public class ImageGrFormatHelper
    {
        private const double GRAY_WEIGHT_RED = 0.299;
        private const double GRAY_WEIGHT_GREEN = 0.587;
        private const double GRAY_WEIGHT_BLUE = 0.114;
        private static (double wRed, double wGreen, double wBlue) defaultGw = (GRAY_WEIGHT_RED, GRAY_WEIGHT_GREEN, GRAY_WEIGHT_BLUE);
        /// <summary>
        /// 灰度图像数据转换为灰度表格数据。
        /// </summary>
        /// <param name="bitmap">需要获取灰度数据的图像。</param>
        /// <param name="grayMode">指定的灰度模式。</param>
        /// <param name="weight">当灰度模式为GrayMode.WAvgCustomize时，该参数可以自定义加权平均值灰度模式的权重数据。</param>
        /// <returns>该操作将会返回一个Carlos.Extends.Table数据类型，该数据类型能够以表格的形式存放指定图像的每一个像素的灰度数据。</returns>
        /// <exception cref="ArgumentNullException">当图像数据为空时，则将会抛出这个异常。</exception>
        /// <exception cref="ArgumentOutOfRangeException">当灰度模式参数无效时，则将会抛出这个异常。</exception>
        /// <exception cref="NotSupportedException">当灰度模式参数不受支持时，则将会抛出这个异常。</exception>
        public static Table<byte> GetImageGrayData(Bitmap bitmap, GrayMode grayMode, (double weightRed, double weightGreen, double weightBlue)? weight = null)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));
            if (!Enum.IsDefined(typeof(GrayMode), grayMode))
                throw new ArgumentOutOfRangeException(nameof(grayMode));
            var (weightRed, weightGreen, weightBlue) = weight ?? (GRAY_WEIGHT_RED, GRAY_WEIGHT_GREEN, GRAY_WEIGHT_BLUE);
            int height = bitmap.Height;
            int width = bitmap.Width;
            var grayData = new Table<byte>(height, width);
            Color pixel;
            byte grayValue;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixel = bitmap.GetPixel(x, y);
                    grayValue = grayMode switch
                    {
                        GrayMode.Max => Math.Max(pixel.R, Math.Max(pixel.G, pixel.B)),
                        GrayMode.Avg => (byte)((pixel.R + pixel.G + pixel.B) / 3),
                        GrayMode.WAvgEyes => (byte)(defaultGw.wRed * pixel.R + defaultGw.wGreen * pixel.G + defaultGw.wBlue * pixel.B),
                        GrayMode.WAvgCustomize => (byte)(weightRed * pixel.R + weightGreen * pixel.G + weightBlue * pixel.B),
                        _ => throw new NotSupportedException($"GrayMode {grayMode} is not supported.")
                    };
                    grayData[y + 1, x + 1] = grayValue;
                }
            }
            return grayData;
        }
        /// <summary>
        /// 利用灰度表格数据创建一个灰度图像。
        /// </summary>
        /// <param name="grayData">指定的灰度数据集。</param>
        /// <returns>该操作将会返回一个Bitmap对象，用于存放创建好的灰度图像。</returns>
        /// <exception cref="ArgumentNullException">如果灰度数据为NULL时，则将抛出这个异常。</exception>
        public static Bitmap CreateGrayImage(Table<byte> grayData)
        {
            if (grayData == null)
                throw new ArgumentNullException(nameof(grayData));
            int height = (int)grayData.Rows;
            int width = (int)grayData.Cols;
            Bitmap bitmap = new Bitmap(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte grayValue = grayData[y + 1, x + 1];
                    bitmap.SetPixel(x, y, Color.FromArgb(grayValue, grayValue, grayValue));
                }
            }
            return bitmap;
        }
        /// <summary>
        /// 获取灰度值到频率的映射字典。
        /// </summary>
        /// <param name="minFreq">指定的最小频率。</param>
        /// <param name="maxFreq">指定的最大频率。</param>
        /// <returns>该操作将会返回一个字典，用于存放灰度值到频率的映射数据。</returns>
        /// <exception cref="ArgumentException">当最小频率大于或等于最大频率，或者最大频率与最小频率的差值小于256时，则将会抛出这个异常。</exception>
        public static Dictionary<byte, int> GetGrayToFreqDictionary(int minFreq, int maxFreq)
        {
            var freqDict = new Dictionary<byte, int>(256);
            for (int i = 0; i < 256; i++)
            {
                if (minFreq >= maxFreq || maxFreq - minFreq < 256)
                    throw new ArgumentException("The format of the frequency range data is incorrect.");
                else
                    freqDict.Add((byte)i, minFreq + ((maxFreq - minFreq) / 256) * i);
            }
            return freqDict;
        }
        /// <summary>
        /// 将图像灰阶化之后转换为波形文件。
        /// </summary>
        /// <param name="fileName">波形文件的文件名称（含保存位置）。</param>
        /// <param name="bitmap">需要被转换为声音格式的图像，但是转换声音之前必须灰阶化。</param>
        /// <param name="grayMode">图像的灰阶模式。</param>
        /// <param name="information">转换为声波文件之前的重要信息。</param>
        /// <exception cref="ArgumentNullException">当文件名参数为空，或者为空白时，则将抛出这个异常。</exception>
        /// <exception cref="ArgumentOutOfRangeException">当图像的分辨率超过32767x32767时，则将会抛出这个异常。</exception>
        /// <remarks>
        /// 需要注意的问题是，转换为WAV文件的频率数据中，最后四个数据分别是图像的高度、宽度、转换为声音之后的最小频率和最大频率，这些数据将会被附加到WAV文件的末尾。
        /// </remarks>
        public static void ConvertToWaveFile(string fileName, Bitmap bitmap, GrayMode grayMode, GoldenRecordInformation information)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));
            if (bitmap.Height > short.MaxValue || bitmap.Width > short.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(bitmap), "The height or width of the bitmap must not exceed 32767 pixels.");
            var grayData = GetImageGrayData(bitmap, grayMode);
            var frequencies = new int[grayData.Length + 4];
            frequencies = FreqMapping(
            GetGrayToFreqDictionary(
                information.MinimumFreq,
                information.MaximumFreq
                ),
            grayData.ToArray()
            );
            frequencies[^4] = bitmap.Height;
            frequencies[^3] = bitmap.Width;
            frequencies[^2] = information.MinimumFreq;
            frequencies[^1] = information.MaximumFreq;
            double durationPerNote = 1.0f / (double)information.FreqResolution;
            int sampleRate = information.FreqResolution * 2;
            long totalSamples = (long)(frequencies.Length * sampleRate * durationPerNote);
            byte[] audioData = new byte[totalSamples * 2];
            int sampleIndex = 0;
            foreach (int frequency in frequencies)
            {
                int samplesForThisNote = (int)(sampleRate * durationPerNote);
                for (int sample = 0; sample < samplesForThisNote; sample++)
                {
                    double t = (double)sample / sampleRate;
                    double wave = Math.Sin(2 * Math.PI * frequency * t);
                    short sampleValue = (short)(wave * short.MaxValue);
                    byte[] sampleBytes = BitConverter.GetBytes(sampleValue);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(sampleBytes);
                    }
                    sampleBytes.CopyTo(audioData, sampleIndex * 2);
                    sampleIndex++;
                }
            }
            using FileStream fs = new FileStream(fileName, FileMode.Create);
            WriteWavHeader(fs, totalSamples, (int)sampleRate);
            fs.Write(audioData, 0, audioData.Length);
        }
        /// <summary>
        /// 创建WAV文件的文件头。
        /// </summary>
        /// <param name="fs">文件流。</param>
        /// <param name="numSamples">WAV文件的样本数。</param>
        /// <param name="sampleRate">WAV文件的采样率。</param>
        private static void WriteWavHeader(FileStream fs, long numSamples, int sampleRate)
        {
            long totalDataLen = numSamples * 2 + 36;
            long ungappedDataLen = numSamples * 2;
            byte[] header = new byte[44];
            header[0] = (byte)'R'; header[1] = (byte)'I'; header[2] = (byte)'F'; header[3] = (byte)'F';
            BitConverter.GetBytes((int)(totalDataLen - 8)).CopyTo(header, 4);
            header[8] = (byte)'W'; header[9] = (byte)'A'; header[10] = (byte)'V'; header[11] = (byte)'E';
            header[12] = (byte)'f'; header[13] = (byte)'m'; header[14] = (byte)'t'; header[15] = (byte)' ';
            BitConverter.GetBytes(16).CopyTo(header, 16);
            BitConverter.GetBytes((short)1).CopyTo(header, 20);
            BitConverter.GetBytes((short)1).CopyTo(header, 22);
            BitConverter.GetBytes(sampleRate).CopyTo(header, 24);
            long byteRate = sampleRate * 2;
            BitConverter.GetBytes((int)byteRate).CopyTo(header, 28);
            BitConverter.GetBytes((short)2).CopyTo(header, 32);
            BitConverter.GetBytes((short)16).CopyTo(header, 34);
            header[36] = (byte)'d'; header[37] = (byte)'a'; header[38] = (byte)'t'; header[39] = (byte)'a';
            BitConverter.GetBytes((int)ungappedDataLen).CopyTo(header, 40);
            fs.Write(header, 0, header.Length);
        }
        /// <summary>
        /// 将灰阶数据通过映射表转换为频率数据。
        /// </summary>
        /// <param name="dict">灰阶数据与频率的映射表。</param>
        /// <param name="grayData">灰阶数据。</param>
        /// <returns>该操作将会返回一个int数组，用于存放转换之后的频率数据。</returns>
        private static int[] FreqMapping(Dictionary<byte, int> dict, byte[] grayData)
        {
            int[] mappedData = new int[grayData.Length];
            Parallel.For(0, grayData.Length, i =>
            {
                if (dict.TryGetValue(grayData[i], out int freq))
                    mappedData[i] = freq;
                else
                    mappedData[i] = 0;
            });
            return mappedData;
        }
    }
}
