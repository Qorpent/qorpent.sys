using System;
using System.Diagnostics;
using NUnit.Framework;

namespace Qorpent.Serialization.Tests.ObjectXml {
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
	_FACT = '${_MONTH},${_SMONTH}'
	_DEFTITLE = '{PERIODNAME} {YEAR}'
	_DEFCOL = Б1
	_FP_FACTSNG = -201
	_FP_PLAN = -301
	_FP_PLANSNG = -302

base colset abstract	

colset cs_basis  abstract 
	col '${_colcode,_DEFCOL}' '${_title,_DEFTITLE}' customcode='${_ccbasis,.code}_FACT_MAIN' forperiods='${_FACT}' condition='${.condition}'
	col '${_colcode,_DEFCOL}' '${_title,_DEFTITLE}' customcode='${_ccbasis,.code}_FACT_SNG' period='${_FP_FACTSNG}' forperiods='${_NOJAN}' condition='${.condition:True} and COL_SNG'
	col '${_colcode,_DEFCOL}' '${_title,_DEFTITLE}' customcode='${_ccbasis,.code}_FACT_SNG' period='${_FP_PLAN}' condition='${.condition:True} and COL_PLAN'
	col '${_colcode,_DEFCOL}' '${_title,_DEFTITLE}' customcode='${_ccbasis,.code}_FACT_SNG' period='${_FP_PLANSNG}' forperiods='${_NOJAN}' condition='${.condition:True} and COL_PLAN and COL_SNG'

cs_basis cs_fact _colcode=Б1 static
cs_basis cs_fact_pd _colcode=Pd static

colset b1_and_pd 
	import cs_fact
	import cs_fact_pd

	";
			var result = Compile(code);
			Debug .Print(result.Working[2].Compiled.ToString());
	

		}

		[Test]
		[Explicit("timing")]
		[Repeat(3000)]
		public void ColsetWithImportAndCodeSubstSample3000Timing()
		{
			var code = @"
class base abstract static
	_JAN = 11
	_NOJAN = '12,13,14,15,16,17,18,19,110,111,112'
	_MONTH = '${_JAN},${_NOJAN}'
	_KVART = '1,2,3,4'
	_SMONTH = '${_KVART},22,24,25,27,28,210,211'
	_FACT = '${_MONTH},${_SMONTH}'
	_DEFTITLE = '{PERIODNAME} {YEAR}'
	_DEFCOL = Б1
	_FP_FACTSNG = -201
	_FP_PLAN = -301
	_FP_PLANSNG = -302

base colset abstract	

colset cs_basis  abstract 
	col '${_colcode,_DEFCOL}' '${_title,_DEFTITLE}' customcode='${_ccbasis,.code}_FACT_MAIN' forperiods='${_FACT}' condition='${.condition}'
	col '${_colcode,_DEFCOL}' '${_title,_DEFTITLE}' customcode='${_ccbasis,.code}_FACT_SNG' period='${_FP_FACTSNG}' forperiods='${_NOJAN}' condition='${.condition:True} and COL_SNG'
	col '${_colcode,_DEFCOL}' '${_title,_DEFTITLE}' customcode='${_ccbasis,.code}_FACT_SNG' period='${_FP_PLAN}' condition='${.condition:True} and COL_PLAN'
	col '${_colcode,_DEFCOL}' '${_title,_DEFTITLE}' customcode='${_ccbasis,.code}_FACT_SNG' period='${_FP_PLANSNG}' forperiods='${_NOJAN}' condition='${.condition:True} and COL_PLAN and COL_SNG'

cs_basis cs_fact _colcode=Б1 static
cs_basis cs_fact_pd _colcode=Pd static

colset b1_and_pd 
	import cs_fact
	import cs_fact_pd

	";
		 Compile(code);
		


		}
	}
}