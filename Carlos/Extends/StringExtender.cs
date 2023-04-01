﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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
        /// <param name="source">需要被反转顺序的字符串。</param>
        /// <returns>该操作将会返回一个全新的String实例，这个实例包含了source参数指定字符串的反转字符串。</returns>
        public static string Reversal(this string source) => source.SafeReversal();
        /// <summary>
        /// 反转字符串中每一个字符的顺序，可以选择是否使用指针操作来实现字符串操作，大多数情况下，用指针实现这个操作将会略微减少时间上的开销。
        /// </summary>
        /// <param name="source">需要被反转顺序的字符串。</param>
        /// <param name="isUsingPtr">请谨慎使用这个参数，该参数用于确定是否使用指针来执行字符串反转的操作，如果这个参数为true，则将会使用指针来实现字符串反转操作，反之则使用标准模式来实现这个操作。</param>
        /// <returns>该操作将会返回一个全新的String实例，这个实例包含了source参数指定字符串的反转字符串。</returns>
        /// <remarks>
        /// <para>由于String属于不可变对象，因此，所有的safe操作对于CLR而言都是安全的。因此，通过指针强行修改String对象的操作对于CLR而言属于未定义的行为（Undefined behavior），虽说在很多情况下，这种行为可以达到用户预期的目的，但是这些操作依旧存在不少隐患，这些隐患会导致某些场合下，导致代码无法达到用户的预期，比如说下面的这段代码：</para>
        /// <code language="cs">
        /// public static void Main(string[] args)
        /// {
        ///     string a = "devil";
        ///     string b = "devil";
        ///     a.Reversal(true);
        ///     Console.WriteLine(b);
        /// }
        /// </code>
        /// <para>如果不通过unsafe的方式执行字符串反转操作，上面的代码可以确定会输出<c>devil</c>。但是上面的代码一旦执行，有很大概率会输出这样一行结果：<c>lived</c>。</para>
        /// <para>这是由于CLR会针对不可变设计的对象在性能开销上做了一定程度的性能改善与优化，因此看似两个毫无关联且值相同的String对象，一旦对某一个String对象执行了unsafe操作，将会影响到另外一个String对象，因为这两个毫无关联的String对象，其引用地址可能指向的都是同一个托管堆。</para>
        /// </remarks>
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
        /// 通过更加安全与高效的方式来反转字符串，是String.Reversal(true)的安全版本。
        /// </summary>
        /// <param name="source">需要被反转顺序的字符串</param>
        /// <returns>该操作将会返回一个全新的String实例，这个实例包含了source参数指定字符串的反转字符串。</returns>
        public static string SafeReversal(this string source) => string.Create(
            source.Length,
            source,
            (dst, source) =>
            {
                var src = source.AsSpan();
                for (var i = 0; i < src.Length; i++)
                {
                    dst[i] = src[^(i + 1)];
                }
            });
        /// <summary>
        /// 拼接两个字符串，该操作是通过指针的方式实现。
        /// </summary>
        /// <param name="source">原始字符串。</param>
        /// <param name="spliced">需要拼接到原始字符串后面的字符串。</param>
        /// <returns>该操作将会返回一个新的字符串，这个字符串正是拼接之后的字符串。</returns>
        public static string Splice(this string source, string spliced)
        {
            char[] result;
            unsafe
            {
                int i = 0, j = 0;
                fixed (char* src = source, spl = spliced)
                {
                    while (src[i] != '\0') i++;
                    while (spl[j] != '\0')
                    {
                        src[i] = spl[j];
                        i++;
                        j++;
                    }
                    //src[i] = '\0';
                    result = new char[i + 1];
                    for (int index = 0; index < i; index++) result[index] = src[index];
                }
            }
            return new string(result);
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
