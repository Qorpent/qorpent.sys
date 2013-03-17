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
// PROJECT ORIGIN: Qorpent.Mvc/ResetHandlersAction.cs
#endregion
using Qorpent.Events;

namespace Qorpent.Mvc.Actions {
	///<summary>
	///	Возвращает перечень объектов обработки команды сброса состояния <see cref="ResetAction" />
	///</summary>
	///<remarks>
	///	Наиболее часто результатом является массив <see cref="StandardResetHandler" />, соответственно
	///	для анализа доступно <see cref="StandardResetHandler.PreResetInfo" />
	///</remarks>
	///<example>
	///	<para> Пример строки вызова </para>
	///	<strong>http://myserver/myapp/_sys/resethandlers.json.qweb</strong>
	///</example>
	///<example>
	///	<para> Пример результата </para>
	///	<code>{
	///		"0": {
	///		"TargetTypeName": "Qorpent.IoC.Container",
	///		"PreResetInfo": {
	///		"outcache": 9,
	///		"poolsize": 0
	///		},
	///		"AllIsSupported": false,
	///		"SupportedOptions": {
	///		"0": "container",
	///		"1": "ioc.container"
	///		}
	///		},
	///		"1": {
	///		"TargetTypeName": "Qorpent.Mvc.MvcFactory",
	///		"PreResetInfo": {
	///		"actions": 2,
	///		"renders": 3,
	///		"views": 3
	///		},
	///		"AllIsSupported": true,
	///		"SupportedOptions": {
	///		"0": "all",
	///		"1": "mvcfactory",
	///		"2": "mvc.factory"
	///		}
	///		},
	///		"2": {
	///		"TargetTypeName": "Qorpent.IO.FileNameResolver",
	///		"PreResetInfo": {
	///		"cacheSize": 2
	///		},
	///		"AllIsSupported": true,
	///		"SupportedOptions": {
	///		"0": "all",
	///		"1": "filenameresolver"
	///		}
	///		}
	///		}</code>
	///</example>
	[Action("_sys.resethandlers", Role = "DEFAULT", Help = "Список возможных объектов для сброса состояния")]
	public class ResetHandlersAction : ActionBase {
		/// <summary>
		/// 	processing of execution - main method of action
		/// </summary>
		/// <returns> </returns>
		protected override object MainProcess() {
			return
				Context.Application.Events.GetHandlers(typeof (ResetEvent), Context.User);
		}
	}
}