using System;
using System.Runtime.InteropServices;
namespace Carlos.Environments
{
    /// <summary>
    /// 用于获取计算机信息的类。
    /// </summary>
    public sealed class ComputerInfo
    {
        /// <summary>
        /// 获取当前用户下的主机名称和用户名。
        /// </summary>
        /// <returns>该操作将会返回一个以DOMAIN\UserName形式的字符串。</returns>
        public static string Whoami() => $"{Environment.MachineName}\\{Environment.UserName}";
        /// <summary>
        /// 获取当前计算机正在运行的操作系统信息。
        /// </summary>
        /// <returns>该操作将会返回一个可能包含操作系统制造提供商，操作系统名称和操作系统版本序列的字符串。</returns>
        public static string OS() => RuntimeInformation.OSDescription;
        /// <summary>
        /// 获取当前操作系统的类型。
        /// </summary>
        /// <returns>该操作将会返回一个字符串，用于表示当前操作系统的类型，比如说Windows，如果无法判断其操作系统的类型，则将会返回“UNKNOWN_OS_PLATFORM”。</returns>
        public static string OSType()
        {
            string os = "UNKNOWN_OS_PLATFORM";
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            bool isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            bool isMacOS = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
            if (isWindows) os = "Windows";
            if (isLinux) os = "Linux";
            if (isMacOS) os = "macOS";
            return os;
        }
        /// <summary>
        /// 获取当前操作系统的运行平台。
        /// </summary>
        /// <returns>该操作将会返回一个字符串，这个字符串表示当前操作系统或者运行环境的平台，比如说X86，Arm64。</returns>
        public static string Architecture() => RuntimeInformation.OSArchitecture.ToString().ToLower();
    }
}
