using Microsoft.VisualStudio.TestTools.UnitTesting;
using Carlos.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Carlos.Media.Tests
{
    [TestClass()]
    public class ImageGrayHelperTests
    {
        [TestMethod()]
        public void HasAlphaChannelTest()
        {
            Bitmap bitmap = (Bitmap)Bitmap.FromFile(@"D:\Media\Pictures\Other\2017110917545743.jpg");
            bool hasAlphaChannel = ImageGrayHelper.HasAlphaChannel(bitmap);
            Console.WriteLine(hasAlphaChannel);
        }
    }
}
