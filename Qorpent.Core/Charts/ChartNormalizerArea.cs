using System;

namespace Qorpent.Charts {
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum ChartNormalizerArea {
        /// <summary>
        /// 
        /// </summary>
        Default = 0,
        /// <summary>
        ///     Хер знает
        /// </summary>
        Unknown = Default,
        /// <summary>
        /// 
        /// </summary>
        Global = 1,
        /// <summary>
        /// 
        /// </summary>
        XScale = 2,
        /// <summary>
        /// 
        /// </summary>
        YScale = 4,
        /// <summary>
        /// 
        /// </summary>
        YScaleSecond = 8,
        /// <summary>
        /// 
        /// </summary>
        Labels = 16,
        /// <summary>
        /// 
        /// </summary>
        Colors = 32,
        /// <summary>
        /// 
        /// </summary>
        Formatting = 64,
        /// <summary>
        /// 
        /// </summary>
        Anchors = 128
    }
}
