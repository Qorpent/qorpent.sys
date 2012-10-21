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
// Original file : json_serializer_test.cs
// Project: Qorpent.Serialization.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using NUnit.Framework;

namespace Qorpent.Serialization.Tests {
	[TestFixture]
	public class json_serializer_test {
		private void test(object obj, string expected) {
			Assert.AreEqual(expected, new JsonSerializer().DoSerialize(null, obj));
		}


		public enum testEnum {
			X,
			Y,
		}

		public class parent {
			public int X { get; set; }
			[SerializeNotNullOnly] public int Y;
			[IgnoreSerialize] public int Z;
			public child child1;
			[SerializeNotNullOnly] public child child2;
			[Serialize] public nschild child3;
			[SerializeNotNullOnly]
			public nschild child4;
		}

		[Serialize]
		public class child {
			public int id;
			[SerializeNotNullOnly] public int id2;
		}

		public class nschild {
			public int id;
			[SerializeNotNullOnly] public int id2;
		}

		[Serialize(CamelNames = true)]
		public class lowercaseclass {
			public string SoStrangeNameWithBigCamelWords { get; set; }
		}


		[Test]
		public void array_serialized() {
			var a = new object[] {1, true, "test!"};

			test(a, @"{""0"": 1, ""1"": true, ""2"": ""test!""}");
		}

		[Test]
		public void array_serialized_string_with_quots() {
			var a = new object[] {1, true, "\"test\"!"};

			test(a, @"{""0"": 1, ""1"": true, ""2"": ""\""test\""!""}");
		}

		[Test]
		public void bool_serialized() {
			test(true, "true");
			test(false, "false");
		}

		[Test]
		public void camel_case_used() {
			test(new lowercaseclass {SoStrangeNameWithBigCamelWords = "x"}, @"{""soStrangeNameWithBigCamelWords"": ""x""}");
		}


		[Test]
		public void can_process_xelement() {
			test(
				new XElement("root", new XAttribute("id", 1), new XElement("child", new XText("data")),
				             new XElement("child2", new XCData("data2"))),
				@"{""id"": ""1"", ""child"": {""_text"": ""data""}, ""child2"": {""_text"": ""data2""}}");
		}

		[Test]
		public void complex_class_with_attributes_processed() {
			var x = new parent();
			test(x, @"{""child1"": null, ""child3"": null, ""X"": 0}");
			x.Y = 1;
			test(x, @"{""Y"": 1, ""child1"": null, ""child3"": null, ""X"": 0}");
			x.child2 = new child();
			test(x, @"{""Y"": 1, ""child1"": null, ""child2"": {""id"": 0}, ""child3"": null, ""X"": 0}");
			x.child2.id2 = 3;
			test(x, @"{""Y"": 1, ""child1"": null, ""child2"": {""id"": 0, ""id2"": 3}, ""child3"": null, ""X"": 0}");
		}

		[Test]
		public void complex_class_with_dictionary_and_array_serialized() {
			var dict = new Dictionary<string, object>();
			dict["x"] = 1;
			dict["y'x"] = true;
			dict["z"] = "test!";
			var a = new object[] {1, true, "test!"};
			var obj = new {name = "x", dict, a};
			test(obj,
			     @"{""name"": ""x"", ""dict"": {""x"": 1, ""y'x"": true, ""z"": ""test!""}, ""a"": {""0"": 1, ""1"": true, ""2"": ""test!""}}");
		}

		[Test]
		public void datetime_serialized() {
			test(new DateTime(2010, 1, 12, 13, 15, 36), "\"##new Date(2010,0,12,13,15,36)\"");
		}

		[Test]
		public void dictionary_serialized() {
			var dict = new Dictionary<string, object>();
			dict["x"] = 1;
			dict["y'x"] = true;
			dict["z"] = "test!";
			test(dict, @"{""x"": 1, ""y'x"": true, ""z"": ""test!""}");
		}

		[Test]
		public void double_float_decimal_serialized() {
			test(111111.1111d, "111111.1111");
			test(111111.1111m, "111111.1111");
			test(111111.5f, "111111.5");
		}

		[Test]
		public void enum_serialized() {
			test(testEnum.X, "\"X\"");
			test(testEnum.Y, "\"Y\"");
		}

		[Test]
		public void int_and_long_serialized() {
			test((long) 1111111111, "1111111111");
			test(1111111111, "1111111111");
		}

		[Test]
		public void nested_objects_in_anonymous_classes_are_serialized() {
			test(new {x = 1, y = new {z = 0}}, @"{""x"": 1, ""y"": {""z"": 0}}");
		}

		[Test]
		public void null_serialized() {
			test(null, "null");
		}

		[Test]
		public void string_escaped_CRLFT_and_serialized() {
			test("\ttest'n\r\n", @"""\ttest'n\r\n""");
		}

		[Test]
		public void string_escaped_and_serialized() {
			test(@"\test'n", @"""\\test'n""");
		}

		[Test]
		public void string_serialized() {
			test("test", @"""test""");
		}
	}
}