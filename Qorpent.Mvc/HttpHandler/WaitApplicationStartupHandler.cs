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
		/// 	��������� ��������� ���-�������� ���� ��� ����������������� �������� HttpHandler, ������� ��������� ��������� <see
		/// 	 cref="T:System.Web.IHttpHandler" />.
		/// </summary>
		/// <param name="context"> ������ <see cref="T:System.Web.HttpContext" /> , ��������������� ������ �� ���������� ��������� ������� (��������, Request, Response, Session � Server), ������������ ��� ������������ HTTP-��������. </param>
		public void ProcessRequest(HttpContext context) {
			const string enstring = "application you requested now is in startup mode, please retry access after minute";
			const string rustring = "���������� �� ������ ������ ��������� � ������ ��������, ���������� ��������� ��� ����� ����� ������";
			const string statestring = "application startup";
			const int statecode = 200;
			Writeout(context, statestring, statecode, enstring, rustring);
		}

		/// <summary>
		/// 	���������� ��������, ����������� ����������, ����� �� ������ ������ ������������ ��������� ������ <see
		/// 	 cref="T:System.Web.IHttpHandler" />.
		/// </summary>
		/// <returns> �������� true, ���� ��������� <see cref="T:System.Web.IHttpHandler" /> �������� ��� ���������� �������������; � ��������� ������ � �������� false. </returns>
		public bool IsReusable {
			get { return false; }
		}
	}
}