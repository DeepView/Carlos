namespace Carlos.Media
{
    /// <summary>
    /// 定义一个适用于多媒体播放器的通用性接口。
    /// </summary>
    public interface IPlayer
    {
        /// <summary>
        /// 播放或者继续播放当前的多媒体。
        /// </summary>
        void Play();
        /// <summary>
        /// 暂停播放当前的多媒体。
        /// </summary>
        void Pause();
        /// <summary>
        /// 停止播放当前的多媒体。
        /// </summary>
        void Stop();
    }
}
