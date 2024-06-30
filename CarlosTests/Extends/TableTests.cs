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
    public class TableTests
    {
        [TestMethod()]
        public void TableTest()
        {
            Table<int> table = new(10, 10);
            for (int i = 0; i < table.Length; i++)
            {
                (int row, int col) = table.GetPosition(i);
                table[row, col] = i;
                Console.WriteLine($"index {i}: {table[row, col]}");
            }
        }
        [TestMethod()]
        public void TableCloneTest()
        {
            Table<ContentSecurityString> t = new(1024, 1024);
            Table<ContentSecurityString> c = t.Clone();
            Console.WriteLine($"c.Length={c.Length}");
            t[1, 1] = new ContentSecurityString("I love you!");
            Console.WriteLine($"v={t[1, 1]}");
            Console.WriteLine($"v={c[1, 1]}");
        }
    }
}