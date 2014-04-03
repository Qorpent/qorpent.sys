using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Xml;
using System.Xml.Linq;
using Qorpent.Json;
using Qorpent.Serialization;

namespace Qorpent.Uson
{
	/// <summary>
	/// Расширения для USon
	/// </summary>
	public static class UsonExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static UObj ToUson(this object obj)
		{
			if (obj is UObj) return (UObj) obj;
			if (obj is XElement){
				return XmlToUson((XElement) obj,true);
			}
			var result = UObjSerializerSupport.ToUson(obj) ;
			if (result is UObj) return (UObj) result;
			var real = new UObj();
			real.Properties["__value"] = obj;
			real.UObjMode = UObjMode.Value;
			return real;
		}

		/// <summary>
		/// Конвертирует XElement в UObj
		/// </summary>
		/// <param name="e"></param>
		/// <param name="root"></param>
		/// <returns></returns>
		public static UObj XmlToUson(XElement e,bool root){
			dynamic result = new UObj();
			dynamic target = result;
			if (root){
				result[e.Name.LocalName] = new object();
				target = result[e.Name.LocalName];
			}
			foreach (var attribute in e.Attributes()){
				target[attribute.Name.LocalName] = attribute.Value;
			}
			var eldict = e.Elements().GroupBy(_ => _.Name.LocalName, _ => _);
			foreach (var grp in eldict){
				foreach (var element in grp){
					var item = XmlToUson(element, false);
					target[grp.Key].push(item);
				}
			}
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="objects"></param>
		/// <returns></returns>
		public static UObj UsonDeepExtend(this object obj, params object[] objects)
		{
			return obj.ToUson().deepextend((object[]) objects);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="objects"></param>
		/// <returns></returns>
		public static UObj UsonExtend(this object obj, params object[] objects)
		{
			return obj.ToUson().extend((object[])objects);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="objects"></param>
		/// <returns></returns>
		public static UObj UsonDeepDefaults(this object obj, params object[] objects)
		{
			return obj.ToUson().deepdefaults((object[])objects);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="objects"></param>
		/// <returns></returns>
		public static UObj UsonDefaults(this object obj, params object[] objects)
		{
			return obj.ToUson().defaults((object[])objects);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="output"></param>
		/// <param name="mode"></param>
		public static void ToJson(UObj obj, TextWriter output, UObjSerializeMode mode = UObjSerializeMode.None)
		{
			if (obj.UObjMode == UObjMode.Default)
			{
				ToJsonObject(obj,output, mode);
			}
			else if (obj.UObjMode == UObjMode.Array)
			{
				ToJsonArray(obj,output, mode);
			}else if (obj.UObjMode == UObjMode.Value)
			{
				var val = obj.Properties["__value"];
				if (val != null)
				{
					ToJsonValue(output, mode, val);
				}
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="output"></param>
		/// <param name="mode"></param>
		public static void ToJsonObject(UObj obj,TextWriter output, UObjSerializeMode mode)
		{
			output.Write("{");
			bool hasprop = false;
			if (mode.HasFlag(UObjSerializeMode.KeepType) && null != obj._srctype)
			{
				output.Write("\"{0}\":\"{1}, {2}\"", "_srctype", obj._srctype.FullName, obj._srctype.Assembly.GetName().Name);
				hasprop = true;
			}
			foreach (var property in obj.Properties.OrderBy(_ => _.Key))
			{
				if (property.Value is UObj)
				{
					if (((UObj) property.Value).UObjMode == UObjMode.Fake)
					{
						continue;
					}
				}
				if (hasprop)
				{
					output.Write(",");
				}
				else
				{
					hasprop = true;
				}
				output.Write("\"" + property.Key + "\"");
				output.Write(":");
				ToJsonValue(output,mode,property.Value);
			}
			output.Write("}");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="output"></param>
		/// <param name="mode"></param>
		public static void ToJsonArray(UObj obj, TextWriter output, UObjSerializeMode mode)
		{
			output.Write("[");
			bool hasprop = false;
			foreach (var item in obj.Array)
			{
				if (item is UObj)
				{
					if (((UObj)item).UObjMode == UObjMode.Fake)
					{
						continue;
					}
				}
				if (hasprop)
				{
					output.Write(",");
				}
				else
				{
					hasprop = true;
				}
				ToJsonValue(output, mode, item);
			}
			output.Write("]");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="output"></param>
		/// <param name="mode"></param>
		/// <param name="item"></param>
		public static void ToJsonValue(TextWriter output, UObjSerializeMode mode, object item)
		{
			if (null == item)
			{
				output.Write("null");
			}
			else if (item is UObj)
			{
				ToJson(((UObj) item),output, mode);
			}
			else if (item is string)
			{
				output.Write("\"" + (item as string).Escape(EscapingType.JsonValue).Replace("\\'", "'") + "\"");
			}
			else if (item is bool)
			{
				output.Write(item.ToString().ToLower());
			}
			else if (item is decimal)
			{
				output.Write(((decimal) item).ToString("0.#####", CultureInfo.InvariantCulture));
			}
			else if (item is float)
			{
				output.Write(((float) item).ToString("0.#####", CultureInfo.InvariantCulture));
			}
			else if (item is int)
			{
				output.Write(item);
			}
			else if (item is DateTime)
			{
				output.Write("\"" + ((DateTime) item).ToString("yyyy-MM-dd hh:mm:ss") + "\"");
			}
			else
			{
				output.Write("\"" + item.ToString().Escape(EscapingType.JsonValue).Replace("\\'","'") + "\"");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		public static string ToXmlValue(object item)
		{
			if (item is string)
			{
				return (item as string).Escape(EscapingType.XmlAttribute);
			}
			else if (item is bool)
			{
				return item.ToString().ToLower();
			}
			else if (item is decimal)
			{
				return ((decimal)item).ToString("0.#####", CultureInfo.InvariantCulture);
			}
			else if (item is float)
			{
				return ((float)item).ToString("0.#####", CultureInfo.InvariantCulture);
			}
			else if (item is int)
			{
				return item.ToString();
			}
			else if (item is DateTime)
			{
				return ((DateTime)item).ToString("yyyy-MM-dd hh:mm:ss");
			}
			else
			{
				return item.ToString().Escape(EscapingType.XmlAttribute);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="parent"></param>
		/// <returns></returns>
		public static object ToUson(object obj,UObj parent = null)
		{
			if (obj == null || obj is string || obj.GetType().IsValueType)
			{
				return obj;
			}
			var result = new UObj { Parent = parent, _srctype = obj.GetType()};
			if (obj is UObj)
			{
				result.UObjMode = (obj as UObj).UObjMode;
				foreach (var p in ((UObj)obj).Properties)
				{
					result.Properties[p.Key] = ToUson(p.Value);
				}
				return result;
			}
			if (obj is JsonItem)
			{
				if (obj is JsonObject)
				{
					foreach (var p in ((JsonObject)obj).Properties)
					{
						result.Properties[p.Name.ToString()] = ToUson(p.Value);
					}	
				}
				else if (obj is JsonArray)
				{
					foreach (var p in ((JsonObject)obj).Properties)
					{
						result.Properties[p.Name.ToString()] = ToUson(p.Value);
					}
				}else if (obj is JsonValue)
				{
					var jv = (JsonValue) obj;
					if(jv.Type==JsonTokenType.String)return jv.Value;
					if(jv.Type==JsonTokenType.Number)return Decimal.Parse( jv.Value);
					if (jv.Type == JsonTokenType.Null) return null;
					if (jv.Type == JsonTokenType.Bool) return Boolean.Parse(jv.Value);
					return jv.Value;
				}
				
			}
			if (obj is Array)
			{
				result.UObjMode = UObjMode.Array;
				foreach (var item in ((IEnumerable)obj))
				{
					result.Array.Add(ToUson(item));
				}
			}else if (obj.GetType().Name.StartsWith("Dictionary"))
			{
				result.UObjMode = UObjMode.Default;
				foreach (dynamic item in ((IEnumerable)obj))
				{
					result.Properties[item.Key.ToString()] = item.Value;
				}
	
			}else if (obj.GetType().Name.StartsWith("List"))
			{
				result.UObjMode = UObjMode.Array;
				foreach (var item in ((IEnumerable)obj))
				{
					result.Array.Add(ToUson(item));
				}
			}

			else
			{
				foreach (var p in SerializableItem.GetSerializableItems(obj))
				{
					result.Properties[p.Name] = ToUson(p.Value);
				}	
			}
			
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="writer"></param>
		/// <param name="mode"></param>
		public static void WriteXml(UObj obj, XmlWriter writer, UObjSerializeMode mode)
		{
			WriteXml(obj, "result",null, writer, mode);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="elementName"></param>
		/// <param name="advAttributes"></param>
		/// <param name="writer"></param>
		/// <param name="mode"></param>
		private static void WriteXml(UObj obj, string elementName, string advAttributes, XmlWriter writer, UObjSerializeMode mode)
		{
			if(obj.UObjMode==UObjMode.Fake)return;			
			writer.WriteStartElement(elementName);
			if (!String.IsNullOrWhiteSpace(advAttributes))
			{
				writer.WriteRaw(" "+advAttributes+" ");
			}
			if (mode.HasFlag(UObjSerializeMode.KeepType) && null != obj._srctype)
			{
				writer.WriteAttributeString("_srctype",String.Format("{0}, {1}",obj._srctype.FullName,obj._srctype.Assembly.GetName().Name));
			}
			if (obj.UObjMode == UObjMode.Default)
			{
				foreach (var property in obj.Properties)
				{
					if (property.Value == null) continue;
					if (property.Value is string || property.Value.GetType().IsValueType)
					{
						writer.WriteAttributeString(property.Key, ToXmlValue(property.Value));
					}
				}
				foreach (var property in obj.Properties)
				{
					if (property.Value == null) continue;
					if (!(property.Value is string || property.Value.GetType().IsValueType))
					{
						WriteXml((UObj) property.Value, property.Key, null, writer, mode);
					}
				}
			}
			else if (obj.UObjMode == UObjMode.Array){
				writer.WriteAttributeString("_array", "true");
				foreach (var p in obj.Array){
					if (null == p){
						writer.WriteElementString("item", "");
					}
					else if (p is string || p.GetType().IsValueType){
						writer.WriteElementString("item", ToXmlValue(p));
					}
					else{
						WriteXml(p as UObj, "item", null, writer, mode);
					}
				}
			}
			else{
				writer.WriteElementString("value",obj.Properties["__value"].ToString());
			}
			writer.WriteEndElement();
		}
	}
}