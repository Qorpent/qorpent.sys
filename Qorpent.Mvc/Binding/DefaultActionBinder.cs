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
// Original file : DefaultActionBinder.cs
// Project: Qorpent.Mvc
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.Reflection;
using Qorpent.IoC;
using Qorpent.Utils;

namespace Qorpent.Mvc.Binding {
	/// <summary>
	/// 	Default action binder of action types, used by ActionTypeDescriptor
	/// </summary>
	[ContainerComponent(Lifestyle.Transient)]
	public class DefaultActionBinder : IActionBinder {
		/// <summary>
		/// 	Executes binding action to context
		/// </summary>
		/// <param name="action"> </param>
		/// <param name="context"> </param>
		public void Bind(ActionDescriptor action, IMvcContext context) {
			lock (this) {
				if (null == _binders) {
					Setup(action.ActionType);
				}
				foreach (var binder in _binders) {
					binder.Bind(action.Action, context);
				}
			}
		}

		/// <summary>
		/// 	Setup some info on actionDescriptor for following usage
		/// </summary>
		/// <param name="type"> </param>
		public void Setup(Type type) {
			if (null == _binders) {
				var binders = new List<BindExecutor>();
				var members = type.GetMembers(BindingFlags.SetField | BindingFlags.SetProperty |
				                              BindingFlags.Instance |
				                              BindingFlags.Public | BindingFlags.NonPublic);
				foreach (var m in members) {
					if (m.GetCustomAttributes(typeof (BindAttribute), true).Length != 0) {
						var a = m.GetCustomAttributes(typeof (BindAttribute), true)[0] as BindAttribute;
						binders.Add(new BindExecutor(a, new ValueMember(m, false)));
					}
				}
				_binders = binders.ToArray();
			}
		}

		private BindExecutor[] _binders;
	}
}