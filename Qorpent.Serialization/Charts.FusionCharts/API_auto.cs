
namespace Qorpent.Charts.FusionCharts {
///<summary>
///Описывает атрибуты и прочие соглашения по атрибутам FusionChart
///</summary>
  public static partial class Api {

    ///<summary>Animation</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute gives you the option to control
///animation in your charts. If you do not want to animate any part of
///the chart, set this as 0.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_Animation = "animation";
  
    ///<summary>Palette</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>FusionCharts XT uses the concept of Color
///Palettes. Each chart has 5 pre-defined color palettes which you can
///choose from. Each palette renders the chart in a different color
///theme. Valid values are 1-5.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_Palette = "palette";
  
    ///<summary>PaletteColors</summary>
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
    public const string Chart_PaletteColors = "paletteColors";
  
    ///<summary>ShowAboutMenuItem</summary>
    ///<remarks>
///&lt;desc>Setting this to 1 shows up a custom
///context menu in the chart, which can be customized to show your
///text and can be linked to your Url. For e.g., you can set the
///context menu of the chart to include "About your company name" and
///then link to your company home page. By default, the chart shows
///"About FusionCharts" when right clicked.&lt;/desc>
    ///</remarks>
    public const string Chart_ShowAboutMenuItem = "showAboutMenuItem";
  
    ///<summary>AboutMenuItemLabel</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_AboutMenuItemLabel = "aboutMenuItemLabel";
  
    ///<summary>AboutMenuItemLink</summary>
    ///<remarks>
///&lt;desc>Link for the custom context menu
///item. You can specify the link in 
///&lt;a href="http://docs.fusioncharts.com/charts/contents/DrillDown/LinkFormat.html" target="_blank">FusionCharts Link format&lt;/a>
///to be able to open the same in new window, pop-ups, frames or as
///JavaScript links.&lt;/desc>
    ///</remarks>
    public const string Chart_AboutMenuItemLink = "aboutMenuItemLink";
  
    ///<summary>ShowZeroPies</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Configuration whether to show pies with 0 values
///(and their values). Otherwise, they won't show up in the
///chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ShowZeroPies = "showZeroPies";
  
    ///<summary>ShowPercentValues</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to show percentage values in labels of
///the chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ShowPercentValues = "showPercentValues";
  
    ///<summary>ShowPercentInToolTip</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to show percentage values in tool
///tip.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ShowPercentInToolTip = "showPercentInToolTip";
  
    ///<summary>ShowLabels</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to show labels on the chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ShowLabels = "showLabels";
  
    ///<summary>ShowValues</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to show values on the chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ShowValues = "showValues";
  
    ///<summary>LabelSepChar</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>The character to separate the data label and
///data values on the chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_LabelSepChar = "labelSepChar";
  
    ///<summary>DefaultAnimation</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>By default, each chart animates some of its
///elements. If you wish to switch off the default animation patterns,
///you can set this attribute to 0. It can be particularly useful when
///you want to define your own animation patterns using STYLE
///feature.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_DefaultAnimation = "defaultAnimation";
  
    ///<summary>ClickURL</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_ClickURL = "clickURL";
  
    ///<summary>Caption</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_Caption = "caption";
  
    ///<summary>SubCaption</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_SubCaption = "subCaption";
  
    ///<summary>ShowBorder</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to show a border around the chart or
///not.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ShowBorder = "showBorder";
  
    ///<summary>BorderColor</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_BorderColor = "borderColor";
  
    ///<summary>BorderThickness</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Border thickness of the chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_BorderThickness = "borderThickness";
  
    ///<summary>BorderAlpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Border alpha of the chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_BorderAlpha = "borderAlpha";
  
    ///<summary>BgColor</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_BgColor = "bgColor";
  
    ///<summary>BgAlpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Sets the alpha (transparency) for the
///background. If you've opted for gradient background, you need to
///set a list of alpha(s) separated by comma. See 
///&lt;strong>Advanced charting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/Gradients.html" target="_blank">Using Gradients&lt;/a> page for
///more details.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_BgAlpha = "bgAlpha";
  
    ///<summary>BgRatio</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you've opted for a gradient background, this
///attribute lets you set the ratio of each color constituent. See 
///&lt;strong>Advanced charting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/Gradients.html" target="_blank">Using Gradients&lt;/a> page for
///more details.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_BgRatio = "bgRatio";
  
    ///<summary>BgAngle</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Sets the angle of the background color, in case
///of a gradient. See 
///&lt;strong>Advanced charting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/Gradients.html" target="_blank">Using Gradients&lt;/a> page for
///more details.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_BgAngle = "bgAngle";
  
    ///<summary>BgImage</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_BgImage = "bgImage";
  
    ///<summary>BgImageAlpha</summary>
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
    public const string Chart_BgImageAlpha = "bgImageAlpha";
  
    ///<summary>BgImageDisplayMode</summary>
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
    public const string Chart_BgImageDisplayMode = "bgImageDisplayMode";
  
    ///<summary>BgImageVAlign</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Helps you to vertically align the background
///image.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_BgImageVAlign = "bgImageVAlign";
  
    ///<summary>BgImageHAlign</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Helps you to horizontally align the background
///image.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_BgImageHAlign = "bgImageHAlign";
  
    ///<summary>BgImageScale</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Helps you magnify the background image.This
///attribute will only work when the attribute&lt;/span>
///  &lt;span>bgImageDisplayMode&lt;/span>
///  &lt;span>is set to none, center or tile.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_BgImageScale = "bgImageScale";
  
    ///<summary>LogoURL</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>You can load an external logo (JPEG/PNG/SWF) on
///the chart once it has rendered. This attribute lets you specify the
///URL of the same. Owing to Flash Player security settings, you can
///only specify logo that are on the same sub-domain as that of the
///SWF file of the chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_LogoURL = "logoURL";
  
    ///<summary>LogoPosition</summary>
    ///<remarks>
///&lt;desc>Where to position the logo on the
///chart:
///&lt;ul>&lt;li>&lt;span>TL - Top-left&lt;/span>&lt;/li>&lt;li>&lt;span>TR - Top-right&lt;/span>&lt;/li>&lt;li>&lt;span>BR - Bottom right&lt;/span>&lt;/li>&lt;li>&lt;span>BL - Bottom left&lt;/span>&lt;/li>&lt;li>&lt;span>CC - Center of screen&lt;/span>&lt;/li>&lt;/ul>&lt;/desc>
    ///</remarks>
    public const string Chart_LogoPosition = "logoPosition";
  
    ///<summary>LogoAlpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Once the logo has loaded on the chart, you can
///configure its opacity using this attribute.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_LogoAlpha = "logoAlpha";
  
    ///<summary>LogoScale</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>You can also change the scale of externally
///loaded logo at run-time by specifying a value for this
///parameter.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_LogoScale = "logoScale";
  
    ///<summary>LogoLink</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you want to link the logo to an external URL,
///specify the link in this attribute. The link can be in 
///&lt;a href="http://docs.fusioncharts.com/charts/contents/DrillDown/LinkFormat.html" target="_blank">FusionCharts Link
///format&lt;/a>, allowing you to link to new windows, pop-ups, frames
///etc.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_LogoLink = "logoLink";
  
    ///<summary>ShowPlotBorder</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether the column, area, pie etc. border will
///show up.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ShowPlotBorder = "showPlotBorder";
  
    ///<summary>PlotBorderColor</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_PlotBorderColor = "plotBorderColor";
  
    ///<summary>PlotBorderThickness</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Thickness for column, area, pie border&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_PlotBorderThickness = "plotBorderThickness";
  
    ///<summary>PlotBorderAlpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Alpha for column, area, pie border&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_PlotBorderAlpha = "plotBorderAlpha";
  
    ///<summary>PlotFillAlpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute lets you set the fill alpha for
///plot.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_PlotFillAlpha = "plotFillAlpha";
  
    ///<summary>ShowShadow</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to show shadow for pie/doughnuts.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ShowShadow = "showShadow";
  
    ///<summary>Use3DLighting</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to use advanced gradients and shadow
///effects to create better looking 3D charts.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_Use3DLighting = "use3DLighting";
  
    ///<summary>Radius3D</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>You can define the 3D Radius of chart in
///percentage using this attribute. It basically helps you set the
///bevel distance for the pie/doughnut (if in 3D Lighting
///Mode).&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_Radius3D = "radius3D";
  
    ///<summary>SlicingDistance</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you've opted to slice a particular
///pie/doughnut slice, using this attribute you can control the
///distance between the slice and the center of chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_SlicingDistance = "slicingDistance";
  
    ///<summary>PieRadius</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute lets you explicitly set the outer
///radius of the chart. FusionCharts XT automatically calculates the
///best fit pie radius for the chart. This attribute is useful if you
///want to enforce one of your own values.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_PieRadius = "pieRadius";
  
    ///<summary>StartingAngle</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>By default, the pie chart starts from angle 0
///degree i.e., the first pie slice starts plotting from 0 degree
///angle. If you want to change the starting angle of the chart, use
///this attribute.
///It obeys the conventions of co-ordinate geometry where 0
///degrees means hand of a clock at 3. Starting angle Increases
///anti-clockwise.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_StartingAngle = "startingAngle";
  
    ///<summary>EnableRotation</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>The pie charts have three modes: Slicing,
///Rotation and Link. By default, a chart starts in Slicing mode. If
///you need to enable rotation from XML, set this attribute to
///1.&lt;/span>
///  &lt;span>But, when links are defined, the chart always
///works in Link mode irrespective of the value of this
///attribute.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_EnableRotation = "enableRotation";
  
    ///<summary>EnableSmartLabels</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to use smart labels or not.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_EnableSmartLabels = "enableSmartLabels";
  
    ///<summary>SkipOverlapLabels</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to skip labels that are overlapping even
///when using smart labels. If not, they might overlap if there are
///too many labels.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_SkipOverlapLabels = "skipOverlapLabels";
  
    ///<summary>IsSmartLineSlanted</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>The smart lines (smart label connector lines)
///can appear in two ways: Slanted or Straight. This attribute lets
///you choose between them.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_IsSmartLineSlanted = "isSmartLineSlanted";
  
    ///<summary>SmartLineColor</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Color of smart label connector lines.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_SmartLineColor = "smartLineColor";
  
    ///<summary>SmartLineThickness</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Thickness of smart label connector lines.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_SmartLineThickness = "smartLineThickness";
  
    ///<summary>SmartLineAlpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Alpha of smart label connector lines.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_SmartLineAlpha = "smartLineAlpha";
  
    ///<summary>LabelDistance</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute helps you set the distance of the
///label/value text boxes from the pie/doughnut edge. This attribute
///will work only when the attribute&lt;/span>
///  &lt;span>enableSmartLabels&lt;/span>
///  &lt;span>is set to '0'.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_LabelDistance = "labelDistance";
  
    ///<summary>SmartLabelClearance</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Clearance distance of a label (for sliced-in
///pies) from an adjacent sliced out pies.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_SmartLabelClearance = "smartLabelClearance";
  
    ///<summary>ManageLabelOverflow</summary>
    ///<remarks>
///&lt;desc>This attribute tries to
///manage overflow of data labels. If enabled, data labels are either
///wrapped or truncated with ellipses to prevent them from overflowing
///out of the chart canvas.
///&lt;p>In case 
///&lt;span>smartLabels&lt;/span> is disabled, the labels
///are wrapped to avoid the overflow. Since 
///&lt;span>smartLabels&lt;/span> is disabled, the
///wrapped labels might get overlapped.&lt;/p>When 
///&lt;span>smartLabels&lt;/span> is enabled, management
///of the overflowing labels fit in the "quadrant specific smart
///labeling algorithm". Data labels try to wrap first. In case, there
///is constrain of space in the quadrant, the labels get truncated
///with ellipses.&lt;/desc>
    ///</remarks>
    public const string Chart_ManageLabelOverflow = "manageLabelOverflow";
  
    ///<summary>UseEllipsesWhenOverflow</summary>
    ///<remarks>
///&lt;desc>When enabled, long data labels are
///truncated by adding ellipses to prevent them from overflowing the
///chart background. The default value is 1. This setting works only
///when 
///&lt;span>manageLabelOverflow&lt;/span> is set to
///1.&lt;/desc>
    ///</remarks>
    public const string Chart_UseEllipsesWhenOverflow = "useEllipsesWhenOverflow";
  
    ///<summary>FormatNumber</summary>
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
    public const string Chart_FormatNumber = "formatNumber";
  
    ///<summary>FormatNumberScale</summary>
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
    public const string Chart_FormatNumberScale = "formatNumberScale";
  
    ///<summary>DefaultNumberScale</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_DefaultNumberScale = "defaultNumberScale";
  
    ///<summary>NumberScaleUnit</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_NumberScaleUnit = "numberScaleUnit";
  
    ///<summary>NumberScaleValue</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_NumberScaleValue = "numberScaleValue";
  
    ///<summary>ScaleRecursively</summary>
    ///<remarks>
///&lt;desc>Whether to scale the number
///recursively? For more details, please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Rec_Num_Scaling.html" target="_blank">Recursive
///Number Scaling&lt;/a> page.&lt;/desc>
    ///</remarks>
    public const string Chart_ScaleRecursively = "scaleRecursively";
  
    ///<summary>MaxScaleRecursion</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_MaxScaleRecursion = "maxScaleRecursion";
  
    ///<summary>ScaleSeparator</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_ScaleSeparator = "scaleSeparator";
  
    ///<summary>NumberPrefix</summary>
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
    public const string Chart_NumberPrefix = "numberPrefix";
  
    ///<summary>NumberSuffix</summary>
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
    public const string Chart_NumberSuffix = "numberSuffix";
  
    ///<summary>DecimalSeparator</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This option helps you specify the character to
///be used as the decimal separator in a number. For more details,
///please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Basics.html#separator" target="_blank">Basics&lt;/a>
///page.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_DecimalSeparator = "decimalSeparator";
  
    ///<summary>ThousandSeparator</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This option helps you specify the character to
///be used as the thousands separator in a number. For more details,
///please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Basics.html#separator" target="_blank">Basics&lt;/a>
///page.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ThousandSeparator = "thousandSeparator";
  
    ///<summary>ThousandSeparatorPosition</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_ThousandSeparatorPosition = "thousandSeparatorPosition";
  
    ///<summary>InDecimalSeparator</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>In some countries, commas are used as decimal
///separators and dots as thousand separators. In XML, if you specify
///such values, it will give an error while converting to number. So,
///FusionCharts XT accepts the input decimal and thousand separator
///from user, so that it can convert it accordingly into the required
///format. This attribute lets you input the decimal separator. For
///more details, please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Input.html" target="_blank">Input Number
///Formatting&lt;/a> page.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_InDecimalSeparator = "inDecimalSeparator";
  
    ///<summary>InThousandSeparator</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>In some countries, commas are used as decimal
///separators and dots as thousand separators. In XML, if you specify
///such values, it will give an error while converting to number. So,
///FusionCharts XT accepts the input decimal and thousand separator
///from user, so that it can convert it accordingly into the required
///format. This attribute lets you input the thousand separator. For
///more details, please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Input.html" target="_blank">Input Number
///Formatting&lt;/a> page.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_InThousandSeparator = "inThousandSeparator";
  
    ///<summary>Decimals</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Number of decimal places to which all numbers on
///the chart will be rounded to. For more details, please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Basics.html#yaxis" target="_blank">Basics&lt;/a>
///page.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_Decimals = "decimals";
  
    ///<summary>ForceDecimals</summary>
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
    public const string Chart_ForceDecimals = "forceDecimals";
  
    ///<summary>BaseFont</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute lets you set the font face
///(family) of all the text (data labels, values etc.) on
///chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_BaseFont = "baseFont";
  
    ///<summary>BaseFontSize</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute sets the base font size of the
///chart i.e., all the values and the names in the chart which lie on
///the canvas will be displayed using the font size provided
///here.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_BaseFontSize = "baseFontSize";
  
    ///<summary>BaseFontColor</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_BaseFontColor = "baseFontColor";
  
    ///<summary>ShowLegend</summary>
    ///<remarks>
///&lt;desc>Whether to show legend on
///the chart.&lt;/desc>
    ///</remarks>
    public const string Chart_ShowLegend = "showLegend";
  
