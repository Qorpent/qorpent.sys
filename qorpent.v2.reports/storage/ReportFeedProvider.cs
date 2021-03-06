using qorpent.v2.reports.model;
using Qorpent;
using Qorpent.IoC;

namespace qorpent.v2.reports.storage {
    [ContainerComponent(Lifestyle = Lifestyle.Singleton, ServiceType = typeof(IReportFeedProvider), Name = "qorpent.reports.reportfeedprovider")]
    public class ReportFeedProvider : CachedSourceService<IReportFeedSource, IReportFeed, IReportFeedLease>, IReportFeedProvider
    {
    }
}