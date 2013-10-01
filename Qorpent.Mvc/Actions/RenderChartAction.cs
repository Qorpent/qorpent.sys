﻿using Qorpent.Mvc.Binding;

namespace Qorpent.Mvc.Actions {
    /// <summary>
    /// Действие отрисовки fusionchart скрипта 
    /// </summary>
    [Action("fc.renderchart")]
    public class RenderChartAction : ActionBase {
        /// <summary>
        /// Возвращает fusionchart конфиг
        /// </summary>
        /// <returns></returns>
        protected override object MainProcess() {
            return @"<chart caption='Monthly Revenue' xAxisName='Month' yAxisName='Revenue' numberPrefix='$' showValues='0'>

   <set label='Jan' value='420000' />
   <set label='Feb' value='910000' />
   <set label='Mar' value='720000' />
   <set label='Apr' value='550000' />
   <set label='May' value='810000' />
   <set label='Jun' value='510000' />
   <set label='Jul' value='680000' />
   <set label='Aug' value='620000' />
   <set label='Sep' value='610000' />
   <set label='Oct' value='490000' />
   <set label='Nov' value='530000' />
   <set label='Dec' value='330000' />

   <trendLines>
      <line startValue='700000' color='009933' displayvalue='Target' /> 
   </trendLines>

   <styles>

      <definition>
         <style name='CanvasAnim' type='animation' param='_xScale' start= '0' duration='1' />
      </definition>

      <application>
         <apply toObject='Canvas' styles='CanvasAnim' />
      </application>   

   </styles>

</chart>";
        }
    }
}
