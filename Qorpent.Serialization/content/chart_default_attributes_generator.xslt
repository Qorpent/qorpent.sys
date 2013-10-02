<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl" xmlns:f="func"
>
  <xsl:import href="scripts.xslt"/>
    <xsl:output method="text" indent="yes"/>
  <xsl:template match="/">
    <xsl:text disable-output-escaping="yes">
using System;
using Qorpent.Charts.FusionCharts;
namespace Qorpent.Charts {
  public partial class Chart {
</xsl:text>
   
         <xsl:apply-templates select="//attribute[@element='Chart' and @default='true']" mode="default"/>
      <xsl:text>
   
   }
}
</xsl:text>
  </xsl:template>
  <xsl:template match="attribute" mode="default" xml:space="preserve">
    <xsl:variable name="const" select="concat(' FusionChartApi.', @element,'_',f:PascalCase(@name))"/>
    ///&lt;summary><xsl:value-of select="f:PascalCase(@name)"/> (<xsl:value-of select="f:Replace(@category,'&amp;','&amp;amp;')"/>)&lt;/summary>
    ///&lt;remarks><xsl:value-of select="f:AsComment(.)" />
    ///&lt;/remarks>
    public <xsl:value-of select="f:ToSystemType(@type,@range)"/> <xsl:value-of select="f:PascalCase(@name)"/> {
    get { return Get&lt;<xsl:value-of select="f:ToSystemType(@type,@range)"/>&gt;(<xsl:value-of select="$const"/> ); }
        set { Set(<xsl:value-of select="$const"/>, value); }
    }
  </xsl:template>
</xsl:stylesheet>
