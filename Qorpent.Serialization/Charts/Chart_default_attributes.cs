
using System;
using Qorpent.Charts.FusionCharts;
namespace Qorpent.Charts {
  public partial class Chart {

    
    ///<summary>Palette (FunctionalAttributes)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>FusionCharts XT uses the concept of Color
///Palettes. Each chart has 5 pre-defined color palettes which you can
///choose from. Each palette renders the chart in a different color
///theme. Valid values are 1-5.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public int Palette {
    get { return Get<int>( FusionChartApi.Chart_Palette ); }
        set { Set( FusionChartApi.Chart_Palette, value); }
    }
  
    
    ///<summary>PaletteColors (FunctionalAttributes)</summary>
    ///<remarks>
///&lt;desc>While the 
///&lt;span>palette&lt;/span> attribute allows to select
///a palette theme that applies to chart background, canvas, font and
///tool-tips, it does not change the colors of data items (i.e.,
///column, line, pie etc.). Using 
///&lt;span>paletteColors&lt;/span> attribute, you can
///specify your custom list of hex colors for the data items. The list
///of colors have to be separated by comma e.g., 
///&lt;span>&amp;lt;chart
///paletteColors='FF0000,0372AB,FF5904...'&amp;gt;&lt;/span>. The chart will
///cycle through the list of specified colors and then render the data
///plot accordingly.
///&lt;p>To use the same set of colors throughout all your charts in a
///web application, you can store the list of palette colors in your
///application globally and then provide the same in each chart
///XML.&lt;/p>&lt;/desc>
    ///</remarks>
    public string PaletteColors {
    get { return Get<string>( FusionChartApi.Chart_PaletteColors ); }
        set { Set( FusionChartApi.Chart_PaletteColors, value); }
    }
  
    
    ///<summary>ShowAboutMenuItem (FunctionalAttributes)</summary>
    ///<remarks>
///&lt;desc>Setting this to 1 shows up a custom
///context menu in the chart, which can be customized to show your
///text and can be linked to your Url. For e.g., you can set the
///context menu of the chart to include "About your company name" and
///then link to your company home page. By default, the chart shows
///"About FusionCharts" when right clicked.&lt;/desc>
    ///</remarks>
    public bool ShowAboutMenuItem {
    get { return Get<bool>( FusionChartApi.Chart_ShowAboutMenuItem ); }
        set { Set( FusionChartApi.Chart_ShowAboutMenuItem, value); }
    }
  
    
    ///<summary>AboutMenuItemLabel (FunctionalAttributes)</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public string AboutMenuItemLabel {
    get { return Get<string>( FusionChartApi.Chart_AboutMenuItemLabel ); }
        set { Set( FusionChartApi.Chart_AboutMenuItemLabel, value); }
    }
  
    
    ///<summary>AboutMenuItemLink (FunctionalAttributes)</summary>
    ///<remarks>
///&lt;desc>Link for the custom context menu
///item. You can specify the link in 
///&lt;a href="http://docs.fusioncharts.com/charts/contents/DrillDown/LinkFormat.html" target="_blank">FusionCharts Link format&lt;/a>
///to be able to open the same in new window, pop-ups, frames or as
///JavaScript links.&lt;/desc>
    ///</remarks>
    public string AboutMenuItemLink {
    get { return Get<string>( FusionChartApi.Chart_AboutMenuItemLink ); }
        set { Set( FusionChartApi.Chart_AboutMenuItemLink, value); }
    }
  
    
    ///<summary>ShowLabels (FunctionalAttributes)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to show labels on the chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public bool ShowLabels {
    get { return Get<bool>( FusionChartApi.Chart_ShowLabels ); }
        set { Set( FusionChartApi.Chart_ShowLabels, value); }
    }
  
    
    ///<summary>ShowValues (FunctionalAttributes)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to show values on the chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public bool ShowValues {
    get { return Get<bool>( FusionChartApi.Chart_ShowValues ); }
        set { Set( FusionChartApi.Chart_ShowValues, value); }
    }
  
    
    ///<summary>Caption (ChartTitles)</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public string Caption {
    get { return Get<string>( FusionChartApi.Chart_Caption ); }
        set { Set( FusionChartApi.Chart_Caption, value); }
    }
  
    
    ///<summary>SubCaption (ChartTitles)</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public string SubCaption {
    get { return Get<string>( FusionChartApi.Chart_SubCaption ); }
        set { Set( FusionChartApi.Chart_SubCaption, value); }
    }
  
    
    ///<summary>ShowBorder (ChartCosmetics)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to show a border around the chart or
