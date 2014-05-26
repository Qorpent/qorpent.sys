using System;
using System.Xml.Linq;
using NUnit.Framework;
using Qorpent.Json;

namespace Qorpent.Serialization.Tests
{
    [TestFixture]
    public class XmlJson2WayConversionTest
    {

        private static void BaseTest(string literal)
        {
            var xml = new JsonParser().ParseXml(literal);
            Console.WriteLine(xml);
            var json = new XmlToJsonConverter().ConvertToJson(xml);
            json = Simplify(json); 
            Console.WriteLine(json);
            Assert.AreEqual(Simplify(literal),json);
        }

        public static string Simplify(string json) {
            json = SimplifyStep(json);
            json = SimplifyStep(json);
            json = SimplifyStep(json);
            return json;
        }

        private static string SimplifyStep(string json) {
            json = json.Trim().Replace("  ", " ").Replace("\t", "").Replace("\r", "").Replace("\n","").Replace(" : ", ":")
                       .Replace(" [", "[")
                       .Replace(" {", "{")
                       .Replace(" ]", "]")
                       .Replace(" }", "}")
                       .Replace("] ", "]")
                       .Replace("} ", "}")
                       .Replace("[ ", "[")
                       .Replace("{ ", "{")
                       .Replace(" \"", "\"")
                       .Replace("\" ", "\"")
                ;
            return json;
        }

        [TestCase("1")]
        [TestCase("true")]
        [TestCase("\"\"")]
        [TestCase("null")]
        [TestCase("false")]
        [TestCase("-1.22")]
        [TestCase("\"str\"")]
        [TestCase("\"str\\r\\n\"")]
        public void LiteralEquivalenceTranslation(string json) {
            BaseTest(json);
        }

		[Test]
		public void BugInEscapeInLiteral()
		{
			var json = new XmlToJsonConverter().ConvertToJson(XElement.Parse("<value __jsontype=\"String\">str\n</value>"));
			Console.WriteLine(json);
			Assert.AreEqual("\"str\\n\"", json);
		}
		[Test]
		public void BugInEscapeInLiteral2()
		{
			var xml = new JsonParser().ParseXml("\"str\\n\"");
			Console.WriteLine(xml.ToString());
			Assert.AreEqual("<value __jsontype=\"String\">str\r\n</value>", xml.ToString());
		}

        
        [TestCase("{}")]
        [TestCase("{\"a\":true}")]
        [TestCase("{\"a\":\"\"}")]
        [TestCase("{\"a\":null}")]
        [TestCase("{\"a\":true,\"b\":false}")]
        [TestCase("{\"a\":1,\"b\":2}")]
        [TestCase("{\"a\":1,\"b\":\"aa2\"}")]
        public void SimpleOneLevelObjectEquivalence(string json)
        {
            BaseTest(json);
            
        }

        [TestCase("[]")]
        [TestCase("[1,2]")]
        [TestCase("[1]")]
        [TestCase("[\"a\",\"b\"]")]
        [TestCase("[true,null,\"b\"]")]
        public void SimpleOneLevelArrayEquivalence(string json)
        {
            BaseTest(json);

        }


        [TestCase("[[]]")]
        [TestCase("[1,[1,2]]")]
        public void SimpleTwoLevelArrayEquivalence(string json)
        {
            BaseTest(json);

        }

        [TestCase("{\"a\":{}}")]
        [TestCase("{\"a\":{\"c\":1},\"b\":{\"d\":2}}")]
        public void SimpleTwoLevelObject(string json)
        {
            BaseTest(json);

        }


        [TestCase(@"{
  ""code"": ""znak_com_test1"",
  ""group"": ""znakcom"",
  ""hardscript"": """",
  ""name"": ""Знак.ком"",
  ""source"":""znak.com"",
  ""samples"": [
     ""http://znak.com/svrdl/articles/16-09-19-53/101205.html"",
     ""http://znak.com/tumen/articles/16-09-20-26/101208.html"",
     ""http://znak.com/urfo/articles/16-09-19-56/101206.html""
  ],
  ""script"": []
}
")]
        public void Q156Bug(string json)
        {
            BaseTest(json);

        }

        [TestCase(@"{ ""code"" : """", ""group"" : """", ""hardscript"" : """", ""name"" : """", ""RobotId"" : """", ""source"" : """", ""samples"" : [""http://znak.com/svrdl/articles/16-09-19-53/101205.html""], ""script"" : [] }")]
        public void Q157Bug(string json)
        {
            BaseTest(json);

        }


       
    }
}
