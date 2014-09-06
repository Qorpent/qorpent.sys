﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using Qorpent.Utils.Extensions;
namespace Qorpent.Serialization.Tests.BSharp {
	[TestFixture]
	public class ConditionalInternals:CompileTestBase {
		[Test]
		public void NonConditional() {
			var code = @"
class A
	test 1
	test 2
	test 3
";
			var result = Compile(code).Get("A");
			Assert.AreEqual(3,result.Compiled.Elements().Count());
		}

		[Test]
		[Category("NOTC")]
		public void Q229_Build534_OverridenConditional_Stability(){
			for (var i = 0; i < 1000; i++){
				try{
					OverridenConditional();
				}
				catch(Exception e){
					Console.WriteLine(i);
					Assert.Fail(e.ToString());
				}
			}
		}

		

		[Test]
		public void OverridenConditional()
		{
			var code = @"

class A
	test 1 if='${x}'
	test 2 if='!${x}'
	test 3 if='${x} | z'
A B x=y y=1
";
			var result = Compile(code).Get("B");
			Assert.AreEqual(2, result.Compiled.Elements().Count());

			code = @"

class A
	test 1 if='${x}'
	test 2 if='!${x} & !z'
	test 3 if='${x} | z'
A B x=y z=1
";
			result = Compile(code).Get("B");
			Assert.AreEqual(1, result.Compiled.Elements().Count());
		}

        [Test]
        public void NamespaceCanBeConditional()
        {
            var code = @"
namespace A if='USE_A'
    class B
";
            var r = Compile(code);
            Assert.AreEqual(0, r.Working.Count);
            r = Compile(code, new Dictionary<string, string> { { "USE_A", "1" } });
            Assert.AreEqual(1, r.Working.Count);
        }

        [Test]
        public void OverridesCanBeConditional()
        {
            var code = @"
~class A x=2 if='USE_A'
class A x=1
";
            var r = Compile(code);
            Assert.AreEqual("1", r.Working[0].Compiled.Attr("x"));
            r = Compile(code, new Dictionary<string, string> { { "USE_A", "1" } });
            Assert.AreEqual("2", r.Working[0].Compiled.Attr("x"));
        }

		[Test]
		public void ClassCanBeConditional() {
			var code = @"
class A x=1 z=true if='z'
	test 1 if='x'
	test 2 if='x | y'
	test 3 if='! x'
";
			Assert.NotNull( Compile(code).Get("A"));

			code = @"
class A x=1 z=false if='z'
	test 1 if='x'
	test 2 if='x | y'
	test 3 if='! x'
";
			Assert.Null(Compile(code).Get("A"));
		}

		[Test]
		public void CanSupplyConditionsWithCompiler()
		{
			var code = @"
class A x=1 z=false if='z'
	test 1 if='x'
	test 2 if='x | y'
	test 3 if='! x'
";
			Assert.NotNull(Compile(code, new Dictionary<string,string>{{"z","true"}}).Get("A"));

			code = @"
class A x=1 z=true if='z'
	test 1 if='x'
	test 2 if='x | y'
	test 3 if='! x'
";
			Assert.Null(Compile(code, new Dictionary<string, string> { { "z", "false" } }).Get("A"));
		}

		[Test]
		public void SimpleConditional()
		{
			var code = @"
class A x=1
	test 1 if='x'
	test 2 if='x | y'
	test 3 if='! x'
";
			var result = Compile(code).Get("A");
			Assert.AreEqual(2, result.Compiled.Elements().Count());
			code = @"
class A 
	test 1 if='x'
	test 2 if='x | y'
	test 3 if='! x'
";
			result = Compile(code).Get("A");
			Assert.AreEqual(1, result.Compiled.Elements().Count());

			code = @"
class A y=1
	test 1 if='x'
	test 2 if='x | y'
	test 3 if='! x'
";
			result = Compile(code).Get("A");
			Assert.AreEqual(2, result.Compiled.Elements().Count());
		}

		[Test]
		public void AnyLevelConditional() {
			var code = @"
class A y=1
	test 1 
		testl if='x'
	test 2 
		testl if='y'
	test 3
		testl
";
			var result = Compile(code).Get("A");
			Assert.AreEqual(2, result.Compiled.Descendants("testl").Count());
		}
	}
}