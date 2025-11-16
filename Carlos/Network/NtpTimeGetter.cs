using System;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;
namespace Carlos.Network
{
    /// <summary>
    /// 适用于获取互联网时间，以及同步本地时间的类。
    /// </summary>
    public class NtpTimeGetter
    {
        private string mNtpServer;//指定的NTP服务器。
        private ManualResetEvent mDoneEvent;//通知线程等待用的事件。
        private DateTime mDateTime;//存储已经更新的时间。
        /// <summary>
        /// 设置当前本地时间及日期。
        /// </summary>
        /// <param name="systemTime">一个SYSTEMTIME结构的指针，包含了新的本地日期和时间。</param>
        /// <returns>如果函数调用成功，则返回值为非零值,如果函数失败，返回值是零,为了得到扩展的错误信息，请调用GetLastError函数 。</returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern bool SetLocalTime([In] ref LocalSystemTime systemTime);
        /// <summary>
        /// 构造函数，创建一个指定NTP服务器的NTP时间获取实例。
        /// </summary>
        /// <param name="ntpServer">指定的NTP服务器。</param>
        public NtpTimeGetter(string ntpServer)
        {
            mDateTime = DateTime.Now;
            mNtpServer = ntpServer;
            mDoneEvent = new ManualResetEvent(false);
        }
        /// <summary>
        /// 构造函数，创建一个指定NTP服务器和线程等待通知事件的NTP时间获取实例。
        /// </summary>
        /// <param name="ntpServer">指定的NTP服务器。</param>
        /// <param name="doneEvent">用于通知线程等待用的事件。</param>
        public NtpTimeGetter(string ntpServer, ManualResetEvent doneEvent)
        {
            mDateTime = DateTime.Now;
            mNtpServer = ntpServer;
            mDoneEvent = doneEvent;
        }
        /// <summary>
        /// 获取或设置当前实例的NTP服务器。
        /// </summary>
        public string NtpServer { get => mNtpServer; set => mNtpServer = value; }
        /// <summary>
        /// 获取或设置当前实例需要更新的时间。
        /// </summary>
        public DateTime Time { get => mDateTime; set => mDateTime = value; }
        /// <summary>
        /// 从当前实例指定的NTP服务器获取时间，但不会应用到本地计算机。
        /// </summary>
        /// <exception cref="Exception">当网络不可用时，则会抛出这个异常。</exception>
        public void UpdateTimeFromNetwork()
        {
            try
            {
                byte[] ntpData = new byte[48];
                ntpData[0] = 0x1B;
                IPAddress[] addresses = Dns.GetHostEntry(NtpServer).AddressList;
                IPEndPoint ipEndPoint = new IPEndPoint(addresses[0], 123);
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.Connect(ipEndPoint);
                socket.ReceiveTimeout = 3000;
                socket.Send(ntpData);
                socket.Receive(ntpData);
                socket.Close();
                const byte serverReplyTime = 40;
                ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);
                ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);
                intPart = SwapEndianness(intPart);
                fractPart = SwapEndianness(fractPart);
                ulong milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
                DateTime networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);
                Time = networkDateTime.ToLocalTime();
            }
            catch (Exception exception)
            {
                if (exception != null) throw exception.InnerException;
                else throw new Exception("Network unavailable.");
            }
        }
        /// <summary>
        /// 多线程多服务器更新时间，但不会将时间应用到本地计算机。
        /// </summary>
        /// <param name="ntpServers">指定多个NTP服务器，用“|”符号分割，比如说："ntp.api.bz|time.windows.com|210.72.145.44|time.nist.gov"</param>
        public void UpdateTimeWithMultithreading(string ntpServers)
        {
            string[] ntpServerArray = ntpServers.Split('|');
            Thread[] ths = new Thread[ntpServerArray.Length];
            NtpTimeGetter[] ntpTime = new NtpTimeGetter[ntpServerArray.Length];
            ManualResetEvent[] mre = new ManualResetEvent[ntpServerArray.Length];
            for (int i = 0; i < ntpServerArray.Length; i++)
            {
                mre[i] = new ManualResetEvent(false);
                NtpTimeGetter ntp = new NtpTimeGetter(ntpServerArray[i], mre[i]);
                ntpTime[i] = ntp;
                ThreadPool.QueueUserWorkItem(ntp.ThreadPoolCallback, i);
            }
            WaitHandle.WaitAny(mre, 15000);
            DateTime dt = new DateTime(1900, 1, 1, 0, 0, 0, 0);
            for (int i = 0; i < ntpServerArray.Length; i++)
            {
                if (ntpTime[i].Time > dt) Time = ntpTime[i].Time;
            }
        }
        /// <summary>
        /// 将Time属性所包含的时间更新本地计算机时间，不过这个方法可能需要以管理员身份运行或者提升特权。
        /// </summary>
        /// <param name="win32ErrorCode">需要传递并且用于开发者参考的错误代码。</param>
        /// <param name="win32ErrorInformation">需要传递并且用于开发者参考的错误消息。</param>
        /// <returns>如果操作成功则会返回true，否则会返回false。</returns>
        /// <remarks>
        /// <para>需要在调用方访问Carlos.Application.Win32Privileges.PrivilegeGetter.NeedAdministratorsPrivilege()方法，然后才能调用此方法，否则将会抛出System.Security.SecurityException异常。</para>
        /// <para>但是在.NET Core运行时中，这个方法将不再支持PrincipalPermission属性，请改用在项目的manifest文件中提升用户的安全策略，这个则需要修改项目manifest文件的requestedExecutionLevel设置，可供参考的设置代码如下所示。</para>
        /// <code language="xml">
        /// &lt;requestedExecutionLevel level="requireAdministrator" uiAccess="false"/&gt;
        /// </code>
        /// <para>另外需要补充的是，在.NET Core项目中，一旦修改了manifest文件中的requestedExecutionLevel设置，则不需要访问Carlos.Application.Win32Privileges.PrivilegeGetter.NeedAdministratorsPrivilege()方法。</para>
        /// </remarks>
        public bool UpdateLocalTime(out long win32ErrorCode, out string win32ErrorInformation)
        {
            win32ErrorCode = 0x0000;
            win32ErrorInformation = Win32ApiHelper.FormatErrorCode(0x0000);
            LocalSystemTime systemTime = new()
            {
                Year = Convert.ToUInt16(Time.Year),
                Month = Convert.ToUInt16(Time.Month),
                DayOfWeek = Convert.ToUInt16(Time.DayOfWeek),
                Day = Convert.ToUInt16(Time.Day),
                Hour = Convert.ToUInt16(Time.Hour),
                Minute = Convert.ToUInt16(Time.Minute),
                Second = Convert.ToUInt16(Time.Second),
                Miliseconds = Convert.ToUInt16(Time.Millisecond)
            };
            bool result = SetLocalTime(ref systemTime);
            if (!result)
            {
                win32ErrorCode = Win32ApiHelper.GetLastWin32ApiError();
                win32ErrorInformation = Win32ApiHelper.FormatErrorCode(win32ErrorCode);
            }
            return result;
        }
        /// <summary>
        /// 交换字节顺序。
        /// </summary>
        /// <param name="swapedNum">用于执行该操作的必要参数。</param>
        /// <returns>返回一个交换字节顺序之后的结果。</returns>
        private static uint SwapEndianness(ulong swapedNum)
        {
            return (uint)(((swapedNum & 0xff) << 24) + ((swapedNum & 0xff00) << 8) + ((swapedNum & 0xff0000) >> 8) + ((swapedNum & 0xff000000) >> 24));
        }
        /// <summary>
        /// 线程池回调。
        /// </summary>
        /// <param name="threadContext">指定的线程上下文。</param>
        private void ThreadPoolCallback(object threadContext)
        {
            UpdateTimeFromNetwork();
            if (Time.Year >= 2014) mDoneEvent.Set();
        }
    }
}
