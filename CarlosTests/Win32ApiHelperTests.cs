using Microsoft.VisualStudio.TestTools.UnitTesting;
using Carlos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Carlos.Tests
{
    [TestClass()]
    public class Win32ApiHelperTests
    {
        [TestMethod()]
        public void GetWindowHandleHexStringTest()
        {
            string hexStr = Win32ApiHelper.GetWindowHandleHexString(new System.Drawing.Point(1, 1));
            Console.WriteLine(hexStr);
        }

        [TestMethod()]
        public void CheckEntryPointTest()
        {
            bool isExists = Win32ApiHelper.CheckEntryPoint("user32.dll", "WindowFromPoint");
            Console.WriteLine(isExists);
        }

        [TestMethod()]
        public void LoadOrReleaseDllTest()
        {
            nint handle = Win32ApiHelper.LoadDynamicLinkLibrary(@"gdi32.dll");
            Console.WriteLine(handle.ToString("X"));
            bool isCompleted = Win32ApiHelper.ReleaseDynamicLinkLibrary(handle);
            Console.WriteLine(isCompleted);
            int syserr = Marshal.GetLastWin32Error();
            string errmsg = Win32ApiHelper.FormatErrorCode(syserr);
            Console.WriteLine(errmsg);
        }
    }
}