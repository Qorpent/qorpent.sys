using System;
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
		/// Контсекст генерации PHTML
		/// </summary>
		public PortableHtmlContext Context { get; set; }
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
			var ctx = Context ?? new PortableHtmlContext();
			if (ConvertLineBreaksToHtmlBreaks){
				ReplaceLineBreaksToHtmlBreaks(result);
			}
			CheckSourceTrust(result, ctx);
			ProcessBreaks(result);
			ExpandLinks(result);
			ExpandInlines(result);
			CollapseParas(result);
			ProcessRootNodes(result);
			ProcessTextTrimming(result);
		    JoinFollowingSameInlines(result);
			if (!KeepFormatting){
				RemoveFormatsFromTotallyFormattedParagraphs(result);
			}

			Finalize(result);
			result.Name = "div";
			return result;
		}

	    private void JoinFollowingSameInlines(XElement result) {
           var inlines =
                  result.Descendants().Where(_ => _.Name.LocalName!="img" && -1 != Array.IndexOf(PortableHtmlSchema.InlineElements, _.Name.LocalName));
	        var invalid = inlines.Where(_ => {
	            var e = _.NextNode as XElement;	         
	            if (null == e) return false;
	            return e.Name.LocalName == _.Name.LocalName;
	        }).Reverse().ToArray();
	        foreach (var inline in invalid) {
	            var i = inline.NextNode as XElement;
	            inline.Value += " " + i.Value;
                i.Remove();
	        }
	    }

	    private void ExpandInlines(XElement result){
			var inlines =
				result.Descendants().Where(_ => -1 != Array.IndexOf(PortableHtmlSchema.InlineElements, _.Name.LocalName));
			var badinlines = inlines.Where(_ => _.Descendants("p").Any());
			foreach (var badinline in badinlines.Reverse().ToArray()){
				badinline.ReplaceWith(badinline.Nodes());
			}
		}

		private void Finalize(XElement result){
			foreach (var descendant in result.Descendants("p")){
				descendant.SetAttributeValue("__auto",null);
			}
			
		}

		private void CheckSourceTrust(XElement result, PortableHtmlContext ctx){
			foreach (var descendant in result.Descendants("img").ToArray()){
				if (null == descendant.Attribute("src") || string.IsNullOrWhiteSpace(descendant.Attribute("src").Value)){
					descendant.Remove();
				}
				else{
					var isTrust = ctx.GetUriTrustState(descendant.Attribute("src").Value, true) == PortableHtmlSchemaErorr.None;
					if (!isTrust){
						descendant.SetAttributeValue("phtml_src", descendant.Attribute("src").Value);
						descendant.SetAttributeValue("src", "/phtml_non_trust_image.png");
					}
				}
			}
			foreach (var descendant in result.Descendants("a").ToArray()){
				if (null == descendant.Attribute("href") || string.IsNullOrWhiteSpace(descendant.Attribute("href").Value)){
					descendant.ReplaceWith(descendant.Nodes());
				}
				else{
					var isTrust = ctx.GetUriTrustState(descendant.Attribute("href").Value, false) == PortableHtmlSchemaErorr.None;
					if (!isTrust){
						descendant.SetAttributeValue("phtml_href", descendant.Attribute("href").Value);
						descendant.SetAttributeValue("href", "/phtml_non_trust_link.html");
					}
				}
			}

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
							   ( (null == _.PreviousNode && null == _.NextNode )
							   || (string.IsNullOrWhiteSpace(_.Value) && !_.Descendants("img").Any())
							   )).ToArray();
					if (0 == illegalFormats.Length) break;
					foreach (var illegalFormat in illegalFormats){
						illegalFormat.ReplaceWith(illegalFormat.Nodes());
					}
				}
			}
		}

		private void ProcessTextTrimming(XElement result){
			foreach (var t in result.DescendantNodes().OfType<XText>()){
				if (t.Parent == null) continue;
				while (t.NextNode is XText){
					t.Value += (t.NextNode as XText).Value;
					t.NextNode.Remove();
				}
				if (t.Value.Length == 0) continue;
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
				t.Value =Regex.Replace(t.Value, @"(\s)\s*", "$1");

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

		private static void ExpandLinks(XElement result){
			foreach (var linkedP in result.Descendants("a").Where(_ => _.Descendants("p").Any()).ToArray()){
				foreach (var p in linkedP.Descendants("p").Reverse().ToArray()){
					p.AddFirst(" ");
					p.Add(" ");
					p.ReplaceWith(p.Nodes());
				}
				var para = new XElement("p", linkedP);
				linkedP.ReplaceWith(para);
			}
		}

		private static void ProcessBreaks(XElement result){
			foreach (var p in result.Descendants("br").ToArray()){
				var pre = p.PreviousNode;
				var post = p.NextNode;
				if (null != pre &&
				    !(pre is XElement && (((XElement) pre).Name.LocalName == "p" || ((XElement) pre).Name.LocalName == "br"))){
					var prep = new XElement("p");
					pre.AddBeforeSelf(prep);
					pre.Remove();
					prep.Add(pre);
					prep.SetAttributeValue("__auto", true); //annotations can be kiled in replaces,attributes better
				}
				if (null != post &&
				    !(post is XElement && (((XElement) post).Name.LocalName == "p" || ((XElement) post).Name.LocalName == "br"))){
					var postp = new XElement("p");
					post.AddBeforeSelf(postp);
					post.Remove();
					postp.Add(post);
					postp.SetAttributeValue("__auto", true); //annotations can be kiled in replaces,attributes better
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

		/// <summary>
		/// Вычисляет дайджест HTML и возвращает его 
		/// </summary>
		/// <param name="src"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		public string GetDigest(XElement src, int size = 400){
			var images = src.Descendants("img").ToArray();
			images.Remove();
			var html = Convert(src);
			var pars = html.Elements().ToArray();
			if (pars.Length == 0) {
				return images.Length == 0 ? "(Документ не содержит текста)" : "(Документ состоит из изображений)";
			}
			var strings = CollectSourceParasForDigest(size, pars);
			var full = string.Join("... ", strings);
			if (full.Length > size*1.05){
				CropDigest(size, full, strings);
			}
			else{
				NoCropDigest(strings);
					
			}
			var result = string.Join(" ", strings);	
			if (result.Trim().Length <= 3){
				return "(Документ не содержит текста)";
			}
			return result;
		}

		private static void NoCropDigest(List<string> strings){
			for (var i = 0; i < strings.Count - 1; i++){
				if (strings[i].EndsWith(".")){
					strings[i] += "..";
				}
				else{
					strings[i] += "...";
				}
			}
		}

		private static void CropDigest(int size, string full, List<string> strings){
			var crop = (full.Length - size)/strings.Count;
			for (var i = 0; i < strings.Count; i++){
				var str = strings[i];
				if (str.Length - crop <= 20){
					crop += crop/2;
					continue;
				}
				var basis = str.Substring(0, str.Length - crop);
				var ending = basis.LastIndexOf(' ');
				basis = basis.Substring(0, ending);
				if (!basis.EndsWith(".")){
					basis = basis + "...";
				}
				else{
					if (i != strings.Count - 1){
						basis += "..";
					}
				}
				strings[i] = basis;
			}
		}

		private static List<string> CollectSourceParasForDigest(int size, XElement[] pars){
			var strings = new List<string>();
			strings.Add(pars[0].Value);
			if (pars.Length >= 5 && size >= 600){
				strings.Add(pars[pars.Length/2 - 1].Value);
				strings.Add(pars[pars.Length/2 + 1].Value);
			}
			else{
				if (pars.Length >= 3 && size >= 400){
					strings.Add(pars[pars.Length/2].Value);
				}
			}
			if (pars.Length >= 2 && size >= 200){
				strings.Add(pars[pars.Length - 1].Value);
			}
			return strings;
		}
	}
}