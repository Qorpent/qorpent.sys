using System;

namespace Qorpent.Scaffolding.Application {
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum AppMenuType {
        /// <summary>
        /// 
        /// </summary>
        None = 0,
        /// <summary>
        /// 
        /// </summary>
        Inline = 1,
        /// <summary>
        /// 
        /// </summary>
        Ribbon = 2,
        /// <summary>
        /// 
        /// </summary>
        DropDown = 4,
        /// <summary>
        /// 
        /// </summary>
        Default = Inline
    }
}