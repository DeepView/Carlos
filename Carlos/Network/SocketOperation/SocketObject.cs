using System.Net;
namespace Carlos.Network.SocketOperation
{
    /// <summary>
    /// 一个Socket抽象类，抽象3个方法，初始化Socket（含一个构造），停止和启动方法。
    /// </summary>
    /// <remarks>此抽象类为TcpServer与TcpClient的基类，前者实现后者抽象方法。</remarks>
    public abstract class SocketObject
    {
        /// <summary>
        /// 通过IP地址，端口号来初始化一个Socket。
        /// </summary>
        /// <param name="ipaddress">指定的IP地址。</param>
        /// <param name="port">指定的端口号。</param>
        public abstract void InitSocket(IPAddress ipaddress, int port);
        /// <summary>
        /// 通过IP地址（字符串形式），端口号来初始化一个Socket。
        /// </summary>
        /// <param name="ipaddress">指定的IP地址。</param>
        /// <param name="port">指定的端口号。</param>
        public abstract void InitSocket(string ipaddress, int port);
        /// <summary>
        /// 开始监听。
        /// </summary>
        public abstract void Start();
        /// <summary>
        /// 结束监听。
        /// </summary>
        public abstract void Stop();
    }
}
