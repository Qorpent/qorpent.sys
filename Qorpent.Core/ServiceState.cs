using System;
using System.Diagnostics;

namespace Qorpent {
    /// <summary>
    ///     State of the Service
    /// </summary>
    public static class ServiceState {
        /// <summary>
        ///     Время, затраченное процессором в минутах
        /// </summary>
        public static Int64 CpuMinutes {
            get {return Convert.ToInt64(Process.GetCurrentProcess().TotalProcessorTime.Minutes);}
        }

        /// <summary>
        ///     Count of current opened forms
        /// </summary>
        public static Int64 TotalQueriesHandled { get; set; }

        /// <summary>
        ///     Увеличение счётчика общего количества обработанных запросов
        /// </summary>
        public static void TotalQueriesHandledIncrease() {
            TotalQueriesHandled++;
        }
    }
}
