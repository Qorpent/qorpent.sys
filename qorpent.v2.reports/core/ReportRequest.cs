using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using qorpent.v2.console;
using Qorpent;
using Qorpent.Core.Tests.Experiments;
using Qorpent.Experiments;
using Qorpent.IoC;
using Qorpent.IO.Http;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.Extensions.Html;

namespace qorpent.v2.reports.core {
    [ContainerComponent(Lifestyle.Transient,ServiceType = typeof(IReportRequest),Name="qorpent.reports.request")]
    public class ReportRequest:IReportRequest,IJsonSerializable {
        private IList<string> _flags;

        public ReportRequest() {
            
        }

        public ReportRequest(WebContext context) {
            WebContext = context;
            Initialize();
        }

        public ReportRequest(IConsoleContext context) {
            ConsoleContext = context;
            Initialize();
        }
        public string Id { get; set; }
        public bool DataOnly { get; set; }
        public string Format { get; set; }
        public object Query { get; set; }
        public object Json { get; set; }

        public IList<string> Flags
        {
            get { return _flags??(_flags=new List<string>()); }
            set { _flags = value; }
        }

        public IScope Scope { get; set; }
        public WebContext WebContext { get; set; }
        public IConsoleContext ConsoleContext { get; set; }
        public IIdentity User { get; set; }

        public void Initialize() {
            if (null != WebContext) {
                InitializeFromWebContext();
            }else if (null != ConsoleContext) {
                InitializeFromConsoleContext();
            }
            Flags = Flags.Distinct().ToList();
        }

        private void InitializeFromConsoleContext() {
            var xml = ConsoleContext.GetBxl();
            Id = xml.ChooseAttr("id", "code");
            DataOnly = xml.ResolveFlag("dataonly");
            Format = xml.Attr("format");
            Query = xml.AttrOrValue("query");
            var j = xml.Attr("json");
            if (!string.IsNullOrWhiteSpace(j)) {
                Json = j.jsonify();
            }
            var flags = xml.Attr("flags").SmartSplit();
            foreach (var fl in flags) {
                Flags.Add(fl);

            }
        }

        private void InitializeFromWebContext() {
            var p = RequestParameters.Create(WebContext);
            User = WebContext.User.Identity;
            Id = p.Get("id");
            DataOnly = p.Get("dataonly").ToBool();
            Format = p.Get("format");
            Query = p.Get("query");
            var flags = p.Get("flags").SmartSplit();
               foreach (var f in flags) {
                    Flags.Add(f);
                }
            
            if (null != p.Json) {
                Json = p.Json;
                var jq = p.Json.get("query");
                if (null != jq) {
                    Query = jq;
                }
                var jflags = p.Json.get("flags");
                if (null != jflags) {
                    if (jflags is string) {
                        foreach (var f in ((string) jflags).SmartSplit()) {
                            Flags.Add(f);
                        }
                    }
                    else {
                        foreach (string f in jflags as IEnumerable) {
                            Flags.Add(f);
                        }
                    }
                }
            }
        }

        public void WriteAsJson(TextWriter output, string mode, ISerializationAnnotator annotator, bool pretty = false, int level = 0) {
            var jw = new JsonWriter(output, pretty:pretty, level:level);
            jw.OpenObject();
            jw.WriteProperty("id",Id);
            jw.WriteProperty("dataonly", DataOnly);
            jw.WriteProperty("format", Format);
            jw.WriteProperty("query", Query);
            jw.WriteProperty("json",this.Json);
            jw.CloseObject();
        }
    }
}