using System;
using NUnit.Framework;
using Qorpent.Json;

namespace Qorpent.Dsl.Tests.Json {
	[TestFixture]
	public class ParserTest {
		private const string json = @"{
        'in' : 'content',
        'type' : 'text',
        'actions' : [
                {
                        'actiontype' : 'select',
                        'code' : 'title',
                        'selector' : '#title',
                },
                {
                        'actiontype' : 'select',
                        'code' : 'date',
                        'selector' : '.data',
                },
                {
                        'actiontype' : 'select',
                        'code' : 'text',
                        'selector' : '#content',
                        'type': 'xml'
                },
        ]
}";

		[Test]
		public void XmlTest() {
			var item = new JsonParser().Parse(json);
			var xml = item.WriteToXml();
			Console.WriteLine(xml.ToString());
		}

		[Test]
		public void MainParserTest() {
			var item = new JsonParser().Parse(json);
			Console.WriteLine(item.ToString(true));
			Console.WriteLine("---------------------------------------------");
			Console.WriteLine(item.ToString(false));
			Assert.AreEqual(@"{""in"":""content"",""type"":""text"",""actions"":[{""actiontype"":""select"",""code"":""title"",""selector"":""#title""},{""actiontype"":""select"",""code"":""date"",""selector"":"".data""},{""actiontype"":""select"",""code"":""text"",""selector"":""#content"",""type"":""xml""}]}", item.ToString(false));
		}
		[Test]
		[Explicit]
		[Repeat(10000)]
		public void MainParserTestRepeated()
		{
			 new JsonParser().Parse(json);
			
		}
		[Test]
		public void TokenizerTest() {
			var tokens = new Tokenizer().Tokenize(json);
			var ts = string.Join(";", tokens);
			Console.WriteLine(ts);
			Assert.AreEqual(@"BeginObject:{;String:in;Colon::;String:content;Comma:,;String:type;Colon::;String:text;Comma:,;String:actions;Colon::;OpenArray:[;BeginObject:{;String:actiontype;Colon::;String:select;Comma:,;String:code;Colon::;String:title;Comma:,;String:selector;Colon::;String:#title;Comma:,;CloseObject:};Comma:,;BeginObject:{;String:actiontype;Colon::;String:select;Comma:,;String:code;Colon::;String:date;Comma:,;String:selector;Colon::;String:.data;Comma:,;CloseObject:};Comma:,;BeginObject:{;String:actiontype;Colon::;String:select;Comma:,;String:code;Colon::;String:text;Comma:,;String:selector;Colon::;String:#content;Comma:,;String:type;Colon::;String:xml;CloseObject:};Comma:,;CloseArray:];CloseObject:}", ts);
		}
	}
}