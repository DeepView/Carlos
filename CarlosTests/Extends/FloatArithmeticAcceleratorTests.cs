using Microsoft.VisualStudio.TestTools.UnitTesting;
using Carlos.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carlos.Extends.Tests
{
    [TestClass()]
    public class FloatArithmeticAcceleratorTests
    {
        [TestMethod()]
        public void AccelTest()
        {
            Console.WriteLine($"{DateTime.Now}");
            FloatArithmeticAccelerator accelerator = new(0);
            //accelerator.PrintDeviceInfos();
            accelerator.Build();
            Console.WriteLine($"devicesCount = {accelerator.Devices.Length}");
            double[] a = { 1, 2, 3 };
            double[] b = { 4, 5, 6 };
            double[] result = accelerator.Execute(a, b, Enumerations.BasicArithmeticOperations.Add);
            accelerator.Dispose();
            for (int i = 0; i < result.Length; i++)
                Console.WriteLine($"\tResult 01 = {result[i]}");
            Console.WriteLine("");
        }
    }
}
