#region LICENSE

// Copyright 2007-2012 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Solution: Qorpent
// Original file : ConsoleArgumentHelperTest.cs
// Project: Qorpent.Utils.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;
using NUnit.Framework;

namespace Qorpent.Utils.Tests {
	[TestFixture]
	public class ConsoleArgumentHelperTest {
		[SetUp]
		public void Setup() {
			var args = new[] {"p1", "p2", "--x", "XVAL", "--y-p", "YVAL", "--b", "--c", "--u", "A", "B", "C"};
			_dict = new ConsoleArgumentHelper().ParseDictionary(args);
			_targs = new ConsoleArgumentHelper().Parse<TestArgs>(args);
		}


		private IDictionary<string, string> _dict;
		private TestArgs _targs;

		public class TestArgs {
			public string Arg1;
			public string Arg2;
			public string Arg8;
			public string Arg9;
			public bool B;
			public string C;
			public string U;
			public string X;
			public string Yp;
		}

		[Test]
		public void ClassApplyParseTest() {
			Assert.AreEqual("p1", _targs.Arg1);
			Assert.AreEqual("p2", _targs.Arg2);
			Assert.AreEqual("XVAL", _targs.X);
			Assert.AreEqual("YVAL", _targs.Yp);
			Assert.AreEqual(true, _targs.B);
			Assert.AreEqual("1", _targs.C);
			Assert.AreEqual("A", _targs.U);
			Assert.AreEqual("B", _targs.Arg8);
			Assert.AreEqual("C", _targs.Arg9);
		}

		[Test]
		public void DictionaryParseTest() {
			Assert.AreEqual("p1", _dict["arg1"]);
			Assert.AreEqual("p2", _dict["arg2"]);
			Assert.AreEqual("XVAL", _dict["x"]);
			Assert.AreEqual("YVAL", _dict["y-p"]);
			Assert.AreEqual("1", _dict["b"]);
			Assert.AreEqual("1", _dict["c"]);
			Assert.AreEqual("A", _dict["u"]);
			Assert.AreEqual("B", _dict["arg8"]);
			Assert.AreEqual("C", _dict["arg9"]);
		}

        [Test]
	    public void ShortCutParametersTest() {
            var dict = new ConsoleArgumentHelper().ParseDictionary(new[] {"-x", "a", "-y", "-z"});
            Assert.AreEqual("1",dict["~z"]);
            Assert.AreEqual("1",dict["~y"]);
            Assert.AreEqual("a",dict["~x"]);
        }

        [Test]
        public void Q305_Nagative_Numbers()
        {
            var dict = new ConsoleArgumentHelper().ParseDictionary(new[] { "-x", "-1" });
            Assert.AreEqual("-1", dict["~x"]);
        }
	}
}