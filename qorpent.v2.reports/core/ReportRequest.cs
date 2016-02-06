using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using qorpent.v2.console;
using qorpent.v2.reports.model;
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
            Parameters = new Dictionary<string, object>();
        }

        public IReport PreparedReport { get; set; }
        public Stream Stream { get; set; }

        public ReportRequest(WebContext context) :this(){
            WebContext = context;
            Initialize();
        }

        public ReportRequest(IConsoleContext context):this() {
            ConsoleContext = context;
            Initialize();
        }
        public string Id { get; set; }
        public bool DataOnly { get; set; }
        public string Format { get; set; }
        public object Query { get; set; }
        public object Json { get; set; }
        public bool Standalone { get; set; }
        public IDictionary<string,object> Parameters { get; set; } 
        public bool NoFinalizeOnError { get; set; }

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
            Standalone = xml.ResolveFlag("standalone");
            var j = xml.Attr("json");
            foreach (var attribute in xml.Attributes()) {
                if (attribute.Name.LocalName.StartsWith("p.")) {
                    Parameters[attribute.Name.LocalName.Substring(2)] = attribute.Value;
                }
            }
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
            Standalone = p.Get("standalone").ToBool();
            var flags = p.Get("flags").SmartSplit();
               foreach (var f in flags) {
                    Flags.Add(f);
                }
            foreach (var parameter in p.GetParameters()) {
                if (parameter.Key.StartsWith("p.")) {
                    Parameters[parameter.Key.Substring(2)] = parameter.Value;
                }
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

                var parameters = p.Json.map("parameters");
                if (null != parameters) {
                    foreach (var parameter in parameters) {
                        Parameters[parameter.Key] = parameter.Value;
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
            jw.WriteProperty("standalone",Standalone);
            jw.WriteProperty("parameters",this.Parameters);
            jw.WriteProperty("flags",this.Flags);
            jw.CloseObject();
        }
    }
}