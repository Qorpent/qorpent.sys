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
    public string match(string str, string pattern, int group){
    return Regex.Match(str,pattern).Groups[group].Value;
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


  <xsl:template match="category[fun:contains(@name,'&gt; element')]">
      <xsl:variable name="subelement" select="fun:match(@name,'of &lt;(lineset)&gt;',1)"/>  
      <xsl:copy>
        <xsl:attribute name="element">
          <xsl:value-of select="fun:match(@name,'&lt;(\w+)&gt;',1)"/>
        </xsl:attribute>
        <xsl:if test="$subelement">
          <xsl:attribute name="parent">
          <xsl:value-of select="$subelement"/>
        </xsl:attribute>
        </xsl:if>
        <xsl:apply-templates select="@* | node()"/>
      </xsl:copy>
  </xsl:template>


  <xsl:template match="part">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
      <xsl:apply-templates select=".//category" mode="special"/>
    </xsl:copy>
  </xsl:template>
  <xsl:template match="category[@name='Vertical data separator lines']"/>
  <xsl:template match="category[@name='Trend-lines']"/>
  
  <xsl:template match="category" mode="special" />
  <xsl:template match="category[@name='Vertical data separator lines']" mode="special">
        <xsl:copy>
          <xsl:attribute name="element">vLine</xsl:attribute>
          <xsl:apply-templates select="@* | node()"/>
      </xsl:copy>
  </xsl:template>
  <xsl:template match="category[@name='Trend-lines']" mode="special">
     <xsl:copy>
          <xsl:attribute name="element">line</xsl:attribute>
          <xsl:apply-templates select="@* | node()"/>
      </xsl:copy>
  
  </xsl:template>

 
  
</xsl:stylesheet>
