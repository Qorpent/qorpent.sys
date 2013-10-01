
using System;
    namespace Qorpent.Charts.FusionCharts{
    ///<summary>
 ///Типы элементов графиков FusionChart
 ///</summary>
    [Flags]
    public enum FusionChartElementType {
    ///<summary>Не указан</summary>
    None = 0,
    
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
  
    ///<summary>LineSet</summary>
    Lineset = 1<<8,    
  
    }
}
  