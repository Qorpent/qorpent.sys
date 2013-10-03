transform blocklang
report PRD_TEST, "Презентация тест"
	pr_dtype=MSColumn 
	view=verstka_1	
	pr_typecol=AHD 
	slotview=graph_slot
	pr_measure=тыс.руб.
	pr_numberscalevalue="1,1,1"
	pr_decimals = 0
	pr_clearempty=1
	numberformat="#,0"
	prg_showvalues="1"
	prg_showsum="1"
	pr_linethikness=4
	pr_legendposition="BOTTOM"
	pr_measure="DYNAMIC"
	pr_clearempty = 1	
	pr_pieradius=180
	pr_ylimitsplus=5  
	pr_ylimits='auto'
	singlecontents=true
	pr_connectnull=true
	newstylerender=true
	pr_ylimitsplus = 3 
	pr_hidevalueslover = 3 
	singlecontents=true
	pr_plotspace=40
	process="top_management"


	block 'Основные параметры бюджета', role="FIN_, DR_, MTR_, COST_" , targetgroup=top_management
		slide 'Котировки металлов в текущем году', key='kotir_ME', pr_measure=руб.,	pr_linethikness=4,  pr_numberscalevalue="1,1,1"
		pr_typecol=month_b1, uselines=1, colcategory=1, 
		pr_anchorbordercolor=669ACD,  pr_anchorbgcolor=053666 
			slot 'Котировки МЕДИ на LMЕ',  pr_measure="$ \\за тонну CU", 
				pr_contextstyle="font-size~9px;top~211px;padding~4px;left~15px;border~none;background-color~#e9e9e9;font-weight~bold;font-family~Arial;color~red;"
				pr_contexttext="ТПФП"
				ref m203102 'На конец периода', pr_anchorradius=8, pr_anchorsides=3
				ref m203117 'Среднее за период', pr_anchorradius=4, pr_linedashed=1, pr_linedashlen=5, pr_linedashgap=5, 
				pr_yminvalue=6700
				pr_ymaxvalue=8200
			slot 'Котировки ЦИНКА на LMЕ', pr_measure="$ \\за тонну ZN", pr_anchorbgcolor=6CB87A, pr_anchorbordercolor=6CB87A
				ref m203103 'На конец периода',  pr_anchorradius=8, pr_anchorsides=3
				ref m203118 'Среднее за период', pr_anchorradius=4, pr_linedashed=1, pr_linedashlen=5, pr_linedashgap=5
				pr_contextstyle="font-size~9px;top~211px;padding~4px;left~15px;border~none;background-color~#e9e9e9;font-weight~bold;font-family~Arial;color~red;"
				pr_contexttext="ТПФП"
				pr_yminvalue=1700
				pr_ymaxvalue=2200
			slot 'Котировки золота ЦБ РФ',pr_measure="рубль \\за грамм AU",  
				ref m203122 'На конец периода',  pr_anchorradius=9, pr_anchorsides=3
				ref m203127 'Среднее за период', pr_anchorradius=4, pr_linedashed=1, pr_linedashlen=6, pr_linedashgap=6
				pr_contextstyle="font-size~9px;top~77px;padding~4px;left~15px;border~none;background-color~#eeeeee;font-weight~bold;font-family~Arial;color~red;"
				pr_contexttext="ТПФП"
				pr_yminvalue=1250
				pr_ymaxvalue=1650
			slot 'Котировки серебра ЦБ РФ',pr_measure="рубль \\за грамм AG", numberformat="#,#.##", pr_decimals = 1, pr_anchorbgcolor=6CB87A
				ref m203123 'На конец периода',  pr_anchorradius=8, pr_anchorsides=3
				ref m203128 'Среднее за период', pr_anchorradius=4, pr_linedashed=1, pr_linedashlen=5, pr_linedashgap=5
				pr_contextstyle="font-size~9px;top~80px;padding~4px 2px;left~2px;border~none;background-color~#eeeeee;font-weight~bold;font-family~Arial;color~red;"
				pr_contexttext="ТПФП"
				pr_yminvalue=18
				pr_ymaxvalue=32
