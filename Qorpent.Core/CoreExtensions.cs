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
// PROJECT ORIGIN: Qorpent.Utils/ConvertExtensions.cs
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace Qorpent.Utils.Extensions {
	///<summary>
	///	Contains utility functoins for safe and lightweight type and str->type conversion
	///</summary>
	public static class CoreExtensions {
		/// <summary>
		/// 	Null-safe (in/out) conversion to string of any object
		/// </summary>
		/// <param name="x"> </param>
		/// <param name="dateformat"></param>
		/// <param name="decimalformat"></param>
		/// <returns> </returns>
		public static string ToStr(this object x, string dateformat = null, string decimalformat = null){
			if (x == null) return String.Empty;
			if (null != dateformat && x is DateTime) return ((DateTime) x).ToString(dateformat);
			if (null != decimalformat && x is decimal) return ((decimal) x).ToString(decimalformat);
			return x.ToString();
		}
		/// <summary>
		/// Форматирует MD5Hash в строку из цифр с игнорированием ноля
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string MD5BasedDigitHash(this string text){
			var data = Encoding.UTF8.GetBytes(text);
			return Md5BasedDigitHash(data);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static string Md5BasedDigitHash(byte[] data){
			var hash = MD5.Create().ComputeHash(data);
			var result = new StringBuilder();
			var fst = BitConverter.ToUInt64(hash, 0);
			var sec = BitConverter.ToUInt64(hash, 8);
			result.Append(fst);
			result.Append(sec);
			return result.ToString();
		}

		/// <summary>
		/// 	test if enumerable is not null and contains not null elements
		/// </summary>
		/// <param name="e"> </param>
		/// <returns> </returns>
		public static bool IsNotEmpty(this IEnumerable e)
		{
			return !IsEmptyCollection(e);
		}

		/// <summary>
		/// 	test if enumerable is null or no elements or all elements are nulls
		/// </summary>
		/// <param name="e"> </param>
		/// <returns> </returns>
		public static bool IsEmptyCollection(this IEnumerable e)
		{
			return null == e || !e.OfType<object>().Any();
		}
		/// <summary>
		/// Checks that value is in range
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="testValue"></param>
		/// <param name="minValue"></param>
		/// <param name="maxValue"></param>
		/// <returns></returns>
		public static bool Between<TValue>(this TValue testValue, TValue minValue, TValue maxValue)
		   where TValue : struct
		{
			decimal it = testValue.ToDecimal();
			decimal min = minValue.ToDecimal();
			decimal max = maxValue.ToDecimal();
			return it >= min && it <= max;
		}

		/// <summary>
		/// 	Null-safe (in) conversion to bool of any object
		/// 	(effective boolean feature)
		/// 	1. nulls => false
		/// 	2. bools => self
		/// 	3. strings :
		/// 	3.a empty,ws => false
		/// 	3.b in "0","False" => false
		/// 	3.c all other => true
		/// 	4. enumerables :
		/// 	4.a without not-null items => false
		/// 	4.b with any not-null item => true
		/// 	5. IConvertible - by IConvertible.ToBoolean
		/// 	6. all other reference types => true (not null behavior)
		/// 	7. DateTime :
		/// 	7.а G Qorpent.Const.Begin => true
		/// 	7.b LE Qorpent.Const.Begin => false
		/// 	8. all other - Convert.ToBoolean
		/// </summary>
		/// <param name="x"> </param>
		/// <returns> </returns>
		public static bool ToBool(this object x) {
			if (null == x) {
				return false;
			}
			if (x is bool) {
				return (bool) x;
			}
			var s = x as string;
			if (s != null) {
				if (String.IsNullOrWhiteSpace(s)) {
					return false;
				}
				if ("0" == s) {
					return false; //usual "False" alternative
				}
				return "FALSE" != s.ToUpperInvariant();
			}
			var enumerable = x as IEnumerable;
			if (enumerable != null) {
				return (enumerable).OfType<object>().Any(item => null != item);
			}
			var convertible = x as IConvertible;
			if (convertible != null) {
				return (convertible).ToBoolean(CultureInfo.InvariantCulture);
			}
			return !x.GetType().IsValueType || Convert.ToBoolean(x);
		}

		/// <summary>
		/// 	Generic overload of type conversion (short) - unsafe (re-throws exceptions)
		/// </summary>
		/// <param name="x"> </param>
		/// <typeparam name="T"> </typeparam>
		/// <returns> </returns>
		public static T To<T>(this object x) {
			return To<T>(x, false);
		}

		/// <summary>
		/// 	Full generic version of safe type conversion
		/// </summary>
		/// <param name="x"> source object </param>
		/// <param name="safe"> true - def on exceptions </param>
		/// <param name="def"> returns if x converted to default(T) (or if exception in safe mode) </param>
		/// <param name="defgen"> generator for default Value </param>
		/// <typeparam name="T"> type to be converted to </typeparam>
		/// <returns> </returns>
		public static T To<T>(this object x, bool safe, T def = default(T), Func<T> defgen = null) {
			Func<object> objdefgen = null;
			if (null != defgen) {
				objdefgen = () => defgen();
			}
			return (T) x.ToTargetType(typeof (T), safe, def, objdefgen);
		}

		/// <summary>
		/// 	Converts given object to given type using custom conversion logic
		/// 	with some embeded extensions.
		/// </summary>
		/// <param name="x"> object to be converted </param>
		/// <param name="type"> type to be converted to </param>
		/// <param name="safe"> true - def instead exception </param>
		/// <param name="def"> default Value to return instead of default of type </param>
		/// <param name="defgen"> default generator to call and return result instead of default of type </param>
		/// <returns> </returns>
		public static object ToTargetType(this object x, Type type, bool safe = false, object def = null, Func<object> defgen = null) {
			try {
				bool converted;
				var result = TryConvert(x, type, out converted);
				if (!converted) {
					if (null != defgen) {
						return defgen();
					}
					if (null != def) {
						return def;
					}
				}
				return result;
			}
			catch (Exception) {
				if (safe) {
					if (null != defgen) {
						return defgen();
					}
					if (null != def) {
						return def;
					}
					return type.IsValueType ? Activator.CreateInstance(type) : null;
				}
				throw;
			}
		}

		private static object TryConvert(object x, Type type, out bool converted) {
			converted = false; //default state is not valid convert
			if (null == x) {
				if (type.IsValueType) {
					return Activator.CreateInstance(type);
				}
				var constructorInfo = type.GetConstructor(Type.EmptyTypes);
				if (constructorInfo != null) {
					return  constructorInfo.Invoke(null);
				}
				return null;
			}
			if (type.IsInstanceOfType(x)) {
				// null conversion - return object itself, it's final
				converted = true;
				return x;
			}
			/*var xElement = x as XElement;
			if (xElement != null) {
				converted = true;
				return xElement.Deserialize(type, null);
			}*/
			if (type.IsEnum) {
				if (x is int) {
					converted = true;
					var name = Enum.GetName(type, x);
					return Enum.Parse(type, name, true);
				}
				if (x is string) {
					
					converted = true;
					if (String.IsNullOrWhiteSpace(x as string)) {
						return Activator.CreateInstance(type);
					}
                    return ConvertEnum(type, x as string);
				}
			}
			try {
				converted = true;
				if (type == typeof (string)) {
					return x.ToStr();
				}
				if (type == typeof (int)) {
					return x.ToInt();
				}
				if (type == typeof (bool)) {
					return x.ToBool();
				}
				if (type == typeof (decimal)) {
					return x.ToDecimal();
				}
				if (type == typeof (DateTime)) {
					var ds = x.ToStr();
					if (String.IsNullOrWhiteSpace(ds)) {
						converted = false;
						return new DateTime(1900, 1, 1);
					}
					return DateTime.ParseExact(x.ToStr(), QorpentConst.Date.StandardDateFormats, CultureInfo.InvariantCulture,
					                           DateTimeStyles.AllowWhiteSpaces);
				}
				if (type.IsEnum) {
					if (x is ValueType) {
						var val = x.ToInt();
						return Enum.ToObject(type, val);
					}
                    return ConvertEnum(type, x.ToStr());
				}

				if (x is string[] && type == typeof (int[])) {
					return ((string[]) x).Select(_ => _.ToInt()).ToArray();
				}

				try {
					return Convert.ChangeType(x, type);
				}
				catch (Exception) {
					converted = false;
					if (type.IsValueType) {
						throw;
					}
					return null;
				}
			}
			catch (Exception ex) {
				throw new FormatException(
					String.Format("Cannot convert {0} of type {1} to {2}", "x", x == null ? "no type" : x.GetType().FullName, type),
					ex);
			}
		}

        private static object ConvertEnum(Type type, string x)
        {
            if (!x.Contains("+"))
            {
                return Enum.Parse(type, x as string, true);
            }
            else
            {
                var result = Activator.CreateInstance(type);
                var subitems = x.Split('+');
	            var utype = Enum.GetUnderlyingType(type);
                foreach (var s in subitems)
                {
	                if (utype == typeof (int)) {
		                result = (int) result | (int) ConvertEnum(type, s);
	                }else if (utype == typeof (ulong)) {
		                result = (ulong) result | (ulong) ConvertEnum(type, s);
	                }
                }
                return result;
            }
        }

		/// <summary>
		/// 	converts given object to DateTime with different formats
		/// </summary>
		/// <param name="obj"> </param>
		/// <param name="safe"></param>
		/// <returns> </returns>
		public static DateTime ToDate(this object obj,bool safe = false) {
			if (null == obj) {
				return new DateTime(1900, 1, 1);
			}
			if (obj is DateTime) {
				return (DateTime) obj;
			}
			var s = obj.ToStr();
			if (String.IsNullOrWhiteSpace(s)) {
				return new DateTime(1900, 1, 1);
			}
			try{
				return DateTime.ParseExact(s, QorpentConst.Date.StandardDateFormats, CultureInfo.InvariantCulture,
				                           DateTimeStyles.AllowWhiteSpaces);
			}
			catch (Exception ex){
				if(safe)return new DateTime(1900,1,1);
				throw ex;
			}

		}

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="x"></param>
	    /// <param name="extended"></param>
	    /// <returns></returns>
	    public static int ToInt(this double x, bool extended) {
            if (!extended) {
                return x.ToInt();
            }

            if (
                (x >= 1) || (x <= -1)
            ) {
                return Convert.ToInt32(x);
            }

            if (
                (x <= 0.5) && (x >= -0.5)
            ) {
                return 0;
            }

            if (x > 1.0) {
                return 1;
            }

	        return -1;
	    }
		/// <summary>
		/// 	converts object to Int32 null safe
		/// </summary>
		/// <param name="x"> </param>
		/// <returns> </returns>
		public static int ToInt(this object x) {
			if (null == x) {
				return 0;
			}
			if (Equals(String.Empty, x)) {
				return 0;
			}
			if (x is int) {
				return (int) x;
			}
			if (x is long)
			{
				return Convert.ToInt32(x);
			}
			if (x is decimal || x is double || x is Single) {
				return Convert.ToInt32(x);
			}
			return Convert.ToInt32(x.ToDecimal(true));
		}
		/// <summary>
		/// 	converts object to Int32 null safe
		/// </summary>
		/// <param name="x"> </param>
		/// <returns> </returns>
		public static long ToLong(this object x)
		{
			if (null == x)
			{
				return 0;
			}
			if (Equals(String.Empty, x))
			{
				return 0;
			}
			if (x is int)
			{
				return (int)x;
			}
			if (x is long)
			{
				return (long)x;
			}
			if (x is decimal || x is double || x is Single)
			{
				return Convert.ToInt64(x);
			}
			return Convert.ToInt64(x.ToDecimal(true));
		}

		/// <summary>
		/// 	converts object to decimal, null safe, format issues avoid
		/// </summary>
		/// <param name="x"> </param>
		/// <param name="safe"> </param>
		/// <returns> </returns>
		/// <exception cref="Exception"></exception>
		public static decimal ToDecimal(this object x, bool safe = false) {
			try {
				if (null == x) {
					return 0;
				}
				if (Equals(String.Empty, x)) {
					return 0;
				}
				if (x is decimal) {
					return (decimal) x;
				}
				var s = x as string;
				if (s != null) {
					if ("-" == s || "--" == s || "" == s || "error" == s) {
						return 0;
					}
					var cleandedString = (s).Replace(" ", String.Empty).Replace(",", ".");
					try {
						return Decimal.Parse(cleandedString, NumberFormatInfo.InvariantInfo);
					}
					catch (FormatException) {
						throw new Exception("format of '" + cleandedString + "' cannot be parsed as decimal");
					}
				}
				return Convert.ToDecimal(x);
			}
			catch {
				if (safe) {
					return 0;
				}
				throw;
			}
		}

		/// <summary>
		/// 	Converts object properties and values to dictionary
		/// </summary>
		/// <param name="obj"> </param>
		/// <returns> </returns>
		public static IDictionary<string, object> ToDict(this object obj) {
			if(obj is IDictionary<string, object>) {
				return (IDictionary<string, object>) obj;
			}
            if (obj is IDictionary<string, string>) {
                return ((IDictionary<string, string>) obj).ToDictionary(_=>_.Key,_=>(object)_.Value);
            }
			if (obj is IEnumerable<KeyValuePair<string, object>>){
				var dresult= new Dictionary<string, object>();
				foreach (var pair in (IEnumerable<KeyValuePair<string, object>>)obj){
					dresult[pair.Key] = pair.Value;
				}
				return dresult;
			}
			
			var result = new Dictionary<string, object>();
			foreach (var p in obj.GetType().GetProperties()) {
				result.Add(p.Name, p.GetValue(obj, null));
			}
			return result;
		}

		// ReSharper restore InconsistentNaming
		// ReSharper restore MemberCanBePrivate.Global
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
					if (String.IsNullOrWhiteSpace(Name)) {
						Name = element.Value;
					}
					if (String.IsNullOrWhiteSpace(Name)) {
						Name = Code;
					}
				}
				File = element.ChooseAttr("_file", "__file");
				Line = ToInt(element.ChooseAttr("_line", "__line"));
				Column = ToInt(element.ChooseAttr("_col", "__col"));
				Value = element.SelfValue();
			}

			/// <summary>
			/// 	Собственное значение элемента
			/// </summary>
			public string Value { get; set; }
			/// <summary>
			/// 
			/// </summary>
			/// <returns></returns>
			public string GetEfficienValue()
			{
				if (!String.IsNullOrWhiteSpace(Value)) return Value;
				if (!String.IsNullOrWhiteSpace(Name)) return Name;
				return Code;
			}

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
		/// 
		/// </summary>
		/// <param name="e"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static string AttrOrValue(this XElement e, string name){
			if (null != e.Attribute(name)) return e.Attribute(name).Value;
			return e.Value;
		}

		/// <summary>
		/// 	Возвращает только собственное значение элемента (конкатенация текстовых элементов через пробел)
		/// </summary>
		/// <param name="xElement"> </param>
		/// <returns> </returns>
		public static string SelfValue(this XElement xElement) {
			if (xElement == null) {
				throw new ArgumentNullException("xElement");
			}
			return String.Join(" ",xElement.Nodes().OfType<XText>().Select(x => x.Value));
		}

		/// <summary>
		///     Устанавливает атрибут, если значение не null
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public static XElement SetAttr(this XElement parent, string name, object value) {
			if (value != null) {
				if (value is string) {
					if (String.IsNullOrWhiteSpace(value as string)) {
						return parent;
					}
				}

				parent.SetAttributeValue(name, value);
			}

			return parent;
		}


	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="sourceElement"></param>
	    /// <param name="name"></param>
	    /// <param name="defaultvalue"></param>
	    /// <param name="returnDefaultIfEmpty"></param>
	    /// <returns></returns>
	    public static string El(this XElement sourceElement, XName name, string defaultvalue = "",
	        bool returnDefaultIfEmpty = false) {

            if (null == sourceElement) {
                return defaultvalue;
            }
            var el = sourceElement.Element(name);
            var value = null == el ? defaultvalue : el.Value;

            if (!returnDefaultIfEmpty) {
                return value ?? defaultvalue;
            }
            return string.IsNullOrWhiteSpace(value) ? defaultvalue : value;
	    }



	    /// <summary>
		/// 	Returns not-null string Value of elemnt's attribute (null-safe, existence-ignorance)
		/// </summary>
		/// <param name="sourceElement"> Element from which attribute requested </param>
		/// <param name="name"> name of requested attribute (can be string) </param>
		/// <param name="defaultvalue"> default Value if not attribute existed </param>
		/// <param name="returnDefaultIfEmpty">Returns default value even if value is not null, but is an empty string</param>
		/// <returns> string representation of attribute or empty string </returns>
		public static string Attr(this XElement sourceElement, XName name, string defaultvalue = "", bool returnDefaultIfEmpty = false) {
			if (null == sourceElement) {
				return defaultvalue;
			}
			var attr = sourceElement.Attribute(name);
			var value = null == attr ? defaultvalue : attr.Value;
			 
			if (!returnDefaultIfEmpty) {
				return value ?? defaultvalue;
			}
			return string.IsNullOrWhiteSpace(value) ? defaultvalue : value;
		}
		/// <summary>
		/// Производит резолюцию значений атрибутов, имен атрибутов и возвращает максимально валидный результат
		/// </summary>
		/// <param name="e"></param>
		/// <param name="names"></param>
		/// <returns></returns>
		public static string GetSmartValue(this XElement e,params string[] names){
			if (null == e) return "";
			foreach (var name in names){
				if (null != e.Attribute(name)) return e.Attribute(name).Value;
				if (e.Attributes().Any(_ => _.Value == name))
				{
					return "1";
				}	
			}
			return "";

		}
	}
}