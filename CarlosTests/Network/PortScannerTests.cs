using Microsoft.VisualStudio.TestTools.UnitTesting;
using Carlos.Network;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Carlos.Network.Tests
{
   [TestClass()]
   public class PortScannerTests
   {
      [TestMethod("Get All Ports's Status Test")]
      public void GetPortStatusTest()
      {
         PortScanner scanner = new PortScanner();
         scanner.ClearOpenedPortsList();
         scanner.Scan();
         for (int i = 0; i < scanner.OpenedPorts.Count; i++)
         {
            Console.WriteLine($"The port {scanner.OpenedPorts[i]} is opened.");
         }
         Console.WriteLine("Scan work is complated.");
      }
   }
}