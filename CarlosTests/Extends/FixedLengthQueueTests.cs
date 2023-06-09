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
            FixedLengthQueue<int> queue = new(10);
            for (int i = 0; i < 20; i++)
            {
                queue.Add(i);
                Console.WriteLine($"Head = {queue.Head.Element}, Tail = {queue.Tail.Element}, Count = {queue.Count}");
                if (i >= queue.Length) Console.WriteLine($"\tRecycle Data = {queue.Recycle.Element}");
            }
            Console.WriteLine($"Length = {queue.Length}, Count = {queue.Count}\r\n");
            for (int i = 0; i < queue.Length; i++) Console.Write($"{queue[i].Element},");
            queue.Clear();
            Console.WriteLine($"\r\nLength = {queue.Length}, Count = {queue.Count}");
            queue.Dispose();
        }
        [TestMethod()]
        public void RecycleTest()
        {
            FixedLengthQueue<int> queue = new(10);
            ListNode<int> node;
            for (int i = 0; i < queue.Length; i++)
            {
                queue.Add(i);
                node = queue[i];
                Console.WriteLine($"Index {i} : Element = {node.Element}");
            }
            Console.WriteLine($"\tLength = {queue.Length}, Count = {queue.Count}");
            //queue.Add(1024);
            queue.Remove(index: 4);
            Console.WriteLine($"RecycleData = {(queue.Recycle == null ? "#NULL" : queue.Recycle.Element + ", Next:" + queue.Recycle.Next.Element)}");
            Console.WriteLine($"RecycleNodeIndex = {queue.RecycleNodeIndex}");
            queue.Recovery();
            for (int i = 0; i < queue.Count; i++)
            {
                node = queue[i];
                Console.WriteLine($"Index {i} : Element = {node.Element}, NextElement = {(node.Next == null ? "#NULL" : node.Next.Element)}");
            }
            Console.WriteLine($"\tLength = {queue.Length}, Count = {queue.Count}");
            Console.WriteLine($"RecycleData = {(queue.Recycle == null ? "#NULL" : queue.Recycle.Element + ", Next:" + queue.Recycle.Next.Element)}");
            Console.WriteLine($"RecycleNodeIndex = {queue.RecycleNodeIndex}");
        }
    }
}