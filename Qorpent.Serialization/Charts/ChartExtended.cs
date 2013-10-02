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
        get { return Get<bool>(FusionChartApi.Chart_Animation); }
        set { Set(FusionChartApi.Chart_Animation, value); }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool DefaultAnimation {
		get { return Get<bool>(FusionChartApi.Chart_DefaultAnimation); }
		set { Set(FusionChartApi.Chart_DefaultAnimation, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public string ClickURL {
		get { return Get<string>(FusionChartApi.Chart_ClickURL); }
		set { Set(FusionChartApi.Chart_ClickURL, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool ShowPlotBorder {
		get { return Get<bool>(FusionChartApi.Chart_ShowPlotBorder); }
		set { Set(FusionChartApi.Chart_ShowPlotBorder, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public IChartColor PlotBorderColor {
		get { return Get<IChartColor>(FusionChartApi.Chart_PlotBorderColor); }
		set { Set(FusionChartApi.Chart_PlotBorderColor, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int PlotBorderThickness {
		get { return Get<int>(FusionChartApi.Chart_PlotBorderThickness); }
		set { Set(FusionChartApi.Chart_PlotBorderThickness, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int PlotBorderAlpha {
		get { return Get<int>(FusionChartApi.Chart_PlotBorderAlpha); }
		set { Set(FusionChartApi.Chart_PlotBorderAlpha, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int PlotFillAlpha {
		get { return Get<int>(FusionChartApi.Chart_PlotFillAlpha); }
		set { Set(FusionChartApi.Chart_PlotFillAlpha, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool ShowShadow {
		get { return Get<bool>(FusionChartApi.Chart_ShowShadow); }
		set { Set(FusionChartApi.Chart_ShowShadow, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool UseEllipsesWhenOverflow {
		get { return Get<bool>(FusionChartApi.Chart_UseEllipsesWhenOverflow); }
		set { Set(FusionChartApi.Chart_UseEllipsesWhenOverflow, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public string InDecimalSeparator {
		get { return Get<string>(FusionChartApi.Chart_InDecimalSeparator); }
		set { Set(FusionChartApi.Chart_InDecimalSeparator, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public string InThousandSeparator {
		get { return Get<string>(FusionChartApi.Chart_InThousandSeparator); }
		set { Set(FusionChartApi.Chart_InThousandSeparator, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool ShowLegend {
		get { return Get<bool>(FusionChartApi.Chart_ShowLegend); }
		set { Set(FusionChartApi.Chart_ShowLegend, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public string LegendPosition {
		get { return Get<string>(FusionChartApi.Chart_LegendPosition); }
		set { Set(FusionChartApi.Chart_LegendPosition, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public string LegendCaption {
		get { return Get<string>(FusionChartApi.Chart_LegendCaption); }
		set { Set(FusionChartApi.Chart_LegendCaption, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int LegendIconScale {
		get { return Get<int>(FusionChartApi.Chart_LegendIconScale); }
		set { Set(FusionChartApi.Chart_LegendIconScale, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public IChartColor LegendBgColor {
		get { return Get<IChartColor>(FusionChartApi.Chart_LegendBgColor); }
		set { Set(FusionChartApi.Chart_LegendBgColor, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int LegendBgAlpha {
		get { return Get<int>(FusionChartApi.Chart_LegendBgAlpha); }
		set { Set(FusionChartApi.Chart_LegendBgAlpha, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public IChartColor LegendBorderColor {
		get { return Get<IChartColor>(FusionChartApi.Chart_LegendBorderColor); }
		set { Set(FusionChartApi.Chart_LegendBorderColor, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int LegendBorderThickness {
		get { return Get<int>(FusionChartApi.Chart_LegendBorderThickness); }
		set { Set(FusionChartApi.Chart_LegendBorderThickness, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int LegendBorderAlpha {
		get { return Get<int>(FusionChartApi.Chart_LegendBorderAlpha); }
		set { Set(FusionChartApi.Chart_LegendBorderAlpha, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool LegendShadow {
		get { return Get<bool>(FusionChartApi.Chart_LegendShadow); }
		set { Set(FusionChartApi.Chart_LegendShadow, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool LegendAllowDrag {
		get { return Get<bool>(FusionChartApi.Chart_LegendAllowDrag); }
		set { Set(FusionChartApi.Chart_LegendAllowDrag, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public IChartColor LegendScrollBgColor {
		get { return Get<IChartColor>(FusionChartApi.Chart_LegendScrollBgColor); }
		set { Set(FusionChartApi.Chart_LegendScrollBgColor, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public IChartColor LegendScrollBarColor {
		get { return Get<IChartColor>(FusionChartApi.Chart_LegendScrollBarColor); }
		set { Set(FusionChartApi.Chart_LegendScrollBarColor, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public IChartColor LegendScrollBtnColor {
		get { return Get<IChartColor>(FusionChartApi.Chart_LegendScrollBtnColor); }
		set { Set(FusionChartApi.Chart_LegendScrollBtnColor, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool ReverseLegend {
		get { return Get<bool>(FusionChartApi.Chart_ReverseLegend); }
		set { Set(FusionChartApi.Chart_ReverseLegend, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool InteractiveLegend {
		get { return Get<bool>(FusionChartApi.Chart_InteractiveLegend); }
		set { Set(FusionChartApi.Chart_InteractiveLegend, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int LegendNumColumns {
		get { return Get<int>(FusionChartApi.Chart_LegendNumColumns); }
		set { Set(FusionChartApi.Chart_LegendNumColumns, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool MinimiseWrappingInLegend {
		get { return Get<bool>(FusionChartApi.Chart_MinimiseWrappingInLegend); }
		set { Set(FusionChartApi.Chart_MinimiseWrappingInLegend, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public string ToolTipSepChar {
		get { return Get<string>(FusionChartApi.Chart_ToolTipSepChar); }
		set { Set(FusionChartApi.Chart_ToolTipSepChar, value); }
    }  
    /// <summary>
    /// 
    /// </summary>
    public string Link {
		get { return Get<string>(FusionChartApi.Chart_Link); }
		set { Set(FusionChartApi.Chart_Link, value); }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool ShowValue {
		get { return Get<bool>(FusionChartApi.Chart_ShowValue); }
		set { Set(FusionChartApi.Chart_ShowValue, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public string LabelDisplay {
		get { return Get<string>(FusionChartApi.Chart_LabelDisplay); }
		set { Set(FusionChartApi.Chart_LabelDisplay, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool RotateLabels {
		get { return Get<bool>(FusionChartApi.Chart_RotateLabels); }
		set { Set(FusionChartApi.Chart_RotateLabels, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool SlantLabels {
		get { return Get<bool>(FusionChartApi.Chart_SlantLabels); }
		set { Set(FusionChartApi.Chart_SlantLabels, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int LabelStep {
		get { return Get<int>(FusionChartApi.Chart_LabelStep); }
		set { Set(FusionChartApi.Chart_LabelStep, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int StaggerLines {
		get { return Get<int>(FusionChartApi.Chart_StaggerLines); }
		set { Set(FusionChartApi.Chart_StaggerLines, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool RotateValues {
		get { return Get<bool>(FusionChartApi.Chart_RotateValues); }
		set { Set(FusionChartApi.Chart_RotateValues, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool ShowYAxisValues {
		get { return Get<bool>(FusionChartApi.Chart_ShowYAxisValues); }
		set { Set(FusionChartApi.Chart_ShowYAxisValues, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool ShowLimits {
		get { return Get<bool>(FusionChartApi.Chart_ShowLimits); }
		set { Set(FusionChartApi.Chart_ShowLimits, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool ShowDivLineValues {
		get { return Get<bool>(FusionChartApi.Chart_ShowDivLineValues); }
		set { Set(FusionChartApi.Chart_ShowDivLineValues, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int YAxisValuesStep {
		get { return Get<int>(FusionChartApi.Chart_YAxisValuesStep); }
		set { Set(FusionChartApi.Chart_YAxisValuesStep, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool AdjustDiv {
		get { return Get<bool>(FusionChartApi.Chart_AdjustDiv); }
		set { Set(FusionChartApi.Chart_AdjustDiv, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool RotateYAxisName {
		get { return Get<bool>(FusionChartApi.Chart_RotateYAxisName); }
		set { Set(FusionChartApi.Chart_RotateYAxisName, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int YAxisNameWidth {
		get { return Get<int>(FusionChartApi.Chart_YAxisNameWidth); }
		set { Set(FusionChartApi.Chart_YAxisNameWidth, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int YAxisMinValue {
		get { return Get<int>(FusionChartApi.Chart_YAxisMinValue); }
		set { Set(FusionChartApi.Chart_YAxisMinValue, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int YAxisMaxValue {
		get { return Get<int>(FusionChartApi.Chart_YAxisMaxValue); }
		set { Set(FusionChartApi.Chart_YAxisMaxValue, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool CenterYaxisName {
		get { return Get<bool>(FusionChartApi.Chart_CenterYaxisName); }
		set { Set(FusionChartApi.Chart_CenterYaxisName, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public string YAxisName {
		get { return Get<string>(FusionChartApi.Chart_YAxisName); }
		set { Set(FusionChartApi.Chart_YAxisName, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int CanvasBgAlpha {
		get { return Get<int>(FusionChartApi.Chart_CanvasBgAlpha); }
		set { Set(FusionChartApi.Chart_CanvasBgAlpha, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int CanvasBgRatio {
		get { return Get<int>(FusionChartApi.Chart_CanvasBgRatio); }
		set { Set(FusionChartApi.Chart_CanvasBgRatio, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int CanvasBgAngle {
		get { return Get<int>(FusionChartApi.Chart_CanvasBgAngle); }
		set { Set(FusionChartApi.Chart_CanvasBgAngle, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public IChartColor CanvasBorderColor {
		get { return Get<IChartColor>(FusionChartApi.Chart_CanvasBorderColor); }
		set { Set(FusionChartApi.Chart_CanvasBorderColor, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int CanvasBorderThickness {
		get { return Get<int>(FusionChartApi.Chart_CanvasBorderThickness); }
		set { Set(FusionChartApi.Chart_CanvasBorderThickness, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int CanvasBorderAlpha {
		get { return Get<int>(FusionChartApi.Chart_CanvasBorderAlpha); }
		set { Set(FusionChartApi.Chart_CanvasBorderAlpha, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int NumDivLines {
		get { return Get<int>(FusionChartApi.Chart_NumDivLines); }
		set { Set(FusionChartApi.Chart_NumDivLines, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool DivLineIsDashed {
		get { return Get<bool>(FusionChartApi.Chart_DivLineIsDashed); }
		set { Set(FusionChartApi.Chart_DivLineIsDashed, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int DivLineDashLen {
		get { return Get<int>(FusionChartApi.Chart_DivLineDashLen); }
		set { Set(FusionChartApi.Chart_DivLineDashLen, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int DivLineDashGap {
		get { return Get<int>(FusionChartApi.Chart_DivLineDashGap); }
		set { Set(FusionChartApi.Chart_DivLineDashGap, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public IChartColor ZeroPlaneColor {
		get { return Get<IChartColor>(FusionChartApi.Chart_ZeroPlaneColor); }
		set { Set(FusionChartApi.Chart_ZeroPlaneColor, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int ZeroPlaneThickness {
		get { return Get<int>(FusionChartApi.Chart_ZeroPlaneThickness); }
		set { Set(FusionChartApi.Chart_ZeroPlaneThickness, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int ZeroPlaneAlpha {
		get { return Get<int>(FusionChartApi.Chart_ZeroPlaneAlpha); }
		set { Set(FusionChartApi.Chart_ZeroPlaneAlpha, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool ShowZeroPlaneValue {
		get { return Get<bool>(FusionChartApi.Chart_ShowZeroPlaneValue); }
		set { Set(FusionChartApi.Chart_ShowZeroPlaneValue, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool ShowAlternateHGridColor {
		get { return Get<bool>(FusionChartApi.Chart_ShowAlternateHGridColor); }
		set { Set(FusionChartApi.Chart_ShowAlternateHGridColor, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public IChartColor AlternateHGridColor {
		get { return Get<IChartColor>(FusionChartApi.Chart_AlternateHGridColor); }
		set { Set(FusionChartApi.Chart_AlternateHGridColor, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int AlternateHGridAlpha {
		get { return Get<int>(FusionChartApi.Chart_AlternateHGridAlpha); }
		set { Set(FusionChartApi.Chart_AlternateHGridAlpha, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int XAxisNamePadding {
		get { return Get<int>(FusionChartApi.Chart_XAxisNamePadding); }
		set { Set(FusionChartApi.Chart_XAxisNamePadding, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int YAxisNamePadding {
		get { return Get<int>(FusionChartApi.Chart_YAxisNamePadding); }
		set { Set(FusionChartApi.Chart_YAxisNamePadding, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int YAxisValuesPadding {
		get { return Get<int>(FusionChartApi.Chart_YAxisValuesPadding); }
		set { Set(FusionChartApi.Chart_YAxisValuesPadding, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int LabelPadding {
		get { return Get<int>(FusionChartApi.Chart_LabelPadding); }
		set { Set(FusionChartApi.Chart_LabelPadding, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int ValuePadding {
		get { return Get<int>(FusionChartApi.Chart_ValuePadding); }
		set { Set(FusionChartApi.Chart_ValuePadding, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int PlotSpacePercent {
		get { return Get<int>(FusionChartApi.Chart_PlotSpacePercent); }
		set { Set(FusionChartApi.Chart_PlotSpacePercent, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int CanvasLeftMargin {
		get { return Get<int>(FusionChartApi.Chart_CanvasLeftMargin); }
		set { Set(FusionChartApi.Chart_CanvasLeftMargin, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int CanvasRightMargin {
		get { return Get<int>(FusionChartApi.Chart_CanvasRightMargin); }
		set { Set(FusionChartApi.Chart_CanvasRightMargin, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int CanvasTopMargin {
		get { return Get<int>(FusionChartApi.Chart_CanvasTopMargin); }
		set { Set(FusionChartApi.Chart_CanvasTopMargin, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int CanvasBottomMargin {
		get { return Get<int>(FusionChartApi.Chart_CanvasBottomMargin); }
		set { Set(FusionChartApi.Chart_CanvasBottomMargin, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public IChartColor Color {
		get { return Get<IChartColor>(FusionChartApi.Chart_Color); }
		set { Set(FusionChartApi.Chart_Color, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int Thickness {
		get { return Get<int>(FusionChartApi.Chart_Thickness); }
		set { Set(FusionChartApi.Chart_Thickness, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int Alpha {
		get { return Get<int>(FusionChartApi.Chart_Alpha); }
		set { Set(FusionChartApi.Chart_Alpha, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool Dashed {
		get { return Get<bool>(FusionChartApi.Chart_Dashed); }
		set { Set(FusionChartApi.Chart_Dashed, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int DashLen {
		get { return Get<int>(FusionChartApi.Chart_DashLen); }
		set { Set(FusionChartApi.Chart_DashLen, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int DashGap {
		get { return Get<int>(FusionChartApi.Chart_DashGap); }
		set { Set(FusionChartApi.Chart_DashGap, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public string Label {
		get { return Get<string>(FusionChartApi.Chart_Label); }
		set { Set(FusionChartApi.Chart_Label, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool ShowLabelBorder {
		get { return Get<bool>(FusionChartApi.Chart_ShowLabelBorder); }
		set { Set(FusionChartApi.Chart_ShowLabelBorder, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int LinePosition {
		get { return Get<int>(FusionChartApi.Chart_LinePosition); }
		set { Set(FusionChartApi.Chart_LinePosition, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int LabelPosition {
		get { return Get<int>(FusionChartApi.Chart_LabelPosition); }
		set { Set(FusionChartApi.Chart_LabelPosition, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public string LabelHAlign {
		get { return Get<string>(FusionChartApi.Chart_LabelHAlign); }
		set { Set(FusionChartApi.Chart_LabelHAlign, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public string LabelVAlign {
		get { return Get<string>(FusionChartApi.Chart_LabelVAlign); }
		set { Set(FusionChartApi.Chart_LabelVAlign, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool IsTrendZone {
		get { return Get<bool>(FusionChartApi.Chart_IsTrendZone); }
		set { Set(FusionChartApi.Chart_IsTrendZone, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool ShowOnTop {
		get { return Get<bool>(FusionChartApi.Chart_ShowOnTop); }
		set { Set(FusionChartApi.Chart_ShowOnTop, value); }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool ValueOnRight {
		get { return Get<bool>(FusionChartApi.Chart_ValueOnRight); }
		set { Set(FusionChartApi.Chart_ValueOnRight, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public string ToolText {
		get { return Get<string>(FusionChartApi.Chart_ToolText); }
		set { Set(FusionChartApi.Chart_ToolText, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool LegendMarkerCircle {
		get { return Get<bool>(FusionChartApi.Chart_LegendMarkerCircle); }
		set { Set(FusionChartApi.Chart_LegendMarkerCircle, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public bool SeriesNameInToolTip {
		get { return Get<bool>(FusionChartApi.Chart_SeriesNameInToolTip); }
		set { Set(FusionChartApi.Chart_SeriesNameInToolTip, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int LegendPadding {
		get { return Get<int>(FusionChartApi.Chart_LegendPadding); }
		set { Set(FusionChartApi.Chart_LegendPadding, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public string Font {
		get { return Get<string>(FusionChartApi.Chart_Font); }
		set { Set(FusionChartApi.Chart_Font, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public int FontSize {
		get { return Get<int>(FusionChartApi.Chart_FontSize); }
		set { Set(FusionChartApi.Chart_FontSize, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public IChartColor FontColor {
		get { return Get<IChartColor>(FusionChartApi.Chart_FontColor); }
		set { Set(FusionChartApi.Chart_FontColor, value); }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool ShowLabel {
		get { return Get<bool>(FusionChartApi.Chart_ShowLabel); }
		set { Set(FusionChartApi.Chart_ShowLabel, value); }
    }
  
    /// <summary>
    /// 
    /// </summary>
    public string SeriesName {
		get { return Get<string>(FusionChartApi.Chart_SeriesName); }
		set { Set(FusionChartApi.Chart_SeriesName, value); }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool ShowValues {
		get { return Get<bool>(FusionChartApi.Chart_ShowValues); }
		set { Set(FusionChartApi.Chart_ShowValues, value); }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IncludeInLegend {
		get { return Get<bool>(FusionChartApi.Chart_IncludeInLegend); }
		set { Set(FusionChartApi.Chart_IncludeInLegend, value); }
    }
  }
}
  