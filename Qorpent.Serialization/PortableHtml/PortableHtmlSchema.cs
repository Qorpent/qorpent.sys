using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.PortableHtml{
	/// <summary>
	/// Вспомогательный класс для проверки соответствия строки и/или XML стандарту PHTML
	/// </summary>
	public class PortableHtmlSchema{

		/// <summary>
		/// Перечень запрещенных элементов
		/// </summary>
		public static readonly string[] DeprecatedElements = new[]{
			"script",
			"object",
			"embed",
			"applet",
			"iframe",
			"style",
			"form",
			"input",
			"button",
			"select",
			"textarea"
		};
		/// <summary>
		/// Допустимое локальное имя для рута
		/// </summary>
		public const string AllowedRootName = "div";

		/// <summary>
		/// Перечень запрещенных элементов
		/// </summary>
		public static readonly string[] DeprecatedAttributePrefixes = new[]{
			"on","ng"
		};

		/// <summary>
		/// Перечень запрещенных элементов
		/// </summary>
		public static readonly string[] DeprecatedAttributes = new[]{
			"id","class","style","width","height","name","value"
		};
		/// <summary>
		/// Элемент, который обязательно пустой
		/// </summary>
		public const string EmptyRequiredElement = "img";

		/// <summary>
		/// Вспомогательный метод проверки допустимости имени элемента в схеме
		/// </summary>
		/// <param name="tagname"></param>
		/// <returns></returns>
		public static bool IsAllowedTag(string tagname){
			return -1 == Array.IndexOf(DeprecatedElements, tagname.ToLowerInvariant());
		}
		/// <summary>
		/// Метод проверки разрешенных атрибутов
		/// </summary>
		/// <param name="attributename"></param>
		/// <returns></returns>
		public static PortableHtmlSchemaErorr GetAttributeErrorState(string attributename){
			var aname = attributename.ToLowerInvariant();
			if (-1 != Array.IndexOf(DeprecatedAttributes, aname)){
				if (aname == "id" ||  aname == "style" || aname=="class"){
					return PortableHtmlSchemaErorr.CssAttributeDetected;
				}
				return PortableHtmlSchemaErorr.DeprecatedAttributeDetected;
			}
			if (DeprecatedAttributePrefixes.Any(aname.StartsWith)){
				if (aname.StartsWith("on")) return PortableHtmlSchemaErorr.EventAttributeDetected;
				return PortableHtmlSchemaErorr.AngularAttributeDetected;
			}
			return PortableHtmlSchemaErorr.None;
		}

		/// <summary>
		/// Валидизация исходного HTML
		/// </summary>
		/// <param name="srcHtml">Строка HTML для проверки соответствия PHTML</param>
		/// <param name="context">Контекст обработки PHTML (дополнительные настройки, опции, списки доверения)</param>
		/// <returns></returns>
		public static PortableHtmlContext Validate(string srcHtml, PortableHtmlContext context = null)
		{
			context = context ?? new PortableHtmlContext();
			if (string.IsNullOrWhiteSpace(srcHtml)){
				return context.SetError(PortableHtmlSchemaErorr.EmptyInput); 
			}
			var rootedHtml = "<root>" + srcHtml + "</root>";
			try{
				var xml = XElement.Parse(rootedHtml);
				
				var rootElementsCount = xml.Elements().Count();
				if (1 < rootElementsCount){
					return context.SetError(PortableHtmlSchemaErorr.NoRootTag); 
				}
				//необходимо эту проверку вызывать в том числе здесь,
				//так как иначе при передаче в XML могут быть проигнорированы трейловые комментарии
				var hasComment = xml.DescendantNodes().OfType<XComment>().Any();
				if (hasComment)
				{
					return context.SetError(PortableHtmlSchemaErorr.CommentsDetected); 
				}

				//необходимо эту проверку вызывать в том числе здесь,
				//так как иначе при передаче в XML могут быть проигнорированы трейловые инструкции
				var hasProcessing = xml.DescendantNodes().OfType<XProcessingInstruction>().Any();
				if (hasProcessing)
				{
					return context.SetError(PortableHtmlSchemaErorr.ProcessingInstructionsDetected); 
				}

				//необходимо эту проверку вызывать в том числе здесь,
				//так как иначе при передаче в XML могут быть проигнорированы трейловые CDATA
				var hasCData = xml.DescendantNodes().OfType<XCData>().Any();
				if (hasCData)
				{
					return context.SetError(PortableHtmlSchemaErorr.CdataDetected);
				}
				var realRoot = XElement.Parse(srcHtml,LoadOptions.SetLineInfo);

				if (realRoot.Name.LocalName != AllowedRootName){
					return context.SetError(PortableHtmlSchemaErorr.InvalidRootTag);
				}
				//hack way to provide addition info to method
				realRoot.AddAnnotation(CheckedBySourceCheckerAnnotation.Default);
				return Validate(realRoot,context);
			}
			catch (XmlException e){
				return context.SetError(PortableHtmlSchemaErorr.NonXml,e:e);
			}
			
		}
		/// <summary>
		/// Hacked class to provide typed-annotation to xml to disable not required checking of XML after 
		/// source string parsing
		/// </summary>
		private class CheckedBySourceCheckerAnnotation{public static readonly CheckedBySourceCheckerAnnotation Default=new CheckedBySourceCheckerAnnotation(); }
		

		/// <summary>
		/// Валидизация исходного XML
		/// </summary>
		/// <param name="srcXml">Элемент для проверки соответствия PHTML</param>
		/// <param name="context">Контекст обработки PHTML (дополнительные настройки, опции, списки доверения)</param>
		/// <returns></returns>
		public static PortableHtmlContext Validate(XElement srcXml, PortableHtmlContext context = null)
		{
			context = context ?? new PortableHtmlContext();
			context.SourceXml = srcXml;
			//hack way to provide addition info to method
			if (null == srcXml.Annotation<CheckedBySourceCheckerAnnotation>()){
				CheckRootElement(context);
				if (!context.IsActive) return context;
				CheckNoComments(context);
				if (!context.IsActive) return context;
				CheckNoProcessingInstructions(context);
				if (!context.IsActive) return context;
			}

			CheckDeprecatedElements(context);
			if (!context.IsActive) return context;
			CheckNamespaces(context);
			if (!context.IsActive) return context;
			CheckDeprecatedAttributes(context);
			if (!context.IsActive) return context;
			CheckUpperCase(context);
			if (!context.IsActive) return context;
			CheckEmptyElements(context);
			if (!context.IsActive) return context;
			CheckAnchorLinks(context);
			if (!context.IsActive) return context;
			CheckImageLinks(context);

			return context;
		}

		/// <summary>
		/// Проверка валидности настройки ссылок у A и IMG
		/// </summary>
		/// <param name="context"></param>
		private static void CheckImageLinks(PortableHtmlContext context){
			var srcXml = context.SourceXml;
			var images = srcXml.Descendants("img").Where(context.InChecking).ToArray();
			
			foreach (var img in images){
				var src = img.Attribute("src");
				if (null == src){
					context.SetError(PortableHtmlSchemaErorr.NoRequiredSrcAttributeInImg, img);
				}
				else{
					var error = context.GetUriTrustState(src.Value,true);
					if (error != PortableHtmlSchemaErorr.None){
						context.SetError(error, img);
					}
				}
			}

		
		}

		/// <summary>
		/// Проверка валидности настройки ссылок у A и IMG
		/// </summary>
		/// <param name="context"></param>
		private static void CheckAnchorLinks(PortableHtmlContext context)
		{
			var anchors = context.SourceXml.Descendants("a").Where(context.InChecking).ToArray();
			foreach (var anchor in anchors)
			{
				var href = anchor.Attribute("href");
				if (null == href)
				{
					context.SetError(PortableHtmlSchemaErorr.NoRequiredHrefAttributeInA, anchor);
				}
				else
				{
					var error = context.GetUriTrustState(href.Value, false);
					if (error != PortableHtmlSchemaErorr.None)
					{
						context.SetError(error, anchor);
					}
				}

				var target = anchor.Attribute("target");
				if (null != target){
					context.SetError(PortableHtmlSchemaErorr.DeprecatedAttributeDetected, a: target);
				}
			}
		}

		/// <summary>
		/// Проверяет наличие запрещенных атрибутов
		/// </summary>
		/// <param name="context"></param>
		private static void CheckUpperCase(PortableHtmlContext context){
			foreach (var e in context.SourceXml.DescendantsAndSelf().Where(context.InChecking))
			{
				if (e.Name.LocalName.ToLowerInvariant() != e.Name.LocalName){
					context.SetError(PortableHtmlSchemaErorr.UpperCaseDetected, e);
				}
				if (!context.InChecking(e)) continue;
				foreach (var a in e.Attributes()){
					if (a.Name.LocalName.ToLowerInvariant() != a.Name.LocalName){
						context.SetError(PortableHtmlSchemaErorr.UpperCaseDetected, a: a);
					}
				}
			}

		}

		/// <summary>
		/// Проверяет наличие запрещенных атрибутов
		/// </summary>
		/// <param name="context"></param>
		private static void CheckEmptyElements(PortableHtmlContext context){
			foreach (var e in context.SourceXml.Descendants().Where(context.InChecking))
			{
				if (e.Name.LocalName == EmptyRequiredElement){
					
					if (e.Nodes().Any()){
						context.SetError(PortableHtmlSchemaErorr.NonEmptyImg, e);

					}
				}
				else{
					if (string.IsNullOrWhiteSpace(e.Value) && !e.Descendants(EmptyRequiredElement).Any()){
						context.SetError(PortableHtmlSchemaErorr.EmptyElement, e);
					}
				}
			}

		}

		/// <summary>
		/// Проверяет наличие запрещенных атрибутов
		/// </summary>
		/// <param name="context"></param>
		private static void CheckDeprecatedAttributes(PortableHtmlContext context)
		{
			foreach (var e in context.SourceXml.DescendantsAndSelf().Where(context.InChecking))
			{
				foreach (var a in e.Attributes()){
					var state =  GetAttributeErrorState(a.Name.LocalName);
					if (state != PortableHtmlSchemaErorr.None){
						context.SetError(state, a:a);
					}					
				}
			}

		}

		/// <summary>
		/// Проверяет наличие запрещенных элементов из списка
		/// </summary>
		/// <param name="context"></param>
		private static void CheckDeprecatedElements(PortableHtmlContext context)
		{
			var deprecates =
				context.SourceXml.Descendants()
					 .Where(context.InChecking)
				      .Where(_ => !IsAllowedTag(_.Name.LocalName))
				      .ToArray();
			foreach (var e in deprecates){
				var error = (e.Name.LocalName.ToLowerInvariant() + "Detected").To<PortableHtmlSchemaErorr>();
				context.SetError(error, e);
			}

		}


		/// <summary>
		/// Проверка на отсутствие определения пространств имен
		/// </summary>
		/// <param name="context"></param>
		private static void CheckNamespaces(PortableHtmlContext context)
		{
			foreach (var e in context.SourceXml.DescendantsAndSelf().Where(context.InChecking)){
				if (e.Name.NamespaceName!=""){
					context.SetError(PortableHtmlSchemaErorr.NamespaceDeclarationDetected, e);
				}
				foreach (var a in e.Attributes()){
					if (a.Name.NamespaceName != "" || a.Name.LocalName == "xmlns"){
						context.SetError(PortableHtmlSchemaErorr.NamespaceDeclarationDetected, a: a);
					}
				}
			}
		}

		

		/// <summary>
		/// Проверка на отсутствие комментариев
		/// </summary>
		private static void CheckNoComments(PortableHtmlContext context)
		{
			var hasComment = context.SourceXml.DescendantNodes().OfType<XComment>().Any();
			if (hasComment){
				context.SetError(PortableHtmlSchemaErorr.CommentsDetected);
			}
		}
		/// <summary>
		/// Проверка на отсутствие инструкций препроцессора
		/// </summary>
		private static void CheckNoProcessingInstructions(PortableHtmlContext context)
		{
			var hasProcessingInstructions = context.SourceXml.DescendantNodes().OfType<XProcessingInstruction>().Any();
			if (hasProcessingInstructions){
				context.SetError(PortableHtmlSchemaErorr.ProcessingInstructionsDetected);
			}
		}

		/// <summary>
		/// Проверка наличия корневого элемента div в соответствии с 'has_root_container'
		/// </summary>
		/// <param name="context"></param>
		private static void CheckRootElement(PortableHtmlContext context){
			var srcXml = context.SourceXml;
			if (srcXml.Name.LocalName != AllowedRootName){
				context.SetError(PortableHtmlSchemaErorr.InvalidRootTag);
			}
		}
	}
}