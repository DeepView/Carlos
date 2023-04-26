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
    public class FixedLengthQueueTests
    {
        [TestMethod()]
        public void FixedLengthQueueTest()
        {
            FixedLengthQueue<int> queue = new FixedLengthQueue<int>(10);
            for(int i = 0; i < 20; i++)
            {
                queue.Add(i);
                Console.WriteLine($"Head = {queue.Head.Element}, Tail = {queue.Tail.Element}, Count = {queue.Count}");
            }
            Console.WriteLine($"Length = {queue.Length}, Count = {queue.Count}");
            for(int i =0;i <queue.Length;i++) Console.Write($"{queue[i].Element},");
            queue.Clear();
            Console.WriteLine($"\r\nLength = {queue.Length}, Count = {queue.Count}");
            queue.Dispose();
        }
    }
}