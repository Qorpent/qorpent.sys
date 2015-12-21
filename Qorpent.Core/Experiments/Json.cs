using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Channels;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using Qorpent.Json;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;
using Map = System.Collections.Generic.IDictionary<string,object>;  
using Arr = System.Array;  

namespace Qorpent.Experiments {

   

    /// <summary>
    /// 
    /// </summary>
    public static class Json {

        private static char[] seps = new[] {'.', '['};
        /// <summary>
        /// Получить значение из объекта по пути
        /// </summary>
        /// <param name="json"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static object Get(object json, string path) {
            if (null == json) return null;
            
            var parts = path.Split(seps);
            if (parts.Length > 1 && parts[0] == "*") {
                json = Search(json, parts[1]);
                if (null == json) return null;
                parts = parts.Skip(1).ToArray();
            }
            for (var i = 0; i < parts.Length; i++) {
                var p = parts[i];
                if (p[p.Length-1]==']') {
                    if (p.StartsWith("\"")) {
                        parts[i] = p.Substring(1, p.Length - 3);

                    }
                    else {
                        parts[i] = "[" + parts[i];
                    }
                }
            }
            return GetInternal(json,parts);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IEnumerable<object> Select(object json, string path) {
            var result = Get(json, path);
            if (result is IEnumerable<object>) {
                var res =  (IEnumerable<object>) result;
                foreach (var re in res) {
                    yield return re;
                }
            }
            yield break;
        }
        /// <summary>
        /// Находит первый объект, содержащий указанное имя
        /// </summary>
        /// <param name="pathstart"></param>
        /// <returns></returns>
        public static object Search(object root,string pathstart) {
            var dict = root as IDictionary<string,object>;
            var arr = root as object[];
            if (null != dict) {
                if (dict.ContainsKey(pathstart)) {
                    return dict;
                }
                foreach (var prop in dict) {
                        var subsearch = Search(prop.Value, pathstart);
                        if (null != subsearch) {
                            return subsearch;
                        }
                    
                }
            }
            else if (null != arr) {
                foreach (var o in arr) {
                    var subresult = Search(o, pathstart);
                    if (null != subresult) {
                        return subresult;
                    }
                }
            }

            return null;
        }


        private static object GetInternal(object json, string[] pathParts) {
            var current = json;
            foreach(var pp in pathParts) {
                current = GetInternal(current, pp);
                if (null == current) return null;
            }
            return current;
        }
        private static object GetInternal(object json, string pathPart) {
            if (null == json) return null;
            if (json is XElement) {
                var xml = json as XElement;
                var attr = xml.Attribute(pathPart);
                if (null != attr) {
                    return attr.Value;
                }
                var e = xml.Element(pathPart);
                return e;
            }
            if (json is Array) {
                var a = json as object[];
                if (pathPart.ToLowerInvariant() == "length") return a.Length;
                if (pathPart.StartsWith("[") && pathPart.EndsWith("]")) {
                    var pos = Convert.ToInt32(pathPart.Substring(1, pathPart.Length - 2));
                    if (a.Length <= pos) return null;
                    return a[pos];
                }
                return null;
            }
            if (json is IDictionary<string, object>) {
                var d = json as IDictionary<string, object>;
                if (d.ContainsKey(pathPart)) return d[pathPart];
                return null;
            }
            if (json is IDictionary<string, string>)
            {
                var d = json as IDictionary<string, string>;
                if (d.ContainsKey(pathPart)) return d[pathPart];
                return null;
            }
            return ByReflection(json, pathPart);
        }

        private static object ByReflection(object json, string pathPart) {
            var type = json.GetType();
            var prop = json.GetType().GetProperty(pathPart);
            if (null != prop) {
                return prop.GetValue(json);
            }
            var fld = json.GetType().GetField(pathPart);
            if (null != fld) {
                return fld.GetValue(json);
            }

            prop = json.GetType()
                .GetProperty(pathPart,
                    BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public |
                    BindingFlags.IgnoreCase);
            if (null != prop) {
                return prop.GetValue(json);
            }
            fld = json.GetType()
                .GetField(pathPart,
                    BindingFlags.Instance | BindingFlags.GetField | BindingFlags.Public |
                    BindingFlags.IgnoreCase);
            if (null != fld) {
                return fld.GetValue(json);
            }

            return null;
        }

