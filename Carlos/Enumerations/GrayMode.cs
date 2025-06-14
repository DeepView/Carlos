using System;
using System.Collections.Generic;
using System.Text;

namespace Carlos.Enumerations
{
    public enum GrayMode : int
    {
        /// <summary>
        /// 最大值法灰度模式，取RGB三个分量中的最大值作为灰度值：[Gray = \max(R,G,B)]该方法生成的图像亮度较高，可能导致细节丢失。
        /// </summary>
        [EnumerationDescription("Max")]
        Max = 0,
        /// <summary>
        /// 算术平均值灰度模式，采用算术平均值计算灰度值：[Gray = \frac{R+G+B}{3}]这种方法生成的图像亮度较柔和，但可能弱化色彩对比度。
        /// </summary>
        [EnumerationDescription("Avg")]
        Avg = 1,
        /// <summary>
        /// 加权平均值灰度模式，采用加权平均值计算灰度值，通常使用公式：[Gray = 0.299R + 0.587G + 0.114B]这种方法更符合人眼对颜色的感知，生成的图像亮度和对比度较为平衡。2020年的实验数据显示，相较平均值法，加权平均法的边缘保留率提升约18%。
        /// </summary>
        [EnumerationDescription("WAvgEyes")]
        WAvgEyes = 2,
        /// <summary>
        /// 自定义权重的加权平均值灰度模式，允许用户自定义RGB三个分量的权重来计算灰度值。用户可以根据具体需求调整权重，以达到最佳的视觉效果或特定应用需求。
        /// </summary>
        [EnumerationDescription("WAvgCustomize")]
        WAvgCustomize = 3
    }
}
