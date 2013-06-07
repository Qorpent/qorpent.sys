using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Qorpent.IoC;
using Qorpent.Wiki;

namespace Qorpent.Serialization {
	/// <summary>
	/// </summary>
	/// <remarks>
	/// </remarks>
	[ContainerComponent(Lifestyle.Transient, ServiceType = typeof(ISerializer), Name = "wiki.serializer")]
	public class WikiSerializer : IWikiSerializer
	{
		
		/// <summary>
		/// Отрисовывает переданный текстовой контент в виде Wiki
		/// </summary>
		/// <param name="usage"></param>
		/// <param name="page"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public string ToHTML(string usage, WikiPage page, object context) {
			var lines = Preprocess(usage, page.Text, context);
			IList<string> processed = new List<string>();
			ProcessHTML(lines, processed, usage, page, context);
			return string.Join("\n",processed.ToArray()).Replace("__BLOCK__", "[[");
		}

		private void ProcessHTML(string[] lines, IList<string> processed, string usage, WikiPage page, object context) {
			bool codeblock = false;
			bool nowikiblock = false;
			bool table = false;
			bool firstrow = false;
			bool headclosed = false;
			string suffix = "";
			for (var i = 0; i < lines.Length; i++) {
				var curline = lines[i];
				if (string.IsNullOrWhiteSpace(curline)) continue;
				if (CheckoutCodeBlock(processed, usage, page, context, curline, ref codeblock)) continue;
				//WIKI IGNORANCE SUPPORT WITH BLOCK AND INLINE
				if (CheckoutNoWikiBlock(processed, curline, ref nowikiblock)) continue;

				if (Regex.IsMatch(curline,@"^\|",RegexOptions.Compiled)) {
					if (!table) {
						processed.Add("<table>");
						processed.Add("<thead>");
						table = true;
						firstrow = true;
						headclosed = false;
					}
				}else{
					if(table){
						processed.Add("</tbody>");
						processed.Add("</table>");
						table = false;
					}
				}
								
				if ( table ) {
					var tde = firstrow ? "th" : "td" ;
					var keephead  = false;
					if(!firstrow){
						if(Regex.IsMatch(curline,@"^\|\{\+\}",RegexOptions.Compiled)){
							curline = Regex.Replace(curline,@"^\|\{\+\}","|");
							firstrow = true;
							tde = "th";
						}
					}
					var items = curline.Split('|');
					var row = "";
					if(!firstrow && headclosed){
						row+="</thead></tbody>";
						headclosed = true;
					}
					row += "<tr>";
					for(var i = 0;i<items.Length;i++){
						if(i==0||i==items.Length-1)continue; //ignore left-right starters
						var cell = items[i].Trim();
						cell = processDefault(cell).trim();
						var spanmatch = cell.match(/^\{(\d+)?(,(\d+))?\}/);
						if(spanmatch){
							cell = cell.replace(/^\{(\d+)?(,(\d+))?\}/,'');
							var rowspan = 1;
							var colspan = 1;
							
							if(spanmatch[1]){
								colspan = spanmatch[1];
							}
							if (spanmatch[3]){
								rowspan = spanmatch[3];
							}
							row += "<"+tde+" rowspan='"+rowspan+"' colspan='"+colspan+"' >" + cell+"</"+tde+">";
						}else {
							row += "<"+tde+">" + cell+"</"+tde+">";
						}
					}
					
					
					row+="</tr>";
					processed.Add(row);
					firstrow = false;
					continue;
				}
			}
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

		private bool CheckoutCodeBlock(IList<string> processed, string usage, WikiPage page, object context, string curline,
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
			throw new System.NotImplementedException();
		}


		/*
	  qwiki.preprocess = function(processor){
var preprocessedText  = processor.text;
if(this.beforePreprocess){
	preprocessedText = this.beforePreprocess(processor,preprocessedText);
}
var preprocessedText = '\n'+processor.text.trim()+'\n';
preprocessedText = preprocessedText.replace(/\r/g,'');
// firstly we must meet 1-st feature - we unify and process line delimiters
preprocessedText = preprocessedText.replace(/\n\n+/g,"\n\n[BR]\n\n");
preprocessedText = preprocessedText.replace(/\n([\=\%\!\-\|])([^\n]+)/g,'\n\n$1$2\n\n');
// then we must split lines for block elements
preprocessedText = preprocessedText.replace(/(\[\[\/?\w+\]\])/g,"\n\n\n\n$1\n\n\n\n");
// and finally remove ambigous lines
preprocessedText = preprocessedText.replace(/\n\n+/g,"__LINER__");
preprocessedText = preprocessedText.replace(/\n/g,"&nbsp;");
preprocessedText = preprocessedText.replace(/__LINER__/g,"\n");
if(this.afterPreprocess){
	preprocessedText = this.afterPreprocess(processor,preprocessedText);
}
return preprocessedText;
};*/


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