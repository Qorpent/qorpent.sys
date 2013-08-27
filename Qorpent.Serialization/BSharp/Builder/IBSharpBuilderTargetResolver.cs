using System.Collections.Generic;

namespace Qorpent.BSharp.Builder {
    /// <summary>
    /// 
    /// </summary>
    public interface IBSharpBuilderTargetResolver {
        /// <summary>
        ///     Проект
        /// </summary>
        IBSharpProject Project { get; set; }

        /// <summary>
        ///     Резольвинг  с учётом инклудов и эксклудов
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> Resolve();
    }
}