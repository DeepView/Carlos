namespace Carlos.Enumerations
{
    /// <summary>
    /// 适用于wingdi.h头文件或者gdi32.dll库文件的一些Win32Api的枚举，该枚举在GetDeviceCaps函数中用处较为广泛。
    /// </summary>
    /// <remarks>
    /// 该枚举对应了GetDeviceCaps系统函数中需要使用的index宏定义，关于这个宏定义，请参考C++源文件wingdi.h，或者参考页面
    /// <see href="https://learn.microsoft.com/zh-cn/windows/win32/api/wingdi/nf-wingdi-getdevicecaps">GetDeviceCaps 函数 (wingdi.h)</see>
    /// </remarks>
    public enum DeviceCapsIndex : int
    {
        /// <summary>
        /// 设备驱动程序版本。
        /// </summary>
        DriverVersion = 0,
        /// <summary>
        /// 设备的分类。
        /// </summary>
        Technology = 2,
        /// <summary>
        /// 物理屏幕的宽度（单位：毫米）。
        /// </summary>
        HorizontalSize = 4,
        /// <summary>
        /// 物理屏幕的高度（单位：毫米）。
        /// </summary>
        VerticalSize = 6,
        /// <summary>
        /// 物理屏幕的像素高度。
        /// </summary>
        HorizontalPixel = 8,
        /// <summary>
        /// 物理屏幕的像素宽度。
        /// </summary>
        VerticalPixel = 10,
        /// <summary>
        /// 每个像素的位数。
        /// </summary>
        BitPerPixel = 12,
        /// <summary>
        /// 颜色平面数。
        /// </summary>
        Planes = 14,
        /// <summary>
        /// 特定于设备的画笔数。
        /// </summary>
        NumberOfBrushes = 16,
        /// <summary>
        /// 设备指定笔数。
        /// </summary>
        NumberOfPens = 18,
        /// <summary>
        /// 设备具有的标记数。
        /// </summary>
        NumberOfMarkets = 20,
        /// <summary>
        /// 设备具有的字体数。
        /// </summary>
        NumberOfFonts = 22,
        /// <summary>
        /// 设备颜色表中的条目数（如果设备的颜色深度不超过每像素 8 位）。
        /// </summary>
        NumberOfColors = 24,
        /// <summary>
        /// 保留参数，设备描述符所需的大小。
        /// </summary>
        SizeForDeviceDescriptor = 26,
        /// <summary>
        /// 指示设备的曲线功能的值。
        /// </summary>
        CurveCapabilities = 28,
        /// <summary>
        /// 指示设备的线路功能的值。
        /// </summary>
        LineCapabilities = 30,
        /// <summary>
        /// 指示设备的多边形功能的值。
        /// </summary>
        PolygonalCapabilities = 32,
        /// <summary>
        /// 指示设备的文本功能的值。
        /// </summary>
        TextCapabilities = 34,
        /// <summary>
        /// 指示设备的剪辑功能的标志。 
        /// </summary>
        ClippingCapabilities = 36,
        /// <summary>
        /// 指示设备的光栅功能的值。
        /// </summary>
        BitbltCapabilities = 38,
        /// <summary>
        /// 用于线条绘制的设备像素的相对宽度，等效于wingdi.h头文件中的ASPECTX。
        /// </summary>
        LddRelativeX = 40,
        /// <summary>
        /// 用于线条绘制的设备像素的相对高度，等效于wingdi.h头文件中的ASPECTY。
        /// </summary>
        LddRelativeY = 42,
        /// <summary>
        /// 用于线条绘制的设备像素的对角宽度，等效于wingdi.h头文件中的ASPECTXY。
        /// </summary>
        LddDiagonalLength = 44,
        /// <summary>
        /// 每个逻辑英寸沿屏幕宽度的像素数。 在具有多个显示监视器的系统中，此值对于所有监视器都是相同的。
        /// </summary>
        LogicalPixelsX = 88,
        /// <summary>
        /// 沿屏幕高度每逻辑英寸的像素数。 在具有多个显示监视器的系统中，此值对于所有监视器都是相同的。
        /// </summary>
        LogicalPixelsY = 90,
        /// <summary>
        /// 系统调色板中的条目数。
        /// </summary>
        SizePalette = 104,
        /// <summary>
        /// 系统调色板中的保留条目数。
        /// </summary>
        ReservedSizePalette = 106,
        /// <summary>
        /// 设备的实际颜色分辨率（以每像素位数为单位）。
        /// </summary>
        ColorResolving = 108,
        /// <summary>
        /// 对于打印设备：物理页面的宽度（以设备单位为单位）。
        /// </summary>
        PhysicalPageWidth = 110,
        /// <summary>
        /// 对于打印设备：物理页面的高度（以设备单位为单位）。
        /// </summary>
        PhysicalPageHeight = 111,
        /// <summary>
        /// 对于打印设备：从物理页面的左边缘到可打印区域的左边缘的距离（以设备单位为单位）。 
        /// </summary>
        PhysicalPageOffsetX = 112,
        /// <summary>
        /// 对于打印设备：从物理页面的上边缘到可打印区域的上边缘的距离（以设备单位为单位）。
        /// </summary>
        PhysicalPageOffsetY = 113,
        /// <summary>
        /// 打印机X轴的缩放系数。
        /// </summary>
        ScalingFactorX = 114,
        /// <summary>
        /// 打印机Y轴的缩放系数。
        /// </summary>
        ScalingFactorY = 115,
        /// <summary>
        /// 对于显示设备：设备的当前垂直刷新率，以每秒周期 (Hz) 。
        /// </summary>
        VerticalRefresh = 116,
        /// <summary>
        /// 可视桌面的以像素为单位的宽度，如果有多个显示器组成桌面，则这个值可能大于单一显示器的宽度。
        /// </summary>
        DesktopWidth = 117,
        /// <summary>
        /// 可视桌面的以像素为单位的高度，如果有多个显示器组成桌面，则这个值可能大于单一显示器的高度。
        /// </summary>
        DesktopHeight = 118,
        /// <summary>
        /// 首选水平绘制对齐方式，表示为像素的倍数，为了获得最佳绘制性能，窗口应水平对齐到此值的倍数。
        /// </summary>
        BitAlignment = 119,
        /// <summary>
        /// 指示设备的着色和混合功能的值。
        /// </summary>
        ShadeBlendCapabilities = 120
    }
}
