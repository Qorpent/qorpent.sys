using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;
using Qorpent.IntermediateFormat;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Tests.IntermediateFormat
{
	[TestFixture]
	public class IntermediateDocumentTest
	{
		[Serialize]
		class TestClass{
			public int x;
			public int y;
			public int z;
		}
		[Test]
		public void BasicFormatting(){
			var doc = new IntermediateFormatDocument{
				Code = "code1",
				Name = "name1",
				Prototype = "prototype1",
				Layer = IntermediateFormatLayer.StandaloneDocument
			};
			doc.Set("test", new TestClass { x = 1, y = 2 });
			doc.Set("best", "best");
			var subdoc =  new IntermediateFormatDocument{
				Code = "code2",
				Name = "name2",
				Prototype = "prototype2",
				Layer = IntermediateFormatLayer.Row
			};
			subdoc.Set("test2", new TestClass { x = 3, y = 4, z = 5 });
			subdoc.Set("vest", "vest");
			doc.AddChildDocument(subdoc);


			var xml = doc.ToXml();
			
			PrintXml(xml);
			Assert.AreEqual(@"<document code=""code1"" name=""name1"" prototype=""prototype1"" layer=""StandaloneDocument"" best=""best"">
  <item code=""test"" type=""TestClass"">
    <body x=""1"" y=""2"" z=""0"" />
  </item>
  <document code=""code2"" name=""name2"" prototype=""prototype2"" layer=""Row"" vest=""vest"">
    <item code=""test2"" type=""TestClass"">
      <body x=""3"" y=""4"" z=""5"" />
    </item>
  </document>
</document>".Trim().LfOnly(), xml.ToString().Trim().LfOnly());
		}

		private static void PrintXml(XElement xml){
			Console.WriteLine(xml.ToString());
			Console.WriteLine();
			Console.WriteLine("---------------------------");
			Console.WriteLine();
			Console.WriteLine(xml.ToString().Replace("\"", "\"\""));
		}
	}
}