        /// <summary>
        /// Retrives null, object[] or IDictionary&lt;string,object&gt;
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static unsafe object Parse(string json) {
            buffer = buffer ?? new StringBuilder();
            if (!string.IsNullOrWhiteSpace(json)) {
                fixed (char* j = json) {
                    var cur = j;
                    return ReadDocument(cur, j);
                }
            }
            return null;
        }

        public static object Jsonify(object src) {
            if (src is string) return Parse(src as string);
            var json = Stringify(src);
            return Parse(json);
        }

        private static unsafe object ReadDocument(char* cur, char* basis) {
            object result = null;
            bool defined = false;
            while (true) {
                var c = *cur;
                if (c == 0) {
                    return result;
                }
                if (!char.IsWhiteSpace(c)) {
                    if (defined) {
                        throw new Exception("Illegal data after end " + (cur - basis)/2);
                    }
                    if (c == '"') {
                        result = ReadString(cur, basis, out cur);
                        defined = true;
                        continue;
                    }
                    if (c == '-' || char.IsDigit(c)) {
                        result = ReadNumber(cur, basis, out cur);
                        defined = true;
                        continue;
                    }
                    if (c == '{') {
                        result = ReadObject(cur, basis, out cur);
                        defined = true;
                        continue;
                    }
                    if (c == '[') {
                        result = ReadArray(cur, basis, out cur);
                        defined = true;
                        continue;
                    }
                    if (c == 't') {
                        result = ReadTrue(cur, basis, out cur);
                        defined = true;
                        continue;
                    }
                    if (c == 'f') {
                        result = ReadFalse(cur, basis, out cur);
                        defined = true;
                        continue;
                    }
                    if (c == 'n') {
                        ReadNull(cur, basis, out cur);
                        defined = true;
                        continue;
                    }

                    throw new Exception("Illegal character " + (int) c + " at " + (cur - basis)/2);
                }

                cur += 1;
            }
        }

        private static unsafe object ReadNull(char* cur, char* basis, out char* pos) {
            cur += 1;
            if ('u' != *cur) {
                throw new Exception("invalid null literal at " + (cur - basis + 1));
            }
            cur += 1;
            if ('l' != *cur) {
                throw new Exception("invalid null literal at " + (cur - basis + 1));
            }
            cur += 1;
            if ('l' != *cur) {
                throw new Exception("invalid null literal at " + (cur - basis + 1));
            }
            var term = *(cur + 1);
            if (0 != term && ',' != term && ']' != term && '}' != term && !char.IsWhiteSpace(term)) {
                throw new Exception("invalid null literal at " + (cur - basis + 1));
            }
            pos = cur + 1;
            return null;
        }

        private static unsafe bool ReadFalse(char* cur, char* basis, out char* pos) {
            cur += 1;
            if ('a' != *cur) {
                throw new Exception("invalid false literal at " + (cur - basis));
            }
            cur += 1;
            if ('l' != *cur) {
                throw new Exception("invalid false literal at " + (cur - basis));
            }
            cur += 1;
            if ('s' != *cur) {
                throw new Exception("invalid false literal at " + (cur - basis));
            }
            cur += 1;
            if ('e' != *cur) {
                throw new Exception("invalid false literal at " + (cur - basis));
            }
            var term = *(cur + 1);
            if (0 != term && ',' != term && ']' != term && '}' != term && !char.IsWhiteSpace(term)) {
                throw new Exception("invalid false literal at " + (cur - basis));
            }
            pos = cur + 1;
            return false;
        }

        private static unsafe bool ReadTrue(char* cur, char* basis, out char* next) {
            cur += 1;
            if ('r' != *cur) {
                throw new Exception("invalid true literal at " + (cur - basis + 1));
            }
            cur += 1;
            if ('u' != *cur) {
                throw new Exception("invalid true literal at " + (cur - basis + 1));
            }
            cur += 1;
            if ('e' != *cur) {
                throw new Exception("invalid true literal at " + (cur - basis + 1));
            }
            var term = *(cur + 1);
            if (0 != term && ',' != term && ']' != term && '}' != term && !char.IsWhiteSpace(term)) {
                throw new Exception("invalid true literal at " + (cur - basis + 1));
            }
            next = cur + 1;
            return true;
        }