///not.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public bool ShowBorder {
    get { return Get<bool>( FusionChartApi.Chart_ShowBorder ); }
        set { Set( FusionChartApi.Chart_ShowBorder, value); }
    }
  
    
    ///<summary>BorderColor (ChartCosmetics)</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public FusionChartColor BorderColor {
    get { return Get<FusionChartColor>( FusionChartApi.Chart_BorderColor ); }
        set { Set( FusionChartApi.Chart_BorderColor, value); }
    }
  
    
    ///<summary>BorderThickness (ChartCosmetics)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Border thickness of the chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public int BorderThickness {
    get { return Get<int>( FusionChartApi.Chart_BorderThickness ); }
        set { Set( FusionChartApi.Chart_BorderThickness, value); }
    }
  
    
    ///<summary>BorderAlpha (ChartCosmetics)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Border alpha of the chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public int BorderAlpha {
    get { return Get<int>( FusionChartApi.Chart_BorderAlpha ); }
        set { Set( FusionChartApi.Chart_BorderAlpha, value); }
    }
  
    
    ///<summary>BgColor (ChartCosmetics)</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public FusionChartColor BgColor {
    get { return Get<FusionChartColor>( FusionChartApi.Chart_BgColor ); }
        set { Set( FusionChartApi.Chart_BgColor, value); }
    }
  
    
    ///<summary>BgAlpha (ChartCosmetics)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Sets the alpha (transparency) for the
///background. If you've opted for gradient background, you need to
///set a list of alpha(s) separated by comma. See 
///&lt;strong>Advanced charting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/Gradients.html" target="_blank">Using Gradients&lt;/a> page for
///more details.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public int BgAlpha {
    get { return Get<int>( FusionChartApi.Chart_BgAlpha ); }
        set { Set( FusionChartApi.Chart_BgAlpha, value); }
    }
  
    
    ///<summary>BgRatio (ChartCosmetics)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you've opted for a gradient background, this
///attribute lets you set the ratio of each color constituent. See 
///&lt;strong>Advanced charting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/Gradients.html" target="_blank">Using Gradients&lt;/a> page for
///more details.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public int BgRatio {
    get { return Get<int>( FusionChartApi.Chart_BgRatio ); }
        set { Set( FusionChartApi.Chart_BgRatio, value); }
    }
  
    
    ///<summary>BgAngle (ChartCosmetics)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Sets the angle of the background color, in case
///of a gradient. See 
///&lt;strong>Advanced charting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/Gradients.html" target="_blank">Using Gradients&lt;/a> page for
///more details.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public int BgAngle {
    get { return Get<int>( FusionChartApi.Chart_BgAngle ); }
        set { Set( FusionChartApi.Chart_BgAngle, value); }
    }
  
    
    ///<summary>BgImage (ChartCosmetics)</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public string BgImage {
    get { return Get<string>( FusionChartApi.Chart_BgImage ); }
        set { Set( FusionChartApi.Chart_BgImage, value); }
    }
  
    
    ///<summary>BgImageAlpha (ChartCosmetics)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Helps you specify alpha for the loaded
///background image or Flash movie.&lt;/span>
///  &lt;p>
///    &lt;span>Loading of Flash movie is deprecated and is not
///supported by JavaScript charts.&lt;/span>
///  &lt;/p>
///&lt;/desc>
    ///</remarks>
    public int BgImageAlpha {
    get { return Get<int>( FusionChartApi.Chart_BgImageAlpha ); }
        set { Set( FusionChartApi.Chart_BgImageAlpha, value); }
    }
  
    
    ///<summary>BgImageDisplayMode (ChartCosmetics)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Helps you specify the mode in which the
