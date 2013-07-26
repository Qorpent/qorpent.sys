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
// Original file : JsonToXmlParserTest.cs
// Project: Qorpent.Dsl.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Xml.Linq;
using NUnit.Framework;

namespace Qorpent.Dsl.Tests {
	[TestFixture]
	[Ignore("использует устаревшую нотацию")]
	public class JsonToXmlParserTest {
		private void test(string json, string xml) {
			var res = new JsonToXmlParser().Parse(json);
			Console.WriteLine(res.ToString(SaveOptions.DisableFormatting));
			Assert.AreEqual(xml, res.ToString(SaveOptions.DisableFormatting));
		}

		private void error(string json) {
			var result = Assert.Throws<JsonToXmlParser.JsonParserException>(() => new JsonToXmlParser().Parse(json));
			Console.WriteLine(result.Message);
		}

		[Test]
		public void bug_literal_array() {
			test(@" [ true , false] ", "<array><value>true</value><value>false</value></array>");
		}

		[Test]
		public void complex_json() {
			test(@"{ ""s"" :1, ""a"" : [1,2] , ""z"" : {x : true, y: { a : 2, b : [""s"", ""y""] ,} }}",
			     @"<object s=""1""><a><value>1</value><value>2</value></a><z x=""true""><y a=""2""><b><value>s</value><value>y</value></b></y></z></object>");
		}

		[Test]
		public void digital_dot_minus_test() {
			test(@" -123.4 ", "<value>-123.4</value>");
		}

		[Test]
		public void digital_dot_test() {
			test(@" 123.4 ", "<value>123.4</value>");
		}

		[Test]
		public void digital_test() {
			test(@" 1234 ", "<value>1234</value>");
		}

		[Test]
		public void double_didgit_error() {
			error(@" 123.4.5 ");
		}

		[Test]
		public void double_minus_error() {
			error(@" --123.4 ");
		}


		[Test]
		public void empty_object() {
			test(@"{}", "<object />");
		}

		[Test]
		public void end_number_on_dot_error() {
			error(@" 1234. ");
		}

		[Test]
		public void error_on_not_closed() {
			error(@" ""te\""st ");
		}

		[Test]
		public void error_symbols_after_string() {
			error(@" ""te\""st"" xx ");
		}

		[Test]
		public void five_level_and_number_names() {
			test(@"{ 0 : {1 : {2 : {3 : {4 : 5} } } } }", @"<object><_0><_1><_2><_3 _4=""5"" /></_2></_1></_0></object>");
		}

		[Test]
		public void full_JSON_no_arrays() {
			test(@"{ ""x"" : { ""y"" : 1, ""z"": { ""a"" : ""b ""}, ""u"" : null } , ""a"" : true}",
			     @"<object a=""true""><x y=""1"" u=""null""><z a=""b "" /></x></object>");
		}


		[Test]
		public void illegal_column() {
			error(@"{ x 1 : }");
			error(@"{ x :: 1 }");
		}

		[Test]
		public void illegal_comma() {
			error(@"{ x 1 , }");
			error(@"{ x : 1 ,, }");
			error(@"{ , x : 1 }");
		}

		[Test]
		public void literal_test() {
			test("null_test_1", "<value>null_test_1</value>");
		}

		[Test]
		public void nested_objects() {
			test(@"{ x : {y : 1, z: {a : b}} , }", @"<object><x y=""1""><z a=""b"" /></x></object>");
		}

		[Test]
		public void null_ignore_trailings_test() {
			test(@"
   null
", "<value>null</value>");
		}

		[Test]
		public void null_no_interupt_test() {
			Assert.Throws<JsonToXmlParser.JsonParserException>(() => test("nu ll", "<value>null</value>"));
		}

		[Test]
		public void null_test() {
			test("null", "<value>null</value>");
		}

		[Test]
		public void object_with_digit_attribute() {
			test(@"{x:1}", @"<object x=""1"" />");
		}

		[Test]
		public void object_with_literal_attribute() {
			test(@"{y : true}", @"<object y=""true"" />");
		}

		[Test]
		public void object_with_string_attribute() {
			test(@"{z: ""str"" }", @"<object z=""str"" />");
		}

		[Test]
		public void object_with_three_simple_values() {
			test(@"{x:1, y:""str"", ""z"" : true}", @"<object x=""1"" y=""str"" z=""true"" />");
		}

		[Test]
		public void objects_in_array() {
			test(@" [1, {x :2} ] ", @"<array><value>1</value><object x=""2"" /></array>");
		}

		[Test]
		public void simple_array() {
			test(@" [1,2] ", "<array><value>1</value><value>2</value></array>");
		}

		[Test]
		public void simple_array_str_literal() {
			test(@" [1,2, ""3"" , null ] ", "<array><value>1</value><value>2</value><value>3</value><value>null</value></array>");
		}


		[Test]
		public void single_minus_error() {
			error(@" - ");
		}


		[Test]
		public void string_quot_test() {
			test(@" ""te\""st"" ", "<value>te\"st</value>");
		}

		[Test]
		public void string_test() {
			test(@" ""test1"" ", "<value>test1</value>");
		}

		[Test]
		public void trail_commas() {
			test(@"{ x : 1, }", @"<object x=""1"" />");
		}

		[Test]
		public void usualJSON() {
			test(@"{ ""x"":1, y:""str"", ""z"" : ""true""}", @"<object x=""1"" y=""str"" z=""true"" />");
		}
	}
}