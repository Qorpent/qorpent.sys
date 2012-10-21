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
// Original file : ActionDescriptor.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Qorpent.Mvc.Binding;
using Qorpent.Serialization;

namespace Qorpent.Mvc {
	/// <summary>
	/// 	Describes action phase of MVC execution
	/// </summary>
	[Serialize]
	public class ActionDescriptor : MvcStepDescriptor {
		/// <summary>
		/// 	Creates new descriptor over given action
		/// </summary>
		/// <param name="action"> </param>
		public ActionDescriptor(IAction action) {
			Action = action;
			Name = ActionAttribute.GetName(action);
			DirectRole = ActionAttribute.GetRole(action);
			Help = ActionAttribute.GetHelp(action);
			RoleContext = ActionAttribute.GetRoleContext(action);
			var contextualAction = action as IContextualAction;
			if (contextualAction != null) {
				contextualAction.SetDescriptor(this);
			}
		}

		/// <summary>
		/// 	Reference to action itself
		/// </summary>
		[IgnoreSerialize] public IAction Action { get; set; }


		/// <summary>
		/// 	Full name of action type
		/// </summary>
		public string ActionTypeName {
			get {
				if (null == Action) {
					return null;
				}
				return Action.GetType().AssemblyQualifiedName;
			}
		}


		/// <summary>
		/// 	Shortcut to action type
		/// </summary>
		public Type ActionType {
			get {
				if (null == Action) {
					return null;
				}
				return Action.GetType();
			}
		}

		/// <summary>
		/// 	Retrieves info about last modified of given action
		/// </summary>
		[IgnoreSerialize] public override DateTime LastModified {
			get {
				var notModifiedStateProvider = Action as INotModifiedStateProvider;
				if (notModifiedStateProvider != null) {
					return notModifiedStateProvider.LastModified;
				}
				return new DateTime();
			}
		}

		/// <summary>
		/// 	Retrieves ETag of action
		/// </summary>
		[IgnoreSerialize] public override string ETag {
			get { return ((INotModifiedStateProvider) Action).ETag; }
		}


		/// <summary>
		/// 	True if underlined action support 304 state
		/// </summary>
		public override bool SupportNotModifiedState {
			get {
				if (null == Action) {
					return false;
				}
				var notModifiedStateProvider = Action as INotModifiedStateProvider;
				if (notModifiedStateProvider != null) {
					return notModifiedStateProvider.SupportNotModifiedState;
				}
				return false;
			}
		}

		/// <summary>
		/// 	Help string about action
		/// </summary>
		public string Help {
			get { return _help ?? ""; }
			set { _help = value; }
		}

		/// <summary>
		/// 	Возвращает список связей (параметров) действия
		/// </summary>
		/// <returns> </returns>
		public IEnumerable<BindAttribute> GetBindings() {
			var binders =
				(from p in
					 Action.GetType().GetMembers(BindingFlags.SetProperty | BindingFlags.SetField | BindingFlags.Instance |
					                             BindingFlags.Public | BindingFlags.NonPublic)
				 let a = p.GetCustomAttributes(typeof (BindAttribute), true).OfType<BindAttribute>().FirstOrDefault()
				 where a != null
				 select new {a, p}).ToArray();
			foreach (var binder in binders) {
				binder.a.Member = binder.p;
			}
			return binders.Select(x => x.a).ToArray();
		}

		/// <summary>
		/// 	Binds given context to action
		/// </summary>
		/// <param name="context"> </param>
		public void Bind(IMvcContext context) {
			if (null == _binder) {
				_binder = Factory.GetBinder();
				_binder.Setup(Action.GetType());
			}
			_binder.Bind(this, context);
		}

		/// <summary>
		/// 	Executes given context on action with binding to it
		/// </summary>
		/// <param name="context"> </param>
		/// <returns> </returns>
		public object Process(IMvcContext context) {
			Bind(context);
			return Action.Process(context);
		}

		/// <summary>
		/// 	Подготовка полной строки с описанием роли
		/// </summary>
		/// <returns> </returns>
		protected override string PrepareRole() {
			var denyrole = Name.Replace(".", "_").ToUpperInvariant() + "_DENY";
			var allowrole = Name.Replace(".", "_").ToUpperInvariant() + "_ALLOW";
			var role = DirectRole;
			if (string.IsNullOrEmpty(role) || "DEFAULT" == role) {
				return "!" + denyrole + ",DEFAULT";
			}
			if ((role.Contains("&") || role.Contains("!") || role.Contains("|")) && !(role.Contains(","))) {
				//формульная роль
				return string.Format("{0} & ( ( {1} ) | {2} ) ", denyrole, role, allowrole);
			}
			//если же это обычный список ролей
			return string.Format("!{0},{1},{2}", denyrole, role, allowrole);
		}

		private IActionBinder _binder;
		private string _help;
	}
}