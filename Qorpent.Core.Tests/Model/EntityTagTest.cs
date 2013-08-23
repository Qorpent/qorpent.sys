using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Model;

namespace Qorpent.Core.Tests.Model
{
	[TestFixture]
	public class EntityTagTest
	{
		[TestCase("/x:1/","x",null,true)]
		[TestCase("","x",null,false)]
		[TestCase("/x: 1 / /y : 3/","x",null,true)]
		[TestCase("/x:1/ /y : 3/","x","1",true)]
		[TestCase("/x: 1 / /y : 3/","x","1",true)]
		[TestCase("/x: 1 / /y : 3/","z",null,false)]
		public void IsTagSetWorksNoValue(string tag, string name, string value, bool result)
		{
			var e = new Entity {Tag = tag};
			Assert.AreEqual(result, e.IsTagSet(name,value));
		}

		[TestCase("/x:1/", "x", "1")]
		[TestCase("", "x", "")]
		[TestCase("/x: 1 / /y : 3/", "x", "1")]
		public void TagGetWorks(string tag, string name, string result) {
			var e = new Entity { Tag = tag };
			Assert.AreEqual(result, e.TagGet(name));
		}
	}
}
