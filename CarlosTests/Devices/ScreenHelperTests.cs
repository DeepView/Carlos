using Microsoft.VisualStudio.TestTools.UnitTesting;
using Carlos.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Carlos.Devices.Tests
{
    [TestClass()]
    public class ScreenHelperTests
    {
        [TestMethod()]
        public void ScreenTest()
        {
            Size res = Screen.GetResolving();
            Console.WriteLine($"Resolving = {res.Width}*{res.Height}");
            Console.WriteLine($"BitsPerPixel = {Screen.GetBitsPerPixel()} bit");
            Console.WriteLine($"RefreshRate = {Screen.GetRefreshRate()} Hz");
            Console.WriteLine($"IsTouch = {Screen.IsTouchScreen(out int touchNumber)}/TouchNumber is {touchNumber}");
        }
    }
}