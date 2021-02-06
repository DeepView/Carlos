using Microsoft.VisualStudio.TestTools.UnitTesting;
using Carlos.Extends;
using System;
using System.Collections.Generic;
using System.Text;

namespace Carlos.Extends.Tests
{
   [TestClass()]
   public class BaseConverterTests
   {
      [TestMethod("Base Conversion Test")]
      public void ConvertToTest()
      {
         Console.WriteLine($"DEC->HEX,16: {BaseConverter.ConvertTo("15", 10, 16)}");
         Console.WriteLine($"BIN->OCT,1111: {BaseConverter.ConvertTo("1111", 2, 8)}");
         Console.WriteLine($"OCT->DEC,17: {BaseConverter.ConvertTo("17", 8, 10)}");
      }
   }
}