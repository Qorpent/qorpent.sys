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
// Original file : QViewBindAttribute.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Qorpent.Mvc.QView {
	/// <summary>
	/// 	Describes bind behavior for QView properties to calling context
	/// </summary>
	public class QViewBindAttribute : Attribute {
		private const BindingFlags allattr = BindingFlags.NonPublic | BindingFlags.Public |
		                                     BindingFlags.Instance |
		                                     BindingFlags.SetProperty | BindingFlags.SetField
		                           ;

		/// <summary>
		/// 	Name of item to be bound to
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 	Default value for binding
		/// </summary>
		public object Default { get; set; }

		/// <summary>
		/// 	Back reference to member
		/// </summary>
		public MemberInfo Member { get; set; }

		/// <summary>
		/// </summary>
		/// <returns> </returns>
		protected string GetName() {
			return Name ?? Member.Name;
		}

		/// <summary>
		/// 	Retrieves data from context
		/// </summary>
		/// <param name="context"> </param>
		/// <returns> </returns>
		public virtual object GetDataToBeBound(IQViewContext context) {
			if (context.AdvancedData != null) {
				if (context.AdvancedData is IDictionary<string, object>) {
					var d = context.AdvancedData as IDictionary<string, object>;
					if (d.ContainsKey(GetName())) {
						return d[GetName()];
					}
				}
				else {
					var member = context.AdvancedData.GetType().GetMember(GetName(), allattr).FirstOrDefault();
					if (null != member) {
						if (member is PropertyInfo) {
							return ((PropertyInfo) member).GetValue(context.AdvancedData, null);
						}
						if (member is FieldInfo) {
							return ((FieldInfo) member).GetValue(context.AdvancedData);
						}
					}
				}
			}


			if (context.ViewData != null) {
				var member =
					context.ViewData.GetType().GetMembers(allattr)
						.Where(x => x.Name == GetName()).FirstOrDefault();
				if(null==member) {
					member =
					context.ViewData.GetType().GetMembers(allattr)
						.Where(x => x.Name.ToLowerInvariant() == GetName().ToLowerInvariant()).FirstOrDefault();
				}
				if (null != member) {
					if (member is FieldInfo) {
						return ((FieldInfo) member).GetValue(context.ViewData);
					}
					if (member is PropertyInfo) {
						return ((PropertyInfo) member).GetValue(context.ViewData, null);
					}
				}
			}
			if (context.Context != null && context.Context.Parameters.ContainsKey(GetName().ToLower())) {
				return context.Context.Parameters[GetName()];
			}
			return Default;
		}
	}
}