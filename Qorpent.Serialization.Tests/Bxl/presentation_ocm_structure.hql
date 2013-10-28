report PRES_OCM, "Презентация для OCM"
	pr_dtype=MSColumn 
	view=verstka_1	
	pr_typecol=AHD 
	slotview=graph_slot
	pr_measure=тыс.руб.
	pr_numberscalevalue="1,1,1"
	pr_decimals = 0
	numberformat="#,0"
	prg_showvalues="1"
	prg_showsum="1"
	pr_linethikness=4
	pr_measure="DYNAMIC"
	fixedobj=1046
	protocoltype=div
	protocoldiv=OCM
	pr_legendposition="BOTTOM"
	pr_pieradius=180
	pr_clearempty = 1	
	pr_ylimits='auto'
	pr_ylimitsplus=0
	pr_hidevalueslover = 3 
	singlecontents=true
	pr_plotspace=40
	newstylerender=true



	# !НЕ ЗНАЮ ЧТО ДЕЛАЕТ ТЭГ PROCESS, НО БЕЗ НЕГО НИЧЕ НЕ РАБОТАЕТ
	process="top_management"


	 
	
#	pr_colors='B84A1F,D6C445,842006,85715D'
#	pr_colors='C9501A (кир),89211B(кольч),842006,85715D'
	block 'Производство', role="OBPROIZ_,MTR_,COST_", targetgroup="div_OCM", process=protocol, 
		slide 'Объемы производства по видам проката и предприятиям группы УГМК-ОЦМ',
			protocol=1 protocolview=m140table
			pr_dtype=MSStackedColumn, uselines=1,
			objset="1066,468,415,497"	
			slot 'Производство медного проката',
				ref m140852 'Прокат медный' protocol=1 
				ref m140853 'Прокат латунный' protocol=1 
				ref m140854 'Прокат бронзовый' protocol=1 
				ref m140855 'Прокат медно-никелевый' protocol=1 
		slide 'Выполнение объемов производства',noheader=1, slideformat=onebigtable, protocolview=proizv,  
			protocol=1 	pr_tablestyle="font-size~125%;"	
			pr_tableclass="six-col-bold level1-bold", pr_rowheight=25, pr_colwidth="350,110px"
#			slot 'Производство меди и цинка ' slotview=table_slot,  ts_tree=true, #ts_m140=true,  ts_showroot=true	
			slot 'Производство медного проката',
				ref m140852 'Прокат медный' protocol=1 
				ref m140853 'Прокат латунный' protocol=1 
				ref m140854 'Прокат бронзовый' protocol=1 
				ref m140855 'Прокат медно-никелевый' protocol=1 

			
	qxi::include template_protocol.bxl : " //*[@key='Показатели ФХД_протокол'] "	
	
	block 'Титульный лист', noheader=1, targetgroup="div_OCM", process=top_management, contents=true
		slide 'Титульный лист', slideformat=frontpageslot
			slot 'Титульный лист', slotview=frontpage

	block 'Основные параметры бюджета', role="FIN_, DR_, MTR_, COST_" , targetgroup="div_OCM", process=top_management, contents=true
		qxi::include template_budget.bxl : " //*[@key='osn_budget'] "
#		slide 'Котировки металлов в текущем году', pr_measure=руб.,	pr_linethikness=4, slotview=metalprice, 
			slot 'Котировки МЕДИ на LMЕ', metal=cu, pr_caption='Цены на медь', pr_measure="$ \\за тонну CU"
				row PRD_MACRO_1, 'Медь'
				pr_yminvalue=6500
				pr_ymaxvalue=9000
			slot 'Котировки ЦИНКА на LMЕ', metal=zn, pr_caption='Цены на цинк', pr_measure="$ \\за тонну ZN"
				row PRD_MACRO_1, 'Цинк'
				pr_yminvalue=1700
				pr_ymaxvalue=2200
			slot 'Котировки НИКЕЛЯ на LMЕ', metal=ni, pr_caption='Цены на никель', pr_measure="$ \\за тонну NI"
				row PRD_MACRO_1, 'Никель'
				pr_yminvalue=10000
				pr_ymaxvalue=25000
			slot 'Котировки СВИНЦА на LMЕ', metal=pb, pr_caption='Цены на свинец', pr_measure="$ \\за тонну PB"
				row PRD_MACRO_1, 'Свинец'
				pr_yminvalue=1500
				pr_ymaxvalue=2500
#		qxi::include template_budget.bxl : " //*[@key='kotir_valute'] "	
		
		slide 'Котировки металлов на LME, цены на сырье и продукцию', targetgroup="div_OCM", process=top_management, contents=true
			slideformat=twobigslots
			colcategory=1, uselines=1, pr_sum=obj_1046, pr_numberscalevalue="1,1,1", pr_measure="$~за тонну"
			pr_pyname="Котировки LME, $", pr_syname="Цены на сырье и продукцию,$"
			pr_typecol=month_b1, pr_linedashlen=3, pr_linedashgap=10
			pr_anchorbordercolor=669ACD,  pr_anchorbgcolor=053666 
			slot 'Медный прокат'
				pr_colors='F2A5AA,D6C445,B84A1F'
				pr_ylimits='5000,9000|5000,12000'
				ref m203117 'Цена меди на LME ($ за тонну)', renderas=Line, pr_linedashed=1, pr_anchorradius=3,
# 			    ref m205762 'Цена лом меди ($ за тонну)', renderas=Line
#				ref m205761 'Цена катодной меди ($ за тонну)', renderas=Line
				ref m205711 'Цена проката медного ($ за тонну)', renderas=Line, pr_anchorradius=7, pr_anchorsides=3
				ref m2031421 'Цена сырья для проката медного ($ за тонну)', renderas=Area, pr_anchorradius=5, pr_anchorsides=5
			slot 'Латунный прокат'
				usear=1, colcategory=1,
				pr_colors='BFBFBB,E8E8E8,E09070,D6C445'
#				pr_ylimits='0,8200'
				ref m203118 'Цена цинка на  LME ($ за тонну)', renderas=Line, pr_linedashed=1, pr_anchorradius=3
#				ref m205751 'Цена цинка чушкового ($ за тонну)', renderas=Line
#				ref m205762 'Цена лом меди ($ за тонну)', renderas=Line
				ref m205721 'Цена проката латунного ($ за тонну)', renderas=Line, pr_anchorradius=7, pr_anchorsides=3
				ref m2031422 'Цена сырья для проката латунного ($ за тонну)', renderas=Area, pr_anchorradius=5, pr_anchorsides=5		
				ref m203117 'Цена меди на LME ($ за тонну)', renderas=Line, pr_linedashed=1, pr_linedashlen=5, pr_linedashgap=15
				
		slide 'Котировки металлов на LME, цены на сырье и продукцию', targetgroup="div_OCM", process=top_management, contents=true
			slideformat=twobigslots
			colcategory=1, uselines=1, pr_sum=obj_1046, pr_numberscalevalue="1,1,1", pr_measure="$~за тонну"
			pr_pyname="Котировки LME, $", pr_syname="Цены на сырье и продукцию,$"
			pr_typecol=month_b1, pr_linedashlen=3, pr_linedashgap=10
			pr_anchorbordercolor=669ACD,  pr_anchorbgcolor=053666 			
			slot 'Бронзовый прокат'
#				pr_colors='F2A5AA,E09070,8F8381'
				pr_ylimits='5000,9000'
				ref m203117 'Цена меди на  LME ($ за тонну)', renderas=Line, pr_linedashed=1, pr_anchorradius=3
#				ref m205762 'Цена лом меди ($ за тонну)'
				ref m205731 'Цена проката бронзового($ за тонну)', renderas=Line, , pr_anchorradius=7, pr_anchorsides=3
				ref m2031423 'Цена сырья для проката бронзового($ за тонну)', renderas=Area, pr_anchorradius=5, pr_anchorsides=5
			slot 'Медно-никелевый прокат' 
				pr_ylimits='16000,23000'
#				pr_colors='A6AAA9,F2A5AA,B3A291'
				ref m203117 'Цена меди на  LME ($ за тонну)', renderas=Line, pr_linedashed=1, pr_anchorradius=3
				ref m2031191 'Цена никеля на  LME ($ за тонну)', renderas=Line, pr_linedashed=1, pr_linedashlen=5, pr_linedashgap=15,
