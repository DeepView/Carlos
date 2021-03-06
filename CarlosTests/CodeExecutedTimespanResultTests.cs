using Microsoft.VisualStudio.TestTools.UnitTesting;
using Carlos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Carlos.Tests
{
   [TestClass()]
   public class CodeExecutedTimespanResultTests
   {
      [TestMethod("Code Executed Time Count Test")]
      public void ExecTest()
      {
         using CodeExecutedTimespanResult res = new CodeExecutedTimespanResult();
         System.Threading.Thread.Sleep(1500);
      }
   }
}
