using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using NUnit.Framework;
using Qorpent.Experiments;
using Qorpent.Utils.Extensions;

namespace Qorpent.Bridge.Tests.Utils {
    [TestFixture]
    public class Json2Test {
        [TestCase("true", true)]
        [TestCase("false", false)]
        [TestCase("null", null)]
        [TestCase("   true   ", true)]
        [TestCase("   false  ", false)]
        [TestCase("   null  ", null)]
        [TestCase("1", 1.0)]
        [TestCase("1.1", 1.1)]
        [TestCase("-1.1", -1.1)]
        [TestCase("2E3", 2000.0)]
        [TestCase("2.534e3", 2534.0)]
        [TestCase("-2e3", -2000.0)]
        [TestCase("-2E-3", -0.002)]
        [TestCase("2E-3", 0.002)]
        [TestCase("  -1.1  ", -1.1)]
        [TestCase("  2E3  ", 2000.0)]
        [TestCase("\"a\"", "a")]
        [TestCase(" \"a\" ", "a")]
        [TestCase(" \"abc\" ", "abc")]
        [TestCase(" \"ab c\" ", "ab c")]
        [TestCase(" \"ab\\rc\" ", "ab\rc")]
        [TestCase(" \"ab\\nc\" ", "ab\nc")]
        [TestCase(" \"ab\\\\c\" ", "ab\\c")]
        [TestCase(" \"ab/c\" ", "ab/c")]
        [TestCase(" \"ab\\u1D02c\" ", "abᴂc")]
        [TestCase(" \"ab\\u1d02c\" ", "abᴂc")]
        public void TestPrimitives(string src, object result) {
            Assert.AreEqual(result, Experiments.Json.Parse(src));
        }

        [TestCase("fals")]
        [TestCase("nul")]
        [TestCase("bull")]
        [TestCase("falser")]
        [TestCase(".1")]
        [TestCase("1.1.1")]
        [TestCase("-.1")]
        [TestCase("1-1")]
        [TestCase("1E")]
        [TestCase("1E--1")]
        [TestCase("1EE1")]
        [TestCase("1E1.1")]
        [TestCase("\"abc")]
        [TestCase("\"\\c\"")]
        [TestCase("\"\\uABC\"")]
        public void InvalidPrimitives(string src) {
            Assert.Throws<Exception>(() => {
                Experiments.Json.Parse(src);
            });
        }
        [Test]
        public void CanParseColonInString() {
            Experiments.Json.Parse("\"fff:fff\"");
            Experiments.Json.Parse("{\"a\":\"fff:fff\"}");
        }

        [TestCase("[ 1 , 2 , 3 ]")]
        [TestCase("[1 ,2 ,3 ]")]
        [TestCase("[1,2,3]")]
        public void CanReadNumericArray(string json) {
            CollectionAssert.AreEquivalent(new object[] {1, 2, 3}, Experiments.Json.Parse(json) as object[]);
        }

        [Test]
        public void CanReadMixedArray() {
            var json = " [ 12.3, true, false , \"hello\", null, [ 1, 3, 4] ] ";
            CollectionAssert.AreEquivalent(new object[] {12.3, true, false, "hello", null, new object[] {1, 3, 4}},
                Experiments.Json.Parse(json) as object[]);
        }

        [Test]
        public void CanParseSimpleObject() {
            var res = Experiments.Json.Parse("{}") as IDictionary<string, object>;
            Assert.NotNull(res);
            Assert.AreEqual(0, res.Count);
        }

        [Test]
        public void CanParseSimpleObjectWitParam() {
            var res = Experiments.Json.Parse("{\"x\":1 , \"y\" : 2}") as IDictionary<string, object>;
            Assert.NotNull(res);
            Assert.AreEqual(2, res.Count);
            Assert.AreEqual(1, res["x"]);
            Assert.AreEqual(2, res["y"]);
        }

        private const string complexJson = @"
{
  'abc' : 1 ,
  'x' : 'hello' ,
  'y'  : true,
  'z' : [ 1, 'he' , null ] ,
   'u' : {
       'a' : {

            'b': 2
         }

      } 
}
";

