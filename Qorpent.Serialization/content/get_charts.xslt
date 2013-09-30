<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
    <xsl:output method="text" indent="yes"/>


  <xsl:template match="/">
using System;
    namespace Qorpent.Charts.FusionCharts{
    ///&lt;summary>
 ///Типы графиков FusionChart
 ///&lt;/summary>
    [Flags]
    public enum FusionChartType {
      <xsl:apply-templates select="(.//part/@charttype)"/>
    }
}
  </xsl:template>
  <xsl:template match="@charttype">
    ///&lt;summary><xsl:value-of select="."/>&lt;/summary>
    <xsl:value-of select="."/> = 1&lt;&lt;<xsl:value-of select="position()"/>,    
  </xsl:template>
</xsl:stylesheet>
