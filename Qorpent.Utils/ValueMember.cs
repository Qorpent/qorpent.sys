#region LICENSE
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
// PROJECT ORIGIN: Qorpent.Utils/ValueMember.cs
#endregion
using System;
using System.Reflection;

namespace Qorpent.Utils {
	/// <summary>
	/// 	Wraps property or field to single interface with type conversion
	/// 	helper to avoid wrong .NET reflection class design
	/// </summary>
	public class ValueMember {
		/// <summary>
		/// 	creates property wrapper
		/// </summary>
		/// <param name="property"> </param>
		/// <param name="checkprivacy"> true - to force private checking - in this mode it will prevent get or set values to non-public members </param>
		public ValueMember(PropertyInfo property, bool checkprivacy) {
			_checkprivacy = checkprivacy;
			_property = property;
		}

		/// <summary>
		/// 	creates field wrapper
		/// </summary>
		/// <param name="field"> </param>
		/// <param name="checkprivacy"> true - to force private checking - in this mode it will prevent get or set values to non-public members </param>
		public ValueMember(FieldInfo field, bool checkprivacy) {
			_checkprivacy = checkprivacy;
			_field = field;
		}

		/// <summary>
		/// 	creates wrapper with member type checking
		/// </summary>
		/// <param name="info"> </param>
		/// <param name="checkprivacy"> true - to force private checking - in this mode it will prevent get or set values to non-public members </param>
		public ValueMember(MemberInfo info, bool checkprivacy) {
			if (info is PropertyInfo) {
				_property = (PropertyInfo) info;
			}
			else if (info is FieldInfo) {
				_field = (FieldInfo) info;
			}
			else {
				throw new ReflectionExtensionsException("cannot create ClassAttribute on " + info);
			}
			_checkprivacy = checkprivacy;
		}

		/// <summary>
		/// 	returns target Class member
		/// </summary>
		public MemberInfo Member {
			get { return (MemberInfo) _property ?? _field; }
		}

		/// <summary>
		/// 	target member type
		/// </summary>
		public Type Type {
			get {
				if (null != _property) {
					return _property.PropertyType;
				}
				if (null != _field) {
					return _field.FieldType;
				}
				throw new ReflectionExtensionsException("cannot return Type - no field or property targeted");
			}
		}

		/// <summary>
		/// 	returns true if member can be assigned - is field or property with set accessor
		/// </summary>
		public bool CanBeAssigned {
			get {
				if (null != _property) {
					var setter = _property.GetSetMethod(true);
					if (null == setter) {
						return false;
					}
					return !CheckPrivacy || setter.IsPublic;
				}
				if (null != _field) {
					return !CheckPrivacy || _field.IsPublic;
				}
				throw new ReflectionExtensionsException("cannot check assigning - no field or property targeted");
			}
		}

		/// <summary>
		/// 	true if U can use Get method on this member
		/// </summary>
		/// <exception cref="ReflectionExtensionsException"></exception>
		public bool CanBeRetrieved {
			get {
				if (null != _property) {
					var getter = _property.GetGetMethod(true);
					if (null == getter) {
						return false;
					}
					if (!CheckPrivacy) {
						return true;
					}
					return getter.IsPublic;
				}
				if (null != _field) {
					if (!CheckPrivacy) {
						return true;
					}
					return _field.IsPublic;
				}
				throw new ReflectionExtensionsException("cannot check retrieving - no field or property targeted");
			}
		}

		/// <summary>
		/// 	indicates that this member wrapper do control on member privacy (not allow read/write not public members)
		/// </summary>
		public bool CheckPrivacy {
			get { return _checkprivacy; }
		}

		/// <summary>
		/// 	assignes Value to target member (indexers can be used)
		/// </summary>
		/// <param name="targetObject"> </param>
		/// <param name="value"> </param>
		/// <param name="indexers"> </param>
		/// <returns> </returns>
		/// <exception cref="ReflectionExtensionsException"></exception>
		public ValueMember Set(object targetObject, object value, object[] indexers = null) {
			if (!CanBeAssigned) {
				throw new ReflectionExtensionsException("U cannot call Set on this member");
			}
			if (null != _property) {
				_property.SetValue(targetObject, value, indexers);
			}
			else if (null != _field) {
				_field.SetValue(targetObject, value);
			}
			else {
				throw new ReflectionExtensionsException("cannot set Value - no field or property targeted");
			}

			return this;
		}

		/// <summary>
		/// 	returns Value of member on target object
		/// </summary>
		/// <param name="targetObject"> </param>
		/// <param name="indexers"> </param>
		/// <returns> </returns>
		public object Get(object targetObject, object[] indexers = null) {
			if (!CanBeRetrieved) {
				throw new ReflectionExtensionsException("U cannot call Get on this member");
			}
			if (null != _property) {
				return _property.GetValue(targetObject, indexers);
			}
			if (null != _field) {
				return _field.GetValue(targetObject);
			}
			throw new ReflectionExtensionsException("cannot get Value - no field or property targeted");
		}

		private readonly bool _checkprivacy;
		private readonly FieldInfo _field;
		private readonly PropertyInfo _property;
	}
}