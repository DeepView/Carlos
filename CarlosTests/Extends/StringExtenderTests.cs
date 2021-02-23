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
   }
}