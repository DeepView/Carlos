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
    public class CommonRegexExpressionTests
    {
        [TestMethod()]
        public void MatchTest()
        {
            string regex = CommonRegexExpression.ChinesePostalCode;
            bool result = "610000".Match(regex);
            Console.WriteLine(@$"Result = {result}");
        }
    }
}
