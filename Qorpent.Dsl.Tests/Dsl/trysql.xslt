<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl" xmlns:sql="http://qorpent/dsl/sql">
	<xsl:output method="text" indent="yes" />
	<xsl:param name="SELECTD" />

	<xsl:template match="@* | node()">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>

	<xsl:template match="/" xml:space="default">
		<xsl:apply-templates select="//str" />
		<xsl:if test="$SELECTD = '1'" xml:space="preserve">
		select 'D'       
    </xsl:if>
	</xsl:template>


	<xsl:template match="str">
		select
		<xsl:value-of select="sql:str(.)" />
	</xsl:template>


</xsl:stylesheet>