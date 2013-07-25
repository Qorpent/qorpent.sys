using System;
using System.IO;
using System.Xml.Linq;
using NUnit.Framework;

namespace Qorpent.Serialization.Tests {
    [TestFixture]
    public  class Q18_Xml_To_Json_Bug_Test {
        JsonSerializer json = new JsonSerializer();
        private XElement data = XElement.Parse(@"<root><item Name=""_db.addconnection.action"" Help="""" 
ActionTypeName=""Qorpent.Data.REST.AddConnection, Qorpent.Data, Version=1.0.0.0, Culture=neutral, 
PublicKeyToken=9b7e542f177f0300"" DirectRole=""DEVELOPER"" Role=""!_DB_ADDCONNECTION_ACTION_DENY,DEVELOPER,_DB_ADDCONNECTION_ACTION_ALLOW"" 
__idx=""0""><Parameters><item key=""temporal""><value name=""temporal"" type=""Boolean"" help="""" required=""false"" pattern="""">
<fixedset /><defval /></value></item><item key=""name""><value name=""name"" type=""Str"" help="""" required=""true"" pattern="""">
<fixedset /><defval /></value></item><item key=""connection""><value name=""connection"" type=""Str"" help="""" required=""true"" 
pattern=""""><fixedset /><defval /></value></item></Parameters></item><item Name=""_db.removeconnection.action"" Help="""" 
ActionTypeName=""Qorpent.Data.REST.RemoveConnection, Qorpent.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9b7e542f177f0300"" 
DirectRole=""DEVELOPER"" Role=""!_DB_REMOVECONNECTION_ACTION_DENY,DEVELOPER,_DB_REMOVECONNECTION_ACTION_ALLOW"" __idx=""2"">
<Parameters><item key=""temporal""><value name=""temporal"" type=""Boolean"" help="""" required=""false"" pattern=""""><fixedset />
<defval /></value></item><item key=""name""><value name=""name"" type=""Str"" help="""" required=""true"" pattern="""">
<fixedset /><defval /></value></item></Parameters></item></root>
");

        private string badresult =
            @"{""item"": {""0"": {""Name"": ""_db.addconnection.action"", ""Help"": """", ""ActionTypeName"": ""Qorpent.Data.REST.AddConnection, Qorpent.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9b7e542f177f0300"", ""DirectRole"": ""DEVELOPER"", ""Role"": ""!_DB_ADDCONNECTION_ACTION_DENY,DEVELOPER,_DB_ADDCONNECTION_ACTION_ALLOW"", ""__idx"": ""0"", ""Parameters"": {""item"": {""0"": {""key"": ""temporal"", ""value"": {""name"": ""temporal"", ""type"": ""Boolean"", ""help"": """", ""required"": ""false"", ""pattern"": """", ""fixedset"": {}, ""defval"": {}}}, ""1"": {""key"": ""name"", ""value"": {""name"": ""name"", ""type"": ""Str"", ""help"": """", ""required"": ""true"", ""pattern"": """", ""fixedset"": {}, ""defval"": {}}}""2"": {""key"": ""connection"", ""value"": {""name"": ""connection"", ""type"": ""Str"", ""help"": """", ""required"": ""true"", ""pattern"": """", ""fixedset"": {}, ""defval"": {}}}, }, }}""1"": {""Name"": ""_db.removeconnection.action"", ""Help"": """", ""ActionTypeName"": ""Qorpent.Data.REST.RemoveConnection, Qorpent.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9b7e542f177f0300"", ""DirectRole"": ""DEVELOPER"", ""Role"": ""!_DB_REMOVECONNECTION_ACTION_DENY,DEVELOPER,_DB_REMOVECONNECTION_ACTION_ALLOW"", ""__idx"": ""2"", ""Parameters"": {""item"": {""0"": {""key"": ""temporal"", ""value"": {""name"": ""temporal"", ""type"": ""Boolean"", ""help"": """", ""required"": ""false"", ""pattern"": """", ""fixedset"": {}, ""defval"": {}}}""1"": {""key"": ""name"", ""value"": {""name"": ""name"", ""type"": ""Str"", ""help"": """", ""required"": ""true"", ""pattern"": """", ""fixedset"": {}, ""defval"": {}}}, }, }}, }, }";

        private string goodresult =
            @"{""item"": {""0"": {""Name"": ""_db.addconnection.action"", ""Help"": """", ""ActionTypeName"": ""Qorpent.Data.REST.AddConnection, Qorpent.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9b7e542f177f0300"", ""DirectRole"": ""DEVELOPER"", ""Role"": ""!_DB_ADDCONNECTION_ACTION_DENY,DEVELOPER,_DB_ADDCONNECTION_ACTION_ALLOW"", ""__idx"": ""0"", ""Parameters"": {""item"": {""0"": {""key"": ""temporal"", ""value"": {""name"": ""temporal"", ""type"": ""Boolean"", ""help"": """", ""required"": ""false"", ""pattern"": """", ""fixedset"": {}, ""defval"": {}}}, ""1"": {""key"": ""name"", ""value"": {""name"": ""name"", ""type"": ""Str"", ""help"": """", ""required"": ""true"", ""pattern"": """", ""fixedset"": {}, ""defval"": {}}}, ""2"": {""key"": ""connection"", ""value"": {""name"": ""connection"", ""type"": ""Str"", ""help"": """", ""required"": ""true"", ""pattern"": """", ""fixedset"": {}, ""defval"": {}}}}}}, ""1"": {""Name"": ""_db.removeconnection.action"", ""Help"": """", ""ActionTypeName"": ""Qorpent.Data.REST.RemoveConnection, Qorpent.Data, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9b7e542f177f0300"", ""DirectRole"": ""DEVELOPER"", ""Role"": ""!_DB_REMOVECONNECTION_ACTION_DENY,DEVELOPER,_DB_REMOVECONNECTION_ACTION_ALLOW"", ""__idx"": ""2"", ""Parameters"": {""item"": {""0"": {""key"": ""temporal"", ""value"": {""name"": ""temporal"", ""type"": ""Boolean"", ""help"": """", ""required"": ""false"", ""pattern"": """", ""fixedset"": {}, ""defval"": {}}}, ""1"": {""key"": ""name"", ""value"": {""name"": ""name"", ""type"": ""Str"", ""help"": """", ""required"": ""true"", ""pattern"": """", ""fixedset"": {}, ""defval"": {}}}}}}}}";

        [Test]
        public void Q18_Not_Reproduced_Yet() {
            var sw = new StringWriter();
            json.Serialize("test", data, sw);
            Console.WriteLine(sw.ToString());
            Assert.AreNotEqual(badresult,sw.ToString().Replace("  "," ").Trim());
        }

        [Test]
		[Explicit]
        public void Q18_Valid_Result()
        {
            var sw = new StringWriter();
            json.Serialize("test", data, sw);
            Console.WriteLine(sw.ToString());
            Assert.AreEqual(goodresult, sw.ToString().Replace("  ", " ").Trim());
        }

        [Test]
        public void Small_Test() {
            var sw = new StringWriter();
            json.Serialize("test", XElement.Parse("<r><a key='a'><i key='1'/><i key='2'/><i key='3'/></a><a key='b'><i key='1'/><i key='2'/><i key='3'/></a></r>"), sw);
            Console.WriteLine(sw.ToString());
            Assert.AreEqual(@"{""a"": {""0"": {""key"": ""a"", ""i"": {""0"": {""key"": ""1""}, ""1"": {""key"": ""2""}, ""2"": {""key"": ""3""}}}, ""1"": {""key"": ""b"", ""i"": {""0"": {""key"": ""1""}, ""1"": {""key"": ""2""}, ""2"": {""key"": ""3""}}}}}", sw.ToString().Replace("  ", " ").Trim());
        }

        [Test]
        public void Q19_Small_Test()
        {
            var sw = new StringWriter();
            json.Serialize("test", XElement.Parse("<r><item key='a'><item key='x'/><item key='y'/><item key='z'/></item><item key='b'><item key='x'/><item key='y'/><item key='z'/></item></r>"), sw);
            Console.WriteLine(sw.ToString());
            Assert.AreEqual(@"{""a"": {""x"": {}, ""y"": {}, ""z"": {}}, ""b"": {""x"": {}, ""y"": {}, ""z"": {}}}", sw.ToString().Replace("  ", " ").Trim());
        }
    }
}