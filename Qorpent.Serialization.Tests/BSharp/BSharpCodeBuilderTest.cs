﻿using System;
using NUnit.Framework;
using Qorpent.BSharp;
using Qorpent.Bxl;

namespace Qorpent.Serialization.Tests.BSharp {
    [TestFixture]
    public class BSharpCodeBuilderTest {
        [Test]
        public void CommentBlockTest() {
            var builder = new BSharpCodeBuilder();
            builder.WriteCommentBlock("MyTitle",new{test="The best",rest="Not zest!"});
            var result = builder.ToString();
            Console.WriteLine(result);
            Assert.AreEqual(@"########################################################################################################################
####                                                    MyTitle                                                     ####
####                    test                           : The best                                                   ####
####                    rest                           : Not zest!                                                  ####
########################################################################################################################
",result);
        }
        [Test]
        public void NestedNamespaces()
        {
            var builder = new BSharpCodeBuilder();
            builder.StartNamespace("test");
            builder.StartNamespace("best");
            var result = builder.ToString();
            Console.WriteLine(result);
            Assert.AreEqual(@"namespace test
	namespace best
", result);
            var xml = new BxlParser().Parse(result);
            Console.WriteLine(xml);
        }
        [Test]
        public void NamespaceClassElement()
        {
            var builder = new BSharpCodeBuilder();
            builder.StartNamespace("test");
            builder.StartClass("mya",new{a=1,b="23",c="dsdsd gfgfg !!!"});
            builder.WriteAttributesLined(new{x=2,y=true});
            builder.WriteElement("test","a",inlineattributes:new{a=5,b=6});
            builder.EndClass();
            builder.StartClass("mya2", new { a = 3, b = "23", c = "dsdsd gfgfg !!!" });
            builder.WriteAttributesLined(new { x = 3, y = false });
            builder.EndClass();
            var result = builder.ToString();
            Console.WriteLine(result);
            Assert.AreEqual(@"namespace test
	class mya a=1 b=23 c='dsdsd gfgfg !!!'
		x=2
		y=True
		test a a=5 b=6
	class mya2 a=3 b=23 c='dsdsd gfgfg !!!'
		x=3
", result);
            var xml = new BxlParser().Parse(result);
            Console.WriteLine(xml);
        }

        [Test]
        public void EqualReParse() {
            Assert.AreEqual("%test+,:", "__PERC__test__PLUS____COMMA____DBL__".Unescape(EscapingType.XmlName));
        }
        [Test]
        public void BugToManyEmptyLines() {
            var b = new BSharpCodeBuilder();
            b.WriteElement("x","y","z",linedattributes:new{x=(string)null,y=(string)null});
            b.WriteElement("x","y","z");
            var result = b.ToString();
            Console.WriteLine(result);
            Assert.AreEqual(@"x y 'z'
x y 'z'
",result);

        }

        [Test]
        public void NamespaceClassElementWithEscapes()
        {
            var builder = new BSharpCodeBuilder();
            builder.StartNamespace("test");
            builder.StartClass("mya", new { a = 1, b = "23", c = "dsdsd gfgfg !!!" });
            builder.WriteAttributesLined(new { x = 2, y = true });
            builder.WriteElement("%test+,:", "a",value:"trtr\r\ndsds", inlineattributes: new { a = 5, b = 6 });
            builder.EndClass();
            builder.StartClass("mya2", new { a = 3, b = "23", c = "dsdsd\r\ngfgfg !!!" });
            builder.WriteAttributesLined(new { x = 3, y = false });
            builder.EndClass();
            var result = builder.ToString();
            Console.WriteLine(result);
            Assert.AreEqual(@"namespace test
	class mya a=1 b=23 c='dsdsd gfgfg !!!'
		x=2
		y=True
		%test+__COMMA____DBL__ a a=5 b=6 : """"""trtr
dsds""""""
	class mya2 a=3 b=23 c=""""""dsdsd
gfgfg !!!""""""
		x=3
", result);
            var xml = new BxlParser().Parse(result);
            Console.WriteLine(xml);
        }
    }
}