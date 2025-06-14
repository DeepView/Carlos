using System;
using System.Drawing;
using Carlos.Enumerations;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
namespace Carlos.Devices
{
    /// <summary>
    /// 显示器帮助类，用于实现一些常用的显示器控制操作以及硬件参数提取。
    /// </summary>
    public class Screen
    {
        private const short SYSTEM_DEFAULT_BRIGHTNESS_WITH_GAMMA = 127;//操作系统默认的基于Gamma的亮度值。
        private const int SM_DIGITIZER = 94;//API常量，用于指定设备支持的数字化器输入类型的位掩码的常量。
        private const int SM_MAXIMUMTOUCHES = 95;//API常量，用于获取当前设备支持的最大触摸点的数量的常量。
        private const int DESKTOPVERTRES = 117;//API常量，适用于GetDeviceCaps函数的常量DESKTOPVERTRES。
        private const int DESKTOPHORZRES = 118;//API常量，适用于GetDeviceCaps函数的常量DESKTOPHORZRES。
        private const int BITSPIXEL = 12;//API常量，适用于GetDeviceCaps函数的常量BITSPIXEL。
        /// <summary>
        /// 设置Direct彩色显示器上的伽玛斜坡, 其驱动程序支持硬件中可下载的伽玛坡道。
        /// </summary>
        /// <param name="hdc">指定Direct彩色显示器的设备上下文。</param>
        /// <param name="ramp">一个缓冲区包含Gamma坡道指针。伽马斜坡是在三个256个单词元素数组中指定的，其中包含帧缓冲区中的RGB值和数字模拟转换器（DAC）值之间的映射。阵列的顺序是红，绿，蓝。</param>
        /// <returns>如果这个操作无异常发生，并且达到了用户需要的效果，则会返回true，否则将会返回false。</returns>
        [DllImport("gdi32.dll")]
        public unsafe static extern bool SetDeviceGammaRamp(int hdc, void* ramp);
        /// <summary>
        /// 获取具有支持硬件中可下载伽马斜坡的驱动程序的Direct彩色显示器上的伽玛坡道。
        /// </summary>
        /// <param name="hdc">指定Direct彩色显示器的设备上下文。</param>
        /// <param name="ramp">一个缓冲区包含Gamma坡道指针。伽马斜坡是在三个256个单词元素数组中指定的，其中包含帧缓冲区中的RGB值和数字模拟转换器（DAC）值之间的映射。阵列的顺序是红，绿，蓝。</param>
        /// <returns>如果这个操作无异常发生，则会返回true，否则将会返回false。</returns>
        [DllImport("gdi32.dll")]
        public unsafe static extern bool GetDeviceGammaRamp(int hdc, void* ramp);
        /// <summary>
        /// 设置Direct彩色显示器上的伽玛斜坡, 其驱动程序支持硬件中可下载的伽玛坡道。
        /// </summary>
        /// <param name="hdc">指定Direct彩色显示器的设备上下文。</param>
        /// <param name="ramp">Gamma坡道值结构。伽马斜坡是在三个256个单词元素数组中指定的，其中包含帧缓冲区中的RGB值和数字模拟转换器（DAC）值之间的映射。阵列的顺序是红，绿，蓝。</param>
        /// <returns>如果这个操作无异常发生，并且达到了用户需要的效果，则会返回true，否则将会返回false。</returns>
        [DllImport("gdi32.dll")]
        public static extern int GetDeviceGammaRamp(IntPtr hdc, ref GammaSlope ramp);
        /// <summary>
        /// 获取具有支持硬件中可下载伽马斜坡的驱动程序的Direct彩色显示器上的伽玛坡道。
        /// </summary>
        /// <param name="hdc">指定Direct彩色显示器的设备上下文。</param>
        /// <param name="ramp">Gamma坡道值结构。伽马斜坡是在三个256个单词元素数组中指定的，其中包含帧缓冲区中的RGB值和数字模拟转换器（DAC）值之间的映射。阵列的顺序是红，绿，蓝。</param>
        /// <returns>如果这个操作无异常发生，则会返回true，否则将会返回false。</returns>
        [DllImport("gdi32.dll")]
        public static extern int SetDeviceGammaRamp(IntPtr hdc, ref GammaSlope ramp);
        /// <summary>
        /// 该函数把缺省显示设备的设置改变为由devMode设定的图形模式，要改变一个特定显示设备的设置，请使用ChangeDisplaySettingsEx函数。
        /// </summary>
        /// <param name="devMode">指向一个描述转变图表的SDevicesInitAndPrinterEnvironment的指针。</param>
        /// <param name="flags">表明了图形模式如何改变。</param>
        /// <returns>该函数会返回一个整数，这个整数表示了该操作执行之后的状态。</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int ChangeDisplaySettings([In] ref DevicesMode devMode, int flags);
        /// <summary>
        /// 获取显示设备的一个图形模式设备，通过对该函数一系列的调用可以得到显示设备所有的图形模式信息。
        /// </summary>
        /// <param name="deviceName">指向一个以null的结尾的字符串，该字符串指定了显示设备。</param>
        /// <param name="modeNum">表明要检索的信息类型。</param>
        /// <param name="devMode">DevicesInitAndPrinterEnvironment结构的指针，该结构存储指定图形模式的信息，在调用EnumDisplaySettings之前，设置dmSize为sizeof(DevicesInitAndPrinterEnvironment)，并且以字节为单位，设置DriveExtra元素为接收专用驱动数据可用的附加空间。</param>
        /// <returns>如果这个操作无异常发生，则会返回true，否则将会返回false。</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DevicesMode devMode);
        /// <summary>
        /// 该函数用来访问使用设备描述表的设备数据，应用程序指定相应设备描述表的句柄和说明该函数访问数据类型的索引来访问这些数据。
        /// </summary>
        /// <param name="hdc">设备上下文环境的句柄。</param>
        /// <param name="index">
        /// 指定返回项，该参数取值可以参考
        /// <see href="https://learn.microsoft.com/zh-cn/windows/win32/api/wingdi/nf-wingdi-getdevicecaps">GetDeviceCaps函数</see>
        /// 的技术文档。
        /// 目前该参数可用<see cref="DeviceCapsIndex">DeviceCapsIndex枚举</see>进行参数传递。
        /// </param>
        /// <returns>返回值指定所需项的值，当索引为BITSPIXEL且设备有15bpp或16bpp时，返回值为16。</returns>
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern int GetDeviceCaps(IntPtr hdc, int index);
        /// <summary>
        /// 该函数返回桌面窗口的句柄，桌面窗口覆盖整个屏幕，是一个要在其上绘制所有的图标和其他窗口的区域。
        /// </summary>
        /// <returns>函数返回桌面窗口的句柄。</returns>
        [DllImport("user32.dll")]
        public extern static IntPtr GetDesktopWindow();
        /// <summary>
        /// 检索显示元素和系统配置设置的各种系统度量。
        /// </summary>
        /// <param name="index">要检索的系统度量或配置设置。</param>
        /// <returns>如果函数操作成功，返回值是请求的系统度量或配置设置。如果函数操作失败，返回值为0。GetLastError不会为该函数提供扩展错误信息。</returns>
        /// <remarks>
        /// 如果需要获取该函数的更多信息，可以参考
        /// <see href="https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-getsystemmetrics">GetSystemMetrics函数</see>
        /// 的技术文档。
        /// </remarks>
        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int index);
        /// <summary>
        /// 检索指定坐标点的像素的RGB颜色值。
        /// </summary>
        /// <param name="hdc">设备环境句柄。</param>
        /// <param name="x">指定要检查的像素点的逻辑X轴坐标。</param>
        /// <param name="y">指定要检查的像素点的逻辑Y轴坐标。</param>
        /// <returns>返回值是该象像点的RGB值。如果指定的像素点在当前剪辑区之外，那么返回值是CLR_INVALID。</returns>
        [DllImport("gdi32.dll")]
        private static extern uint GetPixel(IntPtr hdc, int x, int y);
        /// <summary>
        /// 为一个设备创建设备上下文环境。
        /// </summary>
        /// <param name="driverName">指向一个以Null结尾的字符串的指针，该字符串为显示驱动指定DISPLAY或者指定一个打印驱动程序名，通常为WINSPOOL。</param>
        /// <param name="deviceName">指向一个以null结尾的字符串的指针，该字符串指定了正在使用的特定输出设备的名字，它不是打印机模式名。该参数必须被使用。</param>
        /// <param name="output">该参数在32位应用中被忽略；并置为Null，它主要是为了提供与16位应用程序兼容，更多的信息参见下面的注释部分。</param>
        /// <param name="initData">指向包含设备驱动程序的设备指定初始化数据的DEVMODE结构的指针，DocumentProperties函数检索指定设备获取已填充的结构，</param>
        /// <returns>成功，返回值是特定设备的设备上下文环境的句柄；失败，返回值为Null。</returns>
        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateDC(string driverName, string deviceName, string output, IntPtr initData);
        /// <summary>
        /// 删除指定的设备上下文环境
        /// </summary>
        /// <param name="dc">设备上下文环境的句柄。</param>
        /// <returns>成功，返回非零值；失败，返回零。</returns>
        [DllImport("gdi32.dll")]
        private static extern bool DeleteDC(IntPtr dc);
        /// <summary>
        /// 该函数用来访问使用设备描述表的设备数据，该操作将会使用Zero句柄和说明该函数访问数据类型的索引来访问这些数据。
        /// </summary>
        /// <param name="deviceCapsIndex">指定返回项，或者是需要获取的数据。</param>
        /// <returns>该操作将会返回deviceCapsIndex参数指定所需项的值。</returns>
        public static int GetDeviceCapabilities(DeviceCapsIndex deviceCapsIndex) => GetDeviceCapabilities(IntPtr.Zero, deviceCapsIndex);
        /// <summary>
        /// 该函数用来访问使用设备描述表的设备数据，该操作将会使用指定的设备描述表句柄和说明该函数访问数据类型的索引来访问这些数据。
        /// </summary>
        /// <param name="hdc">指定的设备描述表句柄。</param>
        /// <param name="deviceCapsIndex">指定返回项，或者是需要获取的数据。</param>
        /// <returns>该操作将会返回deviceCapsIndex参数指定所需项的值。</returns>
        public static int GetDeviceCapabilities(IntPtr hdc, DeviceCapsIndex deviceCapsIndex) => GetDeviceCaps(hdc, (int)deviceCapsIndex);
        /// <summary>
        /// 设置当前显示器的分辨率。
        /// </summary>
        /// <param name="width">指定分辨率的宽度。</param>
        /// <param name="height">指定分辨率的高度。</param>
        /// <returns>该操作会返回一个Win32Api错误代码，如果这个错误代码为非0，则表示操作失败，否则表示操作成功。</returns>
        public static bool SetResolving(int width, int height) => SetResolution(width, height, GetRefreshRate(), GetBitsPerPixel(), out _);
        /// <summary>
        /// 设置当前图形监视器的分辨率，并重新指定屏幕刷新率和像素色彩深度。
        /// </summary>
        /// <param name="width">指定分辨率的宽度。</param>
        /// <param name="height">指定分辨率的高度。</param>
        /// <param name="refreshRate">指定的屏幕刷新率。</param>
        /// <param name="bitsPerPixel">指定的像素色彩位数。</param>
        /// <param name="win32ErrorCode">一个Win32Api错误代码，如果这个错误代码为非0，则表示操作失败，否则表示操作成功。</param>
        /// <returns>该操作会返回一个Boolean类型，如果该返回值为true则表示操作成功，否则表示操作失败。如果操作失败，可以访问<see cref="Win32ApiHelper.FormatErrorCode(long)">FormatErrorCode</see>方法来获取更多信息。</returns>
        public static bool SetResolution(int width, int height, int refreshRate, int bitsPerPixel, out long win32ErrorCode)
        {
            DevicesMode devM = new DevicesMode
            {
                Size = (short)Marshal.SizeOf(typeof(DevicesMode))
            };
            EnumDisplaySettings(null, 0, ref devM);
            devM.PelsHeight = height;
            devM.PelsWidth = width;
            devM.DisplayFrequency = refreshRate;
            devM.BitsPerPel = bitsPerPixel;
            win32ErrorCode = ChangeDisplaySettings(ref devM, 0);
            return win32ErrorCode == 0;
        }
        /// <summary>
        /// 获取当前显示器的分辨率。
        /// </summary>
        /// <returns>该操作将会返回一个Size类型的数据，其中将会包含显示器分辨率的值Width和Height。</returns>
        public static Size GetResolution()
        {
            IntPtr hdc = Win32ApiHelper.GetDeviceContext(IntPtr.Zero);
            int width = GetDeviceCapabilities(hdc, DeviceCapsIndex.HorizontalPixel);
            int height = GetDeviceCapabilities(hdc, DeviceCapsIndex.VerticalPixel);
            Win32ApiHelper.ReleaseDeviceContext(IntPtr.Zero, hdc);
            return new Size(width, height);
        }
        /// <summary>
        /// 获取当前显示屏每个像素的位数。
        /// </summary>
        /// <returns>该操作将会返回一个int数据，用于表述当前的像素色彩位数。</returns>
        public static int GetBitsPerPixel()
        {
            IntPtr hdc = Win32ApiHelper.GetDeviceContext(IntPtr.Zero);
            int bitsPerPixel = GetDeviceCapabilities(hdc, DeviceCapsIndex.BitPerPixel);
            Win32ApiHelper.ReleaseDeviceContext(IntPtr.Zero, hdc);
            return bitsPerPixel;
        }
        /// <summary>
        /// 获取当前显示屏已应用的刷新率。
        /// </summary>
        /// <returns>该操作将会返回一个int数据，用于表述当前显示器正在使用的刷新率。</returns>
        public static int GetRefreshRate()
        {
            IntPtr hdc = Win32ApiHelper.GetDeviceContext(IntPtr.Zero);
            int refreshRate = GetDeviceCapabilities(hdc, DeviceCapsIndex.VerticalRefresh);
            Win32ApiHelper.ReleaseDeviceContext(IntPtr.Zero, hdc);
            return refreshRate;
        }
        /// <summary>
        /// 获取当前实例所对应的主显示器是否是触摸屏。
        /// </summary>
        /// <param name="numberOfTouchPoint">该参数会传出一个硬件能支持的触摸点的数量，如果硬件不支持触摸屏，则该参数值为0。</param>
        /// <returns>该操作将会返回一个Boolean值，这个值用于表示当前设备是否支持触摸屏操作，如果这个值为true，则表示该显示设备支持触摸操作，否则表示该设备不支持触摸操作，或者触摸设备被操作系统禁用或卸载，或者触摸设备已损坏或未检测到。</returns>
        public static bool IsTouchScreen(out int numberOfTouchPoint)
        {
            bool isTouch = true;
            byte digitizerStatus = (byte)GetSystemMetrics(SM_DIGITIZER);
            if ((digitizerStatus & (0x80 + 0x40)) == 0)
            {
                isTouch = false;
                numberOfTouchPoint = 0;
            }
            else numberOfTouchPoint = GetSystemMetrics(SM_MAXIMUMTOUCHES);
            return isTouch;
        }
        /// <summary>
        /// 从Gamma值的基础上设置当前显示设备的亮度。
        /// </summary>
        /// <param name="gamma">需要调整的Gamma值，有效范围是[0, 255]。</param>
        /// <returns>该操作如果达到了用户需要的效果，则会返回true，否则会返回false。</returns>
        public static unsafe bool SetBrightnessWithGamma(short gamma)
        {
            if (gamma > 255) gamma = 255;
            if (gamma < 0) gamma = 0;
            short* gArray = stackalloc short[3 * 256];
            short* idx = gArray;
            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 256; i++)
                {
                    int arrayVal = i * (gamma + 128);
                    if (arrayVal > ushort.MaxValue) arrayVal = ushort.MaxValue;
                    *idx = (short)arrayVal;
                    idx++;
                }
            }
            int hdc = Graphics.FromHwnd(IntPtr.Zero).GetHdc().ToInt32();
            return SetDeviceGammaRamp(hdc, gArray);
        }
        /// <summary>
        /// 重置当前显示设备的亮度，该操作是基于SetBrightnessWithGamma方法实现的。
        /// </summary>
        public static void ResetBrightnessWithGamma() => SetBrightnessWithGamma(SYSTEM_DEFAULT_BRIGHTNESS_WITH_GAMMA);
        /// <summary>
        /// 获取指定像素的颜色。
        /// </summary>
        /// <param name="pixelPosition">指定像素在屏幕上的位置。</param>
        /// <returns>该操作将会返回一个RGB结构的Color值，用于表示参数pixelPosition所指定像素的颜色。</returns>
        public static Color GetPixelColor(Point pixelPosition)
        {
            IntPtr displayDC = CreateDC("DISPLAY", null, null, IntPtr.Zero);
            uint colorref = GetPixel(displayDC, pixelPosition.X, pixelPosition.Y);
            DeleteDC(displayDC);
            byte r = (byte)colorref;
            byte g = ((byte)(((short)(colorref)) >> 8));
            byte b = ((byte)((colorref) >> 16));
            return Color.FromArgb(r, g, b);
        }
        /// <summary>
        /// 截取屏幕快照（全屏截取）。
        /// </summary>
        /// <returns>该操作将会返回一个Bitmap对象，用于将截取的屏幕快照存储到该对象之中，从而方便后续处理。</returns>
        public static Bitmap Capture()
        {
            Size res = GetResolution();
            Rectangle rectangle = new Rectangle
            {
                Location = new Point(0, 0),
                Width = res.Width,
                Height = res.Height
            };
            return Capture(rectangle);
        }
        /// <summary>
        /// 截取指定区域的屏幕快照（局部截取）。
        /// </summary>
        /// <param name="captureArea">需要被截取快照的屏幕区域。</param>
        /// <returns>该操作将会返回一个Bitmap对象，用于将截取的屏幕快照存储到该对象之中，从而方便后续处理。</returns>
        public static Bitmap Capture(Rectangle captureArea)
        {
            Bitmap screenshot = new Bitmap(
                captureArea.Width,
                captureArea.Height,
                PixelFormat.Format32bppArgb
            );
            Graphics g = Graphics.FromImage(screenshot);
            g.CopyFromScreen(
                captureArea.Location,
                Point.Empty,
                captureArea.Size,
                CopyPixelOperation.SourceCopy
            );
            return screenshot;
        }
    }
}
