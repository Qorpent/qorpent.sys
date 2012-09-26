<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
   xmlns:c="http://qorpent/qview"
>
  <xsl:output method="text" indent="yes" omit-xml-declaration="yes" />
	<xsl:param name="_name"></xsl:param>
	<xsl:param name="_path"></xsl:param>
	<xsl:param name="_root"></xsl:param>
	<xsl:variable name="myname" select="c:getname($_path, $_root)"/>
<xsl:variable name="myactionname" select="c:getactionname($_path, $_root)"/>
	<xsl:variable name="myclsname" select="c:getclsname($_path, $_root)"/>
	<xsl:variable name="mylevel" select="c:getlevel($_path, $_root)"/>
	<xsl:variable name="clsname"><xsl:value-of select="$myclsname"/>_<xsl:value-of select="$mylevel"/>_View</xsl:variable>
  <xsl:variable name="basecls">
    <xsl:choose >
      <xsl:when test="/root/inherit"><xsl:value-of select="/root/inherit"/></xsl:when>
      <xsl:otherwise>QViewBase</xsl:otherwise>
    </xsl:choose>
  </xsl:variable>
  <xsl:template match="/">
<xsl:apply-templates select="root/action" mode="action"/>
@@@@/<xsl:value-of select="$clsname"/>.cs/@@@@
#region LICENSE

// Copyright 2007-2012 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Original file : <xsl:value-of select="$_path"/>
// Type : VIEW


#endregion
    //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
	using System;
	using System.Collections.Generic;
	using Qorpent.Utils;
	using Qorpent.Utils.Extensions;
	using Qorpent.Mvc;
	using Qorpent.Mvc.QView;
	  using System.Linq;
	  <xsl:apply-templates select="root/using" mode="using"/>
[QView("<xsl:value-of select="$myname"/>", QViewLevel.<xsl:value-of select="$mylevel"/>, Filename="<xsl:value-of select="c:esc($_path)"/>")]
public partial class <xsl:value-of select="$clsname"/> : <xsl:value-of select="$basecls"/> {
  /*CONSTBLOCK*/
<xsl:apply-templates select="root/data" mode="bind"/>
  <xsl:apply-templates select="root/bind/*" mode="bind"/>
    <xsl:apply-templates select="root/share/*" mode="bind"/>
    <xsl:apply-templates select="//capture/@__code" mode="bind"/>
//src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
    <xsl:if test="root/resources">  
      protected override void buildResources() {
      <xsl:apply-templates select="root/resources/*" mode="resource"/>
      }
    </xsl:if>

    protected override void Render(){
      <xsl:choose>
      <xsl:when test="root/render">
        <xsl:apply-templates select="root/render/*" mode="render"/>
      </xsl:when>
        <xsl:otherwise>
          <xsl:apply-templates select="root/*" mode="render"/>
        </xsl:otherwise>
      </xsl:choose>
	  }


	  }
	@@@@@@@@

	  @@@@/<xsl:value-of select="$clsname"/>._resource_partial_.cs/@@@@
#region LICENSE

// Copyright 2007-2012 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Original file : <xsl:value-of select="$_path"/>
// Type = View Extentor


#endregion
	using System.Collections.Generic;
	 
	  partial class <xsl:value-of select="$clsname"/> {
		static bool _resource_loaded = false;
		static object _resource_lock = new object();
		static IDictionary&lt;string,string&gt; _static_resources = new Dictionary&lt;string,string&gt;();
		protected override IDictionary&lt;string, string&gt; _getResources(){
		  return _static_resources;
		}
		protected override bool _getResourceLoaded(){  
		  return _resource_loaded;
		}
		protected override void _setResourceLoaded(){  
		  _resource_loaded = true;
		}
		protected override object _getResourceLock(){  
		  return _resource_lock;
		}
		
	  }
	  @@@@@@@@

  </xsl:template>

<xsl:template match="/root/action" mode="render"/>

<xsl:template match="/root/action" mode="action">

@@@@/<xsl:value-of select="$clsname"/>._action_.cs/@@@@
#region LICENSE

