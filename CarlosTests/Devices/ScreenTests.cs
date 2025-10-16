using Microsoft.VisualStudio.TestTools.UnitTesting;
using Carlos.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.Versioning;

namespace Carlos.Devices.Tests
{
    [TestClass()]
    public class ScreenTests
    {
        [TestMethod()]
        [SupportedOSPlatform("windows6.1")]
        public void SetBrightnessWithGammaTest()
        {
            Screen.SetBrightnessWithGamma(0);
        }

        [TestMethod()]
        [SupportedOSPlatform("windows6.1")]
        public void ResetBrightnessWithGammaTest()
        {
            Screen.ResetBrightnessWithGamma();
        }

        [TestMethod()]
        [SupportedOSPlatform("windows6.1")]
        public void GetPixelColorTest()
        {
            Color color = Screen.GetPixelColor(new Point(0, 0));
            Console.WriteLine(color);
        }

        [TestMethod()]
        [SupportedOSPlatform("windows6.1")]
        public void CaptureTest()
        {
            Bitmap bitmap = Screen.Capture();
            bitmap.Save(@"D:\screen.bmp");
            bitmap = Screen.Capture(new Rectangle(0, 0, 300, 400));
            bitmap.Save(@"D:\screen_0_0_300_400.bmp");
        }
    }
}
