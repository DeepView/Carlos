using Microsoft.VisualStudio.TestTools.UnitTesting;
using Carlos.Extends;
using System;
using System.Collections.Generic;
using System.Text;

namespace Carlos.Extends.Tests
{
   [TestClass()]
   public class ContentSecurityStringTests
   {
      [TestMethod("Cyclic Encrypt/Decrypt Test")]
      public void CyclicDETest()
      {
         ContentSecurityString css = new ContentSecurityString("Cabinink", false, true, 3);
         css.CyclicEncrypt(@"cabinink");
         Console.WriteLine(css.Content);
         css.CyclicDecrypt(@"cabinink");
         Console.WriteLine(css.Content);
      }
      [TestMethod("Single Encrypt/Decrypt Test")]
      public void SingleDETest()
      {
         ContentSecurityString css = new ContentSecurityString("Cabinink", false, true, 3);
         css.Encrypt(@"cabinink");
         Console.WriteLine(css.Content);
         css.Decrypt(@"cabinink");
         Console.WriteLine(css.Content);
      }
   }
}