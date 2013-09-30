
using System;
    namespace Qorpent.Charts.FusionCharts{
    ///<summary>
 ///Типы графиков FusionChart
 ///</summary>
    [Flags]
    public enum FusionChartType :long {
      
    ///<summary>Column2D</summary>
    Column2D = 1<<1,    
  
    ///<summary>Pareto3D</summary>
    Pareto3D = 1<<2,    
  
    ///<summary>Pie3D</summary>
    Pie3D = 1<<3,    
  
    ///<summary>MSColumn2D</summary>
    MSColumn2D = 1<<4,    
  
    ///<summary>Pie2D</summary>
    Pie2D = 1<<5,    
  
    ///<summary>MSArea2D</summary>
    MSArea2D = 1<<6,    
  
    ///<summary>Area2D</summary>
    Area2D = 1<<7,    
  
    ///<summary>Bar2D</summary>
    Bar2D = 1<<8,    
  
    ///<summary>Pareto2D</summary>
    Pareto2D = 1<<9,    
  
    ///<summary>Marimekko</summary>
    Marimekko = 1<<10,    
  
    ///<summary>MSColumn3D</summary>
    MSColumn3D = 1<<11,    
  
    ///<summary>MSLine</summary>
    MSLine = 1<<12,    
  
    ///<summary>ZoomLine</summary>
    ZoomLine = 1<<13,    
  
    ///<summary>StCol3D</summary>
    StCol3D = 1<<14,    
  
    ///<summary>Doughnut3D</summary>
    Doughnut3D = 1<<15,    
  
    ///<summary>MSStCol</summary>
    MSStCol = 1<<16,    
  
    ///<summary>StCol2D</summary>
    StCol2D = 1<<17,    
  
    ///<summary>StackedBar3D</summary>
    StackedBar3D = 1<<18,    
  
    ///<summary>Combi2D</summary>
    Combi2D = 1<<19,    
  
    ///<summary>Combi3D</summary>
    Combi3D = 1<<20,    
  
    ///<summary>StCol2DLine</summary>
    StCol2DLine = 1<<21,    
  
    ///<summary>MSCol3DLineDY</summary>
    MSCol3DLineDY = 1<<22,    
  
    ///<summary>Col3DLine</summary>
    Col3DLine = 1<<23,    
  
    ///<summary>MSStColLineDY</summary>
    MSStColLineDY = 1<<24,    
  
    ///<summary>StCol3DLine</summary>
    StCol3DLine = 1<<25,    
  
    ///<summary>Scatter</summary>
    Scatter = 1<<26,    
  
    ///<summary>ScrollColumn2D</summary>
    ScrollColumn2D = 1<<27,    
  
    ///<summary>Combi2DDY</summary>
    Combi2DDY = 1<<28,    
  
    ///<summary>ScrollArea2D</summary>
    ScrollArea2D = 1<<29,    
  
    ///<summary>ScrollCombi2D</summary>
    ScrollCombi2D = 1<<30,    
  
    ///<summary>StBar2D</summary>
    StBar2D = 1<<31,    
  
    ///<summary>StCol3DLineDY</summary>
    StCol3DLineDY = 1<<32,    
  
    ///<summary>StArea2D</summary>
    StArea2D = 1<<33,    
  
    ///<summary>Bubble</summary>
    Bubble = 1<<34,    
  
    ///<summary>Column3D</summary>
    Column3D = 1<<35,    
  
    ///<summary>ScrollLine2D</summary>
    ScrollLine2D = 1<<36,    
  
    ///<summary>Doughnut2D</summary>
    Doughnut2D = 1<<37,    
  
    ///<summary>ScrollStackedCol2D</summary>
    ScrollStackedCol2D = 1<<38,    
  
    ///<summary>ScrollCombiDY2D</summary>
    ScrollCombiDY2D = 1<<39,    
  
    ///<summary>MSBar3D</summary>
    MSBar3D = 1<<40,    
  
    ///<summary>MSBar2D</summary>
    MSBar2D = 1<<41,    
  
    ///<summary>Line2D</summary>
    Line2D = 1<<42,
    }
}
  