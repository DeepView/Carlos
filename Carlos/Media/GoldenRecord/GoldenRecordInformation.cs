using System;
using System.Collections.Generic;
using System.Text;

namespace Carlos.Media.GoldenRecord
{
    /// <summary>
    /// 黄金唱片信息结构体，包含频率范围和分辨率等信息。
    /// </summary>
    public struct  GoldenRecordInformation
    {
        /// <summary>
        /// 声音的最小频率。
        /// </summary>
        public int MinimumFreq { get; set; }
        /// <summary>
        /// 声音的最大频率。
        /// </summary>
        public int MaximumFreq { get; set; }
        /// <summary>
        /// 声音的频率分辨率，表示每个频率点之间的间隔。
        /// </summary>
        public int FreqResolution { get; set; }
    }
}
