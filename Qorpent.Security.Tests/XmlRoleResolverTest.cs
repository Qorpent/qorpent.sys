using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Xml.Linq;
using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Security.Tests
{
	[TestFixture]
	public class XmlRoleResolverTest {
		private string bxl = @"
role A 'B,C'
role B 'D'
user X A
user Y 'B,C'
		";

		[TestCase("X", "A", true)]
		[TestCase("X","B",true)]
		[TestCase("X", "C", true)]
		[TestCase("X", "D", true)]
		[TestCase("Y", "A", false)]
		[TestCase("Y", "B", true)]
		[TestCase("Y", "C", true)]
		[TestCase("Y", "D", true)]
		public void TestOfUserInRole(string user,string role, bool result) {
			var rr = new FileBasedRoleProvider();
			rr.DirectXml = XmlExtensions.GetXmlFromAny(bxl);
			Assert.AreEqual(result,rr.IsInRole(new GenericPrincipal(new GenericIdentity(user),null ), role));
		}
	}
}
