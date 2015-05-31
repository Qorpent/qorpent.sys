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
// Original file : StubRoleResolver.cs
// Project: Qorpent.Core.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using qorpent.v2.security.authorization;
using Qorpent.Mvc;
using Qorpent.Security;

namespace Qorpent.Core.Tests.Security {
	public class StubRoleResolver : IRoleResolverService {
		public StubRoleResolver(IEnumerable<string> roles) {
			rolemaps = new List<string>(roles.Select(x => x.Replace("\\", "/").ToUpperInvariant()));
		}

		public StubRoleResolver(params string[] roles) {
			rolemaps = new List<string>(roles.Select(x => x.Replace("\\", "/").ToUpperInvariant()));
		}


		public bool IsInRole(IIdentity identity, string role, bool exact = false) {
			var test_string = (identity.Name + "_" + role).Replace("\\", "/").ToUpperInvariant();
			var result = rolemaps.Contains(test_string);
			if (!result && !exact) {
				test_string = (identity.Name + "_ADMIN").Replace("\\", "/").ToUpperInvariant();
				return rolemaps.Contains(test_string);
			}
			return result;
		}

	    public void Clear() {
	        throw new System.NotImplementedException();
	    }

	    public bool IsInRole(string username, string role, bool exact = false, IMvcContext callcontext = null,
		                     object customcontext = null) {
			var principal = new GenericPrincipal(new GenericIdentity(username), null);
			return IsInRole(principal.Identity, role, exact);
		}

	    public IEnumerable<string> GetRoles(IPrincipal principal, IMvcContext callcontext = null, object customcontext = null) {
	        throw new System.NotImplementedException();
	    }

	    private readonly List<string> rolemaps;
	    public IEnumerable<IRoleResolver> GetExtensions() {
	        throw new System.NotImplementedException();
	    }

	    public void RegisterExtension(IRoleResolver extension) {
	        throw new System.NotImplementedException();
	    }

	    public void RemoveExtension(IRoleResolver extension) {
	        throw new System.NotImplementedException();
	    }

	    public void ClearExtensions() {
	        throw new System.NotImplementedException();
	    }
	}
}