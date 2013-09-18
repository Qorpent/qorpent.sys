#region LICENSE
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
// PROJECT ORIGIN: Qorpent.Utils/XmlExtensions.cs
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Qorpent.Applications;
using Qorpent.Bxl;
using Qorpent.Json;
using Qorpent.Serialization;

namespace Qorpent.Utils.Extensions {
	/// <summary>
	/// 	Xml related extensions of common XNodes
	/// </summary>
	public static class XmlExtensions {
		private static readonly string[] Idatributes = new[] {"id", "code", "__id", "__code", "ID"};

		/// <summary>
		/// 	Возвращает только собственное значение элемента (конкатенация текстовых элементов через пробел)
		/// </summary>
		/// <param name="xElement"> </param>
		/// <returns> </returns>
		public static string SelfValue(this XElement xElement) {
			if (xElement == null) {
				throw new ArgumentNullException("xElement");
			}
			return xElement.Nodes().OfType<XText>().Select(x => x.Value).ConcatString(" ");
		}

        /// <summary>
        /// Формирует сериализуемый в  JSON XML-массив в соответствии с внутренними соглашениями по коду
        /// </summary>
        /// <param name="items"></param>
        /// <param name="itemname"></param>
        /// <returns></returns>
        public static XElement ToMvcXmlArray(this IEnumerable<XElement> items, string itemname ) {
            return new XElement("result", new XElement(itemname, new XAttribute(JsonItem.JsonTypeAttributeName, "array"), items));
        }
		/// <summary>
		/// Производит поиск атрибутов по имени и/или вхождению строки
		/// </summary>
		/// <param name="e"></param>
		/// <param name="attributename"></param>
		/// <param name="contains"></param>
		/// <param name="skipself"></param>
		/// <param name="selfonly"></param>
		/// <returns></returns>
		public static bool HasAttributes(this XElement e, string attributename = null, string contains = null,
		                                 bool skipself = false, bool selfonly = false) {
			if (!skipself) {
				foreach (var a in e.Attributes()) {
					if (null == attributename || a.Name.LocalName == attributename) {
						if (null == contains || a.Value.Contains(contains)) {
							return true;
						}
					}
				}
			}
			return !selfonly && e.Elements().Any(c => HasAttributes(c, attributename, contains));
		}

		/// <summary>
		/// 	returns qorpent/bxl bound descriptor of XElement
		/// </summary>
		/// <param name="x"> </param>
		/// <param name="explicitname"></param>
		/// <returns> </returns>
		public static XmlElementDescriptor Describe(this XElement x,bool explicitname = false) {
			return new XmlElementDescriptor(x,explicitname);
		}
		/// <summary>
		/// Получает значение атрибута id, code или само значение элемента
		/// </summary>
		/// <param name="xml"></param>
		/// <param name="def"></param>
		/// <returns></returns>
		public static string IdCodeOrValue(this XElement xml, string def = "")
		{
			var a = xml.Attribute("id");
			if (null != a) return a.Value;
			a = xml.Attribute("code");
			if (null != a) return a.Value;
			return def;
		}

		/// <summary>
		/// Читает либо элемент либо атрибут с именем
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="xml"></param>
		/// <param name="name"></param>
		/// <param name="def"></param>
		/// <returns></returns>
		public static T ElementOrAttr<T>(this XElement xml, string name, T def = default(T))
		{
			if (xml == null) return def;
			var e = xml.Element(name);
			if (e != null) return e.Value.To<T>();
			var val = xml.Attr(name);
			if(val.IsNotEmpty()) return val.To<T>();
			return def;
		}

		
		/// <summary>
		/// 	resolves firstly matched attribute or returns empty string
		/// </summary>
		/// <param name="e"> </param>
		/// <param name="candidates"> </param>
		/// <returns> </returns>
		public static string ChooseAttr(this XElement e, params string[] candidates) {
			foreach (var candidate in candidates) {
				var a = e.Attribute(candidate);
				if (null != a) {
					return a.Value;
				}
			}
			return String.Empty;
		}

