using Microsoft.VisualStudio.TestTools.UnitTesting;
using Carlos.Extends;
using System;
using System.Collections.Generic;
using System.Text;

namespace Carlos.Extends.Tests
{
    [TestClass()]
    public class StringExtenderTests
    {
        [TestMethod("String Equals with Pointer")]
        public void IsEqualsTest()
        {
            bool isEquals = StringExtender.IsEquals("qwertyuiopasdfghjklzxcvbnm", "qwertyuiopasdfghjklzxcvbnm");
            Console.WriteLine(isEquals);
        }
        [TestMethod("CLR's String Equals Test")]
        public void ClrStringEquals()
        {
            bool isEquals = "qwertyuiopasdfghjklzxcvbnm".Equals("qwertyuiopasdfghjklzxcvbnm");
            Console.WriteLine(isEquals);
        }
        [TestMethod("Convert to ASCII Code Test")]
        public unsafe void ToCodeTest()
        {
            int[] ascii = StringExtender.ToCode("12345").ToArray();
            for (int i = 0; i < ascii.Length; i++)
            {
                Console.WriteLine(ascii[i]);
            }
        }

        [TestMethod()]
        public void ReversalTest()
        {
            string st = "We're going to KTV to sing tonight. Do you want to go? If you want to go, please contact me through WeChat before 6:30 pm.";
            string combined = "";
            for (int i = 0; i < 5000; i++)
            {
                combined += st;
            }
            bool isUsingPtr = false;
            _ = StringExtender.Reversal(combined, isUsingPtr);
            Console.WriteLine(isUsingPtr ? "Used Pointor" : "Normal");
        }

        [TestMethod()]
        public void LeftTest()
        {
            string str = "ABCDEFG";
            string left = str.Left(2);
            Console.WriteLine(left);
        }
    }
}