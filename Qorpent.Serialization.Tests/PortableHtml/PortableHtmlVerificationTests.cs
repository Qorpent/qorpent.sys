using System;
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
		private void testSchema(object srcHtml, bool isValid, PortableHtmlSchemaErorr error, Type exceptionType){
			var result =(srcHtml is string || null==srcHtml)? PortableHtmlSchemaValidator.Validate((string)srcHtml):PortableHtmlSchemaValidator.Validate((XElement)srcHtml);
			Assert.AreEqual(isValid,result.Ok,"Общий статус валидации неверен");
			Assert.AreEqual(error,result.SchemaError, "Статус ошибки валидации неверен");
			if (null != exceptionType){
				Assert.NotNull(result.Exception,"Ожидалось исключение");
				Assert.AreEqual(exceptionType,result.Exception.GetType(),"Исключение имеет неверный тип");
			}
		}

		
		[TestCase("<div></div>", true, PortableHtmlSchemaErorr.None, Description = "Требуется тег div")]
		[TestCase("<DIV></DIV>", false, PortableHtmlSchemaErorr.InvalidRootTag, Description = "Требуется тег div")]
		[TestCase("<p></p>", false, PortableHtmlSchemaErorr.InvalidRootTag, Description = "Требуется тег div")]
		[TestCase("<p></p><p></p>", false, PortableHtmlSchemaErorr.NoRootTag, Description = "Фрагменты не разрешены")]
		[TestCase("<div></div><div></div>",false,PortableHtmlSchemaErorr.NoRootTag,Description = "Фрагменты не разрешены")]
		[Test(Description = "Выполнение требования 'has_root_container'")]
		public void RootDivElementRequired(string srcHtml, bool isValid, PortableHtmlSchemaErorr error){
			testSchema(srcHtml,isValid,error,null);
		}

		[TestCase("<div></div>", true, PortableHtmlSchemaErorr.None, Description = "Требуется тег div")]
		[TestCase("<DIV></DIV>", false, PortableHtmlSchemaErorr.InvalidRootTag, Description = "Требуется тег div")]
		[TestCase("<p></p>", false, PortableHtmlSchemaErorr.InvalidRootTag, Description = "Требуется тег div")]
		[Test(Description = "Выполнение требования 'has_root_container'")]
		public void RootDivElementRequiredCover(string srcHtml, bool isValid, PortableHtmlSchemaErorr error)
		{
			testSchema(XElement.Parse(srcHtml), isValid, error, null);
		}

		[TestCase("", Description = "Требуется не пустой HTML на входе")]
		[TestCase("  ", Description = "Требуется не пустой HTML на входе")]
		[TestCase("				\r\n", Description = "Требуется не пустой HTML на входе")]
		[TestCase((string)null, Description = "Требуется не пустой HTML на входе")]
		[Test(Description = "Выполнение требования 'xml_compliant','html5_compliant'")]
		public void InvalidOnEmptyString(string srcHtml)
		{
			testSchema(srcHtml, false, PortableHtmlSchemaErorr.EmptyInput, null);
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
			testSchema(srcHtml, false, PortableHtmlSchemaErorr.CommentsDetected, null);
		}

		[TestCase("<div><p><!--x--></p></div>", Description = "Комментарии запрещены")]
		[TestCase("<div><!--x--></div>", Description = "Комментарии запрещены")]
		[Test(Description = "Выполнение требования 'no_comments'")]
		public void CommentsAreNotAllowedCover(string srcHtml)
		{
			testSchema(XElement.Parse(srcHtml), false, PortableHtmlSchemaErorr.CommentsDetected, null);
		}
		
		
		[TestCase("<?some ?><div></div>", Description = "Инструкции процессинга запрещены")]
		[TestCase("<div><?some ?></div>", Description = "Инструкции процессинга запрещены")]
		[TestCase("<div></div><?some ?>", Description = "Инструкции процессинга запрещены")]
		[Test(Description = "Выполнение требования 'no_processing'")]
		public void ProcessingInstructionsAreNotAllowed(string srcHtml)
		{
			testSchema(srcHtml, false, PortableHtmlSchemaErorr.ProcessingInstructionsDetected, null);
		}
		[TestCase("<div><?some ?></div>", Description = "Инструкции процессинга запрещены")]
		[Test(Description = "Выполнение требования 'no_processing'")]
		public void ProcessingInstructionsAreNotAllowedCover(string srcHtml)
		{
			testSchema(XElement.Parse(srcHtml), false, PortableHtmlSchemaErorr.ProcessingInstructionsDetected, null);
		}

		[TestCase("<div xmlns:x='a'></div>",  Description = "Пространства имен не поддерживаются")]
		[TestCase("<div xmlns=''></div>",  Description = "Пространства имен не поддерживаются")]
		[TestCase("<div xmlns='a'></div>", Description = "Пространства имен не поддерживаются")]
		[Test(Description = "Выполнение требования 'no_namespace'")]
		public void NamespacesAreNotAllowed(string srcHtml)
		{
			testSchema(srcHtml, false, PortableHtmlSchemaErorr.NamespaceDeclarationDetected, null);
		}
		[TestCase("<![CDATA[x]]><div></div>", Description = "Блоки CDATA запрещены")]
		[TestCase("<div><![CDATA[x]]></div>", Description = "Блоки CDATA запрещены")]
		[TestCase("<div></div><![CDATA[x]]>", Description = "Блоки CDATA запрещены")]
		[Test(Description = "Выполнение требования 'no_cdata'")]
		public void CDataIsNotAllowed(string srcHtml){
			testSchema(srcHtml, false, PortableHtmlSchemaErorr.CdataDetected, null);
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
		[TestCase("textarea",	Description = "Запрещенный тег")]
		[Test(Description = "Выполнение требования 'deprecated_tags'")]
		public void DangerousTagsTestNotAllowed(string tagName){
			var html = "<div><" + tagName + "/></div>";
			var error = (tagName + "Detected").To<PortableHtmlSchemaErorr>();
			testSchema(html, false, error, null);
			html = "<div><" + tagName.ToUpper() + "/></div>";
			testSchema(html, false, error, null);
		}
	}
}
