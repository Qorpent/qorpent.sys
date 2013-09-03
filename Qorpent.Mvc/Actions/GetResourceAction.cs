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
// PROJECT ORIGIN: Qorpent.Mvc/GetResourceAction.cs
#endregion
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Qorpent.Mvc.Binding;
using Qorpent.Security;

namespace Qorpent.Mvc.Actions {
	/// <summary>
	/// 	Действие для получения ресурса
	/// </summary>
	[Action("_sys.getresource", Help = "Позволяет получить зашитый в сборку ресурс, считается безопасной операцией;" +
	                                   "если ресурс в составе имени имеет qvexr (QView Export Resource), то он рассматривается" +
	                                   " как общедоступный иначе требуется доступ уровня DEVELOPER", Role = "DEFAULT")]
	public class GetResourceAction : ActionBase {
		/// <summary>
		/// 	ETag подставной NONCHANGEABLE, не варьирует по файлам
		/// </summary>
		/// <returns> </returns>
		protected override string EvalEtag() {
			return "NONCHANGEABLE";
		}

		/// <summary>
		/// 	Временем кэша ресурса считается время загрузки приложения
		/// </summary>
		/// <returns> </returns>
		protected override DateTime EvalLastModified() {
			return Application.StartTime;
		}

		/// <summary>
		/// 	Поддерживает, ресурсы не меняются
		/// </summary>
		/// <returns> </returns>
		protected override bool GetSupportNotModified() {
			return true;
		}

		/// <summary>
		/// 	Загружает сборку и ресурс по имени
		/// </summary>
		protected override void Initialize() {
			_assembly = System.Reflection.Assembly.Load(Assembly);
			if (null != _assembly) {
				_resourcename =
					_assembly.GetManifestResourceNames().FirstOrDefault(x => x.ToLowerInvariant().EndsWith(ResourceName.ToLowerInvariant()));
			}
		}

		/// <summary>
		/// 	Проверяет наличие сборки и ресурса
		/// </summary>
		protected override void Validate() {
			if (null == _assembly) {
				throw new QorpentException("Сборка не найдера - " + Assembly);
			}
			if (null == _resourcename) {
				throw new QorpentException("Не найдено ресурса с именем - " + ResourceName);
			}
		}

		/// <summary>
		/// 	Если имя ресурса (итоговое) содержит qvexr, то он рассматривается как общедоступный
		/// 	иначе требуется роль DEVELOPER
		/// </summary>
		protected override void Authorize() {
			base.Authorize();
			if (!_resourcename.Contains("qvexr")) {
				if (!IsInRole("DEVELOPER")) {
					throw new QorpentSecurityException("Недостаточно полномочий для доступа к закрытым ресурасм ");
				}
			}
		}

		/// <summary>
		/// 	processing of execution - main method of action
		/// </summary>
		/// <returns> </returns>
		protected override object MainProcess() {
			var result = new FileDescriptor
				{Name = _assembly.GetName().Name + "/" + _resourcename, LastWriteTime = Application.StartTime, Role = ""};
			using (var stream = _assembly.GetManifestResourceStream(_resourcename)) {
				Debug.Assert(stream != null, "stream != null");
				var sr = new StreamReader(stream);
				result.Content = sr.ReadToEnd();
			}

			result.Length = result.Content.Length;
			result.MimeType = MimeMapper.Get(_resourcename);
			return result;
		}

		/// <summary>
		/// 	Параметр - имя сборки
		/// </summary>
		[Bind(Help = "Имя сборки", Name = "a")] public string Assembly;

		private Assembly _assembly;

		/// <summary>
		/// 	Паарметр - имя ресурса
		/// </summary>
		[Bind(Help = "Имя ресурса (можно суффикс)",Name="r")] public string ResourceName;

		private string _resourcename;
	}
}