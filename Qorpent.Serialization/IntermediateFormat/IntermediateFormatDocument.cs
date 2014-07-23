using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Qorpent.Config;
using Qorpent.Model;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.IntermediateFormat {
	/// <summary>
	/// Переносимая структура в формате IntermediateDocument
	/// </summary>
	public class IntermediateFormatDocument: ConfigBase,IWithCode,IWithName {
		
		/// <summary>
		/// Ссылка на запрос - контейнер
		/// </summary>
		public IntermediateFormatQuery Query{
			get { return Get<IntermediateFormatQuery>("query"); }
			set { Set("query",value); }
		}

		/// <summary>
		/// Ссылка на запрос - контейнер
		/// </summary>
		public IntermediateFormatLayer Layer
		{
			get { return Get<IntermediateFormatLayer>("layer"); }
			set { Set("layer", value); }
		}
		/// <summary>
		/// Прототип, класс документа
		/// </summary>
		public string Prototype{
			get { return Get<string>("prototype"); }
			set { Set("prototype", value); }
		}

		/// <summary>
		/// Прототип, класс документа
		/// </summary>
		public string Code
		{
			get { return Get<string>("code"); }
			set { Set("code", value); }
		}

		/// <summary>
		/// Прототип, класс документа
		/// </summary>
		public string Name
		{
			get { return Get<string>("name"); }
			set { Set("name", value); }
		}

		/// <summary>
		/// Записывает ZIF документ в поток в соответствии с запросом
		/// </summary>
		/// <param name="output">Поток вывода</param>
		/// <param name="query">Запрос ZIF</param>
		public virtual void Write(Stream output, IntermediateFormatQuery query = null){
			query = query ?? Query;
			if (null == query){
				throw new IntermediateFormatException("query was not given for document output",null);
			}
			var xml = ToXml(query);
			if (query.Format == IntermediateFormatOutputType.Xml){
				using (var sw = new StreamWriter(output, Encoding.UTF8)){
					if (string.IsNullOrWhiteSpace(query.Xslt)){
						sw.Write(xml.ToString());
					}
					else{
						WriteoutWithTransform(query, xml, sw);
					}

					sw.Flush();
					
				}
			}else if (query.Format == IntermediateFormatOutputType.Json){
				throw new NotSupportedException("JSON is not supportedas output format  for now");
			}else if (query.Format == IntermediateFormatOutputType.Bxl){
				throw new NotSupportedException("BXL is not supported as output format for now");
			}
		}

		private void WriteoutWithTransform(IntermediateFormatQuery query, XElement xml, StreamWriter sw){
			var transform = new XslCompiledTransform();
			var xsltfile = ResolveRealXsltPath(query.Xslt);
			transform.Load(xsltfile, XsltSettings.TrustedXslt, new XmlUrlResolver());
			var transformargs = new XsltArgumentList();
			transformargs.AddExtensionObject("ext:doc", this);
			transformargs.AddExtensionObject("ext:query", query);
			foreach (var pair in query){
				transformargs.AddParam(pair.Key, "", pair.Value.ToStr());
			}
			transform.Transform(xml.CreateReader(), transformargs, sw);
		}

		private string ResolveRealXsltPath(string xsltpath){
			if (!xsltpath.EndsWith(".xslt")){
				xsltpath += ".xslt";
			}
			return xsltpath;
		}

		/// <summary>
		/// Формирует XML - представление документа
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public virtual  XElement ToXml(IntermediateFormatQuery query=null){
			query = query ?? Query;
			var xser = new XmlSerializer();
			var result = new XElement(IntermediateFormatSyntax.DocumentElement);
			result.SetAttributeValue("code",Code);
			result.SetAttributeValue("name",Name);
			result.SetAttributeValue("prototype",Prototype);
			result.SetAttributeValue("layer",Layer);
			foreach (var pair in options){
				var key = pair.Key;
				
				if(IsSystemDefined(key))continue;
				var value = pair.Value;
				if (value == null || (value is string && string.IsNullOrWhiteSpace(value as string))) continue;
				if (value.GetType().IsValueType || value is string){
					result.SetAttributeValue(key, value);
				}
				else{
					if (value.GetType().IsDefined(typeof(SerializeAttribute), true) || value.GetType().Name.Contains("AnonymousType")){
						var xml = XElement.Parse(xser.Serialize("body", value)).Elements().First();
						var item = new XElement(IntermediateFormatSyntax.DocumentItemElement);
						item.SetAttributeValue("code",key);
						item.SetAttributeValue("type",value.GetType().Name);
						
						item.Add(xml);
						result.Add(item);
					}
				}
				
			}
			foreach (var document in Children){
				result.Add(document.ToXml(query));
			}
			return result;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		protected virtual bool IsSystemDefined(string key){
			return key=="code"||key=="name"||key=="prototype"||key=="layer"||key=="query";
		}

		/// <summary>
		/// 
		/// </summary>
		private readonly IList<IntermediateFormatDocument> _children =new List<IntermediateFormatDocument>();
		/// <summary>
		/// 
		/// </summary>
		public IntermediateFormatDocument[] Children{
			get { return _children.ToArray(); }
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="document"></param>
		public void AddChildDocument(IntermediateFormatDocument document){
			document.SetParent(this);
			_children.Add(document);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="document"></param>
		public void RemoveChildDocument(IntermediateFormatDocument document){
			_children.Remove(document);
		}
	}
}