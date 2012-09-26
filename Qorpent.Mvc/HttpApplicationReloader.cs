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
// Original file : HttpApplicationReloader.cs
// Project: Qorpent.Mvc
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Timers;
using Qorpent.Applications;
using Qorpent.Events;
using Qorpent.IoC;

namespace Qorpent.Mvc {
	/// <summary>
	/// 	Перегружает полностью серверное приложение с задержкой 2 секунды
	/// </summary>
	[ContainerComponent(Lifestyle.Transient)]
	[RequireReset(Role = "DEVELOPER", All = false, Options = new[] {"appreload"})]
	public class HttpApplicationReloader : IApplicationStartup, IResetable {
		/// <summary>
		/// </summary>
		public HttpApplicationReloader() {
			_timer = new Timer {Enabled = false, Interval = TimeSpan.FromSeconds(2).TotalMilliseconds};
			_timer.Stop();
			_timer.Elapsed += timer_Elapsed;
		}


		/// <summary>
		/// 	An index of object
		/// </summary>
		public int Idx { get; set; }

		/// <summary>
		/// 	Executes some startup logic against given application
		/// </summary>
		/// <param name="application"> </param>
		public void Execute(IApplication application) {
			application.Events.Add(new StandardResetHandler(this));
		}


		/// <summary>
		/// 	Вызывает сброс основного домена приложения
		/// </summary>
		public object Reset(ResetEventData data) {
			_timer.Enabled = true;
			_timer.Start();
			return "reloaded";
		}

		/// <summary>
		/// 	Возващает объект, описывающий состояние до очистки
		/// </summary>
		/// <returns> </returns>
		public object GetPreResetInfo() {
			return null;
		}


		private void timer_Elapsed(object sender, ElapsedEventArgs e) {
			_timer.Stop();
			AppDomain.Unload(AppDomain.CurrentDomain);
		}

		private readonly Timer _timer;
	}
}