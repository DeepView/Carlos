namespace Carlos.Enumerations
{
    /// <summary>
    /// 日期显示方式的枚举。
    /// </summary>
    public enum DateDisplayCategory : int
    {
        /// <summary>
        /// 序列表示。
        /// </summary>
        [EnumerationDescription("纯序列表示")]
        OnlySerial = 0x0000,
        /// <summary>
        /// 斜线符号分割。
        /// </summary>
        [EnumerationDescription("斜线符号分割")]
        SolidusSegmentation = 0x0001,
        /// <summary>
        /// 短横线符号分割。
        /// </summary>
        [EnumerationDescription("短横线符号分割")]
        DashedSegmentation = 0x0002,
        /// <summary>
        /// 句点符号分割。
        /// </summary>
        [EnumerationDescription("句点符号分割")]
        PointSegmentation = 0x0003,
        /// <summary>
        /// 波浪号分割。
        /// </summary>
        [EnumerationDescription("波浪号分割")]
        WavyLineSegmentation = 0x0004,
        /// <summary>
        /// 中文单位分割。
        /// </summary>
        [EnumerationDescription("汉字计量单位分割")]
        ChineseSegmentation = 0x0006
    }

}
