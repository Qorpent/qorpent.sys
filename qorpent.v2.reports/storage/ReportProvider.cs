﻿using qorpent.v2.reports.model;
using Qorpent;
using Qorpent.IoC;

namespace qorpent.v2.reports.storage {
    [ContainerComponent(Lifestyle = Lifestyle.Singleton,ServiceType = typeof(IReportProvider),Name="qorpent.reports.reportprovider")]
    public class ReportProvider : CachedSourceService<IReportSource,IReport,IReportLease>, IReportProvider
    {
    }
}