﻿#region LICENSE

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
using System.Collections;
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
		[Serialize]
		public class DeadLion {
			public int Id {
				get { return 0; }
			}
		}
		[Serialize(IgnoreEnumerable = true)]
		public class Chelioz : IEnumerable<DeadLion> {
			public string Value {
				get { return "qorpent"; }
			}
			public IEnumerator<DeadLion> GetEnumerator() {
				return (new[] {new DeadLion()} as IEnumerable<DeadLion>).GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator() {
				return GetEnumerator();
			}
		}
		public class Dno {
			[Serialize(ItemName = "dno", NoIndex = true)]
			public DeadLion[]  DeadDnoCollection {
				get { return new[] {new DeadLion(), new DeadLion() }; }
			}
		}
		public class TestNamedCollection {
			[Serialize(ItemName = "num")]
			public int[] Values {
				get { return new[] {10, 2, 30}; }
			}
		}
		[Serialize]
		public class Chelios {
			public int Id { get; set; }
		}
		public class CheliosPrison {
			[Serialize(ItemName = "chelios")]
			public ICollection<Chelios> CheliosCollection {
				get { return new List<Chelios> {new Chelios {Id = 1}, new Chelios {Id = -1}};}
			}
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
		public void IgnoreEnumerableTest() {
			var c = new Chelioz();
			test(c, @"<root><Chelioz Value=""qorpent"" /></root>");
		}
		[Test]
		public void CanDoNotIndexDeadLions() {
			var t = new Dno();
			test(t, @"<root><Dno><DeadDnoCollection><dno Id=""0"" /><dno Id=""0"" /></DeadDnoCollection></Dno></root>");
		}
		[Test]
		public void IscorrectSerializeCollectionWithItemName() {
			var t = new CheliosPrison();
			test(t, @"<root><CheliosPrison><CheliosCollection><chelios Id=""1"" __idx=""0"" /><chelios Id=""-1"" __idx=""1"" /></CheliosCollection></CheliosPrison></root>");
		}
		[Test]
		public void IscorrectSerializeWithItemName() {
			var t = new TestNamedCollection();
			test(t, @"<root><TestNamedCollection><Values><num __idx=""0"">10</num><num __idx=""1"">2</num><num __idx=""2"">30</num></Values></TestNamedCollection></root>");
		}
		[Test]
		public void array_serialized() {
			var a = new object[] {1, true, "test!"};

			test(a, @"<root><Object__><item __idx=""0"">1</item><item __idx=""1"">true</item><item __idx=""2"">test!</item></Object__></root>");
		}

		[Test]
		public void array_serialized_anonym()
		{
			var a = new object[] { new{x=1},new{y=2} };

			test(a, @"<root><Object__><item x=""1"" __idx=""0"" /><item y=""2"" __idx=""1"" /></Object__></root>");
		}

		[Test]
		public void bool_serialized() {
			test(true, "<root><value>true</value></root>");
			test(false, "<root><value>false</value></root>");
		}

		[Test]
		public void bug_incorrect_element_placement() {
			var x = new parent();
			x.Y = 1;
			x.child2 = new child();
			test(x, @"<root><parent Y=""1"" X=""0""><child1 /><child2 id=""0"" /><child3 /></parent></root>");
		}

		[Test]
		public void bug_insuficient_nesting_in_xml_serialization() {
			test(new {z = new {a = 2, b = 3}},
			     @"<root><anonymous><z a=""2"" b=""3"" /></anonymous></root>");
		}

		[Test]
		public void complex_class_with_attributes_processed() {
			var x = new parent();
			test(x, @"<root><parent X=""0""><child1 /><child3 /></parent></root>");
			x.Y = 1;
			test(x, @"<root><parent Y=""1"" X=""0""><child1 /><child3 /></parent></root>");
			x.child2 = new child();
			test(x, @"<root><parent Y=""1"" X=""0""><child1 /><child2 id=""0"" /><child3 /></parent></root>");
			x.child2.id2 = 3;
			test(x, @"<root><parent Y=""1"" X=""0""><child1 /><child2 id=""0"" id2=""3"" /><child3 /></parent></root>");
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
			     @"<root><anonymous name=""x""><dict><item key=""x"">1</item><item key=""y'x"">true</item><item key=""z"">test!</item></dict><a><item __idx=""0"">1</item><item __idx=""1"">true</item><item __idx=""2"">test!</item></a></anonymous></root>");
		}

		[Test]
		public void datetime_serialized() {
			test(new DateTime(2010, 1, 12, 13, 15, 36), "<root><value>2010-01-12T13:15:36</value></root>");
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
			test(111111.1111d, "<root><value>111111.1111</value></root>");
			test(111111.1111m, "<root><value>111111.1111</value></root>");
			test(111111.5f, "<root><value>111111.5</value></root>"
				);
		}

		[Test]
		public void enum_serialized() {
			test(testEnum.X, "<root><value>X</value></root>");
			test(testEnum.Y, "<root><value>Y</value></root>");
		}

		[Test]
		public void int_and_long_serialized() {
			test((long) 1111111111, "<root><value>1111111111</value></root>");
			test(1111111111, "<root><value>1111111111</value></root>");
		}

		[Test]
		public void null_serialized() {
			test(null, "<root><value /></root>"
				);
		}

		[Test]
		public void string_escaped_CRLFT_and_serialized() {
			test("\ttest'n\r\n", "<root><value>\ttest'n\r\n</value></root>"
				);
		}

		[Test]
		public void string_escaped_and_serialized() {
			test(@"\test'n", "<root><value>\\test'n</value></root>"
				);
		}

		[Test]
		public void string_serialized() {
			test("test", "<root><value>test</value></root>");
		}

		[Test]
		public void try_to_catch_bug_with_nulls() {
			test(new {x = (xml_serializer_test) null, y = (string) null}, "<root><anonymous y=\"\"><x /></anonymous></root>");
		}

		[Test]
		public void x_element_processed() {
			test(new XElement("root", new XAttribute("id", 1)), "<root id=\"1\" />");
		}
	}
}