		/// <summary>
		/// 	Returns not-null string Value of elemnt's attribute (null-safe, existence-ignorance)
		/// </summary>
		/// <param name="sourceElement"> Element from which attribute requested </param>
		/// <param name="name"> name of requested attribute (can be string) </param>
		/// <param name="defaultvalue"> default Value if not attribute existed </param>
		/// <returns> string representation of attribute or empty string </returns>
		public static string Attr(this XElement sourceElement, XName name, string defaultvalue = "") {
			if (null == sourceElement) {
				return defaultvalue;
			}
			var attr = sourceElement.Attribute(name);
			return null == attr ? defaultvalue : attr.Value;
		}
		/// <summary>
		/// Резолюция атрибута Code
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public static string GetCode(this XElement e) {
			return Attr(e, "code");
		}
		/// <summary>
		/// Резолюция атрибута name
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public static string GetName(this XElement e)
		{
			return Attr(e, "name");
		}

		/// <summary>
		/// 	Resolves default Qorpent ID attribute for element in following priority - id, code, __id, __code, ID
		/// </summary>
		/// <param name="sourceElement"> </param>
		/// <returns> </returns>
		public static string Id(this XElement sourceElement) {
			if (null == sourceElement) {
				return String.Empty;
			}
			var result = Idatributes
				.Select(idatribute => sourceElement.Attr(idatribute))
				.FirstOrDefault(id => id.IsNotEmpty());
			return result ?? String.Empty;
		}

		/// <summary>
		/// 	Returns not-null string Value of attribute, searching it up to first occurance start from given element
		/// </summary>
		/// <param name="sourceElement"> Element from which attribute requested </param>
		/// <param name="name"> name of requested attribute (can be string) </param>
		/// <returns> string representation of attribute or empty string </returns>
		public static string ResolveAttr(this XElement sourceElement, XName name) {
			if (null == sourceElement) {
				return String.Empty;
			}
			var attr = sourceElement.Attribute(name);
			return null != attr ? attr.Value : sourceElement.Parent.ResolveAttr(name);
		}

		/// <summary>
		/// 	Replaces all non-match XName symbols in given string with special substitutes
		/// </summary>
		/// <param name="nameCandidate"> </param>
		/// <returns> </returns>
		public static string ConvertToXNameCompatibleString(this string nameCandidate)
		{
		    return nameCandidate.EscapeXmlName();
		}

		/// <summary>
		/// 	creates and returns an object with applying attributes to it from XML (in full safe mode)
		/// 	(if type is ValueType Value or attribute will be converted) generic version
		/// </summary>
		/// <param name="element"> source XML element </param>
		/// <param name="attributeName"> attribute to use (for Value types) if empty - Object will be used </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static T Deserialize<T>(this XElement element, string attributeName = "") {
			return (T) Deserialize(element, typeof (T), attributeName);
		}

