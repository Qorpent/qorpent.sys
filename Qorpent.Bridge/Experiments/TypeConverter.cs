using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if EXBRIDGE
namespace Qorpent.Experiments.Utils
#else
namespace Qorpent.Utils
#endif
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
    public static partial class TypeConverter {
        private const string TCIE01 = "TCIE01 Given numeric exceed minint-maxint range and cannot be casted to Int32";
        private const string TCIE02 = "TCIE02 Given datetime exceed minint-maxint range and cannot be casted to Int32";
        private const string TCIE03 = "TCIE03 Given type cannot be casted to Int32";
        private const string TCIE04 = "TCIE04 Invalid numeric format for Int32";

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
                    return (decimal) Math.Round(dbl);
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
        /// <returns> </returns>
        public static IDictionary<string, object> ToDict(this object obj)
        {
            if (obj is IDictionary<string, object>)
            {
                return (IDictionary<string, object>)obj;
            }
            if (obj is IDictionary<string, string>)
            {
                return ((IDictionary<string, string>)obj).ToDictionary(_ => _.Key, _ => (object)_.Value);
            }
            if (obj is IEnumerable<KeyValuePair<string, object>>)
            {
                var dresult = new Dictionary<string, object>();
                foreach (var pair in (IEnumerable<KeyValuePair<string, object>>)obj)
                {
                    dresult[pair.Key] = pair.Value;
                }
                return dresult;
            }

            var result = new Dictionary<string, object>();
            foreach (var p in obj.GetType().GetProperties())
            {
                result.Add(p.Name, p.GetValue(obj, null));
            }
            return result;
        }
    }
}