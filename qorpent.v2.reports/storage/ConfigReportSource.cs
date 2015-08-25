using System.Linq;
using System.Text.RegularExpressions;
using qorpent.v2.model;
using qorpent.v2.query;
using qorpent.v2.reports.model;
using Qorpent;
using Qorpent.BSharp;
using Qorpent.Experiments;
using Qorpent.IoC;

namespace qorpent.v2.reports.storage {
    [ContainerComponent(Lifestyle = Lifestyle.Singleton, ServiceType = typeof(IReportSource), Name = "qorpent.reports.reportconfigsource")]
    public class ConfigReportSource : ObjectSourceBase<IReport>, IReportSource {
        private IBSharpContext _context;
        public IBSharpContext Context
        {
            get
            {
                return _context ?? (_context = ConfigProvider.GetContext());
            }
            set { _context = value; }
        }

        public override IReport Get(string id, IScope scope = null) {
            var cls = Context.Get(id);
            if (null == cls) {
                cls = Context.ResolveAll("report").FirstOrDefault(_ => _.Name == id);
            }
            if (null == cls) {
                return null;
            }
            return Item.Create<Report>(cls.Compiled);
        }

        public override SearchResult<IReport> Search(object query = null, IScope scope = null) {
            var reports = Context.ResolveAll("report").Where(_=>IsMatch(_, query as string ?? query.jsonify().str("query"))).ToArray();
            return new SearchResult<IReport> {
                Total = reports.Length,
                Size = reports.Length,
                Items = reports.Select(_=>Item.Create<Report>(_.Compiled)).OrderBy(_=>_.Idx).ToArray()
            };
        }

        private bool IsMatch(IBSharpClass cls, string query) {
            var str = query as string ?? query.jsonify().str("query");
            if (string.IsNullOrWhiteSpace(str)) {
                return true;
            }
            if (Regex.IsMatch(cls.FullName, str)) {
                return true;
                    
            }
            return false;
        }
    }
}