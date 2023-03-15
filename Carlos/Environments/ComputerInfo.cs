using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
namespace Carlos.Environments
{
    public sealed class ComputerInfo
    {
        public static string Whoami() => $"{Environment.MachineName}\\{Environment.UserName}";
    }
}