#				ref m205761 'Цена катодной меди ($ за тонну)'
				ref m205741 'Цена проката медно-никелевого($ за тонну)', renderas=Line, pr_anchorradius=7, pr_anchorsides=3
				ref m2031424 'Цена сырья для медно-никелевого проката($ за тонну)', renderas=Areа, pr_anchorradius=5, pr_anchorsides=5
				
		slide 'Цены по видам проката', targetgroup=budget
			colcategory=1, uselines=1 pr_typecol=month_b1 
			pr_colors='76E3D6,A091DB,DEC9A6,F2A5AA'
			slot 'Медный прокат', objset="468,415,497,1046"
				pr_ylimits='250000,320000'
				ref m205710 'Цена'
			slot 'Латунный прокат', objset="468,415,497,1046"
				pr_ylimits='180000,300000'
				ref m205720 'Цена'
			slot 'Бронзовый прокат',  objset="468,415,497,1046"
				pr_ylimits='280000,650000'
				ref m205730 'Цена'
			slot 'Медно-никелевый прокат', objset="468,415,497,1046"
				pr_ylimits='300000,450000'
				ref m205740 'Цена' 
				
	
	
		slide 'Объемы покупки сырья', role="MTR_" ,  targetgroup=budget
			pr_typecol=month_b1
			pr_colreplace="Б1=SUMMAFACTCALC;PLAN=SSSUMMAPLANSNG;PLANDIV4=SSSUMMAPLANSNG;OZHIDPREDGODDIV4=SUMMAFACTCALC"
			slot 'Котировка CU на LME, цена покупки катодов медных в долларах',  pr_dtype=cureal
				pr_colors=FF8800
				pr_ylimits='300,4500'
				ref z4201364 'Катоды медные - цена ($ за тонну)'
			slot 'Котировка CU на LME, средняя цена покупки лома меди в долларах', pr_dtype=cureal
				pr_colors=FF8800
				pr_ylimits='300,4500'
				ref z4201320 'Лом меди - цена ($ за тонну)'
			slot 'Котировка CU на LME, средняя цена покупки лома латуни в долларах',  pr_dtype=cureal
				pr_colors=FF8800
				pr_ylimits='300,4500'
				ref z4201310 'Лом латуни - цена ($ за тонну)'
			slot 'Котировка ZN на LME, цена реализации цинка в цинковом концентрате в долларах', pr_dtype=znreal
				pr_colors=BFBFBB
				pr_ylimits='100,1000|10000,45000'
				ref z4201370 'Цинк чушковый цена ($ за тонну)'
	
	block 'Поставка сырья', role="OBPROIZ_,MTR_,COST_", targetgroup="div_OCM", process=budget, contents=true , pr_hide=true 	
		slide 'Структрура сырья по видам', noheader=1, pr_measure="    ", slideformat=twobigslots,
			slot 'Структура покупного сырья в нат.выражении группы УГМЦ-ОЦМ ожидаемая за 2012 г., %', role="DR_"
#				pr_dtype=Pie,
				pr_dtype=MSStackedColumn			
				#pr_typecol=bdg_zatr
				pr_sum = "1046,468,497,415,1066" 
				pr_pieheight=50, pr_labelformat='%'
#				row 'Лом и отходы', formula='$m15023000@PDKOL.conto("BA010_91,BA003")?' 
				row 'Лом меди', formula='$m15023200@PDKOL.conto("BA010_91,BA003")?' rowobjset = "468,497,415"
				row 'Лом латуни',formula='$m15023110@PDKOL.conto("BA010_91,BA003")?' rowobjset = "468,497,415"
				row 'Лом бронзы',formula='$m15023300@PDKOL.conto("BA010_91,BA003")?' rowobjset = "468,497,415"
				row 'Катоды медные', formula='$m15031600@PDKOL.conto("BA010_91,BA003")?'
				row 'Катоды медные по ТУ', formula='$m15031650@PDKOL.conto("BA010_91,BA003")?'
				row 'Катанка медная', formula='$m15041110@PDKOL.conto("BA010_91,BA003")?' rowobjset = "1066"
				row 'Цинк чушковый', formula='$m15033111@PDKOL.conto("BA010_91,BA003")?'
				row 'Никель', formula='$m15032805@PDKOL.conto("BA010_91,BA003")?'	
				row 'Прочие металлы', formula='$m15032801@PDKOL.conto("BA010_91")? + $m15032802@PDKOL.conto("BA010_91")? + $m15032803@PDKOL.conto("BA010_91")? + $m15032804@PDKOL.conto("BA010_91")? + $m15032806@PDKOL.conto("BA010_91")?'	
			slot 'Структура покупного сырья в нат.выражении группы УГМЦ-ОЦМ ТПФП 2013 г., %', role="DR_"
				#pr_dtype=Pie,
				#pr_typecol=Fact_tek
				pr_dtype=MSStackedColumn
				pr_pieheight=50, pr_labelformat='%'
				row 'Лом и отходы', formula='$m15023000@PDKOL.conto("BA010_91")?'
#				row 'Лом меди', formula='$m15023200@PDKOL.conto("BA010_91")?'
#				row 'Лом латуни',formula='$m15023110@PDKOL.conto("BA010_91")?'
#				row 'Лом бронзы', formula='$m15023300@PDKOL.conto("BA010_91,BA003")?'
				row 'Катоды медные', formula='$m15031600@PDKOL.conto("BA010_91,BA003")?'
				row 'Катоды медные по ТУ', formula='$m15031650@PDKOL.conto("BA010_91,BA003")?'
				row 'Катанка медная', formula='$m15041110@PDKOL.conto("BA010_91,BA003")?'
				row 'Цинк чушковый', formula='$m15033111@PDKOL.conto("BA010_91,BA003")?'
				row 'Никель', formula='$m15032805@PDKOL.conto("BA010_91,BA003")?'	
				row 'Прочие металлы', formula='$m15032801@PDKOL.conto("BA010_91")? + $m15032802@PDKOL.conto("BA010_91")? + $m15032803@PDKOL.conto("BA010_91")? + $m15032804@PDKOL.conto("BA010_91")? + $m15032806@PDKOL.conto("BA010_91")?'
		slide 'Факторный анализ сырья', noheader=1, pr_measure="    ", pr_sum=div_OCM #,  pr_hide=true 		
			slot 'Влияние цен и потребления сырья на себестоимость', slotview=table_slot, , pr_typecol=fa_pdsumma
				pr_rowheight=23, pr_colwidth="none,0,85px", pr_measure="тыс.руб."
				pr_tablestyle="font-size~125%;"	
				ref m15023000 'Лом и отходы'
