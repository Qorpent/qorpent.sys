
using System;
using System.Collections;
using System.Collections.Generic;
namespace Qorpent.Charts.FusionCharts {
///<summary>
///Описывает атрибуты и прочие соглашения по атрибутам FusionChart
///</summary>
  public static partial class Api {

     ///<summary>Полный реестр атрибутов</summary>
    public static readonly IDictionary<string,FusionChartAttributeDescriptor> Attributes = new Dictionary<string,FusionChartAttributeDescriptor>{
         
    { Chart_Animation, new FusionChartAttributeDescriptor { 
      Name = Chart_Animation,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_Palette, new FusionChartAttributeDescriptor { 
      Name = Chart_Palette,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_PaletteColors, new FusionChartAttributeDescriptor { 
      Name = Chart_PaletteColors,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowAboutMenuItem, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowAboutMenuItem,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_AboutMenuItemLabel, new FusionChartAttributeDescriptor { 
      Name = Chart_AboutMenuItemLabel,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_AboutMenuItemLink, new FusionChartAttributeDescriptor { 
      Name = Chart_AboutMenuItemLink,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowZeroPies, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowZeroPies,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowPercentValues, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowPercentValues,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D | FusionChartType.StackedColumn3D | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowPercentInToolTip, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowPercentInToolTip,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D | FusionChartType.StackedColumn3D | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowLabels, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowLabels,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowValues, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowValues,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_LabelSepChar, new FusionChartAttributeDescriptor { 
      Name = Chart_LabelSepChar,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_DefaultAnimation, new FusionChartAttributeDescriptor { 
      Name = Chart_DefaultAnimation,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ClickURL, new FusionChartAttributeDescriptor { 
      Name = Chart_ClickURL,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_Caption, new FusionChartAttributeDescriptor { 
      Name = Chart_Caption,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_SubCaption, new FusionChartAttributeDescriptor { 
      Name = Chart_SubCaption,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowBorder, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowBorder,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_BorderColor, new FusionChartAttributeDescriptor { 
      Name = Chart_BorderColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_BorderThickness, new FusionChartAttributeDescriptor { 
      Name = Chart_BorderThickness,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_BorderAlpha, new FusionChartAttributeDescriptor { 
      Name = Chart_BorderAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_BgColor, new FusionChartAttributeDescriptor { 
      Name = Chart_BgColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_BgAlpha, new FusionChartAttributeDescriptor { 
      Name = Chart_BgAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_BgRatio, new FusionChartAttributeDescriptor { 
      Name = Chart_BgRatio,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_BgAngle, new FusionChartAttributeDescriptor { 
      Name = Chart_BgAngle,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_BgImage, new FusionChartAttributeDescriptor { 
      Name = Chart_BgImage,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_BgImageAlpha, new FusionChartAttributeDescriptor { 
      Name = Chart_BgImageAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_BgImageDisplayMode, new FusionChartAttributeDescriptor { 
      Name = Chart_BgImageDisplayMode,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_BgImageVAlign, new FusionChartAttributeDescriptor { 
      Name = Chart_BgImageVAlign,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_BgImageHAlign, new FusionChartAttributeDescriptor { 
      Name = Chart_BgImageHAlign,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_BgImageScale, new FusionChartAttributeDescriptor { 
      Name = Chart_BgImageScale,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_LogoURL, new FusionChartAttributeDescriptor { 
      Name = Chart_LogoURL,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_LogoPosition, new FusionChartAttributeDescriptor { 
      Name = Chart_LogoPosition,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_LogoAlpha, new FusionChartAttributeDescriptor { 
      Name = Chart_LogoAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_LogoScale, new FusionChartAttributeDescriptor { 
      Name = Chart_LogoScale,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_LogoLink, new FusionChartAttributeDescriptor { 
      Name = Chart_LogoLink,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowPlotBorder, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowPlotBorder,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_PlotBorderColor, new FusionChartAttributeDescriptor { 
      Name = Chart_PlotBorderColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_PlotBorderThickness, new FusionChartAttributeDescriptor { 
      Name = Chart_PlotBorderThickness,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_PlotBorderAlpha, new FusionChartAttributeDescriptor { 
      Name = Chart_PlotBorderAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_PlotFillAlpha, new FusionChartAttributeDescriptor { 
      Name = Chart_PlotFillAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowShadow, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowShadow,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_Use3DLighting, new FusionChartAttributeDescriptor { 
      Name = Chart_Use3DLighting,
      Charts = FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Doughnut2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn3D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.StackedBar3D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_Radius3D, new FusionChartAttributeDescriptor { 
      Name = Chart_Radius3D,
      Charts = FusionChartType.Pie2D | FusionChartType.Doughnut2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_SlicingDistance, new FusionChartAttributeDescriptor { 
      Name = Chart_SlicingDistance,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_PieRadius, new FusionChartAttributeDescriptor { 
      Name = Chart_PieRadius,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_StartingAngle, new FusionChartAttributeDescriptor { 
      Name = Chart_StartingAngle,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_EnableRotation, new FusionChartAttributeDescriptor { 
      Name = Chart_EnableRotation,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_EnableSmartLabels, new FusionChartAttributeDescriptor { 
      Name = Chart_EnableSmartLabels,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_SkipOverlapLabels, new FusionChartAttributeDescriptor { 
      Name = Chart_SkipOverlapLabels,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_IsSmartLineSlanted, new FusionChartAttributeDescriptor { 
      Name = Chart_IsSmartLineSlanted,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_SmartLineColor, new FusionChartAttributeDescriptor { 
      Name = Chart_SmartLineColor,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_SmartLineThickness, new FusionChartAttributeDescriptor { 
      Name = Chart_SmartLineThickness,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_SmartLineAlpha, new FusionChartAttributeDescriptor { 
      Name = Chart_SmartLineAlpha,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_LabelDistance, new FusionChartAttributeDescriptor { 
      Name = Chart_LabelDistance,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_SmartLabelClearance, new FusionChartAttributeDescriptor { 
      Name = Chart_SmartLabelClearance,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ManageLabelOverflow, new FusionChartAttributeDescriptor { 
      Name = Chart_ManageLabelOverflow,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_UseEllipsesWhenOverflow, new FusionChartAttributeDescriptor { 
      Name = Chart_UseEllipsesWhenOverflow,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_FormatNumber, new FusionChartAttributeDescriptor { 
      Name = Chart_FormatNumber,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_FormatNumberScale, new FusionChartAttributeDescriptor { 
      Name = Chart_FormatNumberScale,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_DefaultNumberScale, new FusionChartAttributeDescriptor { 
      Name = Chart_DefaultNumberScale,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_NumberScaleUnit, new FusionChartAttributeDescriptor { 
      Name = Chart_NumberScaleUnit,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_NumberScaleValue, new FusionChartAttributeDescriptor { 
      Name = Chart_NumberScaleValue,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ScaleRecursively, new FusionChartAttributeDescriptor { 
      Name = Chart_ScaleRecursively,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_MaxScaleRecursion, new FusionChartAttributeDescriptor { 
      Name = Chart_MaxScaleRecursion,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ScaleSeparator, new FusionChartAttributeDescriptor { 
      Name = Chart_ScaleSeparator,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_NumberPrefix, new FusionChartAttributeDescriptor { 
      Name = Chart_NumberPrefix,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_NumberSuffix, new FusionChartAttributeDescriptor { 
      Name = Chart_NumberSuffix,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_DecimalSeparator, new FusionChartAttributeDescriptor { 
      Name = Chart_DecimalSeparator,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ThousandSeparator, new FusionChartAttributeDescriptor { 
      Name = Chart_ThousandSeparator,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ThousandSeparatorPosition, new FusionChartAttributeDescriptor { 
      Name = Chart_ThousandSeparatorPosition,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_InDecimalSeparator, new FusionChartAttributeDescriptor { 
      Name = Chart_InDecimalSeparator,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_InThousandSeparator, new FusionChartAttributeDescriptor { 
      Name = Chart_InThousandSeparator,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_Decimals, new FusionChartAttributeDescriptor { 
      Name = Chart_Decimals,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ForceDecimals, new FusionChartAttributeDescriptor { 
      Name = Chart_ForceDecimals,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_BaseFont, new FusionChartAttributeDescriptor { 
      Name = Chart_BaseFont,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_BaseFontSize, new FusionChartAttributeDescriptor { 
      Name = Chart_BaseFontSize,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_BaseFontColor, new FusionChartAttributeDescriptor { 
      Name = Chart_BaseFontColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowLegend, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowLegend,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_LegendPosition, new FusionChartAttributeDescriptor { 
      Name = Chart_LegendPosition,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_LegendCaption, new FusionChartAttributeDescriptor { 
      Name = Chart_LegendCaption,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_LegendIconScale, new FusionChartAttributeDescriptor { 
      Name = Chart_LegendIconScale,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_LegendBgColor, new FusionChartAttributeDescriptor { 
      Name = Chart_LegendBgColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_LegendBgAlpha, new FusionChartAttributeDescriptor { 
      Name = Chart_LegendBgAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_LegendBorderColor, new FusionChartAttributeDescriptor { 
      Name = Chart_LegendBorderColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_LegendBorderThickness, new FusionChartAttributeDescriptor { 
      Name = Chart_LegendBorderThickness,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_LegendBorderAlpha, new FusionChartAttributeDescriptor { 
      Name = Chart_LegendBorderAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_LegendShadow, new FusionChartAttributeDescriptor { 
      Name = Chart_LegendShadow,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_LegendAllowDrag, new FusionChartAttributeDescriptor { 
      Name = Chart_LegendAllowDrag,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_LegendScrollBgColor, new FusionChartAttributeDescriptor { 
      Name = Chart_LegendScrollBgColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_LegendScrollBarColor, new FusionChartAttributeDescriptor { 
      Name = Chart_LegendScrollBarColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_LegendScrollBtnColor, new FusionChartAttributeDescriptor { 
      Name = Chart_LegendScrollBtnColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ReverseLegend, new FusionChartAttributeDescriptor { 
      Name = Chart_ReverseLegend,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_InteractiveLegend, new FusionChartAttributeDescriptor { 
      Name = Chart_InteractiveLegend,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_LegendNumColumns, new FusionChartAttributeDescriptor { 
      Name = Chart_LegendNumColumns,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_MinimiseWrappingInLegend, new FusionChartAttributeDescriptor { 
      Name = Chart_MinimiseWrappingInLegend,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowToolTip, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowToolTip,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ToolTipBgColor, new FusionChartAttributeDescriptor { 
      Name = Chart_ToolTipBgColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ToolTipBorderColor, new FusionChartAttributeDescriptor { 
      Name = Chart_ToolTipBorderColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ToolTipSepChar, new FusionChartAttributeDescriptor { 
      Name = Chart_ToolTipSepChar,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowToolTipShadow, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowToolTipShadow,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_CaptionPadding, new FusionChartAttributeDescriptor { 
      Name = Chart_CaptionPadding,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ChartLeftMargin, new FusionChartAttributeDescriptor { 
      Name = Chart_ChartLeftMargin,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ChartRightMargin, new FusionChartAttributeDescriptor { 
      Name = Chart_ChartRightMargin,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ChartTopMargin, new FusionChartAttributeDescriptor { 
      Name = Chart_ChartTopMargin,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ChartBottomMargin, new FusionChartAttributeDescriptor { 
      Name = Chart_ChartBottomMargin,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Set_BorderColor, new FusionChartAttributeDescriptor { 
      Name = Set_BorderColor,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Set,
    } },
  
    { Set_BorderAlpha, new FusionChartAttributeDescriptor { 
      Name = Set_BorderAlpha,
      Charts = FusionChartType.Pie2D | FusionChartType.Doughnut2D,
      Element = FusionChartElementType.Set,
    } },
  
    { Set_IsSliced, new FusionChartAttributeDescriptor { 
      Name = Set_IsSliced,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Set,
    } },
  
    { Set_Label, new FusionChartAttributeDescriptor { 
      Name = Set_Label,
      Charts = FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D,
      Element = FusionChartElementType.Set,
    } },
  
    { Set_Value, new FusionChartAttributeDescriptor { 
      Name = Set_Value,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Set,
    } },
  
    { Set_DisplayValue, new FusionChartAttributeDescriptor { 
      Name = Set_DisplayValue,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Set,
    } },
  
    { Set_Color, new FusionChartAttributeDescriptor { 
      Name = Set_Color,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Set,
    } },
  
    { Set_Link, new FusionChartAttributeDescriptor { 
      Name = Set_Link,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Set,
    } },
  
    { Set_ToolText, new FusionChartAttributeDescriptor { 
      Name = Set_ToolText,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Set,
    } },
  
    { Set_Dashed, new FusionChartAttributeDescriptor { 
      Name = Set_Dashed,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Set,
    } },
  
    { Set_Alpha, new FusionChartAttributeDescriptor { 
      Name = Set_Alpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Set,
    } },
  
    { Set_ShowLabel, new FusionChartAttributeDescriptor { 
      Name = Set_ShowLabel,
      Charts = FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D,
      Element = FusionChartElementType.Set,
    } },
  
    { Set_ShowValue, new FusionChartAttributeDescriptor { 
      Name = Set_ShowValue,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Set,
    } },
  
    { Chart_LabelDisplay, new FusionChartAttributeDescriptor { 
      Name = Chart_LabelDisplay,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_RotateLabels, new FusionChartAttributeDescriptor { 
      Name = Chart_RotateLabels,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_SlantLabels, new FusionChartAttributeDescriptor { 
      Name = Chart_SlantLabels,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_LabelStep, new FusionChartAttributeDescriptor { 
      Name = Chart_LabelStep,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_StaggerLines, new FusionChartAttributeDescriptor { 
      Name = Chart_StaggerLines,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_RotateValues, new FusionChartAttributeDescriptor { 
      Name = Chart_RotateValues,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_PlaceValuesInside, new FusionChartAttributeDescriptor { 
      Name = Chart_PlaceValuesInside,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSColumn3D | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedBar3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowYAxisValues, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowYAxisValues,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowLimits, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowLimits,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowDivLineValues, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowDivLineValues,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_YAxisValuesStep, new FusionChartAttributeDescriptor { 
      Name = Chart_YAxisValuesStep,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_AdjustDiv, new FusionChartAttributeDescriptor { 
      Name = Chart_AdjustDiv,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_RotateYAxisName, new FusionChartAttributeDescriptor { 
      Name = Chart_RotateYAxisName,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_YAxisNameWidth, new FusionChartAttributeDescriptor { 
      Name = Chart_YAxisNameWidth,
      Charts = FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_YAxisMinValue, new FusionChartAttributeDescriptor { 
      Name = Chart_YAxisMinValue,
      Charts = FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_YAxisMaxValue, new FusionChartAttributeDescriptor { 
      Name = Chart_YAxisMaxValue,
      Charts = FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_SetAdaptiveYMin, new FusionChartAttributeDescriptor { 
      Name = Chart_SetAdaptiveYMin,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_CenterYaxisName, new FusionChartAttributeDescriptor { 
      Name = Chart_CenterYaxisName,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_XAxisName, new FusionChartAttributeDescriptor { 
      Name = Chart_XAxisName,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_YAxisName, new FusionChartAttributeDescriptor { 
      Name = Chart_YAxisName,
      Charts = FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_CanvasBgColor, new FusionChartAttributeDescriptor { 
      Name = Chart_CanvasBgColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_CanvasBgAlpha, new FusionChartAttributeDescriptor { 
      Name = Chart_CanvasBgAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_CanvasBgRatio, new FusionChartAttributeDescriptor { 
      Name = Chart_CanvasBgRatio,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_CanvasBgAngle, new FusionChartAttributeDescriptor { 
      Name = Chart_CanvasBgAngle,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_CanvasBorderColor, new FusionChartAttributeDescriptor { 
      Name = Chart_CanvasBorderColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_CanvasBorderThickness, new FusionChartAttributeDescriptor { 
      Name = Chart_CanvasBorderThickness,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_CanvasBorderAlpha, new FusionChartAttributeDescriptor { 
      Name = Chart_CanvasBorderAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowVLineLabelBorder, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowVLineLabelBorder,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_UseRoundEdges, new FusionChartAttributeDescriptor { 
      Name = Chart_UseRoundEdges,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSBar2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_PlotBorderDashed, new FusionChartAttributeDescriptor { 
      Name = Chart_PlotBorderDashed,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_PlotBorderDashLen, new FusionChartAttributeDescriptor { 
      Name = Chart_PlotBorderDashLen,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_PlotBorderDashGap, new FusionChartAttributeDescriptor { 
      Name = Chart_PlotBorderDashGap,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_PlotFillAngle, new FusionChartAttributeDescriptor { 
      Name = Chart_PlotFillAngle,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_PlotFillRatio, new FusionChartAttributeDescriptor { 
      Name = Chart_PlotFillRatio,
      Charts = FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSBar2D | FusionChartType.Marimekko | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_PlotGradientColor, new FusionChartAttributeDescriptor { 
      Name = Chart_PlotGradientColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_NumDivLines, new FusionChartAttributeDescriptor { 
      Name = Chart_NumDivLines,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_DivLineColor, new FusionChartAttributeDescriptor { 
      Name = Chart_DivLineColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_DivLineThickness, new FusionChartAttributeDescriptor { 
      Name = Chart_DivLineThickness,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_DivLineAlpha, new FusionChartAttributeDescriptor { 
      Name = Chart_DivLineAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_DivLineIsDashed, new FusionChartAttributeDescriptor { 
      Name = Chart_DivLineIsDashed,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_DivLineDashLen, new FusionChartAttributeDescriptor { 
      Name = Chart_DivLineDashLen,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_DivLineDashGap, new FusionChartAttributeDescriptor { 
      Name = Chart_DivLineDashGap,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ZeroPlaneColor, new FusionChartAttributeDescriptor { 
      Name = Chart_ZeroPlaneColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ZeroPlaneThickness, new FusionChartAttributeDescriptor { 
      Name = Chart_ZeroPlaneThickness,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ZeroPlaneAlpha, new FusionChartAttributeDescriptor { 
      Name = Chart_ZeroPlaneAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowZeroPlaneValue, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowZeroPlaneValue,
      Charts = FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowAlternateHGridColor, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowAlternateHGridColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_AlternateHGridColor, new FusionChartAttributeDescriptor { 
      Name = Chart_AlternateHGridColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_AlternateHGridAlpha, new FusionChartAttributeDescriptor { 
      Name = Chart_AlternateHGridAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ForceYAxisValueDecimals, new FusionChartAttributeDescriptor { 
      Name = Chart_ForceYAxisValueDecimals,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_YAxisValueDecimals, new FusionChartAttributeDescriptor { 
      Name = Chart_YAxisValueDecimals,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_OutCnvBaseFont, new FusionChartAttributeDescriptor { 
      Name = Chart_OutCnvBaseFont,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_OutCnvBaseFontSize, new FusionChartAttributeDescriptor { 
      Name = Chart_OutCnvBaseFontSize,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_OutCnvBaseFontColor, new FusionChartAttributeDescriptor { 
      Name = Chart_OutCnvBaseFontColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_XAxisNamePadding, new FusionChartAttributeDescriptor { 
      Name = Chart_XAxisNamePadding,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_YAxisNamePadding, new FusionChartAttributeDescriptor { 
      Name = Chart_YAxisNamePadding,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_YAxisValuesPadding, new FusionChartAttributeDescriptor { 
      Name = Chart_YAxisValuesPadding,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_LabelPadding, new FusionChartAttributeDescriptor { 
      Name = Chart_LabelPadding,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ValuePadding, new FusionChartAttributeDescriptor { 
      Name = Chart_ValuePadding,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.Marimekko | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_PlotSpacePercent, new FusionChartAttributeDescriptor { 
      Name = Chart_PlotSpacePercent,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSColumn3D | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_CanvasLeftMargin, new FusionChartAttributeDescriptor { 
      Name = Chart_CanvasLeftMargin,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_CanvasRightMargin, new FusionChartAttributeDescriptor { 
      Name = Chart_CanvasRightMargin,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_CanvasTopMargin, new FusionChartAttributeDescriptor { 
      Name = Chart_CanvasTopMargin,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_CanvasBottomMargin, new FusionChartAttributeDescriptor { 
      Name = Chart_CanvasBottomMargin,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { VLine_Color, new FusionChartAttributeDescriptor { 
      Name = VLine_Color,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.VLine,
    } },
  
    { VLine_Thickness, new FusionChartAttributeDescriptor { 
      Name = VLine_Thickness,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.VLine,
    } },
  
    { VLine_Alpha, new FusionChartAttributeDescriptor { 
      Name = VLine_Alpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.VLine,
    } },
  
    { VLine_Dashed, new FusionChartAttributeDescriptor { 
      Name = VLine_Dashed,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.VLine,
    } },
  
    { VLine_DashLen, new FusionChartAttributeDescriptor { 
      Name = VLine_DashLen,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.VLine,
    } },
  
    { VLine_DashGap, new FusionChartAttributeDescriptor { 
      Name = VLine_DashGap,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.VLine,
    } },
  
    { VLine_Label, new FusionChartAttributeDescriptor { 
      Name = VLine_Label,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.VLine,
    } },
  
    { VLine_ShowLabelBorder, new FusionChartAttributeDescriptor { 
      Name = VLine_ShowLabelBorder,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.VLine,
    } },
  
    { VLine_LinePosition, new FusionChartAttributeDescriptor { 
      Name = VLine_LinePosition,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.VLine,
    } },
  
    { VLine_LabelPosition, new FusionChartAttributeDescriptor { 
      Name = VLine_LabelPosition,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.VLine,
    } },
  
    { VLine_LabelHAlign, new FusionChartAttributeDescriptor { 
      Name = VLine_LabelHAlign,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.VLine,
    } },
  
    { VLine_LabelVAlign, new FusionChartAttributeDescriptor { 
      Name = VLine_LabelVAlign,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.VLine,
    } },
  
    { Line_StartValue, new FusionChartAttributeDescriptor { 
      Name = Line_StartValue,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Line,
    } },
  
    { Line_EndValue, new FusionChartAttributeDescriptor { 
      Name = Line_EndValue,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Line,
    } },
  
    { Line_DisplayValue, new FusionChartAttributeDescriptor { 
      Name = Line_DisplayValue,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Line,
    } },
  
    { Line_Color, new FusionChartAttributeDescriptor { 
      Name = Line_Color,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Line,
    } },
  
    { Line_IsTrendZone, new FusionChartAttributeDescriptor { 
      Name = Line_IsTrendZone,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Line,
    } },
  
    { Line_ShowOnTop, new FusionChartAttributeDescriptor { 
      Name = Line_ShowOnTop,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Line,
    } },
  
    { Line_Thickness, new FusionChartAttributeDescriptor { 
      Name = Line_Thickness,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Line,
    } },
  
    { Line_Alpha, new FusionChartAttributeDescriptor { 
      Name = Line_Alpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Line,
    } },
  
    { Line_Dashed, new FusionChartAttributeDescriptor { 
      Name = Line_Dashed,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Line,
    } },
  
    { Line_DashLen, new FusionChartAttributeDescriptor { 
      Name = Line_DashLen,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Line,
    } },
  
    { Line_DashGap, new FusionChartAttributeDescriptor { 
      Name = Line_DashGap,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Line,
    } },
  
    { Line_ValueOnRight, new FusionChartAttributeDescriptor { 
      Name = Line_ValueOnRight,
      Charts = FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine,
      Element = FusionChartElementType.Line,
    } },
  
    { Line_ToolText, new FusionChartAttributeDescriptor { 
      Name = Line_ToolText,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Line,
    } },
  
    { Chart_PieInnerFaceAlpha, new FusionChartAttributeDescriptor { 
      Name = Chart_PieInnerFaceAlpha,
      Charts = FusionChartType.Pie3D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_PieOuterFaceAlpha, new FusionChartAttributeDescriptor { 
      Name = Chart_PieOuterFaceAlpha,
      Charts = FusionChartType.Pie3D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_PieYScale, new FusionChartAttributeDescriptor { 
      Name = Chart_PieYScale,
      Charts = FusionChartType.Pie3D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_PieSliceDepth, new FusionChartAttributeDescriptor { 
      Name = Chart_PieSliceDepth,
      Charts = FusionChartType.Pie3D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_MaxColWidth, new FusionChartAttributeDescriptor { 
      Name = Chart_MaxColWidth,
      Charts = FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Pareto3D | FusionChartType.MSColumn3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_CanvasBaseColor, new FusionChartAttributeDescriptor { 
      Name = Chart_CanvasBaseColor,
      Charts = FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Pareto3D | FusionChartType.MSColumn3D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.StackedBar3D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowCanvasBg, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowCanvasBg,
      Charts = FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Pareto3D | FusionChartType.MSColumn3D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.StackedBar3D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowCanvasBase, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowCanvasBase,
      Charts = FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Pareto3D | FusionChartType.MSColumn3D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.StackedBar3D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_CanvasBaseDepth, new FusionChartAttributeDescriptor { 
      Name = Chart_CanvasBaseDepth,
      Charts = FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Pareto3D | FusionChartType.MSColumn3D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.StackedBar3D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_CanvasBgDepth, new FusionChartAttributeDescriptor { 
      Name = Chart_CanvasBgDepth,
      Charts = FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Pareto3D | FusionChartType.MSColumn3D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.StackedBar3D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_OverlapColumns, new FusionChartAttributeDescriptor { 
      Name = Chart_OverlapColumns,
      Charts = FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Pareto3D | FusionChartType.MSColumn3D | FusionChartType.StackedColumn3D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ZeroPlaneShowBorder, new FusionChartAttributeDescriptor { 
      Name = Chart_ZeroPlaneShowBorder,
      Charts = FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Pareto3D | FusionChartType.MSColumn3D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.StackedBar3D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ZeroPlaneBorderColor, new FusionChartAttributeDescriptor { 
      Name = Chart_ZeroPlaneBorderColor,
      Charts = FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Pareto3D | FusionChartType.MSColumn3D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.StackedBar3D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_MaxLabelWidthPercent, new FusionChartAttributeDescriptor { 
      Name = Chart_MaxLabelWidthPercent,
      Charts = FusionChartType.Bar2D | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_RotateXAxisName, new FusionChartAttributeDescriptor { 
      Name = Chart_RotateXAxisName,
      Charts = FusionChartType.Bar2D | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_XAxisNameWidth, new FusionChartAttributeDescriptor { 
      Name = Chart_XAxisNameWidth,
      Charts = FusionChartType.Bar2D | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_CenterXaxisName, new FusionChartAttributeDescriptor { 
      Name = Chart_CenterXaxisName,
      Charts = FusionChartType.Bar2D | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowAlternateVGridColor, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowAlternateVGridColor,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.StackedArea2D | FusionChartType.StackedBar2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_AlternateVGridColor, new FusionChartAttributeDescriptor { 
      Name = Chart_AlternateVGridColor,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.StackedArea2D | FusionChartType.StackedBar2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_AlternateVGridAlpha, new FusionChartAttributeDescriptor { 
      Name = Chart_AlternateVGridAlpha,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.StackedArea2D | FusionChartType.StackedBar2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ConnectNullData, new FusionChartAttributeDescriptor { 
      Name = Chart_ConnectNullData,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedArea2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ValuePosition, new FusionChartAttributeDescriptor { 
      Name = Chart_ValuePosition,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.ZoomLine | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_LineColor, new FusionChartAttributeDescriptor { 
      Name = Chart_LineColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.ZoomLine | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_LineThickness, new FusionChartAttributeDescriptor { 
      Name = Chart_LineThickness,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.ZoomLine | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_LineAlpha, new FusionChartAttributeDescriptor { 
      Name = Chart_LineAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.ZoomLine | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_LineDashed, new FusionChartAttributeDescriptor { 
      Name = Chart_LineDashed,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_LineDashLen, new FusionChartAttributeDescriptor { 
      Name = Chart_LineDashLen,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_LineDashGap, new FusionChartAttributeDescriptor { 
      Name = Chart_LineDashGap,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_DrawAnchors, new FusionChartAttributeDescriptor { 
      Name = Chart_DrawAnchors,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_AnchorSides, new FusionChartAttributeDescriptor { 
      Name = Chart_AnchorSides,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_AnchorRadius, new FusionChartAttributeDescriptor { 
      Name = Chart_AnchorRadius,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_AnchorBorderColor, new FusionChartAttributeDescriptor { 
      Name = Chart_AnchorBorderColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_AnchorBorderThickness, new FusionChartAttributeDescriptor { 
      Name = Chart_AnchorBorderThickness,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_AnchorBgColor, new FusionChartAttributeDescriptor { 
      Name = Chart_AnchorBgColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_AnchorAlpha, new FusionChartAttributeDescriptor { 
      Name = Chart_AnchorAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_AnchorBgAlpha, new FusionChartAttributeDescriptor { 
      Name = Chart_AnchorBgAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_NumVDivLines, new FusionChartAttributeDescriptor { 
      Name = Chart_NumVDivLines,
      Charts = FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_VDivLineColor, new FusionChartAttributeDescriptor { 
      Name = Chart_VDivLineColor,
      Charts = FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_VDivLineThickness, new FusionChartAttributeDescriptor { 
      Name = Chart_VDivLineThickness,
      Charts = FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_VDivLineAlpha, new FusionChartAttributeDescriptor { 
      Name = Chart_VDivLineAlpha,
      Charts = FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_VDivLineIsDashed, new FusionChartAttributeDescriptor { 
      Name = Chart_VDivLineIsDashed,
      Charts = FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_VDivLineDashLen, new FusionChartAttributeDescriptor { 
      Name = Chart_VDivLineDashLen,
      Charts = FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_VDivLineDashGap, new FusionChartAttributeDescriptor { 
      Name = Chart_VDivLineDashGap,
      Charts = FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowZeroPlane, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowZeroPlane,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.MSLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_CanvasPadding, new FusionChartAttributeDescriptor { 
      Name = Chart_CanvasPadding,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.MSBar3D | FusionChartType.StackedArea2D | FusionChartType.StackedBar3D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Set_ValuePosition, new FusionChartAttributeDescriptor { 
      Name = Set_ValuePosition,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Set,
    } },
  
    { Set_AnchorSides, new FusionChartAttributeDescriptor { 
      Name = Set_AnchorSides,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Set,
    } },
  
    { Set_AnchorRadius, new FusionChartAttributeDescriptor { 
      Name = Set_AnchorRadius,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Set,
    } },
  
    { Set_AnchorBorderColor, new FusionChartAttributeDescriptor { 
      Name = Set_AnchorBorderColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Set,
    } },
  
    { Set_AnchorBorderThickness, new FusionChartAttributeDescriptor { 
      Name = Set_AnchorBorderThickness,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Set,
    } },
  
    { Set_AnchorBgColor, new FusionChartAttributeDescriptor { 
      Name = Set_AnchorBgColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Set,
    } },
  
    { Set_AnchorAlpha, new FusionChartAttributeDescriptor { 
      Name = Set_AnchorAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Set,
    } },
  
    { Set_AnchorBgAlpha, new FusionChartAttributeDescriptor { 
      Name = Set_AnchorBgAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Set,
    } },
  
    { Chart_DoughnutRadius, new FusionChartAttributeDescriptor { 
      Name = Chart_DoughnutRadius,
      Charts = FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_PlotFillColor, new FusionChartAttributeDescriptor { 
      Name = Chart_PlotFillColor,
      Charts = FusionChartType.ScrollArea2D | FusionChartType.MSArea | FusionChartType.StackedArea2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowSecondaryLimits, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowSecondaryLimits,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.Pareto2D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowDivLineSecondaryValue, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowDivLineSecondaryValue,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.Pareto2D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_PYAxisMaxValue, new FusionChartAttributeDescriptor { 
      Name = Chart_PYAxisMaxValue,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.Pareto2D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_PYAxisMinValue, new FusionChartAttributeDescriptor { 
      Name = Chart_PYAxisMinValue,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.Pareto2D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_PYAxisNameWidth, new FusionChartAttributeDescriptor { 
      Name = Chart_PYAxisNameWidth,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.Pareto2D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_SYAxisNameWidth, new FusionChartAttributeDescriptor { 
      Name = Chart_SYAxisNameWidth,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.Pareto2D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowCumulativeLine, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowCumulativeLine,
      Charts = FusionChartType.Pareto3D | FusionChartType.Pareto2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowLineValues, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowLineValues,
      Charts = FusionChartType.Pareto3D | FusionChartType.Pareto2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_PrimaryAxisOnLeft, new FusionChartAttributeDescriptor { 
      Name = Chart_PrimaryAxisOnLeft,
      Charts = FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_Use3DLineShift, new FusionChartAttributeDescriptor { 
      Name = Chart_Use3DLineShift,
      Charts = FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_SYAXisName, new FusionChartAttributeDescriptor { 
      Name = Chart_SYAXisName,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.Pareto2D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_PYAxisName, new FusionChartAttributeDescriptor { 
      Name = Chart_PYAxisName,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.Pareto2D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_Sdecimals, new FusionChartAttributeDescriptor { 
      Name = Chart_Sdecimals,
      Charts = FusionChartType.Pareto3D | FusionChartType.Pareto2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_SforceDecimals, new FusionChartAttributeDescriptor { 
      Name = Chart_SforceDecimals,
      Charts = FusionChartType.Pareto3D | FusionChartType.Pareto2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_SyAxisValueDecimals, new FusionChartAttributeDescriptor { 
      Name = Chart_SyAxisValueDecimals,
      Charts = FusionChartType.Pareto3D | FusionChartType.Pareto2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Line_ParentYAxis, new FusionChartAttributeDescriptor { 
      Name = Line_ParentYAxis,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.Pareto2D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Line,
    } },
  
    { Chart_LegendMarkerCircle, new FusionChartAttributeDescriptor { 
      Name = Chart_LegendMarkerCircle,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_SeriesNameInToolTip, new FusionChartAttributeDescriptor { 
      Name = Chart_SeriesNameInToolTip,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_LegendPadding, new FusionChartAttributeDescriptor { 
      Name = Chart_LegendPadding,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Categories_Font, new FusionChartAttributeDescriptor { 
      Name = Categories_Font,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Categories,
    } },
  
    { Categories_FontSize, new FusionChartAttributeDescriptor { 
      Name = Categories_FontSize,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Categories,
    } },
  
    { Categories_FontColor, new FusionChartAttributeDescriptor { 
      Name = Categories_FontColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Categories,
    } },
  
    { Category_Label, new FusionChartAttributeDescriptor { 
      Name = Category_Label,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Category,
    } },
  
    { Category_ShowLabel, new FusionChartAttributeDescriptor { 
      Name = Category_ShowLabel,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Category,
    } },
  
    { Category_ToolText, new FusionChartAttributeDescriptor { 
      Name = Category_ToolText,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Category,
    } },
  
    { Dataset_SeriesName, new FusionChartAttributeDescriptor { 
      Name = Dataset_SeriesName,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Dataset_Color, new FusionChartAttributeDescriptor { 
      Name = Dataset_Color,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Dataset_Alpha, new FusionChartAttributeDescriptor { 
      Name = Dataset_Alpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Dataset_Ratio, new FusionChartAttributeDescriptor { 
      Name = Dataset_Ratio,
      Charts = FusionChartType.Pie2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.MSColumn2D | FusionChartType.MSBar2D | FusionChartType.Marimekko | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Dataset_ShowValues, new FusionChartAttributeDescriptor { 
      Name = Dataset_ShowValues,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Dataset_Dashed, new FusionChartAttributeDescriptor { 
      Name = Dataset_Dashed,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Dataset_IncludeInLegend, new FusionChartAttributeDescriptor { 
      Name = Dataset_IncludeInLegend,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Dataset_ValuePosition, new FusionChartAttributeDescriptor { 
      Name = Dataset_ValuePosition,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.ZoomLine | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Dataset_DrawAnchors, new FusionChartAttributeDescriptor { 
      Name = Dataset_DrawAnchors,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Dataset_AnchorSides, new FusionChartAttributeDescriptor { 
      Name = Dataset_AnchorSides,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Dataset_AnchorRadius, new FusionChartAttributeDescriptor { 
      Name = Dataset_AnchorRadius,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Dataset_AnchorBorderColor, new FusionChartAttributeDescriptor { 
      Name = Dataset_AnchorBorderColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Dataset_AnchorBorderThickness, new FusionChartAttributeDescriptor { 
      Name = Dataset_AnchorBorderThickness,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Dataset_AnchorBgColor, new FusionChartAttributeDescriptor { 
      Name = Dataset_AnchorBgColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Dataset_AnchorAlpha, new FusionChartAttributeDescriptor { 
      Name = Dataset_AnchorAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Dataset_AnchorBgAlpha, new FusionChartAttributeDescriptor { 
      Name = Dataset_AnchorBgAlpha,
      Charts = FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.ScrollArea2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedArea2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Dataset_LineThickness, new FusionChartAttributeDescriptor { 
      Name = Dataset_LineThickness,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.ZoomLine | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Dataset_LineDashLen, new FusionChartAttributeDescriptor { 
      Name = Dataset_LineDashLen,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Dataset_LineDashGap, new FusionChartAttributeDescriptor { 
      Name = Dataset_LineDashGap,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Dataset_ShowPlotBorder, new FusionChartAttributeDescriptor { 
      Name = Dataset_ShowPlotBorder,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie3D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSArea | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Dataset_PlotBorderColor, new FusionChartAttributeDescriptor { 
      Name = Dataset_PlotBorderColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie3D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSArea | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Dataset_PlotBorderThickness, new FusionChartAttributeDescriptor { 
      Name = Dataset_PlotBorderThickness,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie3D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSArea | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Dataset_PlotBorderAlpha, new FusionChartAttributeDescriptor { 
      Name = Dataset_PlotBorderAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie3D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSArea | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Chart_CompactDataMode, new FusionChartAttributeDescriptor { 
      Name = Chart_CompactDataMode,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_DataSeparator, new FusionChartAttributeDescriptor { 
      Name = Chart_DataSeparator,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_Axis, new FusionChartAttributeDescriptor { 
      Name = Chart_Axis,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_LogBase, new FusionChartAttributeDescriptor { 
      Name = Chart_LogBase,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_NumMinorLogDivLines, new FusionChartAttributeDescriptor { 
      Name = Chart_NumMinorLogDivLines,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_DynamicAxis, new FusionChartAttributeDescriptor { 
      Name = Chart_DynamicAxis,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_DivIntervalHints, new FusionChartAttributeDescriptor { 
      Name = Chart_DivIntervalHints,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_AllowPinMode, new FusionChartAttributeDescriptor { 
      Name = Chart_AllowPinMode,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_NumVisibleLabels, new FusionChartAttributeDescriptor { 
      Name = Chart_NumVisibleLabels,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_AnchorMinRenderDistance, new FusionChartAttributeDescriptor { 
      Name = Chart_AnchorMinRenderDistance,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowVDivLines, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowVDivLines,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_DisplayStartIndex, new FusionChartAttributeDescriptor { 
      Name = Chart_DisplayStartIndex,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_DisplayEndIndex, new FusionChartAttributeDescriptor { 
      Name = Chart_DisplayEndIndex,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_DrawToolbarButtons, new FusionChartAttributeDescriptor { 
      Name = Chart_DrawToolbarButtons,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_PixelsPerPoint, new FusionChartAttributeDescriptor { 
      Name = Chart_PixelsPerPoint,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_PaletteThemeColor, new FusionChartAttributeDescriptor { 
      Name = Chart_PaletteThemeColor,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ToolbarButtonColor, new FusionChartAttributeDescriptor { 
      Name = Chart_ToolbarButtonColor,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ToolbarButtonFontColor, new FusionChartAttributeDescriptor { 
      Name = Chart_ToolbarButtonFontColor,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ZoomPaneBorderColor, new FusionChartAttributeDescriptor { 
      Name = Chart_ZoomPaneBorderColor,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ZoomPaneBgColor, new FusionChartAttributeDescriptor { 
      Name = Chart_ZoomPaneBgColor,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ZoomPaneBgAlpha, new FusionChartAttributeDescriptor { 
      Name = Chart_ZoomPaneBgAlpha,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_PinLineThicknessDelta, new FusionChartAttributeDescriptor { 
      Name = Chart_PinLineThicknessDelta,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_PinPaneBorderColor, new FusionChartAttributeDescriptor { 
      Name = Chart_PinPaneBorderColor,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_PinPaneBgColor, new FusionChartAttributeDescriptor { 
      Name = Chart_PinPaneBgColor,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_PinPaneBgAlpha, new FusionChartAttributeDescriptor { 
      Name = Chart_PinPaneBgAlpha,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ToolTipBarColor, new FusionChartAttributeDescriptor { 
      Name = Chart_ToolTipBarColor,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_MouseCursorColor, new FusionChartAttributeDescriptor { 
      Name = Chart_MouseCursorColor,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_BtnResetChartTitle, new FusionChartAttributeDescriptor { 
      Name = Chart_BtnResetChartTitle,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_BtnZoomOutTitle, new FusionChartAttributeDescriptor { 
      Name = Chart_BtnZoomOutTitle,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_BtnSwitchtoZoomModeTitle, new FusionChartAttributeDescriptor { 
      Name = Chart_BtnSwitchtoZoomModeTitle,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_BtnSwitchToPinModeTitle, new FusionChartAttributeDescriptor { 
      Name = Chart_BtnSwitchToPinModeTitle,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowToolBarButtonTooltext, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowToolBarButtonTooltext,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_BtnResetChartTooltext, new FusionChartAttributeDescriptor { 
      Name = Chart_BtnResetChartTooltext,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_BtnSwitchToPinModeTooltext, new FusionChartAttributeDescriptor { 
      Name = Chart_BtnSwitchToPinModeTooltext,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ZoomOutMenuItemLabel, new FusionChartAttributeDescriptor { 
      Name = Chart_ZoomOutMenuItemLabel,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ResetChartMenuItemLabel, new FusionChartAttributeDescriptor { 
      Name = Chart_ResetChartMenuItemLabel,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ZoomModeMenuItemLabel, new FusionChartAttributeDescriptor { 
      Name = Chart_ZoomModeMenuItemLabel,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_PinModeMenuItemLabel, new FusionChartAttributeDescriptor { 
      Name = Chart_PinModeMenuItemLabel,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ToolBarBtnTextVMargin, new FusionChartAttributeDescriptor { 
      Name = Chart_ToolBarBtnTextVMargin,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ToolBarBtnTextHMargin, new FusionChartAttributeDescriptor { 
      Name = Chart_ToolBarBtnTextHMargin,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ToolBarBtnHPadding, new FusionChartAttributeDescriptor { 
      Name = Chart_ToolBarBtnHPadding,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ToolBarBtnVPadding, new FusionChartAttributeDescriptor { 
      Name = Chart_ToolBarBtnVPadding,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ScrollColor, new FusionChartAttributeDescriptor { 
      Name = Chart_ScrollColor,
      Charts = FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ScrollHeight, new FusionChartAttributeDescriptor { 
      Name = Chart_ScrollHeight,
      Charts = FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ScrollPadding, new FusionChartAttributeDescriptor { 
      Name = Chart_ScrollPadding,
      Charts = FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ScrollBtnWidth, new FusionChartAttributeDescriptor { 
      Name = Chart_ScrollBtnWidth,
      Charts = FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ScrollBtnPadding, new FusionChartAttributeDescriptor { 
      Name = Chart_ScrollBtnPadding,
      Charts = FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Set_StartIndex, new FusionChartAttributeDescriptor { 
      Name = Set_StartIndex,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Set,
    } },
  
    { Set_EndIndex, new FusionChartAttributeDescriptor { 
      Name = Set_EndIndex,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Set,
    } },
  
    { Set_DisplayAlways, new FusionChartAttributeDescriptor { 
      Name = Set_DisplayAlways,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Set,
    } },
  
    { Set_DisplayWhenCount, new FusionChartAttributeDescriptor { 
      Name = Set_DisplayWhenCount,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Set,
    } },
  
    { Set_ShowOnTop, new FusionChartAttributeDescriptor { 
      Name = Set_ShowOnTop,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Set,
    } },
  
    { Set_Thickness, new FusionChartAttributeDescriptor { 
      Name = Set_Thickness,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.ZoomLine,
      Element = FusionChartElementType.Set,
    } },
  
    { Set_ValueOnTop, new FusionChartAttributeDescriptor { 
      Name = Set_ValueOnTop,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Set,
    } },
  
    { Chart_MaxBarHeight, new FusionChartAttributeDescriptor { 
      Name = Chart_MaxBarHeight,
      Charts = FusionChartType.MSBar3D | FusionChartType.StackedBar3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_BarDepth, new FusionChartAttributeDescriptor { 
      Name = Chart_BarDepth,
      Charts = FusionChartType.MSBar3D | FusionChartType.StackedBar3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_OverlapBars, new FusionChartAttributeDescriptor { 
      Name = Chart_OverlapBars,
      Charts = FusionChartType.MSBar3D | FusionChartType.StackedBar3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowSum, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowSum,
      Charts = FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Doughnut2D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_Stack100Percent, new FusionChartAttributeDescriptor { 
      Name = Chart_Stack100Percent,
      Charts = FusionChartType.Doughnut2D | FusionChartType.StackedColumn3D | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_UsePercentDistribution, new FusionChartAttributeDescriptor { 
      Name = Chart_UsePercentDistribution,
      Charts = FusionChartType.Marimekko,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowXAxisPercentValues, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowXAxisPercentValues,
      Charts = FusionChartType.Marimekko,
      Element = FusionChartElementType.Chart,
    } },
  
    { Category_WidthPercent, new FusionChartAttributeDescriptor { 
      Name = Category_WidthPercent,
      Charts = FusionChartType.Marimekko,
      Element = FusionChartElementType.Category,
    } },
  
    { Chart_Animate3D, new FusionChartAttributeDescriptor { 
      Name = Chart_Animate3D,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ExeTime, new FusionChartAttributeDescriptor { 
      Name = Chart_ExeTime,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_XAxisTickColor, new FusionChartAttributeDescriptor { 
      Name = Chart_XAxisTickColor,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_XAxisTickAlpha, new FusionChartAttributeDescriptor { 
      Name = Chart_XAxisTickAlpha,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_XAxisTickThickness, new FusionChartAttributeDescriptor { 
      Name = Chart_XAxisTickThickness,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_Is2D, new FusionChartAttributeDescriptor { 
      Name = Chart_Is2D,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_Clustered, new FusionChartAttributeDescriptor { 
      Name = Chart_Clustered,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ChartOrder, new FusionChartAttributeDescriptor { 
      Name = Chart_ChartOrder,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ChartOnTop, new FusionChartAttributeDescriptor { 
      Name = Chart_ChartOnTop,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_AutoScaling, new FusionChartAttributeDescriptor { 
      Name = Chart_AutoScaling,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_AllowScaling, new FusionChartAttributeDescriptor { 
      Name = Chart_AllowScaling,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_StartAngX, new FusionChartAttributeDescriptor { 
      Name = Chart_StartAngX,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_StartAngY, new FusionChartAttributeDescriptor { 
      Name = Chart_StartAngY,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_EndAngX, new FusionChartAttributeDescriptor { 
      Name = Chart_EndAngX,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_EndAngY, new FusionChartAttributeDescriptor { 
      Name = Chart_EndAngY,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_CameraAngX, new FusionChartAttributeDescriptor { 
      Name = Chart_CameraAngX,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_CameraAngY, new FusionChartAttributeDescriptor { 
      Name = Chart_CameraAngY,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_LightAngX, new FusionChartAttributeDescriptor { 
      Name = Chart_LightAngX,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_LightAngY, new FusionChartAttributeDescriptor { 
      Name = Chart_LightAngY,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_Intensity, new FusionChartAttributeDescriptor { 
      Name = Chart_Intensity,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_DynamicShading, new FusionChartAttributeDescriptor { 
      Name = Chart_DynamicShading,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_Bright2D, new FusionChartAttributeDescriptor { 
      Name = Chart_Bright2D,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_AllowRotation, new FusionChartAttributeDescriptor { 
      Name = Chart_AllowRotation,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ConstrainVerticalRotation, new FusionChartAttributeDescriptor { 
      Name = Chart_ConstrainVerticalRotation,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_MinVerticalRotAngle, new FusionChartAttributeDescriptor { 
      Name = Chart_MinVerticalRotAngle,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_MaxVerticalRotAngle, new FusionChartAttributeDescriptor { 
      Name = Chart_MaxVerticalRotAngle,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ConstrainHorizontalRotation, new FusionChartAttributeDescriptor { 
      Name = Chart_ConstrainHorizontalRotation,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_MinHorizontalRotAngle, new FusionChartAttributeDescriptor { 
      Name = Chart_MinHorizontalRotAngle,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_MaxHorizontalRotAngle, new FusionChartAttributeDescriptor { 
      Name = Chart_MaxHorizontalRotAngle,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ZDepth, new FusionChartAttributeDescriptor { 
      Name = Chart_ZDepth,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ZGapPlot, new FusionChartAttributeDescriptor { 
      Name = Chart_ZGapPlot,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_YzWallDepth, new FusionChartAttributeDescriptor { 
      Name = Chart_YzWallDepth,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ZxWallDepth, new FusionChartAttributeDescriptor { 
      Name = Chart_ZxWallDepth,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_XyWallDepth, new FusionChartAttributeDescriptor { 
      Name = Chart_XyWallDepth,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_DivLineEffect, new FusionChartAttributeDescriptor { 
      Name = Chart_DivLineEffect,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ZeroPlaneMesh, new FusionChartAttributeDescriptor { 
      Name = Chart_ZeroPlaneMesh,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_YLabelGap, new FusionChartAttributeDescriptor { 
      Name = Chart_YLabelGap,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_XLabelGap, new FusionChartAttributeDescriptor { 
      Name = Chart_XLabelGap,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Dataset_RenderAs, new FusionChartAttributeDescriptor { 
      Name = Dataset_RenderAs,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Chart_AreaOverColumns, new FusionChartAttributeDescriptor { 
      Name = Chart_AreaOverColumns,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Dataset_ParentYAxis, new FusionChartAttributeDescriptor { 
      Name = Dataset_ParentYAxis,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Chart_, new FusionChartAttributeDescriptor { 
      Name = Chart_,
      Charts = FusionChartType.Pie3D | FusionChartType.MSColumnLine3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_SYAxisMinValue, new FusionChartAttributeDescriptor { 
      Name = Chart_SYAxisMinValue,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_SYAxisMaxValue, new FusionChartAttributeDescriptor { 
      Name = Chart_SYAxisMaxValue,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_SetAdaptiveSYMin, new FusionChartAttributeDescriptor { 
      Name = Chart_SetAdaptiveSYMin,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_SyncAxisLimits, new FusionChartAttributeDescriptor { 
      Name = Chart_SyncAxisLimits,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowPZeroPlaneValue, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowPZeroPlaneValue,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowSZeroPlaneValue, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowSZeroPlaneValue,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_SScaleRecursively, new FusionChartAttributeDescriptor { 
      Name = Chart_SScaleRecursively,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_SMaxScaleRecursion, new FusionChartAttributeDescriptor { 
      Name = Chart_SMaxScaleRecursion,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_SScaleSeparator, new FusionChartAttributeDescriptor { 
      Name = Chart_SScaleSeparator,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ForceSYAxisValueDecimals, new FusionChartAttributeDescriptor { 
      Name = Chart_ForceSYAxisValueDecimals,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_SFormatNumber, new FusionChartAttributeDescriptor { 
      Name = Chart_SFormatNumber,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_SFormatNumberScale, new FusionChartAttributeDescriptor { 
      Name = Chart_SFormatNumberScale,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_SDefaultNumberScale, new FusionChartAttributeDescriptor { 
      Name = Chart_SDefaultNumberScale,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_SNumberScaleUnit, new FusionChartAttributeDescriptor { 
      Name = Chart_SNumberScaleUnit,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_SNumberScaleValue, new FusionChartAttributeDescriptor { 
      Name = Chart_SNumberScaleValue,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_SNumberPrefix, new FusionChartAttributeDescriptor { 
      Name = Chart_SNumberPrefix,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_SNumberSuffix, new FusionChartAttributeDescriptor { 
      Name = Chart_SNumberSuffix,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_SDecimals, new FusionChartAttributeDescriptor { 
      Name = Chart_SDecimals,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_SYAxisValueDecimals, new FusionChartAttributeDescriptor { 
      Name = Chart_SYAxisValueDecimals,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { Lineset_IncludeInLegend, new FusionChartAttributeDescriptor { 
      Name = Lineset_IncludeInLegend,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { Lineset_SeriesName, new FusionChartAttributeDescriptor { 
      Name = Lineset_SeriesName,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { Lineset_Color, new FusionChartAttributeDescriptor { 
      Name = Lineset_Color,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { Lineset_Alpha, new FusionChartAttributeDescriptor { 
      Name = Lineset_Alpha,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { Lineset_ShowValues, new FusionChartAttributeDescriptor { 
      Name = Lineset_ShowValues,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { Lineset_ValuePosition, new FusionChartAttributeDescriptor { 
      Name = Lineset_ValuePosition,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { Lineset_Dashed, new FusionChartAttributeDescriptor { 
      Name = Lineset_Dashed,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { Lineset_LineDashLen, new FusionChartAttributeDescriptor { 
      Name = Lineset_LineDashLen,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { Lineset_LineDashGap, new FusionChartAttributeDescriptor { 
      Name = Lineset_LineDashGap,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { Lineset_LineThickness, new FusionChartAttributeDescriptor { 
      Name = Lineset_LineThickness,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { Lineset_DrawAnchors, new FusionChartAttributeDescriptor { 
      Name = Lineset_DrawAnchors,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { Lineset_AnchorSides, new FusionChartAttributeDescriptor { 
      Name = Lineset_AnchorSides,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { Lineset_AnchorRadius, new FusionChartAttributeDescriptor { 
      Name = Lineset_AnchorRadius,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { Lineset_AnchorBorderColor, new FusionChartAttributeDescriptor { 
      Name = Lineset_AnchorBorderColor,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { Lineset_AnchorBorderThickness, new FusionChartAttributeDescriptor { 
      Name = Lineset_AnchorBorderThickness,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { Lineset_AnchorBgColor, new FusionChartAttributeDescriptor { 
      Name = Lineset_AnchorBgColor,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { Lineset_AnchorBgAlpha, new FusionChartAttributeDescriptor { 
      Name = Lineset_AnchorBgAlpha,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { Lineset_AnchorAlpha, new FusionChartAttributeDescriptor { 
      Name = Lineset_AnchorAlpha,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { Chart_ClipBubbles, new FusionChartAttributeDescriptor { 
      Name = Chart_ClipBubbles,
      Charts = FusionChartType.Pie3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_NegativeColor, new FusionChartAttributeDescriptor { 
      Name = Chart_NegativeColor,
      Charts = FusionChartType.Pie3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_XAxisLabelMode, new FusionChartAttributeDescriptor { 
      Name = Chart_XAxisLabelMode,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowXAxisValues, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowXAxisValues,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowVLimits, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowVLimits,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowVDivLineValues, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowVDivLineValues,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_XAxisMinValue, new FusionChartAttributeDescriptor { 
      Name = Chart_XAxisMinValue,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_XAxisMaxValue, new FusionChartAttributeDescriptor { 
      Name = Chart_XAxisMaxValue,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_BubbleScale, new FusionChartAttributeDescriptor { 
      Name = Chart_BubbleScale,
      Charts = FusionChartType.Pie3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_XAxisValuesStep, new FusionChartAttributeDescriptor { 
      Name = Chart_XAxisValuesStep,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_AdjustVDiv, new FusionChartAttributeDescriptor { 
      Name = Chart_AdjustVDiv,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_SetAdaptiveXMin, new FusionChartAttributeDescriptor { 
      Name = Chart_SetAdaptiveXMin,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowRegressionLine, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowRegressionLine,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowYOnX, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowYOnX,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_RegressionLineColor, new FusionChartAttributeDescriptor { 
      Name = Chart_RegressionLineColor,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_RegressionLineThickness, new FusionChartAttributeDescriptor { 
      Name = Chart_RegressionLineThickness,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_RegressionLineAlpha, new FusionChartAttributeDescriptor { 
      Name = Chart_RegressionLineAlpha,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_DrawQuadrant, new FusionChartAttributeDescriptor { 
      Name = Chart_DrawQuadrant,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_QuadrantXVal, new FusionChartAttributeDescriptor { 
      Name = Chart_QuadrantXVal,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_QuadrantYVal, new FusionChartAttributeDescriptor { 
      Name = Chart_QuadrantYVal,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_QuadrantLineColor, new FusionChartAttributeDescriptor { 
      Name = Chart_QuadrantLineColor,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_QuadrantLineThickness, new FusionChartAttributeDescriptor { 
      Name = Chart_QuadrantLineThickness,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_QuadrantLineAlpha, new FusionChartAttributeDescriptor { 
      Name = Chart_QuadrantLineAlpha,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_QuadrantLineIsDashed, new FusionChartAttributeDescriptor { 
      Name = Chart_QuadrantLineIsDashed,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_QuadrantLineDashLen, new FusionChartAttributeDescriptor { 
      Name = Chart_QuadrantLineDashLen,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_QuadrantLineDashGap, new FusionChartAttributeDescriptor { 
      Name = Chart_QuadrantLineDashGap,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_QuadrantLabelTL, new FusionChartAttributeDescriptor { 
      Name = Chart_QuadrantLabelTL,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_QuadrantLabelTR, new FusionChartAttributeDescriptor { 
      Name = Chart_QuadrantLabelTR,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_QuadrantLabelBL, new FusionChartAttributeDescriptor { 
      Name = Chart_QuadrantLabelBL,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_QuadrantLabelBR, new FusionChartAttributeDescriptor { 
      Name = Chart_QuadrantLabelBR,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_QuadrantLabelPadding, new FusionChartAttributeDescriptor { 
      Name = Chart_QuadrantLabelPadding,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_NumVDivlines, new FusionChartAttributeDescriptor { 
      Name = Chart_NumVDivlines,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_VDivlineColor, new FusionChartAttributeDescriptor { 
      Name = Chart_VDivlineColor,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_VDivlineThickness, new FusionChartAttributeDescriptor { 
      Name = Chart_VDivlineThickness,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_VDivlineAlpha, new FusionChartAttributeDescriptor { 
      Name = Chart_VDivlineAlpha,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_VDivlineIsDashed, new FusionChartAttributeDescriptor { 
      Name = Chart_VDivlineIsDashed,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_VDivlineDashLen, new FusionChartAttributeDescriptor { 
      Name = Chart_VDivlineDashLen,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_VDivlineDashGap, new FusionChartAttributeDescriptor { 
      Name = Chart_VDivlineDashGap,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ShowVZeroPlane, new FusionChartAttributeDescriptor { 
      Name = Chart_ShowVZeroPlane,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_VZeroPlaneColor, new FusionChartAttributeDescriptor { 
      Name = Chart_VZeroPlaneColor,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_VZeroPlaneThickness, new FusionChartAttributeDescriptor { 
      Name = Chart_VZeroPlaneThickness,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_VZeroPlaneAlpha, new FusionChartAttributeDescriptor { 
      Name = Chart_VZeroPlaneAlpha,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_YFormatNumber, new FusionChartAttributeDescriptor { 
      Name = Chart_YFormatNumber,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_XFormatNumber, new FusionChartAttributeDescriptor { 
      Name = Chart_XFormatNumber,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_YFormatNumberScale, new FusionChartAttributeDescriptor { 
      Name = Chart_YFormatNumberScale,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_XFormatNumberScale, new FusionChartAttributeDescriptor { 
      Name = Chart_XFormatNumberScale,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_YDefaultNumberScale, new FusionChartAttributeDescriptor { 
      Name = Chart_YDefaultNumberScale,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_XDefaultNumberScale, new FusionChartAttributeDescriptor { 
      Name = Chart_XDefaultNumberScale,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_YNumberScaleUnit, new FusionChartAttributeDescriptor { 
      Name = Chart_YNumberScaleUnit,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_XNumberScaleUnit, new FusionChartAttributeDescriptor { 
      Name = Chart_XNumberScaleUnit,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_XScaleRecursively, new FusionChartAttributeDescriptor { 
      Name = Chart_XScaleRecursively,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_XMaxScaleRecursion, new FusionChartAttributeDescriptor { 
      Name = Chart_XMaxScaleRecursion,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_XScaleSeparator, new FusionChartAttributeDescriptor { 
      Name = Chart_XScaleSeparator,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_YNumberScaleValue, new FusionChartAttributeDescriptor { 
      Name = Chart_YNumberScaleValue,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_XNumberScaleValue, new FusionChartAttributeDescriptor { 
      Name = Chart_XNumberScaleValue,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_YNumberPrefix, new FusionChartAttributeDescriptor { 
      Name = Chart_YNumberPrefix,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_XNumberPrefix, new FusionChartAttributeDescriptor { 
      Name = Chart_XNumberPrefix,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_YNumberSuffix, new FusionChartAttributeDescriptor { 
      Name = Chart_YNumberSuffix,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_XNumberSuffix, new FusionChartAttributeDescriptor { 
      Name = Chart_XNumberSuffix,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ForceXAxisValueDecimals, new FusionChartAttributeDescriptor { 
      Name = Chart_ForceXAxisValueDecimals,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_XAxisValueDecimals, new FusionChartAttributeDescriptor { 
      Name = Chart_XAxisValueDecimals,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Categories_VerticalLineColor, new FusionChartAttributeDescriptor { 
      Name = Categories_VerticalLineColor,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Categories,
    } },
  
    { Categories_VerticalLineThickness, new FusionChartAttributeDescriptor { 
      Name = Categories_VerticalLineThickness,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Categories,
    } },
  
    { Categories_VerticalLineAlpha, new FusionChartAttributeDescriptor { 
      Name = Categories_VerticalLineAlpha,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Categories,
    } },
  
    { Categories_VerticalLineDashed, new FusionChartAttributeDescriptor { 
      Name = Categories_VerticalLineDashed,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Categories,
    } },
  
    { Categories_VerticalLineDashLen, new FusionChartAttributeDescriptor { 
      Name = Categories_VerticalLineDashLen,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Categories,
    } },
  
    { Categories_VerticalLineDashGap, new FusionChartAttributeDescriptor { 
      Name = Categories_VerticalLineDashGap,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Categories,
    } },
  
    { Category_X, new FusionChartAttributeDescriptor { 
      Name = Category_X,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Category,
    } },
  
    { Category_ShowVerticalLine, new FusionChartAttributeDescriptor { 
      Name = Category_ShowVerticalLine,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Category,
    } },
  
    { Category_LineDashed, new FusionChartAttributeDescriptor { 
      Name = Category_LineDashed,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Category,
    } },
  
    { Dataset_PlotFillAlpha, new FusionChartAttributeDescriptor { 
      Name = Dataset_PlotFillAlpha,
      Charts = FusionChartType.Pie3D,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Dataset_ShowRegressionLine, new FusionChartAttributeDescriptor { 
      Name = Dataset_ShowRegressionLine,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Dataset_ShowYOnX, new FusionChartAttributeDescriptor { 
      Name = Dataset_ShowYOnX,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Dataset_RegressionLineColor, new FusionChartAttributeDescriptor { 
      Name = Dataset_RegressionLineColor,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Dataset_RegressionLineThickness, new FusionChartAttributeDescriptor { 
      Name = Dataset_RegressionLineThickness,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Dataset_RegressionLineAlpha, new FusionChartAttributeDescriptor { 
      Name = Dataset_RegressionLineAlpha,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Set_X, new FusionChartAttributeDescriptor { 
      Name = Set_X,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Set,
    } },
  
    { Set_Y, new FusionChartAttributeDescriptor { 
      Name = Set_Y,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Set,
    } },
  
    { Set_Z, new FusionChartAttributeDescriptor { 
      Name = Set_Z,
      Charts = FusionChartType.Pie3D,
      Element = FusionChartElementType.Set,
    } },
  
    { Set_Name, new FusionChartAttributeDescriptor { 
      Name = Set_Name,
      Charts = FusionChartType.Pie3D,
      Element = FusionChartElementType.Set,
    } },
  
    { Set_StartValue, new FusionChartAttributeDescriptor { 
      Name = Set_StartValue,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Set,
    } },
  
    { Set_EndValue, new FusionChartAttributeDescriptor { 
      Name = Set_EndValue,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Set,
    } },
  
    { Set_IsTrendZone, new FusionChartAttributeDescriptor { 
      Name = Set_IsTrendZone,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Set,
    } },
  
    { Set_DashLen, new FusionChartAttributeDescriptor { 
      Name = Set_DashLen,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Set,
    } },
  
    { Set_DashGap, new FusionChartAttributeDescriptor { 
      Name = Set_DashGap,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Set,
    } },
  
    { Dataset_DrawLine, new FusionChartAttributeDescriptor { 
      Name = Dataset_DrawLine,
      Charts = FusionChartType.Column3D,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Dataset_LineColor, new FusionChartAttributeDescriptor { 
      Name = Dataset_LineColor,
      Charts = FusionChartType.Column3D,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Dataset_LineAlpha, new FusionChartAttributeDescriptor { 
      Name = Dataset_LineAlpha,
      Charts = FusionChartType.Column3D,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Dataset_LineDashed, new FusionChartAttributeDescriptor { 
      Name = Dataset_LineDashed,
      Charts = FusionChartType.Column3D,
      Element = FusionChartElementType.Dataset,
    } },
  
    { Chart_NumVisiblePlot, new FusionChartAttributeDescriptor { 
      Name = Chart_NumVisiblePlot,
      Charts = FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { Chart_ScrollToEnd, new FusionChartAttributeDescriptor { 
      Name = Chart_ScrollToEnd,
      Charts = FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    };
   }
}
