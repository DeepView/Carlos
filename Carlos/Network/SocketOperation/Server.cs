using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace Carlos.Network.SocketOperation
{
    /// <summary>
    /// 一个适用于Socket的TCP服务器。
    /// </summary>
    /// <seealso cref="SocketObject" />
    public class Server : SocketObject
    {
        private bool IsStop = false;
        private object obj = new object();
        private Semaphore semap = new Semaphore(5, 5000);
        private TcpListener listener;
        private IPAddress ipAddress;
        private int Port;
        /// <summary>
        /// 获取或设置当前实例的欢迎消息。
        /// </summary>
        public string Boundary { get; set; }
        /// <summary>
        /// 获取或设置当前实例的客户端列表。
        /// </summary>
        public List<Sockets> ClientList { get; set; }
        /// <summary>
        /// 获取或设置当前实例的推送套接字方法。
        /// </summary>
        public PushSockets PushSockets { get; set; }
        /// <summary>
        /// 初始化服务端对象。
        /// </summary>
        /// <param name="ipAddr">指定的IP地址。</param>
        /// <param name="port">指定的监听端口。</param>
        public override void InitSocket(IPAddress ipAddr, int port)
        {
            ipAddress = ipAddr;
            Port = port;
            listener = new TcpListener(ipAddress, Port);
            ClientList = new List<Sockets>();
        }
        /// <summary>
        /// 初始化服务端对象。
        /// </summary>
        /// <param name="ipAddr">指定的字符串形式的IP地址。</param>
        /// <param name="port">指定的监听端口。</param>
        public override void InitSocket(string ipAddr, int port)
        {
            ipAddress = IPAddress.Parse(ipAddr);
            Port = port;
            listener = new TcpListener(ipAddress, Port);
            ClientList = new List<Sockets>();
        }
        /// <summary>
        /// 启动监听，并处理连接。
        /// </summary>
        public override void Start()
        {
            try
            {
                listener.Start();
                Thread AccTh = new Thread(new ThreadStart(delegate
                {
                    while (true)
                    {
                        if (IsStop != false)
                        {
                            break;
                        }
                        GetAcceptTcpClient();
                        Thread.Sleep(1);
                    }
                }));
                AccTh.Start();
            }
            catch (SocketException throwedSocketException)
            {
                Sockets sks = new Sockets
                {
                    ThrowedException = throwedSocketException
                };
                PushSockets?.Invoke(sks);
            }
        }
        /// <summary>
        /// 停止监听。
        /// </summary>
        public override void Stop()
        {
            if (listener != null)
            {
                listener.Stop();
                listener = null;
                IsStop = true;
                PushSockets = null;
            }
        }
        /// <summary>
        /// 等待处理新的连接。
        /// </summary>
        private void GetAcceptTcpClient()
        {
            try
            {
                if (listener.Pending())
                {
                    semap.WaitOne();
                    TcpClient tclient = listener.AcceptTcpClient();
                    Socket socket = tclient.Client;
                    NetworkStream stream = new NetworkStream(socket, true);
                    Sockets sks = new Sockets(tclient.Client.RemoteEndPoint as IPEndPoint, tclient, stream)
                    {
                        NewClientFlag = true
                    };
                    PushSockets?.Invoke(sks);
                    sks.NetworkStream.BeginRead(sks.RecBuffer, 0, sks.RecBuffer.Length, new AsyncCallback(EndReader), sks);
                    AddClientList(sks);
                    semap.Release();
                }
            }
            catch
            {
                return;
            }
        }
        /// <summary>
        /// 异步接收发送的信息。
        /// </summary>
        /// <param name="ir">一个IAsyncResult实例。</param>
        private void EndReader(IAsyncResult ir)
        {
            Sockets sks = ir.AsyncState as Sockets;
            if (sks != null && listener != null)
            {
                try
                {
                    if (sks.NewClientFlag || sks.Offset != 0)
                    {
                        sks.NewClientFlag = false;
                        sks.Offset = sks.NetworkStream.EndRead(ir);
                        PushSockets?.Invoke(sks);
                        sks.NetworkStream.BeginRead(sks.RecBuffer, 0, sks.RecBuffer.Length, new AsyncCallback(EndReader), sks);
                    }
                }
                catch (Exception throwedSocketException)
                {
                    lock (obj)
                    {
                        ClientList.Remove(sks);
                        Sockets sk = sks;
                        sk.ClientDispose = true;
                        sk.ThrowedException = throwedSocketException;
                        PushSockets?.Invoke(sks);
                    }
                }
            }
        }
        /// <summary>
        /// 把指定的Sockets加入到客户端队列。
        /// </summary>
        /// <param name="sk">一个指定的套接字。</param>
        private void AddClientList(Sockets sk)
        {
            lock (obj)
            {
                Sockets sockets = ClientList.Find(o => { return o.Ip == sk.Ip; });
                if (sockets == null) ClientList.Add(sk);
                else
                {
                    ClientList.Remove(sockets);
                    ClientList.Add(sk);
                }
            }
        }
        /// <summary>
        /// 向所有在线的客户端广播消息。
        /// </summary>
        /// <param name="data">需要广播的消息。</param>
        public void Broadcast(string data)
        {
            try
            {
                Parallel.ForEach(ClientList, new ParallelOptions() { MaxDegreeOfParallelism = 5 }, item =>
                {
                    if (item != null) SendTo(item.Ip, data);
                });
            }
            catch (Exception throwedException) { if (throwedException != null) throw throwedException; }
        }
        /// <summary>
        /// 向指定的客户端发送信息。
        /// </summary>
        /// <param name="ip">指定客户端的IP地址，并涵盖端口地址（即ip:port）。</param>
        /// <param name="data">需要发送的数据包。</param>
        public void SendTo(IPEndPoint ip, string data)
        {
            try
            {
                Sockets sks = ClientList.Find(o => { return o.Ip == ip; });
                if (sks == null || !sks.Client.Connected || sks.ClientDispose)
                {
                    Sockets ks = new Sockets();
                    sks.ClientDispose = true;
                    sks.ThrowedException = new Exception("客户端无连接");
                    PushSockets?.Invoke(sks);
                    ClientList.Remove(sks);
                }
                if (sks.Client.Connected)
                {
                    NetworkStream nStream = sks.NetworkStream;
                    if (nStream.CanWrite)
                    {
                        byte[] buffer = Encoding.UTF8.GetBytes(data);
                        nStream.Write(buffer, 0, buffer.Length);
                    }
                    else
                    {
                        nStream = sks.Client.GetStream();
                        if (nStream.CanWrite)
                        {
                            byte[] buffer = Encoding.UTF8.GetBytes(data);
                            nStream.Write(buffer, 0, buffer.Length);
                        }
                        else
                        {
                            ClientList.Remove(sks);
                            Sockets ks = new Sockets();
                            sks.ClientDispose = true;
                            sks.ThrowedException = new Exception("客户端无连接");
                            PushSockets?.Invoke(sks);

                        }
                    }
                }
            }
            catch (Exception throwedSocketException)
            {
                Sockets sks = new Sockets
                {
                    ClientDispose = true,
                    ThrowedException = throwedSocketException
                };
                PushSockets?.Invoke(sks);
            }
        }
    }
    /// <summary>
    /// 用于封装PushSockets的一个委托。
    /// </summary>
    /// <param name="sockets">一个套接字实例。</param>
    public delegate void PushSockets(Sockets sockets);
}