        private static unsafe IDictionary<string, object> ReadObject(char* cur, char* basis, out char* pos) {
            bool defined = false;
            bool wascomma = false; //start of array treats that next item avail
            bool wascolon = false;
            bool wasname = false;
            bool wasvalue = false;
            bool allowname = true;
            string currentName = null;
            IDictionary<string, object> result = new Dictionary<string, object>();
            if ('{' == *cur) {
                cur += 1;
            }
            while (true) {
                var c = *cur;
                if (c == 0) {
                    break;
                }
                if (c == '}') {
                    if (wascomma) {
                        defined = true;
                        break;
                       //throw new Exception("invalid trail comma at " + (cur - basis)); HACK:
                    }
                    if (wascolon) {
                        throw new Exception("invalid end after colon");
                    }
                    if (wasname && !wasvalue) {
                        throw new Exception("invalid end after name");
                    }
                    defined = true;
                    break;
                }
                if (!char.IsWhiteSpace(c)) {
                    if (c == ',') {
                        if (wascomma) {
                            throw new Exception("dbl comma at " + (basis - cur));
                        }
                        if (!wasvalue) {
                            throw new Exception("comma allowed only after value at " + (cur - basis));
                        }
                        wascomma = true;
                        wasname = false;
                        cur += 1;
                        continue;
                    }

                    if (!wasname) {
                        if (c != '"') {
                            throw new Exception("only strings available for names at " + (basis - cur));
                        }
                        if (allowname) {
                            allowname = false;
                        }
                        currentName = ReadString(cur, basis, out cur);
                        wasname = true;
                        wasvalue = false;
                        wascomma = false;
                        continue;
                    }


                    if (c == ':') {
                        if (wascolon || wasvalue) {
                            throw new Exception("colon can be only one time at " + (cur - basis));
                        }
                        wascolon = true;
                    }

                    else {
                        if (c == '"') {
                            result[currentName] = ReadString(cur, basis, out cur);
                            wasvalue = true;
                            wascolon = false;
                            continue;
                        }
                        if (c == '-' || char.IsDigit(c)) {
                            result[currentName] = ReadNumber(cur, basis, out cur);
                            wasvalue = true;
                            wascolon = false;

                            continue;
                        }
                        switch (c) {
                            case '{':
                                result[currentName] = ReadObject(cur, basis, out cur);
                                wasvalue = true;
                                wascolon = false;
                                continue;
                            case '[':
                                result[currentName] = ReadArray(cur, basis, out cur);
                                wasvalue = true;
                                wascolon = false;
                                continue;
                            case 't':
                                result[currentName] = ReadTrue(cur, basis, out cur);
                                wasvalue = true;
                                wascolon = false;
                                continue;
                            case 'f':
                                result[currentName] = ReadFalse(cur, basis, out cur);
                                wasvalue = true;
                                wascolon = false;
                                continue;
                            case 'n':
                                result[currentName] = ReadNull(cur, basis, out cur);
                                wasvalue = true;
                                wascolon = false;
                                continue;
                        }
                        throw new Exception("invalid symbol in object " + (cur - basis));
                    }
                }


                cur += 1;
            }

            if (!defined) {
                throw new Exception("not terminated array at " + (cur - basis));
            }
            pos = cur + 1;
            return result;
        }

        private static unsafe object[] ReadArray(char* cur, char* basis, out char* pos) {
            bool defined = false;
            bool wascomma = true; //start of array treats that next item avail
            IList<object> list = new List<object>();
            if ('[' == *cur) {
                cur += 1;
            }
            while (true) {
                var c = *cur;
                if (c == ']') {
                    //if (wascomma) {
                    //    throw new Exception("invalid trail comma at " + (cur - basis));
                    //}
                    defined = true;
                    break;
                }
                if (c == 0) {
                    break;
                }


                if (!char.IsWhiteSpace(c)) {
                    if (c == ',') {
                        if (wascomma) {
                            throw new Exception("double comma at " + (cur - basis));
                        }
                        wascomma = true;
                    }
                    else {
                        if (c == '"') {
                            list.Add(ReadString(cur, basis, out cur));
                            wascomma = false;
                            continue;
                        }
                        if (c == '-' || char.IsDigit(c)) {
                            list.Add(ReadNumber(cur, basis, out cur));
                            wascomma = false;
                            continue;
                        }
                        switch (c) {
                            case '{':
                                list.Add(ReadObject(cur, basis, out cur));
                                wascomma = false;
                                continue;
                            case '[':
                                list.Add(ReadArray(cur, basis, out cur));
                                wascomma = false;
                                continue;
                            case 't':
                                list.Add(ReadTrue(cur, basis, out cur));
                                wascomma = false;
                                continue;
                            case 'f':
                                list.Add(ReadFalse(cur, basis, out cur));
                                wascomma = false;
                                continue;
                            case 'n':
                                list.Add(ReadNull(cur, basis, out cur));
                                wascomma = false;
                                continue;
                        }
                        throw new Exception("invalid symbol in array " + (basis - cur));
                    }
                }
                cur += 1;
            }
            if (!defined) {
                throw new Exception("not terminated array at " + (basis - cur));
            }
            pos = cur + 1;
            var result = new object[list.Count];
            list.CopyTo(result,0);
            return result;
        }

