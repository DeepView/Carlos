namespace Carlos.Enumerations
{
    /// <summary>
    /// 数学四则运算符枚举。
    /// </summary>
    public enum BasicArithmeticOperations : int
    {
        /// <summary>
        /// 加法运算符。
        /// </summary>
        [EnumerationDescription("add")]
        Add = 0x0000,
        /// <summary>
        /// 减法运算符。
        /// </summary>
        [EnumerationDescription("subtract")]
        Subtract = 0x0001,
        /// <summary>
        /// 乘法运算符。
        /// </summary>
        [EnumerationDescription("multiply")]
        Multiply = 0x0002,
        /// <summary>
        /// 除法运算符。
        /// </summary>
        [EnumerationDescription("divide")]
        Divide = 0x0003
    }
}
