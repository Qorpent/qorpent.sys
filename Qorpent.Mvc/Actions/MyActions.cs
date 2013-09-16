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
// PROJECT ORIGIN: Qorpent.Mvc/MyActions.cs
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Mvc.Binding;

namespace Qorpent.Mvc.Actions {
	/// <summary>
	/// 	Действие для получения списка доступных операций
	/// </summary>
	[Action("_sys.myactions", Role = "DEFAULT", Help = "Позволяет получить список доступных операций")]
	public class MyActions : ActionBase {

        /// <summary>
        /// 
        /// </summary>
	    [Bind] protected string Usage;
		/// <summary>
		/// 
		/// </summary>
	    [Bind] protected string Command;
		/// <summary>
		/// 	В защищенном режиме ищет доступные пользователю действия
		/// </summary>
		/// <returns> </returns>
		protected override object MainProcess() {
            if (Usage == "ui") {
                return RealCommands();
            }

            if (
                Application.Roles.IsInRole(Context.User, "DEVELOPER")
                    ||
                Application.Roles.IsInRole(Context.User, "MASTERUSER")
            ) {
                return GetAllActions();
            }

		    throw new Exception("You don't have needed roles");
		}

        /// <summary>
        ///     
        /// </summary>
        /// <returns></returns>
        private object RealCommands() {
            var actions = GetAllActions();
			if (!string.IsNullOrWhiteSpace(Command)) {
				actions = actions.Where(_ => _.Name.Replace(".action","") == Command);
			}
            var dict = new Dictionary<string, IDictionary<string, object>>();

            foreach (var action in actions) {
                if (!Application.Roles.IsInRole(Context.User, action.DirectRole)) {
                        continue;
                }

                var exploded = action.Name.Split(new[] { "." }, StringSplitOptions.None);

                if (1 == exploded.Length) {
                    throw new Exception("Invalid action name "+action.Name);
                }

                // if domain not exists
                if (!dict.ContainsKey(exploded[0])) {
                    dict.Add(exploded[0], new Dictionary<string, object>());
                }

                var parameters = new List<object>();

                foreach (var binding in action.GetBindings()) {
                    parameters.Add(
						binding
                    );
                }
				
                dict[exploded[0]][exploded[1]] = new {action.Help, action.Arm, parameters= parameters.ToArray()};
            }

            return dict;
        }

        /// <summary>
        ///     Get all actions
        /// </summary>
        /// <returns></returns>
        private IEnumerable<ActionDescriptor> GetAllActions() {
            return Container.GetComponents().Where(
                x => x.ServiceType == typeof(IAction)
            ).Select(
                x => new ActionDescriptor(
                    (IAction)Activator.CreateInstance(x.ImplementationType)
                )
            ).Where(
                x => IsAccessible(x)
            ).ToArray();
        }
	}
}