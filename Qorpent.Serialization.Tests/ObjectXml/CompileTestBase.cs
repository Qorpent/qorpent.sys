using System;
using System.Linq;
using NUnit.Framework;
using Qorpent.Bxl;
using Qorpent.ObjectXml;

namespace Qorpent.Serialization.Tests.ObjectXml {
	public class CompileTestBase {
		protected ObjectXmlCompilerIndex Compile(string code) {
			var xml = new BxlParser().Parse(code, "c.bxl");
			var cfg = new ObjectXmlCompilerConfig();
			cfg.UseInterpolation = true;

			var compiler = new ObjectXmlCompiler();
			compiler.Initialize(cfg);
			return  compiler.Compile(new[] {xml});
		}

		protected ObjectXmlCompilerIndex CompileAll(bool single,params string[] code) {
			var parser = new BxlParser();
			var idx = 0;
			var xmls = code.Select(_ => parser.Parse(_, (idx++) + ".bxl")).ToArray();
			var cfg = new ObjectXmlCompilerConfig();
			cfg.UseInterpolation = true;
			if (single) {
				cfg.SingleSource = true;
			}
			var compiler = new ObjectXmlCompiler();
			compiler.Initialize(cfg);
			var result = compiler.Compile(xmls);
			return result;

		}
	}
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class TbxlSamples : CompileTestBase {
		[Test]
		public void ColsetWithImportAndCodeSubstSample() {
			var code = @"
class base abstract static
	_JAN = 11
	_NOJAN = '12,13,14,15,16,17,18,19,110,111,112'
	_MONTH = '${_JAN},${_NOJAN}'
	_KVART = '1,2,3,4'
	_SMONTH = '${_KVART},22,24,25,27,28,210,211'
	_FACT = '${_MONTH}${_SMONTH}'
	_DEFTITLE = '{PERIODNAME} {YEAR}'
	_DEFCOL = Б1
	_FP_FACTSNG = -201
	_FP_PLAN = -301
	_FP_PLANSNG = -302

base colset abstract	

colset cs_basis  abstract 
	col '${_colcode,_DEFCOL}' '${_title,_DEFTITLE}' customcode='${_ccbasis:code}_FACT_MAIN' forperiods='${_FACT}' condition='${.condition}'
	col '${_colcode,_DEFCOL}' '${_title,_DEFTITLE}' customcode='${_ccbasis:code}_FACT_SNG' period='${_FP_FACTSNG}' forperiods='${_NOJAN}' condition='${.condition:True} and COL_SNG'
	col '${_colcode,_DEFCOL}' '${_title,_DEFTITLE}' customcode='${_ccbasis:code}_FACT_SNG' period='${_FP_PLAN}' condition='${.condition:True} and COL_PLAN'
	col '${_colcode,_DEFCOL}' '${_title,_DEFTITLE}' customcode='${_ccbasis:code}_FACT_SNG' period='${_FP_PLANSNG}' forperiods='${_NOJAN}' condition='${.condition:True} and COL_PLAN and COL_SNG'

cs_basis cs_fact _colcode=Б1 static
cs_basis cs_fact_pd _colcode=Pd static

colset b1_and_pd 
	import cs_fact
	import cs_fact_pd

	";
			var result = Compile(code);
			Console.WriteLine(result.Working[2].Compiled);
			/*foreach (var r in result.Working) {
				Console.WriteLine(r.Compiled);
			}*/

		}
	}
}