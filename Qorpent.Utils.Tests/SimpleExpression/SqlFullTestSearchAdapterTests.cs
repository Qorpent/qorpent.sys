using System;
using NUnit.Framework;
using Qorpent.Utils.Sql;

namespace Qorpent.Utils.Tests.SimpleExpression {
    [TestFixture]
    public class SqlFullTestSearchAdapterTests {

        [TestCase("�* � �", "\"�*\" & �")]
        [TestCase("� � �", "� & �")]
        [TestCase("� �", "� | �")]
        [TestCase("(� �) + (� �)", "( � | � ) & ( � | � )")]
        [TestCase("aaa � ���!", "FORMSOF( INFLECTIONAL, aaa ) & ���")]
        [TestCase("�% � �", "FORMSOF( THESAURUS, � ) & �")]
        [TestCase("� + �", "� & �")]
        [TestCase("�+�", "� & �")]
        [TestCase("�*+�*", "\"�*\" & \"�*\"")]
        [TestCase("� ��� �", "� | �")]
        [TestCase("� ? �", "� | �")]
        [TestCase("�?�", "� | �")]
        [TestCase("� �� �", "� &! �")]
        [TestCase("� - �", "� &! �")]
        [TestCase("�-�", "� &! �")]
        [TestCase("\"�-�\"+(\"�+�\"?\"�?�\")", "\"�-�\" & ( \"�+�\" | \"�?�\" )")]
        public void MainTest(string source, string result) {
            Assert.AreEqual(result,new SqlFullTextSearchAdapter().Convert(source));    
        }

        [Test]
        public void Bug_In_AndQuery() {
            var sqlquey = new SqlFullTextSearchAdapter().Convert("������� + ��������");
            Console.WriteLine(sqlquey);
            Assert.AreEqual("FORMSOF( INFLECTIONAL, ������� ) & FORMSOF( INFLECTIONAL, �������� )",sqlquey);
        }
       
        
      
    }
}