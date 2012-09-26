﻿#region LICENSE

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
// Original file : QViewBinder.cs
// Project: Qorpent.Mvc
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Qorpent.IoC;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc.QView {
	/// <summary>
	/// 	Applys calling context to view
	/// </summary>
	[ContainerComponent(Lifestyle.Transient)]
	public class QViewBinder : IQViewBinder {
		/// <summary>
		/// 	Set view to be bound to
		/// </summary>
		/// <param name="view"> </param>
		public void SetView(IQView view) {
			var binds =
				from m in
					view.GetType().GetMembers(BindingFlags.SetField | BindingFlags.SetProperty | BindingFlags.Instance |
					                          BindingFlags.Public | BindingFlags.NonPublic)
				let ba = m.GetFirstAttribute<QViewBindAttribute>()
				where null != ba
				select new {m, ba};
			bindattributes = new Dictionary<QViewBindAttribute, ValueMember>();
			foreach (var bind in binds) {
				if (bind.ba.Name.IsEmpty()) {
					bind.ba.Name = bind.m.Name;
				}
				bind.ba.Member = bind.m;
				bindattributes[bind.ba] = new ValueMember(bind.m, false);
			}
		}

		/// <summary>
		/// 	Applys context to target view
		/// </summary>
		/// <param name="context"> </param>
		/// <param name="view"> </param>
		public void Apply(IQViewContext context, IQView view) {
			foreach (var bind in bindattributes) {
				var value = bind.Key.GetDataToBeBound(context);
				bind.Value.Set(view, value);
			}
		}

		private Dictionary<QViewBindAttribute, ValueMember> bindattributes;
	}
}