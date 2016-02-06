using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using qorpent.v2.reports.core;
using qorpent.v2.reports.model;
using qorpent.v2.reports.storage;
using Qorpent;
using Qorpent.Experiments;
using Qorpent.IoC;

namespace qorpent.v2.reports.agents {
    [ContainerComponent(Lifestyle.Transient, ServiceType = typeof(IReportAgent), Name = "qorpent.reports.template.render")]
    public class TemplateRenderAgent : ReportAgentBase {
        [Inject]
        public IRenderProvider Renders { get; set; }


        public TemplateRenderAgent() {
            Phase = ReportPhase.Render;
            ; MimeType = "text/html";
        }

        public string MimeType { get; set; }
        public string DocHeader { get; set; }
        public string DocFooter { get; set; }
        public string Footer { get; set; }
        public string Header { get; set; }
        public string Content { get; set; }
        public string Item { get; set; }
        public string BeforeItem { get; set; }
        public string AfterItem { get; set; }

        public bool BuildHtml { get; set; }



        public override void Initialize(IReportAgentDefinition definition) {
            base.Initialize(definition);
            var def = definition.Definition;
            MimeType = def.str("mime");
            DocHeader = def.str("docheader");
            DocFooter = def.str("docfooter");
            Footer = def.str("footer");
            Header = def.str("header");
            Footer = def.str("footer");
            Content = def.str("content");
            Item = def.str("item");
            BeforeItem = def.str("beforeitem");
            AfterItem = def.str("afteritem");
            BuildHtml = def.bul("buildhtml");
            HtmlClass = def.str("htmlclass");
            BodyClass = def.str("bodyclass");
        }

        public string HtmlClass { get; set; }
        public string BodyClass { get; set; }
        public override async Task Execute(IReportContext context, ReportPhase phase, IScope scope = null) {
            SetupMime(context);
            if (BuildHtml) {
                DoBuildHtml(context, scope);
            } else {
                DoStreamRender(context, scope);
            }

        }

        class ItemXml {
            public XElement Before { get; set; }
            public XElement Item { get; set; }
            public XElement After { get; set; }
        }

        class HtmlConfig {
            public XElement DocHeader { get; set; }
            public XElement DocFooter { get; set; }
            public XElement Header { get; set; }
            public XElement Footer { get; set; }
            public XElement Content { get; set; }
            public IList<ItemXml> Items { get; set; }
        }

        private void DoBuildHtml(IReportContext context, IScope scope) {
            var htmlconfig = new HtmlConfig();
            bool standalone = context.Request.Standalone;
            if (standalone) {
                htmlconfig.DocHeader = RenderXml(context, scope, DocHeader, null, "DocHeader");
            }
            htmlconfig.Header = RenderXml(context, scope, Header, null, "Header");
            htmlconfig.Content = RenderXml(context, scope, Content, null, "Content");
            var items = context.Data.arr("items");
            IList<ItemXml> xitems = new List<ItemXml>();
            if (null != items) {
                var i = 0;
                foreach (var item in items) {
                    var itemScope = new Scope(scope);
                    itemScope["_item"] = item;
                    itemScope["_idx"] = i;
                    var xitem = new ItemXml();
                    xitem.Before = RenderXml(context, itemScope, BeforeItem, item, "BeforeItem");
                    xitem.Item = RenderXml(context, itemScope, Item, item, "Item");
                    xitem.After = RenderXml(context, itemScope, AfterItem, item, "AfterItem");
                    if (null != (xitem.Before ?? xitem.After ?? xitem.Before)) {
                        xitems.Add(xitem);
                    }
                    i++;
                }
            }
            htmlconfig.Items = xitems;
            htmlconfig.Footer = RenderXml(context, scope, Footer, null, "Footer");
            XElement docfooter = null;
            if (standalone) {
                htmlconfig.DocFooter = RenderXml(context, scope, DocFooter, null, "DocFooter");
            }
            var xmlhtml = CompileHtml(context, scope, htmlconfig);
            context.Write(xmlhtml.ToString());
        }

