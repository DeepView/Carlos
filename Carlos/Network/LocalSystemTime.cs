using System.Runtime.InteropServices;
namespace Carlos.Network
{
    /// <summary>
    /// 系统时间结构，将文件时间转换为系统时间格式，另外系统时间是以协调世界时（UTC）为基础的。
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct LocalSystemTime
    {
        /// <summary>
        /// 年份。
        /// </summary>
        public ushort Year;
        /// <summary>
        /// 月份。
        /// </summary>
        public ushort Month;
        /// <summary>
        /// 星期，一周的第几天。
        /// </summary>
        public ushort DayOfWeek;
        /// <summary>
        /// 指定月份的某一天。
        /// </summary>
        public ushort Day;
        /// <summary>
        /// 当天的某一个时。
        /// </summary>
        public ushort Hour;
        /// <summary>
        /// 指定时的某一分钟。
        /// </summary>
        public ushort Minute;
        /// <summary>
        /// 指定分钟的某一秒。
        /// </summary>
        public ushort Second;
        /// <summary>
        /// 指定秒数的某一毫秒。
        /// </summary>
        public ushort Miliseconds;
    }
}
