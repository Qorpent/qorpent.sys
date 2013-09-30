
using System;
    namespace Qorpent.Charts.FusionCharts{
    ///<summary>
 ///Типы элементов графиков FusionChart
 ///</summary>
   [Flags]
   public enum FusionChartElementType {
      
    ///<summary>Chart</summary>
    Chart = 1<<1,    
  
    ///<summary>Set</summary>
    Set = 1<<2,    
  
    ///<summary>VLine</summary>
    VLine = 1<<3,    
  
    ///<summary>Line</summary>
    Line = 1<<4,    
  
    ///<summary>Categories</summary>
    Categories = 1<<5,    
  
    ///<summary>Category</summary>
    Category = 1<<6,    
  
    ///<summary>Dataset</summary>
    Dataset = 1<<7,    
  
    ///<summary>Lineset</summary>
    Lineset = 1<<8,    
  
    }
}
  