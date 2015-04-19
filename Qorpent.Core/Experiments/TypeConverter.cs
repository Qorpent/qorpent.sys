using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Qorpent.Experiments;

namespace Qorpent.Utils
{
	/// <summary>
	/// Adds some relax/force logic for system-defined Convert 
	/// </summary>
	/// <remarks>
	/// TypeConverter is oriented to give null-safe, format-safe conversion for most usual types,
	/// that are used in web , cross-process, xml/json to code binding
	/// 
	/// Most of method follows same Sygnature To[TargetType] ( source , defaultValue, safeMode )
	/// where source is object to convert to target type, defaultValue is returned
	/// if source is undefined or invalid, safe - true if we must allow invalid data, false - we
	/// will throw erorr (it means "safe-to-caller")
	/// </remarks>
	public static class TypeConverter {
		private const string TCIE01 = "TCIE01 Given numeric exceed minint-maxint range and cannot be casted to Int32 ";
		private const string TCIE02 = "TCIE02 Given datetime exceed minint-maxint range and cannot be casted to Int32 ";
		private const string TCIE03 = "TCIE03 Given type cannot be casted to Int32 ";
		private const string TCIE04 = "TCIE04 Invalid numeric format for Int32 ";
		private const string TCDE01 = "TCIE04 Invalid source type for DateTime ";
		private const string TCDE02 = "TCIE04 Invalid format for DateTime ";
		private const string DictValueName = "__value";

		/// <summary>
		/// Converts any source to Int32 - most usual Int type
		/// </summary>
		/// <param name="source">Source object</param>
		/// <param name="defaultValue">default value to return for undefined or invalid source</param>
		/// <param name="safe">true (default) - caller get Int anyway (as defaultValue) even if source bad, otherwise error thrown on invalid</param>
		/// <returns>
		/// null - defaultValue 
		/// int - source
		/// string - empty/ws will be treated as defaultValue parses with smart space, comma and dot resolution with Ru-ru orientation - 
		/// if only comma exists - it will be treated as decimal delimiter, if invalid format - safe - defaultValue - othervise - erorr,
		/// will round to nearest int
		/// byte, short - cast to int
		/// uint, long, ulong, double, float, decimal - if in MinValue-MaxValue range - cast to int, if exceed - defaultValue if safe - error otherwise
		/// bool - 0/1
		/// datetime - seconds from 1970 (unix-like int datetime)
		/// enum - int value with limit control
		/// </returns>
		public static int ToInt(object source, int defaultValue = 0, bool safe = true) {
			/* LAST TIMESTAMP CHECKING WITH COMPARENESS TO Qorpent.Sys
			 * ToInt probes count: 10000000
				Serial BRIDGE: 1158
				Parallel BRIDGE: 352
				Serial OLDSYS: 48069
				Parallel OLDSYS: 12320
			 */
			if (null == source) {
				return defaultValue;
			}
			if (source is int) {
				return (int) source;
			}
			var dbl = double.MinValue;
			var s = source as string;
			if (s != null) {
				if (string.IsNullOrWhiteSpace(s)) {
					return defaultValue;
				}
                if (s.All(char.IsDigit))
                {
                    return int.Parse(s);
                }
				dbl = ParseToDouble(s, s.Length, defaultValue, safe);
			}
			else {
				var convertible = source as IConvertible;
				if (null != convertible) {
					if (source is bool) {
						return (bool) source ? 1 : 0;
					}
					if (source is DateTime) {
						var dt = (DateTime) source;
						if (dt > Constants.MinUnixTime && dt < Constants.MaxUnixTime) {
							return (int) (dt.Subtract(Constants.UnixBaseTime).TotalSeconds);
						}
						if (safe) {
							return defaultValue;
						}
						throw new ArgumentException(TCIE02);
					}
					dbl = Convert.ToDouble(source);
				}
			}
			if (dbl != double.MinValue) {
				if (dbl >= int.MinValue && dbl <= int.MaxValue) {
					return (int) Math.Round(dbl);
				}
				if (safe) {
					return defaultValue;
				}
				throw new ArgumentException(TCIE01);
			}
			if (safe) {
				return defaultValue;
			}
			throw new ArgumentException(TCIE03);
		}

