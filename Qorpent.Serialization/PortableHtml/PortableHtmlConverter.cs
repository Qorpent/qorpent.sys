using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Qorpent.PortableHtml{
	/// <summary>
	///     Выполняет преобразование HTML/XML на входе в PHTML
	/// </summary>
	public class PortableHtmlConverter{
		private static readonly IDictionary<string, string> Tagmap = new Dictionary<string, string>{
			{"p", "p"},
			{"div", "p"},
			{"li", "p"},
			{"ul", "p"},
			{"ol", "p"},
			{"table", "p"},
			{"thead", "p"},
			{"tbody", "p"},
			{"tr", "p"},
			{"h1", "p"},
			{"h2", "p"},
			{"h3", "p"},
			{"h4", "p"},
			{"h5", "p"},
			{"h6", "p"},
			{"td", "span"},
			{"img", "img"},
			{"a", "a"},
			{"strong", "strong"},
			{"br", "br"},
			{"em", "em"},
			{"del", "del"},
			{"u", "u"},
			{"sub", "sub"},
			{"sup", "sup"},
			{"b", "strong"},
			{"i", "em"},
			{"strike", "del"},
		};

		/// <summary>
		///     Признак сохранения форматирования
		/// </summary>
		public bool KeepFormatting { get; set; }

		/// <summary>
		///     Конвертирует заголовки в P с подэлементом STRONG
		/// </summary>
		public bool ConvertHeadersToStrongs { get; set; }

		/// <summary>
		///     Конветирует разделители строк в текстовых элементах в  BR
		/// </summary>
		public bool ConvertLineBreaksToHtmlBreaks { get; set; }

		/// <summary>
		///     Конвертирует исходный XML в PHTML-совместимый
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public XElement Convert(XElement source){
			var result = InitalCleanup(source);
			if (ConvertLineBreaksToHtmlBreaks){
				ReplaceLineBreaksToHtmlBreaks(result);
			}
			ProcessBreaks(result);
			CollapseParas(result);
			ProcessRootNodes(result);
			ProcessTextTrimming(result);
			if (!KeepFormatting){
				RemoveFormatsFromTotallyFormattedParagraphs(result);
			}
			result.Name = "div";
			return result;
		}

		private void ReplaceLineBreaksToHtmlBreaks(XElement result){
			foreach (var n in result.DescendantNodes().OfType<XText>().ToArray()){
				if (n.Value.Contains('\n')){
					var subnodes =
						Regex.Split(n.Value, @"[\r\n]+")
						     .Where(_ => !string.IsNullOrWhiteSpace(_))
						     .Select(_ => new XText(_));
					var replace = new List<XNode>();
					var first = true;
					foreach (var subnode in subnodes){
						if (!first){
							replace.Add(new XElement("br"));
						}
						first = false;
						replace.Add(subnode);
					}
					n.ReplaceWith(replace.ToArray());
				}
			}
		}


		private void RemoveFormatsFromTotallyFormattedParagraphs(XElement result){
			foreach (var para in result.Elements()){
				while (true){
					var illegalFormats =
						para.Descendants()
						    .Where(
							    _ =>
							    null == _.Annotation<KeepFormat>() && _.Name.LocalName != "img" && _.Name.LocalName != "a" &&
							    null == _.PreviousNode && null == _.NextNode).ToArray();
					if (0 == illegalFormats.Length) break;
					foreach (var illegalFormat in illegalFormats){
						illegalFormat.ReplaceWith(illegalFormat.Nodes());
					}
				}
			}
		}

		private void ProcessTextTrimming(XElement result){
			foreach (var t in result.DescendantNodes().OfType<XText>()){
				if (t.Value.Contains("&amp;")){
					t.Value = t.Value.Replace("&amp;nbsp;", "\u00A0").Replace("&amp;lt;", "<").Replace("&amp;gt;", ">");
				}
				var waslead = char.IsWhiteSpace(t.Value[0]);
				var wastail = t.Value.Length > 1 && char.IsWhiteSpace(t.Value.Last());
				t.Value = t.Value.Trim();
				if (waslead && t.PreviousNode is XElement){
					t.Value = " " + t.Value;
				}
				if (wastail && t.NextNode is XElement){
					t.Value += " ";
				}
			}
		}

		private void ProcessRootNodes(XElement result){
			//restore auto states from breaks
			foreach (var p in result.Elements("p")){
				var a = p.Attribute("__auto");
				if (null != a){
					a.Remove();
					p.AddAnnotation(AutoPara.Default);
				}
			}
			foreach (var n in result.Nodes().ToArray()){
				if (null != n as XElement && (n as XElement).Name.LocalName == "p"){
					continue;
				}
				if (null != n.PreviousNode && null != n.PreviousNode.Annotation<AutoPara>()){
					((XElement) n.PreviousNode).Add(n);
					n.Remove();
				}
				else{
					var auto = new XElement("p", n);
					auto.AddAnnotation(AutoPara.Default);
					n.ReplaceWith(auto);
				}
			}
		}


		private XElement InitalCleanup(XElement source){
			var result = new XElement(source);
			if (!source.Elements().Any() && source.Value.Contains("<") && source.Value.Contains(">")){
				result = new XElement("div");
				result.Add(XElement.Parse("<p>" + source.Value + "</p>"));
			}
			foreach (var descendant in result.DescendantsAndSelf().Reverse().ToArray()){
				Cleanup(descendant);
			}
			return result;
		}

		private static void CollapseParas(XElement result){
			foreach (var p in result.Descendants("p").Reverse().ToArray()){
				if (p.Descendants("p").Any()){
					p.ReplaceWith(p.Nodes());
				}
			}
		}

		private static void ProcessBreaks(XElement result){
			foreach (var p in result.Descendants("br").ToArray()){
				var pre = p.PreviousNode;
				var post = p.NextNode;
				if (null != pre &&
				    !(pre is XElement && (((XElement) pre).Name.LocalName == "p" || ((XElement) pre).Name.LocalName == "br"))){
					var prep = new XElement("p", pre);
					prep.SetAttributeValue("__auto", true); //annotations can be kiled in replaces,attributes better
					pre.ReplaceWith(prep);
				}
				if (null != post &&
				    !(post is XElement && (((XElement) post).Name.LocalName == "p" || ((XElement) post).Name.LocalName == "br"))){
					var postp = new XElement("p", post);
					postp.SetAttributeValue("__auto", true); //annotations can be kiled in replaces,attributes better
					post.ReplaceWith(postp);
				}
				if (null != p.Parent) p.Remove();
			}
		}


		/// <summary>
		///     Первичная PHTML - очистка элемента
		/// </summary>
		/// <param name="e"></param>
		private void Cleanup(XElement e){
			if (IsLegal(e)){
				SetupElement(e);
			}
			else{
				e.ReplaceWith(e.Nodes());
			}
		}

		private  void SetupElement(XElement e){
			ProcessWellKnownElement(e); //сначала обрабатываем особые требования известных элементов
			SetupAttributes(e);
			ConvertCDataToUsualText(e);
			RemoveComments(e);
		}

		private static void RemoveComments(XElement e){
			foreach (var n in e.Nodes().OfType<XComment>()){
				n.ReplaceWith(" ");
			}
			foreach (var n in e.Nodes().OfType<XProcessingInstruction>()){
				n.ReplaceWith(" ");
			}
		}

		private static void ConvertCDataToUsualText(XElement e){
			foreach (var n in e.Nodes().OfType<XCData>()){
				n.ReplaceWith(new XText(n.Value));
			}
		}

		private static void SetupAttributes(XElement e){
			var attrs = e.Attributes().ToArray();
			foreach (var attribute in attrs){
				if (attribute.Name.LocalName.StartsWith("phtml_")) continue;
				if (attribute.Name.LocalName.ToLowerInvariant() == "src" && e.Name.LocalName == "img"){
					if (attribute.Name.LocalName != "src"){
						e.SetAttributeValue("src", attribute.Value);
					}
					else{
						continue;
					}
				}
				if (attribute.Name.LocalName.ToLowerInvariant() == "href" && e.Name.LocalName == "a"){
					if (attribute.Name.LocalName != "href"){
						e.SetAttributeValue("href", attribute.Value);
					}
					else{
						continue;
					}
				}
				attribute.Remove();
			}
		}

		private bool IsLegal(XElement e){
			//сразу решаем проблему пространств имен и кейса
			e.Name = e.Name.LocalName.ToLowerInvariant();
			//если это известный элемент
			if (Tagmap.ContainsKey(e.Name.LocalName)){
				return true;
			}
			//если неизвестный элемент - корневой, то считаем его также легальным
			if (null == e.Parent){
				//но подставляем ему имя
				e.Name = "div";
				return true;
			}
			return false;
		}

		private void ProcessWellKnownElement(XElement e){
			var oldname = e.Name.LocalName;
			e.Name = Tagmap[e.Name.LocalName];
			if (e.Name == "img"){
				e.Nodes().Remove();
			}
			if (oldname.StartsWith("h") && oldname.Length == 2){
				if (ConvertHeadersToStrongs){
					var str = new XElement("strong", e.Nodes());
					str.AddAnnotation(KeepFormat.Default);
					e.ReplaceAll(str);
				}
				e.SetAttributeValue("phtml_tag", oldname);
			}
			if (e.Name.LocalName == "span"){
				if (oldname != "span"){
					e.SetAttributeValue("phtml_tag", oldname);

					if (null != e.PreviousNode && null != e.Parent){
						e.AddBeforeSelf(new XText("\u00A0"));
					}
					if (null != e.NextNode && null != e.Parent){
						e.AddAfterSelf(new XText("\u00A0"));
					}
				}
			}
			//пустой параграф понимается как аналог BR
			if (e.Name == "p" && string.IsNullOrWhiteSpace(e.Value) && !e.Descendants("img").Any()){
				e.Name = "br";
				e.Value = "";
			}

			if (e.Name == "p" && oldname != "p" && oldname != "div"){
				e.SetAttributeValue("phtml_tag", oldname);
			}
		}
		
		private class AutoPara{
			public static readonly AutoPara Default = new AutoPara();
		}
		private class KeepFormat{
			public static readonly KeepFormat Default = new KeepFormat();
		}
	}
}