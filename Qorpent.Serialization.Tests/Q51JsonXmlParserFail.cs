using System;
using NUnit.Framework;
using Qorpent.Dsl;
using Qorpent.Json;

namespace Qorpent.Serialization.Tests {
	[TestFixture]
	public class Q51JsonXmlParserFail {
		public const string maintest = @"{
	'in' : 'content',
	'type' : 'text',
	'actions' : [
		{
			'actiontype' : 'select',
			'code' : 'title',
			'selector' : '#title',
		},
		{
			'actiontype' : 'select',
			'code' : 'date',
			'selector' : '.data',
		},
		{
			'actiontype' : 'select',
			'code' : 'text',
			'selector' : '#content',
			'type': 'xml'
		},
	]
}";
		public const string minarraytest = @"
{
	a : [1,2]
}";
		public const string notfinishedcomaarraytes = @"
		{
			a : [1,2,],
		}";

		public const string minfail = @"
		{
	'in' : 'content',
	
}";
		public const string minfail2 = @"
		{

	'actions' : [
		{
			'actiontype' : 'select',
			'code' : 'title',
			'selector' : '#title',
		},
],
	
}";

		[TestCase(maintest)]
		[TestCase(minarraytest)]
		[TestCase(notfinishedcomaarraytes)]
		[TestCase(minfail)]
		[TestCase(minfail2)]
		[TestCase("[1,2]")]
		[TestCase("[{x:'1'},{y:'2'}]")]
		[TestCase("[{x:\"1\"},{y:\"2\"}]")]
		[TestCase("{'x':[1,2]}")]
		[TestCase("{x:[1,2]}")]
		[TestCase("{x:['1','2']}")]
		[TestCase("{x:['1','2\\\"']}")]
		[TestCase("{x:['1','2\"']}")]
		[TestCase("{x:[{y:1},2]}")]
		public void TestArrayParsing(string code) {
			Console.WriteLine(((ISpecialXmlParser)new JsonParser()).Parse(code));
		}
	}
}