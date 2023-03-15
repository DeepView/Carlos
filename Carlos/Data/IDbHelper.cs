namespace Carlos.Data
{
    /// <summary>
    /// 适用于数据库操作帮助类的接口。
    /// </summary>
    public interface IDbHelper
    {
        /// <summary>
        /// 获取或设置当前实例的数据库连接字符串。
        /// </summary>
        string ConnectionString { get; set; }
        /// <summary>
        /// 获取当前实例的连接实例是否处于“已连接”的状态。
        /// </summary>
        bool IsConnected { get; }
        /// <summary>
        /// 开始连接到数据库。
        /// </summary>
        /// <returns>如果连接成功，则返回true，否则返回false。</returns>
        bool Connect();
        /// <summary>
        /// 断开与数据库的连接。
        /// </summary>
        /// <returns>如果断开连接成功，则返回true，否则返回false。</returns>
        bool Disconnect();
    }
}
