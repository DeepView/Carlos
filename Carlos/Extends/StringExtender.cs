using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
namespace Carlos.Extends
{
    /// <summary>
    /// 字符串操作扩展类。
    /// </summary>
    public static class StringExtender
    {
        private const int NUM_ZERO = 0;
        /// <summary>
        /// 剪切指定字符串的左边部分。
        /// </summary>
        /// <param name="source">需要被剪切的字符串。</param>
        /// <param name="length">需要剪切的长度。</param>
        /// <returns>该操作将会返回一个全新的字符串，这个字符串就是剪切操作结束之后的数据。</returns>
        /// <remarks>
        /// 下面的一段代码展示了这个方法如何使用。
        /// <code  language="cs">
        /// string src = "HelloWorld";
        /// string s = StringExtender.Left(src, 5);
        /// Console.Write(s);
        /// //Console Output Result:
        /// //Hello
        /// </code>
        /// 当然，<c>StringExtender.Right(String, Int32)</c>的使用方法也一样。
        /// </remarks>
        public static string Left(this string source, int length) => source[..length];
        /// <summary>
        /// 剪切指定字符串的右边部分。
        /// </summary>
        /// <param name="source">需要被剪切的字符串。</param>
        /// <param name="length">需要剪切的长度。</param>
        /// <returns>该操作将会返回一个全新的字符串，这个字符串就是剪切操作结束之后的数据。</returns>
        public static string Right(this string source, int length) => source.Substring(source.Length - length, length);
        /// <summary>
        /// 用字符指针的方式比较两个字符串是否相同。
        /// </summary>
        /// <param name="leftString">第一个字符串。</param>
        /// <param name="rightString">第二个字符串。</param>
        /// <returns>该操作将会返回一个Boolean数据，该数据说明了两个字符串是否相同，如果相同则返回true，否则返回false。</returns>
        /// <remarks>该方法由于使用了指针这一特性，所以包含了一部分相对于CLR环境而言不安全的代码。另外，如果需要比较中文或者其他非英语和阿拉伯数字字符的字符串，则不建议使用该方法，因为该方法中文等字符串可能会造成额外的资源和时间开销。</remarks>
        public static bool IsEquals(this string leftString, string rightString)
        {
            bool bRet = true;
            int nC1 = NUM_ZERO, nC2 = NUM_ZERO, nLen = NUM_ZERO;
            int i = NUM_ZERO;
            if (leftString.Length != rightString.Length) return false;
            nC1 = ((leftString.Length % 4) != NUM_ZERO) ? (4 - leftString.Length % 4) : NUM_ZERO;
            nC2 = ((rightString.Length % 4) != NUM_ZERO) ? (4 - leftString.Length % 4) : NUM_ZERO;
            nLen = (nC1 > nC2) ? nC1 : nC2;
            for (i = NUM_ZERO; i < nLen; i++)
            {
                if (i < nC1) leftString += " ";
                if (i < nC2) rightString += " ";
            }
            unsafe
            {
                fixed (char* psStr1 = leftString) fixed (char* psStr2 = rightString)
                {
                    char* psTemp1 = psStr1;
                    char* psTemp2 = psStr2;
                    while (i < leftString.Length)
                    {
                        if (*(long*)psTemp1 != (*(long*)psTemp2))
                        {
                            bRet = false;
                            break;
                        }
                        i += 4;
                        psTemp1 += 4;
                        psTemp2 += 4;
                    }
                }
            }
            return bRet;
        }
        /// <summary>
        /// 反转字符串中每一个字符的顺序。
        /// </summary>
        /// <param name="source">需要被反转顺序的字符串</param>
        /// <returns>该操作将会返回一个全新的String实例，这个实例包含了source参数指定字符串的反转字符串。</returns>
        public static string Reversal(this string source) => Reversal(source, false);
        /// <summary>
        /// 反转字符串中每一个字符的顺序，可以选择是否使用指针操作来实现字符串操作，大多数情况下，用指针实现这个操作将会略微减少时间上的开销。
        /// </summary>
        /// <param name="source">需要被反转顺序的字符串</param>
        /// <param name="isUsingPtr">是否使用指针来执行字符串反转的操作，如果这个参数为true，则将会使用指针来实现字符串反转操作，反之则使用标准模式来实现这个操作。</param>
        /// <returns>该操作将会返回一个全新的String实例，这个实例包含了source参数指定字符串的反转字符串。</returns>
        public static string Reversal(this string source, bool isUsingPtr)
        {
            if (isUsingPtr)
            {
                char tmp;
                unsafe
                {
                    fixed (char* ps = source)
                    {
                        for (int i = 0; i < source.Length / 2; i++)
                        {
                            tmp = ps[i];
                            ps[i] = ps[source.Length - i - 1];
                            ps[source.Length - i - 1] = tmp;
                        }
                    }
                }
                return source;
            }
            else
            {
                char[] reversal = source.Reverse().ToArray();
                return new string(reversal);
            }
        }
        /// <summary>
        /// 将指定字符串转换为对应的字节码列表。
        /// </summary>
        /// <param name="source">需要被转换的字符串。</param>
        /// <returns>该操作将会返回一个泛型列表，这个列表存储的是一系列Int32数据，用于表示一些字节码。</returns>
        public static List<int> ToCode(this string source)
        {
            char[] src = source.ToArray();
            List<int> result = new List<int>();
            foreach (char item in src)
            {
                int sglCode = Convert.ToInt32(item);
                result.Add(sglCode);
            }
            return result;
        }
        /// <summary>
        /// 一个简化版的String.Split方法，基于数组中的字符将字符串拆分为多个子字符串。
        /// </summary>
        /// <param name="source">需要被拆分的字符串。</param>
        /// <param name="separator">拆分字符串的字符依据。</param>
        /// <returns>该操作将会返回一个字符串数组，这个数组包含了所有拆分出来的字符串。</returns>
        public static string[] Split(this string source, char separator) => Split(source, separator, StringSplitOptions.None);
        /// <summary>
        /// 一个简化版的String.Split方法，基于数组中的字符将字符串拆分为多个子字符串，并指定拆分方式。
        /// </summary>
        /// <param name="source">需要被拆分的字符串。</param>
        /// <param name="separator">拆分字符串的字符依据。</param>
        /// <param name="options">拆分字符串的方式。</param>
        /// <returns>该操作将会返回一个字符串数组，这个数组包含了所有拆分出来的字符串。</returns>
        public static string[] Split(string source, char separator, StringSplitOptions options) => source.Split(new char[] { separator }, options);
        /// <summary>
        /// 将字节码数据转换为指定编码的字符串。
        /// </summary>
        /// <param name="bytes">需要被转换的字节码数据。</param>
        /// <param name="encoding">指定的编码格式。</param>
        /// <returns>该操作将会返回转换成功之后的数据。</returns>
        public static string ConvertEncoding(byte[] bytes, Encoding encoding) => encoding.GetString(bytes);
    }
}
