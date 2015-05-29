using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Qorpent.Security.Tests
{
	[TestFixture]
	[Ignore("has non securable sys requirements")]
	public class LogonTest
	{
		[Test]
		public void TestValidLogon() {
			IntPtr token = IntPtr.Zero;
			var auth = new SysHostLogon().Logon("testusr", "xsw2@WSX",ref token);
			Assert.True(auth);
			Assert.AreNotEqual(token,IntPtr.Zero);
		}

		[Test]
		public void TestInValidLogon()
		{
			IntPtr token = IntPtr.Zero;
			var auth = new SysHostLogon().Logon("testusr", "xsw2@WSX...", ref token);
			Assert.False(auth);
			Assert.AreEqual(token, IntPtr.Zero);
		}
	}
}
