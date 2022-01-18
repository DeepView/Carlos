using System.Runtime.InteropServices;
namespace Carlos.Application.Win32Privilege
{
    /// <summary>
    /// TokenPrivileges 结构包含了一个访问令牌的一组权限信息：即该访问令牌具备的权限。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TokenPrivileges
    {
        /// <summary>
        /// 指定了权限数组的容量。
        /// </summary>
        public int PrivilegeCount;
        /// <summary>
        /// 指定一组的LuidAttributes 结构，每个结构包含了LUID和权限的属性。
        /// </summary>
        public LuidAttributes Privileges;
    }
}