// Copyright 2007-2012 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Original file : <xsl:value-of select="$_path"/>
// Type : embeded Action


#endregion
			using System;
	using System.Collections.Generic;
	using Qorpent.Utils;
	using Qorpent.Utils.Extensions;
	using Qorpent.Mvc;
	using Qorpent.Mvc.QView;
	using Qorpent.Mvc.Binding;
	  using System.Linq;
	 
	[Action("<xsl:value-of select="$myactionname"/>", Role="<xsl:value-of select="c:esc(@role)"/>", Help="<xsl:value-of select="c:esc(@help)"/>" )]
	  public partial class <xsl:value-of select="$clsname"/>_Action : ActionBase {
			<xsl:apply-templates select="bind" mode="actionbind"/>		
			protected override object MainProcess () {
				  <xsl:value-of select="string(body)"/>
			}
	  }
	  @@@@@@@@

</xsl:template>


  <xsl:template match="*" mode="resource">
    <xsl:variable name="n" select="local-name(.)"/>
    <xsl:for-each select="@*">
      //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
        addResource("<xsl:value-of select="$n"/>", "<xsl:value-of select="local-name(.)"/>", "<xsl:value-of select="c:esc(.)"/>");
    </xsl:for-each>
  </xsl:template>
  
  <xsl:template match="using" mode="using">
    //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
using <xsl:value-of select="@__code"/>;
  </xsl:template>

  <xsl:template match="bind/*" mode="bind" xml:space="preserve">
    //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
  [QViewBind] <xsl:value-of select="@__code"/> <xsl:value-of select="local-name(.)"/>;
  </xsl:template>

<xsl:template match="bind" mode="actionbind" xml:space="default">
	<xsl:variable name="type" xml:space="preserve"><xsl:choose><xsl:when test="string(.)"><xsl:value-of select="."/> </xsl:when><xsl:otherwise>string </xsl:otherwise></xsl:choose></xsl:variable>
  [Bind] <xsl:value-of select="$type"/><xsl:text>    </xsl:text><xsl:value-of select="@__code"/>;
  </xsl:template>
    

  <xsl:template match="bind/*[@__name]" mode="bind" xml:space="preserve">
    //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
  [QViewBind(Name="<xsl:value-of select="@__code"/>")] <xsl:value-of select="@__name"/> <xsl:value-of select="local-name(.)"/>;
  </xsl:template>

  <xsl:template match="share/*" mode="bind" xml:space="preserve">
    //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
    <xsl:value-of select="@__code"/> <xsl:value-of select="local-name(.)"/> {
      get { return getShared&lt;<xsl:value-of select="@__code"/>&gt;("<xsl:value-of select="local-name(.)"/>"); }
      set { setShared("<xsl:value-of select="local-name(.)"/>",value); }
   }
  </xsl:template>

  <xsl:template match="data" mode="bind">
    <xsl:variable name="name">
      <xsl:choose>
        <xsl:when test="@__name"><xsl:value-of select="@__code"/></xsl:when>
        <xsl:otherwise>data</xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:variable name="type">
      <xsl:choose>
        <xsl:when test="@__name"><xsl:value-of select="@__name"/></xsl:when>
        <xsl:otherwise><xsl:value-of select="@__code"/></xsl:otherwise>
      </xsl:choose>
    </xsl:variable>

  [ViewData] <xsl:value-of select="$type"/><xsl:text xml:space="preserve"> </xsl:text><xsl:value-of select="$name"/> ;

  </xsl:template>

  <xsl:template match="@_line" mode="render"></xsl:template>
  <xsl:template match="@_file" mode="render"></xsl:template>
  <xsl:template match="_p_" mode="render"></xsl:template>

  <xsl:template match="mainrender" mode="render">
    //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
     this.RenderChild();
  </xsl:template>

<xsl:template match="requirements" mode="render">
    //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
     write("&lt;!-- __REQUIREMENTS__ --&gt;");
  </xsl:template>

