using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Uson;

namespace Qorpent.Serialization.Tests.Uson
{
	[TestFixture]
	public class UsonMainTest
	{
		class test
		{
			public int A;
			public int B;
		}
		[Test]
		public void CanCreateAndUse()
		{
			dynamic uobj = new UObj();
			//one field 
			uobj.a.b.c = 1;
			// second field (not alphabetic order)
			uobj.a.b.a = "tren\nd";
			//only GET will be treated as garbage
			var a = uobj.a.b.u.c.d.f;
			Assert.AreEqual(1,uobj.a.b.c);
			Console.WriteLine(uobj.ToJson());
			//test valid JSON in alphabet mode
			Assert.AreEqual(@"{""a"":{""b"":{""a"":""tren\nd"",""c"":1}}}", uobj.ToJson());
			uobj.a = new test {A = 3, B = 4};
			Console.WriteLine(uobj.ToJson());
			Assert.AreEqual(@"{""a"":{""A"":3,""B"":4}}", uobj.ToJson());
			Console.WriteLine(uobj.ToJson(UObjSerializeMode.KeepType));
			Assert.AreEqual(@"{""a"":{""_srctype"":""Qorpent.Serialization.Tests.Uson.UsonMainTest+test, Qorpent.Serialization.Tests"",""A"":3,""B"":4}}", uobj.ToJson(UObjSerializeMode.KeepType));
			uobj.a = new object[] {"x", 1, true, new DateTime(2002, 5, 2)};
			Console.WriteLine(uobj.ToJson());
			Assert.AreEqual(@"{""a"":[""x"",1,true,""2002-05-02T00:00:00""]}", uobj.ToJson());
			Assert.AreEqual(1,uobj.a[1]);
			Assert.AreEqual(null,uobj.a[10]);
		}

		[Test]
		public void FromDictionary()
		{
			var x = new Dictionary<object, object> {{1, 2}, {"x", false}, {"z", "ccc"}}.ToUson();
			Console.WriteLine(x.ToJson());
			Assert.AreEqual(@"{""1"":2,""x"":false,""z"":""ccc""}",x.ToJson());
		}
		[Test]
		public void UsdJson()
		{
			var x = "{\"$query\":{\"x\":1}}".ToUson();
			Console.WriteLine(x.ToJson());
			Assert.AreEqual("{\"$query\":{\"x\":1}}", x.ToJson());
		}

		[Test]
		public void KeepNumberLikeStringsAsStrings() {
			var obj = new {x = "2.3", y = "2,3"};
			var uobj = obj.ToUson();
			Assert.AreEqual(@"{""x"":2.3,""y"":""2,3""}",uobj.ToJson());
		}

		[Test]
		public void PreventLongsToBeTreatedAsNumbersDueToJavascriptOverflow() {
			var l = 88888888888888888;
			var obj = new { x = l, y = l.ToString() };
			var uobj = obj.ToUson();
			Assert.AreEqual(@"{""x"":""88888888888888888"",""y"":""88888888888888888""}", uobj.ToJson());
		}

		void t(string data, bool data2){
			Console.WriteLine(data,data2);	
		}
		[Test]
		public void CanSupplyDefaultParameters(){
			dynamic o = new UObj();
			t(o.first,o.second);
		}


		[Test]
		public void FromJson()
		{
			var x = "{\"x\":23,\"y\":\"a\"}".ToUson();
			Console.WriteLine(x.ToJson());
			Assert.AreEqual("{\"x\":23,\"y\":\"a\"}",x.ToJson());
		}

		[Test]
		public void Complex()
		{
			var uobj = new {x = "{\"y\":1}"}.ToUson();
			Console.WriteLine(uobj.ToJson());
			Assert.AreEqual(@"{""x"":{""y"":1}}",uobj.ToJson());
		}

		[Test]
		public void ArrayByIndexTest()
		{
			dynamic uobj = new UObj();
			uobj.x[3] = 1;
			Console.WriteLine(uobj.ToJson());
			Assert.AreEqual(@"{""x"":[null,null,null,1]}", uobj.ToJson());
		}

		[Test]
		public void Extend()
		{
			dynamic uobj = new UObj();
			dynamic uobj2 = new UObj();
			uobj.a = 1;
			uobj.b = 2;
			uobj2.a = 3;
			uobj2.c = 4;
			uobj.extend(uobj2);
			Console.WriteLine(uobj.ToJson());
			Assert.AreEqual(@"{""a"":3,""b"":2,""c"":4}", uobj.ToJson());
		}

