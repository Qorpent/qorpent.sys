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
// PROJECT ORIGIN: Qorpent.Core/BindAttribute.cs
#endregion
using System;
using System.Linq;
using System.Reflection;
using Qorpent.Serialization;

namespace Qorpent.Mvc.Binding {
	/// <summary>
	/// 	Describes how to bind input parameters to action during initialization, used by IActionBinder
	/// </summary>
	[Serialize]
	public class BindAttribute : Attribute {
		/// <summary>
		/// 	creates default bind
		/// </summary>
		public BindAttribute() {}

		/// <summary>
		/// 	creates bind with default Value
		/// </summary>
		/// <param name="def"> </param>
		public BindAttribute(object def) {
			Default = def;
		}

		/// <summary>
		/// 	Flags that it will be lrge text, - can force textarea to be shown
		/// </summary>
		public bool IsLargeText { get; set; }

		/// <summary>
		/// 	Flags that parameter can be parsed as dictionary string in form A:B|C:D|X|Y
		/// </summary>
		public bool IsComplexString { get; set; }

		/// <summary>
		/// 	Flags that it's RGB color definition
		/// </summary>
		public bool IsColor { get; set; }

		/// <summary>
		/// 	Mark that it's string bind
		/// </summary>
		[IgnoreSerialize] public bool IsString {
			get { return typeof (string) == TargetType; }
		}

		/// <summary>
		/// 	Mark that it's bool bind
		/// </summary>
		[IgnoreSerialize] public bool IsBool {
			get { return typeof (bool) == TargetType; }
		}

		/// <summary>
		/// 	Mark that it's datatime bind
		/// </summary>
		[IgnoreSerialize] public bool IsDate {
			get { return typeof (DateTime) == TargetType; }
		}

		/// <summary>
		/// 	Mark that it's enumeration bind
		/// </summary>
		[IgnoreSerialize] public bool IsEnum {
			get { return TargetType.IsEnum; }
		}

		/// <summary>
		/// 	Default object to be bind if no in context
		/// </summary>
		[Serialize] public object Default { get; set; }

		/// <summary>
		/// 	back reference to attached MemberInfo
		/// </summary>
		public MemberInfo Member { get; set; }

		/// <summary>
		/// 	Flag that bind reqires client validation
		/// </summary>
		public bool RequireValidation {
			get { return Required || (null != Constraint && 0 != Constraint.Length); }
		}

		/// <summary>
		/// 	Convert to Lower on bind
		/// </summary>
		public bool LowerCase { get; set; }

		/// <summary>
		/// 	Convert to Upper on bind
		/// </summary>
		public bool UpperCase { get; set; }

		/// <summary>
		/// 	Name or parameter in context
		/// </summary>
		public string Name {
			get {
				if (string.IsNullOrEmpty(_parameterName)) {
					return Member.Name;
				}
				return _parameterName;
			}
			set { _parameterName = value; }
		}

		/// <summary>
		/// 	Name of target type to be bound to
		/// </summary>
		public string TypeName {
			get { return TargetType.FullName; }
		}


		/// <summary>
		/// 	Back reference to target bind type
		/// </summary>
		public Type TargetType {
			get {
				if (null == _targettype) {
					lock (this) {
						var fieldInfo = Member as FieldInfo;
						if (fieldInfo != null) {
							_targettype = fieldInfo.FieldType;
						}
						var propertyInfo = Member as PropertyInfo;
						if (propertyInfo != null) {
							_targettype = propertyInfo.PropertyType;
						}
					}
				}
				return _targettype;
			}
		}

		/// <summary>
		/// 	Client/server validation - mark required parameters
		/// </summary>
		public bool Required { get; set; }

		/// <summary>
		/// 	Client/server validation - regex to be checked
		/// </summary>
		public string ValidatePattern { get; set; }

		/// <summary>
		/// 	Error message to be shown on invalid bind
		/// </summary>
		public string ErrorMessage {
			get {
#if !SQL2008
				if (string.IsNullOrWhiteSpace(_errorMessage)) {
#else
				if (string.IsNullOrEmpty(_errorMessage))
				{
#endif
					lock (this) {
						_errorMessage = string.Format("Parameter '{0}' :", Name);
						if (Required) {
							_errorMessage += " is required;";
						}
						if (null != Constraint && 0 != Constraint.Length) {
							_errorMessage += " is constrainted to " + string.Join(", ", Constraint.Select(x => x.ToString()).ToArray()) + ";";
						}
					}
				}
				return _errorMessage;
			}
			set { _errorMessage = value; }
		}

		/// <summary>
		/// 	Набор допустимых значений
		/// </summary>
		public object[] Constraint { get; set; }


		/// <summary>
		/// 	Справочаня информация для отрисовки на клиете
		/// </summary>
		public string Help { get; set; }

		private string _errorMessage;
		private string _parameterName;
		private Type _targettype;
	}
}