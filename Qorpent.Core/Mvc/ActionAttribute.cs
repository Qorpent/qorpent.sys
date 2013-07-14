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
// PROJECT ORIGIN: Qorpent.Core/ActionAttribute.cs
#endregion
using System;
using System.Linq;
using Qorpent.IoC;

namespace Qorpent.Mvc {
	/// <summary>
	/// 	Defines classes that would treated as actions
	/// </summary>
	public class ActionAttribute : ContainerComponentAttribute {
		/// <summary>
		/// 	Creates new action definition
		/// </summary>
		/// <param name="name"> </param>
		public ActionAttribute(string name) {
			if (!name.EndsWith(".action")) {
				name += ".action";
			}
			Name = name;
            Arm = "default";
			ServiceType = typeof (IAction);
			Lifestyle = Lifestyle.Transient;
		}
        /// <summary>
        /// Целевой АРМ действия (Q-24)
        /// </summary>
        public string Arm { get; set; }

		/// <summary>
		/// 	get name of action (attributebased)
		/// </summary>
		/// <param name="action"> </param>
		/// <returns> </returns>
		public static string GetName(IAction action) {
			var type = action.GetType();
			return GetName(type);
		}

        /// <summary>
        /// 	get name of action (attributebased)
        /// </summary>
        /// <param name="action"> </param>
        /// <returns> </returns>
        public static string GetArm(IAction action)
        {
            var type = action.GetType();
            return GetArm(type);
        }

        /// <summary>
        /// </summary>
        /// <param name="type"> </param>
        /// <returns> </returns>
        public static string GetArm(Type type)
        {
            var attr =
                type.GetCustomAttributes(typeof(ActionAttribute), true).OfType<ActionAttribute>().FirstOrDefault();
            if (null == attr)
            {
                return "default";
            }
            return string.IsNullOrWhiteSpace(attr.Arm) ? "default" : attr.Arm;
        }

		/// <summary>
		/// </summary>
		/// <param name="type"> </param>
		/// <returns> </returns>
		public static string GetName(Type type) {
			var attr =
				type.GetCustomAttributes(typeof (ActionAttribute), true).OfType<ActionAttribute>().FirstOrDefault();
			if (null == attr) {
				var result = type.Name.Replace("_", ".").Replace("/", ".");
				if (result.EndsWith(".action")) {
					result = result.Substring(0, type.Name.Length - 7);
				}
				return result;
			}
			return attr.Name.ToLower();
		}

		/// <summary>
		/// 	Get role of given action (attribute-based)
		/// </summary>
		/// <param name="action"> </param>
		/// <returns> </returns>
		public static string GetRole(IAction action) {
			var type = action.GetType();
			var attr =
				type.GetCustomAttributes(typeof (ActionAttribute), true).OfType<ActionAttribute>().FirstOrDefault();
			if (null == attr) {
				return "";
			}
			return attr.Role;
		}

		/// <summary>
		/// 	Возвращает контекст применения роли
		/// </summary>
		/// <param name="action"> </param>
		/// <returns> </returns>
		public static string GetRoleContext(IAction action) {
			var type = action.GetType();
			var attr =
				type.GetCustomAttributes(typeof (ActionAttribute), true).OfType<ActionAttribute>().FirstOrDefault();
			if (null == attr) {
				return "";
			}
			return attr.RoleContext;
		}

		/// <summary>
		/// 	Get help string - attribute based
		/// </summary>
		/// <param name="action"> </param>
		/// <returns> </returns>
		public static string GetHelp(IAction action) {
			var type = action.GetType();
			var attr =
				type.GetCustomAttributes(typeof (ActionAttribute), true).OfType<ActionAttribute>().FirstOrDefault();
			if (null == attr) {
				return "";
			}
			return attr.Help ?? "";
		}
	}
}