		/// <summary>
		/// Converts given object to DateTime
		/// </summary>
		/// <param name="source"></param>
		/// <param name="defaultValue"></param>
		/// <param name="safe"></param>
		public static DateTime ToDate(object source, DateTime defaultValue = default (DateTime), bool safe = true) {
			if (defaultValue == default(DateTime)) {
				defaultValue = Constants.MinDateTime;
			}
			if (null == source)
			{
				return defaultValue;
			}
			if (source is DateTime)
			{
				return (DateTime)source;
			}
			var str = source as string;
			if (null != str) {
				return ParseToDateTime(str, defaultValue, safe);
			}
			
			var convertible = source as IConvertible;
			if (null != convertible) {
				var dbl = convertible.ToDouble(CultureInfo.InvariantCulture);
				if (0 == dbl) {
					return Constants.MinDateTime;
				}
				return Constants.UnixBaseTime.AddSeconds(dbl);
			}
			if (safe)
			{
				return defaultValue;
			}
			throw new ArgumentException(TCDE01);
		}
		/// <summary>
		/// Стандартные форматы даты
		/// </summary>
		private static readonly string[] StandardDateFormats = {
					"dd.MM.yyyy HH:mm:ss","dd.MM.yyyy HH:mm", "dd.MM.yyyy", "yyyy-MM-dd HH:mm","yyyy-MM-dd HH:mm:ss","yyyyMMdd HH:mm","yyyyMMdd","yyyyMMdd HH:mm:ss",
					"yyyy-MM-dd", "yyyyMMddHHmm", "yyyyMMddHHmmss", "yyyy-MM-ddTHH:mm:ss.fffZ", "dd-MM-yyyy"
				};
		private static DateTime ParseToDateTime(string str, DateTime defaultValue, bool safe) {
			DateTime result;
			bool found = false;
			found = DateTime.TryParseExact(str, StandardDateFormats, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out result);
			if (found) return result;
			found = WebDateTimeParser.TryParse(out result, str);
			if (found) return result;
			if (safe) {
				return defaultValue;
			}
			throw new ArgumentException(TCDE02+str);
		}

		/// <summary>
		/// Converts any to Int64
		/// </summary>
		/// <param name="source"></param>
		/// <param name="defaultValue"></param>
		/// <param name="safe"></param>
		/// <returns>explained in ToInt (with correction to 64bit size)</returns>
		public static long ToLong(object source, int defaultValue = 0, bool safe = true) {
			/* LAST TIMESTAMP CHECKING WITH COMPARENESS TO Qorpent.Sys
			 * ToInt probes count: 10000000
				Serial BRIDGE: 1158
				Parallel BRIDGE: 352
				Serial OLDSYS: 48069
				Parallel OLDSYS: 12320
			 */
			if (null == source) {
				return defaultValue;
			}
			if (source is long) {
				return (long) source;
			}
			var dbl = double.MinValue;
			var s = source as string;
			if (s != null) {
				if (string.IsNullOrWhiteSpace(s)) {
					return defaultValue;
				}
			    if (s.All(char.IsDigit)) {
			        return long.Parse(s);
			    }
				dbl = ParseToDouble(s, s.Length, defaultValue, safe);
			}
			else {
				var convertible = source as IConvertible;
				if (null != convertible) {
					if (source is bool) {
;                        return (bool) source ? 1 : 0;
					}
					if (source is DateTime) {
						var dt = (DateTime) source;
						if (dt > Constants.MinUnixTime && dt < Constants.MaxUnixTime) {
							return (int) (dt.Subtract(Constants.UnixBaseTime).TotalSeconds);
						}
						if (safe) {
							return defaultValue;
						}
						throw new ArgumentException(TCIE02);
					}
					dbl = Convert.ToDouble(source);
				}
			}
			if (dbl != double.MinValue) {
				if (dbl >= long.MinValue && dbl <= long.MaxValue) {
					return (long) Math.Round(dbl);
				}
				if (safe) {
					return defaultValue;
				}
				throw new ArgumentException(TCIE01);
			}
			if (safe) {
				return defaultValue;
			}
			throw new ArgumentException(TCIE03);
		}