        private XElement CompileHtml(IReportContext context, IScope scope, HtmlConfig htmlconfig) {
            bool standalone = context.Request.Standalone;
            XElement root = null;
            XElement content = XElement.Parse("<section><header></header><main></main><footer></footer></section>");
            if (standalone) {
                root = XElement.Parse("<html class='" + HtmlClass + $"'><head><meta charset='UTF-8' /></head><body class='{BodyClass}'></body></html>");
                root.Element("body").Add(content);
                if (null != htmlconfig.DocHeader) {
                    root.Element("head").Add(htmlconfig.DocHeader.Elements());
                }

            } else {
                root = content;
            }

            if (null != htmlconfig.Header) {
                content.Element("header").ReplaceWith(htmlconfig.Header);
            }

            var main = content.Element("main");
            if (null != htmlconfig.Content) {
                main.ReplaceWith(htmlconfig.Content);
                main = content.Element("main");
            }

            if (null != htmlconfig.Items && 0 != htmlconfig.Items.Count) {
                foreach (var item in htmlconfig.Items) {
                    if (null == item.Before && null == item.After) {
                        var e = XElement.Parse("<div class='report_item'></div>");
                        if (null != item.Before) {
                            e.Add(item.Before);
                        }
                        if (null != item.Item) {
                            e.Add(item.Item);
                        }
                        if (null != item.After) {
                            e.Add(item.After);
                        }
                        main.Add(e);
                    } else if (null != item.Item) {
                        main.Add(item.Item);
                    }
                }
            }

            if (null != htmlconfig.Footer) {
                content.Element("footer").ReplaceWith(htmlconfig.Footer);
            }


            if (standalone) {
                if (null != htmlconfig.DocFooter) {
                    root.Element("body").Add(htmlconfig.DocFooter);
                }
            }
            return root;
        }

        private void DoStreamRender(IReportContext context, IScope scope) {
            if (context.Request.Standalone) {
                Render(context, scope, DocHeader, null, "DocHeader");
            }
            Render(context, scope, Header, null, "Header");
            Render(context, scope, Content, null, "Content");
            var items = context.Data.arr("items");
            if (null != items) {
                var i = 0;
                foreach (var item in items) {
                    var itemScope = new Scope(scope);
                    itemScope["_item"] = item;
                    itemScope["_idx"] = i;
                    Render(context, itemScope, BeforeItem, item, "BeforeItem");
                    Render(context, itemScope, Item, item, "Item");
                    Render(context, itemScope, AfterItem, item, "AfterItem");
                    i++;
                }
            }
            Render(context, scope, Header, null, "Footer");
            if (context.Request.Standalone) {
                Render(context, scope, DocFooter, null, "DocFooter");
            }
        }

        IDictionary<string, IReportRender> templateCache = new Dictionary<string, IReportRender>();

        private XElement RenderXml(IReportContext context, IScope scope, string templateUri, object item,
            string templatename) {
            var ws = Render(context, scope, templateUri, item, templatename, true);
            if (null == ws) return null;
            return ws.Get<XElement>(templatename);

        }
        private IScope Render(IReportContext context, IScope scope, string templateUri, object item, string templateName, bool cache = false) {
            if (string.IsNullOrWhiteSpace(templateUri)) {
                return null;
            }
            if (context.IsSet("-render_" + templateName.ToLowerInvariant())) {
                return null;
            }

            if (!templateCache.ContainsKey(templateUri)) {
                templateCache[templateUri] = Renders.GetRender(templateUri, scope);
            }
            var template = templateCache[templateUri];
            if (null == template) {
                throw new Exception("cannot find template " + templateUri);
            }
            var ts = scope;
            if (cache) {
                ts = new Scope(ts);
                ts["store_render"] = true;
                ts["no_render"] = true;
                ts["render_name"] = templateName;
            }
            var result = template.Render(context, ts, item);
            return result;
        }

        private void SetupMime(IReportContext context) {
            if (string.IsNullOrWhiteSpace(MimeType)) {
                MimeType = "text/html";
            }
            if ((MimeType.Contains("text") || MimeType.Contains("json")) && !MimeType.Contains("charset")) {
                MimeType += "; charset=utf-8";
            }
            context.SetHeader("Content-Type", MimeType);
        }
    }
}