        [Test]
        public void CanSearch() {
            var obj = new {a = new {b = new {c = 1}}}.jsonify();
            Assert.AreEqual(1,obj.num("*.c"));

            obj = new { a = new { b = new []{new{ c = 1 } }} }.jsonify();
            Assert.AreEqual(1, obj.num("*.c"));
            var json = @"{""response"":{""GeoObjectCollection"":{""metaDataProperty"":{""GeocoderResponseMetaData"":{""request"":"", Асбест, Ленинградская, 26-2"",""found"":""1"",""results"":""10""}},""featureMember"":[{""GeoObject"":{""metaDataProperty"":{""GeocoderMetaData"":{""kind"":""house"",""text"":""Россия, Свердловская область, Асбест, Ленинградская улица, 26К2"",""precision"":""exact"",""AddressDetails"":{""Country"":{""AddressLine"":""Свердловская область, Асбест, Ленинградская улица, 26К2"",""CountryNameCode"":""RU"",""CountryName"":""Россия"",""AdministrativeArea"":{""AdministrativeAreaName"":""Свердловская область"",""SubAdministrativeArea"":{""SubAdministrativeAreaName"":""городской округ Асбест"",""Locality"":{""LocalityName"":""Асбест"",""Thoroughfare"":{""ThoroughfareName"":""Ленинградская улица"",""Premise"":{""PremiseNumber"":""26К2""}}}}}}}}},""description"":""Асбест, Свердловская область, Россия"",""name"":""Ленинградская улица, 26К2"",""boundedBy"":{""Envelope"":{""lowerCorner"":""61.445942 57.008617"",""upperCorner"":""61.462399 57.017595""}},""Point"":{""pos"":""61.454171 57.013106""}}}]}}}".jsonify();
            Assert.AreEqual("61.454171 57.013106",json.str("*.pos"));
        }


        [Test]
        public void CanParseComplexObject() {
            var res = Experiments.Json.Parse(complexJson.Replace("'", "\"")) as IDictionary<string, object>;

            Assert.AreEqual(1, res["abc"]);
            Assert.AreEqual("hello", res["x"]);
            Assert.AreEqual(true, res["y"]);
            CollectionAssert.AreEquivalent(new object[] {1, "he", null}, res["z"] as object[]);
            var u = res["u"] as Dictionary<string, object>;
            var a = u["a"] as Dictionary<string, object>;
            Assert.AreEqual(2, a["b"]);
        }



        [Test]
        public void CanGenerateJson() {
            var obj =
                new {
                    a = 1,
                    aa = 0,
                    b = "c",
                    d = new object[] {1, null, "e"},
                    f = new {g = true, h = false},
                    i = new object[] {},
                    k = new Dictionary<string, string>(),
                    l = new Dictionary<string, string> {{"m", "n"}}
                };
            var json = Experiments.Json.Stringify(obj).Replace("\"", "'");
            Console.WriteLine(json);
            Assert.AreEqual(
                "{'a':1,'aa':0,'b':'c','d':[1,null,'e'],'f':{'g':true,'h':false},'i':[],'k':{},'l':{'m':'n'}}", json);
        }

        [Test]
        public void CanGenerateJsonNotNullByDefault() {
            var obj =
                new {
                    a = 1,
                    aa = 0,
                    b = "c",
                    d = new object[] {1, null, "e"},
                    f = new {g = true, h = false},
                    i = new object[] {},
                    k = new Dictionary<string, string>(),
                    l = new Dictionary<string, string> {{"m", "n"}}
                };
            var json = Experiments.Json.Stringify(obj, "", SerializeMode.OnlyNotNull).Replace("\"", "'");
            Console.WriteLine(json);
            Assert.AreEqual("{'a':1,'b':'c','d':[1,null,'e'],'f':{'g':true},'l':{'m':'n'}}", json);
        }


        private static readonly object json =
            Experiments.Json.Parse(typeof (Json2Test).Assembly.ReadManifestResource("testperformance.json"));


        [TestCase("response.count", 24)]
        [TestCase("response.items[0][\"first_name\"]", "Дарья")]
        [TestCase("response.items[0].first_name", "Дарья")]
        public void GetJson(string path, object result) {
            Assert.AreEqual(result,Experiments.Json.Get(json,path));
        }

        [Test]
        public void ExtendTest() {
            var x = Experiments.Json.Jsonify(new {z=1,
                a = new {b = new object[] {1, 2, 3,new{_id=1,a=2},new{_id=2,b=4}}}});
            var y = Experiments.Json.Jsonify(new {b = 1,  z="_remove_", 
                a = new {c = 2, b = new object[] {4, 5 , new{_remove=1}, new{_id=1,b=3},new{_remove=new{_id=2}}}}});
            var merged = Experiments.Json.Stringify(JsonExtend.Extend(x, y)).Simplify(SimplifyOptions.SingleQuotes);
            Console.WriteLine(merged);
            Assert.AreEqual(@"{'a':{'b':[2,3,4,5,{'_id':1,'a':2,'b':3}],'c':2},'b':1}", merged);
        }
        
    }
}