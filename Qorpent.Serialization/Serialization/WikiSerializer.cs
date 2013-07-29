using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Qorpent.IoC;
using Qorpent.Mvc;
using Qorpent.Wiki;

namespace Qorpent.Serialization {
	/// <summary>
	/// </summary>
	/// <remarks>
	/// </remarks>
	[ContainerComponent(Lifestyle.Transient, ServiceType = typeof(IWikiSerializer), Name = "wiki.serializer")]
	public class WikiSerializer : IWikiSerializer {
		/// <summary>
		/// Версия сериалайзера для контроля 304
		/// </summary>
		public const int SerializerVersion = 1;

		/// <summary>
		/// Играет роль хэш-функции для определенного типа.
		/// </summary>
		/// <returns>
		/// Хэш-код для текущего объекта <see cref="T:System.Object"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode() {
			return SerializerVersion;
		}
		/// <summary>
		/// Отрисовывает переданный текстовой контент в виде Wiki
		/// </summary>
		/// <param name="usage"></param>
		/// <param name="page"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public string ToHTML(string usage, WikiPage page, IMvcContext context) {
			if (string.IsNullOrWhiteSpace(page.Text)) return "";
			var lines = Preprocess(usage, page.Text, context);
			IList<string> processed = new List<string>();
			ProcessHTML(lines, processed, usage, page, context);
			return string.Join("\n",processed.ToArray()).Replace("__BLOCK__", "[[");
		}

		private void ProcessHTML(string[] lines, IList<string> processed, string usage, WikiPage page, IMvcContext context) {
			bool codeblock = false;
			bool nowikiblock = false;
			bool table = false;
			bool ishead = false;
			for (var idx = 0; idx < lines.Length; idx++) {
				var curline = lines[idx];
				if (string.IsNullOrWhiteSpace(curline)) continue;
				if (CheckoutCodeBlock(processed, usage, page, context, curline, ref codeblock)) continue;
				//WIKI IGNORANCE SUPPORT WITH BLOCK AND INLINE
				if (CheckoutNoWikiBlock(processed, curline, ref nowikiblock)) continue;
				if (CheckoutTable(processed, usage, page, context, curline, ref table, ref ishead)) continue;
				if (CheckoutSampleBlock(processed, curline)) continue;
				if (CheckoutPageDelimiter(processed, curline)) continue;
				var defaultProcessed = ProcessDefault(curline, usage, page, context);
				if (!string.IsNullOrWhiteSpace(defaultProcessed)) {
					processed.Add(defaultProcessed);
				}
			}
		}

		private string ProcessDefault(string curline, string usage, WikiPage page, IMvcContext context)
		{
				// BY LINE IGNORE
				if (curline.StartsWith("!")){ //must ignore any wiki syntax this row
					curline = curline.Substring(1);
					return curline;
				}
				/////////////////////////////////////////////////////	
				// LINE BREAK SUPPORT
				if (curline == "[BR]") {
					return "<br/>";
				}
				
				curline = ProcessInline(curline,usage,page,context);
				// references
				curline = ProcessReferences(curline, usage, page, context);

				curline = ProcessLine(curline, usage, page, context);
				
				return curline;
		}

		private string ProcessLine(string curline, string usage, WikiPage page, object context) {
			if (curline.StartsWith("======")){
				curline = "<h6>" + curline.Substring(6) + "</h6>";
			}else if (curline.StartsWith("=====")){
				curline = "<h5>" + curline.Substring(5) + "</h5>";
			}else if (curline.StartsWith("====")){
				curline = "<h4>" + curline.Substring(4) + "</h4>";
			}else if (curline.StartsWith("===")){
				curline = "<h3>" + curline.Substring(3) + "</h3>";
			}else if (curline.StartsWith("==")){
				curline = "<h2>" + curline.Substring(2) + "</h2>";
			}else if (curline.StartsWith("=")){
				curline = "<h1>" + curline.Substring(1) + "</h1>";
			}

			//LIST SUPPORT
			else if (curline.StartsWith("%%%%%%")){
				curline = "<div class='wiki-list wiki-list-6'>" + curline.Substring(6) + "</div>";
			}else if (curline.StartsWith("%%%%%")){
				curline = "<div class='wiki-list wiki-list-5'>" + curline.Substring(5) + "</div>";
			}else if (curline.StartsWith("%%%%")){
				curline = "<div class='wiki-list wiki-list-4'>" + curline.Substring(4) + "</div>";
			}else if (curline.StartsWith("%%%")){
				curline = "<div class='wiki-list wiki-list-3'>" + curline.Substring(3) + "</div>";
			}else if (curline.StartsWith("%%")){
				curline = "<div class='wiki-list wiki-list-2'>" + curline.Substring(2) + "</div>";
			}else if (curline.StartsWith("%")){
				curline = "<div class='wiki-list wiki-list-1'>" + curline.Substring(1) + "</div>";
			}

			else if (curline.StartsWith("№№№№№№"))
			{
				curline = "<div class='wiki-list wiki-list-6 number' >" + curline.Substring(6) + "</div>";
			}
			else if (curline.StartsWith("№№№№№"))
			{
				curline = "<div class='wiki-list wiki-list-5 number'>" + curline.Substring(5) + "</div>";
			}
			else if (curline.StartsWith("№№№№"))
			{
				curline = "<div class='wiki-list wiki-list-4 number'>" + curline.Substring(4) + "</div>";
			}
			else if (curline.StartsWith("№№№"))
			{
				curline = "<div class='wiki-list wiki-list-3 number'>" + curline.Substring(3) + "</div>";
			}
			else if (curline.StartsWith("№№"))
			{
				curline = "<div class='wiki-list wiki-list-2 number'>" + curline.Substring(2) + "</div>";
			}
			else if (curline.StartsWith("№"))
			{
				curline = "<div class='wiki-list wiki-list-1 number'>" + curline.Substring(1) + "</div>";
			}
			
			
			else{
				curline = "<p>"+curline+"</p>";
			}
			return curline;
		}

		private string ProcessReferences(string curline, string usage, WikiPage page, IMvcContext context) {
			return Regex.Replace(curline,ReferenceRegex,m=> ReferenceReplacer(m,usage,page,context));

		}
		/// <summary>
		/// Регекс для ссылок
		/// </summary>
		public const string ReferenceRegex = @"\[(?<addr>[^\s\]]+?)(\s+(?<name>[^|~\]]+?))?([\|~](?<tail>[\s\S]+?))?\]";
		private string ReferenceReplacer(Match match, string usage, WikiPage page, IMvcContext context) {
			var addr = match.Groups["addr"].Value;
			var name = match.Groups["name"].Value;
			var tail = match.Groups["tail"].Value;
			if (string.IsNullOrWhiteSpace(name)) name = addr;
			if (context != null) {
				if (addr.StartsWith("/")) {
					return PageOpenUrl(addr, name, tail, context);
				}
				else if(addr.StartsWith("img:")) {
					return GetWikiImageRef(addr, name, tail, context);
				}
				else if (addr.StartsWith("file:")) {
					return GetWikiBinaryRef(addr, name, tail, context);
				}
				else {
					return GetDefaultReference(addr, name, tail, context);
				}
			}
			else {
				return GetDefaultReference(addr, name, tail, context);
			}

		}

		private string GetWikiBinaryRef(string addr, string name, string tail, IMvcContext context) {
			var appname = context.Application.ApplicationName;
			var call = ("/"+appname+"/wiki/getfile.filedesc.qweb?code=" + addr.Substring(5)).Replace("//","/");
			return string.Format("<a href='{0}' {1}>{2}</a>", call, tail, name);
		}

		private string GetWikiImageRef(string addr, string name, string tail, IMvcContext context) {
			var appname = context.Application.ApplicationName;
			var call = ("/" + appname + "/wiki/getfile.filedesc.qweb?code=" + addr.Substring(4)).Replace("//", "/");
			return string.Format("<img src='{0}' {1} title='{2}' />", call, tail, name);
		}


		private string GetDefaultReference(string addr, string name, string tail, IMvcContext context) {
			if (Regex.IsMatch(addr, @"\.((png)|(gif)|(jpg)|(jpeg))")) {
				return string.Format("<img src='{0}' {1} title='{2}' />", addr, tail, name);
			}

			else {
				return string.Format("<a href='{0}' {1}>{2}</a>", addr, tail, name);
			}
		}

		private string PageOpenUrl(string addr, string name, string tail, IMvcContext context) {
			var appname = context.Application.ApplicationName;
			var call = ("/" + appname + "/wiki/get.wiki.qweb?code=" + addr).Replace("//", "/");
			return string.Format("<a href='{0}' {1}>{2}</a>", call, tail, name);
		}

		private string ProcessInline(string curline, string usage, WikiPage page, object context) {
			curline = Regex.Replace(curline, @"\*\*\*([\s\S]+?)\*\*\*", "<strong>$1</strong>", RegexOptions.Compiled);
			//italic
			curline = Regex.Replace(curline, @"\*\*([\s\S]+?)\*\*", "<em>$1</em>", RegexOptions.Compiled);
			//underline
			curline = Regex.Replace(curline, @"__([\s\S]+?)__", "<ins>$1</ins>", RegexOptions.Compiled);
			//strikeout
			curline = Regex.Replace(curline, @"--([\s\S]+?)--", "<del>$1</del>", RegexOptions.Compiled);
			//subtext new version
			curline = Regex.Replace(curline, @",,([\s\S]+?),,", "<sub>$1</sub>", RegexOptions.Compiled);
			//supertext
			curline = Regex.Replace(curline, @"::([\s\S]+?)::", "<sup>$1</sup>", RegexOptions.Compiled);
			//custom style
			curline = Regex.Replace(curline, @"\{style:([\s\S]+?)\}([\s\S]+?)\{style\}", "<span style=\"$1\">$2</span>",
			                        RegexOptions.Compiled);
			return curline;
		}

		private static bool CheckoutPageDelimiter(IList<string> processed, string curline) {
			if (curline == "----") {
				processed.Add("<hr/>");
				return true;
			}
			return false;
		}

		private static bool CheckoutSampleBlock(IList<string> processed, string curline) {
			if (curline == "[[sample]]") {
				processed.Add("<div class='sample'>");
				return true;
			}

			if (curline == "[[/sample]]") {
				processed.Add("</div>");
				return true;
			}
			return false;
		}

		private bool CheckoutTable(IList<string> processed, string usage, WikiPage page, IMvcContext context, string curline, ref bool table,
		                           ref bool ishead) {
			if (Regex.IsMatch(curline, @"^\|", RegexOptions.Compiled)) {
				if (!table) {
					processed.Add("<table>");
					processed.Add("<thead>");
					table = true;
					ishead = true;
				}
			}
			else {
				if (table) {
					processed.Add("</tbody>");
					processed.Add("</table>");
					table = false;
				}
			}

			if (table) {
				var tde = ishead ? "th" : "td";
				if (!ishead) {
					if (Regex.IsMatch(curline, @"^\|\{\+\}", RegexOptions.Compiled)) {
						curline = Regex.Replace(curline, @"^\|\{\+\}", "|", RegexOptions.Compiled);
						ishead = true;
						tde = "th";
					}
				}
				var items = curline.Split('|');
				var row = "";
				if (!ishead) {
					row += "</thead></tbody>";
				}
				ishead = false;

				row += "<tr>";
				for (var i = 0; i < items.Length; i++) {
					if (i == 0 || i == items.Length - 1) {
						continue; //ignore left-right starters
					}
					var cell = items[i].Trim();
					cell = ProcessInline(cell, usage, page, context);
					cell = ProcessReferences(cell,usage,page,context).Trim();
					var spanmatch = Regex.Match(cell, @"^\{(\d+)?(,(\d+))?\}", RegexOptions.Compiled);
					if (spanmatch.Success) {
						cell = Regex.Replace(cell, @"^\{(\d+)?(,(\d+))?\}", "", RegexOptions.Compiled);
						var rowspan = "1";
						var colspan = "1";

						if (!string.IsNullOrWhiteSpace(spanmatch.Groups[1].Value)) {
							colspan = spanmatch.Groups[1].Value;
						}
						if (!string.IsNullOrWhiteSpace(spanmatch.Groups[3].Value)) {
							rowspan = spanmatch.Groups[3].Value;
						}
						row += "<" + tde + " rowspan='" + rowspan + "' colspan='" + colspan + "' >" + cell + "</" + tde + ">";
					}
					else {
						row += "<" + tde + ">" + cell + "</" + tde + ">";
					}
				}


				row += "</tr>";
				processed.Add(row);

				return true;
			}
			return false;
		}

		


		private static bool CheckoutNoWikiBlock(IList<string> processed, string curline, ref bool nowikiblock) {
			if (curline == "[[/nowiki]]") {
				nowikiblock = false;
				return true;
			}
			if (curline == "[[nowiki]]") {
				nowikiblock = true;
				return true;
			}
			if (nowikiblock) {
				processed.Add(curline);
				return true;
			}
			return false;
		}

		private bool CheckoutCodeBlock(IList<string> processed, string usage, WikiPage page, IMvcContext context, string curline,
		                               ref bool codeblock) {
			if (curline == "[[/code]]") {
				codeblock = false;
				processed.Add("</div>");
				return true;
			}
			if (curline == "[[code]]") {
				codeblock = true;
				processed.Add("<div class='wiki-code'>");
				return true;
			}
			if (codeblock) {
				processed.Add(ProcessCode(curline, usage, page, context));
				return true;
			}
			return false;
		}

		private string ProcessCode(string curline, string usage, WikiPage page, object context) {
			curline = Regex.Replace(curline,@"\&nbsp;"," __BR__ ");
			curline = Regex.Replace(curline,@"\[BR\]","");
			curline = Regex.Replace(curline,@"\s{4}"," __TAB__ ");
			curline = Regex.Replace(curline,@"\t"," __TAB__ ");
			//CODE BLOCKS
			curline = Regex.Replace(curline,@"([!=+\-*\.\\\/;<>%\&\^\:\|]+)","<span _CLASS_ATTR_'operator'>$1</span>");
			curline = Regex.Replace(curline,@"\/\*","<span _CLASS_ATTR_'comment'>");
			curline = Regex.Replace(curline,@"\*\/","</span>");
			curline = Regex.Replace(curline,@"(\#[^""']+)$","<span _CLASS_ATTR_'comment'>$1</span>");
			curline = Regex.Replace(curline,@"\b((var)|(for)|(return)|(foreach)|(while)|(case)|(switch)|(in)|(out)|(private)|(public)|(protected)|(void)|(function)|(class)|(namespace)|(using)|(select)|(where)|(group by)|(order by)|(null)|(true)|(false))\b","<span _CLASS_ATTR_'keyword'>$1</span>");
			curline = Regex.Replace(curline,@"\b((int)|(string)|(DateTime)|(decimal)|(bool)|(nvarchar)|(datetime)|(bit)|(byte)|(float)|(long)|(bigint))\b","<span _CLASS_ATTR_'type'>$1</span>");
			curline = Regex.Replace(curline,@"([\{\}\[\]\(\),]+)","<span _CLASS_ATTR_'delimiter'>$1</span>");
	
			curline = Regex.Replace(curline,"\\\"","_EQ_");
			curline = Regex.Replace(curline,@"""","_DQ_");
			curline = Regex.Replace(curline,"\"([\\s\\S]+?)\"","<span _CLASS_ATTR_'string'>$1</span>");
			curline = Regex.Replace(curline,@"_EQ_","\\\"");
			curline = Regex.Replace(curline,@"_DQ_","\"\"");
	
			curline = Regex.Replace(curline,@"(\b-?\d+(\.\d+)?)","<span _CLASS_ATTR_'number'>$1</span>");
	
	
			curline+="<br/>";
			curline = Regex.Replace(curline,@"_CLASS_ATTR_","class=");
			curline = Regex.Replace(curline,@"__TAB__","&nbsp;&nbsp;&nbsp;&nbsp;");
			curline = Regex.Replace(curline,@"__BR__","<br/>");
			return curline;
		}




		/// <summary>
		/// Осуществляет препроцессинг входного текста WIKI на строки
		/// </summary>
		/// <param name="usage"></param>
		/// <param name="text"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public string[] Preprocess(string usage, string text, object context) {
			var preprocessed = "\n" + text.Trim() + "\n";
			preprocessed = preprocessed.Replace("\r", "");
			preprocessed = Regex.Replace(preprocessed, @"\n\n+", "\n\n[BR]\n\n", RegexOptions.Compiled);
			preprocessed = Regex.Replace(preprocessed, @"\n([\=\%\!\-\|])([^\n]+)", "\n\n$1$2\n\n", RegexOptions.Compiled);
			preprocessed = Regex.Replace(preprocessed, @"(\[\[\/?\w+\]\])", "\n\n\n\n$1\n\n\n\n", RegexOptions.Compiled);
			preprocessed = Regex.Replace(preprocessed, @"\n\n+", "_LINER_", RegexOptions.Compiled);
			preprocessed = Regex.Replace(preprocessed, @"\n", "&nbsp;", RegexOptions.Compiled);
			preprocessed = Regex.Replace(preprocessed, @"_LINER_", "\n", RegexOptions.Compiled);
			return preprocessed.Split('\n');
		}

	

		
	}
}