///background image is to be displayed.&lt;/span>
///  &lt;ul>
///    &lt;li>
///      &lt;span>Stretch - expands the image to fit the
///entire chart area, without maintaining original image
///constraints&lt;/span>
///    &lt;/li>
///    &lt;li>
///      &lt;span>Tile - the image is repeated as a pattern on the
///entire chart area&lt;/span>
///    &lt;/li>
///    &lt;li>
///      &lt;span>Fit - fits the image proportionately on the
///chart area&lt;/span>
///    &lt;/li>
///    &lt;li>
///      &lt;span>Fill -proportionately fills the entire chart
///area with the image&lt;/span>
///    &lt;/li>
///    &lt;li>
///      &lt;span>Center - the image is positioned at the center
///of the chart area&lt;/span>
///    &lt;/li>
///    &lt;li>
///      &lt;span>None - Default mode. None of the above modes are
///applied&lt;/span>
///    &lt;/li>
///  &lt;/ul>For more details 
///&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/BgSWF.html#displaymode" target="_blank">click here&lt;/a>.&lt;/desc>
    ///</remarks>
    public string BgImageDisplayMode {
    get { return Get<string>( FusionChartApi.Chart_BgImageDisplayMode ); }
        set { Set( FusionChartApi.Chart_BgImageDisplayMode, value); }
    }
  
    
    ///<summary>BgImageVAlign (ChartCosmetics)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Helps you to vertically align the background
///image.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public string BgImageVAlign {
    get { return Get<string>( FusionChartApi.Chart_BgImageVAlign ); }
        set { Set( FusionChartApi.Chart_BgImageVAlign, value); }
    }
  
    
    ///<summary>BgImageHAlign (ChartCosmetics)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Helps you to horizontally align the background
///image.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public string BgImageHAlign {
    get { return Get<string>( FusionChartApi.Chart_BgImageHAlign ); }
        set { Set( FusionChartApi.Chart_BgImageHAlign, value); }
    }
  
    
    ///<summary>BgImageScale (ChartCosmetics)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Helps you magnify the background image.This
///attribute will only work when the attribute&lt;/span>
///  &lt;span>bgImageDisplayMode&lt;/span>
///  &lt;span>is set to none, center or tile.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public int BgImageScale {
    get { return Get<int>( FusionChartApi.Chart_BgImageScale ); }
        set { Set( FusionChartApi.Chart_BgImageScale, value); }
    }
  
    
    ///<summary>LogoURL (ChartCosmetics)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>You can load an external logo (JPEG/PNG/SWF) on
///the chart once it has rendered. This attribute lets you specify the
///URL of the same. Owing to Flash Player security settings, you can
///only specify logo that are on the same sub-domain as that of the
///SWF file of the chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public string LogoURL {
    get { return Get<string>( FusionChartApi.Chart_LogoURL ); }
        set { Set( FusionChartApi.Chart_LogoURL, value); }
    }
  
    
    ///<summary>LogoPosition (ChartCosmetics)</summary>
    ///<remarks>
///&lt;desc>Where to position the logo on the
///chart:
///&lt;ul>&lt;li>&lt;span>TL - Top-left&lt;/span>&lt;/li>&lt;li>&lt;span>TR - Top-right&lt;/span>&lt;/li>&lt;li>&lt;span>BR - Bottom right&lt;/span>&lt;/li>&lt;li>&lt;span>BL - Bottom left&lt;/span>&lt;/li>&lt;li>&lt;span>CC - Center of screen&lt;/span>&lt;/li>&lt;/ul>&lt;/desc>
    ///</remarks>
    public string LogoPosition {
    get { return Get<string>( FusionChartApi.Chart_LogoPosition ); }
        set { Set( FusionChartApi.Chart_LogoPosition, value); }
    }
  
    
    ///<summary>LogoAlpha (ChartCosmetics)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Once the logo has loaded on the chart, you can
///configure its opacity using this attribute.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public int LogoAlpha {
    get { return Get<int>( FusionChartApi.Chart_LogoAlpha ); }
        set { Set( FusionChartApi.Chart_LogoAlpha, value); }
    }
  
    
    ///<summary>LogoScale (ChartCosmetics)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>You can also change the scale of externally
