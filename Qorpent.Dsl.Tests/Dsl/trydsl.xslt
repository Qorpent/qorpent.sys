<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
                xmlns:cs="http://qorpent/dsl/csharp"
>
    <xsl:output method="text" indent="yes"/>

    <xsl:template match="@* | node()">
        <xsl:copy>
            <xsl:apply-templates select="@* | node()"/>
        </xsl:copy>
    </xsl:template>

  <xsl:template match="/" xml:space="preserve">
using System;
using Qorpent.Dsl.Tests.Dsl;
using System.Collections.Generic;
namespace Tests {
    public class Test:TryDslFramework.ITryDsl{
    public IEnumerable&lt;string&gt; GetStrs(){
          <xsl:apply-templates select="//str"/>
#if YIELDD
          yield return "D";
#endif
        }
    }
}
  </xsl:template>

  <xsl:template match="str">
            yield return <xsl:value-of select="cs:vstr(.)"/>;
  </xsl:template>
    
  
</xsl:stylesheet>
