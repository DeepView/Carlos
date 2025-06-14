using Microsoft.VisualStudio.TestTools.UnitTesting;
using Carlos;
using Carlos.Media.GoldenRecord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Carlos.Enumerations;
using Carlos.Extends;

namespace Carlos.Media.GoldenRecord.Tests
{
    [TestClass()]
    public class ImageGrFormatHelperTests
    {
        [TestMethod()]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:验证平台兼容性", Justification = "<挂起>")]
        public void GetImageGrayDataTest()
        {
            GrayMode grayMode = GrayMode.WAvgEyes;
            Table<byte> grayData = ImageGrFormatHelper.GetImageGrayData((Bitmap)Bitmap.FromFile(@"D:\bcsz.png"), grayMode);
            Bitmap bitmap = ImageGrFormatHelper.CreateGrayImage(grayData);
            string savedFileName = $@"D:\Gray_Result_{EnumerationDescriptionAttribute.GetEnumDescription(grayMode)}.jpg";
            bitmap.Save(savedFileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            grayData.ToCsvFile($@"D:\Gray_Data_{EnumerationDescriptionAttribute.GetEnumDescription(grayMode)}.csv");
        }

        [TestMethod()]
        public void GetGrayToFreqDictionaryTest()
        {
            Dictionary<byte, int> freqDict = ImageGrFormatHelper.GetGrayToFreqDictionary(20, 20000);
            foreach (var kvp in freqDict)
                Console.WriteLine($"Gray Value: {kvp.Key}, Frequency: {kvp.Value}");
        }

        [TestMethod()]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:验证平台兼容性", Justification = "<挂起>")]
        public void CreateWaveFileTest()
        {
            ImageGrFormatHelper.ConvertToWaveFile(
                @"D:\test.wav",
                (Bitmap)Bitmap.FromFile(@"D:\bcsz.png"),
                GrayMode.WAvgEyes,
                new GoldenRecordInformation
                {
                    MinimumFreq = 20,
                    MaximumFreq = 20000,
                    FreqResolution = 1000 // 1000 Hz resolution
                }
            );
            Console.WriteLine("Wave file created successfully.");
        }
    }
}
