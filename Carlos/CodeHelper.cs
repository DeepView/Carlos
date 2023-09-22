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
            (right, left) = (left, right);
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
        /// <summary>
        /// 执行参数executedCode包含的代码并检查是否会抛出异常。
        /// </summary>
        /// <param name="executedCode">需要执行的代码。</param>
        /// <param name="throwedException">用于保存已经抛出的异常，但这里只能捕获到最先抛出的异常。</param>
        /// <returns>如果该操作不会抛出任何异常，则将会返回false，但是在抛出任何异常的情况下，该方法都会抛出true。</returns>
        public static bool IsThrowedException(Action executedCode, ref Exception throwedException)
        {
            int returnValue = 0;
            Func<int> executed = new Func<int>(delegate
            {
                executedCode.Invoke();
                return returnValue;
            });
            return IsThrowedException(executed, ref returnValue, ref throwedException);
        }
        /// <summary>
        /// 执行参数executedCode包含存在返回值的代码并检查是否会抛出异常。
        /// </summary>
        /// <typeparam name="T">用于表示参数executedCode所封装委托的返回值类型。</typeparam>
        /// <param name="executedCode">需要执行的代码。</param>
        /// <param name="returnValue">参数executedCode封装的委托的返回值。</param>
        /// <param name="throwedException">用于保存已经抛出的异常，但这里只能捕获到最先抛出的异常。</param>
        /// <returns>如果该操作不会抛出任何异常，则将会返回false，但是在抛出任何异常的情况下，该方法都会抛出true。</returns>
        public static bool IsThrowedException<T>(Func<T> executedCode, ref T returnValue, ref Exception throwedException)
        {
            bool result = false;
            try
            {
                returnValue = executedCode.Invoke();
            }
            catch (Exception exception)
            {
                if (exception != null)
                {
                    throwedException = exception.InnerException;
                    result = true;
                }
            }
            return result;
        }
#if WIN32
        /// <summary>
        /// 执行参数executedUnmanagedCode包含的非托管代码并检查是否写入了新的Win32错误代码。
        /// </summary>
        /// <param name="executedUnmanagedCode">需要执行的非托管代码。</param>
        /// <param name="win32ErrorCode">用于已写入的新的Win32错误代码，如果该方法没有写入新的错误代码，则该参数将会保存操作系统中上一个Win32错误代码。</param>
        /// <returns>如果该操作不会写入新的Win32错误代码，则将会返回false，但是在写入任何Win32错误代码的情况下，该方法都会抛出true。</returns>
        public static bool IsWritedWin32ErrorCode(Action executedUnmanagedCode, out long win32ErrorCode)
        {
            int returnValue = 0;
            Func<int> executed = new Func<int>(delegate
            {
                executedUnmanagedCode.Invoke();
                return returnValue;
            });
            return IsWritedWin32ErrorCode(executed, out returnValue, out win32ErrorCode);
        }
        /// <summary>
        /// 执行参数executedUnmanagedCode包含存在返回值的非托管代码并检查是否写入了新的Win32错误代码。
        /// </summary>
        /// <typeparam name="T">用于表示参数executedUnmanagedCode所封装委托的返回值类型。</typeparam>
        /// <param name="executedUnmanagedCode">需要执行的非托管代码。</param>
        /// <param name="returnValue">参数executedUnmanagedCode封装的委托的返回值。</param>
        /// <param name="win32ErrorCode">用于已写入的新的Win32错误代码，如果该方法没有写入新的错误代码，则该参数将会保存操作系统中上一个Win32错误代码。</param>
        /// <returns>如果该操作不会写入新的Win32错误代码，则将会返回false，但是在写入任何Win32错误代码的情况下，该方法都会抛出true。</returns>
        public static bool IsWritedWin32ErrorCode<T>(Func<T> executedUnmanagedCode, out T returnValue, out long win32ErrorCode)
        {
            bool result = false;
            long lastWin32ErrorCode = Win32ApiHelper.GetLastWin32ApiError();
            returnValue = executedUnmanagedCode.Invoke();
            win32ErrorCode = Win32ApiHelper.GetLastWin32ApiError();
            if (lastWin32ErrorCode != win32ErrorCode) result = true;
            else win32ErrorCode = lastWin32ErrorCode;
            return result;
        }
#endif
    }
}
