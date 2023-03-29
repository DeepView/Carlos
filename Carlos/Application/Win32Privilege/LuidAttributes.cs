using System.Runtime.InteropServices;
namespace Carlos.Application.Win32Privilege
{
    /// <summary>
    /// LuidAttributes 结构呈现了本地唯一标志和它的属性。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LuidAttributes
    {
        /// <summary>
        /// 特定的LUID。
        /// </summary>
        public Luid ParticularLuid;
        /// <summary>
        /// 指定了LUID的属性，其值可以是一个32位大小的bit 标志，具体含义根据LUID的定义和使用来看。
        /// </summary>
        public int Attributes;
    }
}
