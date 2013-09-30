<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl fun"  xmlns:fun="fun"
>
    <xsl:output method="xml" indent="yes"/>
  <msxsl:script language="C#" implements-prefix="fun">
    public string trim(string str){
       return str.Trim();
    }
    public bool contains(string str, string other){
       return str.Contains(other);
    }
  </msxsl:script>
    <xsl:template match="@* | node()">
        <xsl:copy>
            <xsl:apply-templates select="@* | node()"/>
        </xsl:copy>
    </xsl:template>

  <xsl:template match="@__UID"></xsl:template>
  <xsl:template match="@class"></xsl:template>
  <xsl:template match="div[@tag='tr' and position()=1]"/>
  <xsl:template match="div[@tag='tr' and fun:contains(.,'Back to top')]" priority="10"/>
  <xsl:template match="div[@tag='td']" />
  
   <xsl:template match="part[not(category)]" />
  
  <xsl:template match="div[@tag='table']">
    <xsl:apply-templates select="node()"/>
  </xsl:template>

  <xsl:template match="category[attrtable]">
    <xsl:apply-templates select="node()"/>
  </xsl:template>

  <xsl:template match="attrtable">
    <xsl:copy>
      <xsl:attribute name="category">
        <xsl:value-of select="../@name"/>
      </xsl:attribute>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>
    
  <xsl:template match="div[@tag='tr' and position()!=1]">
    <attribute name="{fun:trim(*[1])}" type="{fun:trim(*[2])}" range="{fun:trim(*[3])}">
      <xsl:apply-templates select="*[4]/node()"/>
    </attribute>
  </xsl:template>
  
  
</xsl:stylesheet>
