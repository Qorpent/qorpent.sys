using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using qorpent.v2.reports.storage;
using qorpent.v2.security.authorization;
using Qorpent;
using Qorpent.Experiments;
using Qorpent.IoC;
using Qorpent.Log.NewLog;

namespace qorpent.v2.reports.core
{
    [ContainerComponent(ServiceType = typeof(IReportService),Lifestyle = Lifestyle.Singleton,Name="qorpent.report.service")]
    public class ReportService:InitializeAbleService,IReportService
    {
        [Inject]
        public IReportProvider Storage { get; set; }
        [Inject]
        public IRoleResolverService Roles { get; set; }

        public async Task<IReportContext> Execute(IReportRequest request) {
            Logg.Debug(new {op="start",r=request}.stringify());
            var context = InitializeContext(request);
            try {
                var scope = new Scope();
                await ExecutePhase(context, ReportPhase.Init,scope);
                await ExecutePhase(context, ReportPhase.Data, scope);
                await ExecutePhase(context, ReportPhase.Prepare, scope);
                if (request.DataOnly) {
                    if (null != context.Data) {
                        context.SetHeader("Content-Type","application/json; charset=utf-8");
                        context.Write(context.Data.stringify());
                    }
                }
                else {
                    await ExecutePhase(context, ReportPhase.Render, scope);
                }
                await ExecutePhase(context, ReportPhase.Finalize, scope);
            }
            catch (Exception e) {
                Logg.Error(e);
                context.Error = e;
                context.Finish(e);
            }
            finally {
                Logg.Trace(new { op = "success", r = request }.stringify());
                context.Finish();
            }
            return context;
        }

        private async Task ExecutePhase(IReportContext context, ReportPhase phase,IScope scope) {
            Logg.Debug(new {op="start phase",phase});
            var agents = context.Agents.Where(_ => _.Phase.HasFlag(phase)).OrderBy(_ => _.Idx).GroupBy(_ => _.Idx).ToArray();
            foreach (var grp in agents) {
                if (grp.Count() == 1) {
                    await grp.First().Execute(context, phase, scope);
                }
                else if (grp.All(_ => _.Parallel)) {
                    var waitgroup = new List<Task>();
                    foreach (var agent in grp) {
                        waitgroup.Add(agent.Execute(context, phase, scope));
                    }
                    Task.WaitAll(waitgroup.ToArray());
                }
                else {
                    foreach (var agent in grp) {
                        await agent.Execute(context, phase, scope);
                    }
                }

            }
            Logg.Debug(new { op = "end phase", phase });
        }

        public IReportContext InitializeContext(IReportRequest request) {
            var context = ResolveService<IReportContext>("", request);
            context.Log = context.Log ?? this.Logg;
            var report = Storage.Get(request.Id);
            if (null == report) {
                throw new Exception("cannot find report " + request.Id);
            }
            if (null != request.User) {
                if (!string.IsNullOrWhiteSpace(report.Role)) {
                    if (!Roles.IsInRole(request.User, report.Role)) {
                        throw new Exception("not authorized access to report "+report.Id);
                    }
                }
            }
            context.Report = report;
            foreach (var agentdef in report.Agents) {
                var agent = ResolveService<IReportAgent>(agentdef.Name);
                if (null == agent) {
                    throw new Exception("cannot find agent "+agentdef.Name);
                }
                agent.Initialize(agentdef);
                if (agent.IsMatch(context)) {
                    context.Agents.Add(agent);
                }
            }
            return context;
        }
    }
}
