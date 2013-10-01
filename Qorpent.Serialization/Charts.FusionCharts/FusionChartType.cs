
using System;
    namespace Qorpent.Charts.FusionCharts{
    ///<summary>
 ///Типы графиков FusionChart
 ///</summary>
    [Flags]
    public enum FusionChartType :long {
      ///<summary>Не указан</summary>
  None = 0,
  
    ///<summary>Pie2D</summary>
    Pie2D = 1<<1,    
  
    ///<summary>Column2D</summary>
    Column2D = 1<<2,    
  
    ///<summary>Pie3D</summary>
    Pie3D = 1<<3,    
  
    ///<summary>Column3D</summary>
    Column3D = 1<<4,    
  
    ///<summary>Bar2D</summary>
    Bar2D = 1<<5,    
  
    ///<summary>Line</summary>
    Line = 1<<6,    
  
    ///<summary>Doughnut2D</summary>
    Doughnut2D = 1<<7,    
  
    ///<summary>Area2D</summary>
    Area2D = 1<<8,    
  
    ///<summary>Pareto3D</summary>
    Pareto3D = 1<<9,    
  
    ///<summary>Doughnut3D</summary>
    Doughnut3D = 1<<10,    
  
    ///<summary>MSColumn2D</summary>
    MSColumn2D = 1<<11,    
  
    ///<summary>Pareto2D</summary>
    Pareto2D = 1<<12,    
  
    ///<summary>MSLine</summary>
    MSLine = 1<<13,    
  
    ///<summary>MSColumn3D</summary>
    MSColumn3D = 1<<14,    
  
    ///<summary>MSArea</summary>
    MSArea = 1<<15,    
  
    ///<summary>ZoomLine</summary>
    ZoomLine = 1<<16,    
  
    ///<summary>MSBar2D</summary>
    MSBar2D = 1<<17,    
  
    ///<summary>MSBar3D</summary>
    MSBar3D = 1<<18,    
  
    ///<summary>StackedColumn3D</summary>
    StackedColumn3D = 1<<19,    
  
    ///<summary>Marimekko</summary>
    Marimekko = 1<<20,    
  
    ///<summary>StackedArea2D</summary>
    StackedArea2D = 1<<21,    
  
    ///<summary>StackedColumn2D</summary>
    StackedColumn2D = 1<<22,    
  
    ///<summary>StackedBar2D</summary>
    StackedBar2D = 1<<23,    
  
    ///<summary>StackedBar3D</summary>
    StackedBar3D = 1<<24,    
  
    ///<summary>MSStackedColumn2D</summary>
    MSStackedColumn2D = 1<<25,    
  
    ///<summary>MSCombi3D</summary>
    MSCombi3D = 1<<26,    
  
    ///<summary>MSCombi2D</summary>
    MSCombi2D = 1<<27,    
  
    ///<summary>MSColumnLine3D</summary>
    MSColumnLine3D = 1<<28,    
  
    ///<summary>StackedColumn2DLine</summary>
    StackedColumn2DLine = 1<<29,    
  
    ///<summary>StackedColumn3DLine</summary>
    StackedColumn3DLine = 1<<30,    
  
    ///<summary>MSColumn3DLineDY</summary>
    MSColumn3DLineDY = 1<<31,    
  
    ///<summary>MSCombiDY2D</summary>
    MSCombiDY2D = 1<<32,    
  
    ///<summary>MSStackedColumn2DLineDY</summary>
    MSStackedColumn2DLineDY = 1<<33,    
  
    ///<summary>StackedColumn3DLineDY</summary>
    StackedColumn3DLineDY = 1<<34,    
  
    ///<summary>Bubble</summary>
    Bubble = 1<<35,    
  
    ///<summary>Scatter</summary>
    Scatter = 1<<36,    
  
    ///<summary>ScrollLine2D</summary>
    ScrollLine2D = 1<<37,    
  
    ///<summary>ScrollColumn2D</summary>
    ScrollColumn2D = 1<<38,    
  
    ///<summary>ScrollStackedColumn2D</summary>
    ScrollStackedColumn2D = 1<<39,    
  
    ///<summary>ScrollArea2D</summary>
    ScrollArea2D = 1<<40,    
  
    ///<summary>ScrollCombiDY2D</summary>
    ScrollCombiDY2D = 1<<41,    
  
    ///<summary>ScrollCombi2D</summary>
    ScrollCombi2D = 1<<42,    
  
    }
}
  