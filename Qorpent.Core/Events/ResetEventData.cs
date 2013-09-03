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
// PROJECT ORIGIN: Qorpent.Core/ResetEventData.cs
#endregion
using System.Collections.Generic;
using System.Linq;
using Qorpent.Applications;
using Qorpent.Mvc;

namespace Qorpent.Events {
	/// <summary>
	/// 	data for reset event, contains info which items must be reseted
	/// </summary>
	public class ResetEventData : IEventData {
		/// <summary>
		/// 	Создает пустые данне для сброса
		/// </summary>
		public ResetEventData() {}

		/// <summary>
		/// 	Создает данные для сброса на основе переданного запроса
		/// </summary>
		/// <param name="context"> </param>
		public ResetEventData(IMvcContext context) {
			BuildMvcContext(context);
		}

		/// <summary>
		/// 	Создает данные для перезагрузки из признака "все" и набора строк
		/// </summary>
		/// <param name="all"> </param>
		public ResetEventData(bool all) {
			All = all;
		}

		/// <summary>
		/// 	Создает данные для перезагрузки из признака "все" и набора строк
		/// </summary>
		/// <param name="options"> </param>
		public ResetEventData(IEnumerable<string> options) {
			if (null != options) {
				foreach (var option in options) {
					Set(option);
				}
			}
		}

		/// <summary>
		/// 	Indicates that all reset handlers must be invoked
		/// </summary>
		public bool All { get; set; }


		/// <summary>
		/// 	Создает данные для вызова <see cref="ResetEvent" /> на основе контекста приложения
		/// </summary>
		/// <param name="context"> </param>
		/// <returns> </returns>
		public IEventData Build(IApplication context) {
			var ctx = context.CurrentMvcContext ?? new SimpleMvcContext();
			return BuildMvcContext(ctx);
		}


		/// <summary>
		/// 	manual set of option to reset
		/// </summary>
		/// <param name="option"> </param>
		/// <returns> </returns>
		public ResetEventData Set(string option) {
			if (option == "all") {
				All = true;
			}
			else {
				_options.Add(option);
			}
			return this;
		}

		private IEventData BuildMvcContext(IMvcContext ctx) {
			var allparam = ctx.Get("all").ToUpper();
			All = allparam == "TRUE" || allparam == "1";
			foreach (var rp in ctx.Parameters) {
				var key = rp.Key;
				var val = rp.Value;
				if (key.StartsWith("rp.")) {
					key = key.Substring(3);
				}
				if (val == "1") {
					_options.Add(key);
				}
			}
			return this;
		}

		/// <summary>
		/// 	Determines is given string match for current options
		/// </summary>
		/// <param name="data"> </param>
		/// <param name="useAllForAllOptions"> True (по умолчанию) - опция <see cref="All" /> расценивается как замена любой опции </param>
		/// <returns> </returns>
		public bool IsSet(string data, bool useAllForAllOptions = true) {
			if (useAllForAllOptions && All) {
				return true;
			}
			if (_options.Contains(data)) {
				return true;
			}
			if (_options.Contains(data.Split('.')[0])) {
				return true;
			}
			if (null != _options.FirstOrDefault(x => x.StartsWith(data + "."))) {
				return true;
			}
			return false;
		}

		private readonly IList<string> _options = new List<string>();
	}
}