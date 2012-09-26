<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:MSHelp="http://msdn.microsoft.com/mshelp" xmlns:ndoc="urn:ndoc-schema">
	<xsl:template match="ndoc:ndoc" mode="header-section">

		<style type="text/css">
			qorpentimplemented

			{
			display:block;
			border: solid 1px black;
			background-color:FFAAAA;
			color:black;

			}

			note

			{
			display:block;
			border: solid 1px black;
			background-color:yellow;

			}
		</style>

	</xsl:template>
	<xsl:template match="ndoc:source" mode="summary-section">
		Исходный код:
		<a href="https://assoi-svn.ugmk.com:8443/svn/main/{.}" target="_blank">
			<xsl:value-of select="." />
		</a>
	</xsl:template>

	<xsl:template match="ndoc:qorpentimplemented" mode="summary-section">
		<xsl:apply-templates select="." mode="slashdoc" />
	</xsl:template>

	<xsl:template match="ndoc:h3" mode="slashdoc">
		<h3>
			<xsl:value-of select="." />
		</h3>
	</xsl:template>

	<xsl:template match="ndoc:qorpentimplemented" mode="slashdoc">
		<qorpentimplemented>
			Самостоятельная реализация интерфейса рекомендуется только в случае тестирования.
			<br />
			Для прикладного использования следует использовать через IoC стандартную реализацию Qorpent
			<xsl:if test="@ref">
				<br />
				См.:
				<a href="{@ref}.html">
					<xsl:value-of select="." />
				</a>
			</xsl:if>
		</qorpentimplemented>
	</xsl:template>

	<xsl:template match="ndoc:interface//ndoc:invariant" mode="slashdoc">
		<xsl:variable name="parent" select="ancestor::ndoc:interface" />
		<xsl:variable name="impl" select="$parent//ndoc:qorpentimplemented" />
		<note>
			Информация ниже отражает инвариантное поведение
			<a href="{$parent/@assembly}~{$parent/@namespace}.{$parent/@name}.html">
				<xsl:value-of select="$parent/@name" />
			</a>
			в
			<a href="{$impl/@ref}.html">
				<xsl:value-of select="$impl" />
			</a>
		</note>
		<xsl:apply-templates select="node()" mode="slashdoc" />
	</xsl:template>

</xsl:stylesheet>