///loaded logo at run-time by specifying a value for this
///parameter.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public int LogoScale {
    get { return Get<int>( FusionChartApi.Chart_LogoScale ); }
        set { Set( FusionChartApi.Chart_LogoScale, value); }
    }
  
    
    ///<summary>LogoLink (ChartCosmetics)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you want to link the logo to an external URL,
///specify the link in this attribute. The link can be in 
///&lt;a href="http://docs.fusioncharts.com/charts/contents/DrillDown/LinkFormat.html" target="_blank">FusionCharts Link
///format&lt;/a>, allowing you to link to new windows, pop-ups, frames
///etc.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public string LogoLink {
    get { return Get<string>( FusionChartApi.Chart_LogoLink ); }
        set { Set( FusionChartApi.Chart_LogoLink, value); }
    }
  
    
    ///<summary>FormatNumber (NumberFormatting)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This configuration determines whether the
///numbers displayed on the chart will be formatted using commas,
///e.g., 40,000 if 
///&lt;span>formatNumber='1'&lt;/span> and 40000 if 
///&lt;span>formatNumber= '0'&lt;/span>. For more
///details, please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Basics.html" target="_blank">Basics&lt;/a>
///page.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public bool FormatNumber {
    get { return Get<bool>( FusionChartApi.Chart_FormatNumber ); }
        set { Set( FusionChartApi.Chart_FormatNumber, value); }
    }
  
    
    ///<summary>FormatNumberScale (NumberFormatting)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Configuration whether to add K (thousands) and M
///(millions) to a number after truncating and rounding it - e.g., if 
///&lt;span>formatNumberScale&lt;/span> is set to 1, 1043
///will become 1.04K (with decimals set to 2 places). Same with
///numbers in millions - an M will be added at the end. For more
///details, please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Scaling.html" target="_blank">Number
///Scaling&lt;/a> page.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public bool FormatNumberScale {
    get { return Get<bool>( FusionChartApi.Chart_FormatNumberScale ); }
        set { Set( FusionChartApi.Chart_FormatNumberScale, value); }
    }
  
    
    ///<summary>DefaultNumberScale (NumberFormatting)</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public string DefaultNumberScale {
    get { return Get<string>( FusionChartApi.Chart_DefaultNumberScale ); }
        set { Set( FusionChartApi.Chart_DefaultNumberScale, value); }
    }
  
    
    ///<summary>NumberScaleUnit (NumberFormatting)</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public string NumberScaleUnit {
    get { return Get<string>( FusionChartApi.Chart_NumberScaleUnit ); }
        set { Set( FusionChartApi.Chart_NumberScaleUnit, value); }
    }
  
    
    ///<summary>NumberScaleValue (NumberFormatting)</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public string NumberScaleValue {
    get { return Get<string>( FusionChartApi.Chart_NumberScaleValue ); }
        set { Set( FusionChartApi.Chart_NumberScaleValue, value); }
    }
  
    
    ///<summary>ScaleRecursively (NumberFormatting)</summary>
    ///<remarks>
///&lt;desc>Whether to scale the number
///recursively? For more details, please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Rec_Num_Scaling.html" target="_blank">Recursive
///Number Scaling&lt;/a> page.&lt;/desc>
    ///</remarks>
    public bool ScaleRecursively {
    get { return Get<bool>( FusionChartApi.Chart_ScaleRecursively ); }
        set { Set( FusionChartApi.Chart_ScaleRecursively, value); }
    }
  
    
    ///<summary>MaxScaleRecursion (NumberFormatting)</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public int MaxScaleRecursion {
    get { return Get<int>( FusionChartApi.Chart_MaxScaleRecursion ); }
        set { Set( FusionChartApi.Chart_MaxScaleRecursion, value); }
    }
  
    
    ///<summary>ScaleSeparator (NumberFormatting)</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public string ScaleSeparator {
    get { return Get<string>( FusionChartApi.Chart_ScaleSeparator ); }
        set { Set( FusionChartApi.Chart_ScaleSeparator, value); }
    }
  
    
    ///<summary>NumberPrefix (NumberFormatting)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Using this attribute, you could add prefix to
