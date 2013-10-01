using Qorpent.Charts.FusionCharts;
    
namespace Qorpent.Charts {
  /// <summary>
  ///
  /// </summary>
  public partial class Chart {
    /// <summary>
    /// 
    /// </summary>
    public bool Animation {
        get { return Get<bool>(Api.Chart_Animation); }
        set { Set(Api.Chart_Animation, value); }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool DefaultAnimation {
		get { return Get<bool>(Api.Chart_DefaultAnimation); }
		set { Set(Api.Chart_DefaultAnimation, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public string ClickURL {
		get { return Get<string>(Api.Chart_ClickURL); }
		set { Set(Api.Chart_ClickURL, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool ShowPlotBorder {
		get { return Get<bool>(Api.Chart_ShowPlotBorder); }
		set { Set(Api.Chart_ShowPlotBorder, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public IChartColor PlotBorderColor {
		get { return Get<IChartColor>(Api.Chart_PlotBorderColor); }
		set { Set(Api.Chart_PlotBorderColor, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int PlotBorderThickness {
		get { return Get<int>(Api.Chart_PlotBorderThickness); }
		set { Set(Api.Chart_PlotBorderThickness, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int PlotBorderAlpha {
		get { return Get<int>(Api.Chart_PlotBorderAlpha); }
		set { Set(Api.Chart_PlotBorderAlpha, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int PlotFillAlpha {
		get { return Get<int>(Api.Chart_PlotFillAlpha); }
		set { Set(Api.Chart_PlotFillAlpha, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool ShowShadow {
		get { return Get<bool>(Api.Chart_ShowShadow); }
		set { Set(Api.Chart_ShowShadow, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool UseEllipsesWhenOverflow {
		get { return Get<bool>(Api.Chart_UseEllipsesWhenOverflow); }
		set { Set(Api.Chart_UseEllipsesWhenOverflow, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public string InDecimalSeparator {
		get { return Get<string>(Api.Chart_InDecimalSeparator); }
		set { Set(Api.Chart_InDecimalSeparator, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public string InThousandSeparator {
		get { return Get<string>(Api.Chart_InThousandSeparator); }
		set { Set(Api.Chart_InThousandSeparator, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool ShowLegend {
		get { return Get<bool>(Api.Chart_ShowLegend); }
		set { Set(Api.Chart_ShowLegend, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public string LegendPosition {
		get { return Get<string>(Api.Chart_LegendPosition); }
		set { Set(Api.Chart_LegendPosition, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public string LegendCaption {
		get { return Get<string>(Api.Chart_LegendCaption); }
		set { Set(Api.Chart_LegendCaption, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int LegendIconScale {
		get { return Get<int>(Api.Chart_LegendIconScale); }
		set { Set(Api.Chart_LegendIconScale, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public IChartColor LegendBgColor {
		get { return Get<IChartColor>(Api.Chart_LegendBgColor); }
		set { Set(Api.Chart_LegendBgColor, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int LegendBgAlpha {
		get { return Get<int>(Api.Chart_LegendBgAlpha); }
		set { Set(Api.Chart_LegendBgAlpha, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public IChartColor LegendBorderColor {
		get { return Get<IChartColor>(Api.Chart_LegendBorderColor); }
		set { Set(Api.Chart_LegendBorderColor, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int LegendBorderThickness {
		get { return Get<int>(Api.Chart_LegendBorderThickness); }
		set { Set(Api.Chart_LegendBorderThickness, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int LegendBorderAlpha {
		get { return Get<int>(Api.Chart_LegendBorderAlpha); }
		set { Set(Api.Chart_LegendBorderAlpha, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool LegendShadow {
		get { return Get<bool>(Api.Chart_LegendShadow); }
		set { Set(Api.Chart_LegendShadow, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool LegendAllowDrag {
		get { return Get<bool>(Api.Chart_LegendAllowDrag); }
		set { Set(Api.Chart_LegendAllowDrag, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public IChartColor LegendScrollBgColor {
		get { return Get<IChartColor>(Api.Chart_LegendScrollBgColor); }
		set { Set(Api.Chart_LegendScrollBgColor, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public IChartColor LegendScrollBarColor {
		get { return Get<IChartColor>(Api.Chart_LegendScrollBarColor); }
		set { Set(Api.Chart_LegendScrollBarColor, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public IChartColor LegendScrollBtnColor {
		get { return Get<IChartColor>(Api.Chart_LegendScrollBtnColor); }
		set { Set(Api.Chart_LegendScrollBtnColor, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool ReverseLegend {
		get { return Get<bool>(Api.Chart_ReverseLegend); }
		set { Set(Api.Chart_ReverseLegend, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool InteractiveLegend {
		get { return Get<bool>(Api.Chart_InteractiveLegend); }
		set { Set(Api.Chart_InteractiveLegend, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int LegendNumColumns {
		get { return Get<int>(Api.Chart_LegendNumColumns); }
		set { Set(Api.Chart_LegendNumColumns, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool MinimiseWrappingInLegend {
		get { return Get<bool>(Api.Chart_MinimiseWrappingInLegend); }
		set { Set(Api.Chart_MinimiseWrappingInLegend, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public string ToolTipSepChar {
		get { return Get<string>(Api.Chart_ToolTipSepChar); }
		set { Set(Api.Chart_ToolTipSepChar, value); }
    }  
    /// <summary>
    /// 
    /// </summary>
    public string Link {
		get { return Get<string>(Api.Chart_Link); }
		set { Set(Api.Chart_Link, value); }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool ShowValue {
		get { return Get<bool>(Api.Chart_ShowValue); }
		set { Set(Api.Chart_ShowValue, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public string LabelDisplay {
		get { return Get<string>(Api.Chart_LabelDisplay); }
		set { Set(Api.Chart_LabelDisplay, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool RotateLabels {
		get { return Get<bool>(Api.Chart_RotateLabels); }
		set { Set(Api.Chart_RotateLabels, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool SlantLabels {
		get { return Get<bool>(Api.Chart_SlantLabels); }
		set { Set(Api.Chart_SlantLabels, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int LabelStep {
		get { return Get<int>(Api.Chart_LabelStep); }
		set { Set(Api.Chart_LabelStep, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int StaggerLines {
		get { return Get<int>(Api.Chart_StaggerLines); }
		set { Set(Api.Chart_StaggerLines, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool RotateValues {
		get { return Get<bool>(Api.Chart_RotateValues); }
		set { Set(Api.Chart_RotateValues, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool ShowYAxisValues {
		get { return Get<bool>(Api.Chart_ShowYAxisValues); }
		set { Set(Api.Chart_ShowYAxisValues, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool ShowLimits {
		get { return Get<bool>(Api.Chart_ShowLimits); }
		set { Set(Api.Chart_ShowLimits, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool ShowDivLineValues {
		get { return Get<bool>(Api.Chart_ShowDivLineValues); }
		set { Set(Api.Chart_ShowDivLineValues, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int YAxisValuesStep {
		get { return Get<int>(Api.Chart_YAxisValuesStep); }
		set { Set(Api.Chart_YAxisValuesStep, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool AdjustDiv {
		get { return Get<bool>(Api.Chart_AdjustDiv); }
		set { Set(Api.Chart_AdjustDiv, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool RotateYAxisName {
		get { return Get<bool>(Api.Chart_RotateYAxisName); }
		set { Set(Api.Chart_RotateYAxisName, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int YAxisNameWidth {
		get { return Get<int>(Api.Chart_YAxisNameWidth); }
		set { Set(Api.Chart_YAxisNameWidth, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int YAxisMinValue {
		get { return Get<int>(Api.Chart_YAxisMinValue); }
		set { Set(Api.Chart_YAxisMinValue, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int YAxisMaxValue {
		get { return Get<int>(Api.Chart_YAxisMaxValue); }
		set { Set(Api.Chart_YAxisMaxValue, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool CenterYaxisName {
		get { return Get<bool>(Api.Chart_CenterYaxisName); }
		set { Set(Api.Chart_CenterYaxisName, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public string YAxisName {
		get { return Get<string>(Api.Chart_YAxisName); }
		set { Set(Api.Chart_YAxisName, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int CanvasBgAlpha {
		get { return Get<int>(Api.Chart_CanvasBgAlpha); }
		set { Set(Api.Chart_CanvasBgAlpha, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int CanvasBgRatio {
		get { return Get<int>(Api.Chart_CanvasBgRatio); }
		set { Set(Api.Chart_CanvasBgRatio, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int CanvasBgAngle {
		get { return Get<int>(Api.Chart_CanvasBgAngle); }
		set { Set(Api.Chart_CanvasBgAngle, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public IChartColor CanvasBorderColor {
		get { return Get<IChartColor>(Api.Chart_CanvasBorderColor); }
		set { Set(Api.Chart_CanvasBorderColor, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int CanvasBorderThickness {
		get { return Get<int>(Api.Chart_CanvasBorderThickness); }
		set { Set(Api.Chart_CanvasBorderThickness, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int CanvasBorderAlpha {
		get { return Get<int>(Api.Chart_CanvasBorderAlpha); }
		set { Set(Api.Chart_CanvasBorderAlpha, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int NumDivLines {
		get { return Get<int>(Api.Chart_NumDivLines); }
		set { Set(Api.Chart_NumDivLines, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool DivLineIsDashed {
		get { return Get<bool>(Api.Chart_DivLineIsDashed); }
		set { Set(Api.Chart_DivLineIsDashed, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int DivLineDashLen {
		get { return Get<int>(Api.Chart_DivLineDashLen); }
		set { Set(Api.Chart_DivLineDashLen, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int DivLineDashGap {
		get { return Get<int>(Api.Chart_DivLineDashGap); }
		set { Set(Api.Chart_DivLineDashGap, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public IChartColor ZeroPlaneColor {
		get { return Get<IChartColor>(Api.Chart_ZeroPlaneColor); }
		set { Set(Api.Chart_ZeroPlaneColor, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int ZeroPlaneThickness {
		get { return Get<int>(Api.Chart_ZeroPlaneThickness); }
		set { Set(Api.Chart_ZeroPlaneThickness, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int ZeroPlaneAlpha {
		get { return Get<int>(Api.Chart_ZeroPlaneAlpha); }
		set { Set(Api.Chart_ZeroPlaneAlpha, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool ShowZeroPlaneValue {
		get { return Get<bool>(Api.Chart_ShowZeroPlaneValue); }
		set { Set(Api.Chart_ShowZeroPlaneValue, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool ShowAlternateHGridColor {
		get { return Get<bool>(Api.Chart_ShowAlternateHGridColor); }
		set { Set(Api.Chart_ShowAlternateHGridColor, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public IChartColor AlternateHGridColor {
		get { return Get<IChartColor>(Api.Chart_AlternateHGridColor); }
		set { Set(Api.Chart_AlternateHGridColor, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int AlternateHGridAlpha {
		get { return Get<int>(Api.Chart_AlternateHGridAlpha); }
		set { Set(Api.Chart_AlternateHGridAlpha, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int XAxisNamePadding {
		get { return Get<int>(Api.Chart_XAxisNamePadding); }
		set { Set(Api.Chart_XAxisNamePadding, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int YAxisNamePadding {
		get { return Get<int>(Api.Chart_YAxisNamePadding); }
		set { Set(Api.Chart_YAxisNamePadding, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int YAxisValuesPadding {
		get { return Get<int>(Api.Chart_YAxisValuesPadding); }
		set { Set(Api.Chart_YAxisValuesPadding, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int LabelPadding {
		get { return Get<int>(Api.Chart_LabelPadding); }
		set { Set(Api.Chart_LabelPadding, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int ValuePadding {
		get { return Get<int>(Api.Chart_ValuePadding); }
		set { Set(Api.Chart_ValuePadding, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int PlotSpacePercent {
		get { return Get<int>(Api.Chart_PlotSpacePercent); }
		set { Set(Api.Chart_PlotSpacePercent, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int CanvasLeftMargin {
		get { return Get<int>(Api.Chart_CanvasLeftMargin); }
		set { Set(Api.Chart_CanvasLeftMargin, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int CanvasRightMargin {
		get { return Get<int>(Api.Chart_CanvasRightMargin); }
		set { Set(Api.Chart_CanvasRightMargin, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int CanvasTopMargin {
		get { return Get<int>(Api.Chart_CanvasTopMargin); }
		set { Set(Api.Chart_CanvasTopMargin, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int CanvasBottomMargin {
		get { return Get<int>(Api.Chart_CanvasBottomMargin); }
		set { Set(Api.Chart_CanvasBottomMargin, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public IChartColor Color {
		get { return Get<IChartColor>(Api.Chart_Color); }
		set { Set(Api.Chart_Color, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int Thickness {
		get { return Get<int>(Api.Chart_Thickness); }
		set { Set(Api.Chart_Thickness, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int Alpha {
		get { return Get<int>(Api.Chart_Alpha); }
		set { Set(Api.Chart_Alpha, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool Dashed {
		get { return Get<bool>(Api.Chart_Dashed); }
		set { Set(Api.Chart_Dashed, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int DashLen {
		get { return Get<int>(Api.Chart_DashLen); }
		set { Set(Api.Chart_DashLen, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int DashGap {
		get { return Get<int>(Api.Chart_DashGap); }
		set { Set(Api.Chart_DashGap, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public string Label {
		get { return Get<string>(Api.Chart_Label); }
		set { Set(Api.Chart_Label, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool ShowLabelBorder {
		get { return Get<bool>(Api.Chart_ShowLabelBorder); }
		set { Set(Api.Chart_ShowLabelBorder, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int LinePosition {
		get { return Get<int>(Api.Chart_LinePosition); }
		set { Set(Api.Chart_LinePosition, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int LabelPosition {
		get { return Get<int>(Api.Chart_LabelPosition); }
		set { Set(Api.Chart_LabelPosition, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public string LabelHAlign {
		get { return Get<string>(Api.Chart_LabelHAlign); }
		set { Set(Api.Chart_LabelHAlign, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public string LabelVAlign {
		get { return Get<string>(Api.Chart_LabelVAlign); }
		set { Set(Api.Chart_LabelVAlign, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool IsTrendZone {
		get { return Get<bool>(Api.Chart_IsTrendZone); }
		set { Set(Api.Chart_IsTrendZone, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool ShowOnTop {
		get { return Get<bool>(Api.Chart_ShowOnTop); }
		set { Set(Api.Chart_ShowOnTop, value); }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool ValueOnRight {
		get { return Get<bool>(Api.Chart_ValueOnRight); }
		set { Set(Api.Chart_ValueOnRight, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public string ToolText {
		get { return Get<string>(Api.Chart_ToolText); }
		set { Set(Api.Chart_ToolText, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool LegendMarkerCircle {
		get { return Get<bool>(Api.Chart_LegendMarkerCircle); }
		set { Set(Api.Chart_LegendMarkerCircle, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool SeriesNameInToolTip {
		get { return Get<bool>(Api.Chart_SeriesNameInToolTip); }
		set { Set(Api.Chart_SeriesNameInToolTip, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int LegendPadding {
		get { return Get<int>(Api.Chart_LegendPadding); }
		set { Set(Api.Chart_LegendPadding, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public string Font {
		get { return Get<string>(Api.Chart_Font); }
		set { Set(Api.Chart_Font, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int FontSize {
		get { return Get<int>(Api.Chart_FontSize); }
		set { Set(Api.Chart_FontSize, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public IChartColor FontColor {
		get { return Get<IChartColor>(Api.Chart_FontColor); }
		set { Set(Api.Chart_FontColor, value); }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool ShowLabel {
		get { return Get<bool>(Api.Chart_ShowLabel); }
		set { Set(Api.Chart_ShowLabel, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public string SeriesName {
		get { return Get<string>(Api.Chart_SeriesName); }
		set { Set(Api.Chart_SeriesName, value); }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool ShowValues {
		get { return Get<bool>(Api.Chart_ShowValues); }
		set { Set(Api.Chart_ShowValues, value); }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IncludeInLegend {
		get { return Get<bool>(Api.Chart_IncludeInLegend); }
		set { Set(Api.Chart_IncludeInLegend, value); }
    }
  }
}
  