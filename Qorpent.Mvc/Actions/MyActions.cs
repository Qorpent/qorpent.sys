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
	/// 	�������� ��� ��������� ������ ��������� ��������
	/// </summary>
	[Action("_sys.myactions", Role = "REFAULT", Help = "��������� �������� ������ ��������� ��������")]
	public class MyActions : ActionBase {

        /// <summary>
        /// 
        /// </summary>
	    [Bind] protected string Usage;
		/// <summary>
		/// 	� ���������� ������ ���� ��������� ������������ ��������
		/// </summary>
		/// <returns> </returns>
		protected override object MainProcess() {
            if (Usage == "ui") {
                return RealCommands();
            }

            if (
                Application.Roles.IsInRole(Context.LogonUser, "DEVELOPER")
                    ||
                Application.Roles.IsInRole(Context.LogonUser, "MASTERUSER")
            ) {
                return GetAllActions();
            }

		    return null;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private object RealCommands() {
            var logon = Context.LogonUser;
            var actions = GetAllActions();
            var dict = new Dictionary<string, IDictionary<string, IDictionary<string, object>>>();

            foreach (var action in actions) {
                if (!Application.Roles.IsInRole(logon, action.DirectRole)) {
                    continue;
                }

                var exploded = action.Name.Split(new[] { "." }, StringSplitOptions.None);

                // if domain not exists
                if (!dict.ContainsKey(exploded[0])) {
                    dict.Add(exploded[0], new Dictionary<string, IDictionary<string, object>>());
                }

                var parameters = new Dictionary<string, IDictionary<string, object>>();

                foreach (var binding in action.GetBindings()) {
                    parameters.Add(
                        binding.Name,
                        new Dictionary<string, object> {
                            {"Required", binding.Required},
                            {"Help", binding.Help},
                            {"IsBool", binding.IsBool},
                            {"IsColor", binding.IsColor},
                            {"IsComplexString", binding.IsComplexString},
                            {"IsEnum", binding.IsEnum},
                            {"IsLargeText", binding.IsLargeText},
                            {"IsDate", binding.IsDate},
                            {"Default", binding.Default},
                            {"UpperCase", binding.UpperCase},
                            {"LowerCase", binding.LowerCase}
                        }
                    );
                }

                dict[exploded[0]][exploded[1]] = new Dictionary<string, object> {
                    {"help", action.Help},
                    {"parameters", parameters}
                };
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