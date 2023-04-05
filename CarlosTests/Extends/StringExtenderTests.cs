using Microsoft.VisualStudio.TestTools.UnitTesting;
using Carlos.Extends;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Carlos.Extends.Tests
{
    [TestClass()]
    public class StringExtenderTests
    {
        [TestMethod("String Equals with Pointer")]
        public void IsEqualsTest()
        {
            bool isEquals = StringExtender.IsEquals("qwertyuiopasdfghjklzxcvbnm", "qwertyuiopasdfghjklzxcvbnm");
            Console.WriteLine(isEquals);
        }
        [TestMethod("CLR's String Equals Test")]
        public void ClrStringEquals()
        {
            bool isEquals = "qwertyuiopasdfghjklzxcvbnm".Equals("qwertyuiopasdfghjklzxcvbnm");
            Console.WriteLine(isEquals);
        }
        [TestMethod("Convert to ASCII Code Test")]
        public unsafe void ToCodeTest()
        {
            int[] ascii = "12345".ToCode().ToArray();
            for (int i = 0; i < ascii.Length; i++)
            {
                Console.WriteLine(ascii[i]);
            }
        }

        [TestMethod()]
        public void ReversalTest()
        {
            string st = "We're going to KTV to sing tonight. Do you want to go? If you want to go, please contact me through WeChat before 6:30 pm.";
            string combined = "";
            for (int i = 0; i < 5000; i++)
            {
                combined += st;
            }
            _ = combined.UnsafeReverse();
        }

        [TestMethod()]
        public void ReversalTest2()
        {
            string a = "ABCDEFG";
            string b = "ABCDEFG";
            a.UnsafeReverse();
            Console.WriteLine(b);
        }

        [TestMethod()]
        public void LeftTest()
        {
            string str = "ABCDEFG";
            string left = str.Left(2);
            Console.WriteLine(left);
        }

        [TestMethod()]
        public void SpliceTest()
        {
            string s1 = "ABCDEFG";
            string s2 = "12345";
            string r = s2;
            for (int i = 0; i < 2000; i++) s2.Glue(s1);
            Console.WriteLine(s2);
        }

        [TestMethod()]
        public void SpliceTest2()
        {
            string s1 = "ABCDEFG";
            string s2 = "12345";
            string r = s2.Glue(s1);
            for (int i = 0; i < r.Length; i++) Console.WriteLine(r[i]);
        }

        [TestMethod()]
        public void SpliceTestWithStringAdd()
        {
            string s1 = "ABCDEFG";
            string s2 = "12345";
            for (int i = 0; i < 2000; i++) s2 += s1;
            Console.WriteLine(s2);
        }

        [TestMethod()]
        public void SpliceTestWithBuilder()
        {
            string s1 = "ABCDEFG";
            string s2 = "12345";
            StringBuilder sb = new(s2);
            for (int i = 0; i < 2000; i++) sb.Append(s1);
            Console.WriteLine(sb.ToString());
        }

        [TestMethod()]
        public void SafeReversalTest()
        {
            string st = "We're going to KTV to sing tonight. Do you want to go? If you want to go, please contact me through WeChat before 6:30 pm.";
            string combined = "";
            for (int i = 0; i < 5000; i++)
            {
                combined += st;
            }
            _ = combined.SafeReverse();
        }

        private string ReadFile(string path)
        {
            string r;
            StreamReader reader = new StreamReader(path);
            r = reader.ReadToEnd();
            reader.Close();
            return r;
        }

        [TestMethod()]
        public void CutTest()
        {
            string str = ReadFile(@"D:\jqmin.txt");
            string cut;
            int len = str.Length;
            for (int i = 0; i < len - 1; i++) cut = str.Cut(0, i + 1);
        }
        [TestMethod()]
        public void SubstringTest()
        {
            string str = ReadFile(@"D:\jqmin.txt");
            string cut;
            int len = str.Length;
            for (int i = 0; i < len - 1; i++) cut = str.Substring(0, i + 1);
        }

        [TestMethod()]
        public void RightTest()
        {
            string s = "HelloWorld";
            Console.WriteLine(s.Right(5));
        }

        [TestMethod()]
        public void GlueTest()
        {
            string[] strings = { "Apple", "Microsoft", "Intel", "IBM", "Huawei", "NVIDIA", "AMD", "Google", "Oracle", "Alibaba", "Amazon", "Baidu", "Tencent", "Xiaomi", "ZTE" };
            string enterprise = "Enterprise:".Glue(strings, "\r\n\t");
            Console.WriteLine(enterprise);
        }

        [TestMethod()]
        public void ClrGlueTest()
        {
            string[] strings = { "Apple", "Microsoft", "Intel", "IBM", "Huawei", "NVIDIA", "AMD", "Google", "Oracle", "Alibaba", "Amazon", "Baidu", "Tencent", "Xiaomi", "ZTE" };
            string enterprise = "Enterprise:";
            for (int i = 0; i < strings.Length; i++) enterprise += $"\r\n\t{strings[i]}";
            Console.WriteLine(enterprise);
        }
    }
}