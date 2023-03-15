using System;
namespace Carlos.Extends
{
    /// <summary>
    /// 进制转换帮助类。
    /// </summary>
    public sealed class BaseConverter
    {
        /// <summary>
        /// 开始进制转换，如果转换失败，则会返回与源数据相同的数据。
        /// </summary>
        /// <param name="input">需要被转换的数据源。</param>
        /// <param name="fromType">被转换数据的进制类型。</param>
        /// <param name="toType">转换目标的进制类型。</param>
        /// <returns>该操作会返回一个由toType决定的进制类型的数据，且这个数据由string数据类型封装。</returns>
        public static string ConvertTo(string input, byte fromType, byte toType) => fromType switch
        {
            2 => ConvertFromBin(input, toType),
            8 => ConvertFromOct(input, toType),
            10 => ConvertFromDec(input, toType),
            16 => ConvertFromHex(input, toType),
            _ => input,
        };
        /// <summary>
        /// 从二进制转换成其他进制。
        /// </summary>
        /// <param name="input">需要被转换的数据源。</param>
        /// <param name="toType">转换目标的进制类型。</param>
        /// <returns>该操作会返回一个由toType决定的进制类型的数据，且这个数据由string数据类型封装。</returns>
        private static string ConvertFromBin(string input, byte toType) => toType switch
        {
            8 => Convert.ToString(Convert.ToInt64(input, 2), 8),
            10 => Convert.ToInt64(input, 2).ToString(),
            16 => Convert.ToString(Convert.ToInt64(input, 2), 16),
            _ => input,
        };
        /// <summary>
        /// 从八进制转换成其他进制。
        /// </summary>
        /// <param name="input">需要被转换的数据源。</param>
        /// <param name="toType">转换目标的进制类型。</param>
        /// <returns>该操作会返回一个由toType决定的进制类型的数据，且这个数据由string数据类型封装。</returns>
        private static string ConvertFromOct(string input, byte toType) => toType switch
        {
            2 => Convert.ToString(Convert.ToInt64(input, 8), 2),
            10 => Convert.ToInt64(input, 8).ToString(),
            16 => Convert.ToString(Convert.ToInt64(input, 8), 16),
            _ => input,
        };
        /// <summary>
        /// 从十进制转换成其他进制。
        /// </summary>
        /// <param name="input">需要被转换的数据源。</param>
        /// <param name="toType">转换目标的进制类型。</param>
        /// <returns>该操作会返回一个由toType决定的进制类型的数据，且这个数据由string数据类型封装。</returns>
        private static string ConvertFromDec(string input, int toType) => toType switch
        {
            2 => Convert.ToString(Convert.ToInt64(input), 2),
            8 => Convert.ToString(Convert.ToInt64(input), 8),
            16 => Convert.ToString(Convert.ToInt64(input), 16),
            _ => input,
        };
        /// <summary>
        /// 从十六进制转换成其他进制。
        /// </summary>
        /// <param name="input">需要被转换的数据源。</param>
        /// <param name="toType">转换目标的进制类型。</param>
        /// <returns>该操作会返回一个由toType决定的进制类型的数据，且这个数据由string数据类型封装。</returns>
        private static string ConvertFromHex(string input, int toType) => toType switch
        {
            2 => Convert.ToString(Convert.ToInt64(input, 16), 2),
            8 => Convert.ToString(Convert.ToInt64(input, 16), 8),
            10 => Convert.ToInt64(input, 16).ToString(),
            _ => input,
        };
    }
}