		/// <summary>
		/// 	creates and returns an object with applying attributes to it from XML (in full safe mode)
		/// 	(if type is ValueType Value or attribute will be converted)
		/// </summary>
		/// <param name="element"> source XML element </param>
		/// <param name="type"> target type </param>
		/// <param name="attributeName"> attribute to use (for Value types) if empty - Object will be used </param>
		/// <returns> </returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static object Deserialize(this XElement element, Type type, string attributeName) {
			if (element == null) {
				throw new ArgumentNullException("element");
			}
			if (type == null) {
				throw new ArgumentNullException("type");
			}
			if (type.IsValueType) {
				if (attributeName.IsEmpty()) {
					return element.Value.ToTargetType(type);
				}
				var xAttribute = element.Attribute(attributeName);
				return xAttribute != null ? xAttribute.Value.ToTargetType(type) : Activator.CreateInstance(type);
			}
			var result = Activator.CreateInstance(type);
			Apply(element, result);
			return result;
		}

		/// <summary>
		/// 	Applys element's atributes to target object
		/// </summary>
		/// <param name="element"> </param>
		/// <param name="result"> </param>
		/// <param name="excludes"> </param>
		public static T Apply<T>(this XElement element, T result = default(T), params string[] excludes) {
			//if (typeof(T).FullName.StartsWith("System.Collections.Generic.IEnumerable")) {

			//}
			//else if (typeof(T) == typeof(Array)) {

			//}
			//else {
			foreach (var attribute in element.Attributes()) {
				if(null!=excludes && -1!=Array.IndexOf(excludes,attribute.Name.LocalName))continue;

				result.SetValue(attribute.Name.LocalName, attribute.Value, true, true, true, true, true);
			}
			if (result is ICustomXmlApplyer) {
				((ICustomXmlApplyer) result).Apply(element);
			}
			//}


			return result;
		}

		/// <summary>
		/// 	converts given object to xml due it's type - XElement, XmlReader, TextReader, Stream, string are supported
		/// </summary>
		/// <param name="xmlsource"> The xmlsource. </param>
		/// <param name="options"> </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public static XElement GetXmlFromAny(object xmlsource, BxlParserOptions options = BxlParserOptions.None) {
			if (null == xmlsource) {
				throw new ArgumentNullException("xmlsource");
			}
			var xelement = xmlsource as XElement;
			if (null != xelement) {
				return xelement;
			}
			var str = xmlsource as string;
			if (null != str) {
				if (str.Contains("<") && str.Contains(">")) {
					//it's xml string
					return XElement.Parse(str);
				}
				else if (str.Contains("\r") || str.Contains("\n")) //bxl like mark)
				{
					return Application.Current.Container.Get<IBxlParser>().Parse(str, "main", options);
				}
					//it's filename/url
				else if (IsBxlFileName(str)) {
					return Application.Current.Container.Get<IBxlParser>().Parse(File.ReadAllText(str), str);
				}
				else {
					return XElement.Load(str);
				}
			}
			var xmlreader = xmlsource as XmlReader;
			if (null != xmlreader) {
				return XElement.Load(xmlreader);
			}
			var textreader = xmlsource as TextReader;
			if (null != textreader) {
				return XElement.Load(textreader);
			}
			var stream = xmlsource as Stream;
#if SQL2008
			if(null!=stream) {
				using (var sr = new StreamReader(stream)) {
					return XElement.Load(sr);
				}
			}
#else
			if (null != stream) {
				return XElement.Load(stream);
			}
