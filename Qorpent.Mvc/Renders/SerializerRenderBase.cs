﻿#region LICENSE
// Copyright 2007-2013 Qorpent Team - http://github.com/Qorpent
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// PROJECT ORIGIN: Qorpent.Mvc/SerializerRenderBase.cs
#endregion
using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;
using Qorpent.IO;
using Qorpent.IoC;
using Qorpent.Serialization;

namespace Qorpent.Mvc.Renders {
	/// <summary>
	/// 	Base class for usual by-serialize renders
	/// </summary>
	public abstract class SerializerRenderBase : RenderBase {
	    private ISerializer _serializer;
	    private ISerializer _xmlserializer;

        /// <summary>
        /// Ðàñøèðåíèÿ XSLT äëÿ ñåðâåðíûõ XSLT ïðåîáðàçîâàíèé
        /// </summary>
        [Inject]
        public ISerializerXsltExtension[] XsltExtensions { get; set; }

        /// <summary>
        /// Êîìïîíåíò ïîèñêà ôàéëîâ äëÿ ðåçîëþöèè XSLT
        /// </summary>
        [Inject]
        public IFileNameResolver FileNameResolver { get; set; }


	    /// <summary>
		/// 	return format to be used
		/// </summary>
		/// <returns> </returns>
		protected abstract SerializationFormat GetFormat();

		/// <summary>
		/// 	output mime type
		/// </summary>
		/// <returns> </returns>
		protected abstract string GetContentType();

		/// <summary>
		/// 	Renders given context
		/// </summary>
		/// <param name="context"> </param>
		public override void Render(IMvcContext context) {
            context.ContentType = GetContentType();
            if (null != context.ActionResult) {
                if (context.ActionResult is string) {
                    if (IsNativeString(context.ActionResult as string)) {
                        context.Output.Write(context.ActionResult);
                        return;
                    }
                }
            }

			
			if (null == context.ActionResult) {
				context.Output.Write("null");
			}
			else {
			    //Q-11 äîáàâëåíà ïîääåðæêà ïðåä-ïîäãîòîâêè è ôèëüòðàöèè îáúåêòà ïðè ðåíäåðèíãå
			    var objectToRender = PrepareRenderObject(context);
			    /////////////////////////////////////////////////////////////////
			    SendOutput(context, objectToRender);
			}
		}

        /// <summary>
        ///     Проверим, что строке не требуется серилизация
        /// </summary>
        /// <param name="actionResult"></param>
        /// <returns></returns>
	    protected virtual bool IsNativeString(string actionResult) {
	        return false;
	    }

	    /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="objectToRender"></param>
	    protected virtual void SendOutput(IMvcContext context, object objectToRender) {
	        GetMainSerializer().Serialize("result", objectToRender, context.Output);
	    }

	    private object PrepareRenderObject(IMvcContext context) {
            //Q-14 common xpath/xslt support
	        var xpath = context.Get("__xpath");
	        var xslt = context.Get("__xslt");
	        var objectToRender = context.ActionResult;
	        if (!string.IsNullOrWhiteSpace(xpath) || !string.IsNullOrWhiteSpace(xslt)) {
	            objectToRender = TransformResult(context,objectToRender, xpath, xslt);
	        }
	        return objectToRender;
	    }

	    private object TransformResult(IMvcContext context, object objectToRender, string xpath, string xslt) {
            //ñíà÷àëà ïðèâîäèì èñõîäíûé îáúåêò ê IXPathNavigable (÷òî íóæíî êàê äëÿ xpath, òàê è äëÿ xslt)
	        var xnav = ConvertToXmlReader(objectToRender);
            // åñëè ó íàñ åñòü ôèëüòðóþùåå óñëîâèå (çàïðîñ), òî ñíà÷àëà âûïîëíÿåì åãî è ïîëó÷àåì íîâûé
            // äîêóìåíò
            if (!string.IsNullOrWhiteSpace(xpath)) {
                xnav = FilterByXPath(xnav, xpath);
            }
            if (!string.IsNullOrWhiteSpace(xslt)) {
                return TransformResultWithXslt(context, xslt, xnav);    
            }
            return XElement.Load(xnav.CreateNavigator().ReadSubtree());
	        
	    }

	    private object TransformResultWithXslt(IMvcContext context, string xslt, IXPathNavigable xnav) {
	        var resolvedxslt = FileNameResolver.Resolve(
	            new FileSearchQuery {
	                ProbePaths = new[] {"~/styles", "~/usr/xslt"},
	                ProbeFiles = new[] {xslt + ".xslt", xslt},
	                ExistedOnly = true,
	                PathType = FileSearchResultType.FullPath,
					UseCache =false,
	            });
	        if (null == resolvedxslt) {
	            throw new Exception("cannot find xslt with code " + xslt);
	        }
	        var transform = new XslCompiledTransform();
	        transform.Load(resolvedxslt, XsltSettings.TrustedXslt, new XmlUrlResolver());
	        var sw = new StringWriter();
	        var xw = XmlWriter.Create(sw);
	        var args = new XsltArgumentList();
	        args.AddExtensionObject("qorpent.mvc.context", context);
	        foreach (var extension in XsltExtensions) {
	            args.AddExtensionObject(extension.GetNamespace(), extension);
	        }
	        transform.Transform(xnav, args, xw);
	        xw.Flush();
	        xw.Close();
	        return XElement.Parse(sw.ToString());
	    }

	    private IXPathNavigable FilterByXPath(IXPathNavigable xnav, string xpath) {
	        var sw = new StringWriter();
	        var xw = XmlWriter.Create(sw,new XmlWriterSettings{OmitXmlDeclaration = true});
            xw.WriteStartElement("result");
	        var queryResult = xnav.CreateNavigator().Select(xpath);
	        while (queryResult.MoveNext()) {
	            queryResult.Current.WriteSubtree(xw);
	        }
            xw.WriteEndElement();
            xw.Flush();
            xw.Close();
            return new XPathDocument(new StringReader(sw.ToString()));
	    }

	    private IXPathNavigable ConvertToXmlReader(object obj) {
	        if (obj is XElement) return (obj as XElement).CreateNavigator();
	        if (obj is string) return new XPathDocument(new StringReader(obj as string));
	        var stringwriter = new StringWriter();
	        GetXmlSerializer().Serialize("result", obj, stringwriter);
            return new XPathDocument(new StringReader(stringwriter.ToString()));
	    }

	    /// <summary>
		/// 	Renders error, occured in given context
		/// </summary>
		/// <param name="error"> </param>
		/// <param name="context"> </param>
		public override void RenderError(Exception error, IMvcContext context) {
			GetMainSerializer().Serialize("error", error, context.Output);
		}

        /// <summary>
        ///  Èíñïîëüçóåòñÿ äëÿ ïðåäâàðèòåëüíîé òðàíñôîðìàöèè â XML äëÿ âûïîëíåíèÿ òðåáîâàíèé Q-10
        /// </summary>
        /// <returns></returns>
        protected ISerializer GetXmlSerializer() {
            return _xmlserializer = _xmlserializer ?? ResolveService<ISerializerFactory>().GetSerializer(SerializationFormat.Xml);
        }
      

		/// <summary>
		/// </summary>
		/// <returns> </returns>
		protected ISerializer GetMainSerializer() {
			return _serializer = _serializer ?? ResolveService<ISerializerFactory>().GetSerializer(GetFormat());
		}
	}
}