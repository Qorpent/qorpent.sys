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
using System.Threading;
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

        /// <summary>
        /// Создает или возвращает дочерний элемент
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static XElement EnsureSingleElement(this XElement element, string name) {
            var existed = element.Elements(name).ToArray();
            if (existed.Length == 1) {
                return existed[0];
            }
            if (existed.Length == 0) {
                var result = new XElement(name);
                element.Add(result);
                return result;
            }
            throw new Exception("multiple instance");
        }

	    public static XElement Interpolate(this XElement element,object context = null, bool nocopy = false, Action<XmlInterpolation> setup = null) {
	        var e = nocopy ? element : new XElement(element);
	        var xi = new XmlInterpolation();
	        if (null != setup) {
	            setup(xi);
	        }
	        return xi.Interpolate(e,context);
	    }

		/// <summary>
		/// Возвращает полный путь от корня
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public static string GetXPath(this XElement e){
			var result = "";
			var current = e;
			
			while (null != current && null!=current.Parent){
				if (!string.IsNullOrWhiteSpace(result)){
					result = "/" + result;
				}
				var count = current.ElementsBeforeSelf(current.Name).Count()+1;
				result = current.Name.LocalName + "[" + count + "]" +result;
				current = current.Parent;
			}
			result = "./" + result;
			return result;
		}

		/// <summary>
		/// Возвращает полный путь от корня
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public static string GetXPath(this XAttribute e){
			var elXPath = e.Parent.GetXPath();
			return elXPath + "/@" + e.Name.LocalName;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		public static XElement NoEvidenceCopy(this XElement element){
			var result = new XElement(element);
			foreach (var e in result.DescendantsAndSelf()){
				var _file = e.Attribute("_file");
				if(null!=_file)_file.Remove();
				var _line = e.Attribute("_line");
				if (null != _line) _line.Remove();
				var _dir = e.Attribute("_dir");
				if (null != _dir) _dir.Remove();
			}
			return result;
		}


        /// <summary>
        /// имя атрибута с уникаьным номером элемента по умолчанию
        /// </summary>
	    public const string ElementUidAttribute = "__UID";
	    private static readonly string[] Idatributes = new[] {"id", "code", "__id", "__code", "ID"};

	    /// <summary>
	    /// Возвращает true если код, имя, значение равны name или есть атрибут с именем name, приводимый к true
	    /// </summary>
	    /// <param name="x"></param>
	    /// <param name="name"></param>
	    /// <returns></returns>
	    public static bool HasSignificantAttribute(this XElement x,string name) {
            if (null != x.Attribute(name)) return true;
            if (x.GetCode() == name) return true;
            if (x.GetName() == name) return true;
            if (x.Value == name) return true;
            return false;
        }
		/// <summary>
		/// Специальная обработка для элементов вида item F1 F2 F3 ... с учетом BXL кодов и _file, _line
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public static IEnumerable<string> CollectFlags(this XElement e){
			var result = new List<string>();
			foreach (var a in e.Attributes()){
				var name = a.Name.LocalName;
				if (name == "_file" || name == "_line"||name=="__parent") continue;
				var val = a.Value;
				if(val=="1")result.Add(name);
				else if (name == "id" || name == "code" || name == "name") result.Add(val);
				else result.Add(name);
			}
			return result.Distinct().ToArray();
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
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool HasAttribute(this XElement e, string name) {
            if (null == e) return false;
            if (string.IsNullOrWhiteSpace(name)) return false;
            return null != e.Attribute(name);
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
		/// Резолюция атрибута Code
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public static string GetCode(this XElement e) {
			return CoreExtensions.Attr(e, "code");
		}
		/// <summary>
		/// Резолюция атрибута name
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public static string GetName(this XElement e)
		{
			return CoreExtensions.Attr(e, "name");
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
	    /// Присваивает уникальные целочисленные ID каждому элементу в DocOrder
	    /// </summary>
	    /// <param name="xml"></param>
	    /// <param name="attrname"></param>
	    /// <param name="copy"></param>
	    /// <returns></returns>
	    public static XElement GenerateUniqueIdsForElements(this XElement xml, string attrname = ElementUidAttribute, bool copy = false) {
            var result = xml;
            if(copy)result = new XElement(xml);
	        var id = 0;
            foreach (var e in result.DescendantsAndSelf()) {
                e.SetAttributeValue(attrname,id++);
            }
            return result;
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
		    return nameCandidate.Escape(EscapingType.XmlName);
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
		/// <param name="map"></param>
		/// <param name="excludes"> </param>
		public static T Apply<T>(this XElement element, T result = default(T), object map = null, params string[] excludes) {
			//if (typeof(T).FullName.StartsWith("System.Collections.Generic.IEnumerable")) {

			//}
			//else if (typeof(T) == typeof(Array)) {

			//}
			//else {
			if (null == result) {
				result = Activator.CreateInstance<T>();
			}
			foreach (var attribute in element.Attributes()) {
				if(null!=excludes && -1!=Array.IndexOf(excludes,attribute.Name.LocalName))continue;
				var name = attribute.Name.LocalName;
				if (null != map) {
					name = map.GetValue(name, name, ignoreNotFound:true);
				}
				result.SetValue(name, attribute.Value, true, true, true, true, true);
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
        /// <summary>
        ///     Проверяет, присутствует ли элемент с данным именем
        /// </summary>
        /// <param name="xElement">Исходный элемент</param>
        /// <param name="xName">Имя проверяемого элемента</param>
        /// <returns></returns>
        public static bool ContainsElement(this XElement xElement, string xName) {
            if (null == xElement) return false;

            return xElement.Element(xName).IsNotNull();
        }
        /// <summary>
        ///     Попробовать проверить тип JSON-элемента в XML-представлении, если таковой присутствует
        /// </summary>
        /// <param name="xElement">Исходный элемент</param>
        /// <returns></returns>
        public static string TryCheckJsonType(this XElement xElement) {
            return xElement.Attribute(JsonItem.JsonTypeAttributeName).TryGetValue();
        }

	    /// <summary>
	    ///     Вытаскивает значение атрибута элемента с переданным именем
	    /// </summary>
	    /// <param name="xElement">Базовый элемент</param>
	    /// <param name="elName">Имя элемента</param>
	    /// <param name="attrName">Имя атрибута</param>
	    /// <returns>Значение</returns>
	    public static string TryGetElAttrValue(this XElement xElement, XName elName, XName attrName) {
            var el = xElement.Element(elName);
            return el == null ? null : el.Attribute(attrName).TryGetValue();
        }
	    /// <summary>
        ///     Превращает любые имена элементов в нотацию типа «Word» («wORd» -> «Word»)
	    /// </summary>
	    /// <param name="xElement">Исходный элемент</param>
	    /// <param name="includeRoot">Включать корневой элемент</param>
	    /// <returns>Исходный элемент</returns>
	    public static XElement Capitalize(this XElement xElement, bool includeRoot = true) {
            foreach (var el in xElement.XPathSelectElements(includeRoot ? "//*" : "/" + xElement.Name + "/*")) {
                el.Name = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(el.Name.ToString());
            }

            return xElement;
        }
        /// <summary>
        ///     Превращает любые имена элементов в нотацию типа «WORD» («wORd» -> «WORD»)
        /// </summary>
        /// <param name="xElement">Исходный элемент</param>
        /// <param name="includeRoot">Включать корневой элемент</param>
        /// <returns>Исходный элемент</returns>
        public static XElement ElementsToUpperCase(this XElement xElement, bool includeRoot = true) {
            foreach (var el in xElement.XPathSelectElements(includeRoot ? "//*" : "/" + xElement.Name + "/*")) {
                el.Name = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(el.Name.ToString().ToUpper());
            }

            return xElement;
        }
        /// <summary>
        ///     Превращает любые имена элементов в нотацию типа «word» («wORd» -> «word»)
        /// </summary>
        /// <param name="xElement">Исходный элемент</param>
        /// <param name="includeRoot">Включать корневой элемент</param>
        /// <returns>Исходный элемент</returns>
        public static XElement ElementsToLowerCase(this XElement xElement, bool includeRoot = true) {
            foreach (var el in xElement.XPathSelectElements(includeRoot ? "//*" : "/" + xElement.Name + "/*")) {
                el.Name = el.Name.ToString().ToLower();
            }

            return xElement;
        }
        /// <summary>
        /// Создает элемент с телом и атрибутами
        /// </summary>
        /// <param name="tagname"></param>
        /// <param name="text"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
	    public static XElement CreateElement(string tagname, string text = "", object attributes = null) {
            var element = new XElement(tagname);
            if (!String.IsNullOrWhiteSpace(text)) {
                element.Value = text;
            }
            if (null != attributes) {
                var dict = attributes.ToDict();
                foreach (var p in dict) {
                    element.SetAttributeValue(p.Key.Escape(EscapingType.XmlName), p.Value);
                }
            }
            return element;
        }

	    /// <summary>
	    /// Добавляет дочерний элемент и возвращает его
	    /// </summary>
	    /// <param name="parent"></param>
	    /// <param name="tagname"></param>
	    /// <param name="text"></param>
	    /// <param name="attributes"></param>
	    /// <returns></returns>
	    public static XElement AddElement(this XElement parent, string tagname, string text = null, object attributes= null ) {
	        var element = CreateElement(tagname, text, attributes);
	        parent.Add(element);
	        return element;
	    }
	}
}