#endif
			var uri = xmlsource as Uri;
			if (null != uri) {
				return XElement.Load(uri.ToString());
			}
			var olddoc = xmlsource as XmlDocument;
			if (null != olddoc) {
				var sw = new StringWriter();
				var xw = XmlWriter.Create(sw);
				olddoc.ChildNodes[0].WriteTo(xw);
				xw.Flush();
				return XElement.Parse(sw.ToString());
			}
			var nav = xmlsource as XPathNavigator;
			if (null != nav) {
				var sw = new StringWriter();
				var xw = XmlWriter.Create(sw);
				nav.WriteSubtree(xw);
				xw.Flush();
				return XElement.Parse(sw.ToString());
			}
			throw new QorpentException("xmlsource type " + xmlsource.GetType().FullName +
			                           " is not supported to convert to XElement");
		}

		/// <summary>
		/// 	Determines whether [is BXL file name] [the specified filename].
		/// </summary>
		/// <param name="filename"> The filename. </param>
		/// <returns> <c>true</c> if [is BXL file name] [the specified filename]; otherwise, <c>false</c> . </returns>
		/// <remarks>
		/// </remarks>
		public static bool IsBxlFileName(string filename) {
			var ext = Path.GetExtension(filename);
			return ext == ".bxl" || ext == ".hql" || ext == ".tbxl";
		}

		/// <summary>
		/// Берет содержимое дочернего элемента если есть
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="elementName"></param>
		/// <returns></returns>
		public static string GetTextElement(this XElement nav, string elementName)
		{
			return GetTextElement(nav, elementName, "");
		}

		/// <summary>
		/// Берет содержимое дочернего элемента если есть
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="elementName"></param>
		/// <param name="def"> </param>
		/// <returns></returns>
		public static string GetTextElement(this XElement nav, string elementName, string def)
		{
			var e = nav.Element(elementName);
			if (null == e) return def;
			return e.Value;
		}


		#region Nested type: ICustomXmlApplyer

		/// <summary>
		/// </summary>
		public interface ICustomXmlApplyer {
			/// <summary>
			/// </summary>
			/// <param name="element"> </param>
			void Apply(XElement element);
		}

		#endregion

		#region Nested type: XmlElementDescriptor

		/// <summary>
		/// 	describes main qorpent attributes
		/// </summary>
		public class XmlElementDescriptor {
			/// <summary>
			/// 	creates descriptor for element
			/// </summary>
			/// <param name="element"> </param>
			/// <param name="explicitname"></param>
			public XmlElementDescriptor(XElement element,bool explicitname = false) {
				Id = element.ChooseAttr("__id", "id", "__code", "code");
				Code = element.ChooseAttr("__code", "code", "__id", "id");
				Name = element.ChooseAttr("__name", "name");
				if (!explicitname) {
					if (Name.IsEmpty()) {
						Name = element.Value;
					}
					if (Name.IsEmpty()) {
						Name = Code;
					}
				}
				File = element.ChooseAttr("_file", "__file");
				Line = element.ChooseAttr("_line", "__line").ToInt();
				Column = element.ChooseAttr("_col", "__col").ToInt();
				Value = element.SelfValue();
			}

			/// <summary>
			/// 	Собственное значение элемента
			/// </summary>
			public string Value { get; set; }

			/// <summary>
			/// 	returns lexical ing
			/// </summary>
			/// <returns> </returns>
			public string ToWhereString() {
				return String.Format(" at {0}({1}:{2})", File, Line, Column);
			}

			/// <summary>
			/// 	Code of element
			/// </summary>
			public readonly string Code;

			/// <summary>
			/// 	Column of element
			/// </summary>
			public readonly int Column;

			/// <summary>
			/// 	File of element
			/// </summary>
			public readonly string File;

			/// <summary>
			/// 	Id of element
			/// </summary>
			public readonly string Id;

			/// <summary>
			/// 	Line of element
			/// </summary>
			public readonly int Line;

			/// <summary>
			/// 	Name of element
			/// </summary>
			public readonly string Name;
		}

		#endregion
        /// <summary>
        ///     Пытается получить значение текущего элемента, если он не null
        /// </summary>
        /// <param name="xElement">Элемент</param>
        /// <returns>XElement.value OR null</returns>
	    public static string TryGetValue(this XElement xElement) {
	        return xElement != null ? xElement.Value : null;
	    }
        /// <summary>
        ///     Пытается получить значение текущего атрибута, если он не null
        /// </summary>
        /// <param name="xAttribute">Аттрибут</param>
        /// <returns>XElement.value OR null</returns>
        public static string TryGetValue(this XAttribute xAttribute) {
            return xAttribute != null ? xAttribute.Value : null;
        }
        /// <summary>
        ///     Проверяет элемент на NULL
        /// </summary>
        /// <param name="xElement">Элемент</param>
        /// <returns>True, если не NULL, иначе - False</returns>
        public static bool IsNotNull(this XElement xElement) {
            return xElement != null;
        }
        /// <summary>
        ///     Проверяет элемент на НЕ NULL
        /// </summary>
        /// <param name="xElement">Элемент</param>
        /// <returns>True, если NULL, иначе - False</returns>
        public static bool IsNull(this XElement xElement) {
            return !xElement.IsNotNull();
        }
	}
}