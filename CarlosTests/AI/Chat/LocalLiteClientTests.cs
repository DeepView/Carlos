using System;
using System.Collections.Generic;
using System.Text;
using Carlos.AI.Chat;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CarlosTests.AI.Chat
{
    [TestClass()]
    public class LocalLiteClientTests
    {
        [TestMethod()]
        public void LocalLiteClientTest()
        {
            LocalLiteClient client = new("http://localhost:11434", "deepseek-r1:8b");
            client.GetResponse("你好！", (chunk) =>
            {
                Console.Write(chunk);
            });
        }
    }
}
