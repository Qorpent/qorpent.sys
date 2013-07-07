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
// PROJECT ORIGIN: Qorpent.Mvc/BindExecutor.cs
#endregion
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc.Binding {
	/// <summary>
	/// 	Helper wrapper to provide binding implementation, based on BindAttribute
	/// </summary>
	public class BindExecutor {
		/// <summary>
		/// </summary>
		/// <param name="attribute"> </param>
		/// <param name="memberinfo"> </param>
		public BindExecutor(BindAttribute attribute, ValueMember memberinfo) {
			_bindattribute = attribute;
			attribute.Member = memberinfo.Member;
			_memberinfo = memberinfo;
		}

		/// <summary>
		/// </summary>
		protected bool IsComplexString {
			get { return _bindattribute.IsComplexString; }
		}

		/// <summary>
		/// </summary>
		protected string Name {
			get { return _bindattribute.Name; }
		}

		/// <summary>
		/// </summary>
		protected Type TargetType {
			get { return _bindattribute.TargetType; }
		}

		/// <summary>
		/// 	Executes binding
		/// </summary>
		/// <param name="action"> </param>
		/// <param name="context"> </param>
		public void Bind(IAction action, IMvcContext context) {
			lock (this) {
				if (null == action || null == context) {
					return;
				}

				SetupValue(action, context);
				ValidateBinding(action, context);
			}
		}

		private void ValidateBinding(IAction action, IMvcContext context) {
			if (!_bindattribute.RequireValidation) {
				return;
			}
			var val = GetCurrent(action);
			var isValid = GetIsValid(context, val);
			if (!isValid) {
				throw new BindException(_bindattribute, context, val);
			}
		}

		private bool GetIsValid(IMvcContext context, object val) {
			var isValid = true;
			if (_bindattribute.Required) {
				if (TargetType == typeof (string)) {
					if (string.IsNullOrEmpty(val.ToString())) {
						isValid = false;
					}
					else if (null != _bindattribute.ValidatePattern) {
						var pattern = _bindattribute.ValidatePattern;
						if (!new Regex(pattern).IsMatch(val.ToString())) {
							isValid = false;
						}
					}
				}
				else if (TargetType.IsValueType) {
					if (!context.Parameters.ContainsKey(Name.ToLower())) {
						isValid = false;
					}
				}
				else if (TargetType == typeof (XElement)) {
					if (null == val || ((XElement) val).Name.LocalName == "empty") {
						isValid = false;
					}
				}
				else {
					if (null == val) {
						isValid = false;
					}
				}
			}
			if (isValid && (null != _bindattribute.Constraint && 0 != _bindattribute.Constraint.Length) &&
			    (!string.IsNullOrEmpty(val.ToStr()))) {
				if (-1 == Array.IndexOf(_bindattribute.Constraint, val)) {
					isValid = false;
				}
			}
			return isValid;
		}

		private void SetupValue(IAction action, IMvcContext context) {
			var paramname = Name;
			if (TargetType == typeof (XElement)) {
				var val = context.GetXml(Name);
				SetDirectly(action, val);
			}
			else if (TargetType.IsArray) {
				var val = context.GetArray(TargetType.GetElementType(), paramname);
				SetDirectly(action, val);
			}
			else if (typeof (IDictionary<string, string>) == TargetType) {
				var current = (IDictionary<string, string>) GetCurrent(action);
				if (null == current) {
					current = new Dictionary<string, string>();
					SetDirectly(action, current);
				}

				IDictionary<string, string> val;
				if (IsComplexString) {
					var str = context.Get(paramname);
					val = ComplexStringExtension.Parse(str);
				}
				else {
					val = context.GetDict(paramname);
				}
				foreach (var p in val) {
					current[p.Key] = p.Value;
				}
			}
			else {
				object val = context.Get(paramname);
				if (((string) val).IsEmpty()) {
					val = _bindattribute.Default;
				}
				var customBindConverter = _bindattribute as ICustomBindConverter;
				if (customBindConverter != null) {
					customBindConverter.SetConverted(action,(string)val,context,SetDirectly);
					return;
				}
				SetConverted(action, val);
			}
		}

		private void SetConverted(IAction action, object val) {
			var converted = val.ToTargetType(TargetType);
			if (TargetType == typeof (string)) {
				var s = (string) converted;
				if (null != s) {
					if (_bindattribute.LowerCase) {
						s = s.ToLower();
					}
					if (_bindattribute.UpperCase) {
						s = s.ToUpper();
					}
				}
				converted = s;
			}
			SetDirectly(action, converted);
		}

		private object GetCurrent(object action) {
			return _memberinfo.Get(action);
		}

		private void SetDirectly(object action, object val) {
			_memberinfo.Set(action, val);
		}

		private readonly BindAttribute _bindattribute;
		private readonly ValueMember _memberinfo;
	}
}