        private static unsafe double ReadNumber(char* cur, char* basis, out char* pos) {
            double result = 0;
            int exp = 0;
            bool negate = (*cur) == '-';
            int decimals = 0;
            bool wasdot = false;
            bool wasexp = false;
            bool negexp = false;
            char c;
            if (negate) {
                cur += 1;
                c = *cur;
                if (c == '.' || c == 'e' || c == 'E') {
                    throw new Exception("number cannot start with dot or E at " + (cur - basis));
                }
            }
            while (true) {
                c = *cur;
                if (c == 0 || ',' == c || char.IsWhiteSpace(c) || c == ']' || c == '}') {
                    if (wasdot && decimals == 0 || wasexp && exp == 0) {
                        throw new Exception("not terminated number at " + (cur - basis));
                    }
                    break;
                }

                if (c == '.') {
                    if (wasdot || wasexp) {
                        throw new Exception("illegal dot at " + (cur - basis));
                    }
                    wasdot = true;
                }
                else if (char.IsDigit(c)) {
                    if (wasexp) {
                        exp = exp*10 + (c - '0');
                    }
                    else if (wasdot) {
                        decimals++;
                        result = result + (c - '0')/Math.Pow(10, decimals);
                    }
                    else {
                        result = result*10 + (c - '0');
                    }
                }
                else if (c == 'e' || c == 'E') {
                    if (wasexp) {
                        throw new Exception("illegal exp at " + (cur - basis));
                    }
                    var next = *(cur + 1);
                    if (next == '-') {
                        negexp = true;
                        cur += 1;
                    }
                    else if ('+' == next) {
                        cur += 1;
                    }
                    else if (!char.IsDigit(next)) {
                        throw new Exception("invalid exponent at " + (cur - basis));
                    }
                    wasexp = true;
                }
                else {
                    throw new Exception("illegal char at " + (cur - basis));
                }
                cur += 1;
            }

            if (negate) {
                result = -result;
            }
            if (wasexp) {
                if (negexp) {
                    exp = -exp;
                }
                result = result*Math.Pow(10, exp);
            }
            pos = cur;
            return result;
        }

        [ThreadStatic]
        private static StringBuilder buffer;
        private static unsafe string ReadString(char* cur, char* basis, out char* pos) {
            var b = buffer;
            b.Clear();
            bool closed = false;
            while (true) {
                cur += 1;
                var c = *cur;
                if (c == 0) {
                    break;
                }

                if (c == '"') {
                    closed = true;
                    break;
                }

                if (c == '\r' || c == '\n' || c == '\b' || c == '\f') {
                    throw new Exception("invalid char in string at " + (basis - cur));
                }

                if (c == '\\') {
                    cur += 1;
                    var next = *cur;
                    if (next == '\\') {
                        b.Append('\\');
                    }
                    else if (next == '"') {
                        b.Append('"');
                    }
                    else if (next == 'r') {
                        b.Append('\r');
                    }
                    else if (next == 'n') {
                        b.Append('\n');
                    }
                    else if (next == 'b') {
                        b.Append('\b');
                    }
                    else if (next == 't') {
                        b.Append('\t');
                    }
                    else if (next == 'f') {
                        b.Append('\f');
                    }
                    else if (next == '/') {
                        b.Append('/');
                    }
                    else if (next == 'u') {
                        var code = 0;
                        for (var i = 0; i < 4; i++) {
                            cur += 1;
                            c = *cur;
                            if (char.IsDigit(c)) {
                                code = code*16 + (c - '0');
                            }
                            else if (c == 'a' || c == 'b' || c == 'c' || c == 'd' || c == 'e' || c == 'f') {
                                code = code*16 + (c - 'a' + 10);
                            }
                            else if (c == 'A' || c == 'B' || c == 'C' || c == 'D' || c == 'E' || c == 'F') {
                                code = code*16 + (c - 'A' + 10);
                            }
                            else {
                                throw new Exception("invalid hex char code at " + (cur - basis));
                            }
                        }
                        b.Append((char) code);
                    }
                    else {
                        throw new Exception("invalid escape at " + (cur - basis));
                    }
                }
                else {
                    b.Append(c);
                }
            }
            pos = cur + 1;
            if (!closed) {
                throw new Exception("not terminated string at " + (cur - basis));
            }
            return  b.ToString();
        }

