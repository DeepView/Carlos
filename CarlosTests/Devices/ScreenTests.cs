using Microsoft.VisualStudio.TestTools.UnitTesting;
using Carlos.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}