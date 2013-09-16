using System;
using System.Diagnostics;
using NUnit.Framework;

namespace Qorpent.Serialization.Tests.BSharp {
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class TbxlSamples : CompileTestBase {

		[Test]
		public void SampleToSetupPlanFactForms() {
			var code = @"

class thema 'Понятие темы' abstract prototype=Thema
	element form
	element report

thema eco 'Базовая форма по экономике' abstract prototype=EcoThema
	_fact = true 
	_plan = true 
	_korr = false 

	_form = true
	_obj  = true
	_svod = true

	form A if='_form & _fact'
	form B if='_form & _plan'
	form C if='_form & _korr'

	report Aa if='_fact & _svod'
	report Ab if='_fact & _obj'
	
	report Ba if='_plan & _svod'
	report Bb if='_plan & _obj'
	
	report Ca if='_korr & _svod'
	report Cb if='_korr & _obj'

eco balans 'Типа баланс' rootrow=m111 
	+report Ab use=true
		col TEST1

eco s_w_korr 'Типа что-то c коррективами' rootrow=a231 _korr=true

eco simp_report 'Типа тупо просто отчет' rootrow=a431  
	_form=false 
	_plan=false 
	_obj=false

balans ext_balans 'Типа накрутили отчет предприятия по факту'
	~report Ab use=false
	+report Ab name='Это супер!' use2=true
		col TEST2 'Очень нужная колонка'
";
			var result = Compile(code);
			foreach (var th in result.Working) {
				Console.WriteLine(th.Compiled);
				Console.WriteLine(@"

-----------------------------------------------------------------

");
			}
		}


		

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
		[Repeat(200)]
		public void Sample200TimesTiming()
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
class thema 'Понятие темы' abstract
	element form
	element report

thema eco 'Базовая форма по экономике' abstract
	_fact = true 
	_plan = true 
	_korr = false 

	_form = true
	_obj  = true
	_svod = true

	form A if='_form & _fact'
	form B if='_form & _plan'
	form C if='_form & _korr'

	report Aa if='_fact & _svod'
	report Ab if='_fact & _obj'
	
	report Ba if='_plan & _svod'
	report Bb if='_plan & _obj'
	
	report Ca if='_korr & _svod'
	report Cb if='_korr & _obj'

eco balans 'Типа баланс' rootrow=m111

eco s_w_korr 'Типа что-то c коррективами' rootrow=a231 _korr=1

eco simp_report 'Типа тупо просто отчет' rootrow=a431  
	_form=false 
	_plan=false 
	_obj=false

eco ext_balans 'Типа накрутили отчет предприятия по факту'
	import balans
	+report Ab name='Это супер!'
		col TEST 'Очень нужная колонка'
	";
		 Compile(code);
		


		}
	}
}