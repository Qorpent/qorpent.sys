using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Qorpent.Json;
using Qorpent.Utils.Extensions;

namespace Qorpent.Uson
{
	/// <summary>
	/// Динамический класс для представления промежуточных объектов
	/// </summary>
	public sealed class UObj:DynamicObject,IEnumerable<KeyValuePair<string, object>>
	{
		
		/// <summary>
		/// /
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode(){
			unchecked{
				int hashCode = (_srctype != null ? _srctype.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ (_properties != null ? _properties.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ (_array != null ? _array.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ (int) _uObjMode;
				hashCode = (hashCode*397) ^ (Parent != null ? Parent.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ IgnoreCase.GetHashCode();
				return hashCode;
			}
		}
		/// <summary>
		/// Возвращает свойства как словарь
		/// </summary>
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator(){
			return GetEnumerator();
		}

		internal Type _srctype = null;
		private IDictionary<string, object> _properties;
		private IList<object> _array; 
		private UObjMode _uObjMode;
		/// <summary>
		/// Возвращает свойства как словарь
		/// </summary>
		/// <returns></returns>
		public IEnumerator<KeyValuePair<string, object>> GetEnumerator(){
			if (UObjMode == UObjMode.Fake){
				yield break;
			}
			else if (UObjMode == UObjMode.Value){
				yield return new KeyValuePair<string, object>("value", Properties["__value"]);
			}
			else if (UObjMode == UObjMode.Array){
				for(var i=0;i<this.Array.Count;i++){
					yield return new KeyValuePair<string, object>("i" + i, Array[i]);
				}
			}else if (UObjMode == UObjMode.Default){
				foreach (var property in Properties){
					yield return new KeyValuePair<string, object>(property.Key,property.Value);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj){
			if (null == obj) return this.UObjMode == UObjMode.Fake || (this.UObjMode == UObjMode.Value && this.Properties["__value"] == null);
			if (!(obj is UObj)) return this.UObjMode == UObjMode.Value && this.Properties["__value"] != null && this.Properties["__value"].Equals(obj);
			var uobj = obj as UObj;
			if (uobj.UObjMode != this.UObjMode) return false;
			if (uobj.UObjMode == UObjMode.Default){
				if (Properties.Count != uobj.Properties.Count) return false;
				foreach (var property in Properties){
					if (!uobj.Properties.ContainsKey(property.Key)) return false;
					var val = uobj.Properties[property.Key];
					var myval = property.Value;
					if(null==val && null==myval)continue;
					if (null == val || null == myval) return false;
					if (!val.Equals(myval)) return false;
				}
			}
			else if (uobj.UObjMode == UObjMode.Array)
			{
				if (Array.Count != uobj.Array.Count) return false;
				for (var i = 0; i < Array.Count; i++){
					var val = uobj.Array[i];
					var myval = Array[i];
					if (null == val && null == myval) continue;
					if (null == val || null == myval) return false;
					if (!val.Equals(myval)) return false;
				}
			}else if (uobj.UObjMode == UObjMode.Value){
				var val = uobj.Properties["__value"];
				var myval = Properties["__value"];
				if (null == val && null == myval) return true;
				if (null == val || null == myval) return false;
				if (!val.Equals(myval)) return false;
			}
			return true;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public UObjMode UObjMode
		{
			get
			{
				var result =  _uObjMode;
			    if (result == UObjMode.Default) {
			    	if(null!=_srctype){
				        if (_srctype == typeof (JsonArray)) {
				            return UObjMode.Array;
	                    } else if (_srctype.IsValueType) {
	                        return UObjMode.Value;
	                    }
                	}
			    }
			    return result;
			}
			set
			{
				if (this.Parent!=null && this.Parent.UObjMode == UObjMode.Fake && value != UObjMode.Fake)
				{
					this.Parent.UObjMode = value;
				}
				var v = value;
				if (v != UObjMode.Fake && !IsFakeAble())
				{
					if (v == UObjMode.Default && this._array != null && this._array.Count != 0)
					{
						v = UObjMode.Array;
					}else if (v == UObjMode.Array && this._properties != null && this._properties.Count != 0)
					{
						v = UObjMode.Default;
					}
				}
				_uObjMode = value;
				
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="uobj"></param>
		/// <returns></returns>
		public static implicit operator bool(UObj uobj)
		{
			if (null == uobj) return false;
			if (uobj.UObjMode == UObjMode.Fake) return false;
			if (uobj.UObjMode == UObjMode.Array) return uobj.Array!=null && uobj.Array.Count!=0;
			if (uobj.UObjMode == UObjMode.Default) return true;
			if (uobj.UObjMode == UObjMode.Value)
			{
				return uobj.Properties["__value"].ToBool();
			}
			return true;
		}


		
		

		/// <summary>
		/// 
		/// </summary>
		public UObj Parent { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public IDictionary<string, object> Properties
		{
			get { return _properties ?? (_properties = new Dictionary<string, object>()); }
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool IsFakeAble()
		{
			if ((_properties == null||_properties.Count==0) && (_array == null||0==_array.Count)) return true;
			if (null != _properties)
			{
				if (_properties.Values.Any(_ => !(_ is UObj) || !(((UObj) _).IsFakeAble()))) return false;
			}
			if (null != _array)
			{
				if (_array.Any(_ => !(_ is UObj) || !(((UObj)_).IsFakeAble()))) return false;
			}
			return true;
		}
		/// <summary>
		/// 
		/// </summary>
		public IList<object> Array
		{
			get { return _array ?? (_array = new List<object>()); }
		}
        /// <summary>
        /// 
        /// </summary>
	    public bool IgnoreCase { get; set; }

	    /// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public string ToJson(UObjSerializeMode mode = UObjSerializeMode.None)
		{
			var sw = new StringWriter();
			UsonExtensions.ToJson(this,sw,mode);
			return sw.ToString();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mode"></param>
		/// <returns></returns>
		public string ToXmlString(UObjSerializeMode mode = UObjSerializeMode.None)
		{
			var sw = new StringWriter();
			var xs = new XmlWriterSettings();
			xs.OmitXmlDeclaration = true;
			var xw = XmlWriter.Create(sw,xs);
			WriteXml(xw,mode);
			xw.Flush();
			return sw.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mode"></param>
		/// <returns></returns>
		public XElement ToXElement(UObjSerializeMode mode = UObjSerializeMode.None)
		{
			var sw = new StringWriter();
			var xw = XmlWriter.Create(sw);
			WriteXml(xw, mode);
			xw.Flush();
			return XElement.Parse(sw.ToString());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public void WriteXml(XmlWriter writer, UObjSerializeMode mode = UObjSerializeMode.None)
		{
			UsonExtensions.WriteXml(this, writer, mode);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="binder"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
		    if (UObjMode==UObjMode.Array && binder.Name == "length") {
		        result = Array.Count;
                return true;
		    }
			if (!Properties.ContainsKey(binder.Name))
			{
                if (IgnoreCase) {
                    var p = Properties.FirstOrDefault(_ => _.Key.ToLower() == binder.Name.ToLower());
                    if (!string.IsNullOrWhiteSpace(p.Key)) {
                        result = p.Value;
                        return true;
                    }
                }
				Properties[binder.Name] = new UObj {UObjMode = UObjMode.Fake, Parent = this};
			}
            
			result = Properties[binder.Name];
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
			var res = ( Properties[binder.Name] = UObjSerializerSupport.ToUson(value)) as UObj;
			if (null != res)
			{
				if (res.UObjMode == UObjMode.Fake)
				{
					res.UObjMode = UObjMode.Default;	
				}
				
			}
			else
			{
				this.UObjMode = UObjMode.Default;
			}
			return true;
		}

		/// <summary>
		/// /
		/// </summary>
		/// <param name="binder"></param>
		/// <param name="indexes"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
		{
			result = null;
			var idx = indexes[0];
			if (this.UObjMode == UObjMode.Default)
			{
				if (!Properties.ContainsKey(idx.ToString()))
				{
					Properties[idx.ToString()] = new UObj { UObjMode = UObjMode.Fake, Parent = this };
				}
				if (this._properties != null && this._properties.ContainsKey(idx.ToString()))
				{
					result = this._properties[idx.ToString()];
				}
			}
			else if(this.UObjMode==UObjMode.Array)
			{
				if(!(idx is int))throw new Exception("cannot index arrays with not ints");
				if (((int)idx) < 0)
				{
					throw new Exception("index of array cannot be negative");
				}
				if (null != _array && ((int) idx) < _array.Count)
				{
					result = _array[(int) idx];
				}
			}
			
			return true;
		}
		/// <summary>
		/// Конвертация в строку
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static implicit operator string(UObj obj){
			if (null == obj) return "";
			if (obj.UObjMode == UObjMode.Fake) return "";
			if (obj.UObjMode == UObjMode.Value) return obj.Properties["__value"].ToStr();
			return obj.ToJson();
		}

		/// <summary>
		/// Конвертация в строку
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static implicit operator int(UObj obj)
		{
			if (null == obj) return 0;
			if (obj.UObjMode == UObjMode.Fake) return 0;
			if (obj.UObjMode == UObjMode.Value) return obj.Properties["__value"].ToInt();
			if (obj.UObjMode == UObjMode.Array) return obj.Array.Count;
			return obj.Properties.Count;
		}


		/// <summary>
		/// Конвертация в строку
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static implicit operator long(UObj obj)
		{
			if (null == obj) return 0;
			if (obj.UObjMode == UObjMode.Fake) return 0;
			if (obj.UObjMode == UObjMode.Value) return obj.Properties["__value"].ToLong();
			if (obj.UObjMode == UObjMode.Array) return obj.Array.Count;
			return obj.Properties.Count;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="binder"></param>
		/// <param name="indexes"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
		{
			if (this.UObjMode == UObjMode.Fake)
			{
				this.UObjMode = UObjMode.Default;
			}
			var idx = indexes[0];
			if (this.UObjMode == UObjMode.Default)
			{
				if (idx is int && IsFakeAble())
				{
					this.UObjMode = UObjMode.Array;
				}else

				{
					this.Properties[idx.ToString()] = UObjSerializerSupport.ToUson(value);
				}
			}
			if (this.UObjMode == UObjMode.Array)
			{
				if (!(idx is int)) throw new Exception("cannot index arrays with not ints");
				if (((int) idx) < 0)
				{
					throw new Exception("index of array cannot be negative");
				}
				while (((int) idx) >= Array.Count)
				{
					Array.Add(null);
				}
				Array[(int)idx] = UObjSerializerSupport.ToUson(value);
			}
			return true;
		}

	    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
	    public UObj push(IEnumerable<object> args)
	    {
	       return  push((object[]) args.ToArray());
	    }
		/// <summary>
		/// Проверяет наличие свойства
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool isDefined(string name){
			return Properties.ContainsKey(name);

		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		public UObj push(params object[] args)
		{
			
			if (0 == args.Length)
			{
				throw new Exception("require at least one parameter");
			}
			if (this.UObjMode == UObjMode.Fake || (this.UObjMode==UObjMode.Default && IsFakeAble()))
			{
				this.UObjMode = UObjMode.Array;
			}
			if (this.UObjMode != UObjMode.Array)
			{
				throw new Exception("this is not array to call push");
			}
			foreach (var o in args)
			{
				this.Array.Add(UObjSerializerSupport.ToUson(o));
			}
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public UObj extend(params object[] other)
		{
			foreach (var uObj in other)
			{
				extend(uObj, false,false);	
			}
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public UObj deepextend(params object[] other)
		{
			foreach (var uObj in other)
			{
				extend(uObj, false,true);
			}
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="safe"></param>
		/// <param name="deep"></param>
		/// <returns></returns>
		private UObj extend(object obj,bool safe, bool deep)
		{
			UObj other = (obj as UObj) ?? (UObjSerializerSupport.ToUson(obj) as UObj);
			if (null == other)
			{
				return this;
			}
			if (other.UObjMode != UObjMode.Fake)
			{
				if (UObjMode == UObjMode.Fake || IsFakeAble())
				{
					this.UObjMode = other.UObjMode;
				}

				if (this.UObjMode != other.UObjMode)
				{
					throw new Exception("cannot extend distinct USON object types (arrays and objects)");
				}

				if (this.UObjMode == UObjMode.Default)
				{
					foreach (var property in other.Properties)
					{
						if ( this.Properties.ContainsKey(property.Key))
						{
							if (deep && this.Properties[property.Key] is UObj && property.Value is UObj)
							{
								(this.Properties[property.Key] as UObj).extend(property.Value as UObj, safe, deep);
								continue;
							}
							else if(safe)
							{
								continue;	
							}
							
						}
						this.Properties[property.Key] = UObjSerializerSupport.ToUson(property.Value);
					}
				}
				else
				{
					for (var i = 0; i < other.Array.Count; i++)
					{
						if (Array.Count <= i)
						{
							Array.Add(null);
						}
						if (null != this.Array[i])
						{
							if (deep && this.Array[i] is UObj && other.Array[i] is UObj)
							{
								(this.Array[i] as UObj).extend(other.Array[i] as UObj, safe, deep);
								continue;
							}else if (safe)
							{
								continue;
							}
						}
						this.Array[i] = UObjSerializerSupport.ToUson(other.Array[i]);
					}
				}
			}
			return this;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public UObj defaults(params object[] other)
		{
			foreach (var uObj in other)
			{
				extend(uObj, true,false);
			}
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public UObj deepdefaults(params object[] other)
		{
			foreach (var uObj in other)
			{
				extend(uObj, true,true);
			}
			return this;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public object SmartResolve(string id){
			return _properties.Where(_ => _.Key.ToLower() == id.ToLower()).Select(_=>_.Value).FirstOrDefault();
			
		}
	}
}
