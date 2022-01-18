namespace Carlos.Enumerations
{
    /// <summary>
    /// 进程优先级枚举，这个枚举用来表示进程在执行态的优先级。
    /// </summary>
    public enum ProcessPriority : int
    {
        /// <summary>
        /// 指定进程的优先级在 Normal 之上，但在 High 之下。
        /// </summary>
        [EnumerationDescription("次高优先级")]
        AboveNormal = 32768,
        /// <summary>
        /// 指定进程的优先级在 Idle 之上，但在 Normal 之下。
        /// </summary>
        [EnumerationDescription("低优先级")]
        BelowNormal = 16384,
        /// <summary>
        /// 指定进程执行必须立即执行的时间关键任务。
        /// </summary>
        [EnumerationDescription("高优先级")]
        High = 128,
        /// <summary>
        /// 指定此进程的线程只能在系统空闲时运行。
        /// </summary>
        [EnumerationDescription("空闲时运行")]
        Idle = 64,
        /// <summary>
        /// 指定进程没有特殊的安排需求。
        /// </summary>
        [EnumerationDescription("常规")]
        Normal = 32,
        /// <summary>
        /// 指定进程拥有可能的最高优先级。
        /// </summary>
        [EnumerationDescription("实时")]
        RealTime = 256
    }
}
