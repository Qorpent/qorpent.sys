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
            age += (ServiceState.TotalServerReloads) * (ClusterNodeLoadPoints.POINT_PER_RELOAD);
            age += (ServiceState.TotalSessionsHandled) * (ClusterNodeLoadPoints.POINT_PEP_HANDLED_SESSION);

            var ageInfo = new Dictionary<string, Int64> {
                {"TotalQueriesHandled", (ServiceState.TotalQueriesHandled) * (ClusterNodeLoadPoints.POINT_PER_HANDLED_QUERY)},
                {"CpuTime", (ServiceState.CpuMinutes) * (ClusterNodeLoadPoints.POINT_PER_CPU_MINUTE)},
                {"TotalServerReloads", (ServiceState.TotalServerReloads) * (ClusterNodeLoadPoints.POINT_PER_RELOAD)},
                {"TotalSessionsHandled", (ServiceState.TotalSessionsHandled) * (ClusterNodeLoadPoints.POINT_PEP_HANDLED_SESSION)},
                {"Summary", age}
            };

            return ageInfo;
        }
    }
}
