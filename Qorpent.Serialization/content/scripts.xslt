﻿<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl" xmlns:f="func"
>

  <msxsl:script language="C#" implements-prefix="f">
    public string PascalCase (string s){
    if(string.IsNullOrEmpty(s))return "";
    if(s.Contains(" ")){
    var res = new StringBuilder();
    foreach(var n in s.Split(' ')){
    res.Append(PascalCase(n));
    }
    return res.ToString();
    }
    return (s[0].ToString().ToUpper()+s.Substring(1)).Replace(" ","_");
    }
    
    public string AsComment(string s){
    // throw new Exception(s);
    return "\n///"+ s.Replace("\n","\n///").Replace("&amp;","&amp;amp;").Replace("&lt;","&amp;lt;");
    }
    
    public string Replace (string s, string f, string r) {
      return s.Replace(f,r);
    }
  </msxsl:script>
   
</xsl:stylesheet>
