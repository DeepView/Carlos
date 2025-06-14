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
    public class ScreenTests
    {
        [TestMethod()]
        public void SetBrightnessWithGammaTest()
        {
            Screen.SetBrightnessWithGamma(0);
        }
        [TestMethod()]
        public void ResetBrightnessWithGammaTest()
        {
            Screen.ResetBrightnessWithGamma();
        }

        [TestMethod()]
        public void GetPixelColorTest()
        {
            Color color = Screen.GetPixelColor(new Point(0, 0));
            Console.WriteLine(color);
        }

        [TestMethod()]
        public void CaptureTest()
        {
            Bitmap bitmap = Screen.Capture();
            bitmap.Save(@"D:\screen.bmp");
            bitmap = Screen.Capture(new Rectangle(0,0,300,400));
            bitmap.Save(@"D:\screen_0_0_300_400.bmp");
        }
    }
}
