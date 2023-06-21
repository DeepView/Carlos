using Microsoft.VisualStudio.TestTools.UnitTesting;
using Carlos.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Carlos.Application.Tests
{
    [TestClass()]
    public class ProcessManagerTests
    {
        [TestMethod()]
        public void GetProcessByScreenLocationTest()
        {
            Process proc = ProcessManager.GetProcessByScreenLocation(new System.Drawing.Point(2, 2));
            Console.WriteLine(proc.Id);
            Console.WriteLine(proc.ProcessName);
        }
    }
}