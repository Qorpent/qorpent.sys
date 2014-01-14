using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qorpent.Serialization;

namespace Qorpent.Uson
{
	/// <summary>
	/// 
	/// </summary>
	[Flags]
	public enum UObjSerializeMode
	{
		/// <summary>
		/// Отсутствуют
		/// </summary>
		None = 0,
		
		/// <summary>
		/// Информация об исходном типе
		/// </summary>
		KeepType =1,

		
	}

	/// <summary>
	/// 
	/// </summary>
	[Flags]
	public enum UObjMode
	{
		/// <summary>
		/// Обычный
		/// </summary>
		Default = 0,
		/// <summary>
		/// Массив
		/// </summary>
		Array = 1,
		/// <summary>
		/// Информация об исходном типе
		/// </summary>
		Fake = 2,
	}
	/// <summary>
	/// Динамический класс для представления промежуточных объектов
	/// </summary>
	public class UObj:DynamicObject
	{
		private Type _srctype = null;
		IDictionary<string,object> _properties = new Dictionary<string, object>();
		private UObjMode _uObjMode;
		/// <summary>
		/// 
		/// </summary>
		protected UObjMode UObjMode
		{
			get
			{
				return _uObjMode;
			}
			set
			{
				if (_uObjMode == UObjMode.Fake && value == UObjMode.Default)
				{
					this.Parent.UObjMode = UObjMode.Default;
				}
				_uObjMode = value;
				
			}
		}
		/// <summary>
		/// 
		/// </summary>
		protected UObj Parent { get; set; }
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public string ToJson(UObjSerializeMode mode = UObjSerializeMode.None)
		{
			var sw = new StringWriter();
			ToJson(sw,mode);
			return sw.ToString();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="output"></param>
		/// <param name="mode"></param>
		protected void ToJson(TextWriter output,UObjSerializeMode mode = UObjSerializeMode.None)
		{
			if (this.UObjMode == UObjMode.Default)
			{
				output.Write("{");
				bool hasprop = false;
				if (mode.HasFlag(UObjSerializeMode.KeepType) && null != _srctype)
				{
					output.Write("\"{0}\":\"{1}, {2}\"", "_srctype", _srctype.FullName, _srctype.Assembly.GetName().Name);
					hasprop = true;
				}
				foreach (var property in _properties.OrderBy(_=>_.Key))
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
					output.Write("\""+property.Key+"\"");
					output.Write(":");
					if (null == property.Value)
					{
						output.Write("null");
					}
					else if (property.Value is UObj)
					{
						((UObj) property.Value).ToJson(output, mode);
					}
					else if( property.Value is string)
					{
						output.Write("\""+(property.Value as string).Escape(EscapingType.JsonValue)+"\"");
					}else if (property.Value is bool)
					{
						output.Write(property.Value.ToString().ToLower());
					}
					else if(property.Value is decimal)
					{
						output.Write(((decimal)property.Value).ToString("0.#####",CultureInfo.InvariantCulture));
					}else if (property.Value is float)
					{
						output.Write(((float) property.Value).ToString("0.#####", CultureInfo.InvariantCulture));
					}
					else if (property.Value is int)
					{
						output.Write(property.Value);
					}
					else if (property.Value is DateTime)
					{
						output.Write("\""+((DateTime)property.Value).ToString("yyyy-MM-dd hh:mm:ss")+"\"");
					}
					else
					{
						output.Write("\""+property.Value.ToString().Escape(EscapingType.JsonValue)+"\"");
					}
				}
				output.Write("}");
			}
			else
			{
				throw new NotImplementedException();
			}

		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="binder"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			
			if (!_properties.ContainsKey(binder.Name))
			{
				_properties[binder.Name] = new UObj {UObjMode = UObjMode.Fake, Parent = this};
			}
			result = _properties[binder.Name];
			return true;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="binder"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			var res = ( _properties[binder.Name] = ToUson(value)) as UObj;
			if (null != res)
			{
				res.UObjMode = UObjMode.Default;
			}
			else
			{
				this.UObjMode = UObjMode.Default;
			}
			return true;
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
				foreach (var p in ((UObj)obj)._properties)
				{
					result._properties[p.Key] = ToUson(p.Value);
				}
				return result;
			}
			foreach (var p in SerializableItem.GetSerializableItems(obj))
			{
				result._properties[p.Name] = ToUson(p.Value);
			}
			return result;
		}
	}
}
