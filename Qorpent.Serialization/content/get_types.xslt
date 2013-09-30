<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl" xmlns:f="func"
>
    <xsl:output method="text" indent="yes"/>
  <xsl:key name="types" match="attribute" use="@type"/>
  <msxsl:script language="C#" implements-prefix="f">
    public string PascalCase (string s){
       if(string.IsNullOrEmpty(s))return "";
       if(s.Contains(" ")){
         var res = new StringBuilder();
         foreach(var n in s.Split(' ')){
            res.Append(PascalCase(n));
         }
         return res.ToString();
       }
       return (s[0].ToString().ToUpper()+s.Substring(1)).Replace(" ","_");
    }
  </msxsl:script>
  <xsl:template match="/">
using System;
    namespace Qorpent.Charts.FusionCharts{
    ///&lt;summary>
 ///Типы данных графиков FusionChart
 ///&lt;/summary>
   [Flags]
   public enum FusionChartDataType {
      <xsl:apply-templates select="(.//attribute[f:PascalCase(@type)!='' and generate-id()=generate-id(key('types',@type))])"/>
    }
}
  </xsl:template>
  <xsl:template match="attribute">
    ///&lt;summary><xsl:value-of select="f:PascalCase(@type)"/>&lt;/summary>
    <xsl:value-of select="f:PascalCase(@type)"/> = 1&lt;&lt;<xsl:value-of select="position()"/>,    
  </xsl:template>
</xsl:stylesheet>
