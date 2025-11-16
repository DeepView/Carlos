using System;
using System.Timers;
namespace Carlos.Extends
{
    /// <summary>
    /// 一个倒计时类，计时精度为毫秒（millisecond）。
    /// </summary>
    /// <remarks>
    /// 构造函数，初始化倒计时的起始时间。
    /// </remarks>
    /// <param name="startMilliseconds">指定的起始时间，单位为毫秒（millisecond）。</param>
    public class Countdown(int startMilliseconds)
    {
        /// <summary>
        /// 获取或设置倒计时的起始时间，单位为毫秒（millisecond）。
        /// </summary>
        public int StartMilliseconds { get; set; } = startMilliseconds;
        /// <summary>
        /// 获取或设置倒计时剩余的时间，单位为毫秒（millisecond）。
        /// </summary>
        public int RemainingMilliseconds { get; set; } = startMilliseconds;
        /// <summary>
        /// 获取或设置倒计时是否正在运行。
        /// </summary>
        public bool IsRunning { get; set; } = false;
        /// <summary>
        /// 获取或设置倒计时是否已暂停。
        /// </summary>
        public bool IsPaused { get; set; } = false;
        /// <summary>
        /// 当倒计时结束时触发的事件。
        /// </summary>
        public event EventHandler<EventArgs> OnCountdownFinished;
        /// <summary>
        /// 当倒计时暂停时触发的事件。
        /// </summary>
        public event EventHandler<EventArgs> OnCountdownPaused;
        /// <summary>
        /// 当倒计时恢复时触发的事件。
        /// </summary>
        public event EventHandler<EventArgs> OnCountdownResumed;
        /// <summary>
        /// 当倒计时开始时触发的事件。
        /// </summary>
        public event EventHandler<EventArgs> OnCountdownStarted;
        /// <summary>
        /// 当倒计时停止时触发的事件。
        /// </summary>
        public event EventHandler<EventArgs> OnCountdownStopped;
        /// <summary>
        /// 开始倒计时。
        /// </summary>
        public void Start()
        {
            if (IsRunning) return;
            IsRunning = true;
            IsPaused = false;
            RemainingMilliseconds = StartMilliseconds;
            OnCountdownStarted?.Invoke(this, EventArgs.Empty);
            Timer timer = new Timer(1);
            timer.Elapsed += (s, e) =>
            {
                if (IsPaused) return;
                RemainingMilliseconds -= 1;
                if (RemainingMilliseconds <= 0)
                {
                    RemainingMilliseconds = 0;
                    IsRunning = false;
                    timer.Stop();
                    OnCountdownFinished?.Invoke(this, EventArgs.Empty);
                }
            };
            timer.Start();
        }
        /// <summary>
        /// 暂停倒计时。
        /// </summary>
        public void Pause()
        {
            if (!IsRunning || IsPaused) return;
            IsPaused = true;
            OnCountdownPaused?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// 恢复或者继续倒计时。
        /// </summary>
        public void Resume()
        {
            if (!IsRunning || !IsPaused) return;
            IsPaused = false;
            OnCountdownResumed?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// 停止倒计时并重置剩余时间。
        /// </summary>
        public void Stop()
        {
            if (!IsRunning) return;
            IsRunning = false;
            IsPaused = false;
            RemainingMilliseconds = StartMilliseconds;
            OnCountdownStopped?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// 获取倒计时的字符串表示形式，格式为 "hh:mm:ss.fff"。
        /// </summary>
        /// <returns>该操作会返回倒计时的剩余时间的字符串表达形式。</returns>
        public override string ToString()
        {
            TimeSpan time = TimeSpan.FromMilliseconds(RemainingMilliseconds);
            return time.ToString(@"hh\:mm\:ss\.fff");
        }
    }
}
