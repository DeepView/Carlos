using System;
using Carlos.Extends;
using System.Drawing;
using Carlos.Enumerations;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace Carlos.Media
{
    /// <summary>
    /// 图像灰阶化的帮助类。
    /// </summary>
    public class ImageGrayHelper
    {
        private const double GRAY_WEIGHT_R = 0.299f;                                                                                    //默认的红色灰阶化权重。
        private const double GRAY_WEIGHT_G = 0.587f;                                                                                    //默认的绿色灰阶化权重。
        private const double GRAY_WEIGHT_B = 0.114f;                                                                                    //默认的蓝色灰阶化权重。
        private static readonly (double wRed, double wGreen, double wBlue) DEF_GW = (GRAY_WEIGHT_R, GRAY_WEIGHT_G, GRAY_WEIGHT_B);      //默认的灰阶化权重数据。
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
            var (weightRed, weightGreen, weightBlue) = weight ?? DEF_GW;
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
                        GrayMode.WAvgEyes => (byte)(DEF_GW.wRed * pixel.R + DEF_GW.wGreen * pixel.G + DEF_GW.wBlue * pixel.B),
                        GrayMode.WAvgCustomize => (byte)(weightRed * pixel.R + weightGreen * pixel.G + weightBlue * pixel.B),
                        _ => throw new NotSupportedException($"GrayMode {grayMode} is not supported.")
                    };
                    grayData[y + 1, x + 1] = grayValue;
                }
            }
            return grayData;
        }
        /// <summary>
        /// 灰度图像数据转换为灰度表格数据（异步化）。
        /// </summary>
        /// <param name="bitmap">需要获取灰度数据的图像。</param>
        /// <param name="grayMode">指定的灰度模式。</param>
        /// <param name="weight">当灰度模式为GrayMode.WAvgCustomize时，该参数可以自定义加权平均值灰度模式的权重数据。</param>
        /// <returns>该操作将会返回一个Carlos.Extends.Table数据类型，该数据类型能够以表格的形式存放指定图像的每一个像素的灰度数据。</returns>
        /// <exception cref="ArgumentNullException">当图像数据为空时，则将会抛出这个异常。</exception>
        /// <exception cref="ArgumentOutOfRangeException">当灰度模式参数无效时，则将会抛出这个异常。</exception>
        /// <exception cref="NotSupportedException">当灰度模式参数不受支持时，则将会抛出这个异常。</exception>
        public static async Task<Table<byte>> GetImageGrayDataAsync(Bitmap bitmap, GrayMode grayMode, (double weightRed, double weightGreen, double weightBlue)? weight = null)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));
            if (!Enum.IsDefined(typeof(GrayMode), grayMode))
                throw new ArgumentOutOfRangeException(nameof(grayMode));
            var (weightRed, weightGreen, weightBlue) = weight ?? DEF_GW;
            int height = bitmap.Height;
            int width = bitmap.Width;
            var grayData = new Table<byte>(height, width);
            Color pixel;
            byte grayValue;
            await Task.Run(() =>
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        pixel = bitmap.GetPixel(x, y);
                        grayValue = grayMode switch
                        {
                            GrayMode.Max => Math.Max(pixel.R, Math.Max(pixel.G, pixel.B)),
                            GrayMode.Avg => (byte)((pixel.R + pixel.G + pixel.B) / 3),
                            GrayMode.WAvgEyes => (byte)(DEF_GW.wRed * pixel.R + DEF_GW.wGreen * pixel.G + DEF_GW.wBlue * pixel.B),
                            GrayMode.WAvgCustomize => (byte)(weightRed * pixel.R + weightGreen * pixel.G + weightBlue * pixel.B),
                            _ => throw new NotSupportedException($"GrayMode {grayMode} is not supported.")
                        };
                        grayData[y + 1, x + 1] = grayValue;
                    }
                }
            });
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
        /// 利用灰度表格数据创建一个灰度图像（异步化）。
        /// </summary>
        /// <param name="grayData">指定的灰度数据集。</param>
        /// <returns>该操作将会返回一个Bitmap对象，用于存放创建好的灰度图像。</returns>
        /// <exception cref="ArgumentNullException">如果灰度数据为NULL时，则将抛出这个异常。</exception>
        public static async Task<Bitmap> CreateGrayImageAsync(Table<byte> grayData)
        {
            if (grayData == null)
                throw new ArgumentNullException(nameof(grayData));
            int height = (int)grayData.Rows;
            int width = (int)grayData.Cols;
            Bitmap bitmap = new Bitmap(width, height);
            await Task.Run(() =>
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte grayValue = grayData[y + 1, x + 1];
                        bitmap.SetPixel(x, y, Color.FromArgb(grayValue, grayValue, grayValue));
                    }
                }
            });
            return bitmap;
        }
        /// <summary>
        /// 检查指定的Bitmap对象是否具有Alpha通道。
        /// </summary>
        /// <param name="image">指定的Bitmap对象。</param>
        /// <returns>如果指定的Bitmap对象拥有Alpha通道，则会返回true，否则返回false。</returns>
        /// <exception cref="ArgumentNullException">如果参数bitmap为NULL，则将抛出这个异常。</exception>
        public static bool HasAlphaChannel(Image image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image)); 
            PixelFormat format = image.PixelFormat;
            return format == PixelFormat.Format32bppArgb || format == PixelFormat.Format64bppPArgb;
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
                    freqDict.Add((byte)i, minFreq + (maxFreq - minFreq) / 256 * i);
            }
            return freqDict;
        }
    }
}