		/// <summary>
		/// Converts any to Decimal
		/// </summary>
		/// <param name="source"></param>
		/// <param name="defaultValue"></param>
		/// <param name="safe"></param>
		/// <returns>explained in ToInt (with correction to 64bit size)</returns>
		public static decimal ToDecimal(object source, decimal defaultValue = 0, bool safe = true) {
			/* LAST TIMESTAMP CHECKING WITH COMPARENESS TO Qorpent.Sys
			 * ToInt probes count: 10000000
				Serial BRIDGE: 1158
				Parallel BRIDGE: 352
				Serial OLDSYS: 48069
				Parallel OLDSYS: 12320
			 */
			if (null == source) {
				return defaultValue;
			}
			if (source is decimal) {
				return (decimal) source;
			}
			var dbl = double.MinValue;
			var s = source as string;
			if (s != null) {
				if (string.IsNullOrWhiteSpace(s)) {
					return defaultValue;
				}
				dbl = ParseToDouble(s, s.Length, (double)defaultValue, safe);
			}
			else {
				var convertible = source as IConvertible;
				if (null != convertible) {
					if (source is bool) {
						return (bool) source ? 1 : 0;
					}
					if (source is DateTime) {
						var dt = (DateTime) source;
						if (dt > Constants.MinUnixTime && dt < Constants.MaxUnixTime) {
							return (decimal) (dt.Subtract(Constants.UnixBaseTime).TotalSeconds);
						}
						if (safe) {
							return defaultValue;
						}
						throw new ArgumentException(TCIE02);
					}
					dbl = Convert.ToDouble(source);
				}
			}
			if (dbl != double.MinValue) {
				if (dbl >= (double) decimal.MinValue && dbl <= (double) decimal.MaxValue) {
					return (decimal) dbl;
				}
				if (safe) {
					return defaultValue;
				}
				throw new ArgumentException(TCIE01);
			}
			if (safe) {
				return defaultValue;
			}
			throw new ArgumentException(TCIE03);
		}

		/// <summary>
		/// Converts any to Decimal
		/// </summary>
		/// <param name="source"></param>
		/// <param name="defaultValue"></param>
		/// <param name="safe"></param>
		/// <returns>explained in ToInt (with correction to 64bit size)</returns>
		public static double ToDouble(object source, int defaultValue = 0, bool safe = true)
		{
			/* LAST TIMESTAMP CHECKING WITH COMPARENESS TO Qorpent.Sys
			 * ToInt probes count: 10000000
				Serial BRIDGE: 1158
				Parallel BRIDGE: 352
				Serial OLDSYS: 48069
				Parallel OLDSYS: 12320
			 */
			if (null == source)
			{
				return defaultValue;
			}
			if (source is double)
			{
				return (double)source;
			}
			var s = source as string;
			if (s != null)
			{
				if (string.IsNullOrWhiteSpace(s))
				{
					return defaultValue;
				}
				return ParseToDouble(s, s.Length, defaultValue, safe);
			}
			var convertible = source as IConvertible;
			if (null != convertible)
			{
				if (source is bool)
				{
					return (bool)source ? 1 : 0;
				}
				if (source is DateTime)
				{
					var dt = (DateTime)source;
					if (dt > Constants.MinUnixTime && dt < Constants.MaxUnixTime)
					{
						return dt.Subtract(Constants.UnixBaseTime).TotalSeconds;
					}
					if (safe)
					{
						return defaultValue;
					}
					throw new ArgumentException(TCIE02);
				}
				return Convert.ToDouble(source);
			}

			if (safe)
			{
				return defaultValue;
			}
			throw new ArgumentException(TCIE03);
		}

		/// <summary>
		/// Lightweight numeric parser for most usable formats
		/// </summary>
		/// <returns></returns>
		private static double ParseToDouble(string source, int size, double defaultValue = 0, bool safe = true) {
			byte decimals = 0;
			var allowsign = true;
			var wascomma = false;
			var dotdecimal = false;
			var wasdot = false;
			var result = 0d;
			var invalid = false;
			var negate = false;
			for (var i = 0; i < size; i++) {
				var c = source[i];
				if (char.IsWhiteSpace(c)) {
					continue;
				}
				switch (c) {
					case '+':
						if (allowsign) {
							allowsign = false;
							continue;
						}
						invalid = true;
						break;
					case '-':
						if (allowsign) {
							allowsign = false;
							negate = true;
							continue;
						}
						invalid = true;
						break;
					case '.':
						if (wasdot) {
							invalid = true;
							break;
						}
						wasdot = true;
						decimals = 0;
						continue;
					case ',':
						if (wasdot) {
							invalid = true;
							break;
						}
						if (wascomma) {
							decimals = 0;
							dotdecimal = true;
							continue;
						}
						wascomma = true;
						continue;
					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
						double val = (c - '0');
						if (wasdot) {
							decimals++;
							result += (val/Math.Pow(10, decimals));
							continue;
						}
						if (wascomma && !dotdecimal) {
							decimals++;
						}
						result = result*10 + val;
						continue;
					default:
						invalid = true;
						break;
				}
			}
			if (invalid) {
				if (safe) {
					return defaultValue;
				}
				throw new ArgumentException(TCIE04);
			}
			if (decimals != 0 && !wasdot) {
				result = result/(Math.Pow(10, decimals));
			}
			if (negate) {
				result = -result;
			}
			return result;
		}


