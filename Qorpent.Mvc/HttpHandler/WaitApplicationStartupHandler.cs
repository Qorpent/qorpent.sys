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
// PROJECT ORIGIN: Qorpent.Mvc/WaitApplicationStartupHandler.cs
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
			const string enstring = "application you requested now is in startup mode, please retry access after minute";
			const string rustring = "приложение на данный момент находитс€ в режиме загрузки, попробуйте запросить его снова через минуту";
			const string statestring = "application startup";
			const int statecode = 200;
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