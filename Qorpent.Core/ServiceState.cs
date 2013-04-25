using System;
using System.Diagnostics;

namespace Qorpent {
    /// <summary>
    ///     State of the Service
    /// </summary>
    public static class ServiceState {
        /// <summary>
        ///     Count of current work handlers
        /// </summary>
        public static Int64 TotalServerReloads { get; set; }

        /// <summary>
        ///     Время, затраченное процессором в минутах
        /// </summary>
        public static Int64 CpuMinutes {
            get {return Process.GetCurrentProcess().TotalProcessorTime.Ticks;}
        }

        /// <summary>
        ///     Count of current opened forms
        /// </summary>
        public static Int64 TotalQueriesHandled { get; set; }

        /// <summary>
        ///     Count of current sessions
        /// </summary>
        public static Int64 TotalSessionsHandled { get; set; }

        /// <summary>
        ///     Увеличение счётчика общего количества обработанных запросов
        /// </summary>
        public static void TotalQueriesHandledIncrease() {
            TotalQueriesHandled++;
        }

        /// <summary>
        ///     Увеличение счётчика общего количества обработанных сессий
        /// </summary>
        public static void TotalSessionsHandledIncrease() {
            TotalSessionsHandled++;
        }

        /// <summary>
        ///     Увеличение счётчика общего количества обработанных сессий
        /// </summary>
        public static void TotalServerReloadsIncrease() {
            TotalServerReloads++;
        }
    }
}
