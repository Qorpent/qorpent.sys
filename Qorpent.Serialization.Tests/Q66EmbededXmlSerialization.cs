using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;

namespace Qorpent.Serialization
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class Q66EmbededXmlSerialization
	{
		[Test]
		public void Q66Reproduce() {
			var xmlserializer = new XmlSerializer();
			var obj = new {a = 1, b = "x", x = new XElement("embed", new XAttribute("a", "b"))};
			var xml = xmlserializer.Serialize("result", obj);
			Console.WriteLine(xml);
			Assert.AreEqual(@"<result a=""1"" b=""x""><x><embed a=""b"" /></x></result>",xml);
		}
	}
}
