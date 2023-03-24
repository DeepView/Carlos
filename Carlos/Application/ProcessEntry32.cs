using System;
using System.Runtime.InteropServices;
namespace Carlos.Application
{
    /// <summary>
    /// 一个用来存放快照进程信息的结构体，用来Process32First指向第一个进程信息，并将进程信息抽取到ProcessEntry32中。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct ProcessEntry32
    {
        /// <summary>
        /// 这个结构的长度，以字节为单位，初始化一个实例以后调用Process32First函数，设置成员的大小。
        /// </summary>
        public uint Size;
        /// <summary>
        /// 此进程的引用计数，这个成员已经不再被使用，总是设置为零。
        /// </summary>
        public uint Usage;
        /// <summary>
        /// 进程标识符，即PID。
        /// </summary>
        public uint ProcessId;
        /// <summary>
        /// 进程默认堆ID，这个成员已经不再被使用，总是设置为零。
        /// </summary>
        public IntPtr DefaultHeapId;
        /// <summary>
        /// 进程模块ID，这个成员已经不再被使用，总是设置为零。
        /// </summary>
        public uint ModuleId;
        /// <summary>
        /// 此进程开启的线程计数。
        /// </summary>
        public uint Threads;
        /// <summary>
        /// 父进程的进程标识符。
        /// </summary>
        public uint ParentProcessId;
        /// <summary>
        /// 线程优先权，当前进程创建的任何一个线程的基础优先级，即在当前进程内创建线程的话，其基本优先级的值。
        /// </summary>
        public int PriorityClassBase;
        /// <summary>
        /// 进程标记，这个成员已经不再被使用，总是设置为零。
        /// </summary>
        public uint Flags;
        /// <summary>
        /// 进程的可执行文件名称。要获得可执行文件的完整路径，应调用Module32First函数，再检查其返回的MODULEENTRY32结构的szExePath成员。但是，如果被调用进程是一个64位程序，您必须调用QueryFullProcessImageName函数去获取64位进程的可执行文件完整路径名。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string ExecuteFileName;
    }
}
