using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Bxl;
using Qorpent.Mvc.Renders;
using Qorpent.Utils.Extensions;

namespace Qorpent.Host
{
	/// <summary>
	/// 
	/// </summary>
	public class SmartXmlHandler
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		public void Process(HttpListenerContext context){
			var buffer = new byte[context.Request.ContentLength64];
			var readdata = context.Request.InputStream.ReadAsync(buffer, 0, (int)context.Request.ContentLength64);
			var lang = context.Request.QueryString["lang"];
			var format = context.Request.QueryString["format"];
			Func<string, XElement> executor = null;
			if (lang == "bxl"){
				executor = BxlExecutor;
			}
			else{
				executor = BSharpExecutor;
			}
			Action<XElement, HttpListenerResponse> render = null;
			if (format == "wiki"){
				render = RenderAsWiki;
			}
			else{
				render = RenderAsNative;
			}
			readdata.Wait();
			var script = Encoding.UTF8.GetString(buffer, 0, readdata.Result);
			XElement xml = null;
			try{
				xml = executor(script);
			}
			catch (Exception ex){
				xml= new XElement("error",ex.ToString());
			}
			render(xml, context.Response);
		}

		private void RenderAsNative(XElement x, HttpListenerResponse r){
			r.Finish(x.ToString(), "text/xml");
		}

		private void RenderAsWiki(XElement x, HttpListenerResponse r){
			var sb = new StringBuilder();
			BuildWiki(sb, x);
			r.Finish(sb.ToString(),"text/html");
		}

		private void BuildWiki(StringBuilder sb, XElement e){
			bool haserrors = false;
			int nscnt = 1;
			var namespaces = new Dictionary<string, string>();
			foreach (var element in e.DescendantsAndSelf()){
				if (!string.IsNullOrWhiteSpace(element.Name.NamespaceName)){
					if (!namespaces.ContainsKey(element.Name.NamespaceName)){
						namespaces[element.Name.NamespaceName] = "ns" + nscnt++;
					}
				}
				foreach (var attr in element.Attributes()){
					if (!string.IsNullOrWhiteSpace(attr.Name.NamespaceName)){
						if (!namespaces.ContainsKey(attr.Name.NamespaceName)){
							namespaces[attr.Name.NamespaceName] = "ns" + nscnt++;
						}
					}
				}
			}
			sb.AppendLine("<div class='wk-x'>");
			if (namespaces.Count != 0){
				sb.AppendLine("&lt;!-- документы XML полученные из BXL/B# выводятся (в WIKI) не от корня, справка по пространствам имен:");
				sb.AppendLine("<br/>");
				foreach (var ns in namespaces){
					sb.AppendLine(ns.Value + " :: " + ns.Key);
					sb.AppendLine("<br/>");
				}
				sb.AppendLine("-->");
				sb.AppendLine("<br/>");
			}
			foreach (var element in e.Elements()){
				if (element.Name.LocalName == "bx-error"){
					haserrors = true;
					continue;
				}
				BuildWiki(sb,element,0,namespaces);
			}
			sb.AppendLine("</div>");
			if (haserrors){
				sb.AppendLine("<div class='wk-x-error'>");
				foreach (var element in e.Elements()){
					if (element.Name.LocalName != "bx-error") continue;
					Append(sb, element.Value, "wk-x-error-"+element.Attr("level"));
				}
				sb.AppendLine("</div>");
			}
		}

		private void BuildWiki(StringBuilder sb, XElement e, int level,IDictionary<string,string> namespaces  ){
	
			for (var i = 0; i < level*4; i++){
				sb.Append("&nbsp;");
			}
			Append(sb,"&lt;","o",false);
			var realname = e.Name.LocalName;
			if (!string.IsNullOrWhiteSpace(e.Name.NamespaceName)){
				realname = namespaces[e.Name.NamespaceName] + ":" + realname;
			}
			Append(sb,realname,"en "+"en-"+e.Name.LocalName);
			foreach (var a in e.Attributes()){
				sb.Append(" ");
				var attrname = a.Name.LocalName;
				if (!string.IsNullOrWhiteSpace(a.Name.NamespaceName)){
					attrname = namespaces[a.Name.NamespaceName] + ":" + attrname;
				}
				Append(sb, attrname, "an " + "an-" + a.Name.LocalName);
				Append(sb,"=","ae");
				Append(sb,"\"","q");
				Append(sb,a.Value,"av");
				Append(sb,"\"","q");
			}
			if (!e.Nodes().Any()){
				Append(sb, "/&gt", "c", false);
			}
			else{
				Append(sb, "&gt", "c", false);
			}
			bool shortstr =false;
			if (e.Nodes().Count() == 1 && e.Nodes().First() is XText && e.Value.Length<=15){
		
					Append(sb, e.Value, "t");
				shortstr = true;
			}
			else{
				
				sb.Append("<br/>");

				foreach (var node in e.Nodes()){
					if (node is XElement){
						BuildWiki(sb, (XElement) node, level + 1,namespaces);
					}
					else{
						for (var i = 0; i < (level+1) * 4; i++)
						{
							sb.Append("&nbsp;");
						}
						Append(sb, ((XText) node).Value, "t");
						sb.Append("<br/>");
					}
				}

			}


			if (e.Nodes().Any()){
				if(!shortstr)
				{
			

					for (var i = 0; i < level*4; i++){
						sb.Append("&nbsp;");
					}
				}
				Append(sb, "&lt;/", "c", false);
				Append(sb, realname, "en " + "en-" + e.Name.LocalName);
				Append(sb, "&gt;", "c", false);
				sb.Append("<br/>");
			}

			
		}

		private void Append(StringBuilder b,string content, string cls,bool escape =true){
			content = escape ? content.Trim().Replace("&", "&amp;").Replace("<", "&lt;").Replace("\n", "<br/>").Replace("\r","").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;") : content;
			b.Append("<span class='wk-x-" + cls + "'>" +content  + "</span>");
		}

		private XElement BSharpExecutor(string arg){
			var root = new XElement("root");
			var context = (BSharpContext)BSharpCompiler.Compile(arg);
			foreach (var cls in context.Get(BSharpContextDataType.LibPkg)){
				root.Add(cls.Compiled);
			}
			foreach (var e in context.GetErrors(ErrorLevel.Warning)){
				root.Add(new XElement("bx-error",new XAttribute("level",e.Level),e.GetDigest().ToString()));
			}
			return root;
		}

		private XElement BxlExecutor(string arg){
			try{
				return new BxlParser().Parse(arg, "code.bxl", BxlParserOptions.NoLexData);
			}
			catch (Exception ex){
				return new XElement("root",new XElement("bx-error",new XAttribute("level","Error"),ex.Message));
			}
		}
	}
}
