using NUnit.Framework;

namespace Qorpent.Utils.Tests.SimpleExpression {
    [TestFixture]
    public class SimpleExpressionParserTests {

        [TestCase("ab+cd", "(G: (L: ab) (O: +) (L: cd))")]
        [TestCase("ab+ cd", "(G: (L: ab) (O: +) (L: cd))")]
        [TestCase("ab +cd", "(G: (L: ab) (O: +) (L: cd))")]
        [TestCase("ab + cd", "(G: (L: ab) (O: +) (L: cd))")]
        public void CanParseSimpleBinaries(string source, string result) {
            Assert.AreEqual(result,new SimpleExpressionParser().Parse(source).GetDescriptiveString());
        }

        [TestCase("@ab+cd", "(G: @:(L: ab) (O: +) (L: cd))")]
        [TestCase("ab+ @cd", "(G: (L: ab) (O: +) @:(L: cd))")]
        public void CanParsePrefixes(string source, string result)
        {
            Assert.AreEqual(result, new SimpleExpressionParser().Parse(source).GetDescriptiveString());
        }

        [TestCase("a + @( @b-c)", "(G: (L: a) (O: +) @:(G: @:(L: b) (O: -) (L: c)))")]
        public void CanParseGroups(string source, string result)
        {
            Assert.AreEqual(result, new SimpleExpressionParser().Parse(source).GetDescriptiveString());
        }

        [TestCase("a + \"( b\\\"-d)\"", "(G: (L: a) (O: +) (Q(\"): ( b\"-d)))")]
        [TestCase("a + @\"( b-c)\"", "(G: (L: a) (O: +) @:(Q(\"): ( b-c)))")]
        public void CanParseQuotes(string source, string result)
        {
            Assert.AreEqual(result, new SimpleExpressionParser().Parse(source).GetDescriptiveString());
        }

        [TestCase("ab*+cd*", "(G: (L: ab):* (O: +) (L: cd):*)")]
        public void CanParseSuffixes(string source, string result) {
            var parser = new SimpleExpressionParser();
            parser.Operators = new[] {'+'};
            parser.Suffixes = new[] {'*'};
            Assert.AreEqual(result, parser.Parse(source).GetDescriptiveString());
        }

        [TestCase("xxx", "(L: xxx)")]
        [TestCase("@xxx", "@:(L: xxx)")]
        [TestCase("(((@xxx)))", "@:(L: xxx)")]
        public void CanExpandSingle(string source, string result)
        {
            Assert.AreEqual(result, new SimpleExpressionParser().Parse(source).GetDescriptiveString());
        }
    }
}