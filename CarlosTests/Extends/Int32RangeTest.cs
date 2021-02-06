using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Carlos.Extends;
namespace CarlosTests.Extends
{
   [TestClass()]
   public class Int32RangeTest
   {
      [TestMethod("Int32Range Structure Test")]
      public void Int32RangeStructTest()
      {
         Int32Range range = new Int32Range(0, 65535, false, true);
         Console.WriteLine(range);
      }
      [TestMethod("Aggregate Relation Judgment Test")]
      public void InTest()
      {
         Int32Range range = new Int32Range(0, 65535, true, true);
         bool include = range.In(0);
         Console.WriteLine(include);
      }
   }
}
