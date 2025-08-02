using Microsoft.VisualStudio.TestTools.UnitTesting;
using Carlos.Extends;
using System;
using System.Collections.Generic;
using System.Text;

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
            Console.WriteLine($"q1.Value: {q1.Value}, RefCount: {q1.RefCount}");
            Console.WriteLine($"q2.Value: {q2.Value}, RefCount: {q2.RefCount}");
        }
    }
}
