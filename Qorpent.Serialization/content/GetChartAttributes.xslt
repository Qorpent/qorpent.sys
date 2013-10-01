<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="xml" indent="yes"/>
  <xsl:variable name="smallcase" select="'abcdefghijklmnopqrstuvwxyz'" />
  <xsl:variable name="uppercase" select="'ABCDEFGHIJKLMNOPQRSTUVWXYZ'" />

  <xsl:template match="/">
namespace Qorpent.Charts {
  public partial class Chart : IChart {
  <xsl:for-each select="//attribute[@common='true']">
    public <xsl:value-of select="@type" />&#160;<xsl:value-of select="concat(translate(substring(@name, 1, 1), $smallcase, $uppercase), substring(@name, 2))"/> {
      get { return Get&lt;<xsl:value-of select="@type" />&gt;("Chart<xsl:value-of select="concat(translate(substring(@name, 1, 1), $smallcase, $uppercase), substring(@name, 2))"/>AttributeProperty"); }
      set { Set("Chart<xsl:value-of select="concat(translate(substring(@name, 1, 1), $smallcase, $uppercase), substring(@name, 2))"/>AttributeProperty", value); }
    }
  </xsl:for-each>
  }
}
  </xsl:template>
</xsl:stylesheet>
