﻿using System;
using System.Security.Principal;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;
namespace Carlos.Application.Win32Privilege
{
    /// <summary>
    /// Windows操作系统权限获取类。
    /// </summary>
    [Serializable]
    public sealed class PrivilegeGetter
    {
        private const int ERROR_NOT_ALL_ASSIGNED = 1300;//如果进程的访问令牌中没有关联某权限，则AdjustTokenPrivileges函数调用将会返回错误码ERROR_NOT_ALL_ASSIGNED（值为1300）。
        private const string SECURITY_GROUP_ADMINISTRATORS = @"Administrators";//计算机管理员用户组名称。
        /// <summary>
        /// 获取当前进程的一个伪句柄。
        /// </summary>
        /// <returns>获取当前进程的一个伪句柄，只要当前进程需要一个进程句柄，就可以使用这个伪句柄。该句柄可以复制，但不可继承。</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetCurrentProcess();
        /// <summary>
        /// 关闭一个内核对象。
        /// </summary>
        /// <param name="handle">需要被关闭的对象。</param>
        /// <returns>如果执行成功，返回true，如果执行失败，返回false，如果要获取更多的错误信息，请调用Marshal.GetLastWin32Error。</returns>
        /// <remarks>关闭一个内核对象。其中包括文件、文件映射、进程、线程、安全和同步对象等。在CreateThread成功之后会返回一个hThread的handle，且内核对象的计数加1，CloseHandle之后，引用计数减1，当变为0时，系统删除内核对象。若在线程执行完之后，没有调用CloseHandle，在进程执行期间，将会造成内核对象的泄露，相当于句柄泄露，但不同于内存泄露，这势必会对系统的效率带来一定程度上的负面影响。但当进程结束退出后，系统会自动清理这些资源。</remarks>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static extern bool CloseHandle(IntPtr handle);
        /// <summary>
        /// 查看系统权限的特权值。
        /// </summary>
        /// <param name="systemName">需要查看的系统，本地系统直接用NULL（Nothing）。</param>
        /// <param name="jurisdictionName">指向一个以零结尾的字符串，指定特权的名称。</param>
        /// <param name="luidPointer">接收所返回的制定特权名称的信息。</param>
        /// <returns>如果执行成功，返回true，如果执行失败，返回false，如果要获取更多的错误信息，请调用Marshal.GetLastWin32Error。</returns>
        /// <remarks>查看指定系统权限的特权值，如果操作成功则返回true，否则返回false，与此同时，还会将接收的信息反馈到LocallyUniqueIdentifier结构类里面。</remarks>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LookupPrivilegeValue(string systemName, string jurisdictionName, ref Luid luidPointer);
        /// <summary>
        /// 打开与进程相关联的访问令牌。
        /// </summary>
        /// <param name="handle">要修改访问权限的进程句柄。</param>
        /// <param name="operationType">指定你要进行的操作类型。</param>
        /// <param name="tokenPointer">返回的访问令牌指针。</param>
        /// <returns>如果执行成功，返回true，如果执行失败，返回false，如果要获取更多的错误信息，请调用Marshal.GetLastWin32Error。</returns>
        /// <remarks>打开与进程相关联的访问令牌，当修改权限的时候需要用到这个句柄，操作成功返回true，否则返回false。</remarks>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        public static extern bool OpenProcessToken([In()] IntPtr handle, [In()] int operationType, [Out()] IntPtr tokenPointer);
        /// <summary>
        /// 启用或禁止指定访问令牌的特权。
        /// </summary>
        /// <param name="tokenPointer">包含特权的句柄。</param>
        /// <param name="disable">禁用所有权限标志。</param>
        /// <param name="newStateInfo">新特权信息。</param>
        /// <param name="bufferSize">缓冲数据大小,以字节为单位的PreviousState的缓存区。</param>
        /// <param name="privileges">接收被改变特权当前状态的Buffer。</param>
        /// <param name="retunValueSize">接收PreviousState缓存区要求的大小。</param>
        /// <returns>如果执行成功，返回true，如果执行失败，返回false，如果要获取更多的错误信息，请调用Marshal.GetLastWin32Error。</returns>
        /// <remarks>启用或禁用特权一个有TOKEN_ADJUST_PRIVILEGES访问的访问令牌，成功返回true，否则返回false。</remarks>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        public static extern bool AdjustTokenPrivileges(IntPtr tokenPointer, bool disable, ref TokenPrivileges newStateInfo, Int32 bufferSize, TokenPrivileges privileges, Int32 retunValueSize);
        /// <summary>
        /// 授予当前进程需要申请的Windows操作系统权限。
        /// </summary>
        /// <param name="privilegeName">需要申请的权限所对应的权限字符串，该字符串是PrivilegeConst中包含的字符串常量。</param>
        /// <returns>如果操作成功，则将会返回true，否则返回false。</returns>
        public static bool GrantPrivilege(string privilegeName)
        {
            try
            {
                Luid locallyUniqueIdentifier = new Luid();
                if (LookupPrivilegeValue(null, privilegeName, ref locallyUniqueIdentifier))
                {
                    LuidAttributes luidAndAtt = new LuidAttributes()
                    {
                        Attributes = (int)PrivilegeAttributes.SE_PRIVILEGE_ENABLED,
                        ParticularLuid = locallyUniqueIdentifier
                    };
                    TokenPrivileges tokenPrivileges = new TokenPrivileges()
                    {
                        PrivilegeCount = 1,
                        Privileges = luidAndAtt
                    };
                    TokenPrivileges tempTokenPriv = new TokenPrivileges();
                    IntPtr tokenHandle = IntPtr.Zero;
                    try
                    {
                        bool condition = OpenProcessToken(GetCurrentProcess(), (int)TokenAccess.TOKEN_ADJUST_PRIVILEGES | (int)TokenAccess.TOKEN_QUERY, tokenHandle);
                        if (condition)
                        {
                            if (AdjustTokenPrivileges(tokenHandle, false, ref tokenPrivileges, 1024, tempTokenPriv, 0))
                            {
                                if (Marshal.GetLastWin32Error() != ERROR_NOT_ALL_ASSIGNED) return true;
                            }
                        }
                    }
                    finally
                    {
                        if (tokenHandle != IntPtr.Zero) CloseHandle(tokenHandle);
                    }
                }
                return false;
            }
            catch { return false; }
        }
        /// <summary>
        /// 撤销当前进程已经申请的Windows操作系统权限。
        /// </summary>
        /// <param name="privilegeName">需要撤销的权限所对应的权限字符串，该字符串是PrivilegeConst中包含的字符串常量。</param>
        /// <returns>如果操作成功，则将会返回true，否则返回false。</returns>
        public static bool RevokePrivilege(string privilegeName)
        {
            try
            {
                Luid locallyUniqueIdentifier = new Luid();
                if (LookupPrivilegeValue(null, privilegeName, ref locallyUniqueIdentifier))
                {
                    LuidAttributes luidAndAtt = new LuidAttributes()
                    {
                        ParticularLuid = locallyUniqueIdentifier
                    };
                    TokenPrivileges tokenPrivileges = new TokenPrivileges()
                    {
                        PrivilegeCount = 1,
                        Privileges = luidAndAtt
                    };
                    TokenPrivileges tempTokenPriv = new TokenPrivileges();
                    IntPtr tokenHandle = IntPtr.Zero;
                    try
                    {
                        bool condition = OpenProcessToken(GetCurrentProcess(), (int)TokenAccess.TOKEN_ADJUST_PRIVILEGES | (int)TokenAccess.TOKEN_QUERY, tokenHandle);
                        if (condition)
                        {
                            if (AdjustTokenPrivileges(tokenHandle, false, ref tokenPrivileges, 1024, tempTokenPriv, 0))
                            {
                                if (Marshal.GetLastWin32Error() != ERROR_NOT_ALL_ASSIGNED) return true;
                            }
                        }
                    }
                    finally
                    {
                        if (tokenHandle != IntPtr.Zero) CloseHandle(tokenHandle);
                    }
                }
                return false;
            }
            catch { return false; }
        }
        /// <summary>
        /// 要求需要执行的操作必须具备Windows管理员组权限，执行这个操作才允许用管理员身份运行您需要运行的应用程序。
        /// </summary>
        public static void NeedAdministratorsPrivilege() => ChangePrivilegeRole(SECURITY_GROUP_ADMINISTRATORS);
        /// <summary>
        /// 修改代码的执行用户或者安全组，修改之后只允许该用户或者安全组才能执行指定的代码区域。
        /// </summary>
        /// <param name="userOrGroupName">可允许的执行用户或者安全组。</param>
        public static void ChangePrivilegeRole(string userOrGroupName)
        {
            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            PrincipalPermission principalPerm = new PrincipalPermission(null, userOrGroupName);
            principalPerm.Demand();
        }
    }
}
