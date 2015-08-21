using System.Threading.Tasks;
using qorpent.v2.reports.core;

namespace qorpent.v2.reports {
    public interface IReportService {
        Task<IReportContext> Execute(IReportRequest request);
    }
}