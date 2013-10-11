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
// PROJECT ORIGIN: Qorpent.Utils/ReflectionHelper.cs
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils {
	/// <summary>
	/// 	helper to work over native .net reflection - different usefull methods for dynamic apply and so on
	/// </summary>
	public class ReflectionHelper {
		/// <summary>
		/// 	applyes an Value to target object to property or field
		/// 	incapsulate experience of light-weight de-serialization of XML, dictionaries and input parameters (console and web)
		/// </summary>
		/// <param name="target"> target object (not null) to which Value must be applyed </param>
		/// <param name="key"> dict key or name of field or property (not empty) </param>
		/// <param name="value"> assigned Value </param>
		/// <param name="ignorecase"> true - case will be ignored on property/field binding </param>
		/// <param name="ignoreNotFound"> true - no error will be thrown if prop/field will not found (or only found property is read-only) </param>
		/// <param name="publicOnly"> true - only public members will be used </param>
		/// <param name="ignoreTypeConversionError"> true - will not fire error if Value cannot be converted and assigned to property type </param>
		/// <param name="ignoreInnerPropertyExceptions"> true - will not rethrow inner property exceptions on assign </param>
		/// <returns> </returns>
		public object SetValue(object target, object key, object value, bool ignorecase = true, bool ignoreNotFound = false,
		                       bool publicOnly = true, bool ignoreTypeConversionError = false,
		                       bool ignoreInnerPropertyExceptions = false) {
			if (target == null) {
				throw new ReflectionExtensionsException("target is null");
			}
			if (null == key) {
				throw new ReflectionExtensionsException("key is null");
			}
			var type = target.GetType();

			//automatic dictionary redirect
			if (IsDictionary(type)) {
				var dicttype = GetDictionaryTypes(type);
				var setmethod = _setdictvalmethodbase.MakeGenericMethod(dicttype[0], dicttype[1]);
				return setmethod.Invoke(this, new[] {target, key, value});
			}

			var name = key.ToStr();
			if (name.IsEmpty()) {
				throw new ReflectionExtensionsException("name of property or field will be empty");
			}

			// try find mostly priorityzed member
			var classvalue = FindValueMember(type, name, ignorecase, publicOnly, false, true);

			//check availability of member
			if (null == classvalue) {
				if (ignoreNotFound) {
					return target;
				}
				throw new ReflectionExtensionsException("cannot find assignable member with name " + key + " in type " + type +
				                                        "with given options");
			}
			var targettype = classvalue.Type;

            if (targettype == typeof (string[])  && value is string) {
                classvalue.Set(target, ((string) value).SmartSplit().ToArray());
                return target;
            }
            if (targettype == typeof(int[]) && value is string)
            {
                classvalue.Set(target, ((string)value).SmartSplit().Select(_=>_.ToInt()).ToArray());
                return target;
            }

			// get converted Value
			var assignvalue = value.ToTargetType(targettype, ignoreTypeConversionError);

			if (value != null && assignvalue == null) {
				return target; // if conversation gives safely null (non converted) we silently skip assigning
			}

			try {
				classvalue.Set(target, assignvalue);
			}
			catch (Exception ex) {
				if (ignoreInnerPropertyExceptions) {
					return target;
				}
				throw new ReflectionExtensionsException("erorr on property/field assigment in " + key, ex);
			}

			return target;
		}

		/// <summary>
		/// 	checks if given type is IDictionary[] descendant
		/// </summary>
		/// <param name="type"> </param>
		/// <returns> </returns>
		public bool IsDictionary(Type type) {
			if (!type.IsGenericType) {
				return false;
			}
			return _basedictint == type.GetGenericTypeDefinition();
		}

		/// <summary>
		/// </summary>
		/// <param name="type"> </param>
		/// <typeparam name="T"> </typeparam>
		/// <returns> </returns>
		public T Create<T>(Type type) {
			return (T) Activator.CreateInstance(type);
		}

		/// <summary>
		/// 	retrieves generic type definition for given dictionary type
		/// </summary>
		/// <param name="type"> </param>
		/// <returns> </returns>
		/// <exception cref="ReflectionExtensionsException"></exception>
		public Type[] GetDictionaryTypes(Type type) {
			if (!IsDictionary(type)) {
				throw new ReflectionExtensionsException("try to get generic dictionary parameters from non-dictionary type");
			}
			return type.GetGenericArguments();
		}


		/// <summary>
		/// 	wrapper for setting values in dictionary - not tend to use directly
		/// 	type conversion
		/// </summary>
		/// <param name="dictionary"> target dictionary </param>
		/// <param name="key"> key in dictionary </param>
		/// <param name="value"> </param>
		/// <typeparam name="TK"> </typeparam>
		/// <typeparam name="TV"> </typeparam>
		/// <exception cref="ReflectionExtensionsException"></exception>
		public void SetDictionaryValue<TK, TV>(IDictionary<TK, TV> dictionary, object key, object value) {
			if (null == dictionary) {
				throw new ReflectionExtensionsException("cannot set values to null dictionaries");
			}
			var realkey = key.To<TK>();
			if (!typeof (TK).IsValueType && Equals(null, realkey)) {
				throw new ReflectionExtensionsException("key (realkey) cannot be null");
			}
			var valuetoset = value.To<TV>();
			dictionary[realkey] = valuetoset;
		}

		/// <summary>
		/// 	null safe version of GetValue, especially for Dictionaries
		/// 	before we used this functionality in special DictionaryHelper, but it's very close to object's GetValue logic
		/// 	object's GetValue behavior will be redirected to Dict behave if target object id IDictionary[K,V]
		/// </summary>
		/// <param name="dictionary"> dictionary for getvalue </param>
		/// <param name="key"> key in dictionary to find </param>
		/// <param name="def"> default Value FOR NON EXISTED in DICT </param>
		/// <param name="defgen"> default generator FOR NON EXISTED in DICT </param>
		/// <param name="ignorecase"> for STRING keys only - ignorecase mode of key found (direct match will be used first) </param>
		/// <param name="filldict"> if nont EXIST will fill dictionary with default Value </param>
		/// <typeparam name="TK"> KEY TYPE </typeparam>
		/// <typeparam name="TV"> VALUE TYPE </typeparam>
		/// <typeparam name="T"> TARGET TYPE </typeparam>
		/// <returns> </returns>
		public T GetDictionaryValue<TK, TV, T>(IDictionary<TK, TV> dictionary, object key, T def = default(T),
		                                       Func<T> defgen = null, bool ignorecase = false, bool filldict = false) {
			//null safe behaviour
			if (null == dictionary) {
				return defgen == null ? def : defgen();
			}
			var realkey = key.To<TK>();
			if (Equals(null, realkey)) {
				throw new ReflectionExtensionsException("key(realkey) to find in dictionary cannot be null");
			}
			if (dictionary.ContainsKey(realkey)) {
				return dictionary[realkey].To<T>();
			}
			if (typeof (TK) == typeof (string) && ignorecase) {
				foreach (var k in 
					from k in dictionary.Keys
					let s = k as string
					where s.ToUpperInvariant() == ((string) key).ToUpperInvariant()
					select k) {
					return dictionary[k].To<T>();
				}
			}
			var result = defgen == null ? def : defgen();
			if (filldict) {
				dictionary[realkey] = result.To<TV>();
			}
			return result;
		}


		/// <summary>
		/// 	get a Value from object with name resolution, supports defaults and binding hints
		/// 	null safe
		/// </summary>
		/// <param name="target"> target object (not null) from which Value must be retrieved </param>
		/// <param name="key"> key for dict name of field or property (not empty) </param>
		/// <param name="defgen"> default generator on default(T) to be returned </param>
		/// <param name="ignorecase"> true - case will be ignored on property/field binding </param>
		/// <param name="ignoreNotFound"> true - no error will be thrown if prop/field will not found (or only found property is read-only) </param>
		/// <param name="publicOnly"> true - only public members will be used </param>
		/// <param name="ignoreTypeConversionError"> true - will not fire error if Value cannot be converted and assigned to property type </param>
		/// <param name="ignoreInnerPropertyExceptions"> true - will not rethrow inner property exceptions on get </param>
		/// <param name="def"> default Value on default(T) returned </param>
		/// <param name="initialize"> can initialize new item in DICTIONARIES </param>
		/// <returns> </returns>
		public T GetValue<T>(object target, object key, T def = default(T), Func<T> defgen = null, bool ignorecase = true,
		                     bool ignoreNotFound = false, bool publicOnly = true, bool ignoreTypeConversionError = false,
		                     bool ignoreInnerPropertyExceptions = false, bool initialize = false) {
			if (target == null) {
				//safely workaround nulls
				return defgen == null ? def : defgen();
			}
			if (null == key) {
				throw new ReflectionExtensionsException("propOfFieldName is empty");
			}

			var type = target.GetType();

			//automatic dictionary redirect
			if (IsDictionary(type)) {
				var dicttype = GetDictionaryTypes(type);
				var getmethod = _getdictvalmethodbase.MakeGenericMethod(dicttype[0], dicttype[1], typeof (T));
				return (T) getmethod.Invoke(this, new[] {target, key, def, defgen, ignorecase, initialize});
			}

			var name = key.ToStr();
			if (name.IsEmpty()) {
				throw new ReflectionExtensionsException("cannot set property with empty name");
			}

			// try find mostly priorityzed member
			var classvalue = FindValueMember(type, name, ignorecase, publicOnly, true, false);

			//check availability of member
			if (null == classvalue) {
				if (ignoreNotFound) {
					return defgen == null ? def : defgen();
				}
				throw new ReflectionExtensionsException("cannot find readable member with name " + name + " in type " + type +
				                                        "with given options");
			}


			try {
				var rawresult = classvalue.Get(target);
				try {
					var typedresult = rawresult.To(false, def, defgen);
					return typedresult;
				}
				catch (Exception ex) {
					if (ignoreTypeConversionError) {
						return defgen == null ? def : defgen();
					}
					throw new ReflectionExtensionsException(
						"erorr on conversion Value of type " + classvalue.Type + " to " + typeof (T), ex);
				}
			}
			catch (Exception ex) {
				if (ignoreInnerPropertyExceptions) {
					return defgen == null ? def : defgen();
				}
				throw new ReflectionExtensionsException("erorr on property/field getting in " + name, ex);
			}
		}


		/// <summary>
		/// 	checkout given type and return mostly matched property or field in priority Property->Field, Public->Private
		/// </summary>
		/// <param name="type"> </param>
		/// <param name="name"> </param>
		/// <param name="ignoreCase"> </param>
		/// <param name="publicOnly"> </param>
		/// <param name="readableOnly"> </param>
		/// <param name="writeableOnly"> </param>
		/// <returns> </returns>
		public ValueMember FindValueMember(Type type, string name, bool ignoreCase = true, bool publicOnly = false,
		                                   bool readableOnly = false, bool writeableOnly = false) {
			if (null == type) {
				throw new ReflectionExtensionsException("type is null");
			}
			if (name.IsEmpty()) {
				throw new ReflectionExtensionsException("name is empty");
			}
			var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetField | BindingFlags.SetField |
			            BindingFlags.GetProperty | BindingFlags.SetProperty;
			if (ignoreCase) {
				flags = flags | BindingFlags.IgnoreCase;
			}
			if (!publicOnly) {
				flags = flags | BindingFlags.NonPublic;
			}
			return type.FindMembers(MemberTypes.Field | MemberTypes.Property, flags,
			                        (m, x) => ignoreCase ? m.Name.ToUpper() == name.ToUpper() : m.Name == name, null)
				.OrderBy(x => x, new MemberComparer(name))
				.Select(x => new ValueMember(x, publicOnly))
				.FirstOrDefault(x => (!readableOnly || x.CanBeRetrieved) && (!writeableOnly || x.CanBeAssigned));
		}

		/// <summary>
		/// </summary>
		/// <param name="type"> </param>
		/// <typeparam name="T"> </typeparam>
		/// <returns> </returns>
		public T GetFirstAttribute<T>(Type type) where T : Attribute {
			return type.GetCustomAttributes(typeof (T), true).OfType<T>().FirstOrDefault();
		}

		/// <summary>
		/// </summary>
		/// <param name="member"> </param>
		/// <typeparam name="T"> </typeparam>
		/// <returns> </returns>
		public T GetFirstAttribute<T>(MemberInfo member) where T : Attribute {
			return member.GetCustomAttributes(typeof (T), true).OfType<T>().FirstOrDefault();
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
		public IEnumerable<ValueMember> FindAllValueMembers(Type type, Type attributeType = null, bool publicOnly = false,
		                                                    bool readableOnly = false, bool assignableOnly = false) {
			if (null == type) {
				throw new ReflectionExtensionsException("type is null");
			}

			var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetField | BindingFlags.SetField |
			            BindingFlags.GetProperty | BindingFlags.SetProperty;
			if (!publicOnly) {
				flags = flags | BindingFlags.NonPublic;
			}

			return type.FindMembers(MemberTypes.Field | MemberTypes.Property, flags,
			                        (m, x) => null == attributeType || 0 != m.GetCustomAttributes(attributeType, true).Length,
			                        null)
				.Select(x => new ValueMember(x, publicOnly))
				.Where(x => (!readableOnly || x.CanBeRetrieved) && (!assignableOnly || x.CanBeAssigned));
		}

		#region Nested type: MemberComparer

		private class MemberComparer : IComparer<MemberInfo> {
			public MemberComparer(string testname) {
				_testname = testname;
			}


			public int Compare(MemberInfo x, MemberInfo y) {
				if (ReferenceEquals(x, y)) {
					return 0;
				}

				var xpublic = false;
				var ypublic = false;
				if (x is PropertyInfo) {
					var xget = ((PropertyInfo) x).GetGetMethod();
					var xset = ((PropertyInfo) x).GetSetMethod();
					if (xget != null && xget.IsPublic) {
						xpublic = true;
					}
					if (xset != null && xset.IsPublic) {
						xpublic = true;
					}
				}
				else if (x is FieldInfo) {
					xpublic = ((FieldInfo) x).IsPublic;
				}
				else if (x is MethodInfo) {
					xpublic = ((MethodInfo) x).IsPublic;
				}

				if (y is PropertyInfo) {
					var yget = ((PropertyInfo) y).GetGetMethod();
					var yset = ((PropertyInfo) y).GetSetMethod();
					if (yget != null && yget.IsPublic) {
						ypublic = true;
					}
					if (yset != null && yset.IsPublic) {
						ypublic = true;
					}
				}
				else if (y is FieldInfo) {
					ypublic = ((FieldInfo) y).IsPublic;
				}
				else if (y is MethodInfo) {
					ypublic = ((MethodInfo) y).IsPublic;
				}

				if (xpublic && !ypublic) {
					return -1;
				}
				if (ypublic && !xpublic) {
					return 1;
				}
				if (x.Name == _testname) {
					return -1;
				}
				if (y.Name == _testname) {
					return 1;
				}
				if (x is PropertyInfo && y is FieldInfo) {
					return -1;
				}
				if (y is PropertyInfo && x is PropertyInfo) {
					return 1;
				}
				return string.CompareOrdinal(x.Name, y.Name);
			}

			private readonly string _testname;
		}

		#endregion

		private readonly Type _basedictint = typeof (Dictionary<object, object>).GetGenericTypeDefinition();

		private readonly MethodInfo _getdictvalmethodbase =
			typeof (ReflectionHelper).GetMethods().First(x => x.Name == "GetDictionaryValue").
				GetGenericMethodDefinition();

		private readonly MethodInfo _setdictvalmethodbase =
			typeof (ReflectionHelper).GetMethods().First(x => x.Name == "SetDictionaryValue").
				GetGenericMethodDefinition();
	}
}