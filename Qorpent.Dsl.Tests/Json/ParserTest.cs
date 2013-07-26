using System;
using NUnit.Framework;
using Qorpent.Dsl.Json;

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
			Assert.AreEqual(@"{""in"":""content"",""type"":""text"",""actions"":[{""actiontype"":""select"",""code"":""title"",""selector"":""#title"",},{""actiontype"":""select"",""code"":""date"",""selector"":"".data"",},{""actiontype"":""select"",""code"":""text"",""selector"":""#content"",""type"":""xml"",},],}", item.ToString(false));
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
			Assert.AreEqual(@"Open:{;Str:in;Colon::;Str:content;Comma:,;Str:type;Colon::;Str:text;Comma:,;Str:actions;Colon::;OpenArray:[;Open:{;Str:actiontype;Colon::;Str:select;Comma:,;Str:code;Colon::;Str:title;Comma:,;Str:selector;Colon::;Str:#title;Comma:,;Close:};Comma:,;Open:{;Str:actiontype;Colon::;Str:select;Comma:,;Str:code;Colon::;Str:date;Comma:,;Str:selector;Colon::;Str:.data;Comma:,;Close:};Comma:,;Open:{;Str:actiontype;Colon::;Str:select;Comma:,;Str:code;Colon::;Str:text;Comma:,;Str:selector;Colon::;Str:#content;Comma:,;Str:type;Colon::;Str:xml;Close:};Comma:,;CloseArray:];Close:}", ts);
		}
	}
}