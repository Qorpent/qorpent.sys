using System;

namespace Qorpent.Utils {
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
        private const string TCIE01 = "TCIE01 Given Int64 exceed minint-maxint range and cannot be casted to Int32";
        private const string TCIE02 = "TCIE02 Given UInt64 exceed minint-maxint range and cannot be casted to Int32";
        private const string TCIE03 = "TCIE03 Given decimal exceed minint-maxint range and cannot be casted to Int32";
        private const string TCIE04 = "TCIE04 Given double exceed minint-maxint range and cannot be casted to Int32";
        private const string TCIE05 = "TCIE05 Given float exceed minint-maxint range and cannot be casted to Int32";
        private const string TCIE06 = "TCIE06 Given uint exceed minint-maxint range and cannot be casted to Int32";
        private const string TCIE07 = "TCIE07 Given datetime exceed minint-maxint range and cannot be casted to Int32";
        private const string TCIE08 = "TCIE08 Given type cannot be casted to Int32";
        private const string TCIE09 = "TCIE09 Given enum value exceed minint-maxint range and cannot be casted to Int32";
        private const string TCIE10 = "TCIE10 Invalid numeric format for Int32";
        private static readonly DateTime UnixTime = new DateTime(1970, 1, 1);

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

            var s = source as string;
            if (s != null) {
                if (string.IsNullOrWhiteSpace(s)) {
                    return defaultValue;
                }
                return (int) Math.Round(ParseToDouble(s, defaultValue, safe));
            }

            if (source is byte || source is short || source is ushort) {
                return Convert.ToInt32(source);
            }

            if (source is long) {
                var lng = (long) source;
                if (lng >= int.MinValue && lng <= int.MaxValue) {
                    return (int) lng;
                }
                if (safe) {
                    return defaultValue;
                }
                throw new ArgumentException(TCIE01);
            }

            if (source is ulong) {
                var lng = (ulong) source;
                if (lng <= int.MaxValue) {
                    return (int) lng;
                }
                if (safe) {
                    return defaultValue;
                }
                throw new ArgumentException(TCIE02);
            }

            if (source is decimal) {
                var lng = (decimal) source;
                if (lng >= int.MinValue && lng <= int.MaxValue) {
                    return (int) Math.Round(lng);
                }
                if (safe) {
                    return defaultValue;
                }
                throw new ArgumentException(TCIE03);
            }

            if (source is double) {
                var lng = (double) source;
                if (lng >= int.MinValue && lng <= int.MaxValue) {
                    return (int) Math.Round(lng);
                }
                if (safe) {
                    return defaultValue;
                }
                throw new ArgumentException(TCIE04);
            }
            if (source is float) {
                var lng = (float) source;
                if (lng >= int.MinValue && lng <= int.MaxValue) {
                    return (int) Math.Round(lng);
                }
                if (safe) {
                    return defaultValue;
                }
                throw new ArgumentException(TCIE05);
            }

            if (source is uint) {
                var lng = (uint) source;
                if (lng <= int.MaxValue) {
                    return (int) lng;
                }
                if (safe) {
                    return defaultValue;
                }
                throw new ArgumentException(TCIE06);
            }

            if (source is bool) {
                return (bool) source ? 1 : 0;
            }

            if (source is DateTime) {
                var dt = (DateTime) source;
                if (dt.Year > 1903 && dt.Year < 2038) {
                    return (int) (dt.Subtract(UnixTime).TotalSeconds);
                }
                if (safe) {
                    return defaultValue;
                }
                throw new ArgumentException(TCIE07);
            }

            var tp = source.GetType();

            if (tp.IsEnum) {
                if (Enum.GetUnderlyingType(tp) == typeof (int)) {
                    return (int) source;
                }
                var lng = (long) source;
                if (lng >= int.MinValue && lng <= int.MaxValue) {
                    return (int) lng;
                }
                if (safe) {
                    return defaultValue;
                }
                throw new ArgumentException(TCIE09);
            }

            if (safe) {
                return defaultValue;
            }
            throw new ArgumentException(TCIE08);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="defaultValue"></param>
        /// <param name="safe"></param>
        /// <returns></returns>
        private static double ParseToDouble(string source, int defaultValue, bool safe) {
            byte decimals = 0;
            var allowsign = true;
            var wascomma = false;
            var dotdecimal = false;
            var wasdot = false;
            var result = 0d;
            var invalid = false;
            var negate = false;
            foreach (var c in source) {
                if (char.IsWhiteSpace(c)) {
                    continue;
                }
                switch (c) {
                    case '0':
                        if (wascomma && !wasdot && !dotdecimal) {
                            decimals++;
                        }
                        continue;
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
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        double val = (c - '1') + 1;
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
                throw new ArgumentException(TCIE10);
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
            
        }

    }
}