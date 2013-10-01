<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl" xmlns:f="func"
>
  <xsl:import href="scripts.xslt"/>
    <xsl:output method="text" indent="yes"/>
  <xsl:template match="/">
    <xsl:text disable-output-escaping="yes">
using System;
using System.Collections;
using System.Collections.Generic;
namespace Qorpent.Charts.FusionCharts {
///&lt;summary>
///Описывает атрибуты и прочие соглашения по атрибутам FusionChart
///&lt;/summary>
  public static partial class Api {
</xsl:text>
     ///&lt;summary>Полный реестр атрибутов&lt;/summary>
    public static readonly IDictionary&lt;string,FusionChartAttributeDescriptor&gt; Attributes = new Dictionary&lt;string,FusionChartAttributeDescriptor&gt;{
         <xsl:apply-templates select="//attribute" mode="dict"/>
      <xsl:text>
    };
   }
}
</xsl:text>
  </xsl:template>
  <xsl:template match="attribute" mode="dict">
    { <xsl:value-of select="@element"/>_<xsl:value-of select="f:PascalCase(@name)"/>, new FusionChartAttributeDescriptor { 
      Name = <xsl:value-of select="@element"/>_<xsl:value-of select="f:PascalCase(@name)"/>,
      Charts = <xsl:value-of select="concat( 'FusionChartType.' , f:Replace(@chart, '+ ', '| FusionChartType.'))"/>,
      Element = <xsl:value-of select="concat( 'FusionChartElementType.' , f:PascalCase(@element))"/>,
    } },
  </xsl:template>
</xsl:stylesheet>
