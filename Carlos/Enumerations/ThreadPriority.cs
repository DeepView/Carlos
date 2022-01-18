namespace Carlos.Enumerations
{
    /// <summary>
    /// 线程优先级枚举，这个枚举用来表示线程在运行的时候的优先级。
    /// </summary>
    public enum ThreadPriority : int
    {
        /// <summary>
        /// 开始后台处理模式。
        /// </summary>
        [EnumerationDescription("开始后台处理模式")]
        BackgroundBegin = 65536,
        /// <summary>
        /// 终止后台处理模式。
        /// </summary>
        [EnumerationDescription("终止后台处理模式")]
        BackgroundEnd = 131072,
        /// <summary>
        /// 略微高于正常。
        /// </summary>
        [EnumerationDescription("高于正常")]
        AboveNormal = 1,
        /// <summary>
        /// 略微低于正常。
        /// </summary>
        [EnumerationDescription("低于正常")]
        BelowNormal = -1,
        /// <summary>
        /// 极高的优先级。
        /// </summary>
        [EnumerationDescription("最高优先级")]
        Highest = 2,
        /// <summary>
        /// 空闲时运行。
        /// </summary>
        [EnumerationDescription("空闲时运行")]
        Idle = -15,
        /// <summary>
        /// 最低的优先级。
        /// </summary>
        [EnumerationDescription("最低优先级")]
        Lowest = -2,
        /// <summary>
        /// 正常的优先级。
        /// </summary>
        [EnumerationDescription("常规")]
        Normal = 0,
        /// <summary>
        /// 基于时间关键性的优先级。
        /// </summary>
        [EnumerationDescription("基于时间关键性")]
        TimeCritical = 15
    }
}
