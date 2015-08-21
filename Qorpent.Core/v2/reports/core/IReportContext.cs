using System;
using System.Collections.Generic;
using qorpent.v2.reports.model;
using Qorpent;
using Qorpent.Log.NewLog;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.reports.core {
    public interface IReportContext {
        IScope Scope { get; set; }
        IReport Report { get; set; }
        IReportRequest Request { get; set; }
        IList<IReportAgent> Agents { get; }
        ILoggy Log { get; set; }
        Exception Error { get; set; }
        IDictionary<string,object> Data { get; }
        void SetHeader(string name, string value);
        void Write(object data);
        void Finish(Exception error = null);
    }

    public static class ReportContextExtensions {
        public static bool IsSet(this IReportContext context, string flag) {
            if (context.Request.Flags.Contains(flag)) return true;
            if (null != context.Scope) {
                if (context.Scope.ContainsKey(flag)) {
                    return context.Scope.Get(flag).ToBool();
                }
            }
            return false;
        }
    }
}