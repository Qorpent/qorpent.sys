using System;

namespace Qorpent {
    /// <summary>
    ///     State of the Service
    /// </summary>
    public static class ServiceState {
        private static DateTime _startTime;

        /// <summary>
        ///     Count of current work handlers
        /// </summary>
        public static Int64 CurrentHandlers { get; set; }

        /// <summary>
        ///     Count of current IO Operations
        /// </summary>
        public static Int64 CurrentIoOperations { get; set; }

        /// <summary>
        ///     Count of current sessions
        /// </summary>
        public static Int64 CurrentSessions { get; set; }

        /// <summary>
        ///     Uptime of the service
        /// </summary>
        public static TimeSpan Uptime {
            get {
                return DateTime.Now - _startTime;
            }
        }

        /// <summary>
        ///     Default constructor
        /// </summary>
        static ServiceState() {
            _startTime = DateTime.Now;
        }
    }
}
