﻿using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.PortableHtml{
	/// <summary>
	/// Вспомогательный класс для проверки соответствия строки и/или XML стандарту PHTML
	/// </summary>
	public static class PortableHtmlSchema{

		/// <summary>
		/// Перечень запрещенных элементов
		/// </summary>
		public static readonly string[] DangerousElements = new[]{
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
		/// Набор разрешенных в Strict режиме элементов
		/// </summary>
		public static readonly string[] StrictElements = new[]{ 
			"p",
			"img",
			"a",
			"strong",
			"em",
			"u",
			"sub",
			"sup",
			"del",
			"span",
			
		};

		/// <summary>
		/// Набор разрешенных элементов - корневых контейнеров
		/// </summary>
		public static readonly string[] ParaElements = new[]{
			"p", "ul", "ol", "table"
		};

		/// <summary>
		/// Набор элементов, которые не могут содержать никакого текста
		/// </summary>
		public static readonly string[] NoTextElements = new[]{
			"ul","ol","br","table","thead","tbody","tr"
		};

		/// <summary>
		/// Списочные элементы
		/// </summary>
		public static readonly string[] ListElements = new[]{
			"ol",
			"ul",
			"li"
		};

		/// <summary>
		/// Табличные элементы
		/// </summary>
		public static readonly string[] TableElements = new[]{ 
			"table",
			"thead",
			"tbody",
			"tr",
			"th",
			"td"
		};
		/// <summary>
		/// Допустимое локальное имя для рута
		/// </summary>
		public const string AllowedRootName = "div";
		/// <summary>
		/// Имя тега BR
		/// </summary>
		public const string BrTag = "br";


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
					context.SetError(PortableHtmlSchemaErorr.NoRootTag); 
				}
				if (!context.IsActive) return context;
				//необходимо эту проверку вызывать в том числе здесь,
				//так как иначе при передаче в XML могут быть проигнорированы трейловые комментарии
				var hasComment = xml.DescendantNodes().OfType<XComment>().Any();
				if (hasComment)
				{
					context.SetError(PortableHtmlSchemaErorr.CommentsDetected); 
				}
				if (!context.IsActive) return context;
				//необходимо эту проверку вызывать в том числе здесь,
				//так как иначе при передаче в XML могут быть проигнорированы трейловые инструкции
				var hasProcessing = xml.DescendantNodes().OfType<XProcessingInstruction>().Any();
				if (hasProcessing)
				{
					context.SetError(PortableHtmlSchemaErorr.ProcessingInstructionsDetected); 
				}
				if (!context.IsActive) return context;

				//необходимо эту проверку вызывать в том числе здесь,
				//так как иначе при передаче в XML могут быть проигнорированы трейловые CDATA
				var hasCData = xml.DescendantNodes().OfType<XCData>().Any();
				if (hasCData)
				{
					context.SetError(PortableHtmlSchemaErorr.CdataDetected);
				}
				if (!context.IsActive) return context;
				var realRoot = XElement.Parse(srcHtml,LoadOptions.SetLineInfo);

				if (realRoot.Name.LocalName != AllowedRootName){
					context.SetError(PortableHtmlSchemaErorr.InvalidRootTag,realRoot);
				}
				if (!context.IsActive) return context;
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
			if (!context.IsActive) return context;
			CheckRootTextNodes(context);
			if (!context.IsActive) return context;
			CheckRootInlineElements(context);
			if (!context.IsActive) return context;
			CheckNestedParas(context);
			if (!context.IsActive) return context;
			CheckNoTextElementsText(context);
			if (!context.IsActive) return context;
			CheckNoTextElementsInlines(context);
			return context;
		}

		private static void CheckNestedParas(PortableHtmlContext context){
			foreach (var p in context.SourceXml.Descendants()){
				if(!context.IsActive)break;
				if (!context.InChecking(p)) continue;
				if (-1 != Array.IndexOf(ParaElements, p.Name.LocalName)){
					if (p.Parent != context.SourceXml){
						context.SetError(PortableHtmlSchemaErorr.NestedParaElements, p);
					}	
				}
			}
		}

		private static void CheckRootInlineElements(PortableHtmlContext context){
			foreach (var e in context.SourceXml.Elements()){
				if(!context.IsActive)break;
				if (-1 == Array.IndexOf(ParaElements, e.Name.LocalName)){
					context.SetError(PortableHtmlSchemaErorr.RootInline,e);
				}
			}
		}

		private static void CheckNoTextElementsText(PortableHtmlContext context)
		{
			foreach (var e in context.SourceXml.Descendants()){
				if(!context.IsActive)break;
				if(!context.InChecking(e))continue;
				if (-1 != Array.IndexOf(NoTextElements, e.Name.LocalName)){
					if (e.Nodes().OfType<XText>().Any()){
						context.SetError(PortableHtmlSchemaErorr.TextInNonTextElement,e);
					}
				}
			}
			
		}

		private static void CheckNoTextElementsInlines(PortableHtmlContext context){
			foreach (var e in context.SourceXml.Descendants()){
				if (!context.IsActive) break;
				if (!context.InChecking(e)) continue;
				if (-1 != Array.IndexOf(NoTextElements, e.Name.LocalName)){
					foreach (var sube in e.Elements()){
						if (-1 == Array.IndexOf(ParaElements, sube.Name.LocalName)){
							context.SetError(PortableHtmlSchemaErorr.InlineInNonTextElement, e);
						}
					}

				}
			}
		}

		private static void CheckRootTextNodes(PortableHtmlContext context)
		{
			if (context.SourceXml.Nodes().OfType<XText>().Any())
			{
				context.SetError(PortableHtmlSchemaErorr.RootText);
			}
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
				if(!context.IsActive)break;
				foreach (var a in e.Attributes()){
					if (!context.IsActive) break;
					if(!context.InChecking(e))break;
					var state =  context.GetAttributeErrorState(a.Name.LocalName,a.Parent.Name.LocalName);
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
			foreach (var e in context.SourceXml.Descendants().Where(context.InChecking)){
				if(!context.IsActive)break;
				var error = context.GetTagState(e.Name.LocalName);
				if (error != PortableHtmlSchemaErorr.None){
					context.SetError(error, e);
				}
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
				context.SetError(PortableHtmlSchemaErorr.InvalidRootTag, srcXml);
			}
		}
	}
}