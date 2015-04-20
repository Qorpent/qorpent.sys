using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Bxl;
using Qorpent.IO.Http;
using Qorpent.Utils.Extensions;

namespace Qorpent.Host.Handlers{
	/// <summary>
	/// </summary>
	public class SmartXmlHandler{
		private bool _showroot;
		private bool haserrors;

		/// <summary>
		/// </summary>
		/// <param name="context"></param>
		public void Process(WebContext context) {
			var readdata = context.ReadRequestStirngAsync();
		    var parameters = RequestParameters.Create(context);
		    string lang = parameters.Get("lang");
		    string format = parameters.Get("format");

			Func<string, XElement> executor = null;
			if (lang == "bxl"){
				executor = BxlExecutor;
			}
			else{
				executor = BSharpExecutor;
			}
			Action<XElement, WebContext> render = null;
			if (format == "wiki"){
				render = RenderAsWiki;
			}
			else{
				render = RenderAsNative;
			}
			readdata.Wait();

		    string script = readdata.Result;
			_showroot = script.Contains("##showroot");
			XElement xml = null;
			try{
				xml = executor(script);
			}
			catch (Exception ex){
				xml = new XElement("error", ex.ToString());
			}
			render(xml, context);
		}

        private void RenderAsNative(XElement x, WebContext r)
        {
			r.Finish(x.ToString(), "text/xml");
		}

		private void RenderAsWiki(XElement x, WebContext r){
			var sb = new StringBuilder();
			BuildWiki(sb, x);
			r.Finish(sb.ToString(), "text/html");
		}

		private void BuildWiki(StringBuilder sb, XElement e){
			haserrors = false;
			int nscnt = 1;
			var namespaces = new Dictionary<string, string>();

			foreach (XElement element in e.DescendantsAndSelf()){
				if (!string.IsNullOrWhiteSpace(element.Name.NamespaceName)){
					if (element.Name.NamespaceName == "http://www.w3.org/2000/xmlns/") continue;
					if (!namespaces.ContainsKey(element.Name.NamespaceName)){
						namespaces[element.Name.NamespaceName] = "ns" + nscnt++;
					}
				}
				foreach (XAttribute attr in element.Attributes()){
					if (!string.IsNullOrWhiteSpace(attr.Name.NamespaceName)){
						if (attr.Name.NamespaceName == "http://www.w3.org/2000/xmlns/") continue;
						if (!namespaces.ContainsKey(attr.Name.NamespaceName)){
							namespaces[attr.Name.NamespaceName] = "ns" + nscnt++;
						}
					}
				}
			}
			sb.AppendLine("<div class='wk-x'>");
			if (!_showroot){
				if (namespaces.Count != 0){
					sb.AppendLine(
						"&lt;!-- документы XML полученные из BXL/B# выводятся (в WIKI) не от корня, справка по пространствам имен:");
					sb.AppendLine("<br/>");
					foreach (var ns in namespaces){
						sb.AppendLine(ns.Value + " :: " + ns.Key);
						sb.AppendLine("<br/>");
					}
					sb.AppendLine("-->");
					sb.AppendLine("<br/>");
				}
			}
			if (_showroot){
				BuildWiki(sb, e, 0, namespaces);
			}
			else{
				foreach (XElement element in e.Elements()){
					BuildWiki(sb, element, 0, namespaces);
				}
			}
			sb.AppendLine("</div>");
			if (haserrors){
				sb.AppendLine("<div class='wk-x-error'>");
				foreach (XElement element in e.Elements()){
					if (element.Name.LocalName != "bx-error") continue;
					Append(sb, element.Value, "wk-x-error-" + element.Attr("level"), tagname: "div");
				}
				sb.AppendLine("</div>");
			}
		}

		private void BuildWiki(StringBuilder sb, XElement e, int level, IDictionary<string, string> namespaces){
			if (e.Name.LocalName == "bx-error"){
				haserrors = true;
				return;
			}
			for (int i = 0; i < level*4; i++){
				sb.Append("&nbsp;");
			}
			Append(sb, "&lt;", "o", false);
			string realname = e.Name.LocalName;
			if (!string.IsNullOrWhiteSpace(e.Name.NamespaceName)){
				if (e.Name.NamespaceName == "http://www.w3.org/2000/xmlns/"){
					realname = "xmlns:" + realname;
				}
				else{
					realname = namespaces[e.Name.NamespaceName] + ":" + realname;
				}
			}
			Append(sb, realname, "en " + "en-" + e.Name.LocalName);
			foreach (XAttribute a in e.Attributes()){
				sb.Append(" ");
				string attrname = a.Name.LocalName;
				if (!string.IsNullOrWhiteSpace(a.Name.NamespaceName)){
					if (a.Name.NamespaceName == "http://www.w3.org/2000/xmlns/"){
						attrname = "xmlns:" + attrname;
					}
					else{
						attrname = namespaces[a.Name.NamespaceName] + ":" + attrname;
					}
				}
				Append(sb, attrname, "an " + "an-" + a.Name.LocalName);
				Append(sb, "=", "ae");
				Append(sb, "\"", "q");
				Append(sb, a.Value, "av");
				Append(sb, "\"", "q");
			}
			if (!e.Nodes().Any()){
				Append(sb, "/&gt", "c", false);
			}
			else{
				Append(sb, "&gt", "c", false);
			}
			bool shortstr = false;
			if (e.Nodes().Count() == 1 && e.Nodes().First() is XText && e.Value.Length <= 15){
				Append(sb, e.Value, "t");
				shortstr = true;
			}
			else{
				sb.Append("<br/>");

				foreach (XNode node in e.Nodes()){
					if (node is XElement){
						BuildWiki(sb, (XElement) node, level + 1, namespaces);
					}
					else{
						for (int i = 0; i < (level + 1)*4; i++){
							sb.Append("&nbsp;");
						}
						Append(sb, ((XText) node).Value, "t");
						sb.Append("<br/>");
					}
				}
			}


			if (e.Nodes().Any()){
				if (!shortstr){
					for (int i = 0; i < level*4; i++){
						sb.Append("&nbsp;");
					}
				}
				Append(sb, "&lt;/", "c", false);
				Append(sb, realname, "en " + "en-" + e.Name.LocalName);
				Append(sb, "&gt;", "c", false);
				sb.Append("<br/>");
			}
		}

		private void Append(StringBuilder b, string content, string cls, bool escape = true, string tagname = "span"){
			tagname = tagname ?? "span";
			content = escape
				          ? content.Trim()
				                   .Replace("&", "&amp;")
				                   .Replace("<", "&lt;")
				                   .Replace("\n", "<br/>")
				                   .Replace("\r", "")
				                   .Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;")
				          : content;
			b.Append("<" + tagname + " class='wk-x-" + cls + "'>" + content + "</" + tagname + ">");
		}

		private XElement BSharpExecutor(string arg){
			var root = new XElement("root");
			var context = (BSharpContext) BSharpCompiler.Compile(arg);
			foreach (IBSharpClass cls in context.Get(BSharpContextDataType.LibPkg)){
				root.Add(cls.Compiled);
			}
			foreach (BSharpError e in context.GetErrors(ErrorLevel.Warning)){
				root.Add(new XElement("bx-error", new XAttribute("level", e.Level), e.GetDigest().ToString()));
			}
			return root;
		}

		private XElement BxlExecutor(string arg){
			try{
				return new BxlParser().Parse(arg, "code.bxl", BxlParserOptions.NoLexData);
			}
			catch (Exception ex){
				return new XElement("root", new XElement("bx-error", new XAttribute("level", "Error"), ex.Message));
			}
		}
	}
}