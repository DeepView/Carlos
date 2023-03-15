using System;
using Carlos.Extends;
namespace Carlos.Application
{
    /// <summary>
    /// 存储符合CLS标准的程序集的基础信息的结构。
    /// </summary>
    public struct CLSAssemblyInfo : IEquatable<CLSAssemblyInfo>
    {
        /// <summary>
        /// 获取应用程序的名称。
        /// </summary>
        public string Name { get; internal set; }
        /// <summary>
        /// 获取应用程序的文件版本号。
        /// </summary>
        public Version FileVersion { get; internal set; }
        /// <summary>
        /// 获取应用程序的产品版本号。
        /// </summary>
        public Version ProductVersion { get; internal set; }
        /// <summary>
        /// 获取应用程序的版权信息。
        /// </summary>
        public string Copyright { get; internal set; }
        /// <summary>
        /// 获取应用程序的完全限定名称。
        /// </summary>
        public string FullName { get; internal set; }
        /// <summary>
        /// 判断当前结构实例是否和指定的CLSAssemblyInfo实例的内容相同。
        /// </summary>
        /// <param name="other">需要被比较内容是否相同的一个CLSAssemblyInfo实例。</param>
        /// <returns>该操作将会返回一个Boolean类型的值，如果比较结果为完全相同，则返回true，否则返回false。</returns>
        public bool Equals(CLSAssemblyInfo other)
        {
            bool isEquals = true;
            if (!StringExtender.IsEquals(Name, other.Name)) isEquals = false;
            if (!FileVersion.Equals(other.FileVersion)) isEquals = false;
            if (!ProductVersion.Equals(other.ProductVersion)) isEquals = false;
            if (!StringExtender.IsEquals(Copyright, other.Copyright)) isEquals = false;
            if (!StringExtender.IsEquals(FullName, other.FullName)) isEquals = false;
            return isEquals;
        }
        /// <summary>
        /// 获取当前结构实例的字符串表达形式。
        /// </summary>
        /// <returns>该操作返回的是一个有一定阅读意义的结构实例字符串表达形式。</returns>
        public override string ToString() => $"{Name}\n\nFile Version: {FileVersion}\nProduct Version: {ProductVersion}\n\n{Copyright}\n\n{FullName}";
    }
}