///all the numbers visible on the graph. For example, to represent all
///dollars figure on the chart, you could specify this attribute to '
///$' to show like $40000, $50000. For more details, please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Basics.html#prefix" target="_blank">Basics&lt;/a>
///page.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public string NumberPrefix {
    get { return Get<string>( FusionChartApi.Chart_NumberPrefix ); }
        set { Set( FusionChartApi.Chart_NumberPrefix, value); }
    }
  
    
    ///<summary>NumberSuffix (NumberFormatting)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Using this attribute, you could add a suffix to
///all the numbers visible on the graph. For example, to represent all
///figure quantified as per annum on the chart, you could specify this
///attribute to ' /a' to show like 40000/a, 50000/a. For more details,
///please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Basics.html#prefix" target="_blank">Basics&lt;/a>
///page.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public string NumberSuffix {
    get { return Get<string>( FusionChartApi.Chart_NumberSuffix ); }
        set { Set( FusionChartApi.Chart_NumberSuffix, value); }
    }
  
    
    ///<summary>DecimalSeparator (NumberFormatting)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This option helps you specify the character to
///be used as the decimal separator in a number. For more details,
///please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Basics.html#separator" target="_blank">Basics&lt;/a>
///page.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public string DecimalSeparator {
    get { return Get<string>( FusionChartApi.Chart_DecimalSeparator ); }
        set { Set( FusionChartApi.Chart_DecimalSeparator, value); }
    }
  
    
    ///<summary>ThousandSeparator (NumberFormatting)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This option helps you specify the character to
///be used as the thousands separator in a number. For more details,
///please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Basics.html#separator" target="_blank">Basics&lt;/a>
///page.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public string ThousandSeparator {
    get { return Get<string>( FusionChartApi.Chart_ThousandSeparator ); }
        set { Set( FusionChartApi.Chart_ThousandSeparator, value); }
    }
  
    
    ///<summary>ThousandSeparatorPosition (NumberFormatting)</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public int ThousandSeparatorPosition {
    get { return Get<int>( FusionChartApi.Chart_ThousandSeparatorPosition ); }
        set { Set( FusionChartApi.Chart_ThousandSeparatorPosition, value); }
    }
  
    
    ///<summary>Decimals (NumberFormatting)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Number of decimal places to which all numbers on
///the chart will be rounded to. For more details, please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Basics.html#yaxis" target="_blank">Basics&lt;/a>
///page.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public int Decimals {
    get { return Get<int>( FusionChartApi.Chart_Decimals ); }
        set { Set( FusionChartApi.Chart_Decimals, value); }
    }
  
    
    ///<summary>ForceDecimals (NumberFormatting)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to add 0 padding at the end of decimal
///numbers. For example, if you set decimals as 2 and a number is
///23.4. If forceDecimals is set to 1, FusionCharts XT will convert
///the number to 23.40 (note the extra 0 at the end). For more
///details, please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Basics.html#yaxis" target="_blank">Basics&lt;/a>
///page.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public bool ForceDecimals {
    get { return Get<bool>( FusionChartApi.Chart_ForceDecimals ); }
        set { Set( FusionChartApi.Chart_ForceDecimals, value); }
    }
  
    
    ///<summary>BaseFont (FontProperties)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute lets you set the font face
///(family) of all the text (data labels, values etc.) on
///chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public string BaseFont {
    get { return Get<string>( FusionChartApi.Chart_BaseFont ); }
        set { Set( FusionChartApi.Chart_BaseFont, value); }
    }
  
    
    ///<summary>BaseFontSize (FontProperties)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute sets the base font size of the
