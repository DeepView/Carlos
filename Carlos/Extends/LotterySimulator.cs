using System;
namespace Carlos.Extends
{
    /// <summary>
    /// 表示一个抽奖模拟器。
    /// </summary>
    public class LotterySimulator
    {
        /// <summary>
        /// 获取或设置当前实例的初始（或者基本）抽奖概率。
        /// </summary>
        public double InitProbability { get; set; }
        /// <summary>
        /// 获取当前实例的实时抽奖概率。
        /// </summary>
        public double RealtimeProbability { get; private set; }
        /// <summary>
        /// 获取或设置当前实例在抽奖未果之后的概率增量，用于提升实时抽奖概率。这个增量是每次增加的概率，而非每次增加概率的基础概率百分比。
        /// </summary>
        public double Increment { get; set; }
        /// <summary>
        /// 获取或设置当前实例在连续多次抽奖未果之后的必然中奖次数，即中奖的保底次数。
        /// </summary>
        public uint InevitableFrequency { get; set; }
        /// <summary>
        /// 获取当前实例的实时抽奖次数，这个次数会在中奖或者重置抽奖实例之后归零。
        /// </summary>
        public uint RealtimeFrequency { get; private set; }
        /// <summary>
        /// 构造函数，创建一个指定基础概率，概率增量和保底次数的抽奖实例。
        /// </summary>
        /// <param name="initProbability">指定的基础概率。</param>
        /// <param name="increment">指定的概率增量，这个增量是每次增加的概率，而非每次增加概率的基础概率百分比。换言之，这个概率是通过直接加法追加上去的，而非乘以基础概率或者实时概率然后追加。</param>
        /// <param name="inevitableFrequency">指定的保底次数。</param>
        public LotterySimulator(double initProbability, double increment, uint inevitableFrequency)
        {
            InitProbability = initProbability;
            RealtimeProbability = initProbability;
            Increment = increment;
            InevitableFrequency = inevitableFrequency;
            RealtimeFrequency = 0;
        }
        /// <summary>
        /// 重置抽奖实例，这个操作会清零实时抽奖次数，重置实时抽奖概率。
        /// </summary>
        public void Reset()
        {
            RealtimeProbability = InitProbability;
            RealtimeFrequency = 0;
        }
        /// <summary>
        /// 开始抽奖，但抽奖成功后并不会重置实例。
        /// </summary>
        /// <returns>该操作会返回一个Boolean值，如果这个值为true则表示抽奖成功，否则是抽奖未果。</returns>
        public bool Bingo() => Bingo(false);
        /// <summary>
        /// 开始抽奖，并指定是否在抽奖成功后重置实例。
        /// </summary>
        /// <param name="isResetAfterBingo">一个Boolean值，用于指示在抽奖成功后是否重置当前实例。</param>
        /// <returns>该操作会返回一个Boolean值，如果这个值为true则表示抽奖成功，否则是抽奖未果。</returns>
        /// <remarks>值得注意的是，这个操作在传递false参数值的前提下，并抽奖成功，会根据实时抽奖次数是否达到保底次数，来决定是否重置实例，一旦达到了保底次数，则无论如何都会重置实时抽奖次数，且直接返回true。</remarks>
        public bool Bingo(bool isResetAfterBingo)
        {
            RealtimeFrequency++;
            if (RealtimeProbability >= InevitableFrequency)
            {
                Reset();
                return true;
            }
            else
            {
                int range = 100000;
                Random rnd = new Random();
                int bingoRes = rnd.Next(range);
                if (bingoRes <= range * RealtimeProbability)
                {
                    if (isResetAfterBingo) Reset();
                    else RealtimeProbability = InitProbability;
                    return true;
                }
                else
                {
                    RealtimeProbability += Increment;
                    return false;
                }
            }
        }
    }
}
