using System;
using System.Net;
using NUnit.Framework;

namespace Qorpent.Native.Tests
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class HttpApiTest
	{
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Explicit]
		public void TestQorpent8092SamplePort(){
			var ssl = HttpApi.HttpApi.OpenSession().QuerySslEndpoint(new IPEndPoint(0,80));
			Console.WriteLine(ssl);
		}
	}
}
