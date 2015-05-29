using System;
using System.Diagnostics;
using NUnit.Framework;
using qorpent.v2.security.user;
using Qorpent.Bxl;
using Qorpent.Core.Tests.Experiments;
using Qorpent.Experiments;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.Tests.user
{
    [TestFixture]
    public class UserSerializationTest:BaseFixture
    {
        [Test]
        public void CanWriteJson() {
            var user = GetUser(0);
            var js = UserSerializer.GetJson(user, "store").Simplify(SimplifyOptions.SingleQuotes);
            Console.WriteLine(js);
            Assert.AreEqual(@"{'_id':null,'_version':0,'createtime':'0001-01-01T00:00:00+0000','updatetime':'0001-01-01T00:00:00+0000','class':'user','_type':'user','netclass':'qorpent.v2.security.user.User, qorpent.v2.security','login':'login0','name':'myname0','email':'email0','admin':true,'active':false,'expire':'2015-12-31T19:00:00+0000','hash':'35d3986f45fbb6c6f9c22f741a4e7245','salt':'salt0','resetkey':null,'resetexpire':'0001-01-01T00:00:00+0000','publickey':'LS0tLS1CRUdJTiBDRVJUSUZJQ0FURS0tLS0tCk1JSUNXRENDQWNHZ0F3SUJBZ0lKQUpaN04rWnJ5YnUrTUEwR0NTcUdTSWIzRFFFQkJRVUFNRVV4Q3pBSkJnTlYKQkFZVEFrRlZNUk13RVFZRFZRUUlEQXBUYjIxbExWTjBZWFJsTVNFd0h3WURWUVFLREJoSmJuUmxjbTVsZENCWAphV1JuYVhSeklGQjBlU0JNZEdRd0hoY05NVFV3TlRJMk1UUTFPRE15V2hjTk1UWXdOVEkxTVRRMU9ETXlXakJGCk1Rc3dDUVlEVlFRR0V3SkJWVEVUTUJFR0ExVUVDQXdLVTI5dFpTMVRkR0YwWlRFaE1COEdBMVVFQ2d3WVNXNTAKWlhKdVpYUWdWMmxrWjJsMGN5QlFkSGtnVEhSa01JR2ZNQTBHQ1NxR1NJYjNEUUVCQVFVQUE0R05BRENCaVFLQgpnUURiNUQ1QncySjMyRURUK1BiVHB6SC9PYVYwcUlqc2VQV3hlWnVTS2tMbHhnVFQ3RWZZTHNnK3phYldmamw0CjhUdTgxZ3lCSVB5dUdkTzVwWkxkYXoxUU5pMFMxcmIvY3RONG9mQjhGb2FJbXcvSFF0R1NuZ21uMTBGOHlzRmwKOG8zL010TTBadXBjNkY0ZUlrU25kblA0cVlZOFNSZ2QvWG0zUHNuYiswbEtHUUlEQVFBQm8xQXdUakFkQmdOVgpIUTRFRmdRVWI2cnFXWXBhWnkraE11amYxNE9FS00ycXZIa3dId1lEVlIwakJCZ3dGb0FVYjZycVdZcGFaeStoCk11amYxNE9FS00ycXZIa3dEQVlEVlIwVEJBVXdBd0VCL3pBTkJna3Foa2lHOXcwQkFRVUZBQU9CZ1FCaVR4aTIKZmxEQXFONWE1dVRBNXFHUUc1SVpDNVNXRi9LektnUUhmMDgwbTM1MWVDVUxyd3Y2TG9RNzE4NDJyUmFtdmlXawp6QVVQb0NWWVNZQUwvb3JONVNrQWJiVGV6VVlFQTRTMDdxWXd1Vkt4L3llUTBOWG1CT2lIak1pU210MkVUcldPCmRpeTErQXg0bVZ2R2EwWlBLTWhFZjB4VGRzUEV5SmNFWlM3V3h3PT0KLS0tLS1FTkQgQ0VSVElGSUNBVEUtLS0tLQo=','logable':true,'mastergroup':'master0','roles':['role1_0','role2_0'],'groups':['grp1_0','grp2_0'],'custom':{'a':'1_0','b':'2_0'}}", js);
        }

        [Test]
        public void CanReadJson() {
            var user =
                UserSerializer.CreateFromJson(
                    @"{'_id':null,'_version':0,'createtime':'0001-01-01T00:00:00+0000','updatetime':'0001-01-01T00:00:00+0000','class':'user','_type':'user','netclass':'qorpent.v2.security.user.User, qorpent.v2.security','login':'login0','name':'myname0','email':'email0','admin':true,'active':false,'expire':'2015-12-31T19:00:00+0000','hash':'35d3986f45fbb6c6f9c22f741a4e7245','salt':'salt0','resetkey':null,'resetexpire':'0001-01-01T00:00:00+0000','publickey':'LS0tLS1CRUdJTiBDRVJUSUZJQ0FURS0tLS0tCk1JSUNXRENDQWNHZ0F3SUJBZ0lKQUpaN04rWnJ5YnUrTUEwR0NTcUdTSWIzRFFFQkJRVUFNRVV4Q3pBSkJnTlYKQkFZVEFrRlZNUk13RVFZRFZRUUlEQXBUYjIxbExWTjBZWFJsTVNFd0h3WURWUVFLREJoSmJuUmxjbTVsZENCWAphV1JuYVhSeklGQjBlU0JNZEdRd0hoY05NVFV3TlRJMk1UUTFPRE15V2hjTk1UWXdOVEkxTVRRMU9ETXlXakJGCk1Rc3dDUVlEVlFRR0V3SkJWVEVUTUJFR0ExVUVDQXdLVTI5dFpTMVRkR0YwWlRFaE1COEdBMVVFQ2d3WVNXNTAKWlhKdVpYUWdWMmxrWjJsMGN5QlFkSGtnVEhSa01JR2ZNQTBHQ1NxR1NJYjNEUUVCQVFVQUE0R05BRENCaVFLQgpnUURiNUQ1QncySjMyRURUK1BiVHB6SC9PYVYwcUlqc2VQV3hlWnVTS2tMbHhnVFQ3RWZZTHNnK3phYldmamw0CjhUdTgxZ3lCSVB5dUdkTzVwWkxkYXoxUU5pMFMxcmIvY3RONG9mQjhGb2FJbXcvSFF0R1NuZ21uMTBGOHlzRmwKOG8zL010TTBadXBjNkY0ZUlrU25kblA0cVlZOFNSZ2QvWG0zUHNuYiswbEtHUUlEQVFBQm8xQXdUakFkQmdOVgpIUTRFRmdRVWI2cnFXWXBhWnkraE11amYxNE9FS00ycXZIa3dId1lEVlIwakJCZ3dGb0FVYjZycVdZcGFaeStoCk11amYxNE9FS00ycXZIa3dEQVlEVlIwVEJBVXdBd0VCL3pBTkJna3Foa2lHOXcwQkFRVUZBQU9CZ1FCaVR4aTIKZmxEQXFONWE1dVRBNXFHUUc1SVpDNVNXRi9LektnUUhmMDgwbTM1MWVDVUxyd3Y2TG9RNzE4NDJyUmFtdmlXawp6QVVQb0NWWVNZQUwvb3JONVNrQWJiVGV6VVlFQTRTMDdxWXd1Vkt4L3llUTBOWG1CT2lIak1pU210MkVUcldPCmRpeTErQXg0bVZ2R2EwWlBLTWhFZjB4VGRzUEV5SmNFWlM3V3h3PT0KLS0tLS1FTkQgQ0VSVElGSUNBVEUtLS0tLQo=','logable':true,'mastergroup':'master0','roles':['role1_0','role2_0'],'groups':['grp1_0','grp2_0'],'custom':{'a':'1_0','b':'2_0'}}"
                        .Replace("'", "\""));
            Assert.NotNull(user);
            var user1 = GetUser(0);
            Assert.AreEqual(user,user1);
            Assert.AreEqual(user.GetToken(),user1.GetToken());
        }

        [Test]
        public void CanReadXml() {
            var e = new BxlParser().Parse(@"
user login1 name1 hash=hash1 salt=salt1 publickey=key1
    master = master1
    role role1
    role role2
    group grp1
    group grp2
    custom a=1 b=2
","code",BxlParserOptions.ExtractSingle);
            var u = new User();
            UserSerializer.ReadXml(u,e);
            Assert.AreEqual("login1",u.Login);
            Assert.AreEqual("name1", u.Name);
            Assert.AreEqual("hash1", u.Hash);
            Assert.AreEqual("salt1", u.Salt);
            Assert.AreEqual("key1", u.PublicKey);
            Assert.AreEqual("master1", u.MasterGroup);
            Assert.AreEqual("role1",u.Roles[0]);
            Assert.AreEqual("role2", u.Roles[1]);
            Assert.AreEqual("grp1", u.Groups[0]);
            Assert.AreEqual("grp2", u.Groups[1]);
            Assert.AreEqual("1",u.Custom["a"]);
            Assert.AreEqual("2", u.Custom["b"]);
        }

        [Test]
        public void CanWriteCustomJson()
        {
            var user = GetUser<User2>(0);
            user.MyField = "myval1";
            var js = UserSerializer.GetJson(user, "store").Simplify(SimplifyOptions.SingleQuotes);
            Console.WriteLine(js);
            Assert.AreEqual(@"{'_id':null,'_version':0,'createtime':'0001-01-01T00:00:00+0000','updatetime':'0001-01-01T00:00:00+0000','class':'user','_type':'user','netclass':'qorpent.v2.security.Tests.user.UserSerializationTest+User2, qorpent.v2.security.Tests','login':'login0','name':'myname0','email':'email0','admin':true,'active':false,'expire':'2015-12-31T19:00:00+0000','hash':'35d3986f45fbb6c6f9c22f741a4e7245','salt':'salt0','resetkey':null,'resetexpire':'0001-01-01T00:00:00+0000','publickey':'LS0tLS1CRUdJTiBDRVJUSUZJQ0FURS0tLS0tCk1JSUNXRENDQWNHZ0F3SUJBZ0lKQUpaN04rWnJ5YnUrTUEwR0NTcUdTSWIzRFFFQkJRVUFNRVV4Q3pBSkJnTlYKQkFZVEFrRlZNUk13RVFZRFZRUUlEQXBUYjIxbExWTjBZWFJsTVNFd0h3WURWUVFLREJoSmJuUmxjbTVsZENCWAphV1JuYVhSeklGQjBlU0JNZEdRd0hoY05NVFV3TlRJMk1UUTFPRE15V2hjTk1UWXdOVEkxTVRRMU9ETXlXakJGCk1Rc3dDUVlEVlFRR0V3SkJWVEVUTUJFR0ExVUVDQXdLVTI5dFpTMVRkR0YwWlRFaE1COEdBMVVFQ2d3WVNXNTAKWlhKdVpYUWdWMmxrWjJsMGN5QlFkSGtnVEhSa01JR2ZNQTBHQ1NxR1NJYjNEUUVCQVFVQUE0R05BRENCaVFLQgpnUURiNUQ1QncySjMyRURUK1BiVHB6SC9PYVYwcUlqc2VQV3hlWnVTS2tMbHhnVFQ3RWZZTHNnK3phYldmamw0CjhUdTgxZ3lCSVB5dUdkTzVwWkxkYXoxUU5pMFMxcmIvY3RONG9mQjhGb2FJbXcvSFF0R1NuZ21uMTBGOHlzRmwKOG8zL010TTBadXBjNkY0ZUlrU25kblA0cVlZOFNSZ2QvWG0zUHNuYiswbEtHUUlEQVFBQm8xQXdUakFkQmdOVgpIUTRFRmdRVWI2cnFXWXBhWnkraE11amYxNE9FS00ycXZIa3dId1lEVlIwakJCZ3dGb0FVYjZycVdZcGFaeStoCk11amYxNE9FS00ycXZIa3dEQVlEVlIwVEJBVXdBd0VCL3pBTkJna3Foa2lHOXcwQkFRVUZBQU9CZ1FCaVR4aTIKZmxEQXFONWE1dVRBNXFHUUc1SVpDNVNXRi9LektnUUhmMDgwbTM1MWVDVUxyd3Y2TG9RNzE4NDJyUmFtdmlXawp6QVVQb0NWWVNZQUwvb3JONVNrQWJiVGV6VVlFQTRTMDdxWXd1Vkt4L3llUTBOWG1CT2lIak1pU210MkVUcldPCmRpeTErQXg0bVZ2R2EwWlBLTWhFZjB4VGRzUEV5SmNFWlM3V3h3PT0KLS0tLS1FTkQgQ0VSVElGSUNBVEUtLS0tLQo=','logable':true,'mastergroup':'master0','roles':['role1_0','role2_0'],'groups':['grp1_0','grp2_0'],'custom':{'a':'1_0','b':'2_0'},'myfield':'myval1'}", js);

        }
        [Test]
        public void CanReadCustomJson()
        {
            var user =
                UserSerializer.CreateFromJson(
                    @"{'_id':null,'_version':0,'createtime':'0001-01-01T00:00:00+0000','updatetime':'0001-01-01T00:00:00+0000','class':'user','_type':'user','netclass':'qorpent.v2.security.Tests.user.UserSerializationTest+User2, qorpent.v2.security.Tests','login':'login0','name':'myname0','email':'email0','admin':true,'active':false,'expire':'2015-12-31T19:00:00+0000','hash':'35d3986f45fbb6c6f9c22f741a4e7245','salt':'salt0','resetkey':null,'resetexpire':'0001-01-01T00:00:00+0000','publickey':'LS0tLS1CRUdJTiBDRVJUSUZJQ0FURS0tLS0tCk1JSUNXRENDQWNHZ0F3SUJBZ0lKQUpaN04rWnJ5YnUrTUEwR0NTcUdTSWIzRFFFQkJRVUFNRVV4Q3pBSkJnTlYKQkFZVEFrRlZNUk13RVFZRFZRUUlEQXBUYjIxbExWTjBZWFJsTVNFd0h3WURWUVFLREJoSmJuUmxjbTVsZENCWAphV1JuYVhSeklGQjBlU0JNZEdRd0hoY05NVFV3TlRJMk1UUTFPRE15V2hjTk1UWXdOVEkxTVRRMU9ETXlXakJGCk1Rc3dDUVlEVlFRR0V3SkJWVEVUTUJFR0ExVUVDQXdLVTI5dFpTMVRkR0YwWlRFaE1COEdBMVVFQ2d3WVNXNTAKWlhKdVpYUWdWMmxrWjJsMGN5QlFkSGtnVEhSa01JR2ZNQTBHQ1NxR1NJYjNEUUVCQVFVQUE0R05BRENCaVFLQgpnUURiNUQ1QncySjMyRURUK1BiVHB6SC9PYVYwcUlqc2VQV3hlWnVTS2tMbHhnVFQ3RWZZTHNnK3phYldmamw0CjhUdTgxZ3lCSVB5dUdkTzVwWkxkYXoxUU5pMFMxcmIvY3RONG9mQjhGb2FJbXcvSFF0R1NuZ21uMTBGOHlzRmwKOG8zL010TTBadXBjNkY0ZUlrU25kblA0cVlZOFNSZ2QvWG0zUHNuYiswbEtHUUlEQVFBQm8xQXdUakFkQmdOVgpIUTRFRmdRVWI2cnFXWXBhWnkraE11amYxNE9FS00ycXZIa3dId1lEVlIwakJCZ3dGb0FVYjZycVdZcGFaeStoCk11amYxNE9FS00ycXZIa3dEQVlEVlIwVEJBVXdBd0VCL3pBTkJna3Foa2lHOXcwQkFRVUZBQU9CZ1FCaVR4aTIKZmxEQXFONWE1dVRBNXFHUUc1SVpDNVNXRi9LektnUUhmMDgwbTM1MWVDVUxyd3Y2TG9RNzE4NDJyUmFtdmlXawp6QVVQb0NWWVNZQUwvb3JONVNrQWJiVGV6VVlFQTRTMDdxWXd1Vkt4L3llUTBOWG1CT2lIak1pU210MkVUcldPCmRpeTErQXg0bVZ2R2EwWlBLTWhFZjB4VGRzUEV5SmNFWlM3V3h3PT0KLS0tLS1FTkQgQ0VSVElGSUNBVEUtLS0tLQo=','logable':true,'mastergroup':'master0','roles':['role1_0','role2_0'],'groups':['grp1_0','grp2_0'],'custom':{'a':'1_0','b':'2_0'},'myfield':'myval1'}"
                        .Replace("'", "\""));
            Assert.NotNull(user);
            var user1 = GetUser<User2>(0);
            user1.MyField = "myval1";
            Assert.AreEqual(user, user1);
            Assert.AreEqual(user.GetToken(), user1.GetToken());
        }

        public class User2 : User {
         
            public string MyField { get; set; }
            public override void WriteExtensions(JsonWriter writer, string mode, ISerializationAnnotator annotator) {
                writer.WriteProperty("myfield",MyField);
            }

            public override void ReadExtensions(object jsonsrc) {
                MyField = jsonsrc.str("myfield");
            }
        }
        [Test]
        [Explicit]
        public void PerformanceTest() {
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < 10000; i++)
            {
                var usr = GetUser(i % 100);
                var json = UserSerializer.GetJson(usr,"store");
                var usr1 = UserSerializer.CreateFromJson(json);
            }
            sw.Stop();
            Console.WriteLine("10000 s/ds cycles gain "+sw.Elapsed);
            
            Console.WriteLine((10000 /sw.Elapsed.TotalSeconds )  + " per second ");
        }

    }
}
