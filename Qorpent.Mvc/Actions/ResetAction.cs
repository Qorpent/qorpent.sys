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
// PROJECT ORIGIN: Qorpent.Mvc/ResetAction.cs
#endregion
using System.ComponentModel;
using Qorpent.Events;

namespace Qorpent.Mvc.Actions {
	///<summary>
	///	Вызывает событие <see cref="ResetEvent" /> с данными на основе <see cref="ActionBase.Context" />
	///</summary>
	///<remarks>
	///	<para> Позволяет сбросить внутреннее состояние системы (мягкая перезагрузка) </para>
	///	<para> Для вызова полной очистки (кроме тех объектов сброса, который требуют явного указания имени) используйте note </para>
	///	<note>Некоторые объекты очистки, например
	///		<see cref="Container" />
	///		в целях большей сохранности среды приложения
	///		используют только явную перезагрузку</note>
	///	<para> Требуемые ключи сброса можно получить из <see cref="ResetHandlersAction" /> </para>
	///	Возвращает <see cref="ResetEventResult" />, вся информация при этом
	///	находится в <see cref="ResetResultInfo" /> в коллекции <see cref="ResetEventResult.InvokeList" />.
	///	Каждый параметр запроса трансформируется в опцию вызова Reset, включая "all"
	///</remarks>
	///<example>
	///	<para> Пример вызова с опцией "все" </para>
	///	<strong>http://myserver/myapp/_sys/reset.json.qweb?all=1</strong>
	///</example>
	///<example>
	///	<para> Пример вызова с опцией "только filenameresolver и mvcfactory" </para>
	///	<strong>http://myserver/myapp/_sys/reset.json.qweb?filenameresolver=1&amp;mvcfactory=1</strong>
	///</example>
	///<example>
	///	<para> Пример результата </para>
	///	<code>{
	///		"InvokeList": {
	///		"0": {
	///		"Name": "Qorpent.Mvc.MvcFactory"
	///		},
	///		"1": {
	///		"Name": "Qorpent.IO.FileNameResolver",
	///		"Info": {
	///		"Id": 4,
	///		"Version": 2,
	///		"CacheCount": 0
	///		}
	///		}
	///		}
	///		}</code>
	///</example>
	[Action("_sys.reset", Arm="admin")]
	public class ResetAction : ActionBase {
		/// <summary>
		/// 	processing of execution - main method of action
		/// </summary>
		/// <returns> </returns>
		protected override object MainProcess() {
			return Call<ResetEventResult>(new ResetEventData(Context), Context.User);
		}
	}
}