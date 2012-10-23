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
// Original file : bxl_serializer_test.cs
// Project: Qorpent.Serialization.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Tests {
	[TestFixture]
	public class bxl_serializer_test {
		private void test(object obj, string expected) {
			var result = new BxlSerializer().DoSerialize("result", obj);
			Console.WriteLine(result);
			Assert.AreEqual(expected.Trim().LfOnly(), result.Trim().LfOnly());
		}


		[Test]
		public void large_strings() {
			test(new {x = @"very ""tall"" and
wide string", y = 2},
			     "result\r\n\tx=\"\"\"very \"tall\" and\r\nwide string\"\"\"\r\n\ty=2");
		}

		[Test]
		public void obj_to_bxl_test() {
			test(new {x = 1, y = 2, code = "code1", name = "name1", z = new {a = 2, b = 3}},
			     @"result code1, name1
	x=1
	y=2
	z
		a=2
		b=3");
		}
	}
}