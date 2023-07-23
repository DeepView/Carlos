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
    public class LotterySimulatorTests
    {
        [TestMethod()]
        public void LotterySimulatorTest()
        {
            LotterySimulator lottery = new(0.05, 0.001, 20);
            Console.WriteLine($"初始概率：{lottery.InitProbability}");
            Console.WriteLine($"保底次数：{lottery.InevitableFrequency}");
            Console.WriteLine($"概率增量：{lottery.Increment}\n\n");
            for (int i = 0; i < lottery.InevitableFrequency + 1; i++)
            {
                bool bingo = lottery.Bingo(false, 8);
                Console.WriteLine($"\t实时概率：{lottery.RealtimeProbability}");
                Console.WriteLine($"\t实时次数：{lottery.RealtimeFrequency}");
                Console.WriteLine($"\t抽奖结果：{bingo}\n");
            }
        }
    }
}