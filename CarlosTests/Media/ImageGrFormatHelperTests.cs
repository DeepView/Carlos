using Microsoft.VisualStudio.TestTools.UnitTesting;
using Carlos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Carlos.Enumerations;
using Carlos.Extends;
using System.IO;
using Carlos.Media;

namespace CarlosTests.Media
{
    [TestClass()]
    public class ImageGrFormatHelperTests
    {
        [TestMethod()]
        public void GetImageGrayDataTest()
        {
            GrayMode grayMode = GrayMode.WAvgEyes;
            Table<byte> grayData = ImageGrayHelper.GetImageGrayData((Bitmap)Image.FromFile(@"D:\bcsz.png"), grayMode);
            Bitmap bitmap = ImageGrayHelper.CreateGrayImage(grayData);
            string savedFileName = $@"D:\Gray_Result_{EnumerationDescriptionAttribute.GetEnumDescription(grayMode)}.jpg";
            bitmap.Save(savedFileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            grayData.ToCsvFile($@"D:\Gray_Data_{EnumerationDescriptionAttribute.GetEnumDescription(grayMode)}.csv");
        }

        [TestMethod()]
        public void GetGrayToFreqDictionaryTest()
        {
            Dictionary<byte, int> freqDict = ImageGrayHelper.GetGrayToFreqDictionary(20, 20000);
            foreach (var kvp in freqDict)
                Console.WriteLine($"Gray Value: {kvp.Key}, Frequency: {kvp.Value}");
        }
    }
}
