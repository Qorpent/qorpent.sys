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
// PROJECT ORIGIN: Qorpent.Utils/ReflectionExtensions.cs
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

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
		/// �������� �������� ������ ������ � ������ �� �����
		/// </summary>
		/// <param name="target"></param>
		/// <param name="from"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T BindFrom<T>(this T target, object from)
		{
			return BindFrom(target, from, false);
		}

		/// <summary>
		/// �������� �������� ������ ������ � ������ �� �����
		/// </summary>
		/// <param name="target"></param>
		/// <param name="from"></param>
		/// <param name="primitivesOnly"></param>
		/// <param name="excludes"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T BindFrom<T>(this T target, object from, bool primitivesOnly, params string[] excludes)
		{
			excludes = excludes ?? new string[] { };
			PropertyInfo[] props = from.GetType().GetProperties();
			foreach (PropertyInfo src in props)
			{
				if (excludes.ToBool() && src.Name.IsIn(excludes)) continue;

				if (!primitivesOnly || src.PropertyType == typeof(string) || src.PropertyType.IsValueType)
				{
					object val = src.GetValue(from, null);
					if (null != val)
					{
						target.SetValue(src.Name, val, ignoreNotFound:true);
					}
				}
			}
			return target;
		}

		/// <summary>
		/// ������������ ������������� ��� ���� � ���
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static Type ResolveTypeByWellKnownName(string name)
		{
			switch (name)
			{
				case "int":
					return typeof(int);
				case "str":
					return typeof(string);
				case "date":
					return typeof(DateTime);
				case "bool":
					return typeof(bool);
				case "decimal":
					return typeof(decimal);
			}
			return null;
		}
		/// <summary>
		/// ������������ ��� � ������������� �������� ���
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string ResolveWellKnownName(Type type)
		{
			switch (type.Name)
			{
				case "Int32":
					return "int";
				case "String":
					return "str";
				case "DateTime":
					return "date";
				case "Boolean":
					return "bool";
				case "Decimal":
					return "decimal";
			}
			return null;
		}


		/// <summary>
		/// 	Default domain wide helper
		/// </summary>
		public static ReflectionHelper DefaultHelper {
			get { return _helper; }
			set { _helper = value; }
		}
		/// <summary>
		/// Converts given type reference string to Type object
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static Type ToTypeDefinition(this string str)
		{
			return ToTypeDefinition(str, null);
		}

		/// <summary>
		/// Converts given type reference string to Type object using mapping
		/// </summary>
		/// <param name="str"></param>
		/// <param name="map"></param>
		/// <returns></returns>
		/// <exception cref="NullReferenceException"></exception>
		public static Type ToTypeDefinition(this string str, IDictionary<string, Type> map = null)
		{
			if (string.IsNullOrWhiteSpace(str)) return null;

			if (str == "string") return typeof(string);
			if (str == "int") return typeof(int);
			if (str == "decimal") return typeof(decimal);
			if (str == "date") return typeof(DateTime);
			if (str == "bool") return typeof(bool);

			if (str.EndsWith(".dll")) str = str.Substring(0, str.Length - 4);
			if (null != map && map.ContainsKey(str))
			{
				return map[str];
			}
			Type result = Type.GetType(str);
			if (null == result) throw new NullReferenceException(str + " maps to non-correct or unavailable type");
			return result;
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
		/// 	���� � ���� ��� �������� � ���� �� �������� �������� � ��������� ��������� ������������ ��������
		/// </summary>
		/// <param name="type"> ���, � ������� �������� ���������� ����� </param>
		/// <param name="attributeType"> ����������� ��� �������� (����� ������ ��� ������ ������, � ������� �������� ������ ������� </param>
		/// <param name="publicOnly"> ������ ��������� ��������/���� </param>
		/// <param name="readableOnly"> ������ �������� � ���������� ������ (��� ����� ������������) </param>
		/// <param name="assignableOnly"> ������ �������� � ���������� ������ (��� ����� ������������) </param>
		/// <exception cref="ReflectionExtensionsException">�� ������ ��� ��� ������</exception>
		/// <returns> ������������ ���� ��������������� ����� � ������� </returns>
		public static IEnumerable<ValueMember> FindAllValueMembers(this Type type, Type attributeType, bool publicOnly = false,
		                                                           bool readableOnly = false, bool assignableOnly = false) {
			return _helper.FindAllValueMembers(type, attributeType, publicOnly, readableOnly, assignableOnly);
		}


		
		/// <summary>
		/// Shortcut to <paramref name="assembly"/> manifest resource stream with path finding
		/// </summary>
		/// <param name="assembly"></param>
		/// <param name="pathpart"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public static Stream OpenManifestResource(this Assembly assembly, string pathpart) {
			var resourcename = assembly.GetManifestResourceNames().FirstOrDefault(_ => _.ToUpper().EndsWith(pathpart.ToUpper()));
			if(null==resourcename)throw new Exception("resource for "+pathpart+" not found");
			return assembly.GetManifestResourceStream(resourcename);
		}

		/// <summary>
		/// Shortcut to read data from named resource as string
		/// </summary>
		/// <param name="assembly"></param>
		/// <param name="pathpart"></param>
		/// <returns></returns>
		public static string ReadManifestResource(this Assembly assembly, string pathpart) {
			string result = null;
			using (var sr = new StreamReader(assembly.OpenManifestResource(pathpart), Encoding.UTF8)) {
				result = sr.ReadToEnd();
			}
			return result;
		}
	}
}