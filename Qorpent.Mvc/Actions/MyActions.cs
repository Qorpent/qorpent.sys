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
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc.Actions {
	/// <summary>
	/// 	Действие для получения списка доступных операций
	/// </summary>
	[Action("_sys.myactions", Role = "DEVELOPER,MASTERUSER", Help = "Позволяет получить список доступных операций")]
	public class MyActions : ActionBase {
		/// <summary>
		/// 	В защищенном режиме ищет доступные пользователю действия
		/// </summary>
		/// <returns> </returns>
		protected override object MainProcess() {
			return
				Container.GetComponents().Where(x => x.ServiceType == typeof (IAction))
					.Select(x => new ActionDescriptor((IAction) Activator.CreateInstance(x.ImplementationType)))
					//не напрягаем контейнер
					.Where(x => IsAccessible(x))
                    .Select(_ =>
                        new{
                            _.Name,
                            _.Help,
                            _.ActionTypeName,
                            _.DirectRole,
                            _.Role,
                            Parameters = GetParameters(_),
 
                        })
					.ToArray();
		}

	    private IDictionary<string,object> GetParameters(ActionDescriptor actionDescriptor)
	    {
	        return
	            (from vm in ReflectionExtensions.FindAllValueMembers(actionDescriptor.ActionType, typeof (BindAttribute))
	             let attr = vm.Member.GetCustomAttributes(typeof (BindAttribute), true).First() as BindAttribute
	             let name = string.IsNullOrWhiteSpace(attr.Name) ? vm.Member.Name : attr.Name
	             let type = vm.Type.Name
	             select new
	                 {
	                     name,
	                     type,
	                     help = attr.Help,
                         required = attr.Required,
                         pattern = attr.ValidatePattern,
                         fixedset = attr.Constraint,
                         defval = attr.Default,
	                 }
	            ).ToDictionary(_=>_.name,_=>(object)_);
	    }
	}
}