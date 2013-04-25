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

        /// <summary>
        ///     Количество рестартов - 20 баллов
        /// </summary>
        public const Int64 POINT_PER_RELOAD = 20;

        /// <summary>
        ///     Общее кол-во обработанных сессий - 10 баллов за каждую
        /// </summary>
        public const Int64 POINT_PEP_HANDLED_SESSION = 10;
    }
}
