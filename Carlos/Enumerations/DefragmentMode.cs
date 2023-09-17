namespace Carlos.Enumerations
{
    /// <summary>
    /// 固定尺寸二维表的元素反碎片化模式。
    /// </summary>
    public enum DefragmentMode : int
    {
        /// <summary>
        /// 序列化整理。
        /// </summary>
        [EnumerationDescription("序列化整理")]
        Serial = 0x0000,
        /// <summary>
        /// 各行单独整理。
        /// </summary>
        [EnumerationDescription("各行单独整理")]
        Row = 0x0001,
        /// <summary>
        /// 各列单独整理。
        /// </summary>
        [EnumerationDescription("各列单独整理")]
        Col = 0x0002
    }
}
