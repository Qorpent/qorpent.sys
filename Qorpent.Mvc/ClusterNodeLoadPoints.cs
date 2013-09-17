using System;

namespace Qorpent.Mvc {
    /// <summary>
    ///     Node load static points
    /// </summary>
    public static class ClusterNodeLoadPoints {
        /// <summary>
        ///     Общее кол-во веб-запросов - 1 балл
        /// </summary>
        public const Int64 POINT_PER_HANDLED_QUERY = 1;

        /// <summary>
        ///     Общее процессорное время - 1 балл/минута
        /// </summary>
        public const Int64 POINT_PER_CPU_MINUTE = 1;
    }
}
