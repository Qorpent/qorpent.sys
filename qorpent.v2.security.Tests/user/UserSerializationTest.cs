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
            Assert.AreEqual(@"{'_id':null,'_version':0,'createtime':'0001-01-01T00:00:00+0000','updatetime':'0001-01-01T00:00:00+0000','class':'user','_type':'user','netclass':'qorpent.v2.security.user.User, qorpent.v2.security','login':'login0','name':'myname0','email':'email0','admin':true,'isgroup':false,'active':false,'expire':'2015-12-31T19:00:00+0000','publickey':'LS0tLS1CRUdJTiBDRVJUSUZJQ0FURS0tLS0tDQpNSUlDV0RDQ0FjR2dBd0lCQWdJSkFKWjdOK1pyeWJ1K01BMEdDU3FHU0liM0RRRUJCUVVBTUVVeEN6QUpCZ05WDQpCQVlUQWtGVk1STXdFUVlEVlFRSURBcFRiMjFsTFZOMFlYUmxNU0V3SHdZRFZRUUtEQmhKYm5SbGNtNWxkQ0JYDQphV1JuYVhSeklGQjBlU0JNZEdRd0hoY05NVFV3TlRJMk1UUTFPRE15V2hjTk1UWXdOVEkxTVRRMU9ETXlXakJGDQpNUXN3Q1FZRFZRUUdFd0pCVlRFVE1CRUdBMVVFQ0F3S1UyOXRaUzFUZEdGMFpURWhNQjhHQTFVRUNnd1lTVzUwDQpaWEp1WlhRZ1YybGtaMmwwY3lCUWRIa2dUSFJrTUlHZk1BMEdDU3FHU0liM0RRRUJBUVVBQTRHTkFEQ0JpUUtCDQpnUURiNUQ1QncySjMyRURUK1BiVHB6SC9PYVYwcUlqc2VQV3hlWnVTS2tMbHhnVFQ3RWZZTHNnK3phYldmamw0DQo4VHU4MWd5QklQeXVHZE81cFpMZGF6MVFOaTBTMXJiL2N0TjRvZkI4Rm9hSW13L0hRdEdTbmdtbjEwRjh5c0ZsDQo4bzMvTXRNMFp1cGM2RjRlSWtTbmRuUDRxWVk4U1JnZC9YbTNQc25iKzBsS0dRSURBUUFCbzFBd1RqQWRCZ05WDQpIUTRFRmdRVWI2cnFXWXBhWnkraE11amYxNE9FS00ycXZIa3dId1lEVlIwakJCZ3dGb0FVYjZycVdZcGFaeStoDQpNdWpmMTRPRUtNMnF2SGt3REFZRFZSMFRCQVV3QXdFQi96QU5CZ2txaGtpRzl3MEJBUVVGQUFPQmdRQmlUeGkyDQpmbERBcU41YTV1VEE1cUdRRzVJWkM1U1dGL0t6S2dRSGYwODBtMzUxZUNVTHJ3djZMb1E3MTg0MnJSYW12aVdrDQp6QVVQb0NWWVNZQUwvb3JONVNrQWJiVGV6VVlFQTRTMDdxWXd1Vkt4L3llUTBOWG1CT2lIak1pU210MkVUcldPDQpkaXkxK0F4NG1WdkdhMFpQS01oRWYweFRkc1BFeUpjRVpTN1d4dz09DQotLS0tLUVORCBDRVJUSUZJQ0FURS0tLS0tDQo=','hash':'16e001a08cece49745cf04470aabd1d1','salt':'salt1','resetkey':null,'resetexpire':'0001-01-01T00:00:00+0000','logable':false,'domain':'master0','roles':['role1_0','role2_0'],'groups':['grp1_0','grp2_0'],'custom':{'a':'1_0','b':'2_0'}}", js);
        }

        [Test]
        public void CanReadJson() {
            var user =
                UserSerializer.CreateFromJson(
                    @"{'_id':null,'_version':0,'createtime':'0001-01-01T00:00:00+0000','updatetime':'0001-01-01T00:00:00+0000','class':'user','_type':'user','netclass':'qorpent.v2.security.user.User, qorpent.v2.security','login':'login0','name':'myname0','email':'email0','admin':true,'isgroup':false,'active':false,'expire':'2015-12-31T19:00:00+0000','hash':'16e001a08cece49745cf04470aabd1d1','salt':'salt1','resetkey':null,'resetexpire':'0001-01-01T00:00:00+0000','publickey':'LS0tLS1CRUdJTiBDRVJUSUZJQ0FURS0tLS0tDQpNSUlDV0RDQ0FjR2dBd0lCQWdJSkFKWjdOK1pyeWJ1K01BMEdDU3FHU0liM0RRRUJCUVVBTUVVeEN6QUpCZ05WDQpCQVlUQWtGVk1STXdFUVlEVlFRSURBcFRiMjFsTFZOMFlYUmxNU0V3SHdZRFZRUUtEQmhKYm5SbGNtNWxkQ0JYDQphV1JuYVhSeklGQjBlU0JNZEdRd0hoY05NVFV3TlRJMk1UUTFPRE15V2hjTk1UWXdOVEkxTVRRMU9ETXlXakJGDQpNUXN3Q1FZRFZRUUdFd0pCVlRFVE1CRUdBMVVFQ0F3S1UyOXRaUzFUZEdGMFpURWhNQjhHQTFVRUNnd1lTVzUwDQpaWEp1WlhRZ1YybGtaMmwwY3lCUWRIa2dUSFJrTUlHZk1BMEdDU3FHU0liM0RRRUJBUVVBQTRHTkFEQ0JpUUtCDQpnUURiNUQ1QncySjMyRURUK1BiVHB6SC9PYVYwcUlqc2VQV3hlWnVTS2tMbHhnVFQ3RWZZTHNnK3phYldmamw0DQo4VHU4MWd5QklQeXVHZE81cFpMZGF6MVFOaTBTMXJiL2N0TjRvZkI4Rm9hSW13L0hRdEdTbmdtbjEwRjh5c0ZsDQo4bzMvTXRNMFp1cGM2RjRlSWtTbmRuUDRxWVk4U1JnZC9YbTNQc25iKzBsS0dRSURBUUFCbzFBd1RqQWRCZ05WDQpIUTRFRmdRVWI2cnFXWXBhWnkraE11amYxNE9FS00ycXZIa3dId1lEVlIwakJCZ3dGb0FVYjZycVdZcGFaeStoDQpNdWpmMTRPRUtNMnF2SGt3REFZRFZSMFRCQVV3QXdFQi96QU5CZ2txaGtpRzl3MEJBUVVGQUFPQmdRQmlUeGkyDQpmbERBcU41YTV1VEE1cUdRRzVJWkM1U1dGL0t6S2dRSGYwODBtMzUxZUNVTHJ3djZMb1E3MTg0MnJSYW12aVdrDQp6QVVQb0NWWVNZQUwvb3JONVNrQWJiVGV6VVlFQTRTMDdxWXd1Vkt4L3llUTBOWG1CT2lIak1pU210MkVUcldPDQpkaXkxK0F4NG1WdkdhMFpQS01oRWYweFRkc1BFeUpjRVpTN1d4dz09DQotLS0tLUVORCBDRVJUSUZJQ0FURS0tLS0tDQo=','logable':false,'domain':'master0','roles':['role1_0','role2_0'],'groups':['grp1_0','grp2_0'],'custom':{'a':'1_0','b':'2_0'}}"
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
    domain = master1
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
            Assert.AreEqual("master1", u.Domain);
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
            Assert.AreEqual(@"{'_id':null,'_version':0,'createtime':'0001-01-01T00:00:00+0000','updatetime':'0001-01-01T00:00:00+0000','class':'user','_type':'user','netclass':'qorpent.v2.security.Tests.user.UserSerializationTest+User2, qorpent.v2.security.Tests','login':'login0','name':'myname0','email':'email0','admin':true,'isgroup':false,'active':false,'expire':'2015-12-31T19:00:00+0000','publickey':'LS0tLS1CRUdJTiBDRVJUSUZJQ0FURS0tLS0tDQpNSUlDV0RDQ0FjR2dBd0lCQWdJSkFKWjdOK1pyeWJ1K01BMEdDU3FHU0liM0RRRUJCUVVBTUVVeEN6QUpCZ05WDQpCQVlUQWtGVk1STXdFUVlEVlFRSURBcFRiMjFsTFZOMFlYUmxNU0V3SHdZRFZRUUtEQmhKYm5SbGNtNWxkQ0JYDQphV1JuYVhSeklGQjBlU0JNZEdRd0hoY05NVFV3TlRJMk1UUTFPRE15V2hjTk1UWXdOVEkxTVRRMU9ETXlXakJGDQpNUXN3Q1FZRFZRUUdFd0pCVlRFVE1CRUdBMVVFQ0F3S1UyOXRaUzFUZEdGMFpURWhNQjhHQTFVRUNnd1lTVzUwDQpaWEp1WlhRZ1YybGtaMmwwY3lCUWRIa2dUSFJrTUlHZk1BMEdDU3FHU0liM0RRRUJBUVVBQTRHTkFEQ0JpUUtCDQpnUURiNUQ1QncySjMyRURUK1BiVHB6SC9PYVYwcUlqc2VQV3hlWnVTS2tMbHhnVFQ3RWZZTHNnK3phYldmamw0DQo4VHU4MWd5QklQeXVHZE81cFpMZGF6MVFOaTBTMXJiL2N0TjRvZkI4Rm9hSW13L0hRdEdTbmdtbjEwRjh5c0ZsDQo4bzMvTXRNMFp1cGM2RjRlSWtTbmRuUDRxWVk4U1JnZC9YbTNQc25iKzBsS0dRSURBUUFCbzFBd1RqQWRCZ05WDQpIUTRFRmdRVWI2cnFXWXBhWnkraE11amYxNE9FS00ycXZIa3dId1lEVlIwakJCZ3dGb0FVYjZycVdZcGFaeStoDQpNdWpmMTRPRUtNMnF2SGt3REFZRFZSMFRCQVV3QXdFQi96QU5CZ2txaGtpRzl3MEJBUVVGQUFPQmdRQmlUeGkyDQpmbERBcU41YTV1VEE1cUdRRzVJWkM1U1dGL0t6S2dRSGYwODBtMzUxZUNVTHJ3djZMb1E3MTg0MnJSYW12aVdrDQp6QVVQb0NWWVNZQUwvb3JONVNrQWJiVGV6VVlFQTRTMDdxWXd1Vkt4L3llUTBOWG1CT2lIak1pU210MkVUcldPDQpkaXkxK0F4NG1WdkdhMFpQS01oRWYweFRkc1BFeUpjRVpTN1d4dz09DQotLS0tLUVORCBDRVJUSUZJQ0FURS0tLS0tDQo=','hash':'16e001a08cece49745cf04470aabd1d1','salt':'salt1','resetkey':null,'resetexpire':'0001-01-01T00:00:00+0000','logable':false,'domain':'master0','roles':['role1_0','role2_0'],'groups':['grp1_0','grp2_0'],'custom':{'a':'1_0','b':'2_0'},'myfield':'myval1'}", js);

        }
        [Test]
        public void CanReadCustomJson()
        {
            var user =
                UserSerializer.CreateFromJson(
                    @"{'_id':null,'_version':0,'createtime':'0001-01-01T00:00:00+0000','updatetime':'0001-01-01T00:00:00+0000','class':'user','_type':'user','netclass':'qorpent.v2.security.Tests.user.UserSerializationTest+User2, qorpent.v2.security.Tests','login':'login0','name':'myname0','email':'email0','admin':true,'isgroup':false,'active':false,'expire':'2015-12-31T19:00:00+0000','hash':'16e001a08cece49745cf04470aabd1d1','salt':'salt1','resetkey':null,'resetexpire':'0001-01-01T00:00:00+0000','publickey':'LS0tLS1CRUdJTiBDRVJUSUZJQ0FURS0tLS0tDQpNSUlDV0RDQ0FjR2dBd0lCQWdJSkFKWjdOK1pyeWJ1K01BMEdDU3FHU0liM0RRRUJCUVVBTUVVeEN6QUpCZ05WDQpCQVlUQWtGVk1STXdFUVlEVlFRSURBcFRiMjFsTFZOMFlYUmxNU0V3SHdZRFZRUUtEQmhKYm5SbGNtNWxkQ0JYDQphV1JuYVhSeklGQjBlU0JNZEdRd0hoY05NVFV3TlRJMk1UUTFPRE15V2hjTk1UWXdOVEkxTVRRMU9ETXlXakJGDQpNUXN3Q1FZRFZRUUdFd0pCVlRFVE1CRUdBMVVFQ0F3S1UyOXRaUzFUZEdGMFpURWhNQjhHQTFVRUNnd1lTVzUwDQpaWEp1WlhRZ1YybGtaMmwwY3lCUWRIa2dUSFJrTUlHZk1BMEdDU3FHU0liM0RRRUJBUVVBQTRHTkFEQ0JpUUtCDQpnUURiNUQ1QncySjMyRURUK1BiVHB6SC9PYVYwcUlqc2VQV3hlWnVTS2tMbHhnVFQ3RWZZTHNnK3phYldmamw0DQo4VHU4MWd5QklQeXVHZE81cFpMZGF6MVFOaTBTMXJiL2N0TjRvZkI4Rm9hSW13L0hRdEdTbmdtbjEwRjh5c0ZsDQo4bzMvTXRNMFp1cGM2RjRlSWtTbmRuUDRxWVk4U1JnZC9YbTNQc25iKzBsS0dRSURBUUFCbzFBd1RqQWRCZ05WDQpIUTRFRmdRVWI2cnFXWXBhWnkraE11amYxNE9FS00ycXZIa3dId1lEVlIwakJCZ3dGb0FVYjZycVdZcGFaeStoDQpNdWpmMTRPRUtNMnF2SGt3REFZRFZSMFRCQVV3QXdFQi96QU5CZ2txaGtpRzl3MEJBUVVGQUFPQmdRQmlUeGkyDQpmbERBcU41YTV1VEE1cUdRRzVJWkM1U1dGL0t6S2dRSGYwODBtMzUxZUNVTHJ3djZMb1E3MTg0MnJSYW12aVdrDQp6QVVQb0NWWVNZQUwvb3JONVNrQWJiVGV6VVlFQTRTMDdxWXd1Vkt4L3llUTBOWG1CT2lIak1pU210MkVUcldPDQpkaXkxK0F4NG1WdkdhMFpQS01oRWYweFRkc1BFeUpjRVpTN1d4dz09DQotLS0tLUVORCBDRVJUSUZJQ0FURS0tLS0tDQo=','logable':false,'domain':'master0','roles':['role1_0','role2_0'],'groups':['grp1_0','grp2_0'],'custom':{'a':'1_0','b':'2_0'},'myfield':'myval1'}"
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
            CheckRate(i => {
                var usr = GetUser(i % 100);
                var json = UserSerializer.GetJson(usr, "store");
                var usr1 = UserSerializer.CreateFromJson(json);
            });
        }

    }
}
