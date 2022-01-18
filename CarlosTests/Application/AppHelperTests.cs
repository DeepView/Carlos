using Microsoft.VisualStudio.TestTools.UnitTesting;
using Carlos.Application;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Carlos.Application.Tests
{
   [TestClass()]
   public class AppHelperTests
   {
      [TestMethod("Application Instance Getter Test")]
      public void GetAssemblyInstanceTest()
      {
         string clrAsm = @"D:\Coding\CommonPlatform\Carlos\Carlos\Carlos\bin\Debug\netstandard2.1\CarlosSaber.dll";
         Assembly asm = AppHelper.GetCLSAssemblyInstance(clrAsm);
         Console.WriteLine(asm);
         Console.WriteLine(asm.Location);
         CLSAssemblyInfo info = AppHelper.GetCLSAssemblyInfo(asm);
         Console.WriteLine($"\n{info}");
         CLSAssemblyInfo info2 = info;
         Console.WriteLine(info2.Equals(info));
         Console.WriteLine($"\n{info2}");
      }

      [TestMethod("Strong Name Verification Test")]
      public void HasStrongNameSignatureTest()
      {
         bool r = AppHelper.HasStrongNameSignature(@"C:\Windows\System32\cmd.exe");
         Console.WriteLine(r);
      }
   }
}