    ///<summary>LegendPosition</summary>
    ///<remarks>
///&lt;desc>The legend can be plotted at two
///positions on the chart - below the chart (BOTTOM) and on the RIGHT
///side of the chart.&lt;/desc>
    ///</remarks>
    public const string Chart_LegendPosition = "legendPosition";
  
    ///<summary>LegendCaption</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_LegendCaption = "legendCaption";
  
    ///<summary>LegendIconScale</summary>
    ///<remarks>
///&lt;desc>Scaling of legend icon is possible in
///FusionCharts XT. This attribute lets you control the size of the
///legend icon.
///The default scale value is 1. Anything less than 1 reduces
///the size of the legend-icons on the chart. Any value bigger than 1
///enlarges the icons. e.g., 0.5 means half the size, where as, 2
///means twice the size.&lt;/desc>
    ///</remarks>
    public const string Chart_LegendIconScale = "legendIconScale";
  
    ///<summary>LegendBgColor</summary>
    ///<remarks>
///&lt;desc>Background color for the legend.&lt;/desc>
    ///</remarks>
    public const string Chart_LegendBgColor = "legendBgColor";
  
    ///<summary>LegendBgAlpha</summary>
    ///<remarks>
///&lt;desc>Background alpha for the legend.&lt;/desc>
    ///</remarks>
    public const string Chart_LegendBgAlpha = "legendBgAlpha";
  
    ///<summary>LegendBorderColor</summary>
    ///<remarks>
///&lt;desc>Border Color for the legend.&lt;/desc>
    ///</remarks>
    public const string Chart_LegendBorderColor = "legendBorderColor";
  
    ///<summary>LegendBorderThickness</summary>
    ///<remarks>
///&lt;desc>Border thickness for the legend.&lt;/desc>
    ///</remarks>
    public const string Chart_LegendBorderThickness = "legendBorderThickness";
  
    ///<summary>LegendBorderAlpha</summary>
    ///<remarks>
///&lt;desc>Border alpha for the legend.&lt;/desc>
    ///</remarks>
    public const string Chart_LegendBorderAlpha = "legendBorderAlpha";
  
    ///<summary>LegendShadow</summary>
    ///<remarks>
///&lt;desc>Whether to show a shadow for
///legend.&lt;/desc>
    ///</remarks>
    public const string Chart_LegendShadow = "legendShadow";
  
    ///<summary>LegendAllowDrag</summary>
    ///<remarks>
///&lt;desc>The legend can be made drag-able by
///setting this attribute to 1. End viewers of the chart can drag the
///legend around on the chart.&lt;/desc>
    ///</remarks>
    public const string Chart_LegendAllowDrag = "legendAllowDrag";
  
    ///<summary>LegendScrollBgColor</summary>
    ///<remarks>
///&lt;desc>If you've too many items on the
///legend, a scroll bar shows up on the same. This attribute lets you
///configure the background color of the scroll bar.&lt;/desc>
    ///</remarks>
    public const string Chart_LegendScrollBgColor = "legendScrollBgColor";
  
    ///<summary>LegendScrollBarColor</summary>
    ///<remarks>
///&lt;desc>If you've too many items on the
///legend, a scroll bar shows up on the same. This attribute lets you
///configure the bar color of the scroll bar.&lt;/desc>
    ///</remarks>
    public const string Chart_LegendScrollBarColor = "legendScrollBarColor";
  
    ///<summary>LegendScrollBtnColor</summary>
    ///<remarks>
///&lt;desc>If you've too many items on the
///legend, a scroll bar shows up on the same. This attribute lets you
///configure the color of buttons of the scroll bar.&lt;/desc>
    ///</remarks>
    public const string Chart_LegendScrollBtnColor = "legendScrollBtnColor";
  
    ///<summary>ReverseLegend</summary>
    ///<remarks>
///&lt;desc>You can reverse the ordering of
///datasets in the legend by setting this attribute to 1.&lt;/desc>
    ///</remarks>
    public const string Chart_ReverseLegend = "reverseLegend";
  
    ///<summary>InteractiveLegend</summary>
    ///<remarks>
///&lt;desc>This attribute lets you interact with
///the legend in your chart. When you click a particular legend key,
///the associated slice slides out from the chart. Re-clicking the key
///causes the slice to slide in.&lt;/desc>
    ///</remarks>
    public const string Chart_InteractiveLegend = "interactiveLegend";
  
    ///<summary>LegendNumColumns</summary>
    ///<remarks>
///&lt;desc>The legend items are arranged in
///columns. Using this attribute, you can propose the number of
///columns. This value undergoes internal checking on judicious use of
///white-space. In case, the value is found improper, the chart
///auto-calculates the number of columns. When set to 0, the chart
///automatically decides the number of columns.
///The above is applicable when 
///&lt;span>legendPosition&lt;/span> is set to BOTTOM
///&lt;span>.&lt;/span> If you have set RIGHT 
///&lt;span>legendPosition&lt;/span>, the number of
///columns is always set to 1.&lt;/desc>
    ///</remarks>
    public const string Chart_LegendNumColumns = "legendNumColumns";
  
    ///<summary>MinimiseWrappingInLegend</summary>
    ///<remarks>
///&lt;desc>Whether to minimize legend item text
///wrapping.&lt;/desc>
    ///</remarks>
    public const string Chart_MinimiseWrappingInLegend = "minimiseWrappingInLegend";
  
    ///<summary>ShowToolTip</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to show tool tip on chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ShowToolTip = "showToolTip";
  
    ///<summary>ToolTipBgColor</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_ToolTipBgColor = "toolTipBgColor";
  
    ///<summary>ToolTipBorderColor</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_ToolTipBorderColor = "toolTipBorderColor";
  
    ///<summary>ToolTipSepChar</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_ToolTipSepChar = "toolTipSepChar";
  
    ///<summary>ShowToolTipShadow</summary>
    ///<remarks>
///&lt;desc>Whether to show shadow for tool-tips
///on the chart.&lt;/desc>
    ///</remarks>
    public const string Chart_ShowToolTipShadow = "showToolTipShadow";
  
    ///<summary>CaptionPadding</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute lets you control the space (in
///pixels) between the sub-caption and top of the pie. If the
///sub-caption is not defined, it controls the space between caption
///and top of the pie. If neither caption, nor sub-caption is defined,
///this padding does not come into play.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_CaptionPadding = "captionPadding";
  
    ///<summary>ChartLeftMargin</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Amount of empty space that you want to put on
///the left side of your chart. Nothing is rendered in this
///space.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ChartLeftMargin = "chartLeftMargin";
  
    ///<summary>ChartRightMargin</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Amount of empty space that you want to put on
///the right side of your chart. Nothing is rendered in this
///space.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ChartRightMargin = "chartRightMargin";
  
    ///<summary>ChartTopMargin</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Amount of empty space that you want to put on
///the top of your chart. Nothing is rendered in this space.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ChartTopMargin = "chartTopMargin";
  
    ///<summary>ChartBottomMargin</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Amount of empty space that you want to put on
///the bottom of your chart. Nothing is rendered in this space.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ChartBottomMargin = "chartBottomMargin";
  
    ///<summary>BorderColor</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you want to set border color of individual
///pie/doughnut data items, you can specify using this
///attribute.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Set_BorderColor = "borderColor";
  
    ///<summary>BorderAlpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you want to set border alpha of individual
///pie/doughnut data items, you can specify using this
///attribute.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Set_BorderAlpha = "borderAlpha";
  
    ///<summary>IsSliced</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute determines whether the pie
///appears as a part of the total chart or is sliced out as an
///individual item.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Set_IsSliced = "isSliced";
  
    ///<summary>Label</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Set_Label = "label";
  
    ///<summary>Value</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Set_Value = "value";
  
    ///<summary>DisplayValue</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Set_DisplayValue = "displayValue";
  
    ///<summary>Color</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you want to define your own colors for the
///data items on chart, use this attribute to specify color for the
///data item. This attribute accepts hex color codes without #.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Set_Color = "color";
  
    ///<summary>Link</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Set_Link = "link";
  
    ///<summary>ToolText</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Set_ToolText = "toolText";
  
    ///<summary>Dashed</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether the border of this data item should
///appear as dashed. This is particularly useful when you want to
///highlight a data (such as forecast or trend etc.).&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Set_Dashed = "dashed";
  
    ///<summary>Alpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute determines the transparency of a
///data item. The range for this attribute is 0 to 100. 0 means
///complete transparency (the data item wont be shown on the graph)
///and 100 means opaque.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Set_Alpha = "alpha";
  
    ///<summary>ShowLabel</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>You can individually opt to show/hide labels of
///individual data items using this attribute.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Set_ShowLabel = "showLabel";
  
    ///<summary>ShowValue</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>You can individually opt to show/hide values of
///individual data items using this attribute.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Set_ShowValue = "showValue";
  
    ///<summary>LabelDisplay</summary>
    ///<remarks>
///&lt;desc>Using this attribute, you
///can customize the alignment of data labels (x-axis labels). There
///are 5 options: AUTO, WRAP, STAGGER, ROTATE or NONE. By default,
///this attribute is set to AUTO mode which means that the alignment
///of the data labels is determined automatically depending on the
///size of the chart. WRAP wraps the label text if it is too long to
///fit in one line. ROTATE rotates the labels vertically. STAGGER
///divides the labels into multiple lines.&lt;/desc>
    ///</remarks>
    public const string Chart_LabelDisplay = "labelDisplay";
  
    ///<summary>RotateLabels</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute lets you set whether the data
///labels will show up as rotated labels on the chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_RotateLabels = "rotateLabels";
  
    ///<summary>SlantLabels</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you've opted to show rotated labels on chart,
///this attribute lets you set the configuration whether the labels
///will show as slanted labels or fully vertical ones.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_SlantLabels = "slantLabels";
  
    ///<summary>LabelStep</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>By default, all the labels are displayed on the
///chart. However, if you've a set of streaming data (like name of
///months or days of week), you can opt to show every n-th label for
///better clarity. This attribute just lets you do so. When a value
///greater than 1 (n) is set to this attribute, the first label from
///left and every label at the n-th position from left will be
///displayed. e.g., a chart showing data for 12 months and set
///with&lt;/span>
///  &lt;span>labelStep='3'&lt;/span>
///  &lt;span>will show labels for January, April, July and
///October. The rest of the labels will be skipped.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_LabelStep = "labelStep";
  
    ///<summary>StaggerLines</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you've opted for STAGGER mode as&lt;/span>
///  &lt;span>labelDisplay&lt;/span>
///  &lt;span>, using this attribute you can control how many
///lines to stagger the label to. By default, all labels are displayed
///in a single line.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_StaggerLines = "staggerLines";
  
    ///<summary>RotateValues</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you've opted to show data values, you can
///rotate them using this attribute.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_RotateValues = "rotateValues";
  
    ///<summary>PlaceValuesInside</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you've opted to show data values, you can
///show them inside the columns using this attribute. By default, the
///data values show outside the column.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_PlaceValuesInside = "placeValuesInside";
  
    ///<summary>ShowYAxisValues</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>FusionCharts XT y-axis is divided into vertical
///sections using div (divisional) lines. Each div line assumes a
///value based on its position. Using this attribute you can set
///whether to show those div line (y-axis) values or not. This
///attribute shows or hides the y-axis divisional lines and limits.
///The values of&lt;/span>
///  &lt;span>showLimits&lt;/span>
///  &lt;span>and&lt;/span>
///  &lt;span>showDivLineValues&lt;/span>
///  &lt;span>if specified explicitly overrides the value of
///this attribute.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ShowYAxisValues = "showYAxisValues";
  
    ///<summary>ShowLimits</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to show chart limit values. If not
///specified&lt;/span>
///  &lt;span>showYAxisValues&lt;/span>
///  &lt;span>attribute overrides this value.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ShowLimits = "showLimits";
  
    ///<summary>ShowDivLineValues</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to show div line values. If not
///specified&lt;/span>
///  &lt;span>showYAxisValues&lt;/span>
///  &lt;span>attribute overrides this value.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ShowDivLineValues = "showDivLineValues";
  
    ///<summary>YAxisValuesStep</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>By default, all div lines show their values.
///However, you can opt to display every x(th) div line value using
///this attribute.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_YAxisValuesStep = "yAxisValuesStep";
  
    ///<summary>AdjustDiv</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>FusionCharts XT automatically tries to adjust
///divisional lines and limit values based on the data provided.
///However, if you want to set your explicit lower and upper limit
///values and number of divisional lines, first set this attribute to
///false. That will disable automatic adjustment of divisional
///lines.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_AdjustDiv = "adjustDiv";
  
    ///<summary>RotateYAxisName</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you do not wish to rotate y-axis name, set
///this as 0. It specifically comes to use when you've special
///characters (UTF8) in your y-axis name that do not show up in
///rotated mode.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_RotateYAxisName = "rotateYAxisName";
  
    ///<summary>YAxisNameWidth</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you opt to not rotate y-axis name, you can
///choose a maximum width that will be applied to y-axis name.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_YAxisNameWidth = "yAxisNameWidth";
  
    ///<summary>YAxisMinValue</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_YAxisMinValue = "yAxisMinValue";
  
    ///<summary>YAxisMaxValue</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_YAxisMaxValue = "yAxisMaxValue";
  
    ///<summary>SetAdaptiveYMin</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute lets you set whether the y-axis
///lower limit will be 0 (in case of all positive values on chart) or
///should the y-axis lower limit adapt itself to a different figure
///based on values provided to the chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_SetAdaptiveYMin = "setAdaptiveYMin";
  
    ///<summary>CenterYaxisName</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute when set to '1', allows you to
///align the y-axis name with respect to the height of the chart. When
///set to '0', the y-axis name is aligned with respect to the height
///of the canvas.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_CenterYaxisName = "centerYaxisName";
  
    ///<summary>XAxisName</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_XAxisName = "xAxisName";
  
    ///<summary>YAxisName</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_YAxisName = "yAxisName";
  
    ///<summary>CanvasBgColor</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_CanvasBgColor = "canvasBgColor";
  
    ///<summary>CanvasBgAlpha</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_CanvasBgAlpha = "canvasBgAlpha";
  
    ///<summary>CanvasBgRatio</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Helps you specify canvas background ratio for
///gradients.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_CanvasBgRatio = "canvasBgRatio";
  
    ///<summary>CanvasBgAngle</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_CanvasBgAngle = "canvasBgAngle";
  
    ///<summary>CanvasBorderColor</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_CanvasBorderColor = "canvasBorderColor";
  
    ///<summary>CanvasBorderThickness</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Lets you specify canvas border thickness.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_CanvasBorderThickness = "canvasBorderThickness";
  
    ///<summary>CanvasBorderAlpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Lets you control transparency of canvas
///border.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_CanvasBorderAlpha = "canvasBorderAlpha";
  
    ///<summary>ShowVLineLabelBorder</summary>
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
    public const string Chart_ShowVLineLabelBorder = "showVLineLabelBorder";
  
    ///<summary>UseRoundEdges</summary>
    ///<remarks>
///&lt;desc>
///  &lt;p>If you wish to plot columns with round edges and
///fill them with a glass effect gradient, set this attribute to
///1.&lt;/p>
///  &lt;p>The following functionalities will not work when
///this attribute is set to 1:&lt;/p>
///  &lt;ul>
///    &lt;li>
///      &lt;span>showShadow&lt;/span>
///      &lt;span>attribute doesn't work any more. If you want to
///remove shadow from columns, you'll have to over-ride the shadow
///with a new shadow style (applied to DATAPLOT) with alpha as
///0.&lt;/span>
///    &lt;/li>
///    &lt;li>
///      &lt;span>Plot fill properties like gradient color, angle
///etc. will not work any more, as the colors for gradient are now
///calculated by the chart itself.&lt;/span>
///    &lt;/li>
///    &lt;li>
///      &lt;span>Plot border properties also do not work in this
///mode. Also, you cannot render the border as dash in this
///mode.&lt;/span>
///    &lt;/li>
///  &lt;/ul>
///&lt;/desc>
    ///</remarks>
    public const string Chart_UseRoundEdges = "useRoundEdges";
  