        /// <summary>
        /// Преобразует объект в строку JSON
        /// </summary>
        /// <param name="data"></param>
        /// <param name="defaultMode"></param>
        /// <param name="annotator"></param>
        /// <returns></returns>
        public static string Stringify(object data, string jsonmode="", SerializeMode defaultMode = SerializeMode.Serialize, ISerializationAnnotator annotator=null, bool pretty = false, int level=0) {
            var sw = new StringWriter();
            Write(data,sw,jsonmode,defaultMode, annotator,pretty,level);
            return sw.ToString();
        }

        public static string stringify(this object data, string jsonmode="", bool pretty = false, int level = 0) {
            return Stringify(data,jsonmode,pretty:pretty,level:level);
        }

        public static IDictionary<string, object> jsonifymap(this object data) {
            var result = data.jsonify();
            if (null == result) return null;
            if (result is IDictionary<string, object>) return (IDictionary<string, object>) result;
            if (result is object[])return new Dictionary<string, object>{{"items",result}};
            return new Dictionary<string, object>{{"value",result}};
        } 
        public static object jsonify(this object data) {
            if (data is string) {
                var s = data as string;
                if (s.StartsWith("<'")) {
                    s = s.Replace("<", "{").Replace("'", "\"").Replace(">", "}");
                }
                return Parse(s);
            }
            if (data is XElement) {
                return JsonifyXml((XElement) data);
            }
            if (data is Exception) {
                return JsonifyException(data as Exception);
            }
            return Jsonify(data);
        }

        public static IDictionary<string,object> JsonifyException(Exception exception) {
            var result = new Dictionary<string, object> {
                ["type"] = exception.GetType().Name,
                ["message"] = exception.Message,
                ["stack"] = exception.StackTrace
            };
            var agg = exception as AggregateException;
            if (null != agg) {
                result["exceptions"] = agg.InnerExceptions.Select(JsonifyException).OfType<object>().ToArray();
            }
            if (null != exception.InnerException) {
                result["innerexception"] = JsonifyException(exception.InnerException);
            }
            return result;
        }

        private static object JsonifyXml(XElement data) {
            var result = new Dictionary<string, object>();
            foreach (var attribute in data.Attributes()) {
                result[attribute.Name.LocalName] = attribute.Value;
            }
            result["__name"] = data.Name.LocalName;
            var value = string.Join("\r\n", data.Nodes().OfType<XText>().Select(_ => _.Value));
            if (!string.IsNullOrWhiteSpace(value)) {
                result["__value"] = value;
            }
            var elements = data.Elements().ToArray();
            if (elements.Length > 0) {
                var _elements = elements.Select(JsonifyXml).ToArray();
                result["__elements"] = _elements;
            }
            return result;
        }

        public static IEnumerable<object> sel(this object data,string path) {
            return Select(data, path);
        }

        public static string str(this object data, string path) {
            if (null == data) return null;
            var result = Get(data, path);
            string r = null;
            if (null != result)
            {
                r = result.ToStr();
            }
            return r;
        }


        public static IDictionary<string, object> renameKeys(this IDictionary<string, object> dict,
            Func<string, object, string, string> renamer, string path = "") {
            foreach (var p  in dict.ToArray()) {
                var k = p.Key;
                var newkey = renamer(k, p.Value, path);
                if (k != newkey) {
                    dict[newkey] = p.Value;
                    dict.Remove(k);
                }
                var subdict = p.Value as IDictionary<string, object>;
                if (null != subdict) {
                    subdict.renameKeys(renamer, path == "" ? newkey : (path + "." + newkey));
                }
                var subarr = p.Value as object[];
                if (null != subarr) {
                    var i = -1;
                    foreach (var o in subarr) {
                        i++;
                        var m = o as Map;
                        if (null != m) {
                            m.renameKeys(renamer,
                                path == "" ? (newkey + "[" + i + "]") : (path + "." + newkey + "[" + i + "]"));
                        }
                    }
                }
            }
            return dict;
        }


        

