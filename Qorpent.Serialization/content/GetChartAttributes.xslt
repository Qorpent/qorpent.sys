<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="text" indent="yes"/>
  <xsl:variable name="smallcase" select="'abcdefghijklmnopqrstuvwxyz'" />
  <xsl:variable name="uppercase" select="'ABCDEFGHIJKLMNOPQRSTUVWXYZ'" />
  <xsl:template match="/">
using Qorpent.Charts.FusionCharts;
    
namespace Qorpent.Charts {
  /// &lt;summary&gt;
  ///
  /// &lt;/summary&gt;
  public partial class Chart : IChart {
  <xsl:for-each select="//attribute[@common='true']">
    /// &lt;summary&gt;
    /// 
    /// &lt;/summary&gt;
    public <xsl:choose>
      <xsl:when test="@type = 'String'">string</xsl:when>
      <xsl:when test="@type = 'Number'">int</xsl:when>
      <xsl:when test="@type = 'Boolean'">bool</xsl:when>
      <xsl:when test="@type = 'Color'">IChartColor</xsl:when>
      <xsl:otherwise>object</xsl:otherwise>
    </xsl:choose>&#160;<xsl:value-of select="concat(translate(substring(@name, 1, 1), $smallcase, $uppercase), substring(@name, 2))"/> {
      get { return Get&lt;<xsl:choose>
    <xsl:when test="@type = 'String'">string</xsl:when>
    <xsl:when test="@type = 'Number'">int</xsl:when>
    <xsl:when test="@type = 'Boolean'">bool</xsl:when>
    <xsl:when test="@type = 'Color'">IChartColor</xsl:when>
    <xsl:otherwise>object</xsl:otherwise>
  </xsl:choose>&gt;(Api.Chart_<xsl:value-of select="concat(translate(substring(@name, 1, 1), $smallcase, $uppercase), substring(@name, 2))"/>); }
      set { Set(Api.Chart_<xsl:value-of select="concat(translate(substring(@name, 1, 1), $smallcase, $uppercase), substring(@name, 2))"/>, value); }
    }
  </xsl:for-each>
  }
}
  </xsl:template>
</xsl:stylesheet>
