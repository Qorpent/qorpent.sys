using System;
using System.Collections.Generic;
using NUnit.Framework;
using Qorpent.BSharp;

namespace Qorpent.Serialization.Tests.BSharp{
	[TestFixture]
	public class GlobalSupport  : CompileTestBase{
		[Test]
		public void CanInterpolateWithGlobals(){
			var result = Compile(@"
class A a=z${x} b=${y} y=3",null,new{x=1,y=2,z=3});
			var a = result.Get("A");
			var x = a.Compiled.ToString().Replace("\"", "'");
			Console.WriteLine(x);
			Assert.AreEqual("<class code='A' a='z1' b='3' y='3' fullcode='A' />", x);
		}

		[Test]
		public void ConditionsTreatedAsGlobals(){
			var result = Compile(@"
class A a=z${x} b=${y} y=3",new Dictionary<string, string>{{"x","1"},{"y","2"},{"z","3"}});
			var a = result.Get("A");
			var x = a.Compiled.ToString().Replace("\"", "'");
			Console.WriteLine(x);
			Assert.AreEqual("<class code='A' a='z1' b='3' y='3' fullcode='A' />", x);
		}
		
	}
}
