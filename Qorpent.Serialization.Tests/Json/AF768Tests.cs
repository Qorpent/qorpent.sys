using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Json;

namespace Qorpent.Serialization.Tests.Json
{
	/// <summary>
	/// Ошибка связанная с неправильной сериализацией Json в XML
	/// </summary>
	[TestFixture]
	public class AF768Tests
	{
		[Test]
		public void ItsNotReproducedNow() {
			var parser = new JsonParser();
			var xml = parser.ParseXml(@"{""0"":{""id"":""5:5"",""value"":""1"",""ri"":""m1111111_Б2_2013_2""}}");
			Console.WriteLine(xml);
		}

		[Test]
		public void SimpleJsonToXml() {
			var json = new JsonObject();
			json["0"] = new JsonValue("test");
			var xml = json.WriteToXml(null);
			Console.WriteLine(xml);
		}

		[Test]
		public void SimpleJsonObjectToXml()
		{
			var json = new JsonObject();
			json["0"] = new JsonObject();
			var xml = json.WriteToXml(null);
			Console.WriteLine(xml);
		}
	}
}
