using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;
using Qorpent.Json;

namespace Qorpent.Serialization.Tests.Json
{
	[TestFixture]
	public class InvalidXmlToJson
	{
		[Test]
		public void IncorrectSerializingListOfObjects() {
			var objList = new List<object> {new {a = "a"}, new {a = "b"}};
			var serializer = new JsonSerializer();
			var json = serializer.Serialize("root", objList);
			json = json.Replace("\"", "'").Replace(" ", "");
			Console.WriteLine(json);
			Assert.AreEqual("[{'a':'a'},{'a':'b'}]", json);
		}
		[Test]
		public void NoTrailCommas(){
			var xml = XElement.Parse(@"
<a a='b'>
	<int code='x' name='xx'/>
	<int code='y' name='yy'/>	
	<int code='z' name='zz'/>	
</a>
");
			
			var json = new JsonSerializer().Serialize("root", xml);
			var ajson = json.Replace("\"", "'");
			Console.WriteLine(ajson);
			Assert.AreEqual("{'a': 'b', 'int': [{'code': 'x', 'name': 'xx'}, {'code': 'y', 'name': 'yy'}, {'code': 'z', 'name': 'zz'}]}",ajson);
			new JsonParser().Parse(json);
		}


		[Test]
		public void ReproduceBadParseOfSpecification(){
			var xml = XElement.Parse(text: @"<a>
  <i a='b' />
  <i a='c'  />
  <s a='d' />
  <s a='e'  />
</a>");
			var json = new JsonSerializer().Serialize("root", xml);
			var ajson = json.Replace("\"", "'");
			Console.WriteLine(ajson);
			Assert.AreEqual("{'i': [{'a': 'b'}, {'a': 'c'}], 's': [{'a': 'd'}, {'a': 'e'}]}", ajson);
			new JsonParser().Parse(json);
		}
	}
}
