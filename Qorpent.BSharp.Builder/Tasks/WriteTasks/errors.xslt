<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl" xmlns:ex="bsharp-errors"
>
    <xsl:output method="html" indent="yes"/>
  <msxsl:script implements-prefix="ex" language="C#">
    <![CDATA[
    public string toxmlstring(XPathNodeIterator iter) {
    var sw = new System.IO.StringWriter();
    var xw = XmlWriter.Create(sw);
    while (iter.MoveNext()) {
    iter.Current.WriteSubtree(xw);
    xw.Flush();
    }
    var result = sw.ToString();
    return result;
    }
    ]]>
  </msxsl:script>

    <xsl:template match="@* | node()">
        <xsl:copy>
            <xsl:apply-templates select="@* | node()"/>
        </xsl:copy>
    </xsl:template>
  <xsl:template match="/">
    <html>
      <head>
        <title>Отчет об ошибках</title>
        <style type="text/css">
          table.errors {
            border-collapse:collapse;
          }
          table.errors td,table.errors th {
            border : solid 1px gray;
            padding : 2px;
          }
          tr.level-Error td {
            background-color : #FFAAAA;
          }
        </style>
      </head>
      <body>
        <h1>Отчет об ошибках компиляции B#</h1>
        <table class="errors">
          <thead>
            <tr>
              <th>#</th>
              <th>Уровень</th>
              <th>Тип</th>
              <th>Фаза</th>
              <th>Класс</th>
              <th>Сообщение</th>
              <th>Контент</th>
            </tr>
          </thead>
          <tbody>
            <xsl:apply-templates select="//errors/item"/>
          </tbody>
        </table>
      </body>
    </html>
  </xsl:template>
  <xsl:template match="errors/item">
    <tr class="level-{@Level}">
      <td>
        <xsl:value-of select="position()"/>
      </td>
      <td>
        <xsl:value-of select="@Level"/>
      </td>
      <td>
        <xsl:value-of select="@Type"/>
      </td>
      <td>
        <xsl:value-of select="@Phase"/>
      </td>

      <td>
        <xsl:value-of select="@ClassName"/>
      </td>
      <td>
        <xsl:value-of select="@Message"/>
      </td>
      <td>
        <xsl:value-of select="ex:toxmlstring(msxsl:node-set(Xml) )"/>
      </td>
    </tr>
  </xsl:template>
</xsl:stylesheet>
