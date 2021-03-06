﻿#region LICENSE
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
// PROJECT ORIGIN: Qorpent.Serialization/serializableItem.cs
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization {
	/// <summary>
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class SerializableItem {
		/// <summary>
		///		Index
		/// </summary>
		public int Index { get; set; }
		/// <summary>
		///		NoIndex
		/// </summary>
		public bool NoIndex { get; set; }
		/// <summary>
		///		Item name
		/// </summary>
		public string ItemName { get; set; }
		/// <summary>
		/// 	Initializes a new instance of the <see cref="SerializableItem" /> class.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <param name="value"> The value. </param>
		/// <param name="type"> The type. </param>
		/// <remarks>
		/// </remarks>
		public SerializableItem(string name, object value, Type type) {
			Value = value;
			Type = type ?? (null == value ? typeof (object) : value.GetType());
			Name = string.IsNullOrEmpty(name) ? Type.Name : name;
			Debug.Assert(type != null, "type != null");
			IsFinal = (type.IsValueType || type == typeof (string));
		}

		/// <summary>
		/// 	Initializes a new instance of the <see cref="SerializableItem" /> class.
		/// </summary>
		/// <param name="field"> The field. </param>
		/// <param name="target"> The target. </param>
		/// <remarks>
		/// </remarks>
		public SerializableItem(FieldInfo field, object target)
			: this(field.Name, field.GetValue(target), field.FieldType) {
			Member = field;
			_valueprepared = true;
			ApplyMember();
		}

		private void ApplyMember() {
			var sa = Member.GetFirstAttribute<SerializeAttribute>();
			var classSa = Member.DeclaringType.GetFirstAttribute<SerializeAttribute>();
			ApplyAttribute(classSa);
			ApplyAttribute(sa);
			if (Index == 0) {
				Index = 1000;
			}
		}

		private void ApplyAttribute(SerializeAttribute sa) {
			if (null != sa) {
				if (!string.IsNullOrWhiteSpace(sa.ItemName)) {
					ItemName = sa.ItemName;
				}
				NoIndex = sa.NoIndex;
				Index = sa.Index;
				if (sa.CamelNames) {
					Name = Name.Substring(0, 1).ToLower() + Name.Substring(1);
				}
			}
		}

		private SerializeAttribute GetSerializeableAttribute() {
			if (!_typesercache.ContainsKey(Member.DeclaringType))
			{
				var result = Member.DeclaringType.GetFirstAttribute<SerializeAttribute>();
				_typesercache[Member.DeclaringType] = result;
				return result;
			}
			return _typesercache[Member.DeclaringType];
		}

		private static IDictionary<Type, SerializeAttribute> _typesercache = new Dictionary<Type, SerializeAttribute>();

		/// <summary>
		/// 	Initializes a new instance of the <see cref="SerializableItem" /> class.
		/// </summary>
		/// <param name="property"> The property. </param>
		/// <param name="target"> The target. </param>
		/// <remarks>
		/// </remarks>
		public SerializableItem(PropertyInfo property, object target)
			: this(property.Name, null, property.PropertyType) {
			Member = property;
			_target = target;
			_valueprepared = false;
			ApplyMember();
		}

		/// <summary>
		/// 	Gets or sets the member.
		/// </summary>
		/// <value> The member. </value>
		/// <remarks>
		/// </remarks>
		public MemberInfo Member { get; set; }
		static IDictionary<MemberInfo,bool> _issercache = new Dictionary<MemberInfo, bool>();
		/// <summary>
		/// 	Gets a value indicating whether this instance is serializable.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public bool IsSerializable {
			get {
	
					var result = false;
					if (null == Member) return false;
					if (!_issercache.ContainsKey(Member)){
						result = InternalGetIsSerializable();
						_issercache[Member] = result;
					}
					else{
						result = _issercache[Member];
					}
					if (!result) return false;

					if (CheckNull()){
						return true;
					}
					return false;
				
			}
		}

		private bool InternalGetIsSerializable() {
			if (Member.Name == "__interceptors") {
				return false; //ioc includes
			}
			if (Member.DeclaringType != null && Member.DeclaringType.GetCustomAttributes(typeof (CompilerGeneratedAttribute), true)
				                                    .Count() > 0) {
				return true; //anonymous class property|field
			}

			if (CheckIgnore())
			{
				if (CheckType())
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 	Gets the value.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public object Value {
			get {
				if (!_valueprepared) {
					_value = ((PropertyInfo) Member).GetValue(_target, null);
				}
				return _value;
			}
			private set { _value = value; }
		}

		/// <summary>
		/// 	Gets a value indicating whether this instance is final.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public bool IsFinal { get; private set; }

		/// <summary>
		/// 	Gets the name.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public string Name { get; private set; }

		/// <summary>
		/// 	Gets the type.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public Type Type { get; private set; }

		private static readonly object sync = new object();
		/// <summary>
		/// 	Gets the serializable items.
		/// </summary>
		/// <param name="target"> The target. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public static IEnumerable<SerializableItem> GetSerializableItems(object target) {
			lock (sync){
				var result = target.GetType().GetFields()
				                   .Select(field => new SerializableItem(field, target))
				                   .ToList();
				foreach (var field in target.GetType().GetProperties()){
					try{
						result.Add(new SerializableItem(field, target));
					}
					catch (TargetInvocationException){
					}
					catch (TargetParameterCountException){
					}
				}
				return result.Where(x => x.IsSerializable).OrderBy(_ => _.Index).ToArray();
			}
		}

		/// <summary>
		/// 	Checks the null.
		/// </summary>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		private bool CheckNull() {
			if (null == Member) {
				return true;
			}
			if (!IsNotNullSetted()) {
				return true;
			}
			if (IsFinal) {
				if (typeof (string) == Type) {
					return !string.IsNullOrEmpty((string) Value);
				}
				if (typeof (DateTime) == Type) {
					return DateTime.MinValue != (DateTime) Value && !((DateTime)Value).IsDateNull();
				}
				if (Type.IsEnum) {
					return (Value.ToInt())!=0;
				}
				if (Type.IsArray) {
					return 0 != ((Array) Value).Length;
				}
				return !Equals(Value, Activator.CreateInstance(Type));
			}
			//return null != Value;
			if (null == Value) {
				return false;
			}
			if (Value is Array) {
				return 0 != ((Array) Value).Length;
			}
			if (Value.GetType().IsGenericType) {
				if ( typeof (List<>)==Value.GetType().GetGenericTypeDefinition()) {
					return ((IEnumerable) Value).OfType<object>().Count() != 0;
				}
				if (Value.GetType().GetGenericTypeDefinition() == typeof (Dictionary<,>)) {
					return ((IEnumerable) Value).OfType<object>().Count() != 0;
				}
			}
			return true;
		}

		private bool IsNotNullSetted() {
			if(!_notnullonlycache.ContainsKey(Member)) {
				var result =  IsAttrSetted<SerializeNotNullOnlyAttribute>(Member);
				_notnullonlycache[Member] = result;
				return result;
			}
			return _notnullonlycache[Member];
		}

		static IDictionary<MemberInfo,bool> _notnullonlycache =  new Dictionary<MemberInfo, bool>();

		/// <summary>
		/// 	Isattrsetteds the specified type.
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <param name="type"> The type. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		private static bool IsAttrSetted<T>(Type type) where T : Attribute {
			if (0 != type.GetCustomAttributes(typeof (T), true).Length) {
				return true;
			}
			return type.GetInterfaces().Any(i => 0 != i.GetCustomAttributes(typeof (T), true).Length);
		}

		/// <summary>
		/// 	Isattrsetteds the specified member.
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <param name="member"> The member. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		private bool IsAttrSetted<T>(MemberInfo member) where T : Attribute {
			if (0 != member.GetCustomAttributes(typeof (T), true).Length) {
				return true;
			}
			Debug.Assert(member.DeclaringType != null, "member.DeclaringType != null");
			return member.DeclaringType.GetInterfaces().Select(i => i.GetMember(member.Name).FirstOrDefault(x => x.GetType() == member.GetType())).Any(im => null != im && 0 != im.GetCustomAttributes(typeof (T), true).Length);
		}

		/// <summary>
		/// 	Checks the type.
		/// </summary>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		private bool CheckType() {
			try {
				var propertyInfo = Member as PropertyInfo;
				if (propertyInfo != null) {
					//no support for indexers
					var idx = propertyInfo.GetIndexParameters();
					if (0 != idx.Length) {
						return false;
					}
				}
				if (IsFinal) {
					return true;
				}
				if (IsAttrSetted<SerializeAttribute>(Member)) {
					return true;
				}
				if (IsAttrSetted<SerializeNotNullOnlyAttribute>(Member))
				{
					return true;
				}
				if (typeof (IDictionary).IsAssignableFrom(Type) && Value is IDictionary) {
					return true;
				}
				if (typeof (Array).IsAssignableFrom(Type) ||
				    (Value is Array)) {
					return true;
				}
				if (Value != null && Value.GetType().Name[0]=='<') {
					return true;
				}
				return IsAttrSetted<SerializeAttribute>(Type) ;
			}
			catch (Exception) {
				return false;
			}
		}

		/// <summary>
		/// 	Checks the ignore.
		/// </summary>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		private bool CheckIgnore() {
			if (null == Member) {
				return true;
			}
			return !IsAttrSetted<IgnoreSerializeAttribute>(Member);
		}

		/// <summary>
		/// </summary>
		private readonly object _target;

		/// <summary>
		/// </summary>
		private bool _valueprepared;

		/// <summary>
		/// </summary>
		private object _value;
	}
}