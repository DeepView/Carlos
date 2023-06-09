using System.Runtime.InteropServices;
namespace Carlos.Devices
{
    /// <summary>
    /// 用于调节Gamma通道的坡度结构。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct GammaSlope
    {
        /// <summary>
        /// 红色通道Gamma坡度值。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public ushort[] Red;
        /// <summary>
        /// 绿色通道Gamma坡度值。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public ushort[] Green;
        /// <summary>
        /// 蓝色通道Gamma坡度值。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public ushort[] Blue;
    }
}
