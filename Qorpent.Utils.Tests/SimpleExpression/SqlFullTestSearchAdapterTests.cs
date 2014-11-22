using System;
using NUnit.Framework;
using Qorpent.Utils.Sql;

namespace Qorpent.Utils.Tests.SimpleExpression {
    [TestFixture]
    public class SqlFullTestSearchAdapterTests {

        [TestCase("а* и б", "\"а*\" & б")]
        [TestCase("а и б", "а & б")]
        [TestCase("а б", "а | б")]
        [TestCase("(а б) + (в г)", "( а | б ) & ( в | г )")]
        [TestCase("aaa и ббб!", "FORMSOF( INFLECTIONAL, aaa ) & ббб")]
        [TestCase("а% и б", "FORMSOF( THESAURUS, а ) & б")]
        [TestCase("а + б", "а & б")]
        [TestCase("а+б", "а & б")]
        [TestCase("а*+б*", "\"а*\" & \"б*\"")]
        [TestCase("а или б", "а | б")]
        [TestCase("а ? б", "а | б")]
        [TestCase("а?б", "а | б")]
        [TestCase("а не б", "а &! б")]
        [TestCase("а - б", "а &! б")]
        [TestCase("а-б", "а &! б")]
        [TestCase("\"а-б\"+(\"а+б\"?\"а?б\")", "\"а-б\" & ( \"а+б\" | \"а?б\" )")]
        public void MainTest(string source, string result) {
            Assert.AreEqual(result,new SqlFullTextSearchAdapter().Convert(source));    
        }

        [Test]
        public void Bug_In_AndQuery() {
            var sqlquey = new SqlFullTextSearchAdapter().Convert("ройзман + куйвашев");
            Console.WriteLine(sqlquey);
            Assert.AreEqual("FORMSOF( INFLECTIONAL, ройзман ) & FORMSOF( INFLECTIONAL, куйвашев )",sqlquey);
        }

        [Test]
        public void Bug_With_Number()
        {
            var sqlquey = new SqlFullTextSearchAdapter().Convert("Пархоменко + 5");
            Console.WriteLine(sqlquey);
            Assert.AreEqual("FORMSOF( INFLECTIONAL, Пархоменко ) & 5", sqlquey);
        }
       
        
      
    }
}