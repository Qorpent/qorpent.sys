using System;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Json;

namespace Qorpent.Serialization
{
    /// <summary>
    /// Осуществляет преобразование из XML в JSON
    /// </summary>
    public class XmlToJsonConverter {
        private JsonSerializer _baseSerializer;
        private JsonParser _parser;

        private JsonSerializer BaseSerializer {
            get { return _baseSerializer??(_baseSerializer =new JsonSerializer()); }
        }

        private JsonParser Parser {
            get { return _parser ?? (_parser = new JsonParser()); }
        }

	    /// <summary>
	    /// Осуществляет преобразование из XElement в JSON
	    /// </summary>
	    /// <param name="xml"></param>
	    /// <param name="format"></param>
	    /// <returns></returns>
	    public string ConvertToJson(XElement xml,bool format =true) {
	        var result = ConvertToJsonItem(xml);
            return result.ToString(format);
        }

        /// <summary>
        /// Выполняет конвертацию из XML в аналогиченый JSON - элемент
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public JsonItem ConvertToJsonItem(XElement xml) {
            if (xml.Attribute("__isarray") != null) {
                if (xml.Attribute("__isarray").Value == "true") {
                    xml.SetAttributeValue(JsonItem.JsonTypeAttributeName, "array");
                }
            }

            if (null == xml.Attribute(JsonItem.JsonTypeAttributeName)) {
                var json = BaseSerializer.Serialize("root", xml);
                var result = Parser.Parse(json);
				foreach (var n in result.CollectAllValues()) {
					n.Type = JsonTokenType.Auto;
				}
	            return result;
            }
            if (IsValue(xml)) {
                return new JsonValue(xml);
            }else if (IsArray(xml)) {
                return ConvertToJsonArray(xml);
            }
            else if (IsObject(xml))
            {
                return ConvertToJsonObject(xml);
            }
            return null;
        }

        private bool IsObject(XElement xml) {
            return true;
        }

        private bool IsArray(XElement xml) {
            if (null != xml.Attribute(JsonItem.JsonTypeAttributeName))
            {
                var type = xml.Attribute(JsonItem.JsonTypeAttributeName).Value;
                return type == "array";
            }
            return null != xml.Attribute("__isarray");
        }

        private JsonObject ConvertToJsonObject(XElement xml) {
            var result = new JsonObject();
            foreach (var a in xml.Attributes()) {
                if(a.Name.LocalName==JsonItem.JsonTypeAttributeName)continue;
                if(a.Name.LocalName=="__isarray")continue;
                if(a.Name.LocalName=="_file")continue;
                if(a.Name.LocalName=="_line")continue;
                    result[a.Name.LocalName] = new JsonValue {Value = a.Value.Escape(EscapingType.JsonValue), Type = JsonTokenType.Auto};
            }
            foreach (var e in xml.Elements()) {
                result[e.Name.LocalName] = ConvertToJsonItem(e);
            }
            return result;
        }

        private JsonArray ConvertToJsonArray(XElement xml) {
            var result = new JsonArray();
            foreach (var a in xml.Elements())
            {
                result.Values.Add(ConvertToJsonItem(a));
            }
            return result;
        }

        private bool IsValue(XElement xml) {
            if (null != xml.Attribute(JsonItem.JsonTypeAttributeName)) {
                var type = xml.Attribute(JsonItem.JsonTypeAttributeName).Value;
                return type != "object" && type != "array";
            }
            if (xml.HasElements) return false;
            if (xml.Attributes().Any(_ => _.Name.LocalName[0] != '_')) return false;
            return true;
        }
        
    }
}
