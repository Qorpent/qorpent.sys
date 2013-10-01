
using System;

namespace Qorpent.Charts.FusionCharts{
    ///<summary>
 ///Типы данных графиков FusionChart
 ///</summary>
   [Flags]
   public enum FusionChartDataType {
      
    ///<summary>Boolean</summary>
    Boolean = 1<<1,    
  
    ///<summary>Number</summary>
    Number = 1<<2,    
  
    ///<summary>String</summary>
    String = 1<<3,    
  
    ///<summary>Color</summary>
    Color = 1<<4,    
  
    ///<summary>Character</summary>
    Character = 1<<5,    
  
    ///<summary>ColorCode</summary>
    ColorCode = 1<<6,    
  
    }
}
  