#				ref m15023200 'Лом меди'
#				ref m15023110 'Лом латуни'
#				ref m15023300 'Лом бронзы'
				ref m15031600 'Катоды медные'
				ref m15031650 'Катоды медные по ТУ'
				ref m15041110 'Катанка медная'
				ref m15033111 'Цинк чушковый'
				ref m15032805 'Никель'
				row 'Прочие металлы', formula='$m15032801.conto("BA010_91")? + $m15032802.conto("BA010_91")? + $m15032803.conto("BA010_91")? + $m15032804.conto("BA010_91")? + $m15032806.conto("BA010_91")?'


	
	block 'Производство', role="OBPROIZ_,MTR_,COST_", targetgroup="div_OCM", process=top_management, contents=true
		slide 'Структура сырья и объемы производства проката', slideformat=twobigslots, pr_measure="   "
			pr_typecol=AHD, pr_dtype=MSStackedColumn, pr_sum=grp_PRD_OCM
			slot 'Структура покупного сырья,т',  pr_numberscalevalue='10,10,10', # pr_hide=true 
				ref m2061610 'Лом меди'
				ref m2061620 'Лом латуни'
				row 'Катоды медные', level=2, formula='$m15031600@PDKOL.conto("BA010_91, BA003")? + $m15031650@PDKOL.conto("BA010_91, BA003")?'
				ref m2061720 'Цинк чушковый'
				ref m2061730 'Никель'
				ref m2061740 'Катанка медная'	

			
			slot 'Структура покупного сырья,т',  pr_numberscalevalue='10,10,10' , pr_hide=true 
				row 'Лом меди', level=2, formula='$m15023200@RDKOL.conto("BA010_91, BA003")?'
				row 'Лом латуни',level=2, formula='$m15023110@RDKOL.conto("BA010_91, BA003")?'
				row 'Лом бронзы',level=2, formula='$m15023300@RDKOL.conto("BA010_91, BA003")?'
				row 'Катоды медные', level=2, formula='$m15031600@RDKOL.conto("BA010_91, BA003")?'
				row 'Катоды медные по ТУ', level=2, formula='$m15031650@RDKOL.conto("BA010_91, BA003")?'
				row 'Катанка медная', level=2, formula='$m1504110@RDKOL.conto("BA010_91, BA003")?'
				row 'Цинк чушковый', level=2, formula='$m15033111@RDKOL.conto("BA010_91, BA003")?'
				row 'Никель', level=2, formula='$m15032805@RDKOL.conto("BA010_91, BA003")?'	
				row 'Прочие металлы', level=2, formula='$m15032801@RDKOL.conto("BA010_91")? + $m15032802@RDKOL.conto("BA010_91")? + $m15032803@RDKOL.conto("BA010_91")? + $m15032804@RDKOL.conto("BA010_91")? + $m15032806@RDKOL.conto("BA010_91")?'					
			slot 'Выполнение объемов производства по структуре проката,т'
				pr_dtype=MSStackedColumn			
				ref m140852 'Прокат медный'
				ref m140853 'Прокат латунный'
				ref m140854 'Прокат бронзовый'
				ref m140855 'Прокат медно-никелевый'
			
		

	
				
	block 'Показатели ФХД группы УГМК-ОЦМ', role="FIN_", targetgroup="div_OCM", process=top_management, contents=true
		pr_sum=div_OCM
		qxi::include template_budget.bxl : " //*[@key='Показатели ФХД предприятия'] "
		slide 'Прибыль от продаж, чистая прибыль, EBITDA',  noheader=1, slideformat=rbigtwogorslots
			pr_typecol=AHD,  
			slot 'Динамика выручки от продаж и прибыли от продаж без внутреннего оборота группы УГМК-ОЦМ', 
				pr_measure=млн.руб., pr_numberscalevalue="10,10,10", pr_decimals = 1
				pr_ylimits='3000,18000' 
				colcategory=1, usearea=1 
				pr_yname="Выручка"
				row 'Выручка от продаж продукции(товаров, работ, услуг)' formula=" $m15041000@SLREV.toobj(1046)? + $m15041000@SLREV.toobj(1066)? "
				ref m200080, 'Прибыль от продаж'
			slot 'Прибыль от продаж, чистая прибыль, EBITDA ', slotview=table_slot, pr_measure="тыс.руб."
				pr_rowheight=25, pr_colwidth="none,0,90px"
				pr_tablestyle="font-size~100%;"
				ref m1122200, 'Прибыль от продаж'
				ref m1122300, 'Прибыль до налоообложения'
				ref m1122330, 'Проценты к уплате (+)'
				ref m1122320, 'Проценты к получению (-)'
				ref m260400, 'Амортизация (+)'
				ref m212240, 'Результат по курсовой разнице (-)'
				ref m200420, 'EBITDA'
				ref m200200, 'Чистая прибыль'
			slot 'Прибыль от продаж, чистая прибыль, EBITDA'
				pr_measure=млн.руб., pr_numberscalevalue="10,10,10",  pr_decimals = 1
				colcategory=1, uselines=1, pr_anchorradius=5, pr_linedashlen=8, pr_linedashgap=6 pr_anchorbordercolor=669ACD
				ref m200080, 'Прибыль от продаж' , pr_anchorsides=3, pr_anchorradius=8,  
				ref m200200, 'Чистая прибыль' , pr_anchorsides=3, pr_anchorradius=8 ,  pr_anchorbgcolor=053666 
				ref m200420, 'EBITDA', pr_anchorsides=4, pr_anchorradius=6,  pr_linedashed=1
	
		slide 'Прибыль от продаж и рентабельность продаж группы УГМК-ОЦМ',  role="SALE_", slideformat=twobigslots, process=budjet
			slot 'Прибыль от продаж готовой продукции в текущем году'
				pr_dtype=MSStackedColumn, pr_typecol=month_b1
				ref m205510 'Медный прокат'
				ref m205520 'Латунный прокат'
				ref m205530 'Бронзовый прокат'
				ref m205540 'Медно-никелевый прокат'
			slot 'Рентабельность продаж'
				colcategory=1, uselines=1, pr_typecol=month_b1, numberformat="#,#.##", pr_decimals = 2	
				ref m205610 'Медный прокат'
				ref m205620 'Латунный прокат'
				ref m205630 'Бронзовый прокат'
				ref m205640 'Медно-никелевый прокат'
				
		slide 'Прибыль от продаж готовой продукции' , role="SALE_", slideformat=onebigslot, pr_sum=div_OCM
			slot 'Структура прибыли от продаж по видам проката' pr_measure="млн.руб."
	#			pr_dtype=MSStackedColumn,
				pr_dtype=MSStackedBar,
				pr_numberscalevalue="10,10,10", pr_decimals = 1, pr_zeroline=1
			    pr_colors='B84A1F,D6C445,8F8381,B3A291'
				row 'Медный прокат' level=2 formula='$m15041130@SLPRIB.conto("BA090_01,BA090_02,BA090_08")?'	
				row 'Латунный прокат' level=2 formula='$m15041200@SLPRIB.conto("BA090_01,BA090_02,BA090_08")?', 
				row 'Бронзовый прокат' formula='$m15041300@SLPRIB.conto("BA090_01,BA090_02,BA090_08")? '	
				row 'Медно-никелевый прокат' formula='$m15041400@SLPRIB.conto("BA090_01,BA090_02,BA090_08")? '
				
			
		slide 'Влияние цен и себестоимости на прибыль от продаж' , role="SALE_", slideformat=onebigslot, process=budjet		
			slot 'Влияние цен и себестоимости на прибыль от продаж', slotview=table_slot, pr_typecol=fa_prib		
				pr_rowheight=23, pr_colwidth="none,0,85px", pr_measure="тыс.руб."
				pr_tablestyle="font-size~125%;"	
				ref m15041130 'Медный прокат'
				ref m15041200 'Латунный прокат'
				ref m15041300 'Бронзовый прокат'
				ref m15041400 'Медно-никелевый прокат'
			
		slide 'Прибыль от продаж и рентабельность продаж группы УГМК-ОЦМ',  role="SALE_", slideformat=twobigslots, process=budjet
			slot 'Прибыль от продаж готовой продукции в текущем году'
				pr_dtype=MSStackedColumn, pr_typecol=month_b1
				ref m205510 'Медный прокат'
				ref m205520 'Латунный прокат'
				ref m205530 'Бронзовый прокат'
				ref m205540 'Медно-никелевый прокат'
			slot 'Рентабельность продаж'
				colcategory=1, uselines=1, pr_typecol=month_b1, numberformat="#,#.##", pr_decimals = 2	
				ref m205610 'Медный прокат'
				ref m205620 'Латунный прокат'
				ref m205630 'Бронзовый прокат'
				ref m205640 'Медно-никелевый прокат'		
		slide 'Прибыль от продаж производства',  role="SALE_", slideformat=twobigslots,  process=budjet
			slot 'Прибыль от продаж готовой продукции в текущем году'
				pr_dtype=MSStackedColumn, pr_sum=grp_PRD_OCM
				ppr_typecol=AHD
				ref m205510 'Медный прокат'
				ref m205520 'Латунный прокат'
				ref m205530 'Бронзовый прокат'
				ref m205540 'Медно-никелевый прокат'
			slot 'Рентабельность продаж'
				colcategory=1, uselines=1, pr_sum=grp_PRD_OCM
				pr_typecol=AHD, numberformat="#,#.##", pr_decimals = 2, 	
				ref m205610 'Медный прокат'
				ref m205620 'Латунный прокат'
				ref m205630 'Бронзовый прокат'
				ref m205640 'Медно-никелевый прокат'

				
		slide 'Прибыль от продаж по видам проката',  role="SALE_" ,  process=budjet
			pr_sum=div_OCM, pr_typecol=AHD
			pr_dtype=MSStackedColumn, usecombine=1, colcategory=1, prg_showsum=0
			pr_pyname="Прибыль от продаж", pr_syname="Рентабельность"			
			slot 'Прибыль от продаж и рентабельность медного проката'
				ref m205610 'Рентабельность продаж'
				ref m205510 'Прибыль от продаж '
			slot 'Прибыль от продаж и рентабельность латунного проката'
				ref m205620 'Рентабельность продаж'		
				ref m205520 'Прибыль от продаж '				
			slot 'Прибыль от продаж и рентабельность бронзового проката'
				ref m205630 'Рентабельность продаж'	
				ref m205530 'Прибыль от продаж '				
			slot 'Прибыль от продаж и рентабельность медно-никелевого проката'
				ref m205640 'Рентабельность продаж'
				ref m205540 'Прибыль от продаж '				
		
				
		slide 'Прибыль от продаж производства',  role="SALE_", process=budjet
			objset="468,415,497,1066,1046"	
			pr_typecol=month_b1		
			slot 'Прибыль от продаж готовой продукции в текущем году'
				pr_dtype=MSStackedColumn
				ref m205510 'Медный прокат'
			slot 'Прибыль от продаж готовой продукции в текущем году'
				pr_dtype=MSStackedColumn
				ref m205520 'Латунный прокат'
			slot 'Прибыль от продаж готовой продукции в текущем году'
				pr_dtype=MSStackedColumn
				ref m205530 'Бронзовый прокат'
			slot 'Прибыль от продаж готовой продукции в текущем году'
				pr_dtype=MSStackedColumn
				ref m205540 'Медно-никелевый прокат'
		

		slide 'Структура прибыли от продаж по видам проката в сегменте предприятий',  role="SALE_" , process=budjet
			pr_dtype=MSStackedColumn,  pr_typecol=month_b1, objset="468,415,497,1066"
			pr_colors='76E3D6,A091DB,DEC9A6,F2A5AA'
			slot 'Прокат медный'
				ref m205510 'Прокат медный'
			slot 'Прокат латунный'
				ref m205520 'Прокат латунный'
			slot 'Прокат бронзовый'
				ref m205530 'Прокат бронзовый'
			slot 'Прокат медно-никелевый'
				ref m205540 'Прокат медно-никелевый'
				
	block 'Продажи', role="SALE_", targetgroup="div_OCM", process=top_management, contents=true
		slide 'Динамика цен и себестоимость реализации по видам проката УГМК-ОЦМ', slideformat=twobigslots 
			pr_measure="руб.~т" , pr_sum=grp_PRD_OCM, pr_typecol=AHD
			colcategory=1, uselines=1,  #pr_colors='0D9E0F,FF8800,09B3D9'
			pr_anchorradius=5, pr_linedashlen=8, pr_linedashgap=6	
			pr_anchorbordercolor=669ACD,  pr_anchorbgcolor=053666 			
			slot 'Цена и себестоимость реализации медного проката'
				pr_ylimits='260000,300000'
				ref m205710 'цена', pr_anchorradius=8, pr_anchorsides=3
				ref	m205910 'полная СС', pr_anchorradius=5, pr_anchorsides=5
				ref	m205810 'цеховая СС', pr_anchorradius=5,  pr_linedashed=1
#				ref	m2059511 'прямые затраты'				
			slot 'Цена и себестоимость реализации латунного проката'
				pr_ylimits='190000,230000'
				ref m205720 'цена', pr_anchorradius=8, pr_anchorsides=3
				ref	m205920 'полная СС', pr_anchorradius=5, pr_anchorsides=5
				ref	m205820 'цеховая СС' pr_anchorradius=5,  pr_linedashed=1
