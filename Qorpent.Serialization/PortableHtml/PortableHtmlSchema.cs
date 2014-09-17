using System;
using System.Linq;
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
		/// <returns></returns>
		public static PortableSchemaValidationResult Validate(string srcHtml){
			if (string.IsNullOrWhiteSpace(srcHtml)){
				return new PortableSchemaValidationResult{SchemaError = PortableHtmlSchemaErorr.EmptyInput};
			}
			var rootedHtml = "<root>" + srcHtml + "</root>";
			try{
				var xml = XElement.Parse(rootedHtml);
				
				var rootElementsCount = xml.Elements().Count();
				if (1 < rootElementsCount){
					return new PortableSchemaValidationResult{SchemaError = PortableHtmlSchemaErorr.NoRootTag};
				}
				//необходимо эту проверку вызывать в том числе здесь,
				//так как иначе при передаче в XML могут быть проигнорированы трейловые комментарии
				var hasComment = xml.DescendantNodes().OfType<XComment>().Any();
				if (hasComment)
				{
					return new PortableSchemaValidationResult { SchemaError = PortableHtmlSchemaErorr.CommentsDetected };
				}

				//необходимо эту проверку вызывать в том числе здесь,
				//так как иначе при передаче в XML могут быть проигнорированы трейловые инструкции
				var hasProcessing = xml.DescendantNodes().OfType<XProcessingInstruction>().Any();
				if (hasProcessing)
				{
					return new PortableSchemaValidationResult { SchemaError = PortableHtmlSchemaErorr.ProcessingInstructionsDetected };
				}

				//необходимо эту проверку вызывать в том числе здесь,
				//так как иначе при передаче в XML могут быть проигнорированы трейловые CDATA
				var hasCData = xml.DescendantNodes().OfType<XCData>().Any();
				if (hasCData)
				{
					return new PortableSchemaValidationResult { SchemaError = PortableHtmlSchemaErorr.CdataDetected };
				}
				var realRoot = XElement.Parse(srcHtml);

				if (realRoot.Name.LocalName != AllowedRootName){
					return new PortableSchemaValidationResult { SchemaError = PortableHtmlSchemaErorr.InvalidRootTag };
				}
				//hack way to provide addition info to method
				realRoot.AddAnnotation(CheckedBySourceCheckerAnnotation.Default);
				return Validate(realRoot);
			}
			catch (Exception e){
				return new PortableSchemaValidationResult{SchemaError = PortableHtmlSchemaErorr.NonXml, Exception = e};
			}
			
		}
		/// <summary>
		/// Hacked class to provide typed-annotation to xml to disable not required checking of XML after 
		/// source string parsing
		/// </summary>
		private class CheckedBySourceCheckerAnnotation{public static CheckedBySourceCheckerAnnotation Default=new CheckedBySourceCheckerAnnotation(); }
		/// <summary>
		/// Валидизация исходного XML
		/// </summary>
		/// <param name="srcXml">Элемент для проверки соответствия PHTML</param>
		/// <returns></returns>
		public static PortableSchemaValidationResult Validate(XElement srcXml){
			var result = new PortableSchemaValidationResult();
			//hack way to provide addition info to method
			if (null == srcXml.Annotation<CheckedBySourceCheckerAnnotation>()){
				CheckRootElement(srcXml, result);
				CheckNoComments(srcXml, result);
				CheckNoProcessingInstructions(srcXml, result);
			}
			CheckNamespaces(srcXml, result);
			CheckDeprecatedElements(srcXml,result);
			CheckDeprecatedAttributes(srcXml,result);
			return result;
		}
		/// <summary>
		/// Проверяет наличие запрещенных атрибутов
		/// </summary>
		/// <param name="srcXml"></param>
		/// <param name="result"></param>
		private static void CheckDeprecatedAttributes(XElement srcXml, PortableSchemaValidationResult result)
		{
			foreach (var d in srcXml.DescendantsAndSelf())
			{
				foreach (var a in d.Attributes()){
					result.SchemaError |= GetAttributeErrorState(a.Name.LocalName);
				}
			}

		}
		/// <summary>
		/// Проверяет наличие запрещенных элементов из списка
		/// </summary>
		/// <param name="srcXml"></param>
		/// <param name="result"></param>
		private static void CheckDeprecatedElements(XElement srcXml, PortableSchemaValidationResult result){
			var deprecates =
				srcXml.Descendants()
				      .Where(_ => !IsAllowedTag(_.Name.LocalName))
				      .Select(_ => _.Name.LocalName.ToLowerInvariant())
				      .Distinct()
				      .ToArray();
			foreach (var deprecate in deprecates){
				var error = (deprecate + "Detected").To<PortableHtmlSchemaErorr>();
				result.SchemaError |= error;
			}

		}


		/// <summary>
		/// Проверка на отсутствие определения пространств имен
		/// </summary>
		/// <param name="srcXml"></param>
		/// <param name="result"></param>
		private static void CheckNamespaces(XElement srcXml, PortableSchemaValidationResult result){
			foreach (var e in srcXml.DescendantsAndSelf()){
				if (e.Name.NamespaceName!=""){
					result.SchemaError|=PortableHtmlSchemaErorr.NamespaceDeclarationDetected;
					return;
				}
				if (e.Attributes().Any(attribute => attribute.Name.NamespaceName != "" || attribute.Name.LocalName=="xmlns")){
					result.SchemaError |= PortableHtmlSchemaErorr.NamespaceDeclarationDetected;
					return;
				}
			}
		}
		/// <summary>
		/// Проверка на отсутствие комментариев
		/// </summary>
		/// <param name="srcXml"></param>
		/// <param name="result"></param>
		private static void CheckNoComments(XElement srcXml, PortableSchemaValidationResult result){
			var hasComment = srcXml.DescendantNodes().OfType<XComment>().Any();
			if (hasComment){
				result.SchemaError |= PortableHtmlSchemaErorr.CommentsDetected;
			}
		}
		/// <summary>
		/// Проверка на отсутствие инструкций препроцессора
		/// </summary>
		/// <param name="srcXml"></param>
		/// <param name="result"></param>
		private static void CheckNoProcessingInstructions(XElement srcXml, PortableSchemaValidationResult result)
		{
			var hasProcessingInstructions = srcXml.DescendantNodes().OfType<XProcessingInstruction>().Any();
			if (hasProcessingInstructions)
			{
				result.SchemaError |= PortableHtmlSchemaErorr.ProcessingInstructionsDetected;
			}
		}

		/// <summary>
		/// Проверка наличия корневого элемента div в соответствии с 'has_root_container'
		/// </summary>
		/// <param name="srcXml"></param>
		/// <param name="result"></param>
		private static void CheckRootElement(XElement srcXml, PortableSchemaValidationResult result){
			if (srcXml.Name.LocalName != AllowedRootName){
				result.SchemaError |= PortableHtmlSchemaErorr.InvalidRootTag;
			}
		}
	}
}