
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
         
    { "chart_animation", new FusionChartAttributeDescriptor { 
      Name = Chart_Animation,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_palette", new FusionChartAttributeDescriptor { 
      Name = Chart_Palette,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_palettecolors", new FusionChartAttributeDescriptor { 
      Name = Chart_PaletteColors,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showaboutmenuitem", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowAboutMenuItem,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_aboutmenuitemlabel", new FusionChartAttributeDescriptor { 
      Name = Chart_AboutMenuItemLabel,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_aboutmenuitemlink", new FusionChartAttributeDescriptor { 
      Name = Chart_AboutMenuItemLink,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showzeropies", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowZeroPies,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showpercentvalues", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowPercentValues,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D | FusionChartType.StackedColumn3D | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showpercentintooltip", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowPercentInToolTip,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D | FusionChartType.StackedColumn3D | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showlabels", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowLabels,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showvalues", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowValues,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_labelsepchar", new FusionChartAttributeDescriptor { 
      Name = Chart_LabelSepChar,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_defaultanimation", new FusionChartAttributeDescriptor { 
      Name = Chart_DefaultAnimation,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_clickurl", new FusionChartAttributeDescriptor { 
      Name = Chart_ClickURL,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_caption", new FusionChartAttributeDescriptor { 
      Name = Chart_Caption,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_subcaption", new FusionChartAttributeDescriptor { 
      Name = Chart_SubCaption,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showborder", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowBorder,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_bordercolor", new FusionChartAttributeDescriptor { 
      Name = Chart_BorderColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_borderthickness", new FusionChartAttributeDescriptor { 
      Name = Chart_BorderThickness,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_borderalpha", new FusionChartAttributeDescriptor { 
      Name = Chart_BorderAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_bgcolor", new FusionChartAttributeDescriptor { 
      Name = Chart_BgColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_bgalpha", new FusionChartAttributeDescriptor { 
      Name = Chart_BgAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_bgratio", new FusionChartAttributeDescriptor { 
      Name = Chart_BgRatio,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_bgangle", new FusionChartAttributeDescriptor { 
      Name = Chart_BgAngle,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_bgimage", new FusionChartAttributeDescriptor { 
      Name = Chart_BgImage,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_bgimagealpha", new FusionChartAttributeDescriptor { 
      Name = Chart_BgImageAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_bgimagedisplaymode", new FusionChartAttributeDescriptor { 
      Name = Chart_BgImageDisplayMode,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_bgimagevalign", new FusionChartAttributeDescriptor { 
      Name = Chart_BgImageVAlign,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_bgimagehalign", new FusionChartAttributeDescriptor { 
      Name = Chart_BgImageHAlign,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_bgimagescale", new FusionChartAttributeDescriptor { 
      Name = Chart_BgImageScale,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_logourl", new FusionChartAttributeDescriptor { 
      Name = Chart_LogoURL,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_logoposition", new FusionChartAttributeDescriptor { 
      Name = Chart_LogoPosition,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_logoalpha", new FusionChartAttributeDescriptor { 
      Name = Chart_LogoAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_logoscale", new FusionChartAttributeDescriptor { 
      Name = Chart_LogoScale,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_logolink", new FusionChartAttributeDescriptor { 
      Name = Chart_LogoLink,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showplotborder", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowPlotBorder,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_plotbordercolor", new FusionChartAttributeDescriptor { 
      Name = Chart_PlotBorderColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_plotborderthickness", new FusionChartAttributeDescriptor { 
      Name = Chart_PlotBorderThickness,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_plotborderalpha", new FusionChartAttributeDescriptor { 
      Name = Chart_PlotBorderAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_plotfillalpha", new FusionChartAttributeDescriptor { 
      Name = Chart_PlotFillAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showshadow", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowShadow,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_use3dlighting", new FusionChartAttributeDescriptor { 
      Name = Chart_Use3DLighting,
      Charts = FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Doughnut2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn3D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.StackedBar3D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_radius3d", new FusionChartAttributeDescriptor { 
      Name = Chart_Radius3D,
      Charts = FusionChartType.Pie2D | FusionChartType.Doughnut2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_slicingdistance", new FusionChartAttributeDescriptor { 
      Name = Chart_SlicingDistance,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_pieradius", new FusionChartAttributeDescriptor { 
      Name = Chart_PieRadius,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_startingangle", new FusionChartAttributeDescriptor { 
      Name = Chart_StartingAngle,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_enablerotation", new FusionChartAttributeDescriptor { 
      Name = Chart_EnableRotation,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_enablesmartlabels", new FusionChartAttributeDescriptor { 
      Name = Chart_EnableSmartLabels,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_skipoverlaplabels", new FusionChartAttributeDescriptor { 
      Name = Chart_SkipOverlapLabels,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_issmartlineslanted", new FusionChartAttributeDescriptor { 
      Name = Chart_IsSmartLineSlanted,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_smartlinecolor", new FusionChartAttributeDescriptor { 
      Name = Chart_SmartLineColor,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_smartlinethickness", new FusionChartAttributeDescriptor { 
      Name = Chart_SmartLineThickness,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_smartlinealpha", new FusionChartAttributeDescriptor { 
      Name = Chart_SmartLineAlpha,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_labeldistance", new FusionChartAttributeDescriptor { 
      Name = Chart_LabelDistance,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_smartlabelclearance", new FusionChartAttributeDescriptor { 
      Name = Chart_SmartLabelClearance,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_managelabeloverflow", new FusionChartAttributeDescriptor { 
      Name = Chart_ManageLabelOverflow,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_useellipseswhenoverflow", new FusionChartAttributeDescriptor { 
      Name = Chart_UseEllipsesWhenOverflow,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_formatnumber", new FusionChartAttributeDescriptor { 
      Name = Chart_FormatNumber,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_formatnumberscale", new FusionChartAttributeDescriptor { 
      Name = Chart_FormatNumberScale,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_defaultnumberscale", new FusionChartAttributeDescriptor { 
      Name = Chart_DefaultNumberScale,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_numberscaleunit", new FusionChartAttributeDescriptor { 
      Name = Chart_NumberScaleUnit,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_numberscalevalue", new FusionChartAttributeDescriptor { 
      Name = Chart_NumberScaleValue,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_scalerecursively", new FusionChartAttributeDescriptor { 
      Name = Chart_ScaleRecursively,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_maxscalerecursion", new FusionChartAttributeDescriptor { 
      Name = Chart_MaxScaleRecursion,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_scaleseparator", new FusionChartAttributeDescriptor { 
      Name = Chart_ScaleSeparator,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_numberprefix", new FusionChartAttributeDescriptor { 
      Name = Chart_NumberPrefix,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_numbersuffix", new FusionChartAttributeDescriptor { 
      Name = Chart_NumberSuffix,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_decimalseparator", new FusionChartAttributeDescriptor { 
      Name = Chart_DecimalSeparator,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_thousandseparator", new FusionChartAttributeDescriptor { 
      Name = Chart_ThousandSeparator,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_thousandseparatorposition", new FusionChartAttributeDescriptor { 
      Name = Chart_ThousandSeparatorPosition,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_indecimalseparator", new FusionChartAttributeDescriptor { 
      Name = Chart_InDecimalSeparator,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_inthousandseparator", new FusionChartAttributeDescriptor { 
      Name = Chart_InThousandSeparator,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_decimals", new FusionChartAttributeDescriptor { 
      Name = Chart_Decimals,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_forcedecimals", new FusionChartAttributeDescriptor { 
      Name = Chart_ForceDecimals,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_basefont", new FusionChartAttributeDescriptor { 
      Name = Chart_BaseFont,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_basefontsize", new FusionChartAttributeDescriptor { 
      Name = Chart_BaseFontSize,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_basefontcolor", new FusionChartAttributeDescriptor { 
      Name = Chart_BaseFontColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showlegend", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowLegend,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_legendposition", new FusionChartAttributeDescriptor { 
      Name = Chart_LegendPosition,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_legendcaption", new FusionChartAttributeDescriptor { 
      Name = Chart_LegendCaption,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_legendiconscale", new FusionChartAttributeDescriptor { 
      Name = Chart_LegendIconScale,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_legendbgcolor", new FusionChartAttributeDescriptor { 
      Name = Chart_LegendBgColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_legendbgalpha", new FusionChartAttributeDescriptor { 
      Name = Chart_LegendBgAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_legendbordercolor", new FusionChartAttributeDescriptor { 
      Name = Chart_LegendBorderColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_legendborderthickness", new FusionChartAttributeDescriptor { 
      Name = Chart_LegendBorderThickness,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_legendborderalpha", new FusionChartAttributeDescriptor { 
      Name = Chart_LegendBorderAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_legendshadow", new FusionChartAttributeDescriptor { 
      Name = Chart_LegendShadow,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_legendallowdrag", new FusionChartAttributeDescriptor { 
      Name = Chart_LegendAllowDrag,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_legendscrollbgcolor", new FusionChartAttributeDescriptor { 
      Name = Chart_LegendScrollBgColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_legendscrollbarcolor", new FusionChartAttributeDescriptor { 
      Name = Chart_LegendScrollBarColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_legendscrollbtncolor", new FusionChartAttributeDescriptor { 
      Name = Chart_LegendScrollBtnColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_reverselegend", new FusionChartAttributeDescriptor { 
      Name = Chart_ReverseLegend,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_interactivelegend", new FusionChartAttributeDescriptor { 
      Name = Chart_InteractiveLegend,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_legendnumcolumns", new FusionChartAttributeDescriptor { 
      Name = Chart_LegendNumColumns,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_minimisewrappinginlegend", new FusionChartAttributeDescriptor { 
      Name = Chart_MinimiseWrappingInLegend,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showtooltip", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowToolTip,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_tooltipbgcolor", new FusionChartAttributeDescriptor { 
      Name = Chart_ToolTipBgColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_tooltipbordercolor", new FusionChartAttributeDescriptor { 
      Name = Chart_ToolTipBorderColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_tooltipsepchar", new FusionChartAttributeDescriptor { 
      Name = Chart_ToolTipSepChar,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showtooltipshadow", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowToolTipShadow,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_captionpadding", new FusionChartAttributeDescriptor { 
      Name = Chart_CaptionPadding,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_chartleftmargin", new FusionChartAttributeDescriptor { 
      Name = Chart_ChartLeftMargin,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_chartrightmargin", new FusionChartAttributeDescriptor { 
      Name = Chart_ChartRightMargin,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_charttopmargin", new FusionChartAttributeDescriptor { 
      Name = Chart_ChartTopMargin,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_chartbottommargin", new FusionChartAttributeDescriptor { 
      Name = Chart_ChartBottomMargin,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "set_bordercolor", new FusionChartAttributeDescriptor { 
      Name = Set_BorderColor,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Set,
    } },
  
    { "set_borderalpha", new FusionChartAttributeDescriptor { 
      Name = Set_BorderAlpha,
      Charts = FusionChartType.Pie2D | FusionChartType.Doughnut2D,
      Element = FusionChartElementType.Set,
    } },
  
    { "set_issliced", new FusionChartAttributeDescriptor { 
      Name = Set_IsSliced,
      Charts = FusionChartType.Pie2D | FusionChartType.Pie3D | FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Set,
    } },
  
    { "set_label", new FusionChartAttributeDescriptor { 
      Name = Set_Label,
      Charts = FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D,
      Element = FusionChartElementType.Set,
    } },
  
    { "set_value", new FusionChartAttributeDescriptor { 
      Name = Set_Value,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Set,
    } },
  
    { "set_displayvalue", new FusionChartAttributeDescriptor { 
      Name = Set_DisplayValue,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Set,
    } },
  
    { "set_color", new FusionChartAttributeDescriptor { 
      Name = Set_Color,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Set,
    } },
  
    { "set_link", new FusionChartAttributeDescriptor { 
      Name = Set_Link,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Set,
    } },
  
    { "set_tooltext", new FusionChartAttributeDescriptor { 
      Name = Set_ToolText,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Set,
    } },
  
    { "set_dashed", new FusionChartAttributeDescriptor { 
      Name = Set_Dashed,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Set,
    } },
  
    { "set_alpha", new FusionChartAttributeDescriptor { 
      Name = Set_Alpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Set,
    } },
  
    { "set_showlabel", new FusionChartAttributeDescriptor { 
      Name = Set_ShowLabel,
      Charts = FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D,
      Element = FusionChartElementType.Set,
    } },
  
    { "set_showvalue", new FusionChartAttributeDescriptor { 
      Name = Set_ShowValue,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Set,
    } },
  
    { "chart_labeldisplay", new FusionChartAttributeDescriptor { 
      Name = Chart_LabelDisplay,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_rotatelabels", new FusionChartAttributeDescriptor { 
      Name = Chart_RotateLabels,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_slantlabels", new FusionChartAttributeDescriptor { 
      Name = Chart_SlantLabels,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_labelstep", new FusionChartAttributeDescriptor { 
      Name = Chart_LabelStep,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_staggerlines", new FusionChartAttributeDescriptor { 
      Name = Chart_StaggerLines,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_rotatevalues", new FusionChartAttributeDescriptor { 
      Name = Chart_RotateValues,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_placevaluesinside", new FusionChartAttributeDescriptor { 
      Name = Chart_PlaceValuesInside,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSColumn3D | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedBar3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showyaxisvalues", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowYAxisValues,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showlimits", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowLimits,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showdivlinevalues", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowDivLineValues,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_yaxisvaluesstep", new FusionChartAttributeDescriptor { 
      Name = Chart_YAxisValuesStep,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_adjustdiv", new FusionChartAttributeDescriptor { 
      Name = Chart_AdjustDiv,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_rotateyaxisname", new FusionChartAttributeDescriptor { 
      Name = Chart_RotateYAxisName,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_yaxisnamewidth", new FusionChartAttributeDescriptor { 
      Name = Chart_YAxisNameWidth,
      Charts = FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_yaxisminvalue", new FusionChartAttributeDescriptor { 
      Name = Chart_YAxisMinValue,
      Charts = FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_yaxismaxvalue", new FusionChartAttributeDescriptor { 
      Name = Chart_YAxisMaxValue,
      Charts = FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_setadaptiveymin", new FusionChartAttributeDescriptor { 
      Name = Chart_SetAdaptiveYMin,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_centeryaxisname", new FusionChartAttributeDescriptor { 
      Name = Chart_CenterYaxisName,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_xaxisname", new FusionChartAttributeDescriptor { 
      Name = Chart_XAxisName,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_yaxisname", new FusionChartAttributeDescriptor { 
      Name = Chart_YAxisName,
      Charts = FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_canvasbgcolor", new FusionChartAttributeDescriptor { 
      Name = Chart_CanvasBgColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_canvasbgalpha", new FusionChartAttributeDescriptor { 
      Name = Chart_CanvasBgAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_canvasbgratio", new FusionChartAttributeDescriptor { 
      Name = Chart_CanvasBgRatio,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_canvasbgangle", new FusionChartAttributeDescriptor { 
      Name = Chart_CanvasBgAngle,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_canvasbordercolor", new FusionChartAttributeDescriptor { 
      Name = Chart_CanvasBorderColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_canvasborderthickness", new FusionChartAttributeDescriptor { 
      Name = Chart_CanvasBorderThickness,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_canvasborderalpha", new FusionChartAttributeDescriptor { 
      Name = Chart_CanvasBorderAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showvlinelabelborder", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowVLineLabelBorder,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_useroundedges", new FusionChartAttributeDescriptor { 
      Name = Chart_UseRoundEdges,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSBar2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_plotborderdashed", new FusionChartAttributeDescriptor { 
      Name = Chart_PlotBorderDashed,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_plotborderdashlen", new FusionChartAttributeDescriptor { 
      Name = Chart_PlotBorderDashLen,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_plotborderdashgap", new FusionChartAttributeDescriptor { 
      Name = Chart_PlotBorderDashGap,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_plotfillangle", new FusionChartAttributeDescriptor { 
      Name = Chart_PlotFillAngle,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_plotfillratio", new FusionChartAttributeDescriptor { 
      Name = Chart_PlotFillRatio,
      Charts = FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSBar2D | FusionChartType.Marimekko | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_plotgradientcolor", new FusionChartAttributeDescriptor { 
      Name = Chart_PlotGradientColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_numdivlines", new FusionChartAttributeDescriptor { 
      Name = Chart_NumDivLines,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_divlinecolor", new FusionChartAttributeDescriptor { 
      Name = Chart_DivLineColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_divlinethickness", new FusionChartAttributeDescriptor { 
      Name = Chart_DivLineThickness,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_divlinealpha", new FusionChartAttributeDescriptor { 
      Name = Chart_DivLineAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_divlineisdashed", new FusionChartAttributeDescriptor { 
      Name = Chart_DivLineIsDashed,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_divlinedashlen", new FusionChartAttributeDescriptor { 
      Name = Chart_DivLineDashLen,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_divlinedashgap", new FusionChartAttributeDescriptor { 
      Name = Chart_DivLineDashGap,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_zeroplanecolor", new FusionChartAttributeDescriptor { 
      Name = Chart_ZeroPlaneColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_zeroplanethickness", new FusionChartAttributeDescriptor { 
      Name = Chart_ZeroPlaneThickness,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_zeroplanealpha", new FusionChartAttributeDescriptor { 
      Name = Chart_ZeroPlaneAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showzeroplanevalue", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowZeroPlaneValue,
      Charts = FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showalternatehgridcolor", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowAlternateHGridColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_alternatehgridcolor", new FusionChartAttributeDescriptor { 
      Name = Chart_AlternateHGridColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_alternatehgridalpha", new FusionChartAttributeDescriptor { 
      Name = Chart_AlternateHGridAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_forceyaxisvaluedecimals", new FusionChartAttributeDescriptor { 
      Name = Chart_ForceYAxisValueDecimals,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_yaxisvaluedecimals", new FusionChartAttributeDescriptor { 
      Name = Chart_YAxisValueDecimals,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_outcnvbasefont", new FusionChartAttributeDescriptor { 
      Name = Chart_OutCnvBaseFont,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_outcnvbasefontsize", new FusionChartAttributeDescriptor { 
      Name = Chart_OutCnvBaseFontSize,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_outcnvbasefontcolor", new FusionChartAttributeDescriptor { 
      Name = Chart_OutCnvBaseFontColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_xaxisnamepadding", new FusionChartAttributeDescriptor { 
      Name = Chart_XAxisNamePadding,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_yaxisnamepadding", new FusionChartAttributeDescriptor { 
      Name = Chart_YAxisNamePadding,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_yaxisvaluespadding", new FusionChartAttributeDescriptor { 
      Name = Chart_YAxisValuesPadding,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_labelpadding", new FusionChartAttributeDescriptor { 
      Name = Chart_LabelPadding,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_valuepadding", new FusionChartAttributeDescriptor { 
      Name = Chart_ValuePadding,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.Marimekko | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_plotspacepercent", new FusionChartAttributeDescriptor { 
      Name = Chart_PlotSpacePercent,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSColumn3D | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_canvasleftmargin", new FusionChartAttributeDescriptor { 
      Name = Chart_CanvasLeftMargin,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_canvasrightmargin", new FusionChartAttributeDescriptor { 
      Name = Chart_CanvasRightMargin,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_canvastopmargin", new FusionChartAttributeDescriptor { 
      Name = Chart_CanvasTopMargin,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_canvasbottommargin", new FusionChartAttributeDescriptor { 
      Name = Chart_CanvasBottomMargin,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "vline_color", new FusionChartAttributeDescriptor { 
      Name = VLine_Color,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.VLine,
    } },
  
    { "vline_thickness", new FusionChartAttributeDescriptor { 
      Name = VLine_Thickness,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.VLine,
    } },
  
    { "vline_alpha", new FusionChartAttributeDescriptor { 
      Name = VLine_Alpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.VLine,
    } },
  
    { "vline_dashed", new FusionChartAttributeDescriptor { 
      Name = VLine_Dashed,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.VLine,
    } },
  
    { "vline_dashlen", new FusionChartAttributeDescriptor { 
      Name = VLine_DashLen,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.VLine,
    } },
  
    { "vline_dashgap", new FusionChartAttributeDescriptor { 
      Name = VLine_DashGap,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.VLine,
    } },
  
    { "vline_label", new FusionChartAttributeDescriptor { 
      Name = VLine_Label,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.VLine,
    } },
  
    { "vline_showlabelborder", new FusionChartAttributeDescriptor { 
      Name = VLine_ShowLabelBorder,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.VLine,
    } },
  
    { "vline_lineposition", new FusionChartAttributeDescriptor { 
      Name = VLine_LinePosition,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.VLine,
    } },
  
    { "vline_labelposition", new FusionChartAttributeDescriptor { 
      Name = VLine_LabelPosition,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.VLine,
    } },
  
    { "vline_labelhalign", new FusionChartAttributeDescriptor { 
      Name = VLine_LabelHAlign,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.VLine,
    } },
  
    { "vline_labelvalign", new FusionChartAttributeDescriptor { 
      Name = VLine_LabelVAlign,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.VLine,
    } },
  
    { "line_startvalue", new FusionChartAttributeDescriptor { 
      Name = Line_StartValue,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Line,
    } },
  
    { "line_endvalue", new FusionChartAttributeDescriptor { 
      Name = Line_EndValue,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Line,
    } },
  
    { "line_displayvalue", new FusionChartAttributeDescriptor { 
      Name = Line_DisplayValue,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Line,
    } },
  
    { "line_color", new FusionChartAttributeDescriptor { 
      Name = Line_Color,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Line,
    } },
  
    { "line_istrendzone", new FusionChartAttributeDescriptor { 
      Name = Line_IsTrendZone,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Line,
    } },
  
    { "line_showontop", new FusionChartAttributeDescriptor { 
      Name = Line_ShowOnTop,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Line,
    } },
  
    { "line_thickness", new FusionChartAttributeDescriptor { 
      Name = Line_Thickness,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Line,
    } },
  
    { "line_alpha", new FusionChartAttributeDescriptor { 
      Name = Line_Alpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Line,
    } },
  
    { "line_dashed", new FusionChartAttributeDescriptor { 
      Name = Line_Dashed,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Line,
    } },
  
    { "line_dashlen", new FusionChartAttributeDescriptor { 
      Name = Line_DashLen,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Line,
    } },
  
    { "line_dashgap", new FusionChartAttributeDescriptor { 
      Name = Line_DashGap,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Line,
    } },
  
    { "line_valueonright", new FusionChartAttributeDescriptor { 
      Name = Line_ValueOnRight,
      Charts = FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine,
      Element = FusionChartElementType.Line,
    } },
  
    { "line_tooltext", new FusionChartAttributeDescriptor { 
      Name = Line_ToolText,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Line,
    } },
  
    { "chart_pieinnerfacealpha", new FusionChartAttributeDescriptor { 
      Name = Chart_PieInnerFaceAlpha,
      Charts = FusionChartType.Pie3D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_pieouterfacealpha", new FusionChartAttributeDescriptor { 
      Name = Chart_PieOuterFaceAlpha,
      Charts = FusionChartType.Pie3D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_pieyscale", new FusionChartAttributeDescriptor { 
      Name = Chart_PieYScale,
      Charts = FusionChartType.Pie3D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_pieslicedepth", new FusionChartAttributeDescriptor { 
      Name = Chart_PieSliceDepth,
      Charts = FusionChartType.Pie3D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_maxcolwidth", new FusionChartAttributeDescriptor { 
      Name = Chart_MaxColWidth,
      Charts = FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Pareto3D | FusionChartType.MSColumn3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_canvasbasecolor", new FusionChartAttributeDescriptor { 
      Name = Chart_CanvasBaseColor,
      Charts = FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Pareto3D | FusionChartType.MSColumn3D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.StackedBar3D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showcanvasbg", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowCanvasBg,
      Charts = FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Pareto3D | FusionChartType.MSColumn3D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.StackedBar3D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showcanvasbase", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowCanvasBase,
      Charts = FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Pareto3D | FusionChartType.MSColumn3D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.StackedBar3D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_canvasbasedepth", new FusionChartAttributeDescriptor { 
      Name = Chart_CanvasBaseDepth,
      Charts = FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Pareto3D | FusionChartType.MSColumn3D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.StackedBar3D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_canvasbgdepth", new FusionChartAttributeDescriptor { 
      Name = Chart_CanvasBgDepth,
      Charts = FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Pareto3D | FusionChartType.MSColumn3D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.StackedBar3D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_overlapcolumns", new FusionChartAttributeDescriptor { 
      Name = Chart_OverlapColumns,
      Charts = FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Pareto3D | FusionChartType.MSColumn3D | FusionChartType.StackedColumn3D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_zeroplaneshowborder", new FusionChartAttributeDescriptor { 
      Name = Chart_ZeroPlaneShowBorder,
      Charts = FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Pareto3D | FusionChartType.MSColumn3D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.StackedBar3D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_zeroplanebordercolor", new FusionChartAttributeDescriptor { 
      Name = Chart_ZeroPlaneBorderColor,
      Charts = FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Pareto3D | FusionChartType.MSColumn3D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.StackedBar3D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_maxlabelwidthpercent", new FusionChartAttributeDescriptor { 
      Name = Chart_MaxLabelWidthPercent,
      Charts = FusionChartType.Bar2D | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_rotatexaxisname", new FusionChartAttributeDescriptor { 
      Name = Chart_RotateXAxisName,
      Charts = FusionChartType.Bar2D | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_xaxisnamewidth", new FusionChartAttributeDescriptor { 
      Name = Chart_XAxisNameWidth,
      Charts = FusionChartType.Bar2D | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_centerxaxisname", new FusionChartAttributeDescriptor { 
      Name = Chart_CenterXaxisName,
      Charts = FusionChartType.Bar2D | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showalternatevgridcolor", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowAlternateVGridColor,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.StackedArea2D | FusionChartType.StackedBar2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_alternatevgridcolor", new FusionChartAttributeDescriptor { 
      Name = Chart_AlternateVGridColor,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.StackedArea2D | FusionChartType.StackedBar2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_alternatevgridalpha", new FusionChartAttributeDescriptor { 
      Name = Chart_AlternateVGridAlpha,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.StackedArea2D | FusionChartType.StackedBar2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_connectnulldata", new FusionChartAttributeDescriptor { 
      Name = Chart_ConnectNullData,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedArea2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_valueposition", new FusionChartAttributeDescriptor { 
      Name = Chart_ValuePosition,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.ZoomLine | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_linecolor", new FusionChartAttributeDescriptor { 
      Name = Chart_LineColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.ZoomLine | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_linethickness", new FusionChartAttributeDescriptor { 
      Name = Chart_LineThickness,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.ZoomLine | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_linealpha", new FusionChartAttributeDescriptor { 
      Name = Chart_LineAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.ZoomLine | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_linedashed", new FusionChartAttributeDescriptor { 
      Name = Chart_LineDashed,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_linedashlen", new FusionChartAttributeDescriptor { 
      Name = Chart_LineDashLen,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_linedashgap", new FusionChartAttributeDescriptor { 
      Name = Chart_LineDashGap,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_drawanchors", new FusionChartAttributeDescriptor { 
      Name = Chart_DrawAnchors,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_anchorsides", new FusionChartAttributeDescriptor { 
      Name = Chart_AnchorSides,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_anchorradius", new FusionChartAttributeDescriptor { 
      Name = Chart_AnchorRadius,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_anchorbordercolor", new FusionChartAttributeDescriptor { 
      Name = Chart_AnchorBorderColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_anchorborderthickness", new FusionChartAttributeDescriptor { 
      Name = Chart_AnchorBorderThickness,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_anchorbgcolor", new FusionChartAttributeDescriptor { 
      Name = Chart_AnchorBgColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_anchoralpha", new FusionChartAttributeDescriptor { 
      Name = Chart_AnchorAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_anchorbgalpha", new FusionChartAttributeDescriptor { 
      Name = Chart_AnchorBgAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.Pareto2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_numvdivlines", new FusionChartAttributeDescriptor { 
      Name = Chart_NumVDivLines,
      Charts = FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_vdivlinecolor", new FusionChartAttributeDescriptor { 
      Name = Chart_VDivLineColor,
      Charts = FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_vdivlinethickness", new FusionChartAttributeDescriptor { 
      Name = Chart_VDivLineThickness,
      Charts = FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_vdivlinealpha", new FusionChartAttributeDescriptor { 
      Name = Chart_VDivLineAlpha,
      Charts = FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_vdivlineisdashed", new FusionChartAttributeDescriptor { 
      Name = Chart_VDivLineIsDashed,
      Charts = FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_vdivlinedashlen", new FusionChartAttributeDescriptor { 
      Name = Chart_VDivLineDashLen,
      Charts = FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_vdivlinedashgap", new FusionChartAttributeDescriptor { 
      Name = Chart_VDivLineDashGap,
      Charts = FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showzeroplane", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowZeroPlane,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.MSLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_canvaspadding", new FusionChartAttributeDescriptor { 
      Name = Chart_CanvasPadding,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.MSBar3D | FusionChartType.StackedArea2D | FusionChartType.StackedBar3D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "set_valueposition", new FusionChartAttributeDescriptor { 
      Name = Set_ValuePosition,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Set,
    } },
  
    { "set_anchorsides", new FusionChartAttributeDescriptor { 
      Name = Set_AnchorSides,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Set,
    } },
  
    { "set_anchorradius", new FusionChartAttributeDescriptor { 
      Name = Set_AnchorRadius,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Set,
    } },
  
    { "set_anchorbordercolor", new FusionChartAttributeDescriptor { 
      Name = Set_AnchorBorderColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Set,
    } },
  
    { "set_anchorborderthickness", new FusionChartAttributeDescriptor { 
      Name = Set_AnchorBorderThickness,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Set,
    } },
  
    { "set_anchorbgcolor", new FusionChartAttributeDescriptor { 
      Name = Set_AnchorBgColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Set,
    } },
  
    { "set_anchoralpha", new FusionChartAttributeDescriptor { 
      Name = Set_AnchorAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Set,
    } },
  
    { "set_anchorbgalpha", new FusionChartAttributeDescriptor { 
      Name = Set_AnchorBgAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Set,
    } },
  
    { "chart_doughnutradius", new FusionChartAttributeDescriptor { 
      Name = Chart_DoughnutRadius,
      Charts = FusionChartType.Doughnut2D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_plotfillcolor", new FusionChartAttributeDescriptor { 
      Name = Chart_PlotFillColor,
      Charts = FusionChartType.ScrollArea2D | FusionChartType.MSArea | FusionChartType.StackedArea2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showsecondarylimits", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowSecondaryLimits,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.Pareto2D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showdivlinesecondaryvalue", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowDivLineSecondaryValue,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.Pareto2D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_pyaxismaxvalue", new FusionChartAttributeDescriptor { 
      Name = Chart_PYAxisMaxValue,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.Pareto2D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_pyaxisminvalue", new FusionChartAttributeDescriptor { 
      Name = Chart_PYAxisMinValue,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.Pareto2D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_pyaxisnamewidth", new FusionChartAttributeDescriptor { 
      Name = Chart_PYAxisNameWidth,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.Pareto2D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_syaxisnamewidth", new FusionChartAttributeDescriptor { 
      Name = Chart_SYAxisNameWidth,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.Pareto2D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showcumulativeline", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowCumulativeLine,
      Charts = FusionChartType.Pareto3D | FusionChartType.Pareto2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showlinevalues", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowLineValues,
      Charts = FusionChartType.Pareto3D | FusionChartType.Pareto2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_primaryaxisonleft", new FusionChartAttributeDescriptor { 
      Name = Chart_PrimaryAxisOnLeft,
      Charts = FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_use3dlineshift", new FusionChartAttributeDescriptor { 
      Name = Chart_Use3DLineShift,
      Charts = FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_syaxisname", new FusionChartAttributeDescriptor { 
      Name = Chart_SYAXisName,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.Pareto2D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_pyaxisname", new FusionChartAttributeDescriptor { 
      Name = Chart_PYAxisName,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.Pareto2D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_sdecimals", new FusionChartAttributeDescriptor { 
      Name = Chart_Sdecimals,
      Charts = FusionChartType.Pareto3D | FusionChartType.Pareto2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_sforcedecimals", new FusionChartAttributeDescriptor { 
      Name = Chart_SforceDecimals,
      Charts = FusionChartType.Pareto3D | FusionChartType.Pareto2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_syaxisvaluedecimals", new FusionChartAttributeDescriptor { 
      Name = Chart_SyAxisValueDecimals,
      Charts = FusionChartType.Pareto3D | FusionChartType.Pareto2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "line_parentyaxis", new FusionChartAttributeDescriptor { 
      Name = Line_ParentYAxis,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.Pareto2D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Line,
    } },
  
    { "chart_legendmarkercircle", new FusionChartAttributeDescriptor { 
      Name = Chart_LegendMarkerCircle,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_seriesnameintooltip", new FusionChartAttributeDescriptor { 
      Name = Chart_SeriesNameInToolTip,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_legendpadding", new FusionChartAttributeDescriptor { 
      Name = Chart_LegendPadding,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "categories_font", new FusionChartAttributeDescriptor { 
      Name = Categories_Font,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Categories,
    } },
  
    { "categories_fontsize", new FusionChartAttributeDescriptor { 
      Name = Categories_FontSize,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Categories,
    } },
  
    { "categories_fontcolor", new FusionChartAttributeDescriptor { 
      Name = Categories_FontColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Categories,
    } },
  
    { "category_label", new FusionChartAttributeDescriptor { 
      Name = Category_Label,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Category,
    } },
  
    { "category_showlabel", new FusionChartAttributeDescriptor { 
      Name = Category_ShowLabel,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Category,
    } },
  
    { "category_tooltext", new FusionChartAttributeDescriptor { 
      Name = Category_ToolText,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Category,
    } },
  
    { "dataset_seriesname", new FusionChartAttributeDescriptor { 
      Name = Dataset_SeriesName,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "dataset_color", new FusionChartAttributeDescriptor { 
      Name = Dataset_Color,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "dataset_alpha", new FusionChartAttributeDescriptor { 
      Name = Dataset_Alpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "dataset_ratio", new FusionChartAttributeDescriptor { 
      Name = Dataset_Ratio,
      Charts = FusionChartType.Pie2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.MSColumn2D | FusionChartType.MSBar2D | FusionChartType.Marimekko | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.StackedColumn2DLine,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "dataset_showvalues", new FusionChartAttributeDescriptor { 
      Name = Dataset_ShowValues,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "dataset_dashed", new FusionChartAttributeDescriptor { 
      Name = Dataset_Dashed,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "dataset_includeinlegend", new FusionChartAttributeDescriptor { 
      Name = Dataset_IncludeInLegend,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSColumn2D | FusionChartType.MSLine | FusionChartType.MSColumn3D | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.MSBar2D | FusionChartType.MSBar3D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "dataset_valueposition", new FusionChartAttributeDescriptor { 
      Name = Dataset_ValuePosition,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Bar2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.ZoomLine | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "dataset_drawanchors", new FusionChartAttributeDescriptor { 
      Name = Dataset_DrawAnchors,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "dataset_anchorsides", new FusionChartAttributeDescriptor { 
      Name = Dataset_AnchorSides,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "dataset_anchorradius", new FusionChartAttributeDescriptor { 
      Name = Dataset_AnchorRadius,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "dataset_anchorbordercolor", new FusionChartAttributeDescriptor { 
      Name = Dataset_AnchorBorderColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "dataset_anchorborderthickness", new FusionChartAttributeDescriptor { 
      Name = Dataset_AnchorBorderThickness,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "dataset_anchorbgcolor", new FusionChartAttributeDescriptor { 
      Name = Dataset_AnchorBgColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "dataset_anchoralpha", new FusionChartAttributeDescriptor { 
      Name = Dataset_AnchorAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "dataset_anchorbgalpha", new FusionChartAttributeDescriptor { 
      Name = Dataset_AnchorBgAlpha,
      Charts = FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.ScrollArea2D | FusionChartType.MSLine | FusionChartType.MSArea | FusionChartType.ZoomLine | FusionChartType.StackedArea2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "dataset_linethickness", new FusionChartAttributeDescriptor { 
      Name = Dataset_LineThickness,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.ZoomLine | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "dataset_linedashlen", new FusionChartAttributeDescriptor { 
      Name = Dataset_LineDashLen,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "dataset_linedashgap", new FusionChartAttributeDescriptor { 
      Name = Dataset_LineDashGap,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Column2D | FusionChartType.Column3D | FusionChartType.Bar2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSLine | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "dataset_showplotborder", new FusionChartAttributeDescriptor { 
      Name = Dataset_ShowPlotBorder,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie3D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSArea | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "dataset_plotbordercolor", new FusionChartAttributeDescriptor { 
      Name = Dataset_PlotBorderColor,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie3D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSArea | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "dataset_plotborderthickness", new FusionChartAttributeDescriptor { 
      Name = Dataset_PlotBorderThickness,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie3D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSArea | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "dataset_plotborderalpha", new FusionChartAttributeDescriptor { 
      Name = Dataset_PlotBorderAlpha,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie3D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSArea | FusionChartType.StackedArea2D | FusionChartType.MSCombi2D,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "chart_compactdatamode", new FusionChartAttributeDescriptor { 
      Name = Chart_CompactDataMode,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_dataseparator", new FusionChartAttributeDescriptor { 
      Name = Chart_DataSeparator,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_axis", new FusionChartAttributeDescriptor { 
      Name = Chart_Axis,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_logbase", new FusionChartAttributeDescriptor { 
      Name = Chart_LogBase,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_numminorlogdivlines", new FusionChartAttributeDescriptor { 
      Name = Chart_NumMinorLogDivLines,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_dynamicaxis", new FusionChartAttributeDescriptor { 
      Name = Chart_DynamicAxis,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_divintervalhints", new FusionChartAttributeDescriptor { 
      Name = Chart_DivIntervalHints,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_allowpinmode", new FusionChartAttributeDescriptor { 
      Name = Chart_AllowPinMode,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_numvisiblelabels", new FusionChartAttributeDescriptor { 
      Name = Chart_NumVisibleLabels,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_anchorminrenderdistance", new FusionChartAttributeDescriptor { 
      Name = Chart_AnchorMinRenderDistance,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showvdivlines", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowVDivLines,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_displaystartindex", new FusionChartAttributeDescriptor { 
      Name = Chart_DisplayStartIndex,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_displayendindex", new FusionChartAttributeDescriptor { 
      Name = Chart_DisplayEndIndex,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_drawtoolbarbuttons", new FusionChartAttributeDescriptor { 
      Name = Chart_DrawToolbarButtons,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_pixelsperpoint", new FusionChartAttributeDescriptor { 
      Name = Chart_PixelsPerPoint,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_palettethemecolor", new FusionChartAttributeDescriptor { 
      Name = Chart_PaletteThemeColor,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_toolbarbuttoncolor", new FusionChartAttributeDescriptor { 
      Name = Chart_ToolbarButtonColor,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_toolbarbuttonfontcolor", new FusionChartAttributeDescriptor { 
      Name = Chart_ToolbarButtonFontColor,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_zoompanebordercolor", new FusionChartAttributeDescriptor { 
      Name = Chart_ZoomPaneBorderColor,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_zoompanebgcolor", new FusionChartAttributeDescriptor { 
      Name = Chart_ZoomPaneBgColor,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_zoompanebgalpha", new FusionChartAttributeDescriptor { 
      Name = Chart_ZoomPaneBgAlpha,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_pinlinethicknessdelta", new FusionChartAttributeDescriptor { 
      Name = Chart_PinLineThicknessDelta,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_pinpanebordercolor", new FusionChartAttributeDescriptor { 
      Name = Chart_PinPaneBorderColor,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_pinpanebgcolor", new FusionChartAttributeDescriptor { 
      Name = Chart_PinPaneBgColor,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_pinpanebgalpha", new FusionChartAttributeDescriptor { 
      Name = Chart_PinPaneBgAlpha,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_tooltipbarcolor", new FusionChartAttributeDescriptor { 
      Name = Chart_ToolTipBarColor,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_mousecursorcolor", new FusionChartAttributeDescriptor { 
      Name = Chart_MouseCursorColor,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_btnresetcharttitle", new FusionChartAttributeDescriptor { 
      Name = Chart_BtnResetChartTitle,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_btnzoomouttitle", new FusionChartAttributeDescriptor { 
      Name = Chart_BtnZoomOutTitle,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_btnswitchtozoommodetitle", new FusionChartAttributeDescriptor { 
      Name = Chart_BtnSwitchtoZoomModeTitle,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_btnswitchtopinmodetitle", new FusionChartAttributeDescriptor { 
      Name = Chart_BtnSwitchToPinModeTitle,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showtoolbarbuttontooltext", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowToolBarButtonTooltext,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_btnresetcharttooltext", new FusionChartAttributeDescriptor { 
      Name = Chart_BtnResetChartTooltext,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_btnswitchtopinmodetooltext", new FusionChartAttributeDescriptor { 
      Name = Chart_BtnSwitchToPinModeTooltext,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_zoomoutmenuitemlabel", new FusionChartAttributeDescriptor { 
      Name = Chart_ZoomOutMenuItemLabel,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_resetchartmenuitemlabel", new FusionChartAttributeDescriptor { 
      Name = Chart_ResetChartMenuItemLabel,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_zoommodemenuitemlabel", new FusionChartAttributeDescriptor { 
      Name = Chart_ZoomModeMenuItemLabel,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_pinmodemenuitemlabel", new FusionChartAttributeDescriptor { 
      Name = Chart_PinModeMenuItemLabel,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_toolbarbtntextvmargin", new FusionChartAttributeDescriptor { 
      Name = Chart_ToolBarBtnTextVMargin,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_toolbarbtntexthmargin", new FusionChartAttributeDescriptor { 
      Name = Chart_ToolBarBtnTextHMargin,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_toolbarbtnhpadding", new FusionChartAttributeDescriptor { 
      Name = Chart_ToolBarBtnHPadding,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_toolbarbtnvpadding", new FusionChartAttributeDescriptor { 
      Name = Chart_ToolBarBtnVPadding,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_scrollcolor", new FusionChartAttributeDescriptor { 
      Name = Chart_ScrollColor,
      Charts = FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_scrollheight", new FusionChartAttributeDescriptor { 
      Name = Chart_ScrollHeight,
      Charts = FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_scrollpadding", new FusionChartAttributeDescriptor { 
      Name = Chart_ScrollPadding,
      Charts = FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_scrollbtnwidth", new FusionChartAttributeDescriptor { 
      Name = Chart_ScrollBtnWidth,
      Charts = FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_scrollbtnpadding", new FusionChartAttributeDescriptor { 
      Name = Chart_ScrollBtnPadding,
      Charts = FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.ZoomLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "set_startindex", new FusionChartAttributeDescriptor { 
      Name = Set_StartIndex,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Set,
    } },
  
    { "set_endindex", new FusionChartAttributeDescriptor { 
      Name = Set_EndIndex,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Set,
    } },
  
    { "set_displayalways", new FusionChartAttributeDescriptor { 
      Name = Set_DisplayAlways,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Set,
    } },
  
    { "set_displaywhencount", new FusionChartAttributeDescriptor { 
      Name = Set_DisplayWhenCount,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Set,
    } },
  
    { "set_showontop", new FusionChartAttributeDescriptor { 
      Name = Set_ShowOnTop,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Set,
    } },
  
    { "set_thickness", new FusionChartAttributeDescriptor { 
      Name = Set_Thickness,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D | FusionChartType.ZoomLine,
      Element = FusionChartElementType.Set,
    } },
  
    { "set_valueontop", new FusionChartAttributeDescriptor { 
      Name = Set_ValueOnTop,
      Charts = FusionChartType.ZoomLine,
      Element = FusionChartElementType.Set,
    } },
  
    { "chart_maxbarheight", new FusionChartAttributeDescriptor { 
      Name = Chart_MaxBarHeight,
      Charts = FusionChartType.MSBar3D | FusionChartType.StackedBar3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_bardepth", new FusionChartAttributeDescriptor { 
      Name = Chart_BarDepth,
      Charts = FusionChartType.MSBar3D | FusionChartType.StackedBar3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_overlapbars", new FusionChartAttributeDescriptor { 
      Name = Chart_OverlapBars,
      Charts = FusionChartType.MSBar3D | FusionChartType.StackedBar3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showsum", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowSum,
      Charts = FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Doughnut2D | FusionChartType.StackedColumn3D | FusionChartType.Marimekko | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_stack100percent", new FusionChartAttributeDescriptor { 
      Name = Chart_Stack100Percent,
      Charts = FusionChartType.Doughnut2D | FusionChartType.StackedColumn3D | FusionChartType.StackedArea2D | FusionChartType.StackedColumn2D | FusionChartType.StackedBar2D | FusionChartType.StackedBar3D | FusionChartType.MSStackedColumn2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_usepercentdistribution", new FusionChartAttributeDescriptor { 
      Name = Chart_UsePercentDistribution,
      Charts = FusionChartType.Marimekko,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showxaxispercentvalues", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowXAxisPercentValues,
      Charts = FusionChartType.Marimekko,
      Element = FusionChartElementType.Chart,
    } },
  
    { "category_widthpercent", new FusionChartAttributeDescriptor { 
      Name = Category_WidthPercent,
      Charts = FusionChartType.Marimekko,
      Element = FusionChartElementType.Category,
    } },
  
    { "chart_animate3d", new FusionChartAttributeDescriptor { 
      Name = Chart_Animate3D,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_exetime", new FusionChartAttributeDescriptor { 
      Name = Chart_ExeTime,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_xaxistickcolor", new FusionChartAttributeDescriptor { 
      Name = Chart_XAxisTickColor,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_xaxistickalpha", new FusionChartAttributeDescriptor { 
      Name = Chart_XAxisTickAlpha,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_xaxistickthickness", new FusionChartAttributeDescriptor { 
      Name = Chart_XAxisTickThickness,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_is2d", new FusionChartAttributeDescriptor { 
      Name = Chart_Is2D,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_clustered", new FusionChartAttributeDescriptor { 
      Name = Chart_Clustered,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_chartorder", new FusionChartAttributeDescriptor { 
      Name = Chart_ChartOrder,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_chartontop", new FusionChartAttributeDescriptor { 
      Name = Chart_ChartOnTop,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_autoscaling", new FusionChartAttributeDescriptor { 
      Name = Chart_AutoScaling,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_allowscaling", new FusionChartAttributeDescriptor { 
      Name = Chart_AllowScaling,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_startangx", new FusionChartAttributeDescriptor { 
      Name = Chart_StartAngX,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_startangy", new FusionChartAttributeDescriptor { 
      Name = Chart_StartAngY,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_endangx", new FusionChartAttributeDescriptor { 
      Name = Chart_EndAngX,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_endangy", new FusionChartAttributeDescriptor { 
      Name = Chart_EndAngY,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_cameraangx", new FusionChartAttributeDescriptor { 
      Name = Chart_CameraAngX,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_cameraangy", new FusionChartAttributeDescriptor { 
      Name = Chart_CameraAngY,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_lightangx", new FusionChartAttributeDescriptor { 
      Name = Chart_LightAngX,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_lightangy", new FusionChartAttributeDescriptor { 
      Name = Chart_LightAngY,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_intensity", new FusionChartAttributeDescriptor { 
      Name = Chart_Intensity,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_dynamicshading", new FusionChartAttributeDescriptor { 
      Name = Chart_DynamicShading,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_bright2d", new FusionChartAttributeDescriptor { 
      Name = Chart_Bright2D,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_allowrotation", new FusionChartAttributeDescriptor { 
      Name = Chart_AllowRotation,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_constrainverticalrotation", new FusionChartAttributeDescriptor { 
      Name = Chart_ConstrainVerticalRotation,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_minverticalrotangle", new FusionChartAttributeDescriptor { 
      Name = Chart_MinVerticalRotAngle,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_maxverticalrotangle", new FusionChartAttributeDescriptor { 
      Name = Chart_MaxVerticalRotAngle,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_constrainhorizontalrotation", new FusionChartAttributeDescriptor { 
      Name = Chart_ConstrainHorizontalRotation,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_minhorizontalrotangle", new FusionChartAttributeDescriptor { 
      Name = Chart_MinHorizontalRotAngle,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_maxhorizontalrotangle", new FusionChartAttributeDescriptor { 
      Name = Chart_MaxHorizontalRotAngle,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_zdepth", new FusionChartAttributeDescriptor { 
      Name = Chart_ZDepth,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_zgapplot", new FusionChartAttributeDescriptor { 
      Name = Chart_ZGapPlot,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_yzwalldepth", new FusionChartAttributeDescriptor { 
      Name = Chart_YzWallDepth,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_zxwalldepth", new FusionChartAttributeDescriptor { 
      Name = Chart_ZxWallDepth,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_xywalldepth", new FusionChartAttributeDescriptor { 
      Name = Chart_XyWallDepth,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_divlineeffect", new FusionChartAttributeDescriptor { 
      Name = Chart_DivLineEffect,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_zeroplanemesh", new FusionChartAttributeDescriptor { 
      Name = Chart_ZeroPlaneMesh,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_ylabelgap", new FusionChartAttributeDescriptor { 
      Name = Chart_YLabelGap,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_xlabelgap", new FusionChartAttributeDescriptor { 
      Name = Chart_XLabelGap,
      Charts = FusionChartType.MSCombi3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "dataset_renderas", new FusionChartAttributeDescriptor { 
      Name = Dataset_RenderAs,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSCombi3D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.StackedColumn2DLine | FusionChartType.StackedColumn3DLine,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "chart_areaovercolumns", new FusionChartAttributeDescriptor { 
      Name = Chart_AreaOverColumns,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "dataset_parentyaxis", new FusionChartAttributeDescriptor { 
      Name = Dataset_ParentYAxis,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D | FusionChartType.MSCombi2D | FusionChartType.MSColumnLine3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "chart_", new FusionChartAttributeDescriptor { 
      Name = Chart_,
      Charts = FusionChartType.Pie3D | FusionChartType.MSColumnLine3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_syaxisminvalue", new FusionChartAttributeDescriptor { 
      Name = Chart_SYAxisMinValue,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_syaxismaxvalue", new FusionChartAttributeDescriptor { 
      Name = Chart_SYAxisMaxValue,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_setadaptivesymin", new FusionChartAttributeDescriptor { 
      Name = Chart_SetAdaptiveSYMin,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_syncaxislimits", new FusionChartAttributeDescriptor { 
      Name = Chart_SyncAxisLimits,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showpzeroplanevalue", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowPZeroPlaneValue,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showszeroplanevalue", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowSZeroPlaneValue,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_sscalerecursively", new FusionChartAttributeDescriptor { 
      Name = Chart_SScaleRecursively,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_smaxscalerecursion", new FusionChartAttributeDescriptor { 
      Name = Chart_SMaxScaleRecursion,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_sscaleseparator", new FusionChartAttributeDescriptor { 
      Name = Chart_SScaleSeparator,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_forcesyaxisvaluedecimals", new FusionChartAttributeDescriptor { 
      Name = Chart_ForceSYAxisValueDecimals,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_sformatnumber", new FusionChartAttributeDescriptor { 
      Name = Chart_SFormatNumber,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_sformatnumberscale", new FusionChartAttributeDescriptor { 
      Name = Chart_SFormatNumberScale,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_sdefaultnumberscale", new FusionChartAttributeDescriptor { 
      Name = Chart_SDefaultNumberScale,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_snumberscaleunit", new FusionChartAttributeDescriptor { 
      Name = Chart_SNumberScaleUnit,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_snumberscalevalue", new FusionChartAttributeDescriptor { 
      Name = Chart_SNumberScaleValue,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_snumberprefix", new FusionChartAttributeDescriptor { 
      Name = Chart_SNumberPrefix,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_snumbersuffix", new FusionChartAttributeDescriptor { 
      Name = Chart_SNumberSuffix,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_sdecimals", new FusionChartAttributeDescriptor { 
      Name = Chart_SDecimals,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_syaxisvaluedecimals", new FusionChartAttributeDescriptor { 
      Name = Chart_SYAxisValueDecimals,
      Charts = FusionChartType.MSCombiDY2D | FusionChartType.Pie2D | FusionChartType.Column2D | FusionChartType.Pareto3D | FusionChartType.MSColumn3DLineDY,
      Element = FusionChartElementType.Chart,
    } },
  
    { "lineset_includeinlegend", new FusionChartAttributeDescriptor { 
      Name = Lineset_IncludeInLegend,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { "lineset_seriesname", new FusionChartAttributeDescriptor { 
      Name = Lineset_SeriesName,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { "lineset_color", new FusionChartAttributeDescriptor { 
      Name = Lineset_Color,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { "lineset_alpha", new FusionChartAttributeDescriptor { 
      Name = Lineset_Alpha,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { "lineset_showvalues", new FusionChartAttributeDescriptor { 
      Name = Lineset_ShowValues,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { "lineset_valueposition", new FusionChartAttributeDescriptor { 
      Name = Lineset_ValuePosition,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { "lineset_dashed", new FusionChartAttributeDescriptor { 
      Name = Lineset_Dashed,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { "lineset_linedashlen", new FusionChartAttributeDescriptor { 
      Name = Lineset_LineDashLen,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { "lineset_linedashgap", new FusionChartAttributeDescriptor { 
      Name = Lineset_LineDashGap,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { "lineset_linethickness", new FusionChartAttributeDescriptor { 
      Name = Lineset_LineThickness,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { "lineset_drawanchors", new FusionChartAttributeDescriptor { 
      Name = Lineset_DrawAnchors,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { "lineset_anchorsides", new FusionChartAttributeDescriptor { 
      Name = Lineset_AnchorSides,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { "lineset_anchorradius", new FusionChartAttributeDescriptor { 
      Name = Lineset_AnchorRadius,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { "lineset_anchorbordercolor", new FusionChartAttributeDescriptor { 
      Name = Lineset_AnchorBorderColor,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { "lineset_anchorborderthickness", new FusionChartAttributeDescriptor { 
      Name = Lineset_AnchorBorderThickness,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { "lineset_anchorbgcolor", new FusionChartAttributeDescriptor { 
      Name = Lineset_AnchorBgColor,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { "lineset_anchorbgalpha", new FusionChartAttributeDescriptor { 
      Name = Lineset_AnchorBgAlpha,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { "lineset_anchoralpha", new FusionChartAttributeDescriptor { 
      Name = Lineset_AnchorAlpha,
      Charts = FusionChartType.Pie2D,
      Element = FusionChartElementType.Lineset,
    } },
  
    { "chart_clipbubbles", new FusionChartAttributeDescriptor { 
      Name = Chart_ClipBubbles,
      Charts = FusionChartType.Pie3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_negativecolor", new FusionChartAttributeDescriptor { 
      Name = Chart_NegativeColor,
      Charts = FusionChartType.Pie3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_xaxislabelmode", new FusionChartAttributeDescriptor { 
      Name = Chart_XAxisLabelMode,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showxaxisvalues", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowXAxisValues,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showvlimits", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowVLimits,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showvdivlinevalues", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowVDivLineValues,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_xaxisminvalue", new FusionChartAttributeDescriptor { 
      Name = Chart_XAxisMinValue,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_xaxismaxvalue", new FusionChartAttributeDescriptor { 
      Name = Chart_XAxisMaxValue,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_bubblescale", new FusionChartAttributeDescriptor { 
      Name = Chart_BubbleScale,
      Charts = FusionChartType.Pie3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_xaxisvaluesstep", new FusionChartAttributeDescriptor { 
      Name = Chart_XAxisValuesStep,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_adjustvdiv", new FusionChartAttributeDescriptor { 
      Name = Chart_AdjustVDiv,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_setadaptivexmin", new FusionChartAttributeDescriptor { 
      Name = Chart_SetAdaptiveXMin,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showregressionline", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowRegressionLine,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showyonx", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowYOnX,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_regressionlinecolor", new FusionChartAttributeDescriptor { 
      Name = Chart_RegressionLineColor,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_regressionlinethickness", new FusionChartAttributeDescriptor { 
      Name = Chart_RegressionLineThickness,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_regressionlinealpha", new FusionChartAttributeDescriptor { 
      Name = Chart_RegressionLineAlpha,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_drawquadrant", new FusionChartAttributeDescriptor { 
      Name = Chart_DrawQuadrant,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_quadrantxval", new FusionChartAttributeDescriptor { 
      Name = Chart_QuadrantXVal,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_quadrantyval", new FusionChartAttributeDescriptor { 
      Name = Chart_QuadrantYVal,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_quadrantlinecolor", new FusionChartAttributeDescriptor { 
      Name = Chart_QuadrantLineColor,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_quadrantlinethickness", new FusionChartAttributeDescriptor { 
      Name = Chart_QuadrantLineThickness,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_quadrantlinealpha", new FusionChartAttributeDescriptor { 
      Name = Chart_QuadrantLineAlpha,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_quadrantlineisdashed", new FusionChartAttributeDescriptor { 
      Name = Chart_QuadrantLineIsDashed,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_quadrantlinedashlen", new FusionChartAttributeDescriptor { 
      Name = Chart_QuadrantLineDashLen,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_quadrantlinedashgap", new FusionChartAttributeDescriptor { 
      Name = Chart_QuadrantLineDashGap,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_quadrantlabeltl", new FusionChartAttributeDescriptor { 
      Name = Chart_QuadrantLabelTL,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_quadrantlabeltr", new FusionChartAttributeDescriptor { 
      Name = Chart_QuadrantLabelTR,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_quadrantlabelbl", new FusionChartAttributeDescriptor { 
      Name = Chart_QuadrantLabelBL,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_quadrantlabelbr", new FusionChartAttributeDescriptor { 
      Name = Chart_QuadrantLabelBR,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_quadrantlabelpadding", new FusionChartAttributeDescriptor { 
      Name = Chart_QuadrantLabelPadding,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_numvdivlines", new FusionChartAttributeDescriptor { 
      Name = Chart_NumVDivlines,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_vdivlinecolor", new FusionChartAttributeDescriptor { 
      Name = Chart_VDivlineColor,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_vdivlinethickness", new FusionChartAttributeDescriptor { 
      Name = Chart_VDivlineThickness,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_vdivlinealpha", new FusionChartAttributeDescriptor { 
      Name = Chart_VDivlineAlpha,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_vdivlineisdashed", new FusionChartAttributeDescriptor { 
      Name = Chart_VDivlineIsDashed,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_vdivlinedashlen", new FusionChartAttributeDescriptor { 
      Name = Chart_VDivlineDashLen,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_vdivlinedashgap", new FusionChartAttributeDescriptor { 
      Name = Chart_VDivlineDashGap,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_showvzeroplane", new FusionChartAttributeDescriptor { 
      Name = Chart_ShowVZeroPlane,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_vzeroplanecolor", new FusionChartAttributeDescriptor { 
      Name = Chart_VZeroPlaneColor,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_vzeroplanethickness", new FusionChartAttributeDescriptor { 
      Name = Chart_VZeroPlaneThickness,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_vzeroplanealpha", new FusionChartAttributeDescriptor { 
      Name = Chart_VZeroPlaneAlpha,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_yformatnumber", new FusionChartAttributeDescriptor { 
      Name = Chart_YFormatNumber,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_xformatnumber", new FusionChartAttributeDescriptor { 
      Name = Chart_XFormatNumber,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_yformatnumberscale", new FusionChartAttributeDescriptor { 
      Name = Chart_YFormatNumberScale,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_xformatnumberscale", new FusionChartAttributeDescriptor { 
      Name = Chart_XFormatNumberScale,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_ydefaultnumberscale", new FusionChartAttributeDescriptor { 
      Name = Chart_YDefaultNumberScale,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_xdefaultnumberscale", new FusionChartAttributeDescriptor { 
      Name = Chart_XDefaultNumberScale,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_ynumberscaleunit", new FusionChartAttributeDescriptor { 
      Name = Chart_YNumberScaleUnit,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_xnumberscaleunit", new FusionChartAttributeDescriptor { 
      Name = Chart_XNumberScaleUnit,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_xscalerecursively", new FusionChartAttributeDescriptor { 
      Name = Chart_XScaleRecursively,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_xmaxscalerecursion", new FusionChartAttributeDescriptor { 
      Name = Chart_XMaxScaleRecursion,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_xscaleseparator", new FusionChartAttributeDescriptor { 
      Name = Chart_XScaleSeparator,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_ynumberscalevalue", new FusionChartAttributeDescriptor { 
      Name = Chart_YNumberScaleValue,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_xnumberscalevalue", new FusionChartAttributeDescriptor { 
      Name = Chart_XNumberScaleValue,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_ynumberprefix", new FusionChartAttributeDescriptor { 
      Name = Chart_YNumberPrefix,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_xnumberprefix", new FusionChartAttributeDescriptor { 
      Name = Chart_XNumberPrefix,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_ynumbersuffix", new FusionChartAttributeDescriptor { 
      Name = Chart_YNumberSuffix,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_xnumbersuffix", new FusionChartAttributeDescriptor { 
      Name = Chart_XNumberSuffix,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_forcexaxisvaluedecimals", new FusionChartAttributeDescriptor { 
      Name = Chart_ForceXAxisValueDecimals,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_xaxisvaluedecimals", new FusionChartAttributeDescriptor { 
      Name = Chart_XAxisValueDecimals,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "categories_verticallinecolor", new FusionChartAttributeDescriptor { 
      Name = Categories_VerticalLineColor,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Categories,
    } },
  
    { "categories_verticallinethickness", new FusionChartAttributeDescriptor { 
      Name = Categories_VerticalLineThickness,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Categories,
    } },
  
    { "categories_verticallinealpha", new FusionChartAttributeDescriptor { 
      Name = Categories_VerticalLineAlpha,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Categories,
    } },
  
    { "categories_verticallinedashed", new FusionChartAttributeDescriptor { 
      Name = Categories_VerticalLineDashed,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Categories,
    } },
  
    { "categories_verticallinedashlen", new FusionChartAttributeDescriptor { 
      Name = Categories_VerticalLineDashLen,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Categories,
    } },
  
    { "categories_verticallinedashgap", new FusionChartAttributeDescriptor { 
      Name = Categories_VerticalLineDashGap,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Categories,
    } },
  
    { "category_x", new FusionChartAttributeDescriptor { 
      Name = Category_X,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Category,
    } },
  
    { "category_showverticalline", new FusionChartAttributeDescriptor { 
      Name = Category_ShowVerticalLine,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Category,
    } },
  
    { "category_linedashed", new FusionChartAttributeDescriptor { 
      Name = Category_LineDashed,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Category,
    } },
  
    { "dataset_plotfillalpha", new FusionChartAttributeDescriptor { 
      Name = Dataset_PlotFillAlpha,
      Charts = FusionChartType.Pie3D,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "dataset_showregressionline", new FusionChartAttributeDescriptor { 
      Name = Dataset_ShowRegressionLine,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "dataset_showyonx", new FusionChartAttributeDescriptor { 
      Name = Dataset_ShowYOnX,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "dataset_regressionlinecolor", new FusionChartAttributeDescriptor { 
      Name = Dataset_RegressionLineColor,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "dataset_regressionlinethickness", new FusionChartAttributeDescriptor { 
      Name = Dataset_RegressionLineThickness,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "dataset_regressionlinealpha", new FusionChartAttributeDescriptor { 
      Name = Dataset_RegressionLineAlpha,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "set_x", new FusionChartAttributeDescriptor { 
      Name = Set_X,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Set,
    } },
  
    { "set_y", new FusionChartAttributeDescriptor { 
      Name = Set_Y,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Set,
    } },
  
    { "set_z", new FusionChartAttributeDescriptor { 
      Name = Set_Z,
      Charts = FusionChartType.Pie3D,
      Element = FusionChartElementType.Set,
    } },
  
    { "set_name", new FusionChartAttributeDescriptor { 
      Name = Set_Name,
      Charts = FusionChartType.Pie3D,
      Element = FusionChartElementType.Set,
    } },
  
    { "set_startvalue", new FusionChartAttributeDescriptor { 
      Name = Set_StartValue,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Set,
    } },
  
    { "set_endvalue", new FusionChartAttributeDescriptor { 
      Name = Set_EndValue,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Set,
    } },
  
    { "set_istrendzone", new FusionChartAttributeDescriptor { 
      Name = Set_IsTrendZone,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Set,
    } },
  
    { "set_dashlen", new FusionChartAttributeDescriptor { 
      Name = Set_DashLen,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Set,
    } },
  
    { "set_dashgap", new FusionChartAttributeDescriptor { 
      Name = Set_DashGap,
      Charts = FusionChartType.Pie3D | FusionChartType.Column3D,
      Element = FusionChartElementType.Set,
    } },
  
    { "dataset_drawline", new FusionChartAttributeDescriptor { 
      Name = Dataset_DrawLine,
      Charts = FusionChartType.Column3D,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "dataset_linecolor", new FusionChartAttributeDescriptor { 
      Name = Dataset_LineColor,
      Charts = FusionChartType.Column3D,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "dataset_linealpha", new FusionChartAttributeDescriptor { 
      Name = Dataset_LineAlpha,
      Charts = FusionChartType.Column3D,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "dataset_linedashed", new FusionChartAttributeDescriptor { 
      Name = Dataset_LineDashed,
      Charts = FusionChartType.Column3D,
      Element = FusionChartElementType.Dataset,
    } },
  
    { "chart_numvisibleplot", new FusionChartAttributeDescriptor { 
      Name = Chart_NumVisiblePlot,
      Charts = FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    { "chart_scrolltoend", new FusionChartAttributeDescriptor { 
      Name = Chart_ScrollToEnd,
      Charts = FusionChartType.Bar2D | FusionChartType.Line | FusionChartType.Doughnut2D | FusionChartType.ScrollArea2D | FusionChartType.Pareto3D | FusionChartType.ScrollCombi2D,
      Element = FusionChartElementType.Chart,
    } },
  
    };
}
}