#				ref	m2059521 'прямые затраты'					
		slide 'Динамика цен и себестоимость реализации по видам проката УГМК-ОЦМ', slideformat=twobigslots 
			pr_measure="руб.~т" ,  pr_sum=grp_PRD_OCM
			colcategory=1, uselines=1, #pr_colors='0D9E0F,FF8800,09B3D9'
			pr_anchorradius=5, pr_linedashlen=8, pr_linedashgap=6	
			pr_ylimitsplus=5	
			pr_anchorbordercolor=669ACD,  pr_anchorbgcolor=053666 			
			slot 'Цена и себестоимость реализации бронзового проката'
#				pr_ylimits='230000,350000'
				pr_ylimits='270000,320000'
				ref m205730 'цена', pr_anchorradius=8, pr_anchorsides=3
				ref	m205930 'полная СС', pr_anchorradius=5, pr_anchorsides=5
				ref	m205830 'цеховая СС' pr_anchorradius=5,  pr_linedashed=1
#				ref	m2059531 'прямые затраты'	
			slot 'Цена и себестоимость реализации медно-никелевого проката'
#				pr_ylimits='250000,420000'
				pr_ylimits='310000,360000'
				ref m205740 'цена', pr_anchorradius=8, pr_anchorsides=3
				ref	m205940 'полная СС', pr_anchorradius=5, pr_anchorsides=5
				ref	m205840 'цеховая СС' pr_anchorradius=5,  pr_linedashed=1
