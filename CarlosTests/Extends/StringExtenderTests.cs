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
    }
}