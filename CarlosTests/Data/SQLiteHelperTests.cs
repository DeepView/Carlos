using Microsoft.VisualStudio.TestTools.UnitTesting;
using Carlos.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carlos.Data.Tests
{
    [TestClass()]
    public class SQLiteHelperTests
    {
        [TestMethod()]
        public void CreateDatabaseTest()
        {
            SQLiteHelper.CreateDatabase("D:\\sqlite_test.db");
        }
    }
}