#				ref	m2059541 'прямые затраты'	
				
	block 'Себестоимость продукции. Группа УГМК-ОЦМ' , role="COST_",  targetgroup="div_OCM", process=budjet, contents=true
		pr_anchorbordercolor=669ACD,  pr_anchorbgcolor=053666 
		slide 'ЦЕХОВАЯ СЕБЕСТОИМОСТЬ единицы по видам проката 1 ', role="COST_",  prg_showsum=0  
			pr_dtype=MSStackedColumn, pr_typecol=month_b1, pr_measure="$~за тонну"
			pr_colors='76E3D6,A091DB,DEC9A6,F2A5AA'
			slot 'цеховая себестоимость медного проката по предприятиям', objset="1066,468,415,497,"
				ref m205810 'цеховая себестоимость'
			slot 'цеховая себестоимость латунного проката по предприятиям', objset="497,468,415"
				ref m205820 'цеховая себестоимость'
			slot 'цеховая себестоимость бронзового проката по предприятиям',  objset="497,468"
				ref m205830 'цеховая себестоимость'
			slot 'цеховая себестоимость медно-никелевого проката по предприятиям',  objset="497,468,415"
				ref m205840 'цеховая себестоимость'

		slide 'ЦЕХОВАЯ СЕБЕСТОИМОСТЬ единицы по видам проката  ', slideformat=twobigslots,  role="COST_",  prg_showsum=0  
			colcategory=1, usearea=1, pr_typecol=month_b1, pr_colors='76E3D6,A091DB,DEC9A6,F2A5AA'
			slot 'цеховая себестоимость медного проката по предприятиям', objset="468,415,497,1066"
				pr_ylimits='200000,350000'				
				ref m205810 'цеховая себестоимость'
			slot 'цеховая себестоимость латунного проката по предприятиям', objset="468,415,497"
				pr_ylimits='150000,280000'		
				ref m205820 'цеховая себестоимость'
				
		slide 'ЦЕХОВАЯ СЕБЕСТОИМОСТЬ единицы по видам проката ', role="COST_", slideformat=twobigslots, prg_showsum=0  
			colcategory=1, usearea=1, pr_typecol=month_b1, pr_colors='76E3D6,A091DB,DEC9A6,F2A5AA'
			slot 'цеховая себестоимость бронзового проката по предприятиям', objset="468,497"
				pr_ylimits='250000,400000'	
				ref m205830 'цеховая себестоимость'
			slot 'цеховая себестоимость медно-никелевого проката по предприятиям', objset="468,415,497"
				pr_ylimits='250000,400000'	
				ref m205840 'цеховая себестоимость'		
				
					
	block 'Себестоимость продукции. Группа УГМК-ОЦМ' , role="COST_",  targetgroup="div_OCM", process=budjet, contents=true	
		slide 'ЦЕХОВАЯ СЕБЕСТОИМОСТЬ. Расходы передела',  role="COST_", slideformat=twobigslots, #prg_showsum=0  
			pr_dtype=MSStackedColumn, #pr_typecol=month_b1,
			pr_colors='76E3D6,A091DB,DEC9A6,F2A5AA'
			pr_sum=grp_PRD_OCM			
			slot 'Расходы передела плавления (всего)'
				pr_colreplace="Б1=SUMMA;PLANGOD=SUMMAPLANCALC;PLANOPER=SUMMAPLANCALC;PLANDIV4=SUMMAPLANCALCDIV4;OZHIDPREDGODDIV4=SUMMADIV4"
				ref z4202100 'всп.материалы'
				ref z4202200 'топливо'
				ref z4202300 'энергоресурсы'	
				ref z4202400 'расходы по оплате труда'
				ref z4202550 'услуги'	
				ref z4202500 'амортизация'		
				ref z4202700 'общепроизв. расходы'
			slot 'Расходы передела плавления (на единицу)'
				pr_colreplace="Б1=SUMMAEDFACTCALC;PLANGOD=SUMMAEDPLANCALC;PLANOPER=SUMMAEDPLANCALC;PLANKORR=SUMMAEDPLANCALC;PLANDIV4=SUMMAEDPLANCALC;OZHIDPREDGODDIV4=SUMMAEDFACTCALC;OZHIDPREDGOD=SUMMAEDFACTCALC"
				pr_mult=1000
				ref z4202100 'всп.материалы'
				ref z4202200 'топливо'
				ref z4202300 'энергоресурсы'	
				ref z4202400 'расходы по оплате труда'
				ref z4202550 'услуги'	
				ref z4202500 'амортизация'		
				ref z4202700 'общепроизв. расходы'
				
		slide 'ЦЕХОВАЯ СЕБЕСТОИМОСТЬ. Расходы передела',  role="COST_", slideformat=twobigslots
			pr_dtype=MSStackedColumn, #pr_typecol=month_b1,
			pr_colors='76E3D6,A091DB,DEC9A6,F2A5AA'
			pr_sum=grp_PRD_OCM			
			slot 'Расходы передела проката (всего)',
				pr_colreplace="Б1=SUMMA;PLANGOD=SUMMAPLANCALC;PLANOPER=SUMMAPLANCALC;PLANDIV4=SUMMAPLANCALCDIV4;OZHIDPREDGODDIV4=SUMMADIV4"
				ref z4102100 'всп.материалы'
				ref z4102200 'топливо'
				ref z4102300 'энергоресурсы'	
				ref z4102400 'расходы по оплате труда'	
				ref z4102500 'услуги'	
				ref z4102600 'амортизация'	
				ref z4102700 'общепроизв. расходы'		
			slot 'Расходы передела проката (на единицу)',
				pr_colreplace="Б1=SUMMAEDFACTCALC;PLANGOD=SUMMAEDPLANCALC;PLANOPER=SUMMAEDPLANCALC;PLANKORR=SUMMAEDPLANCALC;PLANDIV4=SUMMAEDPLANCALC;OZHIDPREDGODDIV4=SUMMAEDFACTCALC;OZHIDPREDGOD=SUMMAEDFACTCALC"
				pr_mult=1000
				ref z4102100 'всп.материалы'
				ref z4102200 'топливо'
				ref z4102300 'энергоресурсы'	
				ref z4102400 'расходы по оплате труда'	
				ref z4102500 'услуги'	
				ref z4102600 'амортизация'	
				ref z4102700 'общепроизв. расходы'	
				
			
		slide 'ЦЕХОВАЯ СЕБЕСТОИМОСТЬ. Общепроизводственные расходы',  role="COST_", slideformat=twobigslots
			pr_dtype=MSStackedColumn, pr_typecol=month_b1, pr_colors='76E3D6,A091DB,DEC9A6,F2A5AA'
			slot 'Общепроизводственные расходы по видам проката',
				pr_sum=grp_PRD_OCM	
				pr_colreplace="Б1=SUMMA;PLAN=SUMMAPLANCALC;PLANDIV4=SUMMAPLANCALCDIV4;OZHIDPREDGODDIV4=SUMMADIV4"
				ref z41010510 'прокат медный'
				ref z41010520 'топливо'
				ref z41010530 'энергоресурсы'	
				ref z41010540 'расходы по оплате труда'		
			slot 'Общепроизводственные расходы по предприятиям'
				objset="1066,468,415,497"	
				pr_colreplace="Б1=SUMMA;PLAN=SUMMAPLANCALC;PLANDIV4=SUMMAPLANCALCDIV4;OZHIDPREDGODDIV4=SUMMADIV4"
				ref z41010500 'всп.материалы'
				
		slide 'ЦЕХОВАЯ СЕБЕСТОИМОСТЬ. Расходы передела',  role="COST_", slideformat=twobigslots, targetgroup=budget 
			pr_dtype=MSStackedColumn, pr_typecol=month_b1, objset="1066,468,415,497"		
			slot 'Расходы передела плавления (всего)'
				pr_colreplace="Б1=SUMMA;PLAN=SUMMAPLANCALC;PLANDIV4=SUMMAPLANCALCDIV4;OZHIDPREDGODDIV4=SUMMADIV4"
				ref z4202000 'всп.материалы'
			slot 'Расходы передела плавления (на единицу)'
				pr_colreplace="Б1=SUMMAEDFACTCALC;PLAN=SUMMAEDPLANCALC;PLANDIV4=SUMMAEDPLANCALC;OZHIDPREDGODDIV4=SUMMAEDFACTCALC"
				pr_mult=1000
				ref z4202000 'всп.материалы'
				
		slide 'ЦЕХОВАЯ СЕБЕСТОИМОСТЬ. Расходы передела',  role="COST_", slideformat=twobigslots, targetgroup=budget	
			pr_dtype=MSStackedColumn, pr_typecol=month_b1, pr_colors='76E3D6,A091DB,DEC9A6,F2A5AA'
			objset="1066,468,415,497"		
			slot 'Расходы передела проката (всего)',
				pr_colreplace="Б1=SUMMA;PLAN=SUMMAPLANCALC;PLANDIV4=SUMMAPLANCALCDIV4;OZHIDPREDGODDIV4=SUMMADIV4"
				ref z4102000 'расходы передела'
			slot 'Расходы передела проката (на единицу)',
				pr_colreplace="Б1=SUMMAEDFACTCALC;PLAN=SUMMAEDPLANCALC;PLANDIV4=SUMMAEDPLANCALC;OZHIDPREDGODDIV4=SUMMAEDFACTCALC"
				pr_mult=1000
				ref z4102000 'расходы передела'
				

		slide 'Отклонение', noheader=1, pr_measure="    ", slideformat=twobigslots, , targetgroup=budget
			slot 'Структура отклонений по видам сырья', role="DR"
				pr_typecol="AHD_OtklPlan", pr_dtype=Radar, pr_sum=div_OCM,
				pr_colreplace="OTKLPLAN=OTKLPLANCALC"
				ref z4201310 'Лом латуни'
				ref z4201320 'Лом меди'
				ref z4201364 'Катоды медные'
				ref z4201370 'Цинк чушковый'
				ref z4201375 'Катанка медная'
				ref z4201367 'Никель'
				ref z42010100 'Прочее'				
			slot 'Структура отклонений затрат ', role="DR"
				pr_typecol="AHD_OtklPlan", pr_dtype=Radar, pr_sum=div_OCM,
				ref m260831, 'Сырье'				
				ref m260832, 'Прочие ТМЦ'
				ref m260170, 'Топливо'
				ref m260181, 'Электроэнергия'
				ref m260182, 'Теплоэнергия'
				ref m260834, 'Работы и услуги произв. характера'
				ref m260835, 'Расходы по оплате труда'
				ref m260836, 'Амортизация'
				ref m260837, 'Налоги'
				ref m260838, 'Прочие расходы'

		slide 'Отклонение в сегменте предприятий', noheader=1, targetgroup="div_OCM", process=budjet, contents=true
			pr_measure="    ", slideformat=onebigslot, 		
			slot 'Структура отклонений затрат (без учета сырья)', role="DR", pr_typecol="AHD_OtklPlan"
				pr_dtype=Pie, objset="468,415,497,1066,1046" 
				pr_colors='90E7E8,BFBFBB,F7F711,D9987E,C0E890'			
				ref m260830, 'Всего затрат'
		slide 'Отклонение в сегменте предприятий', noheader=1, pr_measure="    ", slideformat=twobigslots, process=budjet,
			slot 'Структура отклонений затрат (без учета сырья)', role="DR", pr_typecol="AHD_OtklPlan"
				pr_dtype=MSStackedColumn, objset="468,415,497,1066,1046" 
				pr_colors='90E7E8,BFBFBB,F7F711,D9987E,C0E890'			
				ref m260830, 'Всего затрат'		
			slot 'Структура отклонений затрат (без учета сырья)', role="DR", pr_typecol="AHD_OtklPlan"
				pr_dtype=MSStackedColumn, pr_sum=div_OCM 
				pr_colors='90E7E8,BFBFBB,F7F711,D9987E,C0E890'	
				ref m260831, 'Сырье'				
				ref m260832, 'Прочие ТМЦ'
				ref m260170, 'Топливо'
				ref m260181, 'Электроэнергия'
				ref m260182, 'Теплоэнергия'
				ref m260834, 'Работы и услуги произв. характера'
				ref m260835, 'Расходы по оплате труда'
				ref m260836, 'Амортизация'
				ref m260837, 'Налоги'
				ref m260838, 'Прочие расходы'	
		slide 'Отклонение в сегменте предприятий', noheader=1, pr_measure="    ", process=budjet,
			slot 'Структура отклонений затрат (без учета сырья)', role="DR", pr_typecol="AHD_OtklPlan"
				pr_dtype=MSStackedColumn, pr_sum=obj_468 
				pr_colors='90E7E8,BFBFBB,F7F711,D9987E,C0E890'	
				ref m260831, 'Сырье'				
				ref m260832, 'Прочие ТМЦ'
				ref m260170, 'Топливо'
				ref m260181, 'Электроэнергия'
				ref m260182, 'Теплоэнергия'
				ref m260834, 'Работы и услуги произв. характера'
				ref m260835, 'Расходы по оплате труда'
				ref m260836, 'Амортизация'
				ref m260837, 'Налоги'
				ref m260838, 'Прочие расходы'	
			slot 'Структура отклонений затрат (без учета сырья)', role="DR", pr_typecol="AHD_OtklPlan"
				pr_dtype=MSStackedColumn, pr_sum=obj_415 
				pr_colors='90E7E8,BFBFBB,F7F711,D9987E,C0E890'	
				ref m260831, 'Сырье'				
				ref m260832, 'Прочие ТМЦ'
				ref m260170, 'Топливо'
				ref m260181, 'Электроэнергия'
				ref m260182, 'Теплоэнергия'
				ref m260834, 'Работы и услуги произв. характера'
				ref m260835, 'Расходы по оплате труда'
				ref m260836, 'Амортизация'
				ref m260837, 'Налоги'
				ref m260838, 'Прочие расходы'	
			slot 'Структура отклонений затрат (без учета сырья)', role="DR", pr_typecol="AHD_OtklPlan"
				pr_dtype=MSStackedColumn, pr_sum=obj_497
				pr_colors='90E7E8,BFBFBB,F7F711,D9987E,C0E890'
				ref m260831, 'Сырье'				
				ref m260832, 'Прочие ТМЦ'
				ref m260170, 'Топливо'
				ref m260181, 'Электроэнергия'
				ref m260182, 'Теплоэнергия'
				ref m260834, 'Работы и услуги произв. характера'
				ref m260835, 'Расходы по оплате труда'
				ref m260836, 'Амортизация'
				ref m260837, 'Налоги'
				ref m260838, 'Прочие расходы'	
			slot 'Структура отклонений затрат (без учета сырья)', role="DR", pr_typecol="AHD_OtklPlan"
				pr_dtype=MSStackedColumn, pr_sum=obj_1066
				pr_colors='90E7E8,BFBFBB,F7F711,D9987E,C0E890'
				ref m260831, 'Сырье'				
				ref m260832, 'Прочие ТМЦ'
				ref m260170, 'Топливо'
				ref m260181, 'Электроэнергия'
				ref m260182, 'Теплоэнергия'
				ref m260834, 'Работы и услуги произв. характера'
				ref m260835, 'Расходы по оплате труда'
				ref m260836, 'Амортизация'
				ref m260837, 'Налоги'
				ref m260838, 'Прочие расходы'	

	block 'Затраты на производство и реализацию продукции' role="DR_,ENERGO_", targetgroup="div_OCM", process=top_management, contents=true
		slide 'Затраты на производство и реализацию продукции', pr_sum=div_OCM :
			qxi::safeimport template_budget.bxl : " //*[@key='Затраты по элементам расходов_div'] "
			
		slide 'Структрура сырья по видам', noheader=1, pr_measure="    ", slideformat=twobigslots, pr_sum=grp_PRD_OCM
			slot 'Структура покупного сырья группы УГМЦ-ОЦМ ТПФП 2013 г. , % по сумме', role="DR_"
				pr_colreplace="Б1=SUMMA;PLANGOD=SUMMAPLANCALC;PLANOPER=SUMMAPLANCALC;PLANDIV4=SUMMAPLANCALC;OZHIDPREDGODDIV4=SUMMA;OZHIDPREDGOD=SUMMA"
				pr_dtype=Pie, pr_typecol=bdg_zatr 
				pr_pieheight=50, pr_labelformat='%'
				ref z4201310 'Лом латуни'
				ref z4201320 'Лом меди'
				ref z4201364 'Катоды медные'
				ref z4201370 'Цинк чушковый'
				ref z4201367 'Никель'
				ref z4201375 'Катанка медная'
				ref z42010100 'Прочее'
			slot 'Структура покупного сырья группы УГМЦ-ОЦМ ожидаемая за 1 полугодие 2013 г., % по сумме', role="DR_"
				pr_colreplace="Б1=SUMMA;PLANGOD=SUMMAPLANCALC;PLANOPER=SUMMAPLANCALC;PLANDIV4=SUMMAPLANCALC;OZHIDPREDGODDIV4=SUMMA;OZHIDPREDGOD=SUMMA"
				pr_dtype=Pie, pr_typecol=Fact_tek
				pr_pieheight=50, pr_labelformat='%'
				ref z4201310 'Лом латуни'
				ref z4201320 'Лом меди'
				ref z4201364 'Катоды медные'
				ref z4201370 'Цинк чушковый'
				ref z4201367 'Никель'
				ref z4201375 'Катанка медная'
				ref z42010100 'Прочее'
				
		slide 'Структрура сырья по видам', noheader=1, pr_measure="    ", slideformat=twobigslots, pr_sum=grp_PRD_OCM
			slot 'Структура покупного сырья группы УГМЦ-ОЦМ  ТПФП 2013 г., % по количеству', role="DR_"
				pr_colreplace="Б1=KOL;PLAN=KOLCALCPLAN;PLANGOD=KOLCALCPLAN;PLANOPER=KOLCALCPLAN;PLANDIV4=KOLCALCPLAN;OZHIDPREDGODDIV4=KOL;OZHIDPREDGOD=KOL"
				pr_dtype=Pie, pr_typecol=bdg_zatr 
				pr_pieheight=50, pr_labelformat='%'
				ref z4201310 'Лом латуни'
				ref z4201320 'Лом меди'
				ref z4201364 'Катоды медные'
				ref z4201370 'Цинк чушковый'
				ref z4201375 'Катанка медная'
				ref z4201367 'Никель'
				ref z42010100 'Прочее'
			slot 'Структура покупного сырья группы УГМЦ-ОЦМ  ожидаемая за 1 полугодие 2013 г., % по количеству', role="DR_"
				pr_colreplace="Б1=KOL;PLAN=KOLCALCPLAN;PLANGOD=KOLCALCPLAN;PLANOPER=KOLCALCPLAN;PLANDIV4=KOLCALCPLAN;OZHIDPREDGODDIV4=KOL;OZHIDPREDGOD=KOL"
				pr_dtype=Pie, pr_typecol=Fact_tek
				pr_pieheight=50, pr_labelformat='%'
				ref z4201310 'Лом латуни'
				ref z4201320 'Лом меди'
				ref z4201364 'Катоды медные'
				ref z4201370 'Цинк чушковый'
				ref z4201375 'Катанка медная'
				ref z4201367 'Никель'
				ref z42010100 'Прочее'		
				
	block 'Энергетика' role="DR_,ENERGO_",  targetgroup="div_OCM", process=budjet,  contents=true	
		slide 'Энергетика. ОАО"КзОЦМ"',  pr_sum=obj_468 :
			qxi::safeimport template_energy.bxl : " //*[@key='energy_prds'] "
		slide 'Энергетика. ЗАО"КЦМ"' pr_sum=obj_497 :
			qxi::safeimport template_energy.bxl : " //*[@key='energy_prds'] "
		slide 'Энергетика. ОАО"РзОЦМ"' pr_sum=obj_415 :
			qxi::safeimport template_energy.bxl : " //*[@key='energy_prds'] "
		slide 'Энергетика. FBC Майденпек' pr_sum=obj_1066 :
			qxi::safeimport template_energy.bxl : " //*[@key='energy_prds'] "	

	block 'Энергетика' role="DR_,ENERGO_",  targetgroup="div_OCM", process=top_management,  contents=true	
		slide 'Цены и потребление газа природного по предприятиям', slideformat=twobigslots
			colcategory=1, uselines=1,  pr_typecol=month_b1, objset="468,415,497", r_filter=""
			slot 'Динамика объемов потребления газа природного', pr_measure=тыс.м3., pr_numberscalevalue='1,1,1'
				pr_colreplace="Б1=KOL;PLANGOD=KOL;PLANOPER=KOL;PLANKORR=KOL;PLANDIV4=PLANKOLDIV4;OZHIDPREDGODDIV4=OZHIDKOLDIV4;OZHIDPREDGOD=KOL;PLANDIV12=PLANKOLDIV12"
				pr_anchorradius=6, pr_anchorsides=5, #pr_anchorbordercolor=669ACD,  pr_anchorbgcolor=053666 
				ref m260171 			
			slot 'Динамика цен на газ природный', pr_measure="руб.~тыс.м3"
				pr_anchorradius=6, pr_anchorsides=3, #pr_anchorbordercolor=0D6D37,  pr_anchorbgcolor=0D6D37
				pr_ylimits='3000,4500'	
				pr_colreplace="Б1=CENALINK;PLANGOD=CENALINK;PLANOPER=CENALINK;PLANKORR=CENALINK;PLANDIV4=CENALINK;OZHIDPREDGODDIV4=CENALINK;OZHIDPREDGOD=CENALINK;PLANDIV12=CENALINK"
				ref m260171
		slide 'Цены и потребление газа природного по предприятиям',slideformat=twobigslots
			colcategory=1, uselines=1, pr_typecol=month_b1, objset="468,415,497,1066", r_filter=""
			slot 'Динамика объемов потребления электроэнергии', pr_measure=тыс.кВт*ч, pr_numberscalevalue='10,10,10'
				pr_anchorradius=6, pr_anchorsides=5, #pr_anchorbordercolor=669ACD,  pr_anchorbgcolor=053666 
				pr_colreplace="Б1=KOL;PLANGOD=KOL;PLANOPER=KOL;PLANKORR=KOL;PLANDIV4=PLANKOLDIV4;OZHIDPREDGODDIV4=OZHIDKOLDIV4;OZHIDPREDGOD=KOL;PLANDIV12=PLANKOLDIV12"
				ref m260181	
			slot 'Динамика цен на электроэнергию', pr_measure="руб.~кВт*ч" ,	numberformat="#.####",	pr_decimals=2,
				pr_anchorradius=6, pr_anchorsides=3, #pr_anchorbordercolor=0D6D37,  pr_anchorbgcolor=0D6D37
				pr_ylimits='1,3'
				pr_colreplace="Б1=CENALINK;PLANGOD=CENALINK;PLANOPER=CENALINK;PLANKORR=CENALINK;PLANDIV4=CENALINK;OZHIDPREDGODDIV4=CENALINK;OZHIDPREDGOD=CENALINK;PLANDIV12=CENALINK"
				ref m260181	
	
	
	block 'Персонал',role="ZP_",  targetgroup="div_OCM", process="personal", contents=true
		slide 'Персонал. ОАО"КзОЦМ"' pr_sum=obj_468 :
			qxi::safeimport template_budget.bxl : " //*[@key='Персонал'] "
		slide 'Персонал. ЗАО"КЦМ"' pr_sum=obj_497 :
			qxi::safeimport template_budget.bxl : " //*[@key='Персонал'] "
		slide 'Персонал. ОАО"РзОЦМ"' pr_sum=obj_415 :
			qxi::safeimport template_budget.bxl : " //*[@key='Персонал'] "
		slide 'Персонал. FBC Майденпек' pr_sum=obj_1066 :
			qxi::safeimport template_budget.bxl : " //*[@key='Персонал'] "	
			
	block 'Персонал',role="ZP_" ,  targetgroup="div_OCM", process=budjet, contents=true
		slide 'Персонал',  key='Персонал',  
			pr_exclude_period="1"  pr_ylimits='auto'
			slot 'Численность на конец отчетного периода', 
				colcategory=1, uselines=1,  pr_anchorradius=6, pr_anchorsides=5, #pr_measure=чел. 
				objset="468,415,497,1066,1046",
				pr_exclude_period="1,306"
				pr_typecol=AHD_OSN, pr_anchorbordercolor=0D6D37,  pr_anchorbgcolor=0D6D37
				ref t110600,'Численность на конец отчетного периода'
			slot 'Производительность труда'
				objset="468,415,497,1066,1046",
				colcategory=1, uselines=1, pr_anchorradius=7, pr_anchorsides=3, 
				pr_measure="нат.ед.(тыс.руб.)~чел.", usereffilter=1
				ref t210161, 'Прокат цветных металлов (т)'	
			slot 'Динамика удельного веса ФЗП со страховыми взносами  в затратах на производство и реализацию продукции'
				pr_sum=div_OCM
				pr_dtype=MSStackedBar, pr_percents=1, pr_measure=%,
				ref m260839, 'Затраты (без сырья)'
				ref m260835, 'ФЗП со стр.взносами'
			slot 'Cреднемесячная заработная плата', pr_measure="руб."
				colcategory=1, colcategory=1, uselines=1,  pr_anchorradius=6, pr_anchorsides=3,
				pr_anchorbordercolor=0D6D37,  pr_anchorbgcolor=0D6D37
				pr_ylimits='auto'
				objset="468,415,497,1066,1046",
				ref t110400, 'Среднемесячная заработная плата, руб.' 
				
	block 'Персонал',role="ZP_" ,  targetgroup="div_OCM", process=top_management, contents=true
		slide 'Основные показатели по труду ОАО"КзОЦМ", ЗАО"КЦМ", ОАО"РзОЦМ" ',  key='Персонал', 
			pr_exclude_period="1"  pr_ylimits='auto'
			slot 'Численность на конец отчетного периода', 
				colcategory=1, uselines=1,  pr_anchorradius=6, pr_anchorsides=5, pr_measure=чел. 
				objset="468,415,497",	pr_ylimits='900,1200'
				pr_exclude_period="1,306"
				pr_typecol=AHD_OSN, pr_anchorbordercolor=0D6D37,  pr_anchorbgcolor=0D6D37
				ref t110600,'Численность на конец отчетного периода' 
			slot 'Производительность труда'
				numberformat="#,#.#" pr_decimals = 1
				objset="468,415,497", pr_ylimits='5,35'
				colcategory=1, uselines=1, pr_anchorradius=7, pr_anchorsides=3, 
				pr_measure="нат.ед.~чел.", usereffilter=1
				ref t210161, 'Прокат цветных металлов (т)'	
			slot 'Динамика удельного веса ФЗП со страховыми взносами  в затратах на производство и реализацию продукции'
				pr_sum=grp_PRD_OCM
				pr_dtype=MSStackedBar, pr_percents=1, pr_measure=%,
				ref m260839, 'Затраты (без сырья)'
				ref m260835, 'ФЗП со стр.взносами'
			slot 'Cреднемесячная заработная плата', pr_measure="руб."
				colcategory=1, colcategory=1, uselines=1,  pr_anchorradius=6, pr_anchorsides=3,
				pr_anchorbordercolor=0D6D37,  pr_anchorbgcolor=0D6D37
				pr_ylimits='17000,25000'
				objset="468,415,497",
				ref t110400, 'Среднемесячная заработная плата, руб.' 
		slide 'Персонал. FBC Майденпек' pr_sum=obj_1066 :
			qxi::safeimport template_budget.bxl : " //*[@key='Персонал'] "		


	block 'Социальные проекты', role="SOC_",  targetgroup="div_OCM", process=top_management, contents=true
		pr_sum=div_OCM
		qxi::import template_budget.bxl : " //*[@key='Соцпроекты'] "
		slide 'Социальные проекты по структуре', noheader=1, slideformat=twobigslots
			objset="468,415,497,1066,1046",
			slot 'Расходы на собственные нужды по предприятиям на 2013 год', colcategory=1
				pr_dtype=Pie, pr_pieheight=50, pr_labelformat='%' pr_typecol=bdg_zatr
				pr_colors='76E3D6,A091DB,DEC9A6,F2A5AA,AFC433'
				ref m600999, 'Расходы на собственные нужды'
			slot 'Расходы на собственные нужды по предприятиям ожидаемые за 1 полугодие 2013 года', colcategory=1
				pr_dtype=Pie, pr_pieheight=50, pr_labelformat='%' pr_typecol=Fact_tek
				pr_colors='76E3D6,A091DB,DEC9A6,F2A5AA,AFC433'
				ref m600999, 'Расходы на собственные нужды'
				
	block 'Ремонтный фонд группа УГМК-ОЦМ', role="DR_,INV_",  targetgroup="div_OCM", process=top_management, contents=true 
		pr_sum=div_OCM
		qxi::import template_budget.bxl : " //*[@key='Ремфонд'] "	
		
		slide 'Изменение структуры основных средств по предприятиям', 
			pr_measure=млн.руб., pr_numberscalevalue="10,10,10"
			pr_dtype=MSStackedBar
			prg_showsum=1, pr_hidevalueslover=3
			pr_typecol=AHD_BB, pr_colreplace="Б1=OSTn;Б2=OSTk"
			slot 'ОАО"КзОЦМ"' pr_sum=obj_468
				qxi::safeimport template_budget.bxl : " //*[@key='Rem_OsnSR'] "			
			slot 'ОАО"РзОЦМ" ' pr_sum=obj_415
				qxi::safeimport template_budget.bxl : " //*[@key='Rem_OsnSR'] "		
			slot 'ЗАО"КЦМ"' pr_sum=obj_497
				qxi::safeimport template_budget.bxl : " //*[@key='Rem_OsnSR'] "		
			slot 'FBS.Майденпек' pr_sum=obj_1066
				qxi::safeimport template_budget.bxl : " //*[@key='Rem_OsnSR'] "		
		slide 'Динамика ремонтный фонда и амортизации по предприятиям', 
			pr_measure=млн.руб.,  pr_numberscalevalue="10,10,10"
			slot 'ОАО"КзОЦМ"' pr_sum=obj_468
				qxi::safeimport template_budget.bxl : " //*[@key='Rem_amort'] "
			slot 'ОАО"РзОЦМ" ' pr_sum=obj_415
				qxi::safeimport template_budget.bxl : " //*[@key='Rem_amort'] "
			slot 'ЗАО"КЦМ"' pr_sum=obj_497
				qxi::safeimport template_budget.bxl : " //*[@key='Rem_amort'] "
			slot 'FBS.Майденпек' pr_sum=obj_1066
				qxi::safeimport template_budget.bxl : " //*[@key='Rem_amort'] "	


	block 'Инвестиции', role="INV_,DR_" ,  targetgroup="div_OCM", process=top_management, contents=true
		slide 'Расходы инвестиционного характера', pr_sum=div_OCM :
			qxi::safeimport template_budget.bxl : " //*[@key='INV_tabl'] "
		
		slide 'Инвестиции и EBITDA в динамике по годам по предприятиям группы УГМК-ОЦМ'
			pr_numberscalevalue="1,1,1000", pr_measure=млн.руб.,
			slot 'ОАО"КзОЦМ"' pr_sum=obj_468
				qxi::safeimport template_budget.bxl : " //*[@key='INV_EBITDA'] "
			slot 'ОАО"РзОЦМ" ' pr_sum=obj_415
				qxi::safeimport template_budget.bxl : " //*[@key='INV_EBITDA'] "
			slot 'ЗАО"КЦМ"' pr_sum=obj_497
				qxi::safeimport template_budget.bxl : " //*[@key='INV_EBITDA'] "
			slot 'FBS.Майденпек' pr_sum=obj_1066
				qxi::safeimport template_budget.bxl : " //*[@key='INV_EBITDA'] "
				
	block 'Налоги, платежи и сборы', role="NAL_",  targetgroup="div_OCM", process=top_management, contents=true
		pr_sum=div_OCM
		qxi::safeimport template_budget.bxl : " //*[@key='Налоги'] "
		
	block 'Казначейство', role="FIN_,DZK_",  targetgroup="div_OCM", process=top_management, contents=true
		pr_numberscalevalue="1,1,1000" , pr_measure=млн.руб.
		slide 'Дебиторская и кредиторская задолженность', role="DZK_", slideformat=twobigslots, pr_sum=div_OCM, #prg_showsum=0
			pr_dtype=MSStackedColumn, #usecombine=1, 
			pr_plotspace=45
			slot 'Структура дебиторской задолженности', 
				pr_typecol=AHD_BB, pr_colreplace="Б2=DZk;Б1=DZn"
				pr_yminvalue=0, pr_ymaxvalue=2000000