    ///<summary>PlotBorderDashed</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether the plot border should appear as
///dashed.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_PlotBorderDashed = "plotBorderDashed";
  
    ///<summary>PlotBorderDashLen</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If plot border is to appear as dash, this
///attribute lets you control the length of each dash.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_PlotBorderDashLen = "plotBorderDashLen";
  
    ///<summary>PlotBorderDashGap</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If plot border is to appear as dash, this
///attribute lets you control the length of each gap between two
///dash.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_PlotBorderDashGap = "plotBorderDashGap";
  
    ///<summary>PlotFillAngle</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you've opted to fill the plot (column, area
///etc.) as gradient, this attribute lets you set the fill angle for
///gradient.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_PlotFillAngle = "plotFillAngle";
  
    ///<summary>PlotFillRatio</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you've opted to fill the plot (column, area
///etc.) as gradient, this attribute lets you set the ratio for
///gradient.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_PlotFillRatio = "plotFillRatio";
  
    ///<summary>PlotGradientColor</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>You can globally add a gradient color to the
///entire plot of chart by specifying the second color as this
///attribute. For example, if you've specified individual colors for
///your columns and now you want a gradient that ends in white. You
///need to specify FFFFFF (white) as this color and the chart will now
///draw plots as gradient.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_PlotGradientColor = "plotGradientColor";
  
    ///<summary>NumDivLines</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Number of horizontal axis division lines that
///you want on the chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_NumDivLines = "numDivLines";
  
    ///<summary>DivLineColor</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_DivLineColor = "divLineColor";
  
    ///<summary>DivLineThickness</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Thickness of divisional lines.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_DivLineThickness = "divLineThickness";
  
    ///<summary>DivLineAlpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Alpha of divisional lines.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_DivLineAlpha = "divLineAlpha";
  
    ///<summary>DivLineIsDashed</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether the divisional lines should display as
///dash.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_DivLineIsDashed = "divLineIsDashed";
  
    ///<summary>DivLineDashLen</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If the divisional lines are to be displayed as
///dash, this attribute lets you control the length of each
///dash.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_DivLineDashLen = "divLineDashLen";
  
    ///<summary>DivLineDashGap</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If the divisional lines are to be displayed as
///dash, this attribute lets you control the length of each gap
///between dash.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_DivLineDashGap = "divLineDashGap";
  
    ///<summary>ZeroPlaneColor</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_ZeroPlaneColor = "zeroPlaneColor";
  
    ///<summary>ZeroPlaneThickness</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Thickness of zero plane.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ZeroPlaneThickness = "zeroPlaneThickness";
  
    ///<summary>ZeroPlaneAlpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Alpha of zero plane.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ZeroPlaneAlpha = "zeroPlaneAlpha";
  
    ///<summary>ShowZeroPlaneValue</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Allows you to show or hide the value on which
///the zero plane exists on the y-axis. By default, the value is
///displayed. This attribute is not supported in JavaScript
///charts.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ShowZeroPlaneValue = "showZeroPlaneValue";
  
    ///<summary>ShowAlternateHGridColor</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to show alternate colored horizontal
///grid bands.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ShowAlternateHGridColor = "showAlternateHGridColor";
  
    ///<summary>AlternateHGridColor</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_AlternateHGridColor = "alternateHGridColor";
  
    ///<summary>AlternateHGridAlpha</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_AlternateHGridAlpha = "alternateHGridAlpha";
  
    ///<summary>ForceYAxisValueDecimals</summary>
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
    public const string Chart_ForceYAxisValueDecimals = "forceYAxisValueDecimals";
  
    ///<summary>YAxisValueDecimals</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you've opted to not adjust div lines, you can
///specify the div line values decimal precision using this attribute.
///For more details, please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Basics.html" target="_blank">Basics&lt;/a>
///page.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_YAxisValueDecimals = "yAxisValueDecimals";
  
    ///<summary>OutCnvBaseFont</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute sets the base font family of the
///chart font which lies outside the canvas i.e., all the values and
///the names in the chart which lie outside the canvas will be
///displayed using the font name provided here.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_OutCnvBaseFont = "outCnvBaseFont";
  
    ///<summary>OutCnvBaseFontSize</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute sets the base font size of the
///chart i.e., all the values and the names in the chart which lie
///outside the canvas will be displayed using the font size provided
///here.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_OutCnvBaseFontSize = "outCnvBaseFontSize";
  
    ///<summary>OutCnvBaseFontColor</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_OutCnvBaseFontColor = "outCnvBaseFontColor";
  
    ///<summary>XAxisNamePadding</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Using this, you can set the distance between the
///top end of x-axis title and the bottom end of data labels (or
///canvas, if data labels are not to be shown).&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_XAxisNamePadding = "xAxisNamePadding";
  
    ///<summary>YAxisNamePadding</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Using this, you can set the distance between the
///right end of y-axis title and the start of y-axis values (or
///canvas, if the y-axis values are not to be shown).&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_YAxisNamePadding = "yAxisNamePadding";
  
    ///<summary>YAxisValuesPadding</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute helps you set the horizontal
///space between the canvas left edge and the y-axis values or trend
///line values (on left/right side). This is particularly useful, when
///you want more space between your canvas and y-axis values.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_YAxisValuesPadding = "yAxisValuesPadding";
  
    ///<summary>LabelPadding</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute sets the vertical space between
///the labels and canvas bottom edge. If you want more space between
///the canvas and the x-axis labels, you can use this attribute to
///control it.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_LabelPadding = "labelPadding";
  
    ///<summary>ValuePadding</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>It sets the vertical space between the end of
///columns and start of value textboxes. This basically helps you
///control the space you want between your columns/anchors and the
///value textboxes.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ValuePadding = "valuePadding";
  
    ///<summary>PlotSpacePercent</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>On a column chart, there is spacing defined
///between two columns. By default, the spacing is set to 20% of
///canvas width. If you intend to increase or decrease the spacing
///between columns, you can do so using this attribute. For example,
///if you wanted all columns to stick to each other without any space
///in between, you can set&lt;/span>
///  &lt;span>plotSpacePercent&lt;/span>
///  &lt;span>to 0. Similarly, if you want very thin columns,
///you can set&lt;/span>
///  &lt;span>plotSpacePercent&lt;/span>
///  &lt;span>to its max value of 80.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_PlotSpacePercent = "plotSpacePercent";
  
    ///<summary>CanvasLeftMargin</summary>
    ///<remarks>
///&lt;desc>This attribute lets you control the
///space between the start of chart canvas and the start (x) of chart.
///In case of 2D charts, the canvas is the visible rectangular box. In
///case of 3D charts, the canvas box is the imaginary box around the
///3D canvas.
///&lt;p>Using this attribute, you can control the amount of empty space
///between the chart left side and canvas left side. By default,
///FusionCharts XT automatically calculates this space depending on
///the elements to be placed on the chart. However, if you want to
///over-ride this value with a 
///&lt;strong>higher&lt;/strong> value, you can use this attribute to
///specify the same. Please note that you cannot specify a margin
///lower than what has been calculated by the chart.&lt;/p>This attribute
///is particularly useful, when you've multiple charts placed in a
///page and want all their canvas start position to align with each
///other - so in such a case, you can set same margin value (a value
///large enough that it doesn't get rejected by chart owing to it
///being lower than the calculated value) for all such charts in the
///page.&lt;/desc>
    ///</remarks>
    public const string Chart_CanvasLeftMargin = "canvasLeftMargin";
  
    ///<summary>CanvasRightMargin</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Like 
///&lt;span>canvasLeftMargin&lt;/span>, this attribute
///lets you set the space between end of canvas and end of
///chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_CanvasRightMargin = "canvasRightMargin";
  
    ///<summary>CanvasTopMargin</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Like 
///&lt;span>canvasLeftMargin&lt;/span>, this attribute
///lets you set the space between top of canvas and top of
///chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_CanvasTopMargin = "canvasTopMargin";
  
    ///<summary>CanvasBottomMargin</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Like 
///&lt;span>canvasLeftMargin&lt;/span>, this attribute
///lets you set the space between bottom of canvas and bottom of
///chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_CanvasBottomMargin = "canvasBottomMargin";
  
    ///<summary>Color</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute defines the color of vertical
///separator line.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string VLine_Color = "color";
  
    ///<summary>Thickness</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Thickness in pixels of the vertical separator
///line.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string VLine_Thickness = "thickness";
  
    ///<summary>Alpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Alpha of the vertical separator line.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string VLine_Alpha = "alpha";
  
    ///<summary>Dashed</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether the vertical separator line should
///appear as dashed.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string VLine_Dashed = "dashed";
  
    ///<summary>DashLen</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If the vertical separator line is to appear as
///dashed, this attribute defines the length of dash.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string VLine_DashLen = "dashLen";
  
    ///<summary>DashGap</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If the vertical separator line is to appear as
///dashed, this attribute defines the length of dash gap.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string VLine_DashGap = "dashGap";
  
    ///<summary>Label</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string VLine_Label = "label";
  
    ///<summary>ShowLabelBorder</summary>
    ///<remarks>
///&lt;desc>Whether to show a border around the
///vLine label.&lt;/desc>
    ///</remarks>
    public const string VLine_ShowLabelBorder = "showLabelBorder";
  
    ///<summary>LinePosition</summary>
    ///<remarks>
///&lt;desc>Helps configure the position of
///vertical line i.e., if a vLine is to be plotted between 2 points
///Jan and Feb, user can set any position between 0 and 1 to indicate
///the relative position of vLine between these two points (0 means
///Jan and 1 means Feb). By default, it’s 0.5 to show in between the
///points.&lt;/desc>
    ///</remarks>
    public const string VLine_LinePosition = "linePosition";
  
    ///<summary>LabelPosition</summary>
    ///<remarks>
///&lt;desc>Helps configure the position of the
///vLine label by setting a relative position between 0 and 1. In
///vertical charts, 0 means top of canvas and 1 means bottom. In
///horizontal charts, 0 means left of canvas and 1 means right.&lt;/desc>
    ///</remarks>
    public const string VLine_LabelPosition = "labelPosition";
  
    ///<summary>LabelHAlign</summary>
    ///<remarks>
///&lt;desc>Horizontal anchor point for the
///alignment of vLine label.&lt;/desc>
    ///</remarks>
    public const string VLine_LabelHAlign = "labelHAlign";
  
    ///<summary>LabelVAlign</summary>
    ///<remarks>
///&lt;desc>Vertical anchor point for the
///alignment of vLine label.&lt;/desc>
    ///</remarks>
    public const string VLine_LabelVAlign = "labelVAlign";
  
    ///<summary>StartValue</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>The starting value for the trendline. Say, if
///you want to plot a slanted trendline from value 102 to 109,
///the&lt;/span>
///  &lt;span>startValue&lt;/span>
///  &lt;span>will be 102.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Line_StartValue = "startValue";
  
    ///<summary>EndValue</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>The ending y-axis value for the trendline. Say,
///if you want to plot a slanted trendline from value 102 to 109,
///the&lt;/span>
///  &lt;span>endValue&lt;/span>
///  &lt;span>will be 109. If you do not specify a value
///for&lt;/span>
///  &lt;span>endValue&lt;/span>
///  &lt;span>, it will automatically assume the same value
///as&lt;/span>
///  &lt;span>startValue&lt;/span>
///  &lt;span>.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Line_EndValue = "endValue";
  
    ///<summary>DisplayValue</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Line_DisplayValue = "displayValue";
  
    ///<summary>Color</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Color of the trend line and its associated
///text.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Line_Color = "color";
  
    ///<summary>IsTrendZone</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether the trend will display a line, or a zone
///(filled colored rectangle).&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Line_IsTrendZone = "isTrendZone";
  
    ///<summary>ShowOnTop</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether the trend line/zone will be displayed
///over data plots or under them.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Line_ShowOnTop = "showOnTop";
  
    ///<summary>Thickness</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you've opted to show the trend as a line,
///this attribute lets you define the thickness of trend line.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Line_Thickness = "thickness";
  
    ///<summary>Alpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Alpha of the trend line.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Line_Alpha = "alpha";
  
    ///<summary>Dashed</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you've opted to show the trend as a line,
///this attribute lets you define whether the trend line will appear
///as dashed.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Line_Dashed = "dashed";
  
    ///<summary>DashLen</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you've opted to show trend line as dash, this
///attribute lets you control the length of each dash.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Line_DashLen = "dashLen";
  
    ///<summary>DashGap</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you've opted to show trend line as dash, this
///attribute lets you control the length of each dash gap.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Line_DashGap = "dashGap";
  
    ///<summary>ValueOnRight</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to show the trend line value on left
///side or right side of chart. This is particularly useful when the
///trend line display values on the chart are colliding with
///divisional lines values on the chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Line_ValueOnRight = "valueOnRight";
  
    ///<summary>ToolText</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Line_ToolText = "toolText";
  
    ///<summary>PieInnerFaceAlpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Alpha of the pie inner face&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_PieInnerFaceAlpha = "pieInnerFaceAlpha";
  
    ///<summary>PieOuterFaceAlpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Alpha of the pie outer face&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_PieOuterFaceAlpha = "pieOuterFaceAlpha";
  
    ///<summary>PieYScale</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute alters the y-perspective of the
///pie in percentage figures. 100 percent means the full pie face is
///visible and 0 percent means only the side face is visible.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_PieYScale = "pieYScale";
  
    ///<summary>PieSliceDepth</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute controls the pie 3D Depth.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_PieSliceDepth = "pieSliceDepth";
  
    ///<summary>MaxColWidth</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Maximum allowed column width&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_MaxColWidth = "maxColWidth";
  
    ///<summary>CanvasBaseColor</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_CanvasBaseColor = "canvasBaseColor";
  
    ///<summary>ShowCanvasBg</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_ShowCanvasBg = "showCanvasBg";
  
    ///<summary>ShowCanvasBase</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_ShowCanvasBase = "showCanvasBase";
  
    ///<summary>CanvasBaseDepth</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Height of canvas base (in pixels)&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_CanvasBaseDepth = "canvasBaseDepth";
  
    ///<summary>CanvasBgDepth</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Depth of Canvas Background&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_CanvasBgDepth = "canvasBgDepth";
  
    ///<summary>OverlapColumns</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_OverlapColumns = "overlapColumns";
  
    ///<summary>ZeroPlaneShowBorder</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_ZeroPlaneShowBorder = "zeroPlaneShowBorder";
  
    ///<summary>ZeroPlaneBorderColor</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_ZeroPlaneBorderColor = "zeroPlaneBorderColor";
  
    ///<summary>MaxLabelWidthPercent</summary>
    ///<remarks>
///&lt;desc>Restricts the maximum
///length of data labels in terms of percentage of the charts width
///that the data labels can occupy. If a data label is longer than the
///specified percentage width then it will either be wrapped or get
///truncated, subject to availability of vertical space. Unnecessary
///space is not reserved for the data labels, in case all of them are
///shorter than the specified maximum width.&lt;/desc>
    ///</remarks>
    public const string Chart_MaxLabelWidthPercent = "maxLabelWidthPercent";
  
    ///<summary>RotateXAxisName</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you do not wish to rotate x-axis name, set
///this as 0. It specifically comes to use when you've special
///characters (UTF8) in your x-axis name that do not show up in
///rotated mode.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_RotateXAxisName = "rotateXAxisName";
  
    ///<summary>XAxisNameWidth</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you opt to not rotate x-axis name, you can
///choose a maximum width that will be applied to x-axis name.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_XAxisNameWidth = "xAxisNameWidth";
  
    ///<summary>CenterXaxisName</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute when set to '1', allows you to
///align the x-axis name with respect to the height of the chart. When
///set to '0', the x-axis name is aligned with respect to the height
///of the canvas.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_CenterXaxisName = "centerXaxisName";
  
    ///<summary>ShowAlternateVGridColor</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to show alternate colored vertical grid
///bands.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ShowAlternateVGridColor = "showAlternateVGridColor";
  
    ///<summary>AlternateVGridColor</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_AlternateVGridColor = "alternateVGridColor";
  
    ///<summary>AlternateVGridAlpha</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_AlternateVGridAlpha = "alternateVGridAlpha";
  
