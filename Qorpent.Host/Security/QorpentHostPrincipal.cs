﻿using System.Security.Principal;

namespace Qorpent.Host.Security{
	/// <summary>
	///     Внутренний принципал Qorpent.Host
	/// </summary>
	internal class QorpentHostPrincipal : IPrincipal{
		private readonly QorpentHostIdentity _identity;
		private readonly UserInfo _info;

		public QorpentHostPrincipal(UserInfo info){
			_identity = new QorpentHostIdentity(info);
			_info = info;
		}

		public UserInfo Info{
			get { return _info; }
		}

		public bool IsInRole(string role){
			if (role == "DEFAULT" || role == "GUEST") return true;
			if (_info.Type == TokenType.Admin) return true;
			return false;
		}

		public IIdentity Identity{
			get { return _identity; }
		}
	}
}