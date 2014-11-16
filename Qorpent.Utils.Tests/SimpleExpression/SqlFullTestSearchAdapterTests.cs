using System;
using NUnit.Framework;
using Qorpent.Utils.Sql;

namespace Qorpent.Utils.Tests.SimpleExpression {
    [TestFixture]
    public class SqlFullTestSearchAdapterTests {

        [TestCase("א* ט ב", "\"א*\" & ב")]
        [TestCase("א ט ב", "א & ב")]
        [TestCase("א ב", "א | ב")]
        [TestCase("(א ב) + (ג ד)", "( א | ב ) & ( ג | ד )")]
        [TestCase("aaa ט בבב!", "FORMSOF( INFLECTIONAL, aaa ) & בבב")]
        [TestCase("א% ט ב", "FORMSOF( THESAURUS, א ) & ב")]
        [TestCase("א + ב", "א & ב")]
        [TestCase("א+ב", "א & ב")]
        [TestCase("א*+ב*", "\"א*\" & \"ב*\"")]
        [TestCase("א טכט ב", "א | ב")]
        [TestCase("א ? ב", "א | ב")]
        [TestCase("א?ב", "א | ב")]
        [TestCase("א םו ב", "א &! ב")]
        [TestCase("א - ב", "א &! ב")]
        [TestCase("א-ב", "א &! ב")]
        [TestCase("\"א-ב\"+(\"א+ב\"?\"א?ב\")", "\"א-ב\" & ( \"א+ב\" | \"א?ב\" )")]
        public void MainTest(string source, string result) {
            Assert.AreEqual(result,new SqlFullTextSearchAdapter().Convert(source));    
        }

        [Test]
        public void Bug_In_AndQuery() {
            var sqlquey = new SqlFullTextSearchAdapter().Convert("נמיחלאם + ךףיגארוג");
            Console.WriteLine(sqlquey);
            Assert.AreEqual("FORMSOF( INFLECTIONAL, נמיחלאם ) & FORMSOF( INFLECTIONAL, ךףיגארוג )",sqlquey);
        }
       
        
      
    }
}