		/// <summary>
		/// Effective boolean value for given object
		/// </summary>
		/// <param name="source"></param>
		/// <param name="defaultValue"></param>
		/// <param name="safe"></param>
		/// <returns>
		/// nulls - false
		/// reference object -true
		/// bool - itself
		/// string - empty/ws/false(ignorecase)/0 - false - otherwise true
		/// all numerics != 0 - true, 0 - false
		/// enums if value ==0 - false, otherwise - true
		/// datetime - if in limit 1900-3000 - true, otherwise - false
		/// 
		/// </returns>
		public static bool ToBool(object source, bool defaultValue = false, bool safe = true) {
			if (null == source) {
				return false;
			}
			if (source is bool) {
				return (bool) source;
			}
			var s = source as string;
			if (s != null) {
				if (String.IsNullOrWhiteSpace(s)) {
					return false;
				}
				if ("0" == s) {
					return false; //usual "False" alternative
				}
				return !s.Equals("FALSE", StringComparison.InvariantCultureIgnoreCase);
			}
			var convertible = source as IConvertible;
			if (convertible != null) {
				if (source is DateTime) {
					var dt = (DateTime) source;
					return dt > Constants.MinDateTime && dt < Constants.MaxDateTime;
				}
				return Convert.ToBoolean(source);
			}

			var collection = source as ICollection;
			if (collection != null) {
				return collection.Count != 0;
			}

			return true;
		}



		/// <summary>
		/// 	Converts object properties and values to dictionary
		/// </summary>
		/// <param name="obj"> </param>
		/// <param name="existed"></param>
		/// <param name="nullsafe"></param>
		/// <param name="itemdelimiter"></param>
		/// <param name="valdelimiter"></param>
		/// <param name="escapechar"></param>
		/// <param name="trim"></param>
		/// <param name="urlescape"></param>
		/// <returns> </returns>
		public static IDictionary<string, object> ToDict(this object obj, IDictionary<string,object> existed = null,  bool nullsafe = true, char itemdelimiter= ';',char valdelimiter='=', char escapechar = '\\', bool trim = true, bool urlescape = false) {
			if (null == obj) {
				if(nullsafe)return new Dictionary<string, object>();
				return null;
			}
			var result = existed ?? new Dictionary<string, object>();
			var str = obj as string;
		   
			if (null != str) {
				ReadAsDictionary(str,itemdelimiter,valdelimiter,trim,escapechar,urlescape,result);
				return result;
			}
			var convertible = obj as IConvertible;
			if (null != convertible) {
				result[DictValueName] = convertible;               
			}
			var collection = obj as ICollection;
			if (null != collection) {
				if (CheckDictionary<string, object>(obj, result)) return result;
				if (CheckDictionary<string, string>(obj, result)) return result;
				if (ReflectionUtils.IsGenericCompatible<IEnumerable<KeyValuePair<object, object>>>(collection)) {
					Func<object, object> keyGetter = null;
					Func<object, object> valGetter = null;
					foreach (var pair in collection) {
						if (null == keyGetter) {
							keyGetter = ReflectionUtils.BuildGetter(pair.GetType().GetProperty("Key"));
							valGetter = ReflectionUtils.BuildGetter(pair.GetType().GetProperty("Value"));
						}
						var key = keyGetter(pair);
						var val = valGetter(pair);
						result[key.ToString()] = val;
					}
				}
				else {
					var idx = 0;
					foreach (var item in collection) {
						result[idx.ToString()] = item;
						idx++;
					}

				}

				return result;
			}

			var type = obj.GetType();
			
			foreach (var p in type.GetProperties()) {
				//result.Add(p.Name, ReflectionUtils.BuildGetter(p)(obj));
				result[p.Name]= p.GetValue(obj);
			}
			return result;
		}

