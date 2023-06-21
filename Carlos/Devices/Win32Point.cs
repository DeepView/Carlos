using System.Runtime.InteropServices;
namespace Carlos.Devices
{
    /// <summary>
    /// 用于方便作为Win32Api的POINT类型传递的坐标点结构。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct Win32Point
    {
        /// <summary>
        /// 表示一个X坐标。
        /// </summary>
        public int X;
        /// <summary>
        /// 表示一个Y坐标。
        /// </summary>
        public int Y;
        /// <summary>
        /// 构造函数，用一个X和Y坐标创建一个Win32Point结构。
        /// </summary>
        /// <param name="x">一个X坐标。</param>
        /// <param name="y">一个Y坐标。</param>
        public Win32Point(int x,int y)
        {
            X = x;
            Y = y;
        }
    }
}
