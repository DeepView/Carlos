using Carlos.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace Carlos.Media.Tests
{
    [TestClass()]
    public class ImageGrayHelperTests
    {
        [TestMethod()]
        [SupportedOSPlatform("windows6.1")]
        public void HasAlphaChannelTest()
        {
            Bitmap bitmap = (Bitmap)Bitmap.FromFile(@"D:\Media\Pictures\Other\2017110917545743.jpg");
            bool hasAlphaChannel = ImageGrayHelper.HasAlphaChannel(bitmap);
            Console.WriteLine(hasAlphaChannel);
        }
    }
}