#				ref m230810, 'ВСЕГО ДЗ'
#				ref	m230790, 'ВСЕГО ДЗ(без группы УГМК-ОЦМ)' 
				ref m230795, 'Группа УГМК (без группы УГМК-ОЦМ)'
				ref m230770, 'Контрагенты прочие'
				ref m230780, 'Налоги, платежи и сборы'
				ref m231740, 'Прочие'
			slot 'Структура кредиторской задолженности' 
				pr_typecol=AHD_BB, pr_colreplace="Б2=KZk;Б1=KZn"
				pr_yminvalue=0, pr_ymaxvalue=3000000
#				ref m230820, 'ВСЕГО КЗ'
#				ref	m230790, 'ВСЕГО KЗ (без группы УГМК-ОЦМ)'
				ref m230795, 'Группа УГМК (без группы УГМК-ОЦМ)'
				ref m230770, 'Контрагенты прочие'
				ref m230780, 'Налоги, платежи и сборы'
				ref m231740, 'Прочие'
		slide 'Дебиторская и кредиторская задолженность', role="DZK_", targetgroup=data_checking, slideformat=twobigslots, pr_sum=div_OCM	
			pr_dtype=MSStackedColumn, usecombine=1 , pr_plotspace=45
			slot 'Структура дебиторской задолженности', 
				pr_typecol=AHD_BB, pr_colreplace="Б2=DZk;Б1=DZn"
				ref m230810, 'ВСЕГО ДЗ'
				ref m231710, 'Группа УГМК (без группы УГМК-ОЦМ)'
				ref m230770, 'Контрагенты прочие'
				ref m230780, 'Налоги, платежи и сборы'
				ref m231740, 'Прочие'
			slot 'Структура кредиторской задолженности' 
				pr_typecol=AHD_BB, pr_colreplace="Б2=KZk;Б1=KZn"
				ref m230820, 'ВСЕГО КЗ'
				ref m231710, 'Группа УГМК (без группы УГМК-ОЦМ)'
				ref m230770, 'Контрагенты прочие'
				ref m230780, 'Налоги, платежи и сборы'
				ref m231740, 'Прочие'
		slide 'Дебиторская и кредиторская задолженность', role="DZK_",   targetgroup=data_checking, slideformat=twobigslots, pr_sum=div_OCM	
			pr_dtype=MSStackedColumn, #usecombine=1 		
			slot 'Структура дебиторской задолженности', 
				pr_typecol=AHD_BB, pr_colreplace="Б2=DZk;Б1=DZn"
				objset="468,415,497,1066,1046",
				ref	m230810, 'ВСЕГО ДЗ'
			slot 'Структура кредиторской задолженности' 
				pr_typecol=AHD_BB, pr_colreplace="Б2=KZk;Б1=KZn"
				objset="468,415,497,1066,1046",
				ref	m230820, 'ВСЕГО ДЗ'
		slide 'Дебиторская и кредиторская задолженность', role="DZK_",   targetgroup=data_checking, slideformat=twobigslots, pr_sum=div_OCM	
			pr_dtype=MSStackedColumn, #usecombine=1 		
			slot 'Структура дебиторской задолженности', 
				pr_typecol=AHD_BB, pr_colreplace="Б2=DZk;Б1=DZn"
				objset="468,415,497,1066,1046",
				ref	m230790, 'ВСЕГО ДЗ'
			slot 'Структура кредиторской задолженности' 
				pr_typecol=AHD_BB, pr_colreplace="Б2=KZk;Б1=KZn"
				objset="468,415,497,1066,1046",
				ref	m230790, 'ВСЕГО ДЗ'
				
		slide 'Изменение запасов', role="DZK_", pr_sum=div_OCM
			qxi::safeimport template_finanaliz.bxl : " //*[@key='kazh_zapas'] "
			
		slide 'Изменение запасов', role="DZK_",  targetgroup=top_management, slideformat=rbigtwogorslots
			pr_clearempty=1, pr_numberscalevalue="10,10,10", pr_sum=div_OCM,  
			slot 'Запасы по структуре на начало периода',  pr_legendposition='NONE', pr_measure="    "
				pr_dtype=Pyramid, pr_typecol=Fact_BB, pr_colreplace="Б2=Б1" pr_sort=1, pr_decimals=1
				ref m1111211, 'Сырье, материалы и др. ' 
				ref	m1111213, 'Затраты в незавершенном производстве' 
				ref	m1111218, 'Готовая продукция и товары для перепродажи' 
				ref	m1111216, 'Прочие запасы и затраты'
			slot 'Запасы по структуре на конец периода',  pr_legendposition='NONE', pr_measure="    "
				pr_dtype=Pyramid, pr_typecol=Fact_BB, pr_sort=1, pr_decimals=1
				ref m1111211, 'Сырье, материалы и др. '
				ref	m1111213, 'Затраты в незавершенном производстве'
				ref	m1111218, 'Готовая продукция и товары для перепродажи'
				ref	m1111216, 'Прочие запасы и затраты'
			slot 'Динамика запасов по структуре на конец периода' 
				pr_typecol=AHD_BB, colcategory=1, usestackedarea=1, usecombine=0
				ref m1111211, 'Сырье, материалы и др. '
				ref	m1111213, 'Затраты в незавершенном производстве'
				ref	m1111218, 'Готовая продукция и товары для перепродажи'		
	#			ref	m1111214, 'Готовая продукция и товары для перепродажи'
	#			ref	m1111215, 'Товары отгруженные'
				ref	m1111216, 'Прочие запасы и затраты'	
			

	
	block 'Производство', role="OBPROIZ_,MTR_,COST_", targetgroup="div_OCM", process=budget, contents=true
		slide 'Структура поставки сырья', slideformat=twobigslots, pr_measure="   ",
			pr_typecol=month_b1, pr_dtype=MSStackedColumn, pr_sum=grp_PRD_OCM
			slot 'Структура сырья по количеству',  pr_numberscalevalue='10,10,10' 
				ref m2061610 'Лом меди'
				ref m2061620 'Лом латуни'
				ref m2061710 'Катоды медные'
				ref m2061720 'Цинк чушковый'
				ref m2061730 'Никель'
				ref m2061740 'Катанка медная'				
			slot 'Структура сырья по сумме', 
				ref m2063610 'Лом меди'
				ref m2063620 'Лом латуни'
				ref m2063710 'Катоды медные'
				ref m2063720 'Цинк чушковый'
				ref m2063730 'Никель'
				ref m2063740 'Катанка медная'	

		slide 'Выполнение объемов производства по группе УГМК-ОЦМ', slideformat=twobigslots, pr_measure="   "
			pr_typecol=month_b1, pr_sum=div_OCM
			slot 'Выполнение объемов производства по структруре проката в текущем году'
				pr_dtype=MSStackedColumn			
				ref m140852 'Прокат медный'
				ref m140853 'Прокат латунный'
				ref m140854 'Прокат бронзовый'
				ref m140855 'Прокат медно-никелевый'
			slot 'Выход годного'
				colcategory=1, uselines=1, pr_decimals = 2
				pr_colreplace="Б1=KOL;PLAN=FIXKOL;PLANDIV4=FIXKOL;OZHIDPREDGODDIV4=KOL"
				ref z41010210 'Прокат медный'
				ref z41010220 'Прокат латунный'
				ref z41010230 'Прокат бронзовый'
				ref z41010240 'Прокат медно-никелевый'
		slide 'Объемы производства по видам проката', 
			pr_dtype=MSStackedColumn, uselines=1, pr_typecol=month_b1
			objset="468,415,497,1066"	
			slot 'Производство медного проката по предприятиям в текущем году',
				ref m140852 'Прокат медный'
			slot 'Производство латунного проката по предприятиям в текущем году'
				ref m140853 'Прокат латунный'
			slot 'Производство бронзового проката по предприятиям в текущем году'
				ref m140854 'Прокат бронзовый'
			slot 'Производство медно-никелевого по предприятиям в динамике по годам'
				ref m140855 'Прокат медно-никелевый'
	    slide 'Объемы производства по видам проката',  targetgroup=budget
			pr_dtype=MSStackedColumn, uselines=1, pr_typecol=month_b1
			pr_numberscalevalue='1,1,1'