        public static string resolvestr(this object data, string path, params string[] paths) {
            var result = Get(data, path);
            string r = null;
            if (null != result) {
                r = result.ToStr();
            }
            if (!string.IsNullOrWhiteSpace(r)) return r;
            if (null == paths || 0 == paths.Length) return r;
            var res = paths.Select(_ => Get(data, _)).FirstOrDefault(_ => null != _);
            if (null == res) return null;
            return res.ToStr();
        }

        public static int resolvenum(this object data, string path, params string[] paths)
        {
            var result = Get(data, path);
            int r = 0;
            if (null != result) {
                return result.ToInt();
            }
            if (null == paths || 0 == paths.Length) return r;

            return paths.Select(_ => Get(data, _)).FirstOrDefault(_ => null != _).ToInt();

        }

        public static decimal resolvedec(this object data, string path, params string[] paths)
        {
            var result = Get(data, path);
            int r = 0;
            if (null != result)
            {
                return result.ToDecimal();
            }
            if (null == paths || 0 == paths.Length) return r;

            return paths.Select(_ => Get(data, _)).FirstOrDefault(_ => null != _).ToDecimal();

        }



        public static T obj<T>(this object data, string path) where T : class, IJsonDeserializable, new() {
            var _o = Get(data, path) as IDictionary<string,object>;
            if (null == _o) {
                return null;
            }
            var result = new T();
            result.LoadFromJson(_o);
            return result;
        }



        public static int num(this object data, string path)
        {
            return Get(data, path).ToInt();
        }
        public static decimal dec(this object data, string path)
        {
            return Get(data, path).ToDecimal();
        }
        public static bool bul(this object data, string path)
        {
            return Get(data, path).ToBool();
        }
        public static bool bul(this object data, string path, bool def) {
            var res = Get(data, path);
            if (null == res) return def;
            return res.ToBool();
        }
        public static DateTime date(this object data, string path)
        {
            return Get(data, path).ToDate(true);
        }
        public static object get(this object data, string path)
        {
            return Get(data, path);
        }
        public static T get<T>(this object data, string path)
        {
            var result= Get(data, path);
            if (null == result) return default(T);
            if (typeof (IJsonDeserializable).IsAssignableFrom(typeof (T))) {
                T _result = Activator.CreateInstance<T>();
                ((IJsonDeserializable)_result).LoadFromJson(result);
                return _result;
            }
            return result.To<T>();
        }

        public static T jto<T>(this object data) {
            if (typeof(IJsonDeserializable).IsAssignableFrom(typeof(T))) {
                var _result = Activator.CreateInstance<T>();
                ((IJsonDeserializable)_result).LoadFromJson(data);
                return _result;
            }
            return data.To<T>();
        }

        public static object[] arr(this object data, string path) {
            if (null == data) return null;
            return (Get(data, path) as object[]);
        }

        public static T[] arr<T>(this object data, string path)
        {
            if (null == data) return new T[] { };
            var result = (Get(data, path) as object[]);
            if (null == result) return new T[] {};
            IList<T> _result = new List<T>();
            if (typeof (IJsonDeserializable).IsAssignableFrom(typeof (T))) {
                foreach (var o in result) {
                    var _item = (IJsonDeserializable)Activator.CreateInstance<T>();
                    _item.LoadFromJson(o);
                    _result.Add((T)_item);
                }
            }
            else {
                foreach (var o in result) {
                    _result.Add(o.To<T>());
                }
            }
            return _result.ToArray();

        }

        public static IDictionary<string,object>[] arrobj(this object data, string path) {
            var result = (Get(data, path) as object[]);
            if(null==result)return null;
             return result.OfType<IDictionary<string,object>>().ToArray();
        }

        public static IDictionary<string, object> nestorself(this object obj, string name)
        {
            return (Get(obj, name) ?? obj) as IDictionary<string, object>;
        }

        public static IDictionary<string, object> nest(this object obj, string name)
        {
            return Get(obj, name) as IDictionary<string, object>;
        }

        public static object arr0(this object data, string path) {
            if (null == data) return null;
            var _arr = (Get(data, path) as object[]);
            if (null == _arr || 0 == _arr.Length) return null;
            return _arr[0];
        }

