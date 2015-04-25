using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Qorpent.Utils;

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

        private static object GetInternal(object json, string[] pathParts) {
            var current = json;
            foreach(var pp in pathParts) {
                current = GetInternal(current, pp);
                if (null == current) return null;
            }
            return current;
        }
        private static object GetInternal(object json, string pathPart) {
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
            return null;
        }

        /// <summary>
        /// Retrives null, object[] or IDictionary&lt;string,object&gt;
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static unsafe object Parse(string json) {
            if (!string.IsNullOrWhiteSpace(json)) {
                fixed (char* j = json) {
                    var cur = j;
                    return ReadDocument(cur, j);
                }
            }
            return null;
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

        private static unsafe string ReadString(char* cur, char* basis, out char* pos) {
            var buffer = new StringBuilder();
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
                        buffer.Append('\\');
                    }
                    else if (next == '"') {
                        buffer.Append('"');
                    }
                    else if (next == 'r') {
                        buffer.Append('\r');
                    }
                    else if (next == 'n') {
                        buffer.Append('\n');
                    }
                    else if (next == 'b') {
                        buffer.Append('\b');
                    }
                    else if (next == 't') {
                        buffer.Append('\t');
                    }
                    else if (next == 'f') {
                        buffer.Append('\f');
                    }
                    else if (next == '/') {
                        buffer.Append('/');
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
                        buffer.Append((char) code);
                    }
                    else {
                        throw new Exception("invalid escape at " + (cur - basis));
                    }
                }
                else {
                    buffer.Append(c);
                }
            }
            pos = cur + 1;
            if (!closed) {
                throw new Exception("not terminated string at " + (cur - basis));
            }
            return buffer.ToString();
        }

        /// <summary>
        /// Преобразует объект в строку JSON
        /// </summary>
        /// <param name="data"></param>
        /// <param name="defaultMode"></param>
        /// <param name="annotator"></param>
        /// <returns></returns>
        public static string Stringify(object data, SerializeMode defaultMode = SerializeMode.Serialize, ISerializationAnnotator annotator=null) {
            var sw = new StringWriter();
            Write(data,sw,defaultMode, annotator);
            return sw.ToString();
        }

        /// <summary>
        /// Записывает обьект в поток в формате JSON
        /// </summary>
        /// <param name="data"></param>
        /// <param name="output"></param>
        /// <param name="defaultMode"></param>
        /// <param name="annotator"></param>
        public static void Write(object data, TextWriter output, SerializeMode defaultMode = SerializeMode.Serialize, ISerializationAnnotator annotator = null)
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
                    WriteObject(collection as IDictionary<string, string>, output,defaultMode, annotator);
                }
                else if (collection is IDictionary<string, object>) {
                    WriteObject(collection as IDictionary<string, object>, output,defaultMode, annotator);
                }
                else if (collection is IDictionary<int, object>)
                {
                    WriteObject(collection as IDictionary<int, object>, output,defaultMode, annotator);
                }
                else {
                    WriteArray(collection, output,defaultMode, annotator);
                }
                return;
                
            }

            WriteObject(data, output,defaultMode, annotator);
        }

        private static void WriteObject(object data, TextWriter output, SerializeMode defaultMode, ISerializationAnnotator annotator)
        {
            if (data is IJsonSerializable) {
                (data as IJsonSerializable).Write(output, annotator);
            }
            else {
                output.Write("{");
                bool first = true;
                var properties = data.GetType().GetProperties();
                foreach (var property in properties) {
                    var mode = null == annotator ? defaultMode : annotator.GetMode(data, property);
                    if (mode == SerializeMode.None) {
                        continue;
                    }
                    var val = property.GetValue(data);
                    if (mode != SerializeMode.Serialize && !TypeConverter.ToBool(val)) {
                        continue;
                    }
                    if (!first) {
                        output.Write(",");
                    }
                    else {
                        first =false;
                    }
                    WriteString(property.Name,output);
                    output.Write(":");
                    Write(val,output,defaultMode,annotator);
                }
                output.Write("}");
            }
        }

        private static void WriteArray(ICollection collection, TextWriter output, SerializeMode defaultMode, ISerializationAnnotator annotator)
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
                Write(i,output,defaultMode,annotator);
            }
            output.Write("]");
        }

        private static void WriteObject<T, TV>(IDictionary<T, TV> dict, TextWriter output, SerializeMode defaultMode, ISerializationAnnotator annotator)
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
                Write(i.Value,output,defaultMode,annotator);
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
            else {
                output.Write((Convert.ToDouble( data)).ToString(CultureInfo.InvariantCulture));
            }
        }

        private static void WriteString(string data, TextWriter output) {
            output.Write("\"");
            foreach (var c in data) {
                if (c == '\r') {
                    output.Write("\\r");
                }
                else if (c == '\n') {
                    output.Write("\\n");
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
            output.Write("\"");
        }
    }
}