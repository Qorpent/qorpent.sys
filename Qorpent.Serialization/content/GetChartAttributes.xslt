<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="xml" indent="yes"/>
  <xsl:template match="/">
      /// &lt;summary>
      ///   Типы графиков FusionChart
      /// &lt;/summary>
      public partial Chart : IChart {
        <xsl:apply-templates select="(//attribute[@common='true']/@name)"/>
      }
    }
  </xsl:template>
</xsl:stylesheet>