///chart i.e., all the values and the names in the chart which lie on
///the canvas will be displayed using the font size provided
///here.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public int BaseFontSize {
    get { return Get<int>( FusionChartApi.Chart_BaseFontSize ); }
        set { Set( FusionChartApi.Chart_BaseFontSize, value); }
    }
  
    
    ///<summary>BaseFontColor (FontProperties)</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public FusionChartColor BaseFontColor {
    get { return Get<FusionChartColor>( FusionChartApi.Chart_BaseFontColor ); }
        set { Set( FusionChartApi.Chart_BaseFontColor, value); }
    }
  
    
    ///<summary>ShowToolTip (Tool-tip)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to show tool tip on chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public bool ShowToolTip {
    get { return Get<bool>( FusionChartApi.Chart_ShowToolTip ); }
        set { Set( FusionChartApi.Chart_ShowToolTip, value); }
    }
  
    
    ///<summary>ToolTipBgColor (Tool-tip)</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public FusionChartColor ToolTipBgColor {
    get { return Get<FusionChartColor>( FusionChartApi.Chart_ToolTipBgColor ); }
        set { Set( FusionChartApi.Chart_ToolTipBgColor, value); }
    }
  
    
    ///<summary>ToolTipBorderColor (Tool-tip)</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public FusionChartColor ToolTipBorderColor {
    get { return Get<FusionChartColor>( FusionChartApi.Chart_ToolTipBorderColor ); }
        set { Set( FusionChartApi.Chart_ToolTipBorderColor, value); }
    }
  
    
    ///<summary>ShowToolTipShadow (Tool-tip)</summary>
    ///<remarks>
///&lt;desc>Whether to show shadow for tool-tips
///on the chart.&lt;/desc>
    ///</remarks>
    public bool ShowToolTipShadow {
    get { return Get<bool>( FusionChartApi.Chart_ShowToolTipShadow ); }
        set { Set( FusionChartApi.Chart_ShowToolTipShadow, value); }
    }
  
    
    ///<summary>CaptionPadding (ChartPadding&amp;Margins)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute lets you control the space (in
///pixels) between the sub-caption and top of the pie. If the
///sub-caption is not defined, it controls the space between caption
///and top of the pie. If neither caption, nor sub-caption is defined,
///this padding does not come into play.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public int CaptionPadding {
    get { return Get<int>( FusionChartApi.Chart_CaptionPadding ); }
        set { Set( FusionChartApi.Chart_CaptionPadding, value); }
    }
  
    
    ///<summary>ChartLeftMargin (ChartPadding&amp;Margins)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Amount of empty space that you want to put on
///the left side of your chart. Nothing is rendered in this
///space.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public int ChartLeftMargin {
    get { return Get<int>( FusionChartApi.Chart_ChartLeftMargin ); }
        set { Set( FusionChartApi.Chart_ChartLeftMargin, value); }
    }
  
    
    ///<summary>ChartRightMargin (ChartPadding&amp;Margins)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Amount of empty space that you want to put on
///the right side of your chart. Nothing is rendered in this
///space.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public int ChartRightMargin {
    get { return Get<int>( FusionChartApi.Chart_ChartRightMargin ); }
        set { Set( FusionChartApi.Chart_ChartRightMargin, value); }
    }
  
    
    ///<summary>ChartTopMargin (ChartPadding&amp;Margins)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Amount of empty space that you want to put on
///the top of your chart. Nothing is rendered in this space.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public int ChartTopMargin {
    get { return Get<int>( FusionChartApi.Chart_ChartTopMargin ); }
        set { Set( FusionChartApi.Chart_ChartTopMargin, value); }
    }
  
    
    ///<summary>ChartBottomMargin (ChartPadding&amp;Margins)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Amount of empty space that you want to put on
///the bottom of your chart. Nothing is rendered in this space.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public int ChartBottomMargin {
    get { return Get<int>( FusionChartApi.Chart_ChartBottomMargin ); }
        set { Set( FusionChartApi.Chart_ChartBottomMargin, value); }
    }
  
    
    ///<summary>XAxisName (ChartTitlesAndAxisNames)</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public string XAxisName {
    get { return Get<string>( FusionChartApi.Chart_XAxisName ); }
        set { Set( FusionChartApi.Chart_XAxisName, value); }
    }
  
    
    ///<summary>CanvasBgColor (ChartCosmetics)</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public FusionChartColor CanvasBgColor {
    get { return Get<FusionChartColor>( FusionChartApi.Chart_CanvasBgColor ); }
        set { Set( FusionChartApi.Chart_CanvasBgColor, value); }
    }
  
    
    ///<summary>ShowVLineLabelBorder (ChartCosmetics)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you've opted to show a label for any of your
