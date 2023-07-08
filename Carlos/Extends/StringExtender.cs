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
        public static string Left(this string source, int length) => source.Cut(0, length);
        /// <summary>
        /// 剪切指定字符串的右边部分。
        /// </summary>
        /// <param name="source">需要被剪切的字符串。</param>
        /// <param name="length">需要剪切的长度。</param>
        /// <returns>该操作将会返回一个全新的字符串，这个字符串就是剪切操作结束之后的数据。</returns>
        public static string Right(this string source, int length) => source.Cut(source.Length - length, length);
        /// <summary>
        /// 从指定索引开始裁剪指定长度的字符串。
        /// </summary>
        /// <param name="source">需要备裁剪的字符串。</param>
        /// <param name="cutRange">一个Carlos.Extend.Int32Range实例，用于描述需要裁剪的范围。</param>
        /// <returns></returns>
        public static string Cut(this string source, Int32Range cutRange) => source.Cut(cutRange.Lower, cutRange.Upper);
        /// <summary>
        /// 从指定索引开始裁剪指定长度的字符串。
        /// </summary>
        /// <param name="source">需要备裁剪的字符串。</param>
        /// <param name="index">一个起始索引，指定从何处开始裁剪。</param>
        /// <param name="length">指定的裁剪长度。</param>
        /// <returns>该操作将会返回一个字符串，这个字符串是通过裁剪而得到的。</returns>
        /// <remarks>该方法和String.Substring(int, int)方法等效，但是在存在大量字符串裁剪的特殊环境下，这个方法的时间开销更低。虽说在单一的字符串裁剪任务中，这两个方法的时间开销差距非常小，但是在大多数情况下，我们还是建议您优先考虑String.Substring(int, int)。</remarks>
        public static string Cut(this string source, int index, int length) => string.Create(length, source, (dst, source) =>
        {
            var src = source.AsSpan();
            var cutStr = src.Slice(index, length);
            for (int i = 0; i < length; i++) dst[i] = cutStr[i];
        });
        /// <summary>
        /// 用字符指针的方式比较两个字符串是否相同。
        /// </summary>
        /// <param name="left">第一个字符串。</param>
        /// <param name="right">第二个字符串。</param>
        /// <returns>该操作将会返回一个Boolean数据，该数据说明了两个字符串是否相同，如果相同则返回true，否则返回false。</returns>
        /// <remarks>该方法由于使用了指针这一特性，所以包含了一部分相对于CLR环境而言不安全的代码。另外，如果需要比较中文或者其他非英语和阿拉伯数字字符的字符串，则不建议使用该方法，因为该方法中文等字符串可能会造成额外的资源和时间开销。</remarks>
        public static bool IsEquals(this string left, string right)
        {
            bool bRet = true;
            int nC1 = NUM_ZERO, nC2 = NUM_ZERO, nLen = NUM_ZERO;
            int i = NUM_ZERO;
            if (left.Length != right.Length) return false;
            nC1 = ((left.Length % 4) != NUM_ZERO) ? (4 - left.Length % 4) : NUM_ZERO;
            nC2 = ((right.Length % 4) != NUM_ZERO) ? (4 - left.Length % 4) : NUM_ZERO;
            nLen = (nC1 > nC2) ? nC1 : nC2;
            for (i = NUM_ZERO; i < nLen; i++)
            {
                if (i < nC1) left += " ";
                if (i < nC2) right += " ";
            }
            unsafe
            {
                fixed (char* psStr1 = left) fixed (char* psStr2 = right)
                {
                    char* psTemp1 = psStr1;
                    char* psTemp2 = psStr2;
                    while (i < left.Length)
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
        /// 使用位运算比较两个字符串是否相同。
        /// </summary>
        /// <param name="left">第一个字符串。</param>
        /// <param name="right">第二个字符串。</param>
        /// <returns>该操作将会返回一个Boolean数据，该数据说明了两个字符串是否相同，如果相同则返回true，否则返回false。</returns>
        public static bool IsEqualsXor(this string left,string right)
        {
            bool equal = true;
            char[] leftChars = left.ToCharArray();
            char[] rightChars = right.ToCharArray();
            if( leftChars.Length != rightChars.Length ) equal = false;
            else
            {
                for(int i = 0;i< leftChars.Length; i++)
                {
                    int xor = leftChars[i] ^ rightChars[i];
                    if( xor != 0 )
                    {
                        equal = false;
                        break;
                    }
                }
            }
            return equal;
        }
        /// <summary>
        /// 通过指针操作的方式来反转字符串中每一个字符的顺序，在绝大多数情况下，相对于传统的字符串反转操作，这个方法将会节省大约10%的时间开销。
        /// </summary>
        /// <param name="source">需要被反转顺序的字符串。</param>
        /// <returns>该操作将会返回一个全新的String实例，这个实例包含了source参数指定字符串的反转字符串。</returns>
        /// <remarks>
        /// <para>由于String属于不可变对象，因此，所有的safe操作对于CLR而言都是安全的。而通过指针强行修改String对象的操作，属于unsafe行为，这对于CLR而言则是属于未定义的行为（Undefined behavior）。虽说在很多情况下，这种行为可以达到用户预期的目的，但是这些操作依旧存在不少隐患，这些隐患会导致某些场合下，导致代码无法达到用户的预期，比如说下面的这段代码：</para>
        /// <code language="cs">
        /// public static void Main(string[] args)
        /// {
        ///     string a = "devil";
        ///     string b = "devil";
        ///     a.UnsafeReverse();
        ///     Console.WriteLine(b);
        /// }
        /// </code>
        /// <para>如果不通过unsafe的方式执行字符串反转操作，上面的代码可以确定会输出<c>devil</c>。但事实是，上面的代码会输出这样一行结果：<c>lived</c>。</para>
        /// <para>这是由于CLR会针对不可变设计的对象在性能开销上做了一定程度的性能改善与优化，因此看似两个毫无关联且值相同的String对象，一旦对某一个String对象执行了unsafe操作，将会影响到另外一个String对象，因为这两个毫无关联的String对象，其引用地址在托管堆中可能指向的都是同一个字符串。</para>
        /// <para>因此，如果您确定某个字符串在它的生命周期内，整个堆栈中只有唯一的一个引用指向了这个字符串，那么使用这个方法将会避免这种不安全因素。虽说避免了这样的不安全因素，但是其他的不安全因素可能依旧存在，所以在使用这个方法的时候，请务必谨慎小心。</para>
        /// </remarks>
        public static string UnsafeReverse(this string source)
        {
            char temp;
            unsafe
            {
                fixed (char* ptrStr = source)
                {
                    for (int i = 0; i < source.Length / 2; i++)
                    {
                        temp = ptrStr[i];
                        ptrStr[i] = ptrStr[source.Length - i - 1];
                        ptrStr[source.Length - i - 1] = temp;
                    }
                }
            }
            return source;
        }
        /// <summary>
        /// 通过更加安全与高效的方式来反转字符串，是String.UnsafeReverse()的安全版本。
        /// </summary>
        /// <param name="source">需要被反转顺序的字符串</param>
        /// <returns>该操作将会返回一个全新的String实例，这个实例包含了source参数指定字符串的反转字符串。</returns>
        public static string SafeReverse(this string source) => string.Create(source.Length, source, (dst, source) =>
        {
            var src = source.AsSpan();
            for (int i = 0; i < src.Length; i++) dst[i] = src[^(i + 1)];
        });
        /// <summary>
        /// 拼接两个字符串，该操作是基于操作Span的方式实现。
        /// </summary>
        /// <param name="source">原始字符串。</param>
        /// <param name="spliced">需要拼接到原始字符串后面的字符串。</param>
        /// <returns>该操作将会返回一个新的字符串，这个字符串正是拼接之后的字符串。</returns>
        public static string Glue(this string source, string spliced) => new StringBuilder(source).Append(spliced.AsSpan()).ToString();
        /// <summary>
        /// 拼接两个字符串，该操作是基于操作Span的方式实现。
        /// </summary>
        /// <param name="source">原始字符串。</param>
        /// <param name="splicedArray">需要拼接到原始字符串后面的字符串数组。</param>
        /// <returns>该操作将会返回一个新的字符串，这个字符串正是拼接之后的字符串。</returns>
        public static string Glue(this string source, string[] splicedArray) => Glue(source, splicedArray, string.Empty);
        /// <summary>
        /// 拼接两个字符串，该操作是基于操作Span的方式实现。
        /// </summary>
        /// <param name="source">原始字符串。</param>
        /// <param name="splicedArray">需要拼接到原始字符串后面的字符串数组。</param>
        /// <param name="delimiter">每个需要拼接的字符串与上一个字符串的分隔符。</param>
        /// <returns>该操作将会返回一个新的字符串，这个字符串正是拼接之后的字符串。</returns>
        public static string Glue(this string source, string[] splicedArray, string delimiter)
        {
            ReadOnlySpan<char> splicedSpan, delimiterSpan = delimiter.AsSpan();
            StringBuilder builder = new StringBuilder(source);
            for (int i = 0; i < splicedArray.Length; i++)
            {
                splicedSpan = splicedArray[i].AsSpan();
                builder.Append(delimiterSpan);
                builder.Append(splicedSpan);
            }
            return builder.ToString();
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