    ///<summary>ConnectNullData</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute lets you control whether empty
///data sets in your data will be connected to each other OR do they
///appear as broken data sets. Please see 
///&lt;strong>Advanced charting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/DiscData.html" target="_blank">Plotting Discontinuous data&lt;/a>
///section for more details on this.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ConnectNullData = "connectNullData";
  
    ///<summary>ValuePosition</summary>
    ///<remarks>
///&lt;desc>If you've opted to show
///data values on the chart, this attribute lets you adjust the
///vertical alignment of data values with respect to dataplots. By
///default, this attribute is set to AUTO mode in which the alignment
///of each data value is determined automatically based on the
///position of each plot point. In ABOVE mode, data values are
///displayed above the plot points unless a plot point is too close to
///the upper edge of the canvas while in BELOW mode, data values are
///displayed below the plot points unless a plot point is too close to
///the lower edge of the canvas.&lt;/desc>
    ///</remarks>
    public const string Chart_ValuePosition = "valuePosition";
  
    ///<summary>LineColor</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Color using which the lines on the chart will be
///drawn.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_LineColor = "lineColor";
  
    ///<summary>LineThickness</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Thickness of the lines on the chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_LineThickness = "lineThickness";
  
    ///<summary>LineAlpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Alpha of the lines on the chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_LineAlpha = "lineAlpha";
  
    ///<summary>LineDashed</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Configuration whether to show the lines on the
///chart as dash.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_LineDashed = "lineDashed";
  
    ///<summary>LineDashLen</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If the lines are to be shown as dashes, this
///attribute defines the length of dash.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_LineDashLen = "lineDashLen";
  
    ///<summary>LineDashGap</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If the lines are to be shown as dashes, this
///attribute defines the length of dash gap.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_LineDashGap = "lineDashGap";
  
    ///<summary>DrawAnchors</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to draw anchors on the chart. If the
///anchors are not shown, then the tool tip and links won't
///work.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_DrawAnchors = "drawAnchors";
  
    ///<summary>AnchorSides</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute sets the number of sides the
///anchor will have. For e.g., an anchor with 3 sides will represent a
///triangle, with 4 it will be a square and so on.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_AnchorSides = "anchorSides";
  
    ///<summary>AnchorRadius</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute sets the radius (in pixels) of
///the anchor. Greater the radius, bigger will be the anchor
///size.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_AnchorRadius = "anchorRadius";
  
    ///<summary>AnchorBorderColor</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Lets you set the border color of anchors.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_AnchorBorderColor = "anchorBorderColor";
  
    ///<summary>AnchorBorderThickness</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Helps you set border thickness of
///anchors.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_AnchorBorderThickness = "anchorBorderThickness";
  
    ///<summary>AnchorBgColor</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Helps you set the background color of
///anchors.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_AnchorBgColor = "anchorBgColor";
  
    ///<summary>AnchorAlpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Helps you set the alpha of entire anchors. If
///you need to hide the anchors on chart but still enable tool tips,
///set this as 0.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_AnchorAlpha = "anchorAlpha";
  
    ///<summary>AnchorBgAlpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Helps you set the alpha of anchor
///background.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_AnchorBgAlpha = "anchorBgAlpha";
  
    ///<summary>NumVDivLines</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_NumVDivLines = "numVDivLines";
  
    ///<summary>VDivLineColor</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_VDivLineColor = "vDivLineColor";
  
    ///<summary>VDivLineThickness</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Thickness of vertical axis division
///lines.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_VDivLineThickness = "vDivLineThickness";
  
    ///<summary>VDivLineAlpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Alpha of vertical axis division lines.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_VDivLineAlpha = "vDivLineAlpha";
  
    ///<summary>VDivLineIsDashed</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether vertical divisional lines appear as
///dashed.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_VDivLineIsDashed = "vDivLineIsDashed";
  
    ///<summary>VDivLineDashLen</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If vertical div lines are dashed, this attribute
///lets you control the width of dash.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_VDivLineDashLen = "vDivLineDashLen";
  
    ///<summary>VDivLineDashGap</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If vertical div lines are dashed, this attribute
///lets you control the width of dash gap.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_VDivLineDashGap = "vDivLineDashGap";
  
    ///<summary>ShowZeroPlane</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to show the zero plane on chart (if
///negative values are present).&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ShowZeroPlane = "showZeroPlane";
  
    ///<summary>CanvasPadding</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Lets you set the space between the canvas border
///and first &amp;amp; last data points&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_CanvasPadding = "canvasPadding";
  
    ///<summary>ValuePosition</summary>
    ///<remarks>
///&lt;desc>If you've opted to show a
///particular data value on the chart, this attribute lets you adjust
///the vertical alignment of individual data values with respect to
///dataplots. By default, this attribute is set to AUTO mode in which
///the alignment of a data value is determined automatically based on
///the position of its plot point. In ABOVE mode, a data value is
///displayed above the plot point unless a plot point is too close to
///the upper edge of the canvas while in BELOW mode, a data value is
///displayed below the plot point unless a plot point is too close to
///the lower edge of the canvas.&lt;/desc>
    ///</remarks>
    public const string Set_ValuePosition = "valuePosition";
  
    ///<summary>AnchorSides</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you want to configure data item specific
///anchor properties, this attribute lets you define the number of
///sides for the anchor of that particular data item.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Set_AnchorSides = "anchorSides";
  
    ///<summary>AnchorRadius</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you want to configure data item specific
///anchor properties, this attribute lets you define the radius for
///the anchor of that particular data item.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Set_AnchorRadius = "anchorRadius";
  
    ///<summary>AnchorBorderColor</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you want to configure data item specific
///anchor properties, this attribute lets you set the border color for
///the anchor of that particular data item.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Set_AnchorBorderColor = "anchorBorderColor";
  
    ///<summary>AnchorBorderThickness</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you want to configure data item specific
///anchor properties, this attribute lets you set the border thickness
///for the anchor of that particular data item.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Set_AnchorBorderThickness = "anchorBorderThickness";
  
    ///<summary>AnchorBgColor</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you want to configure data item specific
///anchor properties, this attribute lets you set the background color
///for the anchor of that particular data item.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Set_AnchorBgColor = "anchorBgColor";
  
    ///<summary>AnchorAlpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you want to configure data item specific
///anchor properties, this attribute lets you set the alpha for the
///anchor of that particular data item.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Set_AnchorAlpha = "anchorAlpha";
  
    ///<summary>AnchorBgAlpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you want to configure data item specific
///anchor properties, this attribute lets you set the background alpha
///for the anchor of that particular data item.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Set_AnchorBgAlpha = "anchorBgAlpha";
  
    ///<summary>DoughnutRadius</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute lets you explicitly set the inner
///radius of the chart. FusionCharts XT automatically calculates the
///best fit radius for the chart. This attribute is useful if you want
///to enforce one of your own values.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_DoughnutRadius = "doughnutRadius";
  
    ///<summary>PlotFillColor</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_PlotFillColor = "plotFillColor";
  
    ///<summary>ShowSecondaryLimits</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to show secondary axis chart limit
///values.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ShowSecondaryLimits = "showSecondaryLimits";
  
    ///<summary>ShowDivLineSecondaryValue</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to show div line values for the
///secondary y-axis.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ShowDivLineSecondaryValue = "showDivLineSecondaryValue";
  
    ///<summary>PYAxisMaxValue</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_PYAxisMaxValue = "PYAxisMaxValue";
  
    ///<summary>PYAxisMinValue</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_PYAxisMinValue = "PYAxisMinValue";
  
    ///<summary>PYAxisNameWidth</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you opt to not rotate y-axis name, you can
///choose a maximum width that will be applied to primary y-axis
///name.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_PYAxisNameWidth = "PYAxisNameWidth";
  
    ///<summary>SYAxisNameWidth</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you opt to not rotate y-axis name, you can
///choose a maximum width that will be applied to secondary y-axis
///name.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_SYAxisNameWidth = "SYAxisNameWidth";
  
    ///<summary>ShowCumulativeLine</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to add the overlay line series over
///column plot. The values of line series is automatically calculated
///by chart as cumulative % values. It is plotted against the
///secondary y-axis.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ShowCumulativeLine = "showCumulativeLine";
  
    ///<summary>ShowLineValues</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to show values for the line
///series.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ShowLineValues = "showLineValues";
  
    ///<summary>PrimaryAxisOnLeft</summary>
    ///<remarks>
///&lt;desc>Setting this attribute to 0 helps in
///reversing the position of primary axis.&lt;/desc>
    ///</remarks>
    public const string Chart_PrimaryAxisOnLeft = "primaryAxisOnLeft";
  
    ///<summary>Use3DLineShift</summary>
    ///<remarks>
///&lt;desc>By default, when a line dataset is
///rendered over a column 3D dataset, the line dataset shifts in the
///z-dimension to give a 3D perspective. But, if you do not wish to
///add the perspective to the line dataset, you may set this attribute
///as 0.&lt;/desc>
    ///</remarks>
    public const string Chart_Use3DLineShift = "use3DLineShift";
  
    ///<summary>SYAXisName</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_SYAXisName = "SYAXisName";
  
    ///<summary>PYAxisName</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_PYAxisName = "PYAxisName";
  
    ///<summary>Sdecimals</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Number of decimal places for the secondary axis
///to which all numbers on the cumulative line will be rounded to. For
///more details, please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Basics.html#rounding" target="_blank">Basics&lt;/a>
///page.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_Sdecimals = "sdecimals";
  
    ///<summary>SforceDecimals</summary>
    ///<remarks>
///&lt;desc>Whether to add 0 padding
///at the end of decimal numbers for the secondary axis. For example,
///If you limit the maximum number of decimal digits to 2, a number
///like 55.345 will be rounded to 55.34. This does not mean that all
///numbers will be displayed with a fixed number of decimal places.
///For instance 55 will not be displayed as 55.00 and 55.1 will not
///become 55.10. In order to have fixed number of decimal places
///attached to all the numbers, set this attribute to 1. For more
///details, please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Basics.html#rounding" target="_blank">Basics&lt;/a>
///page.&lt;/desc>
    ///</remarks>
    public const string Chart_SforceDecimals = "sforceDecimals";
  
    ///<summary>SyAxisValueDecimals</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you've opted to not adjust div lines, you can
///specify the div line values decimal precision using this attribute
///for the secondary axis. For more details, please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Basics.html#yaxis" target="_blank">Basics&lt;/a>
///page.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_SyAxisValueDecimals = "syAxisValueDecimals";
  
    ///<summary>ParentYAxis</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to plot against the primary axis or
///secondary axis. In case of Pareto chart, since secondary axis is
///0-100%, you'll have to specify a value within this range.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Line_ParentYAxis = "parentYAxis";
  
    ///<summary>LegendMarkerCircle</summary>
    ///<remarks>
///&lt;desc>Whether to use
///square legend keys or circular ones.&lt;/desc>
    ///</remarks>
    public const string Chart_LegendMarkerCircle = "legendMarkerCircle";
  
    ///<summary>SeriesNameInToolTip</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>For multi-series charts, FusionCharts XT shows
///the following information in tool tip (unless tool text is
///explicitly defined): "Series Name, Category Name, Data Value". This
///attribute lets you control whether series name will show up in tool
///tip or not.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_SeriesNameInToolTip = "seriesNameInToolTip";
  
    ///<summary>LegendPadding</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Padding of legend from right/bottom side of
///canvas&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_LegendPadding = "legendPadding";
  
    ///<summary>Font</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Lets you specify font face for the x-axis data
///labels.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Categories_Font = "font";
  
    ///<summary>FontSize</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Categories_FontSize = "fontSize";
  
    ///<summary>FontColor</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Lets you specify font color for the x-axis data
///labels.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Categories_FontColor = "fontColor";
  
    ///<summary>Label</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Category_Label = "label";
  
    ///<summary>ShowLabel</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>You can individually opt to show/hide labels of
///individual data items using this attribute.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Category_ShowLabel = "showLabel";
  
    ///<summary>ToolText</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Category_ToolText = "toolText";
  
    ///<summary>SeriesName</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Dataset_SeriesName = "seriesName";
  
    ///<summary>Color</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute sets the color using which
///columns, lines, area of that dataset will be drawn. For column
///chart, you can specify a list of comma separated hex codes to get a
///gradient plot.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Dataset_Color = "color";
  
    ///<summary>Alpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute sets the alpha (transparency) of
///the entire dataset.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Dataset_Alpha = "alpha";
  
    ///<summary>Ratio</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you've opted to show columns as gradients,
///this attribute lets you control the ratio of each color. See 
///&lt;strong>Advanced charting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/Gradients.html" target="_blank">Using Gradients&lt;/a> page for
///more details.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Dataset_Ratio = "ratio";
  
    ///<summary>ShowValues</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to show the values for this
///dataset.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Dataset_ShowValues = "showValues";
  
    ///<summary>Dashed</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether this dataset will appear as
///dashed.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Dataset_Dashed = "dashed";
  
    ///<summary>IncludeInLegend</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to include the seriesName of this
///dataset in legend.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Dataset_IncludeInLegend = "includeInLegend";
  
    ///<summary>ValuePosition</summary>
    ///<remarks>
///&lt;desc>This attribute lets you
///adjust the vertical alignment of data values for all dataplots in
///the dataset. The alignment is set with respect to the position of
///the dataplots on the chart. By default, the attribute is set to
///AUTO mode in which the alignment of each data value is determined
///automatically based on the position of each plot point. In ABOVE
///mode, data values are displayed above the plot points unless a plot
///point is too close to the upper edge of the canvas. While in BELOW
///mode, data values are displayed below the plot points unless a plot
///point is too close to the lower edge of the canvas. The attribute
///will function only if showValue attribute is set to 1 in this
///particular dataset.&lt;/desc>
    ///</remarks>
    public const string Dataset_ValuePosition = "valuePosition";
  
    ///<summary>DrawAnchors</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to draw anchors for the particular
///dataset. If the anchors are not shown, then the tool tip and links
///won't work for the dataset.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Dataset_DrawAnchors = "drawAnchors";
  
    ///<summary>AnchorSides</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Sets the number of sides that the anchors of the
///particular dataset will have. For e.g., an anchor with 3 sides will
///represent a triangle, with 4 it will be a square and so on.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Dataset_AnchorSides = "anchorSides";
  
    ///<summary>AnchorRadius</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute sets the radius (in pixels) of
///the anchors of the particular dataset. Greater the radius, bigger
///will be the anchor size.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Dataset_AnchorRadius = "anchorRadius";
  
    ///<summary>AnchorBorderColor</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Lets you set the border color of anchors of the
///particular dataset.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Dataset_AnchorBorderColor = "anchorBorderColor";
  
    ///<summary>AnchorBorderThickness</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Helps you set border thickness of anchors of the
///particular dataset.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Dataset_AnchorBorderThickness = "anchorBorderThickness";
  
    ///<summary>AnchorBgColor</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Helps you set the background color of anchors of
///the particular dataset.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Dataset_AnchorBgColor = "anchorBgColor";
  
    ///<summary>AnchorAlpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Helps you set the alpha of entire anchors of the
///particular dataset. If you need to hide the anchors for the dataset
///but still enable tool tips, set this as 0.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Dataset_AnchorAlpha = "anchorAlpha";
  
    ///<summary>AnchorBgAlpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Helps you set the alpha of anchor background of
///the particular dataset.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Dataset_AnchorBgAlpha = "anchorBgAlpha";
  
    ///<summary>LineThickness</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Thickness of the lines for the particular
///dataset.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Dataset_LineThickness = "lineThickness";
  
    ///<summary>LineDashLen</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If the lines are to be shown as dash for the
///particular dataset, this attribute defines the length of
///dash.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Dataset_LineDashLen = "lineDashLen";
  
    ///<summary>LineDashGap</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If the lines are to be shown as dash for the
///particular dataset, this attribute defines the length of dash
///gap.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Dataset_LineDashGap = "lineDashGap";
  
    ///<summary>ShowPlotBorder</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to show the border of this
///dataset.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Dataset_ShowPlotBorder = "showPlotBorder";
  
    ///<summary>PlotBorderColor</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Dataset_PlotBorderColor = "plotBorderColor";
  
