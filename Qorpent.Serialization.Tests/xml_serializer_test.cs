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
// Original file : xml_serializer_test.cs
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
	public class xml_serializer_test {
		private void test(object obj, string expected) {
			Console.WriteLine(new XmlSerializer().DoSerialize(null, obj).Trim());
			Assert.AreEqual(expected, new XmlSerializer().DoSerialize(null, obj).Trim());
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
			[SerializeNotNullOnly]public nschild child4;
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

		[Test]
		public void array_serialized() {
			var a = new object[] {1, true, "test!"};

			test(a, @"<Object__><item __idx=""0"">1</item><item __idx=""1"">true</item><item __idx=""2"">test!</item></Object__>");
		}

		[Test]
		public void array_serialized_anonym()
		{
			var a = new object[] { new{x=1},new{y=2} };

			test(a, @"<Object__><item x=""1"" __idx=""0"" /><item y=""2"" __idx=""1"" /></Object__>");
		}

		[Test]
		public void bool_serialized() {
			test(true, "<value>true</value>");
			test(false, "<value>false</value>");
		}

		[Test]
		public void bug_incorrect_element_placement() {
			var x = new parent();
			x.Y = 1;
			x.child2 = new child();
			test(x, @"<parent Y=""1"" X=""0""><child1 /><child2 id=""0"" /><child3 /></parent>");
		}

		[Test]
		public void bug_insuficient_nesting_in_xml_serialization() {
			test(new {z = new {a = 2, b = 3}},
			     @"<anonymous><z a=""2"" b=""3"" /></anonymous>");
		}

		[Test]
		public void complex_class_with_attributes_processed() {
			var x = new parent();
			test(x, @"<parent X=""0""><child1 /><child3 /></parent>");
			x.Y = 1;
			test(x, @"<parent Y=""1"" X=""0""><child1 /><child3 /></parent>");
			x.child2 = new child();
			test(x, @"<parent Y=""1"" X=""0""><child1 /><child2 id=""0"" /><child3 /></parent>");
			x.child2.id2 = 3;
			test(x, @"<parent Y=""1"" X=""0""><child1 /><child2 id=""0"" id2=""3"" /><child3 /></parent>");
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
			     @"<anonymous name=""x""><dict><item key=""x"">1</item><item key=""y'x"">true</item><item key=""z"">test!</item></dict><a><item __idx=""0"">1</item><item __idx=""1"">true</item><item __idx=""2"">test!</item></a></anonymous>");
		}

		[Test]
		public void datetime_serialized() {
			test(new DateTime(2010, 1, 12, 13, 15, 36), "<value>2010-01-12T13:15:36</value>");
		}

		[Test]
		public void dictionary_serialized() {
			var dict = new Dictionary<string, object>();
			dict["x"] = 1;
			dict["y'x"] = true;
			dict["z"] = "test!";
			test(dict, @"<root><item key=""x"">1</item><item key=""y'x"">true</item><item key=""z"">test!</item></root>");
		}

		[Test]
		public void double_float_decimal_serialized() {
			test(111111.1111d, "<value>111111.1111</value>");
			test(111111.1111m, "<value>111111.1111</value>");
			test(111111.5f, "<value>111111.5</value>"
				);
		}

		[Test]
		public void enum_serialized() {
			test(testEnum.X, "<value>X</value>");
			test(testEnum.Y, "<value>Y</value>");
		}

		[Test]
		public void int_and_long_serialized() {
			test((long) 1111111111, "<value>1111111111</value>");
			test(1111111111, "<value>1111111111</value>");
		}

		[Test]
		public void null_serialized() {
			test(null, "<value />"
				);
		}

		[Test]
		public void string_escaped_CRLFT_and_serialized() {
			test("\ttest'n\r\n", "<value>\ttest'n\r\n</value>"
				);
		}

		[Test]
		public void string_escaped_and_serialized() {
			test(@"\test'n", "<value>\\test'n</value>"
				);
		}

		[Test]
		public void string_serialized() {
			test("test", "<value>test</value>");
		}

		[Test]
		public void try_to_catch_bug_with_nulls() {
			test(new {x = (xml_serializer_test) null, y = (string) null}, "<anonymous y=\"\"><x /></anonymous>");
		}

		[Test]
		public void x_element_processed() {
			test(new XElement("root", new XAttribute("id", 1)), "<root id=\"1\" />");
		}
	}
}