using Microsoft.VisualStudio.TestTools.UnitTesting;
using Carlos.Environments;
using System;
using System.Collections.Generic;
using System.Text;

namespace Carlos.Environments.Tests
{
    [TestClass()]
    public class ComputerInfoTests
    {
        [TestMethod()]
        public void WhoamiTest()
        {
            Console.WriteLine(ComputerInfo.Whoami());
        }
    }
}