        public static string arr0s(this object data, string path) {
            if (null == data) return null;
            var _arr = (Get(data, path) as object[]);
            if (null == _arr || 0 == _arr.Length) return null;
            return _arr[0].ToStr();
        }

        public static Map map(this object data, string path) {
            if (null == data) return null;
            return Get(data, path) as Map;
        }

        public static object clone(this object data) {
            if (data is Map) {
                var result = new Dictionary<string, object>();
                foreach (var p in (Map)data) {
                    result[p.Key] = p.Value.clone();
                }
                return result;
            }
            if (data is Arr) {
                var a = data as object[];
                var result = new object[a.Length];
                for (var i = 0; i < a.Length; i++) {
                    result[i] = a[i].clone();
                }
                return result;
            }
            return data;
        }

        public static bool has(this object data, string name) {
            if (data is IDictionary<string, object>) {
                return ((IDictionary<string, object>) data).ContainsKey(name);
            }
            return false;
        }

        /// <summary>
        /// Записывает обьект в поток в формате JSON
        /// </summary>
        /// <param name="data"></param>
        /// <param name="output"></param>
        /// <param name="jsonmode"></param>
        /// <param name="defaultMode"></param>
        /// <param name="annotator"></param>
        /// <param name="pretty"></param>
        /// <param name="level"></param>
        public static void Write(object data, TextWriter output, string jsonmode, SerializeMode defaultMode = SerializeMode.Serialize, ISerializationAnnotator annotator = null, bool pretty = false, int level = 0)
        {
            if (null == data) {
                output.Write("null");
                return;
            }
            if (data is string) {
                WriteString(data as string, output);
                return;
            }
            var convertible = data as IConvertible;
            if (null != convertible) {
                WriteValue(data, output);
                return;
            }

            var collection = data as ICollection;
            if (null != collection) {
                if (collection is IDictionary<string, string>) {
                    WriteObject(collection as IDictionary<string, string>, output,jsonmode,defaultMode, annotator,pretty,level);
                }
                else if (collection is IDictionary<string, object>) {
                    WriteObject(collection as IDictionary<string, object>, output,jsonmode,defaultMode, annotator,pretty, level);
                }
                else if (collection is IDictionary<int, object>)
                {
                    WriteObject(collection as IDictionary<int, object>, output,jsonmode,defaultMode, annotator,pretty, level);
                }
                else {
                    WriteArray(collection, output,jsonmode,defaultMode, annotator,pretty, level);
                }
                return;
                
            }
            var error = data as Exception;
            if (null != error) {
                if (error is IJsonSerializable) {
                    WriteObject(data, output, defaultMode, jsonmode, annotator);
                }
                else {
                    WriteObject(
                    new{
                        type=error.GetType().Name,
                        message = error.Message,
                        stack = error.StackTrace,
                        inner = error.InnerException,
                        exceptions = (error as AggregateException)?.InnerExceptions
                    }, output, defaultMode, jsonmode, annotator);
                }
                return;
            }

            WriteObject(data, output,defaultMode, jsonmode,annotator);
        }

        private static void WriteObject(object data, TextWriter output, SerializeMode defaultMode, string jsonmode, ISerializationAnnotator annotator)
        {
            
            if (data is IJsonSerializable) {
                (data as IJsonSerializable).WriteAsJson(output,jsonmode, annotator);
            }
            else {
                output.Write("{");
                bool first = true;
                var properties = data.GetType().GetProperties().OrderBy(_=>_.Name);
                if (null != annotator) {
                    first = !annotator.Prepend(data, output);
                }
                foreach (var property in properties) {
                    if (null == property.GetGetMethod()) continue;
                    if (property.GetIndexParameters().Length != 0) continue;
                    var mode = null == annotator ? defaultMode : annotator.GetMode(data, property);
                    if (mode == SerializeMode.Unknown) {
                        mode = defaultMode;
                    }
                    if (mode == SerializeMode.None) {
                        continue;
                    }
                    
                    var val = property.GetValue(data);
                    if (mode.HasFlag( SerializeMode.OnlyNotNull ) && !TypeConverter.ToBool(val)) {
                        continue;
                    }
                    if (!first) {
                        output.Write(",");
                    }
                    else {
                        first =false;
                    }
                    var name = property.Name;
                    if (mode.HasFlag(SerializeMode.LowerCase)) {
                        name = name.ToLowerInvariant();
                    }
                    WriteString(name,output);
                    output.Write(":");
                    Write(val,output,jsonmode,defaultMode,annotator);
                }
                output.Write("}");
            }
        }

