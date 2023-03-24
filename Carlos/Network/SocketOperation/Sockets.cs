using System;
using System.Net;
using System.Net.Sockets;
namespace Carlos.Network.SocketOperation
{
    /// <summary>
    /// 一个通过封装的Socket对象实例。
    /// </summary>
    public class Sockets
    {
        /// <summary>
        /// 构造函数，这是一个空构造函数，不会执行任何操作。
        /// </summary>
        public Sockets() : base() { }
        /// <summary>
        /// 构造函数，根据IP地址，TCP客户端和网络流来创建Sockets对象。
        /// </summary>
        /// <param name="ip">指定的IP地址。</param>
        /// <param name="client">TCP客户端，一个TcpClient实例。</param>
        /// <param name="stream">承载TCP客户端Socket的网络流。</param>
        public Sockets(IPEndPoint ip, TcpClient client, NetworkStream stream) : base()
        {
            Ip = ip;
            Client = client;
            NetworkStream = stream;
        }
        /// <summary>
        /// 构造函数，根据IP地址，TCP客户端和网络流来创建Sockets对象，并附加上用户凭据。
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="ip">Ip地址</param>
        /// <param name="client">TcpClient</param>
        /// <param name="stream">承载客户端Socket的网络流</param>
        public Sockets(string username, string password, IPEndPoint ip, TcpClient client, NetworkStream stream) : base()
        {
            UserName = username;
            Password = password;
            Ip = ip;
            Client = client;
            NetworkStream = stream;
        }
        /// <summary>
        /// 一个接收数据的缓冲区。
        /// </summary>
        public byte[] RecBuffer = new byte[8 * 1024];
        /// <summary>
        /// 一个发送数据的缓冲区。
        /// </summary>
        public byte[] SendBuffer = new byte[8 * 1024];
        /// <summary>
        /// 获取或设置在异步接收后的数据包大小。
        /// </summary>
        public int Offset { get; set; }
        /// <summary>
        /// 获取或设置用户名。
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 获取或设置用户密码。
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 获取或设置当前的IP地址和端口号。
        /// </summary>
        public IPEndPoint Ip { get; set; }
        /// <summary>
        /// 获取或设置当前的TCP客户端主通信程序。
        /// </summary>
        public TcpClient Client { get; set; }
        /// <summary>
        /// 获取或设置承载TCP客户端Socket的网络流。
        /// </summary>
        public NetworkStream NetworkStream { get; set; }
        /// <summary>
        /// 获取或设置在实例在通信时产生的异常。
        /// </summary>
        public Exception ThrowedException { get; set; }
        /// <summary>
        /// 获取或设置一个新客户端标识，如果推送器发现此标识为true，那么认为是客户端上线，此属性仅在服务端有效。
        /// </summary>
        public bool NewClientFlag { get; set; }
        /// <summary>
        /// 获取或设置一个客户端退出标识，如果服务端发现此标识为true，那么认为客户端下线，客户端接收此标识时，认为客户端异常，此属性仅在服务端生效。
        /// </summary>
        public bool ClientDispose { get; set; }
    }
}