		private static bool CheckDictionary<K,V>(object obj,  IDictionary<string, object> result) {
			var dss = obj as IEnumerable<KeyValuePair<K,V>> ;
			if (null != dss) {
				foreach (var pair in dss) {
						result[pair.Key.ToString()] = pair.Value;			    
				}
				return true;
			}
			return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="source"></param>
		/// <param name="itemDelimiter"></param>
		/// <param name="valueDelimiter"></param>
		/// <param name="trim"></param>
		/// <param name="escape"></param>
		/// <returns></returns>
		public static IDictionary<string,string> ReadAsDictionary(this string source, char itemDelimiter = ';',
			char valueDelimiter = '=', bool trim = true, char escape = '\\') {
			return ReadAsDictionary<string>(source,itemDelimiter,valueDelimiter,trim,escape);
		}

		/// <summary>
		/// Считывает строку как набор ключ-значение
		/// </summary>
		/// <param name="source">строка - источник</param>
		/// <param name="itemDelimiter">разделитель между отдельными элементами словаря</param>
		/// <param name="valueDelimiter">разделитель имени и значения</param>
		/// <param name="trim">триммирование значений</param>
		/// <param name="escape">экранирующий символ</param>
		/// <param name="urlescape"></param>
		/// <param name="target"></param>
		/// <returns>словарь ключ-значение</returns>
		/// <remarks>при совпадающих ключах выигрывает последний</remarks>
		public static IDictionary<string, T> ReadAsDictionary<T>(this string source, char itemDelimiter = ';',
			char valueDelimiter = '=', bool trim = true, char escape = '\\', bool urlescape = false, IDictionary<string,T> target = null)
		{
			var result = target ?? new Dictionary<string, T>();
			if (!string.IsNullOrWhiteSpace(source))
			{
				var nameBuffer = new StringBuilder();
				var valBuffer = new StringBuilder();
				char c = '\0';
				bool inName = false;
				bool inEscape = false;
				for (var i = 0; i < source.Length; i++)
				{
					c = source[i];
					if (inEscape)
					{
						if (inName)
						{
							valBuffer.Append(c);
						}
						else
						{
							nameBuffer.Append(c);
						}
						inEscape = false;
						continue;
					}
					if (c == escape)
					{
						inEscape = true;
						continue;
					}
					if (c == itemDelimiter) {
						var val = trim ? valBuffer.ToString().Trim() : valBuffer.ToString();
						var key = nameBuffer.ToString().Trim();
						if (urlescape) {
							val = Uri.UnescapeDataString(val.Replace('+',' '));
							key = Uri.UnescapeDataString(key.Replace('+',' '));
							if (trim) {
								val = val.Trim();
								key = key.Trim();
							}
						}
						if (typeof (T) == typeof (string)) {
							(result as IDictionary<string, string>)[key] = val;
						}
						else if (typeof (T) == typeof (object)) {
							(result as IDictionary<string,object>)[key] = val;
						}
						else {
							result[key] = (T)(object)(val);
						}
						
						inName = false;
						nameBuffer.Clear();
						valBuffer.Clear();
						continue;
					}
					if (c == valueDelimiter)
					{
						inName = true;
						continue;
					}
					if (inName)
					{
						valBuffer.Append(c);
					}
					else
					{
						nameBuffer.Append(c);
					}
				}
				if (inName)
				{
					var val = trim ? valBuffer.ToString().Trim() : valBuffer.ToString();
					var key = nameBuffer.ToString().Trim();
					if (urlescape) {
						val = Uri.UnescapeDataString(val.Replace('+', ' '));
						key = Uri.UnescapeDataString(key.Replace('+', ' '));
						if (trim)
						{
							val = val.Trim();
							key = key.Trim();
						}
					}
					if (typeof(T) == typeof(string))
					{
						(result as IDictionary<string, string>)[key] = val;
					}
					else if (typeof(T) == typeof(object))
					{
						(result as IDictionary<string, object>)[key] = val;
					}
					else
					{
						result[key] = (T)(object)(val);
					}
				   
				}
			}
			return result;
		}
	}
}