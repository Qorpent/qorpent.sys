
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
    Pie2D = ((long)1)<<1,    
  
    ///<summary>Column2D</summary>
    Column2D = ((long)1)<<2,    
  
    ///<summary>Pie3D</summary>
    Pie3D = ((long)1)<<3,    
  
    ///<summary>Column3D</summary>
    Column3D = ((long)1)<<4,    
  
    ///<summary>Bar2D</summary>
    Bar2D = ((long)1)<<5,    
  
    ///<summary>Line</summary>
    Line = ((long)1)<<6,    
  
    ///<summary>Doughnut2D</summary>
    Doughnut2D = ((long)1)<<7,    
  
    ///<summary>Area2D</summary>
    Area2D = ((long)1)<<8,    
  
    ///<summary>Pareto3D</summary>
    Pareto3D = ((long)1)<<9,    
  
    ///<summary>Doughnut3D</summary>
    Doughnut3D = ((long)1)<<10,    
  
    ///<summary>MSColumn2D</summary>
    MSColumn2D = ((long)1)<<11,    
  
    ///<summary>Pareto2D</summary>
    Pareto2D = ((long)1)<<12,    
  
    ///<summary>MSLine</summary>
    MSLine = ((long)1)<<13,    
  
    ///<summary>MSColumn3D</summary>
    MSColumn3D = ((long)1)<<14,    
  
    ///<summary>MSArea</summary>
    MSArea = ((long)1)<<15,    
  
    ///<summary>ZoomLine</summary>
    ZoomLine = ((long)1)<<16,    
  
    ///<summary>MSBar2D</summary>
    MSBar2D = ((long)1)<<17,    
  
    ///<summary>MSBar3D</summary>
    MSBar3D = ((long)1)<<18,    
  
    ///<summary>StackedColumn3D</summary>
    StackedColumn3D = ((long)1)<<19,    
  
    ///<summary>Marimekko</summary>
    Marimekko = ((long)1)<<20,    
  
    ///<summary>StackedArea2D</summary>
    StackedArea2D = ((long)1)<<21,    
  
    ///<summary>StackedColumn2D</summary>
    StackedColumn2D = ((long)1)<<22,    
  
    ///<summary>StackedBar2D</summary>
    StackedBar2D = ((long)1)<<23,    
  
    ///<summary>StackedBar3D</summary>
    StackedBar3D = ((long)1)<<24,    
  
    ///<summary>MSStackedColumn2D</summary>
    MSStackedColumn2D = ((long)1)<<25,    
  
    ///<summary>MSCombi3D</summary>
    MSCombi3D = ((long)1)<<26,    
  
    ///<summary>MSCombi2D</summary>
    MSCombi2D = ((long)1)<<27,    
  
    ///<summary>MSColumnLine3D</summary>
    MSColumnLine3D = ((long)1)<<28,    
  
    ///<summary>StackedColumn2DLine</summary>
    StackedColumn2DLine = ((long)1)<<29,    
  
    ///<summary>StackedColumn3DLine</summary>
    StackedColumn3DLine = ((long)1)<<30,    
  
    ///<summary>MSColumn3DLineDY</summary>
    MSColumn3DLineDY = ((long)1)<<31,    
  
    ///<summary>MSCombiDY2D</summary>
    MSCombiDY2D = ((long)1)<<32,    
  
    ///<summary>MSStackedColumn2DLineDY</summary>
    MSStackedColumn2DLineDY = ((long)1)<<33,    
  
    ///<summary>StackedColumn3DLineDY</summary>
    StackedColumn3DLineDY = ((long)1)<<34,    
  
    ///<summary>Bubble</summary>
    Bubble = ((long)1)<<35,    
  
    ///<summary>Scatter</summary>
    Scatter = ((long)1)<<36,    
  
    ///<summary>ScrollLine2D</summary>
    ScrollLine2D = ((long)1)<<37,    
  
    ///<summary>ScrollColumn2D</summary>
    ScrollColumn2D = ((long)1)<<38,    
  
    ///<summary>ScrollStackedColumn2D</summary>
    ScrollStackedColumn2D = ((long)1)<<39,    
  
    ///<summary>ScrollArea2D</summary>
    ScrollArea2D = ((long)1)<<40,    
  
    ///<summary>ScrollCombiDY2D</summary>
    ScrollCombiDY2D = ((long)1)<<41,    
  
    ///<summary>ScrollCombi2D</summary>
    ScrollCombi2D = ((long)1)<<42,    
  
    }
}
  