using System;
using NUnit.Framework;
using Qorpent.IO.Net;

namespace Qorpent.IO.Tests.Net {
	[TestFixture]
	public class HttpResponseTests {
		private const string Rfc3986BaseUri = "http://a/b/c/d;p?q";
		[TestCase("g:h", "g:h", Ignore = true)]
		[TestCase("g", "http://a/b/c/g")]
		[TestCase("./g", "http://a/b/c/g")]
		[TestCase("g/", "http://a/b/c/g/")]
		[TestCase("/g", "http://a/g")]
		[TestCase("//g", "http://g/")]
		[TestCase("?y", "http://a/b/c/d;p?y")]
		[TestCase("g?y", "http://a/b/c/g?y")]
		[TestCase("#s", "http://a/b/c/d;p?q#s")]
		[TestCase("g#s", "http://a/b/c/g#s")]
		[TestCase("g?y#s", "http://a/b/c/g?y#s")]
		[TestCase(";x", "http://a/b/c/;x")]
		[TestCase("g;x", "http://a/b/c/g;x")]
		[TestCase("g;x?y#s", "http://a/b/c/g;x?y#s")]
		[TestCase("", "http://a/b/c/d;p?q")]
		[TestCase(".", "http://a/b/c/")]
		[TestCase("./", "http://a/b/c/")]
		[TestCase("..", "http://a/b/")]
		[TestCase("../", "http://a/b/")]
		[TestCase("../g", "http://a/b/g")]
		[TestCase("../..", "http://a/")]
		[TestCase("../../", "http://a/")]
		[TestCase("../../g", "http://a/g")]
		public void IsRfc3986Supports(string l, string e) {
			var r = new HttpResponse {Uri = new Uri(Rfc3986BaseUri)};
			r.Headers["Location"] = l;
			Assert.AreEqual(e, r.RedirectUri.ToString());
		}
	}
}
