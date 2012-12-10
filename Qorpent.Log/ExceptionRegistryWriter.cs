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
// Original file : ExceptionRegistryWriter.cs
// Project: Qorpent.Log
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;

namespace Qorpent.Log {
	/// <summary>
	/// 	Обработчик ошибок
	/// </summary>
	public class ExceptionRegistryWriter : ServiceBase, ILogWriter {
		/// <summary>
		/// </summary>
		public IExceptionRegistry Registry {
			get {
				if (null == _registry) {
					_registry = ResolveService<IExceptionRegistry>(RegistryName);
				}
				return _registry;
			}
			set { _registry = value; }
		}

		/// <summary>
		/// </summary>
		public string RegistryName { get; set; }

		/// <summary>
		/// 	writes message synchronously on down level
		/// </summary>
		/// <param name="message"> </param>
		public void Write(LogMessage message) {
			if (!_active) {
				return;
			}
			if (null == Registry) {
				_active = false;
				return;
			}
			if (message.Level >= LogLevel.Info && message.Error != null) {
				var title = message.Message;

				var errorlevel = ErrorLevel.None;
				if (message.Level == LogLevel.Info) {
					errorlevel = ErrorLevel.Hint;
				}
				else if (message.Level == LogLevel.Error) {
					errorlevel = ErrorLevel.Error;
				}
				else if (message.Level == LogLevel.Warning) {
					errorlevel = ErrorLevel.Warning;
				}
				else if (message.Level == LogLevel.Fatal) {
					errorlevel = ErrorLevel.Fatal;
				}

				Registry.Send(message.Error, errorlevel, GetAdvancedParams(message), title);
			}
		}

		/// <summary>
		/// 	Minimal log level of writer
		/// </summary>
		public LogLevel Level { get; set; }

		/// <summary>
		/// 	Подготавливает дополнительные параметры ошибки
		/// </summary>
		/// <param name="message"> Сообщение </param>
		/// <returns> Возвращает набор дополнительных параметров </returns>
		private object GetAdvancedParams(LogMessage message) {
			var url = "";
			var action = "";
			var render = "";
			var callinfo = message.MvcCallInfo;
			if (null == callinfo && null != Application.CurrentMvcContext) {
				callinfo = Application.CurrentMvcContext.GetCallInfo();
			}
			if (null != callinfo) {
				url = callinfo.Url;
				action = callinfo.ActionName;
				render = callinfo.RenderName;
			}
			var shorturl = url;
			if(url.Length>60) {
				shorturl = url.Substring(0, 50) + "...";
			}
			var result = new Dictionary<string, object>
				{
					{"server", message.Server},
					{"user", message.User},
					{"logcode", message.Code},
					{"application", message.ApplicationName},
					{"component", message.HostObject},
					{"logger", message.Name},
					{"time", message.Time},
					{"fullurl", url},
					{"url", shorturl},
					{"action", action},
					{"render", render},
				};

			var ex = message.Error;
			while (ex!=null) {
				var exerror = ex as IExceptionRegistryDataException;
				if (null != exerror)
				{
					if (null != exerror.ExceptionRegisryData)
					{
						foreach (var p in exerror.ExceptionRegisryData)
						{
							result[p.Key] = p.Value;
						}
					}
				}
				ex = ex.InnerException;
			}

			
			return result;
		}

		private bool _active = true;
		private IExceptionRegistry _registry;
	}
}