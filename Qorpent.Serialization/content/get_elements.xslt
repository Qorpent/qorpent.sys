<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl" xmlns:f="func"
>
    <xsl:output method="text" indent="yes"/>
  <xsl:key name="categories" match="category" use="concat(@subelement,@element)"/>
  <msxsl:script language="C#" implements-prefix="f">
    public string PascalCase (string s){
       if(string.IsNullOrEmpty(s))return "";
       return s[0].ToString().ToUpper()+s.Substring(1);
    }
  </msxsl:script>
  <xsl:template match="/">
using System;
    namespace Qorpent.Charts.FusionCharts{
    ///&lt;summary>
 ///Типы элементов графиков FusionChart
 ///&lt;/summary>
    [Flags]
    public enum FusionChartElementType {
    ///&lt;summary>Не указан&lt;/summary>
    None = 0,
    <xsl:apply-templates select="(.//category[@element and generate-id()=generate-id(key('categories',concat(@subelement,@element)))])"/>
    }
}
  </xsl:template>
  <xsl:template match="category">
    ///&lt;summary><xsl:value-of select="concat(f:PascalCase(@subelement),f:PascalCase(@element))"/>&lt;/summary>
    <xsl:value-of select="concat(f:PascalCase(@subelement),f:PascalCase(@element))"/> = 1&lt;&lt;<xsl:value-of select="position()"/>,    
  </xsl:template>
</xsl:stylesheet>