    ///<summary>PlotBorderThickness</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Thickness for data plot border of this
///dataset&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Dataset_PlotBorderThickness = "plotBorderThickness";
  
    ///<summary>PlotBorderAlpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Alpha for data plot border of this
///dataset&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Dataset_PlotBorderAlpha = "plotBorderAlpha";
  
    ///<summary>CompactDataMode</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Indicates whether the XML is in compact form or
///not.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_CompactDataMode = "compactDataMode";
  
    ///<summary>DataSeparator</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Specifies the separation character used
///in compact XML.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_DataSeparator = "dataSeparator";
  
    ///<summary>Axis</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Converts linear axis into logarithmic axis and
///vice versa. The chart axis is linear by default.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_Axis = "axis";
  
    ///<summary>LogBase</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Specifies the base for the logarithmic
///scale.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_LogBase = "logBase";
  
    ///<summary>NumMinorLogDivLines</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Specifies the number of minor divisional lines
///to display between two major log divisions. If not specified the
///chart will automatically display certain number of minor divisional
///lines.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_NumMinorLogDivLines = "numMinorLogDivLines";
  
    ///<summary>DynamicAxis</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If enabled the Y-axis scale will automatically
///adapt to the data when the chart is zoomed. This feature is not
///applicable in case of logarithmic scale.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_DynamicAxis = "dynamicAxis";
  
    ///<summary>DivIntervalHints</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Specifies a list of intervals for the divisional
///lines, out of which the chart automatically selects the most
///appropriate interval. This feature is not applicable in case of
///logarithmic scale.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_DivIntervalHints = "divIntervalHints";
  
    ///<summary>AllowPinMode</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Used for enabling/disabling the pin mode
///feature, which is active by default.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_AllowPinMode = "allowPinMode";
  
    ///<summary>NumVisibleLabels</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Specifies the number of data labels that are to
///be visible in one screen.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_NumVisibleLabels = "numVisibleLabels";
  
    ///<summary>AnchorMinRenderDistance</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Sets the minimum distance in pixels at which to
///show anchors. This helps in making the chart clutter free.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_AnchorMinRenderDistance = "anchorMinRenderDistance";
  
    ///<summary>ShowVDivLines</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Enables/disables the rendering of vertical
///divisional line for every data label.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ShowVDivLines = "showVDivLines";
  
    ///<summary>DisplayStartIndex</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Indicates the index of the data label that will
///appear to the extreme left of the chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_DisplayStartIndex = "displayStartIndex";
  
    ///<summary>DisplayEndIndex</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Indicates the index of the data label
///that is to appear at the extreme right of the chart.
///Using&lt;/span>
///  &lt;span>displayStartIndex&lt;/span>
///  &lt;span>and&lt;/span>
///  &lt;span>displayEndIndex&lt;/span>
///  &lt;span>attributes you can set the range of data labels
///that'll be visible when the chart first renders.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_DisplayEndIndex = "displayEndIndex";
  
    ///<summary>DrawToolbarButtons</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Shows/hides the toolbar buttons, these are shown
///by default.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_DrawToolbarButtons = "drawToolbarButtons";
  
    ///<summary>PixelsPerPoint</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Specifies the quantity of pixels to be used for
///producing a data point. A greater number will result in high
///quality display.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_PixelsPerPoint = "pixelsPerPoint";
  
    ///<summary>PaletteThemeColor</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Specifies a color theme that will be applied
///throughout the chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_PaletteThemeColor = "paletteThemeColor";
  
    ///<summary>ToolbarButtonColor</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Sets the color of the toolbar buttons.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ToolbarButtonColor = "toolbarButtonColor";
  
    ///<summary>ToolbarButtonFontColor</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Sets the color of the toolbar font.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ToolbarButtonFontColor = "toolbarButtonFontColor";
  
    ///<summary>ZoomPaneBorderColor</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Sets the color of the zoom pane border.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ZoomPaneBorderColor = "zoomPaneBorderColor";
  
    ///<summary>ZoomPaneBgColor</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Sets the background color of the zoom
///pane.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ZoomPaneBgColor = "zoomPaneBgColor";
  
    ///<summary>ZoomPaneBgAlpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Sets the alpha of the zoom pane.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ZoomPaneBgAlpha = "zoomPaneBgAlpha";
  
    ///<summary>PinLineThicknessDelta</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Sets the thickness of the pinned line when the
///chart is put to pin line mode.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_PinLineThicknessDelta = "pinLineThicknessDelta";
  
    ///<summary>PinPaneBorderColor</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Sets the color of the pin pane border.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_PinPaneBorderColor = "pinPaneBorderColor";
  
    ///<summary>PinPaneBgColor</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Sets the background color of the pin
///pane.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_PinPaneBgColor = "pinPaneBgColor";
  
    ///<summary>PinPaneBgAlpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Sets the alpha of the pin pane.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_PinPaneBgAlpha = "pinPaneBgAlpha";
  
    ///<summary>ToolTipBarColor</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Sets the color of the tooltip bar, which is
///displayed alongside the tooltips.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ToolTipBarColor = "toolTipBarColor";
  
    ///<summary>MouseCursorColor</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Sets the color of the mouse cursor.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_MouseCursorColor = "mouseCursorColor";
  
    ///<summary>BtnResetChartTitle</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Replaces the title of the 'Reset Chart' button
///with provided string.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_BtnResetChartTitle = "btnResetChartTitle";
  
    ///<summary>BtnZoomOutTitle</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Replaces the title of the 'Zoom Out' button with
///the provided string.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_BtnZoomOutTitle = "btnZoomOutTitle";
  
    ///<summary>BtnSwitchtoZoomModeTitle</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Replaces the title of the 'Switch to Zoom Mode'
///button with provided string.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_BtnSwitchtoZoomModeTitle = "btnSwitchtoZoomModeTitle";
  
    ///<summary>BtnSwitchToPinModeTitle</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Replaces the title of the "Switch to Pin Mode'
///button with provided string.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_BtnSwitchToPinModeTitle = "btnSwitchToPinModeTitle";
  
    ///<summary>ShowToolBarButtonTooltext</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Enables/disables display of tooltips for toolbar
///buttons.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ShowToolBarButtonTooltext = "showToolBarButtonTooltext";
  
    ///<summary>BtnResetChartTooltext</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Replaces the default tooltext of the 'Reset
///Chart' button with provided string.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_BtnResetChartTooltext = "btnResetChartTooltext";
  
    ///<summary>BtnSwitchToPinModeTooltext</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Replaces the default tooltext of 'Switch to Pin
///Mode' button with provided string.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_BtnSwitchToPinModeTooltext = "btnSwitchToPinModeTooltext";
  
    ///<summary>ZoomOutMenuItemLabel</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Replaces the 'Zoom Out Chart' menu label with
///provided string.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ZoomOutMenuItemLabel = "zoomOutMenuItemLabel";
  
    ///<summary>ResetChartMenuItemLabel</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Replaces the 'Reset Chart' menu item with
///provided string.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ResetChartMenuItemLabel = "resetChartMenuItemLabel";
  
    ///<summary>ZoomModeMenuItemLabel</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Replaces the 'Zoom Mode' menu item with provided
///string.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ZoomModeMenuItemLabel = "zoomModeMenuItemLabel";
  
    ///<summary>PinModeMenuItemLabel</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Replaces the 'Pin Mode' menu item with provided
///string.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_PinModeMenuItemLabel = "pinModeMenuItemLabel";
  
    ///<summary>ToolBarBtnTextVMargin</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Sets the Vertical margin of toolbar buttons
///(i.e., padding between text and button border).&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ToolBarBtnTextVMargin = "toolBarBtnTextVMargin";
  
    ///<summary>ToolBarBtnTextHMargin</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Sets the Horizontal margin of toolbar buttons
///(i.e., padding between text and button border).&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ToolBarBtnTextHMargin = "toolBarBtnTextHMargin";
  
    ///<summary>ToolBarBtnHPadding</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Sets horizontal spacing between toolbar
///buttons.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ToolBarBtnHPadding = "toolBarBtnHPadding";
  
    ///<summary>ToolBarBtnVPadding</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Sets the vertical spacing between toolbar
///buttons.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ToolBarBtnVPadding = "toolBarBtnVPadding";
  
    ///<summary>ScrollColor</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Color for scroll bar.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ScrollColor = "scrollColor";
  
    ///<summary>ScrollHeight</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Required height for scroll bar.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ScrollHeight = "scrollHeight";
  
    ///<summary>ScrollPadding</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Distance between the end of canvas (bottom part)
///and start of scroll bar.&lt;/span>
///  &lt;p>This feature is not available in
///JavaScript charts.&lt;/p>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ScrollPadding = "scrollPadding";
  
    ///<summary>ScrollBtnWidth</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Width of both scroll bar buttons (left &amp;amp;
///right).&lt;/span>
///  &lt;p>This feature is not available in
///JavaScript charts.&lt;/p>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ScrollBtnWidth = "scrollBtnWidth";
  
    ///<summary>ScrollBtnPadding</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Padding between the scroll buttons and the
///scroll track (background).&lt;/span>
///  &lt;p>This feature is not available in
///JavaScript charts.&lt;/p>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ScrollBtnPadding = "scrollBtnPadding";
  
    ///<summary>StartIndex</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>The starting index for the vertical trendline.
///Index represents the relative position of a data-point from the
///first data-point. The index of the first data-point is 1. A
///vertical trendzone is created when both&lt;/span>
///  &lt;span>startIndex&lt;/span>
///  &lt;span>  and&lt;/span>
///  &lt;span>endIndex&lt;/span>
///  &lt;span>attributes are specified. In case, only&lt;/span>
///  &lt;span>startIndex&lt;/span>
///  &lt;span>is specified, a vertical trendline is
///drawn.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Set_StartIndex = "startIndex";
  
    ///<summary>EndIndex</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>The end index for the vertical trendline. Index
///represents the relative position of a data-point from the first
///data-point. The index of the first data-point is 1. A vertical
///trendzone is created when both 
///&lt;span>startIndex&lt;/span>  and 
///&lt;span>endIndex&lt;/span> attributes are specified.
///In case, only 
///&lt;span>startIndex&lt;/span> is specified, a vertical
///trendline is drawn.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Set_EndIndex = "endIndex";
  
    ///<summary>DisplayAlways</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>It allows you to show or hide the vertical
///trendline/zone in macroscopic view. By default, it is set to 1
///which makes the vertical trendline/zone visible in both microscopic
///and macroscopic views. If you set it to '0,' it will hide the
///vertical trendline in macroscopic view.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Set_DisplayAlways = "displayAlways";
  
    ///<summary>DisplayWhenCount</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>The 
///&lt;span>displayWhenCount&lt;/span> attribute sets the
///condition for visibility of the vertical trendline. For example, if
///the value of this attribute is set to 
///&lt;span>10&lt;/span>, the vertical trendline will be
///visible only if 10, or less than 10 dataplots are displayed on a
///single screen. The value of the 
///&lt;span>displayWhenCount&lt;/span> can be any number,
///which is greater than 1 and less than the total number of dataplots
///that are plotted on the chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Set_DisplayWhenCount = "displayWhenCount";
  
    ///<summary>ShowOnTop</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether the vertical trendline will appear on
///top of the dataplots. By default, it is set to 0.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Set_ShowOnTop = "showOnTop";
  
    ///<summary>Thickness</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you've opted to show the trend as a line,
///this attribute lets you define the thickness of the line.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Set_Thickness = "thickness";
  
    ///<summary>ValueOnTop</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Set_ValueOnTop = "valueOnTop";
  
    ///<summary>MaxBarHeight</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Maximum allowed bar height.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_MaxBarHeight = "maxBarHeight";
  
    ///<summary>BarDepth</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>By default, FusionCharts XT automatically allots
///a 3D depth to each bar, based on the available space. However, if
///you want to specify a custom depth (in pixels) for any bar, you can
///use this attribute.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_BarDepth = "barDepth";
  
    ///<summary>OverlapBars</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_OverlapBars = "overlapBars";
  
    ///<summary>ShowSum</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you want to show sum of all the columns in a
///given stacked column, set this attribute to 1.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ShowSum = "showSum";
  
    ///<summary>Stack100Percent</summary>
    ///<remarks>
///&lt;desc>Setting this attribute to
///1 helps in depicting the values in percentage figures.&lt;/desc>
    ///</remarks>
    public const string Chart_Stack100Percent = "stack100Percent";
  
    ///<summary>UsePercentDistribution</summary>
    ///<remarks>
///&lt;desc>If you do not wish to
///show data values as percentage figures, set this attribute to
///0.&lt;/desc>
    ///</remarks>
    public const string Chart_UsePercentDistribution = "usePercentDistribution";
  
    ///<summary>ShowXAxisPercentValues</summary>
    ///<remarks>
///&lt;desc>If you do not wish to
///show percentage values along the x-axis, set this attribute to
///0.&lt;/desc>
    ///</remarks>
    public const string Chart_ShowXAxisPercentValues = "showXAxisPercentValues";
  
    ///<summary>WidthPercent</summary>
    ///<remarks>
///&lt;desc>FusionCharts XT
///automatically calculates the horizontal width of the columns based
///on the data provided by you. But, using this attribute you can
///specify different width values to each segment. The values in each
///category must be specified in a way that sum of all the figures
///equals to 100 percent in order to render your chart
///successfully.&lt;/desc>
    ///</remarks>
    public const string Category_WidthPercent = "widthPercent";
  
    ///<summary>Animate3D</summary>
    ///<remarks>
///&lt;desc>This attribute enables you to set
///whether the initial animation of the 3D elements in chart canvas
///will occur or not. If set to 0, it will also set off chart canvas'
///rotation while using JavaScript API functions like 
///&lt;span>view2D(), resetView(),
///rotateView().&lt;/span> The view will be updated instantly without any
///rotation or animation.&lt;/desc>
    ///</remarks>
    public const string Chart_Animate3D = "animate3D";
  
    ///<summary>ExeTime</summary>
    ///<remarks>
///&lt;desc>It sets the time in seconds (can
///accept decimals) that the 3D elements of the chart take to animate
///when the chart is rendered initially. This attribute also sets the
///time that the chart canvas will take to animate when the chart is
///transformed to any view (View3D, View 2D, Reset View) using context
///menu (right click menu). This is also applicable while using
///JavaScript API functions - 
///&lt;span>view2D(), view3D, resetView() or
///rotateView()&lt;/span>. Default value is 0.5.&lt;/desc>
    ///</remarks>
    public const string Chart_ExeTime = "exeTime";
  
    ///<summary>XAxisTickColor</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_XAxisTickColor = "xAxisTickColor";
  
    ///<summary>XAxisTickAlpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute allows you to set the
///transparency of the x-axis tick lines.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_XAxisTickAlpha = "xAxisTickAlpha";
  
    ///<summary>XAxisTickThickness</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_XAxisTickThickness = "xAxisTickThickness";
  
    ///<summary>Is2D</summary>
    ///<remarks>
///&lt;desc>Using this you can decide whether
///initially to draw a 2D view of the chart or not.&lt;/desc>
    ///</remarks>
    public const string Chart_Is2D = "is2D";
  
    ///<summary>Clustered</summary>
    ///<remarks>
///&lt;desc>It lets you to set if multi column
///datasets will be rendered in a clustered format or manhattan
///format. By default, this is set to 0 i.e., manhattan format. For
///more details on Clustered and Manhattan arrangement of columns,
///please refer to 
///&lt;a href="http://docs.fusioncharts.com/charts/contents/JavaScript/API/SpecialCharts/Combi3D_JSAPI.html" target="_blank">&lt;strong>3D Chart Attributes&lt;/strong>&lt;/a> page in 
///&lt;strong>Quick Chart Configuration&lt;/strong> section.&lt;/desc>
    ///</remarks>
    public const string Chart_Clustered = "clustered";
  
    ///<summary>ChartOrder</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_ChartOrder = "chartOrder";
  
    ///<summary>ChartOnTop</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>It allows the user to decide whether or not the
///chart canvas will be placed over the extra chart elements (caption,
///subcaption, legend). This feature is visible when the chart canvas
///is zoomed/scaled. The default value is 1.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ChartOnTop = "chartOnTop";
  
