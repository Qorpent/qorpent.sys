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
// PROJECT ORIGIN: Qorpent.Security/ParanoidProvider.cs
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Security.Watchdog
{
	/// <summary>
	/// 
	/// </summary>
	public class ParanoidProvider:IParanoidProvider
	{
		private readonly ParanoidState _state;
		private readonly string _xml;

		/// <summary>
		/// 
		/// </summary>
		public ParanoidProvider(XElement suxml) {
			_state = ParanoidState.Verified;
			_xml = suxml.ToString(); //prevent changing
			if(!suxml.Elements("user").Any(x=>x.Attr("name").IsNotEmpty() && x.Attr("role").Contains("/ADMIN/"))) {
				_state = ParanoidState.NoSuDefined;
			}
		}

		IList<string> _logins = new List<string>();
		/// <summary>
		/// 
		/// </summary>
		/// <param name="login"></param>
		/// <param name="coockievalue"></param>
		public void RegisterLogin(string login, string coockievalue) {
			if(Assembly.GetCallingAssembly().GetName().Name!="Qorpent.Mvc") {
				throw new ParanoidException(ParanoidState.NonMvcCall);
			}
			_logins.Add(login+"/"+coockievalue);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="principal"></param>
		/// <param name="cookie"></param>
		/// <exception cref="ParanoidException"></exception>
		public void CheckLogin(IPrincipal principal, string cookie) {
			var str = principal.Identity.Name + "/" + cookie;
			if(!_logins.Contains(str)) {
				throw new ParanoidException(ParanoidState.NonMatchedCookie);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="login"></param>
		/// <param name="coockievalue"></param>
		public void RemoveLogin(string login, string coockievalue) {

			_logins.Remove(login+"/"+coockievalue);
		}

		/// <summary>
		/// True if role is under Paranoid control
		/// </summary>
		/// <param name="role"></param>
		/// <returns></returns>
		public bool IsSecureRole (string role) {
			if("ADMIN"==role) return true;
			return XElement.Parse(_xml).Elements("role").Any(x => x.Attr("name").ToUpper() == role.ToUpper());
		}

		/// <summary>
		/// Indicates that all is well
		/// </summary>
		public bool OK { get { return State == ParanoidState.Verified; } }

		/// <summary>
		/// State of environment
		/// </summary>
		public ParanoidState State { get { return _state; }}

		/// <summary>
		/// Deteremine if User is under special control
		/// </summary>
		/// <param name="principal"></param>
		/// <returns></returns>
		public bool IsSpecialUser(IPrincipal principal) {
			return XElement.Parse(_xml).Elements("user").Any(x => x.Attr("name").ToLower() == principal.Identity.Name.ToLower());
		}

		/// <summary>
		/// Authenticate user on custom way
		/// </summary>
		/// <param name="principal"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public bool Authenticate(IPrincipal principal, string password) {
			return XElement.Parse(_xml).Elements("user").Any(x => x.Attr("name").ToLower() == principal.Identity.Name.ToLower()
			 && x.Attr("password") == Convert.ToBase64String( MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(password)))
			 && x.Attr("deny").ToLower()!="true"
			);
		}

		/// <summary>
		/// Authenticate user on custom way
		/// </summary>
		/// <param name="principal"></param>
		/// <returns></returns>
		public bool Authenticate(IPrincipal principal)
		{
			return XElement.Parse(_xml).Elements("user").Any(x => x.Attr("name").ToLower() == principal.Identity.Name.ToLower()
			 && x.Attr("deny").ToLower() != "true"
			);
		}

		/// <summary>
		/// Determine user role on custom way
		/// </summary>
		/// <param name="principal"></param>
		/// <param name="role"></param>
		/// <returns></returns>
		public bool IsInRole(IPrincipal principal, string role) {
			if(!IsSpecialUser(principal)) return false;
			var usr = XElement.Parse(_xml).Elements("user").First(x => x.Attr("name").ToLower() == principal.Identity.Name.ToLower());
			return usr.Attr("role").ToUpper().Contains("/" + role.ToUpper() + "/");
		}
	}
}
