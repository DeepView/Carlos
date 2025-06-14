using Microsoft.VisualStudio.TestTools.UnitTesting;
using Carlos.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Carlos.Devices.Tests
{
    [TestClass()]
    public class MouseHelperTests
    {
        [TestMethod()]
        public void RestrictTest()
        {
            MouseHelper.Restrict(new Point(20, 20), new Size(1024, 768));
            Console.WriteLine("The movement range of the mouse has been restricted!");
            System.Threading.Thread.Sleep(5000);
            MouseHelper.Relieve();
            Console.WriteLine("The restriction on the mouse's movement range has been lifted!");
        }

        [TestMethod()]
        public void SendMouseMessageTest()
        {
            string exceptionString = "";
            MouseHelper.SendMouseMessage(new Point(20, 1075), 0x0002, 0, 0,ref exceptionString);
            MouseHelper.SendMouseMessage(new Point(20, 1075), 0x0004, 0, 0, ref exceptionString);
        }
    }
}