    ///<summary>AutoScaling</summary>
    ///<remarks>
///&lt;desc>It lets you decide whether or not the
///chart will be allowed to best fit automatically after a user
///interaction, for example, rotation.&lt;/desc>
    ///</remarks>
    public const string Chart_AutoScaling = "autoScaling";
  
    ///<summary>AllowScaling</summary>
    ///<remarks>
///&lt;desc>It enables you to set whether to
///allow zoom-in/zoom-out using the mouse scroller. Note that scaling
///or use of mouse scroller is compatible only with Windows Operating
///System.&lt;/desc>
    ///</remarks>
    public const string Chart_AllowScaling = "allowScaling";
  
    ///<summary>StartAngX</summary>
    ///<remarks>
///&lt;desc>This attribute lets you specify the
///view angle (for view around the chart vertically) from which the
///rotation starts (when the chart is initialized). The rotation stops
///at 
///&lt;span>endAngX&lt;/span>. If not specified, the
///default values for both the attributes are 30. It works only if 
///&lt;span>animat3D&lt;/span> is 
///&lt;span>1.&lt;/span> For more details on view angles,
///please refer to 
///&lt;a href="http://docs.fusioncharts.com/charts/contents/JavaScript/API/SpecialCharts/Combi3D_JSAPI.html" target="_blank">&lt;strong>3D Chart Attributes&lt;/strong>&lt;/a> page in 
///&lt;strong>Quick Chart Configuration&lt;/strong> section.&lt;/desc>
    ///</remarks>
    public const string Chart_StartAngX = "startAngX";
  
    ///<summary>StartAngY</summary>
    ///<remarks>
///&lt;desc>This attribute lets you specify the
///view angle (for view around the chart horizontally) from which the
///rotation starts (when the chart is initialized). The rotation stops
///at 
///&lt;span>endAngY&lt;/span>. If not specified, the
///default values for both the attributes are -45. It works only if 
///&lt;span>animat3D&lt;/span> is 
///&lt;span>1.For more details on view angles, please refer
///to 
///&lt;a href="http://docs.fusioncharts.com/charts/contents/JavaScript/API/SpecialCharts/Combi3D_JSAPI.html" target="_blank">&lt;strong>3D Chart Attributes&lt;/strong>&lt;/a> page in 
///&lt;strong>Quick Chart Configuration&lt;/strong> section.&lt;/span>&lt;/desc>
    ///</remarks>
    public const string Chart_StartAngY = "startAngY";
  
    ///<summary>EndAngX</summary>
    ///<remarks>
///&lt;desc>This attribute lets you
///specify the view angle (for view around the chart vertically) at
///which the rotation ends (when the chart is initialized). The
///rotation starts at 
///&lt;span>startAngX&lt;/span>. If not specified, the
///default values for both the attributes are 30. It works only if 
///&lt;span>animat3D&lt;/span> is 
///&lt;span>1.&lt;/span>For more details on view angles,
///please refer to 
///&lt;a href="http://docs.fusioncharts.com/charts/contents/JavaScript/API/SpecialCharts/Combi3D_JSAPI.html" target="_blank">&lt;strong>3D Chart Attributes&lt;/strong>&lt;/a> page in 
///&lt;strong>Quick Chart Configuration&lt;/strong> section.&lt;/desc>
    ///</remarks>
    public const string Chart_EndAngX = "endAngX";
  
    ///<summary>EndAngY</summary>
    ///<remarks>
///&lt;desc>This attribute lets you
///specify the view angle (for view around the chart horizontally) at
///which the rotation ends (when the chart is initialized). The
///rotation starts at 
///&lt;span>startAngY&lt;/span>. If not specified, the
///default values for both the attributes are -45. It works only if 
///&lt;span>animat3D&lt;/span> is 
///&lt;span>1.&lt;/span> For more details on view angles,
///please refer to 
///&lt;a href="http://docs.fusioncharts.com/charts/contents/JavaScript/API/SpecialCharts/Combi3D_JSAPI.html" target="_blank">&lt;strong>3D Chart Attributes&lt;/strong>&lt;/a> page in 
///&lt;strong>Quick Chart Configuration&lt;/strong> section.&lt;/desc>
    ///</remarks>
    public const string Chart_EndAngY = "endAngY";
  
    ///<summary>CameraAngX</summary>
    ///<remarks>
///&lt;desc>When 
///&lt;span>animat3D&lt;/span> is 
///&lt;span>0&lt;/span>, this attribute lets you specify
///the camera angle (for view around the chart vertically) from which
///the chart is viewed initially. For more details on view angles,
///please refer to 
///&lt;a href="http://docs.fusioncharts.com/charts/contents/JavaScript/API/SpecialCharts/Combi3D_JSAPI.html" target="_blank">&lt;strong>3D Chart Attributes&lt;/strong>&lt;/a> page in 
///&lt;strong>Quick Chart Configuration&lt;/strong> section.&lt;/desc>
    ///</remarks>
    public const string Chart_CameraAngX = "cameraAngX";
  
    ///<summary>CameraAngY</summary>
    ///<remarks>
///&lt;desc>When 
///&lt;span>animat3D&lt;/span> is 
///&lt;span>0&lt;/span>, this attribute lets you specify
///the camera angle (for view around the chart horizontally) from
///which the chart is viewed initially. For more details on view
///angles, please refer to 
///&lt;a href="http://docs.fusioncharts.com/charts/contents/JavaScript/API/SpecialCharts/Combi3D_JSAPI.html" target="_blank">&lt;strong>3D Chart Attributes&lt;/strong>&lt;/a> page in 
///&lt;strong>Quick Chart Configuration&lt;/strong> section.&lt;/desc>
    ///</remarks>
    public const string Chart_CameraAngY = "cameraAngY";
  
    ///<summary>LightAngX</summary>
    ///<remarks>
///&lt;desc>Using this you can
///specify light x source's x angle. For more details on light angles,
///please refer to 
///&lt;a href="http://docs.fusioncharts.com/charts/contents/JavaScript/API/SpecialCharts/Combi3D_JSAPI.html" target="_blank">&lt;strong>3D Chart Attributes&lt;/strong>&lt;/a> page in 
///&lt;strong>Quick Chart Configuration&lt;/strong> section.&lt;/desc>
    ///</remarks>
    public const string Chart_LightAngX = "lightAngX";
  
    ///<summary>LightAngY</summary>
    ///<remarks>
///&lt;desc>Using this you can
///specify light x source's y angle. For more details on light angles,
///please refer to 
///&lt;a href="http://docs.fusioncharts.com/charts/contents/JavaScript/API/SpecialCharts/Combi3D_JSAPI.html" target="_blank">&lt;strong>3D Chart Attributes&lt;/strong>&lt;/a> page in 
///&lt;strong>Quick Chart Configuration&lt;/strong> section.&lt;/desc>
    ///</remarks>
    public const string Chart_LightAngY = "lightAngY";
  
    ///<summary>Intensity</summary>
    ///<remarks>
///&lt;desc>It lets you to control
///the intensity of light that falls on the 3D chart canvas. 0 denotes
///minimum light while 10 is the maximum. Default value is 2.5.&lt;/desc>
    ///</remarks>
    public const string Chart_Intensity = "intensity";
  
    ///<summary>DynamicShading</summary>
    ///<remarks>
///&lt;desc>Whether to provide a
///light source, which is fixed outside the chart world, to create
///dynamic shades on the chart's face at the time of rotation. If
///dynamicShading is set to 0, a static light will be fixed with the
///chart world. For more details on this and lighting systems, please
///refer to 
///&lt;a href="http://docs.fusioncharts.com/charts/contents/JavaScript/API/SpecialCharts/Combi3D_JSAPI.html" target="_blank">&lt;strong>3D Chart Attributes&lt;/strong>&lt;/a> page in 
///&lt;strong>Quick Chart Configuration&lt;/strong> section.&lt;/desc>
    ///</remarks>
    public const string Chart_DynamicShading = "dynamicShading";
  
    ///<summary>Bright2D</summary>
    ///<remarks>
///&lt;desc>If you've set 
///&lt;span>dynamicShading&lt;/span> to 1, this attribute
///lets you decide whether a brighter view of the chart is rendered in
///2D mode. This if set to 1, 
///&lt;span>lightAngX&lt;/span> and 
///&lt;span>lightAngY&lt;/span> (if defined) are not used
///by the chart. Rather, these light source angles are fixed at 0
///position.&lt;/desc>
    ///</remarks>
    public const string Chart_Bright2D = "bright2D";
  
    ///<summary>AllowRotation</summary>
    ///<remarks>
///&lt;desc>This attribute allows you decide
///whether to stop any user interactive rotation of the chart or not.
///If it is set to 0, the interactive rotation will be barred.
///JavaScript API 
///&lt;span>rotateView()&lt;/span> is not influenced by
///this.&lt;/desc>
    ///</remarks>
    public const string Chart_AllowRotation = "allowRotation";
  
    ///<summary>ConstrainVerticalRotation</summary>
    ///<remarks>
///&lt;desc>This attribute, if set to
///1 will constrain the user from using full 360 degrees vertical
///rotation of 3D chart canvas. If this is set to 1, the user can
///rotate up to 90 degrees(top or bottom) from 0 degree position. User
///can also specify these limits using 
///&lt;span>minVerticalRotAngle&lt;/span> and 
///&lt;span>minVerticalRotAngle&lt;/span> attributes.
///This is only applicable to user's mouse interactivity with the
///chart. JavaScript API 
///&lt;span>rotateView()&lt;/span> is not influenced by
///this.&lt;/desc>
    ///</remarks>
    public const string Chart_ConstrainVerticalRotation = "constrainVerticalRotation";
  
    ///<summary>MinVerticalRotAngle</summary>
    ///<remarks>
///&lt;desc>If you've set 
///&lt;span>constraintVerticalRotation&lt;/span> to 1,
///using this attribute you can set the minimum allowed angle up to
///which a user can rotate the chart vertically. This is only
///applicable to user's mouse interactivity with the chart. JavaScript
///API 
///&lt;span>rotateView()&lt;/span> is not influenced by
///this.&lt;/desc>
    ///</remarks>
    public const string Chart_MinVerticalRotAngle = "minVerticalRotAngle";
  
    ///<summary>MaxVerticalRotAngle</summary>
    ///<remarks>
///&lt;desc>If you've set 
///&lt;span>constraintVerticalRotation&lt;/span> to 1,
///using this attribute you can set the maximum allowed angle up to
///which a user can rotate the chart vertically. This is only
///applicable to user's mouse interactivity with the chart. JavaScript
///API 
///&lt;span>rotateView()&lt;/span> is not influenced by
///this.&lt;/desc>
    ///</remarks>
    public const string Chart_MaxVerticalRotAngle = "maxVerticalRotAngle";
  
    ///<summary>ConstrainHorizontalRotation</summary>
    ///<remarks>
///&lt;desc>This attribute, if set to
///1 will constrain the user from using full 360 degrees vertical
///rotation of 3D chart canvas. If this is set to 1, the user can
///rotate up to 90 degrees(top or bottom) from 0 degree position. User
///can also specify these limits using 
///&lt;span>minHorizontalRotAngle&lt;/span> and 
///&lt;span>minHorizontalRotAngle&lt;/span> attributes.
///This is only applicable to user's mouse interactivity with the
///chart. JavaScript API 
///&lt;span>rotateView()&lt;/span> is not influenced by
///this.&lt;/desc>
    ///</remarks>
    public const string Chart_ConstrainHorizontalRotation = "constrainHorizontalRotation";
  
    ///<summary>MinHorizontalRotAngle</summary>
    ///<remarks>
///&lt;desc>If you've set 
///&lt;span>constraintHorizontalRotation&lt;/span> to 1,
///using this attribute you can set the minimum allowed angle up to
///which a user can rotate the chart horizontally. This is only
///applicable to user's mouse interactivity with the chart. JavaScript
///API 
///&lt;span>rotateView()&lt;/span> is not influenced by
///this. This is only applicable to user's mouse interactivity with
///the chart. JavaScript API 
///&lt;span>rotateView()&lt;/span> is not influenced by
///this.&lt;/desc>
    ///</remarks>
    public const string Chart_MinHorizontalRotAngle = "minHorizontalRotAngle";
  
    ///<summary>MaxHorizontalRotAngle</summary>
    ///<remarks>
///&lt;desc>If you've set 
///&lt;span>constraintHorizontalRotation&lt;/span> to 1,
///using this attribute you can set the maximum allowed angle up to
///which a user can rotate the chart horizontally. This is only
///applicable to user's mouse interactivity with the chart. JavaScript
///API 
///&lt;span>rotateView()&lt;/span> is not influenced by
///this.&lt;/desc>
    ///</remarks>
    public const string Chart_MaxHorizontalRotAngle = "maxHorizontalRotAngle";
  
    ///<summary>ZDepth</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_ZDepth = "zDepth";
  
    ///<summary>ZGapPlot</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_ZGapPlot = "zGapPlot";
  
    ///<summary>YzWallDepth</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_YzWallDepth = "yzWallDepth";
  
    ///<summary>ZxWallDepth</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_ZxWallDepth = "zxWallDepth";
  
    ///<summary>XyWallDepth</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_XyWallDepth = "xyWallDepth";
  
    ///<summary>DivLineEffect</summary>
    ///<remarks>
///&lt;desc>This attribute enables you to apply
///an effect, for example, EMBOSS, or BEVEL to the divisional lines as
///well as to the trendlines.&lt;/desc>
    ///</remarks>
    public const string Chart_DivLineEffect = "divLineEffect";
  
    ///<summary>ZeroPlaneMesh</summary>
    ///<remarks>
///&lt;desc>Whether to draw the zero plane as a
///wireframe mesh or as a filled plane. If set to 1, a mesh on the
///zero plane of the chart will be drawn.&lt;/desc>
    ///</remarks>
    public const string Chart_ZeroPlaneMesh = "zeroPlaneMesh";
  
    ///<summary>YLabelGap</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute helps you set the horizontal
///space between the canvas wall edge and the y-axis values or trend
///line values (on left/right side). This is particularly useful, when
///you want more space between your canvas and y-axis values.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_YLabelGap = "yLabelGap";
  
    ///<summary>XLabelGap</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute sets the vertical space between
///the labels and canvas wall edge. If you want more space between the
///canvas and the x-axis labels, you can use this attribute to control
///it.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_XLabelGap = "xLabelGap";
  
    ///<summary>RenderAs</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute defines what the particular
///dataset will be plotted as. Valid values are COLUMN, AREA or
///LINE.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Dataset_RenderAs = "renderAs";
  
    ///<summary>AreaOverColumns</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you've both area and column plots on the
///combination chart, this attribute lets you configure whether area
///chart will appear over column chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_AreaOverColumns = "areaOverColumns";
  
    ///<summary>ParentYAxis</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>renderAs attribute over-rides this attribute in
///FusionCharts XT for Single Y Combination Charts. This attribute
///allows you to set the parent axis of the dataset - P (primary) or S
///(secondary). Primary datasets are drawn as Columns and secondary
///datasets as lines.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Dataset_ParentYAxis = "parentYAxis";
  
    ///<summary></summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_ = "";
  
    ///<summary>SYAxisMinValue</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_SYAxisMinValue = "SYAxisMinValue";
  
    ///<summary>SYAxisMaxValue</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_SYAxisMaxValue = "SYAxisMaxValue";
  
    ///<summary>SetAdaptiveSYMin</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute lets you set whether the
///secondary y-axis lower limit will be 0 (in case of all positive
///values on chart) or should the y-axis lower limit adapt itself to a
///different figure based on values provided to the chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_SetAdaptiveSYMin = "setAdaptiveSYMin";
  
    ///<summary>SyncAxisLimits</summary>
    ///<remarks>
///&lt;desc>Setting this attribute to 1 lets you
///synchronize the limits of both the primary and secondary axes.&lt;/desc>
    ///</remarks>
    public const string Chart_SyncAxisLimits = "syncAxisLimits";
  
    ///<summary>ShowPZeroPlaneValue</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Allows you to show or hide the value on which
///the zero plane exists on the primary y-axis. By default, the value
///is displayed. This attribute is not supported in JavaScript
///charts.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ShowPZeroPlaneValue = "showPZeroPlaneValue";
  
