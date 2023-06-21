using System;
using System.IO;
using System.Linq;
using System.Drawing;
using Carlos.Devices;
using System.Security;
using System.Threading;
using System.Diagnostics;
using System.Collections;
using Carlos.Enumerations;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Carlos.Application.Win32Privilege;
namespace Carlos.Application
{
    /// <summary>
    /// 进程管理类，这个类包含了一系列的进程操作的静态方法。
    /// </summary>
    public class ProcessManager
    {
        private static Hashtable mProcessHandleHashTable = new Hashtable();//进程句柄哈希表。
        private const int ERROR_SUCCESS = 0;//当成功时需要返回的错误码。
        /// <summary>
        /// 窗口枚举委托。
        /// </summary>
        /// <param name="handle">窗口的句柄。</param>
        /// <param name="lParam">需要传递的特定参数。</param>
        /// <returns>当操作成功是返回true，否则返回false。</returns>
        public delegate bool WindowEnumProc(IntPtr handle, uint lParam);
        /// <summary>
        /// 获取调用线程最近的错误代码值。
        /// </summary>
        /// <returns>Int32</returns>
        /// <remarks>该函数返回调用线程最近的错误代码值，错误代码以单线程为基础来维护的，多线程不重写各自的错误代码值。</remarks>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int GetLastError();
        /// <summary>
        /// 设定线程的优先级。
        /// </summary>
        /// <param name="thread">需要被调整优先级的线程的句柄。</param>
        /// <param name="priority">返回带有THREAD_PRIORITY_???前缀的某个函数，它定义了线程的优级。</param>
        /// <returns>如果执行成功，返回0，如果执行失败，返回非0，如果要获取更多的错误信息，请调用Marshal.GetLastWin32Error。</returns>
        /// <remarks>设定线程的优先级，注意，在Windows里面，线程的优先级同进程的优先级类组合在一起就决定了线程的实际优先级。</remarks>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern long SetThreadPriority(long thread, long priority);
        /// <summary>
        /// 通过获取进程信息为指定的进程、进程使用的堆[HEAP]、模块[MODULE]、线程建立一个快照。
        /// </summary>
        /// <param name="flags">用来指定“快照”中需要返回的对象，可以是TH32CS_SNAPPROCESS等。</param>
        /// <param name="processId">一个进程ID号，用来指定要获取哪一个进程的快照，当获取系统进程列表或获取 当前进程快照时可以设为0。</param>
        /// <returns>调用成功，返回快照的句柄，调用失败，返回INVALID_HANDLE_VALUE，如果要获取更多的错误信息，请调用Marshal.GetLastWin32Error。</returns>
        [DllImport("kernel32.dll")]
        private static extern IntPtr CreateToolhelp32Snapshot(uint flags, uint processId);
        /// <summary>
        /// 进程获取函数，当我们利用函数CreateToolhelp32Snapshot()获得当前运行进程的快照后，我们可以利用process32First函数来获得第一个进程的句柄。
        /// </summary>
        /// <param name="handle">一个快照句柄，存放以前调用的CreateToolhelp32Snapshot函数的返回值。</param>
        /// <param name="pe">一个指向ProcessEntry32结构。它包含进程信息，例如可执行文件的名称、进程标识符和父进程的进程标识符。</param>
        /// <returns>如果执行成功，返回true，如果执行失败，返回false，如果要获取更多的错误信息，请调用Marshal.GetLastWin32Error。</returns>
        [DllImport("kernel32.dll")]
        private static extern int Process32First(IntPtr handle, ref ProcessEntry32 pe);
        /// <summary>
        /// 一个进程获取函数，当我们利用函数CreateToolhelp32Snapshot()获得当前运行进程的快照后,我们可以利用Process32Next函数来获得下一个进程的句柄。
        /// </summary>
        /// <param name="handle">一个快照句柄，存放以前调用的CreateToolhelp32Snapshot函数的返回值。</param>
        /// <param name="pe">一个指向ProcessEntry32结构。它包含进程信息，例如可执行文件的名称、进程标识符和父进程的进程标识符。</param>
        /// <returns>如果执行成功，返回true，如果执行失败，返回false，如果要获取更多的错误信息，请调用Marshal.GetLastWin32Error。</returns>
        [DllImport("kernel32.dll")]
        private static extern int Process32Next(IntPtr handle, ref ProcessEntry32 pe);
        /// <summary>
        /// 找出某个窗口的创建者（线程或进程），返回创建者的标志符。
        /// </summary>
        /// <param name="handle">被查找窗口的句柄。</param>
        /// <param name="id">进程号的存放地址（变量地址）。</param>
        /// <returns>返回线程号，注意，id 是存放进程号的变量。返回值是线程号，id是进程号存放处。</returns>
        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int id);
        /// <summary>
        /// 该函数将指定的消息发送到一个或多个窗口。
        /// </summary>
        /// <param name="windowHandle">其窗口程序将接收消息的窗口的句柄。如果此参数为HWND_BROADCAST，则消息将被发送到系统中所有顶层窗口，包括无效或不可见的非自身拥有的窗口、被覆盖的窗口和弹出式窗口，但消息不被发送到子窗口。</param>
        /// <param name="message">指定被发送的消息。</param>
        /// <param name="wParam">指定附加的消息特定信息。</param>
        /// <param name="lParam">指定附加的消息特定信息。</param>
        /// <returns>返回值指定消息处理的结果，依赖于所发送的消息。</returns>
        /// <remarks>此函数为指定的窗口调用窗口程序，直到窗口程序处理完消息再返回。而和函数PostMessage不同，PostMessage是将一个消息寄送到一个线程的消息队列后就立即返回。另外，需要用HWND_BROADCAST通信的应用程序应当使用函数RegisterWindowMessage来为应用程序间的通信取得一个唯一的消息。</remarks>
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(int windowHandle, int message, int wParam, string lParam);
        /// <summary>
        /// 该函数枚举所有屏幕上的顶层窗口，并将窗口句柄传送给应用程序定义的回调函数。
        /// </summary>
        /// <param name="enumFunc">指向一个应用程序定义的回调函数指针。</param>
        /// <param name="lParam">指定一个传递给回调函数的应用程序定义值。</param>
        /// <returns>如果函数成功，返回值为非零，如果函数失败，返回值为零。若想获得更多错误信息，请调用GetLastError函数。</returns>
        [DllImport("user32.dll", EntryPoint = "EnumWindows", SetLastError = true)]
        private static extern bool EnumWindows(WindowEnumProc enumFunc, uint lParam);
        /// <summary>
        /// 获得一个指定子窗口的父窗口句柄。
        /// </summary>
        /// <param name="handle">函数要获得该子窗口的父窗口句柄。</param>
        /// <returns>如果函数成功，返回值为父窗口句柄。如果窗口无父窗口，则函数返回NULL。若想获得更多错误信息，请调用GetLastError函数。</returns>
        [DllImport("user32.dll", EntryPoint = "GetParent", SetLastError = true)]
        private static extern IntPtr GetParent(IntPtr handle);
        /// <summary>
        /// 该函数确定给定的窗口句柄是否标识一个已存在的窗口。
        /// </summary>
        /// <param name="handle">用于作判定的窗口的句柄。</param>
        /// <returns>如果窗口句柄标识了一个已存在的窗口，返回值为非零；如果窗口句柄标识的窗口不存在，返回值为零。</returns>
        [DllImport("user32.dll", EntryPoint = "IsWindow")]
        private static extern bool IsWindow(IntPtr handle);
        /// <summary>
        /// 设置调用线程的最后一个错误代码。
        /// </summary>
        /// <param name="errorCode">线程的最后一个错误代码。</param>
        [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
        private static extern void SetLastError(uint errorCode);
        /// <summary>
        /// 创建一个指定文件地址的进程。
        /// </summary>
        /// <param name="executeFileUrl">需要创建进程的一个Windows可执行文件。</param>
        /// <returns>该方法会返回这个进程的System.Diagnostics.Process实例。</returns>
        public static Process CreateProcess(string executeFileUrl)
        {
            return CreateProcess(executeFileUrl, ProcessPriority.RealTime);
        }
        /// <summary>
        /// 创建一个指定文件地址的进程，同时会允许指定一个进程的优先级。
        /// </summary>
        /// <param name="executeFileUrl">需要创建进程的一个Windows可执行文件。</param>
        /// <param name="priority">需要被创建的进程的优先级。</param>
        /// <returns>该方法会返回这个进程的System.Diagnostics.Process实例。</returns>
        /// <exception cref="FileNotFoundException">当指定的Windows可执行文件找不到时，则会抛出这个异常。</exception>
        public static Process CreateProcess(string executeFileUrl, ProcessPriority priority)
        {
            Process retval = new Process();
            bool condition = !File.Exists(executeFileUrl) && !Directory.Exists(executeFileUrl);
            if (condition) throw new FileNotFoundException("This excuted file is not found.");
            retval = Process.Start(executeFileUrl);
            try
            {
                retval.PriorityClass = (ProcessPriorityClass)priority;
            }
            catch (NullReferenceException exception)
            {
#if DEBUG
                Console.WriteLine("Throwed Exception => \n{0}\nTime => {1}", exception.ToString(), DateTime.Now.ToString());
#endif
            }
            return retval;
        }
        /// <summary>
        /// 创建一个进程，在创建这个进程的时候会指定相关的权限和域。
        /// </summary>
        /// <param name="executeFileUrl">需要创建进程的一个Windows可执行文件。</param>
        /// <param name="userName">启动进程时使用的用户名。</param>
        /// <param name="password">一个SecureString实例，它包含启动进程时要使用的密码。</param>
        /// <param name="domian">启动进程时要使用的域。</param>
        /// <returns>该方法会返回这个进程的System.Diagnostics.Process实例。</returns>
        /// <exception cref="FileNotFoundException">当指定的Windows可执行文件找不到时，则会抛出这个异常。</exception>
        public static Process CreateProcess(string executeFileUrl, string userName, SecureString password, string domian)
        {
            if (File.Exists(executeFileUrl)) throw new FileNotFoundException("This excuted file is not found.");
            return Process.Start(executeFileUrl, userName, password, domian);
        }
        /// <summary>
        /// 创建一个进程，在创建这个进程会指定一些命令行参数，以及权限和域。
        /// </summary>
        /// <param name="executeFileUrl">需要创建进程的一个Windows可执行文件。</param>
        /// <param name="arguments">启动该进程时传递的命令行实参。</param>
        /// <param name="userName">启动进程时使用的用户名。</param>
        /// <param name="password">一个SecureString实例，它包含启动进程时要使用的密码。</param>
        /// <param name="domian">启动进程时要使用的域。</param>
        /// <returns>该方法会返回这个进程的System.Diagnostics.Process实例。</returns>
        public static Process CreateProcess(string executeFileUrl, string arguments, string userName, SecureString password, string domian)
        {
            return CreateProcess(executeFileUrl, arguments, userName, password, domian, ProcessPriority.RealTime);
        }
        /// <summary>
        /// 创建一个拥有指定优先级的进程，在创建这个进程会指定一些命令行参数，以及权限和域。
        /// </summary>
        /// <param name="executeFileUrl">需要创建进程的一个Windows可执行文件。</param>
        /// <param name="arguments">启动该进程时传递的命令行实参。</param>
        /// <param name="userName">启动进程时使用的用户名。</param>
        /// <param name="password">一个SecureString实例，它包含启动进程时要使用的密码。</param>
        /// <param name="domian">启动进程时要使用的域。</param>
        /// <param name="priority">需要被创建的进程的优先级。</param>
        /// <returns>该方法会返回这个进程的System.Diagnostics.Process实例。</returns>
        /// <exception cref="FileNotFoundException">当指定的Windows可执行文件找不到时，则会抛出这个异常。</exception>
        public static Process CreateProcess(string executeFileUrl, string arguments, string userName, SecureString password, string domian, ProcessPriority priority)
        {
            Process retval;
            if (!File.Exists(executeFileUrl)) throw new FileNotFoundException("This excuted file is not found.");
            retval = Process.Start(executeFileUrl, arguments, userName, password, domian);
            retval.PriorityClass = (ProcessPriorityClass)priority;
            return retval;
        }
        /// <summary>
        /// 终结一个通过进程映像名称指定的进程。
        /// </summary>
        /// <param name="processName">需要被终结的进程的映像名称。</param>
        /// <returns>如果这个进程不存在或者因为其他原因，则会导致进程终结失败，终结失败会返回false。</returns>
        public static bool KillProcess(string processName)
        {
            bool retval = false;
            try
            {
                if (!ProcessExists(processName)) retval = false;
                else
                {
                    foreach (Process item in Process.GetProcesses())
                    {
                        if (item.ProcessName.ToLower() == processName.ToLower())
                        {
                            item.Kill();
                            retval = true;
                            break;
                        }
                    }
                }
            }
            catch { retval = false; }
            return retval;
        }
        /// <summary>
        /// 终结一个通过进程标识符指定的进程
        /// </summary>
        /// <param name="processId">需要被终结的进程的标识符。</param>
        /// <returns>如果这个进程不存在或者因为其他原因，则会导致进程终结失败，终结失败会返回false。</returns>
        public static bool KillProcess(int processId)
        {
            bool retval = false;
            try
            {
                Parallel.ForEach(Process.GetProcesses(), (item, interrupt) =>
                {
                    if (item.Id == processId)
                    {
                        item.Kill();
                        retval = true;
                        interrupt.Stop();
                    }
                });
            }
            catch { retval = false; }
            return retval;
        }
        /// <summary>
        /// 检查指定的进程是否存在。
        /// </summary>
        /// <param name="processName">需要被终结的进程的映像名称。</param>
        /// <returns>如果指定的进程存在则返回true，反之返回false。</returns>
        public static bool ProcessExists(string processName)
        {
            Process[] prc = Process.GetProcesses();
            bool retval = false;
            Parallel.ForEach(prc, (pr, stat) =>
            {
                if (processName == pr.ProcessName)
                {
                    retval = true;
                    stat.Stop();
                }
            });
            return retval;
        }
        /// <summary>
        /// 获取当前所有进程的进程名称。
        /// </summary>
        /// <returns>该操作将会返回一个包含目前所有Windows进程名称的List实例。</returns>
        public static List<string> GetAllProcessesName()
        {
            List<string> list = new List<string>();
            List<Process> procs = Process.GetProcesses().ToList();
            Parallel.For(0, procs.Count, (index) => list.Add(procs[index].ProcessName));
            return list;
        }
        /// <summary>
        /// 更改进程的优先级，但是这个操作可能存在风险，更改成功与否，都会返回一个Boolean值。
        /// </summary>
        /// <param name="processName">需要被修改优先级的进程的映像名称。</param>
        /// <param name="priority">需要被修改的进程优先级。</param>
        /// <returns>如果这个方法的操作成功则为true，反之为false。</returns>
        public static bool ChangProcessPriority(string processName, ProcessPriority priority)
        {
            bool retval = true;
            Process[] ps = Process.GetProcesses();
            foreach (Process p in ps)
            {
                if (p.ProcessName == processName)
                {
                    try
                    {
                        p.PriorityClass = (ProcessPriorityClass)priority;
                    }
                    catch
                    {
                        retval = false;
                    }
                }
            }
            return retval;
        }
        /// <summary>
        /// 更改由指定句柄所表示的线程的优先级。
        /// </summary>
        /// <param name="threadHandle">指定线程的句柄。</param>
        /// <param name="priority">需要设置的线程优先级。</param>
        /// <returns>如果这个方法的操作成功则为true，反之为false。</returns>
        public static bool ChangeThreadPriority(IntPtr threadHandle, Enumerations.ThreadPriority priority) => SetThreadPriority(threadHandle.ToInt64(), (long)priority) != 0;
        /// <summary>
        /// 更改指定线程的优先级。
        /// </summary>
        /// <param name="thread">指定的线程实例。</param>
        /// <param name="priority">需要设置的线程优先级。</param>
        public static void ChangeThreadPriority(Thread thread, System.Threading.ThreadPriority priority) => thread.Priority = priority;
        /// <summary>
        /// 更改指定线程的优先级。
        /// </summary>
        /// <param name="encapsulatedMethod">指定的ThreadStart封装的线程实例。</param>
        /// <param name="priority">需要设置的线程优先级。</param>
        public static void ChangeThreadPriority(ThreadStart encapsulatedMethod, System.Threading.ThreadPriority priority) => _ = new Thread(encapsulatedMethod) { Priority = priority };
        /// <summary>
        /// 获取指定线程的状态。
        /// </summary>
        /// <param name="thread">用户获取状态的线程。</param>
        /// <returns>如果操作无异常，则将会返回指定线程在Windows中的线程状态。</returns>
        public static System.Threading.ThreadState GetThreadState(Thread thread) => thread.ThreadState;
        /// <summary>
        /// 修改指定进程的权限。
        /// </summary>
        /// <param name="processName">指定的待修改进程。</param>
        /// <param name="systemName">需要被操作的系统，弱势本地系统，这里可以指定为空（NULL Or Nothing）。</param>
        /// <param name="operationType">待操作的类型，这个操作类型通畅可以用TOKEN_ADJUST_PRIVILEGES代替。</param>
        /// <param name="privileges">指定的权限（特权）。</param>
        /// <returns>如果这个方法的操作成功则为true，反之为false。</returns>
        /// <remarks>该操作允许用户修改指定的进程的权限，不过这个操作可能存在风险，所以在提升权限的时候要谨慎操作。</remarks>
        public static bool UpdateProcessPrivileges(string processName, string systemName, int operationType, TokenPrivileges privileges)
        {
            IntPtr hwnd_t = IntPtr.Zero;
            bool rv;
            PrivilegeGetter.LookupPrivilegeValue(systemName, processName, ref privileges.Privileges.ParticularLuid);
            PrivilegeGetter.OpenProcessToken(PrivilegeGetter.GetCurrentProcess(), operationType, hwnd_t);
            PrivilegeGetter.AdjustTokenPrivileges(hwnd_t, false, ref privileges, 100000, new TokenPrivileges(), 0);
            rv = GetLastError() == ERROR_SUCCESS;
            PrivilegeGetter.CloseHandle(hwnd_t);
            return rv;
        }
        /// <summary>
        /// 根据窗口标题来获取进程的ID。
        /// </summary>
        /// <param name="windowTitle">指定的窗口标题。</param>
        /// <returns>该操作会返回指定窗口标题所属进程的进程标识符（PID）。</returns>
        public static int GetProcessIdByWindowTitle(string windowTitle)
        {
            int processId = 0;
            Process[] arrayProcess = Process.GetProcesses();
            foreach (Process p in arrayProcess)
            {
                if (p.MainWindowTitle.IndexOf(windowTitle) != -1)
                {
                    processId = p.Id;
                    break;
                }
            }
            return processId;
        }
        /// <summary>
        /// 根据进程名称来获取进程的ID。
        /// </summary>
        /// <param name="processName">指定进程的名称，注意这里的进程名称不需要在后面添加后缀名。</param>
        /// <returns>该操作会返回指定进程名称所对应的进程标识符（PID）。</returns>
        public static int GetProcessIdByImageName(string processName)
        {
            int processId = 0;
            Process[] arrayProcess = Process.GetProcessesByName(processName);
            foreach (Process process in arrayProcess)
            {
                processId = process.Id;
                return processId;
            }
            return processId;
        }
        /// <summary>
        /// 根据窗体标题查找窗口句柄，另外这个操作支持标题模糊匹配。
        /// </summary>
        /// <param name="windowTitle">指定的窗口标题。</param>
        /// <returns>该操作会返回指定窗口标题所对应的窗口句柄。</returns>
        public static IntPtr GetWindowHandleByTitle(string windowTitle)
        {
            Process[] ps = Process.GetProcesses();
            foreach (Process p in ps)
            {
                if (p.MainWindowTitle.IndexOf(windowTitle) != -1) return p.MainWindowHandle;
            }
            return IntPtr.Zero;
        }
        /// <summary>
        /// 根据指定的进程名称获取这个进程的句柄。
        /// </summary>
        /// <param name="processName">指定的进程名称，注意这里的进程名称不需要在后面添加后缀名。</param>
        /// <returns>该操作会返回一个进程句柄，如果操作出现异常，则可能会返回一个Zero句柄。</returns>
        /// <remarks>需要在调用方访问Cabinink.Windows.Privileges.PrivilegeGetter.NeedAdministratorsPrivilege()方法，然后才能调用此方法，否则将会抛出System.Security.SecurityException异常。</remarks>
        public static IntPtr GetHandleByImageName(string processName)
        {
            IntPtr processHandle = IntPtr.Zero;
            Process[] procs = Process.GetProcesses();
            for (int i = 0; i < procs.Length; i++)
            {
                if (processName == procs[i].ProcessName)
                {
                    try
                    {
                        processHandle = procs[i].Handle;
                    }
                    catch (Exception throwedException) { if (throwedException != null) throw throwedException; }
                    break;
                }
            }
            return processHandle;
        }
        /// <summary>
        /// 通过一个窗口句柄获得创建该窗口的进程实例。
        /// </summary>
        /// <param name="windowHandle">一个窗口句柄。</param>
        /// <returns>在没有抛出任何异常的情况下，该操作将会返回一个Process实例。</returns>
        /// <exception cref="ArgumentException">如果这个方法所传递的窗口句柄不存在，则将会抛出这个异常。</exception>
        public static Process GetProcessByHandle(IntPtr windowHandle)
        {
            int threadId = GetWindowThreadProcessId(windowHandle, out int pid);
            if (threadId == 0)
            {
                throw new ArgumentException("This window handle is not existing.", nameof(windowHandle));
            }
            return Process.GetProcessById(pid);
        }
        /// <summary>
        /// 通过当前鼠标光标位置所在窗口来获得创建这个窗口的进程实例。
        /// </summary>
        /// <returns>在没有抛出任何异常的情况下，该操作将会返回一个Process实例。</returns>
        public static Process GetProcessByCursor() => GetProcessByScreenLocation(MouseHelper.GetCursorPosition());
        /// <summary>
        /// 通过指定的屏幕位置所在窗口来获得创建这个窗口的进程实例。
        /// </summary>
        /// <param name="location">指定的屏幕位置。</param>
        /// <returns>在没有抛出任何异常的情况下，该操作将会返回一个Process实例。</returns>
        public static Process GetProcessByScreenLocation(Point location) => GetProcessByHandle(Win32ApiHelper.GetWindowHandle(location));
        /// <summary>
        /// 获取当前窗口的句柄。
        /// </summary>
        /// <param name="processId">窗口所在进程的PID。</param>
        /// <returns>该操作会返回一个窗口句柄，如果操作出现异常，则可能会返回一个Zero句柄。</returns>
        public static IntPtr GetCurrentWindowHandle(uint processId)
        {
            IntPtr ptrWnd = IntPtr.Zero;
            object objWnd = mProcessHandleHashTable[processId];
            if (objWnd != null)
            {
                ptrWnd = (IntPtr)objWnd;
                if (ptrWnd != IntPtr.Zero && IsWindow(ptrWnd)) return ptrWnd;
                else ptrWnd = IntPtr.Zero;
            }
            bool bResult = EnumWindows(new WindowEnumProc(EnumWindowsProc), processId);
            if (!bResult && Marshal.GetLastWin32Error() == 0)
            {
                objWnd = mProcessHandleHashTable[processId];
                if (objWnd != null) ptrWnd = (IntPtr)objWnd;
            }
            return ptrWnd;
        }
        /// <summary>
        /// 窗口枚举，用于作为API函数EnumWindows的回调参数。
        /// </summary>
        /// <param name="handle">窗口的句柄。</param>
        /// <param name="lParam">用于传递的特定参数。</param>
        /// <returns>如果该操作成功，则返回true，否则返回false。</returns>
        public static bool EnumWindowsProc(IntPtr handle, uint lParam)
        {
            if (GetParent(handle) == IntPtr.Zero)
            {
                GetWindowThreadProcessId(handle, out int uiPid);
                if (uiPid == lParam)
                {
                    mProcessHandleHashTable.Add(uiPid, handle);
                    SetLastError(0);
                    return false;
                }
            }
            return true;
        }
    }
}