///vLines in the chart, you can collectively configure whether to show
///border for all such labels using this attribute. If you want to
///show label border for just a particular vLine, you can over-ride
///this value by specifying border configuration for that specific
///vLine.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public bool ShowVLineLabelBorder {
    get { return Get<bool>( FusionChartApi.Chart_ShowVLineLabelBorder ); }
        set { Set( FusionChartApi.Chart_ShowVLineLabelBorder, value); }
    }
  
    
    ///<summary>DivLineColor (DivisionalLines&amp;Grids)</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public FusionChartColor DivLineColor {
    get { return Get<FusionChartColor>( FusionChartApi.Chart_DivLineColor ); }
        set { Set( FusionChartApi.Chart_DivLineColor, value); }
    }
  
    
    ///<summary>DivLineThickness (DivisionalLines&amp;Grids)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Thickness of divisional lines.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public int DivLineThickness {
    get { return Get<int>( FusionChartApi.Chart_DivLineThickness ); }
        set { Set( FusionChartApi.Chart_DivLineThickness, value); }
    }
  
    
    ///<summary>DivLineAlpha (DivisionalLines&amp;Grids)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Alpha of divisional lines.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public int DivLineAlpha {
    get { return Get<int>( FusionChartApi.Chart_DivLineAlpha ); }
        set { Set( FusionChartApi.Chart_DivLineAlpha, value); }
    }
  
    
    ///<summary>ForceYAxisValueDecimals (NumberFormatting)</summary>
    ///<remarks>
///&lt;desc>Whether to forcefully
///attach decimal places to all y-axis values. For example, If you
///limit the maximum number of decimal digits to 2, a number like
///55.345 will be rounded to 55.34. This does not mean that all y-axis
///numbers will be displayed with a fixed number of decimal places.
///For instance 55 will not be displayed as 55.00 and 55.1 will not
///become 55.10. In order to have fixed number of decimal places
///attached to all y-axis numbers, set this attribute to 1. For more
///details, please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Basics.html" target="_blank">Basics&lt;/a>
///page.&lt;/desc>
    ///</remarks>
    public bool ForceYAxisValueDecimals {
    get { return Get<bool>( FusionChartApi.Chart_ForceYAxisValueDecimals ); }
        set { Set( FusionChartApi.Chart_ForceYAxisValueDecimals, value); }
    }
  
    
    ///<summary>YAxisValueDecimals (NumberFormatting)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you've opted to not adjust div lines, you can
///specify the div line values decimal precision using this attribute.
///For more details, please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Basics.html" target="_blank">Basics&lt;/a>
///page.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public int YAxisValueDecimals {
    get { return Get<int>( FusionChartApi.Chart_YAxisValueDecimals ); }
        set { Set( FusionChartApi.Chart_YAxisValueDecimals, value); }
    }
  
    
    ///<summary>OutCnvBaseFont (FontProperties)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute sets the base font family of the
///chart font which lies outside the canvas i.e., all the values and
///the names in the chart which lie outside the canvas will be
///displayed using the font name provided here.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public string OutCnvBaseFont {
    get { return Get<string>( FusionChartApi.Chart_OutCnvBaseFont ); }
        set { Set( FusionChartApi.Chart_OutCnvBaseFont, value); }
    }
  
    
    ///<summary>OutCnvBaseFontSize (FontProperties)</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute sets the base font size of the
///chart i.e., all the values and the names in the chart which lie
///outside the canvas will be displayed using the font size provided
///here.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public int OutCnvBaseFontSize {
    get { return Get<int>( FusionChartApi.Chart_OutCnvBaseFontSize ); }
        set { Set( FusionChartApi.Chart_OutCnvBaseFontSize, value); }
    }
  
    
    ///<summary>OutCnvBaseFontColor (FontProperties)</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public FusionChartColor OutCnvBaseFontColor {
    get { return Get<FusionChartColor>( FusionChartApi.Chart_OutCnvBaseFontColor ); }
        set { Set( FusionChartApi.Chart_OutCnvBaseFontColor, value); }
    }
  
   
   }
}
