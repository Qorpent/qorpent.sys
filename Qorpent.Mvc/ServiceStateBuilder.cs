using System;
using System.Collections.Generic;

namespace Qorpent.Mvc {
    /// <summary>
    /// 
    /// </summary>
    public  class ServiceStateBuilder {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static object Build() {
            Int64 age = 0;

            age += (ServiceState.TotalQueriesHandled) * (ClusterNodeLoadPoints.POINT_PER_HANDLED_QUERY);
            age += (ServiceState.CpuMinutes) * (ClusterNodeLoadPoints.POINT_PER_CPU_MINUTE);

            var ageInfo = new Dictionary<string, Int64> {
                {"TotalQueriesHandled", (ServiceState.TotalQueriesHandled) * (ClusterNodeLoadPoints.POINT_PER_HANDLED_QUERY)},
                {"CpuTime", (ServiceState.CpuMinutes) * (ClusterNodeLoadPoints.POINT_PER_CPU_MINUTE)},
                {"Summary", age}
            };

            return ageInfo;
        }
    }
}
