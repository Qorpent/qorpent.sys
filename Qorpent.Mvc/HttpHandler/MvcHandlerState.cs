using System;

namespace Qorpent.Mvc.HttpHandler {
    /// <summary>
    ///     Class represents state of a MvcHandler instance
    /// </summary>
    public class MvcHandlerState {
        /// <summary>
        ///     MvcHandler start time
        /// </summary>
        public DateTime StartTime {
            get;
            private set;
        }

        /// <summary>
        ///     Default constructor
        /// </summary>
        public MvcHandlerState() {
            StartTime = DateTime.Now;
        }

        /// <summary>
        ///     Return the work time
        /// </summary>
        /// <returns>work time</returns>
        public TimeSpan GetWorkTime() {
            return StartTime - DateTime.Now;
        }

        /// <summary>
        ///     Count of requests
        /// </summary>
        /// <returns></returns>
        public Int64 RequestsCount {
            get;
            private set;
        }

        /// <summary>
        ///     Count of failed requests
        /// </summary>
        public Int64 FailedRequestsCount {
            get;
            private set;
        }

        /// <summary>
        ///     Increate count of requests
        /// </summary>
        public void RequestsCountIncrease() {
            RequestsCount++;
        }

        /// <summary>
        /// Increate count of failed requests
        /// </summary>
        public void FailedRequestsCountIncrease() {
            FailedRequestsCount++;
        }

        /// <summary>
        ///     Match all statistics
        /// </summary>
        public void Checkout() {
            
        }
    }
}