    ///<summary>ShowSZeroPlaneValue</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Allows you to show or hide the value on which
///the zero plane exists on the secondary y-axis. By default, the
///value is displayed. This attribute is not supported in JavaScript
///charts.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ShowSZeroPlaneValue = "showSZeroPlaneValue";
  
    ///<summary>SScaleRecursively</summary>
    ///<remarks>
///&lt;desc>To scale the numbers recursively only
///for the secondary y-axis you need to set this attribute to '1'. For
///more details, please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Rec_Num_Scaling.html" target="_blank">Recursive
///number scaling&lt;/a> page.&lt;/desc>
    ///</remarks>
    public const string Chart_SScaleRecursively = "sScaleRecursively";
  
    ///<summary>SMaxScaleRecursion</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_SMaxScaleRecursion = "sMaxScaleRecursion";
  
    ///<summary>SScaleSeparator</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_SScaleSeparator = "sScaleSeparator";
  
    ///<summary>ForceSYAxisValueDecimals</summary>
    ///<remarks>
///&lt;desc>Whether to forcefully
///attach decimal places to all secondary y-axis values. For example,
///If you limit the maximum number of decimal digits to 2, a number
///like 55.345 will be rounded to 55.34. This does not mean that all
///secondary y-axis numbers will be displayed with a fixed number of
///decimal places. For instance 55 will not be displayed as 55.00 and
///55.1 will not become 55.10. In order to have fixed number of
///decimal places attached to all secondary y-axis numbers, set this
///attribute to 1. For more details, please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Basics.html#yaxis" target="_blank">Basics&lt;/a>
///page.&lt;/desc>
    ///</remarks>
    public const string Chart_ForceSYAxisValueDecimals = "forceSYAxisValueDecimals";
  
    ///<summary>SFormatNumber</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This configuration determines whether the
///numbers belonging to secondary axis will be formatted using commas,
///e.g., 40,000 if 
///&lt;span>sFormatNumber='1'&lt;/span> and 40000 if 
///&lt;span>sFormatNumber= '0'&lt;/span>. For more
///details, please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Basics.html" target="_blank">Basics&lt;/a>
///page.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_SFormatNumber = "sFormatNumber";
  
    ///<summary>SFormatNumberScale</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Configuration whether to add K (thousands) and M
///(millions) to a number belonging to secondary axis after truncating
///and rounding it - e.g., if 
///&lt;span>sFormatNumberScale&lt;/span> is set to 1,
///10434 will become 1.04K (with decimalPrecision set to 2 places).
///Same with numbers in millions - an M will be added at the end. For
///more details, please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Scaling.html" target="_blank">Number
///Scaling&lt;/a> page.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_SFormatNumberScale = "sFormatNumberScale";
  
    ///<summary>SDefaultNumberScale</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_SDefaultNumberScale = "sDefaultNumberScale";
  
    ///<summary>SNumberScaleUnit</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_SNumberScaleUnit = "sNumberScaleUnit";
  
    ///<summary>SNumberScaleValue</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_SNumberScaleValue = "sNumberScaleValue";
  
    ///<summary>SNumberPrefix</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Using this attribute, you could add prefix to
///all the numbers belonging to secondary axis. For more details,
///please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Basics.html#prefix" target="_blank">Basics&lt;/a>
///page.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_SNumberPrefix = "sNumberPrefix";
  
    ///<summary>SNumberSuffix</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Using this attribute, you could add prefix to
///all the numbers belonging to secondary axis. For more details,
///please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Basics.html#prefix" target="_blank">Basics&lt;/a>
///page.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_SNumberSuffix = "sNumberSuffix";
  
    ///<summary>SDecimals</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Number of decimal places to which all numbers
///belonging to secondary axis will be rounded to. For more details,
///please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Basics.html#rounding" target="_blank">Basics&lt;/a>
///page.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_SDecimals = "sDecimals";
  
    ///<summary>SYAxisValueDecimals</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you've opted to not adjust div lines, you can
///specify the secondary div line values decimal precision using this
///attribute. For more details, please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Basics.html#yaxis" target="_blank">Basics&lt;/a>
///page.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_SYAxisValueDecimals = "sYAxisValueDecimals";
  
    ///<summary>IncludeInLegend</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to include the seriesName of this
///line-set in legend.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Lineset_IncludeInLegend = "includeInLegend";
  
    ///<summary>SeriesName</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Lineset_SeriesName = "seriesName";
  
    ///<summary>Color</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute sets the color using which the
///lines of that line-set will be drawn.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Lineset_Color = "color";
  
    ///<summary>Alpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute sets the alpha (transparency) of
///the entire line-set.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Lineset_Alpha = "alpha";
  
    ///<summary>ShowValues</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to show the values for this
///line-set.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Lineset_ShowValues = "showValues";
  
    ///<summary>ValuePosition</summary>
    ///<remarks>
///&lt;desc>This attribute lets you
///adjust the vertical alignment of data values for all lines in the
///line-set.&lt;/desc>
    ///</remarks>
    public const string Lineset_ValuePosition = "valuePosition";
  
    ///<summary>Dashed</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether this line-set will appear as
///dashed&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Lineset_Dashed = "dashed";
  
    ///<summary>LineDashLen</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Lets you set the length of the dash&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Lineset_LineDashLen = "lineDashLen";
  
    ///<summary>LineDashGap</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Lets you set the distance between two
///dashes&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Lineset_LineDashGap = "lineDashGap";
  
    ///<summary>LineThickness</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Sets the thickness of the lines in a
///line-set&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Lineset_LineThickness = "lineThickness";
  
    ///<summary>DrawAnchors</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to show anchors on the line&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Lineset_DrawAnchors = "drawAnchors";
  
    ///<summary>AnchorSides</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Sets the number of sides of the anchors&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Lineset_AnchorSides = "anchorSides";
  
    ///<summary>AnchorRadius</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Sets the radius of the anchors&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Lineset_AnchorRadius = "anchorRadius";
  
    ///<summary>AnchorBorderColor</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Sets the color of the border of the
///anchors&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Lineset_AnchorBorderColor = "anchorBorderColor";
  
    ///<summary>AnchorBorderThickness</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Sets the thickness of the anchor border&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Lineset_AnchorBorderThickness = "anchorBorderThickness";
  
    ///<summary>AnchorBgColor</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Specifies the background of the anchors&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Lineset_AnchorBgColor = "anchorBgColor";
  
    ///<summary>AnchorBgAlpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Sets the transparency of the anchor
///background&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Lineset_AnchorBgAlpha = "anchorBgAlpha";
  
    ///<summary>AnchorAlpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Sets the transparency of all the anchors within
///a&lt;/span>
///  &lt;span>&amp;lt;lineset&amp;gt;&lt;/span>
///  &lt;span>element&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Lineset_AnchorAlpha = "anchorAlpha";
  
    ///<summary>ClipBubbles</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If bigger bubbles are placed on the edges of the
///chart, they might extend beyond the canvas. However, you can opt to
///clip those bubble edges so that they are contained within the
///canvas by setting this attribute to 1.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ClipBubbles = "clipBubbles";
  
    ///<summary>NegativeColor</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>You can opt to color all bubbles with a negative
///Z value on the chart with a single color, which can be specified
///for this attribute.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_NegativeColor = "negativeColor";
  
    ///<summary>XAxisLabelMode</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute is used to render the x-axis
///labels in three different modes. The modes are as following:&lt;/span>
///  &lt;ul>
///    &lt;li>AUTO - Allows the x-axis labels to inherit the properties of
///y-axis (numeric calculations), ignoring the properties of the 
///&lt;span>&amp;lt;categories&amp;gt;&lt;/span> element if
///present in the data.&lt;/li>
///    &lt;li>CATEGORIES - This mode is the default mode of 
///&lt;span>xAxisLabelMode&lt;/span>. It allows the chart
///to show the properties of the 
///&lt;span>&amp;lt;categories&amp;gt;&lt;/span> element on the
///x-axis labels.&lt;/li>
///    &lt;li>MIXED - Allows the chart to show both the properties of y-axis
///(numeric calculations) and the 
///&lt;span>&amp;lt;categories&amp;gt;&lt;/span> element on the
///x-axis simultaneously.&lt;/li>
///  &lt;/ul>For more details on these modes click 
///&lt;a href="http://docs.fusioncharts.com/charts/contents/AttDesc/BubbleScatter.html" target="_blank">here&lt;/a>.
///&lt;/desc>
    ///</remarks>
    public const string Chart_XAxisLabelMode = "xAxisLabelMode";
  
    ///<summary>ShowXAxisValues</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>FusionCharts XT x-axis is divided into
///horizontal sections using div (divisional) lines. Each div line
///assumes a value based on its position. Using this attribute you can
///set whether to show those div line (x-axis) values or not. This
///attribute shows or hides the x-axis divisional lines and limits.
///When specified the values of&lt;/span>
///  &lt;span>showVLimits&lt;/span>
///  &lt;span>and&lt;/span>
///  &lt;span>showVDivLineValues&lt;/span>
///  &lt;span>overrides the value of this
///attribute.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ShowXAxisValues = "showXAxisValues";
  
    ///<summary>ShowVLimits</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to show chart limit values of the
///x-axis. If not specified&lt;/span>
///  &lt;span>showXAxisValues&lt;/span>
///  &lt;span>attribute overrides this value.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ShowVLimits = "showVLimits";
  
    ///<summary>ShowVDivLineValues</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to show vertical div line values. If not
///specified&lt;/span>
///  &lt;span>showXAxisValues&lt;/span>
///  &lt;span>attribute over-rides this value.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ShowVDivLineValues = "showVDivLineValues";
  
    ///<summary>XAxisMinValue</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>The bubble/scatter chart have both x and y axis
///as numeric. This attribute lets you explicitly set the x-axis lower
///limit. If you do not specify this value, FusionCharts XT will
///automatically calculate the best value for you.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_XAxisMinValue = "xAxisMinValue";
  
    ///<summary>XAxisMaxValue</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>The bubble/scatter chart have both x and y axis
///as numeric. This attribute lets you explicitly set the x-axis upper
///limit. If you do not specify this value, FusionCharts XT will
///automatically calculate the best value for you.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_XAxisMaxValue = "xAxisMaxValue";
  
    ///<summary>BubbleScale</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you want to proportionately increase/decrease
///size of all the bubbles on the chart, you can use this attribute. A
///value of less than 1 (but greater than 0) will decrease all the
///bubble sizes and vice versa.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_BubbleScale = "bubbleScale";
  
    ///<summary>XAxisValuesStep</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>By default, all div lines show their values.
///However, you can opt to display every x(th) div line value using
///this attribute.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_XAxisValuesStep = "xAxisValuesStep";
  
    ///<summary>AdjustVDiv</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>FusionCharts XT automatically tries to adjust
///divisional lines and limit values of the x-axis based on the data
///provided. However, if you want to set your explicit lower and upper
///limit values and number of divisional lines for the x-axis, first
///set this attribute to false. That will disable automatic adjustment
///of divisional lines.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_AdjustVDiv = "adjustVDiv";
  
    ///<summary>SetAdaptiveXMin</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute lets you set whether the x-axis
///lower limit will be 0 (in case of all positive values on chart) or
///should the x-axis lower limit adapt itself to a different figure
///based on values provided to the chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_SetAdaptiveXMin = "setAdaptiveXMin";
  
    ///<summary>ShowRegressionLine</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute when set to '1' draws 
///&lt;a href="http://docs.fusioncharts.com/charts/contents/AttDesc/BubbleScatter.html#regression" target="_blank">regression
///lines&lt;/a> for all the datasets in the chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ShowRegressionLine = "showRegressionLine";
  
    ///<summary>ShowYOnX</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>When it is set to '1' the 
///&lt;a href="http://docs.fusioncharts.com/charts/contents/AttDesc/BubbleScatter.html#regression" target="_blank">regression
///lines&lt;/a> are drawn using 
///&lt;strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/AttDesc/BubbleScatter.html#regressionmodes" target="_blank">Y On X&lt;/a>&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/AttDesc/BubbleScatter.html#regressionmodes" target="_blank">mode&lt;/a>
///and when the attribute is set to '0' the regression lines are drawn
///using 
///&lt;strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/AttDesc/BubbleScatter.html#regressionmodes" target="_blank">X On Y&lt;/a>&lt;/strong> mode. By default, this attribute is set to '1'.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ShowYOnX = "showYOnX";
  
    ///<summary>RegressionLineColor</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_RegressionLineColor = "RegressionLineColor";
  
    ///<summary>RegressionLineThickness</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_RegressionLineThickness = "RegressionLineThickness";
  
    ///<summary>RegressionLineAlpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute allows you to set the
///transparency of all the regression lines in a chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_RegressionLineAlpha = "RegressionLineAlpha";
  
    ///<summary>DrawQuadrant</summary>
    ///<remarks>
///&lt;desc>Whether to draw quadrants
///on the chart.&lt;/desc>
    ///</remarks>
    public const string Chart_DrawQuadrant = "drawQuadrant";
  
    ///<summary>QuadrantXVal</summary>
    ///<remarks>
///&lt;desc>This attribute lets you
///set the position of the vertical quadrant line.&lt;/desc>
    ///</remarks>
    public const string Chart_QuadrantXVal = "quadrantXVal";
  
    ///<summary>QuadrantYVal</summary>
    ///<remarks>
///&lt;desc>This attribute lets you
///set the position of the horizontal quadrant line.&lt;/desc>
    ///</remarks>
    public const string Chart_QuadrantYVal = "quadrantYVal";
  
    ///<summary>QuadrantLineColor</summary>
    ///<remarks>
///&lt;desc>Lets you set the color of
///the quadrant lines.&lt;/desc>
    ///</remarks>
    public const string Chart_QuadrantLineColor = "quadrantLineColor";
  
    ///<summary>QuadrantLineThickness</summary>
    ///<remarks>
///&lt;desc>Lets you set the
///thickness of the quadrant lines.&lt;/desc>
    ///</remarks>
    public const string Chart_QuadrantLineThickness = "quadrantLineThickness";
  
    ///<summary>QuadrantLineAlpha</summary>
    ///<remarks>
///&lt;desc>Lets you control the
///transparency of the quadrant lines.&lt;/desc>
    ///</remarks>
    public const string Chart_QuadrantLineAlpha = "quadrantLineAlpha";
  
    ///<summary>QuadrantLineIsDashed</summary>
    ///<remarks>
///&lt;desc>Whether the quadrant
///lines should appear as dashed.&lt;/desc>
    ///</remarks>
    public const string Chart_QuadrantLineIsDashed = "quadrantLineIsDashed";
  
    ///<summary>QuadrantLineDashLen</summary>
    ///<remarks>
///&lt;desc>If you've opted to show
///quadrant lines as dashed, this attribute lets you specify the
///length of the dash.&lt;/desc>
    ///</remarks>
    public const string Chart_QuadrantLineDashLen = "quadrantLineDashLen";
  
    ///<summary>QuadrantLineDashGap</summary>
    ///<remarks>
///&lt;desc>If you've opted to show
///quadrant lines as dashed, this attribute lets you specify the
///length of the dash gap.&lt;/desc>
    ///</remarks>
    public const string Chart_QuadrantLineDashGap = "quadrantLineDashGap";
  
    ///<summary>QuadrantLabelTL</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_QuadrantLabelTL = "quadrantLabelTL";
  
    ///<summary>QuadrantLabelTR</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_QuadrantLabelTR = "quadrantLabelTR";
  
    ///<summary>QuadrantLabelBL</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_QuadrantLabelBL = "quadrantLabelBL";
  
    ///<summary>QuadrantLabelBR</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_QuadrantLabelBR = "quadrantLabelBR";
  
    ///<summary>QuadrantLabelPadding</summary>
    ///<remarks>
///&lt;desc>This attribute lets you
///control the space between quadrant labels and the associated
///quadrant lines.&lt;/desc>
    ///</remarks>
    public const string Chart_QuadrantLabelPadding = "quadrantLabelPadding";
  
    ///<summary>NumVDivlines</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Number of vertical axis division lines that you
///want to display on the chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_NumVDivlines = "numVDivlines";
  
