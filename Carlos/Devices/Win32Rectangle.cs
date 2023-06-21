using System.Drawing;
using System.Runtime.InteropServices;
namespace Carlos.Devices
{
    /// <summary>
    /// 用于方便作为Win32Api的RECT类型传递的矩形结构。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct Win32Rectangle
    {
        /// <summary>
        /// 表示当前矩形的左边位置。
        /// </summary>
        public int Left;
        /// <summary>
        /// 表示当前矩形的顶部位置。
        /// </summary>
        public int Top;
        /// <summary>
        /// 表示当前矩形的右边位置。
        /// </summary>
        public int Right;
        /// <summary>
        /// 表示当前矩形的底部位置。
        /// </summary>
        public int Bottom;
        /// <summary>
        /// 构造函数，创建一个指定尺寸的矩形。
        /// </summary>
        /// <param name="left">矩形的水平位置，或者是矩形的左边位置。</param>
        /// <param name="top">矩形的垂直位置，或者是矩形的顶部位置。</param>
        /// <param name="right">矩形的右边位置。</param>
        /// <param name="bottom">矩形的底部位置。</param>
        public Win32Rectangle(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
        /// <summary>
        /// 运算符重载，将指定的SWin32ApiRectangle结构实例转换为System.Drawing.Rectangle实例。
        /// </summary>
        /// <param name="rect">用于转换的SWin32ApiRectangle结构实例。</param>
        /// <returns>该操作将会转换一个数学意义上与SWin32ApiRectangle结构实例相同的System.Drawing.Rectangle实例。</returns>
        public static implicit operator Rectangle(Win32Rectangle rect) => Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom);
        /// <summary>
        /// 运算符重载，将指定的System.Drawing.Rectangle实例转换为SWin32ApiRectangle结构实例。
        /// </summary>
        /// <param name="rect">用于转换的System.Drawing.Rectangle实例。</param>
        /// <returns>该操作将会转换一个数学意义上与System.Drawing.Rectangle实例相同的SWin32ApiRectangle结构实例。</returns>
        public static implicit operator Win32Rectangle(Rectangle rect) => new Win32Rectangle(rect.Left, rect.Top, rect.Right, rect.Bottom);
    }
}
