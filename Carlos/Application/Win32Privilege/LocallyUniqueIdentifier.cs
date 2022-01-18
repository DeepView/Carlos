using System.Runtime.InteropServices;
namespace Carlos.Application.Win32Privilege
{
    /// <summary>
    /// 本地唯一标志是一个64位的数值，它被保证在产生它的系统上唯一！LUID的在机器被重启前都是唯一的。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LocallyUniqueIdentifier
    {
        /// <summary>
        /// 本地唯一标志的低32位。
        /// </summary>
        public int LowPart;
        /// <summary>
        /// 本地唯一标志的高32位。
        /// </summary>
        public int HighPart;
    }
}
