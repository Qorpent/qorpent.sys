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
            age += Convert.ToInt64((ServiceState.CpuTime.TotalMinutes) * (ClusterNodeLoadPoints.POINT_PER_CPU_MINUTE));

            var ageInfo = new Dictionary<string, object> {
                {"TotalQueriesHandled", (ServiceState.TotalQueriesHandled) * (ClusterNodeLoadPoints.POINT_PER_HANDLED_QUERY)},
                {"TotalCpuTime", ServiceState.CpuTime},
                {"Age", age}
            };

            return ageInfo;
        }
    }
}
