using System;
namespace Carlos
{
   /// <summary>
   /// 一些常用的非常基础的代码或者辅助性工具集合。
   /// </summary>
   public sealed class CodeHelper
   {
      /// <summary>
      /// 交换两个字段的值，这个值的数据类型包括但并不限制于值类型和引用类型。
      /// </summary>
      /// <typeparam name="T">进行值交换的字段的数据类型。</typeparam>
      /// <param name="left">第一个字段。</param>
      /// <param name="right">第二个字段。</param>
      public static void Swap<T>(ref T left, ref T right)
      {
         T temp = left;
         left = right;
         right = temp;
      }
      /// <summary>
      /// 利用位运算安全的交换两个Int64字段的值。
      /// </summary>
      /// <param name="left">第一个字段。</param>
      /// <param name="right">第二个字段。</param>
      public static void SecurityInt64Swap(ref long left, ref long right)
      {
         left ^= right;
         right ^= left;
         left ^= right;
      }
      /// <summary>
      /// 利用位运算安全的交换两个Int32字段的值。
      /// </summary>
      /// <param name="left">第一个字段。</param>
      /// <param name="right">第二个字段。</param>
      public static void SecurityInt32Swap(ref int left, ref int right)
      {
         left ^= right;
         right ^= left;
         left ^= right;
      }
      /// <summary>
      /// 获取需要被执行的代码片段的执行时间长度，但是这个时间长度可能会有1~5毫秒的偏差，在早期版本硬件的计算机中，这个偏差值可能会更高。
      /// </summary>
      /// <param name="codeFragment">用来计算执行时长的代码片段。</param>
      /// <returns>该操作将会返回一个代码片段的执行时长，单位为毫秒（millisecond，ms）。</returns>
      public static long ExecDuration(Action codeFragment)
      {
         long beforeTicks = DateTime.Now.Ticks, afterTicks;
         codeFragment.Invoke();
         afterTicks = DateTime.Now.Ticks;
         long hundredNanoseconds = afterTicks - beforeTicks;
         return hundredNanoseconds / 10000;
      }
      /// <summary>
      /// 获取两个不同时间之间的时间差。
      /// </summary>
      /// <param name="now">当前的时间，或者是被测量或者计算的时间。</param>
      /// <param name="other">另一个时间，比如说是过去的某个时间，或者说是未来的某个时间。</param>
      /// <returns>该操作将会返回一个TimeSpan结构，用来表示两个时间之间的时间差或者时间间隔。</returns>
      public static TimeSpan TimeDiff(DateTime now, DateTime other) => now - other;
   }
}
