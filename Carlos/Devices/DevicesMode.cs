using System.Runtime.InteropServices;
namespace Carlos.Devices
{
    /// <summary>
    /// 相关设备初始化和打印机环境信息的结构体，对应Win32Api中的DEVMODE结构。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DevicesMode
    {
        /// <summary>
        /// 驱动程序支持的设备名称，这个字符串在设备驱动程序之间是相互不同的。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DeviceName;
        /// <summary>
        /// 初始化数据的版本数字，这个结构就基于这些数据。
        /// </summary>
        public short SpecVersion;
        /// <summary>
        /// 打印机驱动程序开发商分配的打印机驱动程序版本号。
        /// </summary>
        public short DriverVersion;
        /// <summary>
        /// SDevicesInitAndPrinterEnvironment结构的大小，以字节为单位，不包括DriverData（与设备有关）成员。
        /// </summary>
        public short Size;
        /// <summary>
        /// 当前结构后面的私有驱动程序数据的数目，以字节为单位，如果设备驱动程序不使用该设备独有的信息，就把这个成员设为零。
        /// </summary>
        public short DriverExtra;
        /// <summary>
        /// 指定了SDevicesInitAndPrinterEnvironment结构的其余成员中哪些已被初始化，第0位（定义为DM）ORIENTATION）代表dmOrientation，第1位（定义为 DM_PAPERSIZE）代表dmPaperSize等等，打印机驱动出现仅支持那些适合打印技术的成员。
        /// </summary>
        public int Fields;
        /// <summary>
        /// 选择纸的方向，这个成员可以为DMORIENT_PORTRAIT（1）或DMORIENT_ LANDSCAPE（2）。
        /// </summary>
        public short Orientation;
        /// <summary>
        /// 选择将用于打印的纸张大小。
        /// </summary>
        public short PaperSize;
        /// <summary>
        /// 重定义由PaperSize成员指定的纸张长度，可用于自定义纸张大小，也可以用于点阵打印机，这种打印机能打出任意长度的纸张，这些值与这个结构中其他指定物理长度的值都是以0.1毫米为单位的。
        /// </summary>
        public short PaperLength;
        /// <summary>
        /// 重载由PaperSize成员指定的纸张宽度。
        /// </summary>
        public short PaperWidth;
        /// <summary>
        /// 指定了打印输出的缩放因子，实际的页面大小为物理纸张的大小乘以Scale/100。
        /// </summary>
        public short Scale;
        /// <summary>
        ///  如果设备支持多页拷贝，则选择了要打印的拷贝数目。
        /// </summary>
        public short Copies;
        /// <summary>
        /// 保留，必须为0。
        /// </summary>
        public short DefaultSource;
        /// <summary>
        /// 指定了打印机的分辨率。
        /// </summary>
        public short PrintQuality;
        /// <summary>
        /// 对于彩色打印机，在彩色和单色之间切换。
        /// </summary>
        public short Color;
        /// <summary>
        /// 为支持双面打印的打印机选择双面打印方式。
        /// </summary>
        public short Duplex;
        /// <summary>
        /// 指定了打印机在y方向的分辨率，以每英寸的点数为单位，如果打印机对该成员进行了初始化，PrintQuality成员指定了打印机在x方向的分辨率，以每英寸点数为单位。
        /// </summary>
        public short YResolution;
        /// <summary>
        /// 指明如何打印TrueType字体。
        /// </summary>
        public short TTOption;
        /// <summary>
        ///  指定在打印多份拷贝的时候是否使用校对。
        /// </summary>
        public short Collate;
        /// <summary>
        /// 指定了要使用的格式名字，例如Letter或Legal，这些名字的完整集合可以通过Windows的EnumForms函数获得。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string FormName;
        /// <summary>
        /// 用于将结构对齐到DWORD边界，不能使用或引用这个成员。它的名字和用法是保留的，在以后的版本中可能会变化。
        /// </summary>
        public short LogPixels;
        /// <summary>
        /// 指定了显示设备的颜色分辨率，以像素的位数为单位，例如16色使用4位，256色使用8位，而65536色使用16位。
        /// </summary>
        public int BitsPerPel;
        /// <summary>
        /// 可见设备表面的以像素为单位的宽度。
        /// </summary>
        public int PelsWidth;
        /// <summary>
        /// 可见设备表面的以像素为单位的高度。
        /// </summary>
        public int PelsHeight;
        /// <summary>
        /// 指定了设备的显示模式。
        /// </summary>
        public int DisplayFlags;
        /// <summary>
        /// 显示设备的特定模式所使用的以赫兹为单位的频率（每秒的周期数）。
        /// </summary>
        public int DisplayFrequency;
    }
}