#			pr_colors='76E3D6,A091DB,DEC9A6,F2A5AA',
			slot 'Выполнение объемов производства по структруре проката ОАО"КзОЦМ"', pr_sum=obj_468
				pr_dtype=MSStackedColumn			
				ref m140852 'Прокат медный'
				ref m140853 'Прокат латунный'
				ref m140854 'Прокат бронзовый'
				ref m140855 'Прокат медно-никелевый'	
			slot 'Выполнение объемов производства по структруре проката ЗАО"КЦМ"', pr_sum=obj_415
				pr_dtype=MSStackedColumn			
				ref m140852 'Прокат медный'
				ref m140853 'Прокат латунный'
				ref m140854 'Прокат бронзовый'
				ref m140855 'Прокат медно-никелевый'	
			slot 'Выполнение объемов производства по структруре проката ОАО"РзОЦМ"', pr_sum=obj_497
				pr_dtype=MSStackedColumn			
				ref m140852 'Прокат медный'
				ref m140853 'Прокат латунный'
				ref m140854 'Прокат бронзовый'
				ref m140855 'Прокат медно-никелевый'
			slot 'Выполнение объемов производства по структруре проката  FBC Майденпек', pr_sum=obj_1066
				pr_dtype=MSStackedColumn			
				ref m140852 'Прокат медный'
				
	block 'Производство', role="OBPROIZ_,MTR_,COST_", targetgroup=history 
		slide 'Выполнение объемов производства по группе УГМК-ОЦМ', slideformat=twobigslots, pr_measure="   "
			pr_dtype=MSStackedColumn, pr_typecol=year_b1, pr_sum=div_OCM, pr_numberscalevalue='1,1,1'