    ///<summary>VDivlineColor</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_VDivlineColor = "vDivlineColor";
  
    ///<summary>VDivlineThickness</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Thickness of vertical divisional lines.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_VDivlineThickness = "vDivlineThickness";
  
    ///<summary>VDivlineAlpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Transparency of the vertical divisional
///lines.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_VDivlineAlpha = "vDivlineAlpha";
  
    ///<summary>VDivlineIsDashed</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether the vertical divisional lines should
///display as dash.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_VDivlineIsDashed = "vDivlineIsDashed";
  
    ///<summary>VDivlineDashLen</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If the vertical divisional lines are to be
///displayed as dash, this attribute lets you control the length of
///each dash.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_VDivlineDashLen = "vDivlineDashLen";
  
    ///<summary>VDivlineDashGap</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If the vertical divisional lines are to
///displayed as dash, this attribute lets you control the length of
///each gap between dash.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_VDivlineDashGap = "vDivlineDashGap";
  
    ///<summary>ShowVZeroPlane</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether to show vertical zero plane.&lt;/span>
///  &lt;span>Zero Plane is the line/plane that appears at 0
///x-position on canvas, when negative data is being shown on the
///chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_ShowVZeroPlane = "showVZeroPlane";
  
    ///<summary>VZeroPlaneColor</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_VZeroPlaneColor = "vZeroPlaneColor";
  
    ///<summary>VZeroPlaneThickness</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Thickness of the vertical zero plane.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_VZeroPlaneThickness = "vZeroPlaneThickness";
  
    ///<summary>VZeroPlaneAlpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Transparency of the vertical zero plane.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_VZeroPlaneAlpha = "vZeroPlaneAlpha";
  
    ///<summary>YFormatNumber</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This configuration determines whether the
///numbers displayed on the y-axis of the chart will be formatted
///using commas, e.g., 40,000 if 
///&lt;span>formatNumber='1'&lt;/span> and 40000 if 
///&lt;span>formatNumber= '0'&lt;/span>. If this
///attribute is not specifically mentioned, the chart inherits the
///default value from&lt;/span>
///  &lt;span>formatNumber&lt;/span>
///  &lt;span>attribute. For more details, please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Basics.html" target="_blank">Basics&lt;/a>
///page.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_YFormatNumber = "yFormatNumber";
  
    ///<summary>XFormatNumber</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This configuration determines whether the
///numbers displayed on the x-axis of the chart will be formatted
///using commas, e.g., 40,000 if 
///&lt;span>formatNumber='1'&lt;/span> and 40000 if 
///&lt;span>formatNumber= '0'&lt;/span>. If this
///attribute is not specifically mentioned, the chart inherits the
///default value from&lt;/span>
///  &lt;span>formatNumber&lt;/span>
///  &lt;span>attribute. For more details, please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Basics.html" target="_blank">Basics&lt;/a>
///page.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_XFormatNumber = "xFormatNumber";
  
    ///<summary>YFormatNumberScale</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Configuration whether to add K (thousands) and M
///(millions) to a number after truncating and rounding it on the
///y-axis - e.g., if&lt;/span>
///  &lt;span>yFormatNumberScale&lt;/span>
///  &lt;span>is set to 1, 1043 will become 1.04K (with
///decimals set to 2 places). Same with numbers in millions - an M
///will be added at the end.&lt;/span>
///  &lt;span>If this attribute is not specifically mentioned,
///the chart inherits the default value from&lt;/span>
///  &lt;span>formatNumberScale&lt;/span>
///  &lt;span>attribute. For more details, please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Scaling.html" target="_blank">Number
///Scaling&lt;/a> page.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_YFormatNumberScale = "yFormatNumberScale";
  
    ///<summary>XFormatNumberScale</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Configuration whether to add K (thousands) and M
///(millions) to a number after truncating and rounding it on the
///x-axis - e.g., if&lt;/span>
///  &lt;span>xFormatNumberScale&lt;/span>
///  &lt;span>is set to 1, 1043 will become 1.04K (with
///decimals set to 2 places). Same with numbers in millions - an M
///will be added at the end.&lt;/span>
///  &lt;span>If this attribute is not specifically mentioned,
///the chart inherits the default value from&lt;/span>
///  &lt;span>formatNumberScale&lt;/span>
///  &lt;span>attribute. For more details, please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Scaling.html" target="_blank">Number
///Scaling&lt;/a> page.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_XFormatNumberScale = "xFormatNumberScale";
  
    ///<summary>YDefaultNumberScale</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_YDefaultNumberScale = "yDefaultNumberScale";
  
    ///<summary>XDefaultNumberScale</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_XDefaultNumberScale = "xDefaultNumberScale";
  
    ///<summary>YNumberScaleUnit</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_YNumberScaleUnit = "yNumberScaleUnit";
  
    ///<summary>XNumberScaleUnit</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_XNumberScaleUnit = "xNumberScaleUnit";
  
    ///<summary>XScaleRecursively</summary>
    ///<remarks>
///&lt;desc>Whether to scale the number
///recursively for the x-axis? This attribute will work only if the
///attribute 
///&lt;span>xAxisLabelMode&lt;/span> is set to '
///&lt;span>AUTO&lt;/span>' or '
///&lt;span>MIXED&lt;/span>'. For more details, please
///see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Rec_Num_Scaling.html" target="_blank">Recursive
///Number Scaling&lt;/a> page.&lt;/desc>
    ///</remarks>
    public const string Chart_XScaleRecursively = "xScaleRecursively";
  
    ///<summary>XMaxScaleRecursion</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_XMaxScaleRecursion = "xMaxScaleRecursion";
  
    ///<summary>XScaleSeparator</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_XScaleSeparator = "xScaleSeparator";
  
    ///<summary>YNumberScaleValue</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_YNumberScaleValue = "yNumberScaleValue";
  
    ///<summary>XNumberScaleValue</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Chart_XNumberScaleValue = "xNumberScaleValue";
  
    ///<summary>YNumberPrefix</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Using this attribute, you could add prefix to
///all the numbers visible on the y-axis. For example, to represent
///all dollars figure on the y-axis, you could specify this attribute
///to ' $' to show like $40000, $50000. If this attribute is not
///specifically mentioned, the chart inherits the default value from 
///&lt;span>NumberPrefix&lt;/span> attribute. For more
///details, please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Basics.html#prefix" target="_blank">Basics&lt;/a>
///page.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_YNumberPrefix = "yNumberPrefix";
  
    ///<summary>XNumberPrefix</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Using this attribute, you could add prefix to
///all the numbers visible on the x-axis. For example, to represent
///all dollars figure on the x-axis, you could specify this attribute
///to ' $' to show like $40000, $50000. If this attribute is not
///specifically mentioned, the chart inherits the default value from 
///&lt;span>NumberPrefix&lt;/span> attribute. For more
///details, please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Basics.html#prefix" target="_blank">Basics&lt;/a>
///page.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_XNumberPrefix = "xNumberPrefix";
  
    ///<summary>YNumberSuffix</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Using this attribute, you could add a suffix to
///all the numbers visible on the y-axis. For example, to represent
///all figure quantified as per annum on the y-axis, you could specify
///this attribute to ' /a' to show like 40000/a, 50000/a. If this
///attribute is not specifically mentioned, the chart inherits the
///default value from 
///&lt;span>NumberSuffix&lt;/span> attribute. For more
///details, please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Basics.html#prefix" target="_blank">Basics&lt;/a>
///page.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_YNumberSuffix = "yNumberSuffix";
  
    ///<summary>XNumberSuffix</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Using this attribute, you could add a suffix to
///all the numbers visible on the x-axis. For example, to represent
///all figure quantified as per annum on the x-axis, you could specify
///this attribute to ' /a' to show like 40000/a, 50000/a.If this
///attribute is not specifically mentioned, the chart inherits the
///default value from 
///&lt;span>NumberSuffix&lt;/span> attribute. For more
///details, please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Basics.html#prefix" target="_blank">Basics&lt;/a>
///page.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_XNumberSuffix = "xNumberSuffix";
  
    ///<summary>ForceXAxisValueDecimals</summary>
    ///<remarks>
///&lt;desc>Whether to forcefully
///attach decimal places to all x-axis values. For example, If you
///limit the maximum number of decimal digits to 2, a number like
///55.345 will be rounded to 55.34. This does not mean that all x-axis
///numbers will be displayed with a fixed number of decimal places.
///For instance 55 will not be displayed as 55.00 and 55.1 will not
///become 55.10. In order to have fixed number of decimal places
///attached to all x-axis numbers, set this attribute to 1. For more
///details, please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Basics.html#yaxis" target="_blank">Basics&lt;/a>
///page.&lt;/desc>
    ///</remarks>
    public const string Chart_ForceXAxisValueDecimals = "forceXAxisValueDecimals";
  
    ///<summary>XAxisValueDecimals</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you've opted not to adjust vertical div
///lines, you can specify the decimal precision of the div line values
///using this attribute. For more details, please see 
///&lt;strong>Advanced Charting &amp;gt; Number Formatting &amp;gt;&lt;/strong>&lt;a href="http://docs.fusioncharts.com/charts/contents/advanced/number-format/Number_Basics.html#yaxis" target="_blank">Basics&lt;/a>
///page.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Chart_XAxisValueDecimals = "xAxisValueDecimals";
  
    ///<summary>VerticalLineColor</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>In the bubble/scatter chart, you can opt to show
///vertical lines for each category label. This attribute lets you set
///the color for all of them.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Categories_VerticalLineColor = "verticalLineColor";
  
    ///<summary>VerticalLineThickness</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Lets you set the thickness for category
///lines.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Categories_VerticalLineThickness = "verticalLineThickness";
  
    ///<summary>VerticalLineAlpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Lets you set the alpha for category
///lines.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Categories_VerticalLineAlpha = "verticalLineAlpha";
  
    ///<summary>VerticalLineDashed</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether the category lines should appear as
///dashed.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Categories_VerticalLineDashed = "verticalLineDashed";
  
    ///<summary>VerticalLineDashLen</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If the category lines are to appear as dash,
///this attribute defines the length of dash.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Categories_VerticalLineDashLen = "verticalLineDashLen";
  
    ///<summary>VerticalLineDashGap</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If the category lines are to appear as dash,
///this attribute defines the length of gap between two consecutive
///dashes.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Categories_VerticalLineDashGap = "verticalLineDashGap";
  
    ///<summary>X</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>The bubble/scatter chart have both x and y axis
///as numeric. This attribute lets you define the x value (numerical
///position on the x-axis) where this category name will be
///placed.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Category_X = "x";
  
    ///<summary>ShowVerticalLine</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether the vertical line should be shown for
///this category.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Category_ShowVerticalLine = "showVerticalLine";
  
    ///<summary>LineDashed</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether the vertical line should appear as
///dashed.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Category_LineDashed = "lineDashed";
  
    ///<summary>PlotFillAlpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute sets the alpha (transparency) of
///the entire dataset.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Dataset_PlotFillAlpha = "plotFillAlpha";
  
    ///<summary>ShowRegressionLine</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute when set to '1'  draws a 
///&lt;a href="http://docs.fusioncharts.com/charts/contents/AttDesc/BubbleScatter.html#regression" target="_blank">regression
///line&lt;/a> for the corresponding 
///&lt;span>&amp;lt;dataset&amp;gt;&lt;/span> element in the
///chart.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Dataset_ShowRegressionLine = "showRegressionLine";
  
    ///<summary>ShowYOnX</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>When it is set to '1' the 
///&lt;a href="http://docs.fusioncharts.com/charts/contents/AttDesc/BubbleScatter.html#regression" target="_blank">regression
///line&lt;/a> is drawn using 
///&lt;a href="http://docs.fusioncharts.com/charts/contents/AttDesc/BubbleScatter.html#regressionmodes" target="_blank">&lt;strong>Y On X&lt;/strong>&lt;/a> mode and when the attribute is set to '0' the regression line
///is drawn using 
///&lt;a href="http://docs.fusioncharts.com/charts/contents/AttDesc/BubbleScatter.html#regressionmodes" target="_blank">&lt;strong>X On Y&lt;/strong>&lt;/a> mode. By default, this attribute is set to '1'.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Dataset_ShowYOnX = "showYOnX";
  
    ///<summary>RegressionLineColor</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Dataset_RegressionLineColor = "RegressionLineColor";
  
    ///<summary>RegressionLineThickness</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Dataset_RegressionLineThickness = "RegressionLineThickness";
  
    ///<summary>RegressionLineAlpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>This attribute allows you to set the
///transparency of the regression line.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Dataset_RegressionLineAlpha = "RegressionLineAlpha";
  
    ///<summary>X</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>X-axis value for the set. The bubble/scatter
///point will be placed horizontally on the x-axis based on this
///value.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Set_X = "x";
  
    ///<summary>Y</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Y-axis value for the set. The bubble/scatter
///point will be placed vertically on the y-axis based on this
///value.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Set_Y = "y";
  
    ///<summary>Z</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Z-axis numerical value for the set of data. The
///size of bubble will depend on this value (with respect to z values
///of other bubbles on the chart).&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Set_Z = "z";
  
    ///<summary>Name</summary>
    ///<remarks>
///&lt;desc />
    ///</remarks>
    public const string Set_Name = "name";
  
    ///<summary>StartValue</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>The starting value for the trendline. Say, if
///you want to plot a slanted trendline from value 102 to 109,
///the&lt;/span>
///  &lt;span>startValue&lt;/span>
///  &lt;span>will be 102.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Set_StartValue = "startValue";
  
    ///<summary>EndValue</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>The ending y-axis value for the trendline. Say,
///if you want to plot a slanted trendline from value 102 to 109, the
///endValue will be 109. If you do not specify a value for&lt;/span>
///  &lt;span>endValue&lt;/span>
///  &lt;span>, it will automatically assume the same value
///as&lt;/span>
///  &lt;span>startValue&lt;/span>
///  &lt;span>.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Set_EndValue = "endValue";
  
    ///<summary>IsTrendZone</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Whether the trend will display a line, or a zone
///(filled colored rectangle).&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Set_IsTrendZone = "isTrendZone";
  
    ///<summary>DashLen</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you've opted to show trend line as dash, this
///attribute lets you control the length of each dash.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Set_DashLen = "dashLen";
  
    ///<summary>DashGap</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>If you've opted to show trend line as dash, this
///attribute lets you control the length of each dash gap.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Set_DashGap = "dashGap";
  
    ///<summary>DrawLine</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>You can opt to connect the scatter points of any
///dataset using lines. This attribute helps you configure
///that.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Dataset_DrawLine = "drawLine";
  
    ///<summary>LineColor</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Color of the line connecting the scatter
///points.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Dataset_LineColor = "lineColor";
  
    ///<summary>LineAlpha</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Alpha of the lines connecting the scatter
///points.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Dataset_LineAlpha = "lineAlpha";
  
    ///<summary>LineDashed</summary>
    ///<remarks>
///&lt;desc>
///  &lt;span>Configuration Whether to show the lines
///connecting the scatter points as dash.&lt;/span>
///&lt;/desc>
    ///</remarks>
    public const string Dataset_LineDashed = "lineDashed";
  
    ///<summary>NumVisiblePlot</summary>
    ///<remarks>
///&lt;desc>This attribute lets you control how
///many data plots will appear in the visible area of the scroll pane.
///&lt;p>For example, if you're plotting a chart with 25 data items in
///each dataset, and you wish to show only 10 data items in the
///visible area, set this attribute as 10. You will now see only 10
///data points on the chart - rest of the data points will be visible
///upon scrolling.&lt;/p>&lt;p>If you want to show all the data points on the chart
///irrespective of the number of data points in your XML data
///document, set this attribute as 0.&lt;/p>&lt;/desc>
    ///</remarks>
    public const string Chart_NumVisiblePlot = "numVisiblePlot";
  
    ///<summary>ScrollToEnd</summary>
    ///<remarks>
///&lt;desc>When the chart renders, you can opt
///to scroll to the end of the chart (to show data at extreme right)
///by setting this attribute as 1.&lt;/desc>
    ///</remarks>
    public const string Chart_ScrollToEnd = "scrollToEnd";
  
   }
}