<xsl:template match="require" mode="render">
    //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
     Require("<xsl:value-of select="@__code"/>");
  </xsl:template>
  <xsl:template match="head/link" mode="render">
    //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
    this.RenderLink("<xsl:value-of select="@__code"/>");
  </xsl:template>
  
  <xsl:template match="bind" mode="render"></xsl:template>

  
  

  <xsl:template match="root/data" mode="render"></xsl:template>
  <xsl:template match="root/share" mode="render"></xsl:template>
  <xsl:template match="root/using" mode="render"></xsl:template>
  <xsl:template match="root/inherit" mode="render"></xsl:template>
  <xsl:template match="root/resources" mode="render"></xsl:template>

  <xsl:template match="table/heads" mode="render">
    <xsl:variable name="e" select="."/>
    //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
      write("&lt;thead&gt;&lt;tr&gt;");
      <xsl:for-each select="@*[local-name()!='__id']">
        <xsl:variable name="a" select="."/>
        <xsl:choose>
          <xsl:when test="string($a)='1' and not (c:issys(local-name(.))=1)">
            //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
            write("&lt;th&gt;",<xsl:value-of select="c:tostr(local-name($a),$e/_p_[@k=local-name($a)],0)"/>,"&lt;/th&gt;");
          </xsl:when>
          <xsl:otherwise>
            //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
            write("&lt;th&gt;",<xsl:value-of select="c:tostr(.,$e/_p_[@k=local-name($a)],0)"/>,"&lt;/th&gt;");
          </xsl:otherwise>
        </xsl:choose>
      </xsl:for-each>
    <xsl:for-each select="*">
      //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
      write("&lt;th&gt;");
      <xsl:apply-templates select="." mode="render"/>
      //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
      write("&lt;/th&gt;");
    </xsl:for-each>
    //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
    write("&lt;/tr&gt;&lt;/thead&gt;");
  </xsl:template>

  <xsl:template match="table//cells" mode="render">
    <xsl:param name="tr">1</xsl:param>
    <xsl:variable name="e" select="."/>
    <xsl:if test="$tr=1">
      //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
    write("&lt;tr&gt;");
    </xsl:if>
    <xsl:for-each select="@*[local-name()!='__id']">
      <xsl:variable name="a" select="."/>
      <xsl:choose>
        <xsl:when test="string($a)='1' and not (c:issys(local-name(.))=1)">
          //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
          write("&lt;td&gt;",<xsl:value-of select="c:tostr(local-name($a),$e/_p_[@k=local-name($a)],0)"/>,"&lt;/td&gt;");
        </xsl:when>
        <xsl:otherwise>
          write("&lt;td&gt;",<xsl:value-of select="c:tostr(.,$e/_p_[@k=local-name($a)],0)"/>,"&lt;/td&gt;");
        </xsl:otherwise>
      </xsl:choose>
      
    </xsl:for-each>
    <xsl:for-each select="*">
      //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
      write("&lt;td&gt;");
      <xsl:apply-templates mode="render" select="."/>
      write("&lt;/td&gt;");
    </xsl:for-each>   
    <xsl:if test="$tr=1">
      write("&lt;/tr&gt;");
    </xsl:if>
  </xsl:template>

  <xsl:template match="table/@for" mode="render"/>
  <xsl:template match="table[@for]" mode="render">
    <xsl:variable name="var">
      <xsl:choose>
        <xsl:when test="@var">
          <xsl:value-of select="@var"/>
        </xsl:when>
        <xsl:otherwise>item</xsl:otherwise>
      </xsl:choose>
    </xsl:variable>

    <xsl:call-template name="opentag"/>
    
    <xsl:apply-templates select=".//heads" mode="render"/>
    //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
    write("&lt;tbody&gt;");
    foreach (var <xsl:value-of select="$var"/> in <xsl:value-of select="@for" /> ){
      write("&lt;tr&gt;");
      <xsl:apply-templates select=".//cells | .//td" mode="render">
        <xsl:with-param name="tr">0</xsl:with-param>
      </xsl:apply-templates>
      write("&lt;/tr&gt;");
    }
    write("&lt;/tbody&gt;");
    <xsl:call-template name="closetag"/>
  </xsl:template>

  <xsl:template name="opentag">
    <xsl:choose>
      <xsl:when test="not(text()) and not(@*[c:issys(local-name(.))=0]) and @__code">
        <xsl:value-of select="c:open_tag(.)"/>
        <xsl:value-of select="c:tag_value(@__code,./_p_[@k='__code'])"/>
      </xsl:when>
      <xsl:when test="@*[local-name()!='_line' and local-name()!='_file']">
        <xsl:value-of select="c:start_tag_head(.)"/>
        <xsl:apply-templates select="./@*" mode="render">
          <xsl:with-param name="e" select="."/>
        </xsl:apply-templates>
        <xsl:value-of select="c:end_tag_head(.)"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="c:open_tag(.)"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="closetag">
    <xsl:value-of select="c:end_tag(.)"/>
  </xsl:template>

  <xsl:template match="*" mode="render">
    <xsl:call-template name="opentag"/>
    <xsl:if test="text()">
      <xsl:value-of select="c:tag_value(./text(),./_p_[@k='_val_'])"/>
    </xsl:if>
    <xsl:apply-templates select="*" mode="render"/>
    <xsl:call-template name="closetag"/>
  </xsl:template>

  <xsl:template match="html" mode="render">
    //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
    write("&lt;!DOCTYPE html&gt;");
    <xsl:call-template name="opentag"/>
    <xsl:if test="text()">
      <xsl:value-of select="c:tag_value(./text(),./_p_[@k='_val_'])"/>
    </xsl:if>
    <xsl:apply-templates select="*" mode="render"/>
    <xsl:call-template name="closetag"/>
  </xsl:template>

  <xsl:template match="set" mode="render">
    <xsl:variable name="e" select="."/>
    <xsl:for-each select="@*">
      //src: <xsl:value-of select="$e/@_file"/> : <xsl:value-of select="$e/@_line"/>
      var <xsl:value-of select="local-name()"/> = <xsl:value-of select="."/>;
    </xsl:for-each>
  </xsl:template>

  <xsl:template match="capture" mode="render" xml:space="preserve">
    //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
    enterTemporaryOutput();
    <xsl:apply-templates select="*" mode="render"/>
    <xsl:value-of select="@__code"/> = getTemporaryOutput();
  </xsl:template>

  <xsl:template match="out" mode="render">
    
    <xsl:variable name="e" select="."/>
    <xsl:for-each select="@*[local-name(.)!='__id' and local-name(.)!='_line' and local-name(.)!='_file']" >
      <xsl:variable name="a" select="."/>
      //src: <xsl:value-of select="$e/@_file"/> : <xsl:value-of select="$e/@_line"/>  
      write(<xsl:value-of select="c:tostr(.,$e/_p_[@k=local-name($a)]  ,1)"/>);
    </xsl:for-each>
    <xsl:if test="text()">
      <xsl:value-of select="c:tag_value(./text(),$e/_p_[@k='_val_'])"/>
    </xsl:if>
  </xsl:template>

  <xsl:template match="capture/@__code" mode="bind">
    <xsl:variable name="this" select="."/>
    <xsl:if test="generate-id(.)=generate-id(//capture/@__code[.=$this])">
      string <xsl:value-of select="."/>;
    </xsl:if>
  </xsl:template>

  <xsl:template match="for" mode="render">
    <xsl:variable name="name">
      <xsl:choose>
        <xsl:when test="@__name"><xsl:value-of select="@__code"/></xsl:when>
        <xsl:otherwise>item</xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:variable name="collection">
      <xsl:choose>
        <xsl:when test="@__name"><xsl:value-of select="@__name"/></xsl:when>
        <xsl:otherwise><xsl:value-of select="@__code"/></xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
    foreach (var <xsl:value-of select="$name"/> in <xsl:value-of select="$collection"/> ){
        <xsl:apply-templates select="*" mode="render"/>
    }


  </xsl:template>
  
   <xsl:template match="for" mode="render">
    <xsl:variable name="name">
      <xsl:choose>
        <xsl:when test="@__name"><xsl:value-of select="@__code"/></xsl:when>
        <xsl:otherwise>item</xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:variable name="collection">
      <xsl:choose>
        <xsl:when test="@__name"><xsl:value-of select="@__name"/></xsl:when>
        <xsl:otherwise><xsl:value-of select="@__code"/></xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
     //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
    foreach (var <xsl:value-of select="$name"/> in <xsl:value-of select="$collection"/> ){
        <xsl:apply-templates select="*" mode="render"/>
    }


  </xsl:template>


  <xsl:template match="foreach" mode="render">
    //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
    foreach <xsl:value-of select="@__code"/>{
    <xsl:apply-templates select="*" mode="render"/>
    }


  </xsl:template>

  <xsl:template match="sub" mode="render">
    
    <xsl:variable name="e" select="."/>
    <xsl:variable name="viewname">
      <xsl:value-of select="c:liftup(@__code,2)"/>
    </xsl:variable>
    <xsl:choose>
      <xsl:when test="@*[c:issys(local-name(.))=0]">
        //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
        Subview(<xsl:value-of select="$viewname"/>,new{
          <xsl:for-each select="@*[c:issys(local-name(.))=0]">
            <xsl:variable name="a" select="."/>
            <xsl:value-of select="local-name(.)"/> = <xsl:value-of select="c:tostr($a,$e/_p_[@k=local-name($a)],1)"/>,
          </xsl:for-each>
        });
      </xsl:when>
      <xsl:otherwise>
        //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
        Subview(<xsl:value-of select="$viewname"/>);
      </xsl:otherwise>
    </xsl:choose>
    
  </xsl:template>

  <xsl:template match="ifrole | ifrolex" mode="render">
    <xsl:variable name="usr">
      <xsl:choose>
        <xsl:when test="@__name">"<xsl:value-of select="@__code"/>"</xsl:when>
        <xsl:otherwise>null</xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:variable name="role">
      <xsl:choose>
        <xsl:when test="@__name">"<xsl:value-of select="@__name"/>"</xsl:when>
        <xsl:otherwise>"<xsl:value-of select="@__code"/>"</xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:variable name="exact">
      <xsl:choose>
        <xsl:when test="local-name(.)='ifrole'">false</xsl:when>
        <xsl:otherwise>true</xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
    if (inrole(<xsl:value-of select="$role"/>,<xsl:value-of select="$usr"/>,<xsl:value-of select="$exact"/>)){
    <xsl:apply-templates select="*" mode="render"/>
    }
  </xsl:template>

  <xsl:template match="while" mode="render">
    //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
    while <xsl:value-of select="c:condition(@__code)"/>{
    <xsl:apply-templates select="*" mode="render"/>
    }
  </xsl:template>
  <xsl:template match="continue" mode="render">
    //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
    continue;
  </xsl:template>
  <xsl:template match="break" mode="render">
    //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
    break;
  </xsl:template>

  <xsl:template match="break" mode="render">
    //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
    return;
  </xsl:template>


  <xsl:template match="if" mode="render">
    //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
    if <xsl:value-of select="c:condition(@__code)"/>{
    <xsl:apply-templates select="*" mode="render"/>
    }
  </xsl:template>
  <xsl:template match="elif" mode="render">
    //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
    else if <xsl:value-of select="c:condition(@__code)"/>{
    <xsl:apply-templates select="*" mode="render"/>
    }
  </xsl:template>
  <xsl:template match="else" mode="render">
    //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
    else{
    <xsl:apply-templates select="*" mode="render"/>
    }
  </xsl:template>

  <xsl:template match="code" mode="render" xml:space="preserve">
    <xsl:value-of select="text()"/>
    <xsl:value-of select="@__code"/>
  </xsl:template>

  <xsl:template match="@*" mode="render" name="attr">
    <xsl:param name="standalone">0</xsl:param>
    <xsl:param name="e"/>
    <xsl:variable name="this" select="local-name(.)"/>
    <xsl:choose>
      <xsl:when test="$standalone = 1">
        <xsl:value-of select="c:tag_value(.,$e/_p_[@k=string($this)])"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="c:attr_value(.,$e/_p_[@k=$this])"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="input/@required | input/@checked | input/@pattern" mode="render">
    <xsl:param name="standalone">0</xsl:param>
    <xsl:param name="e"/>
    <xsl:variable name="this" select="local-name(.)"/>
    <xsl:choose>
      <xsl:when test="$standalone = 1">
        <xsl:value-of select="c:tag_value(.,$e/_p_[@k=string($this)])"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="c:bool_attr(.,$e/_p_[@k=$this])"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

    
  
  <xsl:template match="input/@onchange" mode="render">
      <xsl:choose>
          <xsl:when test="c:isliteral(.) and not(c:isescaped(.))"  >
            //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
              write(" <xsl:value-of select="local-name(.)"/>='<xsl:value-of select="."/>'");
          </xsl:when>
          <xsl:otherwise>
              <xsl:apply-templates mode="render2" select=".">
                  <xsl:with-param name="e" select="./ancestor::*"/>
              </xsl:apply-templates>
          </xsl:otherwise>
      </xsl:choose>
  </xsl:template>



  <xsl:template match="@*" mode="render2">
    <xsl:param name="standalone">0</xsl:param>
    <xsl:param name="e"/>
	<xsl:param name="rn"><xsl:value-of select="local-name(.)"/></xsl:param>
    <xsl:variable name="this" select="local-name(.)"/>
    <xsl:choose>
      <xsl:when test="$standalone = 1">
        <xsl:value-of select="c:tag_value(.,$e/_p_[@k=string($this)])"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="c:attr_value($rn,string(.),$e/_p_[@k=$this])"/>
      </xsl:otherwise>
    </xsl:choose>

  </xsl:template>

  <xsl:template match="input/@type" mode="render">
    <xsl:choose>
      <xsl:when test="string(.)='hidden' or string(.)='text' or string(.)='button' or string(.)='submit' or string(.)='password' or string(.)='checkbox' or string(.)='color' or string(.)='date' "  >
        //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
          write(" type='<xsl:value-of select="."/>'");
      </xsl:when>
      <xsl:otherwise>
        <xsl:apply-templates mode="render2" select=".">
          <xsl:with-param name="e" select="./ancestor::*"/>
        </xsl:apply-templates>
      </xsl:otherwise>
    </xsl:choose>
    
  </xsl:template>

  <xsl:template match="@class" mode="render">
    <xsl:choose>
      <xsl:when test="c:isliteral(.) and not(c:isescaped(.))"  >
        //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
        write(" <xsl:value-of select="local-name(.)"/>='<xsl:value-of select="."/>'");
      </xsl:when>
      <xsl:otherwise>
        <xsl:apply-templates mode="render2" select=".">
          <xsl:with-param name="e" select="./ancestor::*"/>
        </xsl:apply-templates>
      </xsl:otherwise>
    </xsl:choose>

  </xsl:template>