#			pr_colors='B84A1F,D6C445,8F8381,B3A291', 
			slot 'Выполнение объемов производства по структруре проката в текущем году', uselines=1, 
				ref m140852 'Прокат медный'
				ref m140853 'Прокат латунный'
				ref m140854 'Прокат бронзовый'
				ref m140855 'Прокат медно-никелевый'
			slot 'Выполнение объемов производства по структруре проката в динамике по годам'
				ref m140852 'Прокат медный'
				ref m140853 'Прокат латунный'
				ref m140854 'Прокат бронзовый'
				ref m140855 'Прокат медно-никелевый'
		slide 'Объемы производства по видам проката', 
			pr_dtype=MSStackedColumn, objset="468,415,497,1066", pr_numberscalevalue='1,1,1'
#			pr_colors='76E3D6,A091DB,DEC9A6,F2A5AA',
			slot 'Производство медного проката по предприятиям в динамике по годам',objset="468,415,497,1066", pr_typecol=year__b1
				ref m140852 'Прокат медный'
			slot 'Производство латунного проката по предприятиям в динамике по годам',objset="468,415,497", pr_typecol=year__b1
				ref m140853 'Прокат латунный'
		slide 'Объемы производства по видам проката', pr_measure="   "
			pr_dtype=MSStackedColumn, objset="468,415,497", pr_numberscalevalue='1,1,1'
			pr_colors='76E3D6,A091DB,DEC9A6,F2A5AA', 
			slot 'Производство бронзового проката по предприятиям в динамике по годам', pr_typecol=year_b1
				ref m140854 'Прокат бронзовый'
			slot 'Производство медно-никелевого по предприятиям в динамике по годам', pr_typecol=year_b1
				ref m140855 'Прокат медно-никелевый'
	
	
	block 'Социальные проекты', role="SOC_", slideformat=twobigslots
		slide 'Социальные проекты по предприятиям',noheader=1, slideformat=twobigslots,  pr_colors='11F7CD,F7F711,FF8800,259600'
			slot 'Расходы на собственные нужды по предприятиям в динамике по годам'
				pr_dtype=MSStackedColumn, objset="468,415,497,1066,1046", pr_typecol=year_b1
				pr_colors='76E3D6,A091DB,DEC9A6,F2A5AA,AFC433'
				ref m600999, 'Расходы на собственные нужды'
			slot 'Расходы на собственные нужды группы УГМК-ОЦМ по структуре в динамике по годам'
				pr_dtype=MSStackedColumn,  pr_typecol=year_b1
				ref m6001112, 'Благотворительность'
				ref m6001113, 'Расходы на персонал (в т.ч. культурно-спортиная работа, соц. услуги и мат.выплаты)'
				ref m6001114, 'Прочие расходы (в т.ч. СМИ, обучение)'
				ref m6001111, 'Расходы на социальные объекты'