using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
namespace Carlos.Network.SocketOperation
{
    /// <summary>
    /// 一个适用于Socket的TCP客户端。
    /// </summary>
    public class Client : SocketObject
    {
        private bool IsClose = false;
        private Sockets sk;
        private TcpClient client;
        private IPAddress ipAddress;
        private int Port;
        private IPEndPoint ip;
        private NetworkStream NetworkStream;
        /// <summary>
        /// 获取或设置当前实例的推送套接字方法。
        /// </summary>
        public PushSockets PushSockets { get; set; }
        /// <summary>
        /// 初始化客户端对象。
        /// </summary>
        /// <param name="ipAddr">指定的IP地址。</param>
        /// <param name="port">指定的端口。</param>
        public override void InitSocket(IPAddress ipAddr, int port)
        {
            ipAddress = ipAddr;
            Port = port;
            ip = new IPEndPoint(ipAddress, Port);
            client = new TcpClient();
        }
        /// <summary>
        /// 初始化客户端对象。
        /// </summary>
        /// <param name="ipAddr">指定的字符串形式的IP地址。</param>
        /// <param name="port">指定的端口。</param>
        public override void InitSocket(string ipAddr, int port)
        {
            ipAddress = IPAddress.Parse(ipAddr);
            Port = port;
            ip = new IPEndPoint(ipAddress, Port);
            client = new TcpClient();
        }
        /// <summary>
        /// 开始监听，重写Start方法，其实就是连接服务端。
        /// </summary>
        public override void Start() => Connect();
        /// <summary>
        /// 连接服务端。
        /// </summary>
        private void Connect()
        {
            client.Connect(ip);
            NetworkStream = new NetworkStream(client.Client, true);
            sk = new Sockets(ip, client, NetworkStream);
            sk.NetworkStream.BeginRead(sk.RecBuffer, 0, sk.RecBuffer.Length, new AsyncCallback(EndReader), sk);
        }
        /// <summary>
        /// 异步接收发送的信息。
        /// </summary>
        /// <param name="ir">一个IAsyncResult实例。</param>
        private void EndReader(IAsyncResult ir)
        {
            Sockets s = ir.AsyncState as Sockets;
            try
            {
                if (s != null)
                {
                    if (IsClose && client == null)
                    {
                        sk.NetworkStream.Close();
                        sk.NetworkStream.Dispose();
                        return;
                    }
                    s.Offset = s.NetworkStream.EndRead(ir);
                    PushSockets?.Invoke(s);
                    sk.NetworkStream.BeginRead(sk.RecBuffer, 0, sk.RecBuffer.Length, new AsyncCallback(EndReader), sk);
                }
            }
            catch (Exception throwedSocketException)
            {
                Sockets sks = s;
                sks.ThrowedException = throwedSocketException;
                sks.ClientDispose = true;
                PushSockets?.Invoke(sks);
            }
        }
        /// <summary>
        /// 停止监听。
        /// </summary>
        public override void Stop()
        {
            Sockets sks = new Sockets();
            try
            {
                if (client != null)
                {
                    client.Client.Shutdown(SocketShutdown.Both);
                    Thread.Sleep(10);
                    client.Close();
                    IsClose = true;
                    client = null;
                }
                else
                {
                    sks.ThrowedException = new Exception("The client is not initializated.");
                }
                PushSockets?.Invoke(sks);
            }
            catch (Exception throwedException)
            {
                sks.ThrowedException = throwedException;
            }
        }
        /// <summary>
        /// 发送消息。
        /// </summary>
        /// <param name="data">需要被发送的消息。</param>
        public void SendData(string data)
        {
            try
            {
                if (client == null || !client.Connected)
                {
                    Sockets sks = new Sockets
                    {
                        ThrowedException = new Exception("客户端无连接.."),
                        ClientDispose = true
                    };
                    PushSockets?.Invoke(sks);
                }
                if (client.Connected)
                {
                    NetworkStream ??= client.GetStream();
                    byte[] buffer = Encoding.UTF8.GetBytes(data);
                    NetworkStream.Write(buffer, 0, buffer.Length);
                }
            }
            catch (Exception skex)
            {
                Sockets sks = new Sockets
                {
                    ThrowedException = skex,
                    ClientDispose = true
                };
                PushSockets?.Invoke(sks);
            }
        }
    }
}
