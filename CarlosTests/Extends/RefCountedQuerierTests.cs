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
    public class RefCountedQuerierTests
    {
        [TestMethod()]
        public void RefCountedQuerierTest()
        {
            RefCountedQuerier<string> q1 = RefCountedQuerier<string>.GetOrCreate("TestObject");
            RefCountedQuerier<string> q2 = RefCountedQuerier<string>.GetOrCreate("TestObject");
            Console.WriteLine(q1.RefCount);
            Console.WriteLine(q2.RefCount);
        }
    }
}
