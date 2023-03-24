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
            PortScanner scanner = new PortScanner("127.0.0.1", new Extends.Int32Range(32, 1024));
            scanner.ScanOutputHandlerCode = delegate (int port)
            {
                Console.WriteLine($"{port}=opened.");
            };
            scanner.Scan();
        }

        [TestMethod("Single Port Scan's Test")]
        public void GetPortIsOpenedTest()
        {
            bool isOpened = PortScanner.GetPortIsOpened(null, 80);
            Console.WriteLine(isOpened);
        }
    }
}