        private static void WriteArray(ICollection collection, TextWriter output, string jsonmode, SerializeMode defaultMode, ISerializationAnnotator annotator, bool pretty = false, int level=0)
        {
           output.Write("[");
            bool first = true;
            foreach (var i in collection) {
                if (!first) {
                    output.Write(",");
                }
                else {
                    first = false;
                }
                Write(i,output,jsonmode,defaultMode,annotator);
            }
            output.Write("]");
        }

        private static void WriteObject<T, TV>(IDictionary<T, TV> dict, TextWriter output, string jsonmode, SerializeMode defaultMode, ISerializationAnnotator annotator, bool pretty = false, int level =0)
        {
            output.Write("{");
            bool first = true;
            foreach (var i in dict)
            {
                if (!first)
                {
                    output.Write(",");
                }
                else
                {
                    first = false;
                }
                WriteString(i.Key.ToString(),output);
                output.Write(":");
                Write(i.Value,output,jsonmode,defaultMode,annotator);
            }
            output.Write("}");
        }


        private static void WriteValue(object data, TextWriter output) {
            if (data is DateTime) {
                output.Write("\"");
                var d = (DateTime) data;
                var utcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(d);
                output.Write(
                    d.ToString("yyyy-MM-ddTHH:mm:ss" + ((utcOffset < TimeSpan.Zero) ? "-" : "+") +
                               utcOffset.ToString("hhmm")));
                output.Write("\"");
            }
            else if (data is bool) {
                var b = (bool) data;
                output.Write(b ? "true" : "false");
            }
            else if (data is Enum) {
                output.Write("\"");
                output.Write(data.ToString());
                output.Write("\"");
            }
            else if (data is int) {
                output.Write(data.ToString());
            }
            else if (data is long) {
                var l = (long) data;
                if (l < int.MinValue || l > int.MaxValue) {
                    output.Write("\"");
                    output.Write(l.ToString());
                    output.Write("\"");
                }
                else {
                    output.Write(l.ToString());
                }
            }
            else {
                
                output.Write((Convert.ToDouble( data)).ToString(CultureInfo.InvariantCulture));
            }
        }

        private static char[] escaperequire = new[] {'\\', '\r', '\t', '\n', '\f', '\b', '"'};
        private static void WriteString(string data, TextWriter output) {
            output.Write("\"");
            if (-1 == data.IndexOfAny(escaperequire)) {
                output.Write(data);
            }
            else {

                foreach (var c in data) {
                    if (c == '\r') {
                        output.Write("\\r");
                    }
                    else if (c == '\n') {
                        output.Write("\\n");
                    }
                    else if (c == '\t') {
                        output.Write("\\t");
                    }
                    else if (c == '\f') {
                        output.Write("\\f");
                    }
                    else if (c == '\b') {
                        output.Write("\\b");
                    }
                    else if (c == '\\') {
                        output.Write("\\\\");
                    }
                    else if (c == '"') {
                        output.Write("\\\"");
                    }
                    else {
                        output.Write(c);
                    }
                }
            }
            output.Write("\"");
        }

        public static IEnumerable<NodeRef> collect(this object json, Func<object , string , NodeFilter> filter ) {
            var context = "/";
            foreach (var n in internalCollect(json,"/",filter)) {
                yield return n;
            }
        }

        private static IEnumerable<NodeRef> internalCollect(object json, string context, Func<object, string, NodeFilter> filter) {
            var result = filter(json, context);
            if (result.HasFlag(NodeFilter.Return)) {
                yield return new NodeRef {Path = context, Value = json};
            }
            if (!result.HasFlag(NodeFilter.Stop)) {
                if (json is Map) {
                    var m = json as Map;
                    foreach ( var p in m) {
                        foreach (var r in internalCollect(p.Value,context+"/"+p.Key,filter)) {
                            yield return r;
                        }
                    }
                }else if (json is Arr) {
                    var a = (json as Arr).OfType<object>().ToArray();
                    for (var i = 0; i < a.Length; i++) {
                        var val = a[i];
                        foreach (var r in internalCollect(val,context+"["+i+"]",filter)) {
                            yield return r;
                        }
                    }
                }
            }
        }
    }

    [Flags]
    public enum NodeFilter {
        None=0,
        Stop=1,
        Return=2
    }

    public class NodeRef {
        public string Path { get; set; }
        public object Value { get; set; }
    }

}