<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl" xmlns:f="func"
>
  <xsl:import href="scripts.xslt"/>
    <xsl:output method="text" indent="yes"/>
  <xsl:template match="/">
    <xsl:text disable-output-escaping="yes">
namespace Qorpent.Charts.FusionCharts {
///&lt;summary>
///Описывает атрибуты и прочие соглашения по атрибутам FusionChart
///&lt;/summary>
  public static partial class FusionChartApi {
</xsl:text>
    <xsl:apply-templates select="//attribute" mode="constant"/>
    <xsl:text>
   }
}
</xsl:text>
  </xsl:template>
  <xsl:template match="attribute" mode="constant">
    ///&lt;summary><xsl:value-of select="f:PascalCase(@name)"/>&lt;/summary>
    ///&lt;remarks><xsl:value-of select="f:AsComment(.)" />
    ///&lt;/remarks>
    public const string <xsl:value-of select="@element"/>_<xsl:value-of select="f:PascalCase(@name)"/> = "<xsl:value-of select="@name"/>";
  </xsl:template>
</xsl:stylesheet>
