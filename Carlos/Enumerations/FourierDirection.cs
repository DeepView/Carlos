namespace Carlos.Enumerations
{
    /// <summary>
    /// 指定傅里叶变换操作的方向。
    /// </summary>
    public enum FourierDirection : int
    {
        /// <summary>
        /// 正向傅里叶变换
        /// </summary>
        Forward = 1,
        /// <summary>
        /// 逆向傅里叶变换
        /// </summary>
        Inverse = -1,
        /// <summary>
        /// 双向傅里叶变换
        /// </summary>
        Both = 0
    }
}
