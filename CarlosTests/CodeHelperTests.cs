using Carlos;
using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Carlos.Extends;

namespace Carlos.Tests
{
   [TestClass()]
   public class CodeHelperTests
   {
      [TestMethod("Security Long-Integer(64bit) Swap Test")]
      public void SecurityInt64SwapTest()
      {
         long a = 123456, b = 654321;
         CodeHelper.SecurityInt64Swap(ref a, ref b);
         Console.WriteLine($"a={a},b={b}");
      }

      [TestMethod("Security Integer(32bit) Swap Test")]
      public void SecurityInt32SwapTest()
      {
         int a = 1234, b = 4321;
         CodeHelper.SecurityInt32Swap(ref a, ref b);
         Console.WriteLine($"a={a},b={b}");
      }

      [TestMethod("Code Fragment's Execution Duration Test")]
      [SuppressMessage("Style", "IDE0039:使用本地函数", Justification = "<挂起>")]
      public void ExecDurationTest()
      {
         Action code = delegate
         {
            System.Threading.Thread.Sleep(1500);
         };
         long duration = CodeHelper.ExecDuration(code);
         Console.WriteLine(duration);
      }

      [TestMethod("Time Difference's Calculation Test")]
      public void TimeDiffTest()
      {
         DateTime t1 = DateTime.Now, t2 = new DateTime(1949, 10, 01, 10, 00, 00);
         TimeSpan diff = CodeHelper.TimeDiff(t1, t2);
         Console.WriteLine(diff.ToString());
      }

      [TestMethod("Data Swap Test Supporting Generics")]
      public void SwapTest()
      {
         ContentSecurityString csstr01 = new ContentSecurityString("I love China.");
         ContentSecurityString csstr02 = new ContentSecurityString("I love Earth.");
         Console.WriteLine($"Before Swap:\n{csstr01}\n{csstr02}\n");
         CodeHelper.Swap(ref csstr01, ref csstr02);
         Console.WriteLine($"After Swap:\n{csstr01}\n{csstr02}");
      }
   }
}