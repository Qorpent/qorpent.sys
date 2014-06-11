using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Qorpent.BSharp;
using Qorpent.Config;
using Qorpent.Uson;
using Qorpent.Utils.Extensions;

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
		public void BugCanInterpolateWithGlobalsForReports()
		{
			var result = Compile(@"
class A y='${tp.year:2014}' p='${tp.period:1}'", null, new Dictionary<string,object>{{"tp.year",2013},{"tp.period",2}});
			var a = result.Get("A");
			var x = a.Compiled.ToString().Replace("\"", "'");
			Console.WriteLine(x);
			Assert.AreEqual("<class code='A' y='2013' p='2' fullcode='A' />", x);
		}

		[Test]
		public void CanUseUObj(){
			dynamic g = new UObj();
			g.x = 1;
			g.y = 2;
			g.z = 3;
			var result = Compile(@"
class A a=z${x} b=${y} y=3", null, g);
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
	
	
		[Test]
		public void BugNotNormalReportCompilation(){
			var code = @"
class report abstract  prototype=report
	basedir='c:/mnt/reporthub'
		
class bankreport abstract
	import report
	outdir = '${basedir}/~{commonkey}/raw/~{objname}/'
	outfile = '${reportname}${ext}.html'
	period = '${tp.period:1}'
	year = '${tp.year:2014}'

generator
	import bankreport
	dataset
		item out_valuta=RUB 
	dataset 
		item obj=445
	
	dataset
		item template=balans2011_corp_bank	reportname = 'Бухгалтерский баланс (Форма №1)'

";
			var cfg = new BSharpConfig{Global = new ConfigBase()};
			cfg.Global["tp.year"] = "2013";
			cfg.Global["tp.period"] = "2";
			var ctx = BSharpCompiler.Compile(code, cfg);
			foreach (var e in ctx.GetErrors(ErrorLevel.Warning)){	
				Console.WriteLine(e.ToLogString());
			}
			Assert.AreEqual(0, ctx.GetErrors(ErrorLevel.Warning).Count());
			Assert.AreEqual(1,ctx.Get(BSharpContextDataType.Working).Count());
			var cls = ctx.Get(BSharpContextDataType.Working).First();
			Assert.AreEqual("2013",cls.Compiled.Attr("year"));
			Assert.AreEqual("2",cls.Compiled.Attr("period"));
		}
	}
}
