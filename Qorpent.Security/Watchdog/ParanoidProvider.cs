﻿using System;
using System.Linq;
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
			if(!suxml.Elements().Any(x=>x.Attr("user").IsNotEmpty() && x.Attr("role").Contains("/ADMIN/"))) {
				_state = ParanoidState.NoSuDefined;
			}
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
			return XElement.Parse(_xml).Elements().Any(x => x.Attr("user").ToLower() == principal.Identity.Name.ToLower());
		}

		/// <summary>
		/// Authenticate user on custom way
		/// </summary>
		/// <param name="principal"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public bool Authenticate(IPrincipal principal, string password) {
			return XElement.Parse(_xml).Elements().Any(x => x.Attr("user").ToLower() == principal.Identity.Name.ToLower()
			 && x.Attr("password") == Convert.ToBase64String( MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(password)))
			 && x.Attr("deny").ToLower()!="true"
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
			var usr = XElement.Parse(_xml).Elements().First(x => x.Attr("user").ToLower() == principal.Identity.Name.ToLower());
			return usr.Attr("role").ToUpper().Contains("/" + role.ToUpper() + "/");
		}
	}
}
