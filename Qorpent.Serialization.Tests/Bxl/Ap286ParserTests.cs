using System;
using NUnit.Framework;
using Qorpent.Bxl;

namespace Qorpent.Serialization.Tests.Bxl {
    /// <summary>
    /// Проблемы, выявленные при парсе старых тем
    /// </summary>
    [TestFixture]
    public class Ap286ParserTests {
        /// <summary>
        /// Локализованная проблема 1 adding attribute to root not allowed at eco.abstract : 449:23
        /// </summary>
        [Test]
        public void can_parse_AP286_1_localized() {
            var code = @"thema kvartform, ""Квартальные формы ввода и отчетности отчеты"", abst:
	imports assoifin
	imports kvart
	autofillA="""" 
	useformmatrixA = true
	useformmatrixB = true
	useformmatrixC = true
	ra_lib2A = ""ecolibKFACTSVOD""
	rb_lib2A = ""ecolibKFACTORG""
	f_libA = ""ecolibKFACT""
	activecondition = false";
            new BxlParser().Parse(code);
        }

        /// <summary>
        /// Локализованная проблема 1 adding attribute to root not allowed at eco.abstract : 449:23
        /// </summary>
        [TestCase("a c=\"\"","c")]
        [TestCase("a c=\"\",","c")]
        [TestCase("a c=\"\" ","c")]
        [TestCase("a c=\"\", ","c")]
        [TestCase("a c=\"\" d=\"\"","c,d")]
        [TestCase("a c=\"\"   d=\"\"","c,d")]
        [TestCase("a c=\"\",d=\"\"","c,d")]
        [TestCase("a c=\"\", d=\"\"","c,d")]
		[TestCase("a c=''", "c")]
		[TestCase("a c='',", "c")]
		[TestCase("a c='' ", "c")]
		[TestCase("a c='', ", "c")]
		[TestCase("a c='' d=''", "c,d")]
		[TestCase("a c=''   d=''", "c,d")]
		[TestCase("a c='',d=''", "c,d")]
		[TestCase("a c='', d=''", "c,d")]
        public void QPT96_Bug(string srccode, string testattributes)
        {
			var x = new BxlParser().Parse(srccode).Element("a");
			Console.WriteLine(x);
	        foreach (var a in testattributes.Split(',')) {
		        Assert.NotNull(x.Attribute(a));
	        }
	        ;
        }
    }
}