<xsl:template match="@__id" mode="render" priority="100">
	
    <xsl:choose>
      <xsl:when test="c:isliteral(.) and not(c:isescaped(.))"  >
        //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
        write(" id='<xsl:value-of select="."/>'");
		<xsl:if test="not(@class)">
			write(" class='<xsl:value-of select="."/>' ");
		</xsl:if>
      </xsl:when>
      <xsl:otherwise>
        <xsl:apply-templates mode="render2" select=".">
          <xsl:with-param name="e" select="./ancestor::*"/>
			<xsl:with-param name="rn">id</xsl:with-param>
        </xsl:apply-templates>
<xsl:if test="not(@class)">
			 <xsl:apply-templates mode="render2" select=".">
          <xsl:with-param name="e" select="./ancestor::*"/>
			<xsl:with-param name="rn">class</xsl:with-param>
        </xsl:apply-templates>
		</xsl:if>
      </xsl:otherwise>
    </xsl:choose>

  </xsl:template>

<xsl:template match="@__code" mode="render" priority="100"></xsl:template>


  
  <xsl:template match="br" mode="render">
    //src: <xsl:value-of select="@_file"/> : <xsl:value-of select="@_line"/>
    write("&lt;br/&gt;");
  </xsl:template>

</xsl:stylesheet>
