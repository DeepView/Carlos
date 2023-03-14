using System;
using System.Net;
using Carlos.Extends;
using System.Net.Sockets;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace Carlos.Network
{
    /// <summary>
    /// 表示一个端口状态扫描类。
    /// </summary>
    public class PortScanner
    {
        private Int32Range mScanRange;//表示一个有效的端口扫描范围。
        /// <summary>
        /// 构造函数，创建一个默认地址为本机环回地址，端口扫描范围为全范围的扫描实例。
        /// </summary>
        /// <exception cref="NullReferenceException">当IP地址设置不正确时，则将会抛出这个异常。</exception>
        public PortScanner()
        {
            IPAddress.TryParse("127.0.0.1", out IPAddress ipAddress);
            if (ipAddress != null) Host = ipAddress;
            else throw new NullReferenceException("IP address string's format is not true or not null.");
            ScanRange = new Int32Range(0, 65535);
            OpenedPorts = new List<int>();
        }
        /// <summary>
        /// 构造函数，创建一个指定计算机IP地址，端口扫描范围为全范围的扫描实例。
        /// </summary>
        /// <param name="ipAddressString">指定的计算机IP地址。</param>
        /// <exception cref="NullReferenceException">当IP地址设置不正确时，则将会抛出这个异常。</exception>
        public PortScanner(string ipAddressString)
        {
            IPAddress.TryParse(ipAddressString, out IPAddress ipAddress);
            if (ipAddress != null) Host = ipAddress;
            else throw new NullReferenceException("IP address string's format is not true or not null.");
            ScanRange = new Int32Range(0, 65535);
            OpenedPorts = new List<int>();
        }
        /// <summary>
        /// 构造函数，创建一个指定计算机IP地址和指定扫描范围的扫描实例。
        /// </summary>
        /// <param name="ipAddressString">指定的计算机IP地址。</param>
        /// <param name="scanRange">指定的端口扫描范围。</param>
        /// <exception cref="NullReferenceException">当IP地址设置不正确时，则将会抛出这个异常。</exception>
        public PortScanner(string ipAddressString, Int32Range scanRange)
        {
            IPAddress.TryParse(ipAddressString, out IPAddress ipAddress);
            if (ipAddress != null) Host = ipAddress;
            else throw new NullReferenceException("IP address string's format is not true or not null.");
            ScanRange = scanRange;
            OpenedPorts = new List<int>();
        }
        /// <summary>
        /// 获取或设置当前扫描实例的主机地址。
        /// </summary>
        public IPAddress Host { get; set; }
        /// <summary>
        /// 获取或设置当前扫描实例的端口扫描范围。
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">当端口范围设置不当时，则将会抛出这个异常。</exception>
        public Int32Range ScanRange
        {
            get => mScanRange;
            set
            {
                if (value.Lower < 0 || value.Upper > 65535)
                {
                    throw new ArgumentOutOfRangeException("value", "The port range is [0,65535].");
                }
                else mScanRange = value;
            }
        }
        /// <summary>
        /// 获取当前扫描实例在端口扫描操作结束之后的已打开端口的列表。
        /// </summary>
        public List<int> OpenedPorts { get; private set; }
        /// <summary>
        /// 用于存放当扫描成功时需要执行的一些代码。
        /// </summary>
        public ScanOutputHandler ScanOutputHandlerCode { get; set; }
        /// <summary>
        /// 开始在指定扫描范围内扫描已打开的端口，这个操作耗时较长，这个时间长度根据网络环境和本地计算机硬件环境决定，通常这个时间是30~60秒，如果使用了早期版本的计算机硬件，则这个扫描时间可能会更久。
        /// </summary>
        public void Scan()
        {
            Random rand = new Random((int)DateTime.Now.Ticks);
            ClearOpenedPortsList();
            ParallelOptions parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = ScanRange.GetDisparityAbs() >= 1024 ? -1 : 2
            };
            Parallel.For(ScanRange.Lower, ScanRange.Upper + 1, parallelOptions, (port) =>
            {
                Socket scanSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                do
                {
                    try
                    {
                        scanSocket.Bind(new IPEndPoint(IPAddress.Any, rand.Next(65535)));
                        break;
                    }
                    catch (Exception) { }
                } while (true);
                try
                {
                    EndPoint rep = new IPEndPoint(Host, port);
                    scanSocket.BeginConnect(rep, ScanCallback, new ArrayList() { scanSocket, port });
                }
                catch { }
            });
        }
        /// <summary>
        /// 检测指定计算机的指定端口是否被打开或者启用，如果IP地址指定为空，则将把IP地址自动设置为本机环回地址，即127.0.0.1。
        /// </summary>
        /// <param name="ipAddress">指定计算机的IP地址。</param>
        /// <param name="port">需要被检测状态的端口。</param>
        /// <returns>该操作会检测某端口的状态，如果这个端口被打开，则返回true，否则返回false。</returns>
        /// <exception cref="ArgumentOutOfRangeException">如果端口号不在正确的范围之内，则将会抛出这个异常。</exception>
        public static bool GetPortIsOpened(IPAddress ipAddress, int port)
        {
            bool isOpened;
            Int32Range range = new Int32Range(0, 65535);
            if (!range.In(port)) throw new ArgumentOutOfRangeException("port", "The port range is [0,65535].");
            if (ipAddress == null)
            {
                IPAddress.TryParse("127.0.0.1", out IPAddress ip);
                ipAddress = ip;
            }
            IPEndPoint point = new IPEndPoint(ipAddress, port);
            TcpClient tcp = null;
            try
            {
                tcp = new TcpClient();
                tcp.Connect(point);
                isOpened = true;
            }
            catch (Exception) { isOpened = false; }
            if (tcp != null) tcp.Close();
            return isOpened;
        }
        /// <summary>
        /// 清空已启用端口列表的所有数据。
        /// </summary>
        private void ClearOpenedPortsList() => OpenedPorts.Clear();
        /// <summary>
        /// BeginConnect的回调函数。
        /// </summary>
        /// <param name="result">异步Connect的结果。</param>
        private void ScanCallback(IAsyncResult result)
        {
            ArrayList arrList = (ArrayList)result.AsyncState;
            Socket scanSocket = (Socket)arrList[0];
            int port = (int)arrList[1];
            if (result.IsCompleted && scanSocket.Connected)
            {
                OpenedPorts.Add(port);
                if (ScanOutputHandlerCode != null) ScanOutputHandlerCode.Invoke(port);
            }
            scanSocket.Close();
        }
    }
    /// <summary>
    /// 用于存放当PortScanner.ScanCallback私有方法被执行时需要调用的可执行代码的方法。
    /// </summary>
    public delegate void ScanOutputHandler(int port);
}
