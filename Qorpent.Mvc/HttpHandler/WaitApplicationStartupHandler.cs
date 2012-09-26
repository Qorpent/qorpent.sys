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
// Original file : WaitApplicationStartupHandler.cs
// Project: Qorpent.Mvc
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Web;

namespace Qorpent.Mvc.HttpHandler {
	/// <summary>
	/// 	Stub handler to get quick answer if application is in startup
	/// </summary>
	public class WaitApplicationStartupHandler : StubHandlerBase, IHttpHandler {
		/// <summary>
		/// 	–азрешает обработку веб-запросов Ќ““– дл€ пользовательского элемента HttpHandler, который реализует интерфейс <see
		/// 	 cref="T:System.Web.IHttpHandler" />.
		/// </summary>
		/// <param name="context"> ќбъект <see cref="T:System.Web.HttpContext" /> , предоставл€ющий ссылки на внутренние серверные объекты (например, Request, Response, Session и Server), используемые дл€ обслуживани€ HTTP-запросов. </param>
		public void ProcessRequest(HttpContext context) {
			var enstring = "application you requested now is in startup mode, please retry access after minute";
			var rustring =
				"приложение на данный момент находитс€ в режиме загрузки, попробуйте запросить его снова через минуту";
			var statestring = "application startup";
			var statecode = 200;
			Writeout(context, statestring, statecode, enstring, rustring);
		}

		/// <summary>
		/// 	¬озвращает значение, позвол€ющее определить, может ли другой запрос использовать экземпл€р класса <see
		/// 	 cref="T:System.Web.IHttpHandler" />.
		/// </summary>
		/// <returns> «начение true, если экземпл€р <see cref="T:System.Web.IHttpHandler" /> доступен дл€ повторного использовани€; в противном случае Ч значение false. </returns>
		public bool IsReusable {
			get { return false; }
		}
	}
}