		[Test]
		public void ArrayExtend()
		{
			dynamic uobj = new UObj();
			dynamic uobj2 = new UObj();
			uobj.push(null,1,null,2,null,3);
			uobj2.push(-1, 2, -2, null, -3, 4);
			uobj.extend(uobj2);
			Console.WriteLine(uobj.ToJson());
			Assert.AreEqual(@"[-1,2,-2,null,-3,4]", uobj.ToJson());
		}

		[Test]
		public void XmlRenderTest()
		{
			var uobj = new {a = 1, b = new {x = "x", c = new DateTime(2012, 1, 1)},d=new object[]{null,1,"z"}}.ToUson();
			Console.WriteLine(uobj.ToXmlString());
			Assert.AreEqual(@"<result a=""1""><b x=""x"" c=""2012-01-01T12:00:00"" /><d _array=""true""><item /><item>1</item><item>z</item></d></result>",uobj.ToXmlString());
			var xe = uobj.ToXElement();
			Console.WriteLine(xe.ToString());
			Assert.AreEqual(
@"<result a=""1"">
  <b x=""x"" c=""2012-01-01T12:00:00"" />
  <d _array=""true"">
	<item />
	<item>1</item>
	<item>z</item>
  </d>
</result>", xe.ToString());
		}

		[Test]
		public void ArrayDefaults()
		{
			dynamic uobj = new UObj();
			dynamic uobj2 = new UObj();
			uobj.push(null, 1, null, 2, null, 3);
			uobj2.push(-1, 2, -2, null, -3, 4);
			uobj.defaults(uobj2);
			Console.WriteLine(uobj.ToJson());
			Assert.AreEqual(@"[-1,1,-2,2,-3,3]", uobj.ToJson());
		}

		[Test]
		public void DeepExtend()
		{
			dynamic uobj = new UObj();
			dynamic uobj2 = new UObj();
			uobj.a = new {x=1,y=2};
			uobj.b = new {a=3,b=4};
			uobj2.a = new {x=2,z=3};
			uobj2.c = new {a=4,c=5};
			uobj.deepextend(uobj2);
			Console.WriteLine(uobj.ToJson());
			Assert.AreEqual(@"{""a"":{""x"":2,""y"":2,""z"":3},""b"":{""a"":3,""b"":4},""c"":{""a"":4,""c"":5}}", uobj.ToJson());
		}

		[Test]
		public void DeepDefaults()
		{
			dynamic uobj = new UObj();
			dynamic uobj2 = new UObj();
			uobj.a = new { x = 1, y = 2 };
			uobj.b = new { a = 3, b = 4 };
			uobj2.a = new { x = 2, z = 3 };
			uobj2.c = new { a = 4, c = 5 };
			uobj.deepdefaults(uobj2);
			Console.WriteLine(uobj.ToJson());
			Assert.AreEqual(@"{""a"":{""x"":1,""y"":2,""z"":3},""b"":{""a"":3,""b"":4},""c"":{""a"":4,""c"":5}}", uobj.ToJson());
		}


		[Test]
		public void DeepDefaultsWithAutoUson()
		{

			var obj1 = new { a = new { x = 1, y = 2 }, b = new { a = 3, b = 4 } };
			var obj2 = new {a = new {x = 2, z = 3}, c = new {a = 4, c = 5}};
			var uson = obj1.UsonDeepDefaults(obj2);
			Console.WriteLine(uson.ToJson());
			Assert.AreEqual(@"{""a"":{""x"":1,""y"":2,""z"":3},""b"":{""a"":3,""b"":4},""c"":{""a"":4,""c"":5}}", uson.ToJson());
		}


		[Test]
		public void Defaults()
		{
			dynamic uobj = new UObj();
			dynamic uobj2 = new UObj();
			uobj.a = 1;
			uobj.b = 2;
			uobj2.a = 3;
			uobj2.c = 4;
			uobj.defaults(uobj2);
			Console.WriteLine(uobj.ToJson());
			Assert.AreEqual(@"{""a"":1,""b"":2,""c"":4}", uobj.ToJson());
		}

		[Test]
		public void ArrayByPushTest()
		{
			dynamic uobj = new UObj();
			uobj.x.push(1);
			uobj.x.push(2);
			uobj.x.push(3);			
			Console.WriteLine(uobj.ToJson());
			Assert.AreEqual(@"{""x"":[1,2,3]}", uobj.ToJson());
		}


		[Test]
		public void IntToJson(){
			var o = 3.ToUson();
			Assert.AreEqual("3",o.ToJson());
		}
	}

	
}
