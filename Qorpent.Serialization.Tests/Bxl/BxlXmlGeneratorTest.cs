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
// Original file : BxlXmlGeneratorTest.cs
// Project: Qorpent.Bxl.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Bxl.Tests {
	[TestFixture]
	public class BxlXmlGeneratorTest {
		[Test]
		public void CanRecognizeNonStandardAttributes() {
			var tokens = new BxlTokenizer().Tokenize(@"
test +a = 1
	-b = 2
	1x = 3
");
			var xml = new BxlXmlGenerator().Generate(tokens);
			Console.WriteLine(xml.ToString());
			Assert.AreEqual(@"<root><test _line=""2"" __PLUS__a=""1"" __MINUS__b=""2"" _1x=""3"" /></root>".LfOnly(),
			                xml.ToString(SaveOptions.DisableFormatting).LfOnly());
		}

		[Test]
		public void InvalidLevelShiftError() {
			var exc = Assert.Throws<BxlException>(() => new BxlParser().Parse(@"
wrong
		level", ""));
			StringAssert.Contains("levelshift", exc.Message);
		}

		[Test]
		public void NoLexDataAnSafeAttributes() {
			var xml = new BxlParser().Parse(@"test code name", options: MyBxl.GetOptions(true, true));
			Console.WriteLine(xml.ToString());
			Assert.AreEqual(@"<root><test __code=""code"" __id=""code"" __name=""name"" /></root>",
			                xml.ToString(SaveOptions.DisableFormatting));
		}

		[Test]
		public void SupportsNonStandardLiterals() {
			var tokens = new BxlTokenizer().Tokenize(@"
test a = 0A3@$%&^?*()[]02
");
			var xml = new BxlXmlGenerator().Generate(tokens);
			Console.WriteLine(xml.ToString());
			Assert.AreEqual(@"<root><test _line=""2"" a=""0A3@$%&amp;^?*()[]02"" /></root>".LfOnly(),
			                xml.ToString(SaveOptions.DisableFormatting).LfOnly());
		}

		[Test]
		public void bug_in_code() {
			new BxlTokenizer().Tokenize(
				@"
reportset Ab :
	ask und_plan : false  # по умолчанию  
	ask und_soc 
	ask und_soc_dir 
	");
		}


		[Test]
		public void special_prefixed_literals() {
			var tokens = new BxlTokenizer().Tokenize(
				@"
a x = ?x
	b +x
		c ?x
    y=/x
    "
				);
			var xml = new BxlXmlGenerator().Generate(tokens, "test");
			Console.WriteLine(xml.ToString());
			Assert.AreEqual(@"<root>
  <a _file=""test"" _line=""2"" x=""?x"" y=""/x"">
    <b _file=""test"" _line=""3"" code=""+x"" id=""+x"">
      <c _file=""test"" _line=""4"" code=""?x"" id=""?x"" />
    </b>
  </a>
</root>".LfOnly(), xml.ToString().LfOnly());
		}

		[Test]
		public void test_with_no_lex_data() {
			var tokens = new BxlTokenizer().Tokenize(
				@"a x"
				);
			var xml = new BxlXmlGenerator().Generate(tokens, "test", false);
			Console.WriteLine(xml.ToString());
			Assert.AreEqual(@"<root><a code=""x"" id=""x"" /></root>".LfOnly(), xml.ToString(SaveOptions.DisableFormatting).LfOnly());
		}

		[Test]
		public void test_with_safe_anonymous_attr() {
			var tokens = new BxlTokenizer().Tokenize(
				@"a x"
				);
			var xml = new BxlXmlGenerator().Generate(tokens, "test", false, true);
			Console.WriteLine(xml.ToString());
			Assert.AreEqual(@"<root><a __code=""x"" __id=""x"" /></root>".LfOnly(), xml.ToString(SaveOptions.DisableFormatting).LfOnly());
		}

		[Test]
		public void test_with_xml() {
			var tokens = new BxlTokenizer().Tokenize(
				@"
#comment
e1 code1, 'name 1', x.z = 1 : val #comment
	x = 34, z='x' #multiple attributes in any line now allowed
		y = 46 # all attributes are always treated to closer element
	e11 code2 
		y = 35
	e12 code3
	"
				);
			var xml = new BxlXmlGenerator().Generate(tokens);
			Console.WriteLine(xml.ToString());
			Assert.AreEqual(@"<root>
  <e1 _line=""3"" code=""code1"" id=""code1"" name=""name 1"" x.z=""1"" x=""34"" z=""x"" y=""46"">val<e11 _line=""6"" code=""code2"" id=""code2"" y=""35"" /><e12 _line=""8"" code=""code3"" id=""code3"" /></e1>
</root>".LfOnly(), xml.ToString().LfOnly());
		}


		[Test]
		public void valid_syntax_info_bug() {
			var tokens = new BxlTokenizer().Tokenize(
				@"####################################################################################
###################                  Реальные темы                  #################
#####################################################################################

global _ПОДПИСЬ_В_ОТЧЕТЕ_ПРЕДПРИЯТИЯ : ""false""


colset cs_controlpointall, ""TEST1"" :
	col Б1, ""Б1 {2} {0}"", fixed, forpeiods=""_ЯНВ,_НЕ_ЯНВ,_МЕС_СНГ,_ОЖИД_СНГ""
	col Б2, ""Б2 {2} {0}"", fixed, forpeiods=""_ЯНВ,_НЕ_ЯНВ,_МЕС_СНГ,_ОЖИД_СНГ""
	col On, ""On {2} {0}"", fixed, forpeiods=""_ЯНВ,_НЕ_ЯНВ,_МЕС_СНГ,_ОЖИД_СНГ""
	col Pd, ""Pd {2} {0}"", fixed, forpeiods=""_ЯНВ,_НЕ_ЯНВ,_МЕС_СНГ,_ОЖИД_СНГ""
	col Rd, ""Rd {2} {0}"", TEST2, forpeiods=""_ЯНВ,_НЕ_ЯНВ,_МЕС_СНГ,_ОЖИД_СНГ""
	col Ok, ""Ok {2} {0}"", fixed, forpeiods=""_ЯНВ,_НЕ_ЯНВ,_МЕС_СНГ,_ОЖИД_СНГ""
	col DZn, ""DZn {2} {0}"", fixed, forpeiods=""_ЯНВ,_НЕ_ЯНВ,_МЕС_СНГ,_ОЖИД_СНГ""
	col KZn, ""KZn {2} {0}"", fixed, forpeiods=""_ЯНВ,_НЕ_ЯНВ,_МЕС_СНГ,_ОЖИД_СНГ""
	col DZk, ""DZk {2} {0}"", fixed, forpeiods=""_ЯНВ,_НЕ_ЯНВ,_МЕС_СНГ,_ОЖИД_СНГ""
	col KZk, ""KZk {2} {0}"", fixed, forpeiods=""_ЯНВ,_НЕ_ЯНВ,_МЕС_СНГ,_ОЖИД_СНГ""
	col PLAN, ""PLAN {2} {0}"", fixed, forperiods=""251,251,253,254""

colset TEST3, ""Колонки для всех контрольных точек"" :
	col CONTROLONDEB, ""На начало (дебет) {2} {0}"", forpeiods=""_ЯНВ,_НЕ_ЯНВ,_МЕС_СНГ,_ОЖИД_СНГ"", fixed : 
		checkrule action=""|<>|"", value=""0"", cellstyle="";color :red; background : yellow;""
	col CONTROLONKRED, ""На начало {2} {0}"", forpeiods=""_ЯНВ,_НЕ_ЯНВ,_МЕС_СНГ,_ОЖИД_СНГ"", fixed : 
		checkrule action=""|<>|"", value=""0"", cellstyle="";color :red; background : yellow;""
"
				);
			var t1 = tokens.First(x => x.Value == "TEST1");
			var t2 = tokens.First(x => x.Value == "TEST2");
			var t3 = tokens.First(x => x.Value == "TEST3");
			Assert.AreEqual(8, t1.LexInfo.Line);
			Assert.AreEqual(13, t2.LexInfo.Line);
			Assert.AreEqual(21, t3.LexInfo.Line);
		}
	}
}