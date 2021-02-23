using System;
using System.Collections.Generic;
using System.Text;
namespace Carlos.Extends
{
   /// <summary>
   /// 字符串操作扩展类。
   /// </summary>
   public sealed class StringExtender
   {
      private const int NUM_ZERO = 0;
      /// <summary>
      /// 剪切指定字符串的左边部分。
      /// </summary>
      /// <param name="source">需要被剪切的字符串。</param>
      /// <param name="length">需要剪切的长度。</param>
      /// <returns>该操作将会返回一个全新的字符串，这个字符串就是剪切操作结束之后的数据。</returns>
      /// <example>
      /// 下面的一段代码展示了这个方法如何使用。
      /// <code>
      /// string src = "HelloWorld";
      /// string s = StringExtender.Left(src, 5);
      /// Console.Write(s);
      /// //Console Output Result:
      /// //Hello
      /// </code>
      /// </example>
      public static string Left(string source, int length) => source.Substring(NUM_ZERO, length);
      /// <summary>
      /// 剪切指定字符串的右边部分。
      /// </summary>
      /// <param name="source">需要被剪切的字符串。</param>
      /// <param name="length">需要剪切的长度。</param>
      /// <returns>该操作将会返回一个全新的字符串，这个字符串就是剪切操作结束之后的数据。</returns>
      public static string Right(string source, int length) => source.Substring(source.Length - length, length);
      /// <summary>
      /// 用字符指针的方式比较两个字符串是否相同。
      /// </summary>
      /// <param name="leftString">第一个字符串。</param>
      /// <param name="rightString">第二个字符串。</param>
      /// <returns>该操作将会返回一个Boolean数据，该数据说明了两个字符串是否相同，如果相同则返回true，否则返回false。</returns>
      /// <remarks>该方法由于使用了指针这一特性，所以包含了一部分相对于CLR环境而言不安全的代码。另外，如果需要比较中文或者其他非英语和阿拉伯数字字符的字符串，则不建议使用该方法，因为该方法中文等字符串可能会造成额外的资源和时间开销。</remarks>
      public static bool IsEquals(string leftString, string rightString)
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
   }
}
