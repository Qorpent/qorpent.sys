using System.IO;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;
using Qorpent.IO.DirtyVersion.Storage;

namespace Qorpent.IO.Tests.DirtyVersion
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class CopyOnReadStreamTest
	{
		[Test]
		public void CanRedirectOnHashing() {
			var hasher= MD5.Create();
			var frommem = new MemoryStream(Encoding.ASCII.GetBytes("Hello!"));
			var tomem = new MemoryStream();
			var proxy = new CopyOnReadStream(frommem, tomem);
			hasher.ComputeHash(proxy);
			tomem.Position = 0;
			var res = new StreamReader(tomem).ReadToEnd();
			Assert.AreEqual("Hello!",res);
		}
	}
}
