#region LICENSE

// Copyright 2007-2012 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Solution: Qorpent
// Original file : ReflectionExtensions.cs
// Project: Qorpent.Utils
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Qorpent.Utils.Extensions {
	/// <summary>
	/// 	Extensions to work over reflection
	/// </summary>
	public static class ReflectionExtensions {
		private static ReflectionHelper _helper;

		static ReflectionExtensions() {
			_helper = new ReflectionHelper();
		}

		/// <summary>
		/// 	Default domain wide helper
		/// </summary>
		public static ReflectionHelper DefaultHelper {
			get { return _helper; }
			set { _helper = value; }
		}

		/// <summary>
		/// </summary>
		/// <param name="type"> </param>
		/// <typeparam name="T"> </typeparam>
		/// <returns> </returns>
		public static T GetFirstAttribute<T>(this Type type) where T : Attribute {
			return _helper.GetFirstAttribute<T>(type);
		}

		/// <summary>
		/// </summary>
		/// <param name="member"> </param>
		/// <typeparam name="T"> </typeparam>
		/// <returns> </returns>
		public static T GetFirstAttribute<T>(this MemberInfo member) where T : Attribute {
			return _helper.GetFirstAttribute<T>(member);
		}

		/// <summary>
		/// 	applyes an Value to target object to property or field
		/// 	incapsulate experience of light-weight de-serialization of XML, dictionaries and input parameters (console and web)
		/// </summary>
		/// <param name="target"> target object (not null) to which Value must be applyed </param>
		/// <param name="name"> name of field or property (not empty) </param>
		/// <param name="value"> assigned Value </param>
		/// <param name="ignorecase"> true - case will be ignored on property/field binding </param>
		/// <param name="ignoreNotFound"> true - no error will be thrown if prop/field will not found (or only found property is read-only) </param>
		/// <param name="publicOnly"> true - only public members will be used </param>
		/// <param name="ignoreTypeConversionError"> true - will not fire error if Value cannot be converted and assigned to property type </param>
		/// <param name="ignoreInnerPropertyExceptions"> true - will not rethrow inner property exceptions on assign </param>
		/// <returns> </returns>
		public static object SetValue(this object target, object name, object value, bool ignorecase = true,
		                              bool ignoreNotFound = false, bool publicOnly = true,
		                              bool ignoreTypeConversionError = false, bool ignoreInnerPropertyExceptions = false) {
			return _helper.SetValue(target, name, value, ignorecase, ignoreNotFound, publicOnly, ignoreTypeConversionError,
			                        ignoreInnerPropertyExceptions);
		}


		/// <summary>
		/// </summary>
		/// <param name="type"> </param>
		/// <typeparam name="T"> </typeparam>
		/// <returns> </returns>
		public static T Create<T>(this Type type) {
			return _helper.Create<T>(type);
		}

		/// <summary>
		/// 	get a Value from object with name resolution, supports defaults and binding hints
		/// </summary>
		/// <param name="target"> target object (not null) from which Value must be retrieved </param>
		/// <param name="name"> name of field or property (not empty) </param>
		/// <param name="defgen"> default generator on default(T) to be returned </param>
		/// <param name="ignorecase"> true - case will be ignored on property/field binding </param>
		/// <param name="ignoreNotFound"> true - no error will be thrown if prop/field will not found (or only found property is read-only) </param>
		/// <param name="publicOnly"> true - only public members will be used </param>
		/// <param name="ignoreTypeConversionError"> true - will not fire error if Value cannot be converted and assigned to property type </param>
		/// <param name="ignoreInnerPropertyExceptions"> true - will not rethrow inner property exceptions on get </param>
		/// <param name="def"> default Value on default(T) returned </param>
		/// ///
		/// <param name="initialize"> true - allow initialize new dictionary item if not existed </param>
		/// <returns> </returns>
		public static T GetValue<T>(this object target, object name, T def = default(T), Func<T> defgen = null,
		                            bool ignorecase = true, bool ignoreNotFound = false, bool publicOnly = true,
		                            bool ignoreTypeConversionError = false, bool ignoreInnerPropertyExceptions = false,
		                            bool initialize = false) {
			return _helper.GetValue(target, name, def, defgen, ignorecase, ignoreNotFound, publicOnly, ignoreTypeConversionError,
			                        ignoreInnerPropertyExceptions, initialize);
		}

		/// <summary>
		/// 	checkout given type and return mostly matched property or field in priority Property->Field, Public->Private
		/// </summary>
		/// <param name="type"> </param>
		/// <param name="name"> </param>
		/// <param name="ignoreCase"> </param>
		/// <param name="publicOnly"> </param>
		/// <param name="readableOnly"> </param>
		/// <param name="assignableOnly"> </param>
		/// <returns> </returns>
		public static ValueMember FindValueMember(this Type type, string name, bool ignoreCase = true, bool publicOnly = false,
		                                          bool readableOnly = false, bool assignableOnly = false) {
			return _helper.FindValueMember(type, name, ignoreCase, publicOnly, readableOnly, assignableOnly);
		}

		/// <summary>
		/// 	Ищет в типе все свойства и поля по заданным условиям с возможным указанием фильтрующего атрибута
		/// </summary>
		/// <param name="type"> тип, в котором требется произвести поиск </param>
		/// <param name="attributeType"> фильтрующий тип атрибута (отбор только тех членов класса, к которым привязан данный атрибут </param>
		/// <param name="publicOnly"> только публичные свойства/поля </param>
		/// <param name="readableOnly"> только свойства с поддержкой чтения (для полей игнорируется) </param>
		/// <param name="assignableOnly"> только свойства с поддержкой записи (для полей игнорируется) </param>
		/// <exception cref="ReflectionExtensionsException">не указан тип для поиска</exception>
		/// <returns> перечисление всех соответствующих полей и свойств </returns>
		public static IEnumerable<ValueMember> FindAllValueMembers(this Type type, Type attributeType, bool publicOnly = false,
		                                                           bool readableOnly = false, bool assignableOnly = false) {
			return _helper.FindAllValueMembers(type, attributeType, publicOnly, readableOnly, assignableOnly);
		}
	}
}