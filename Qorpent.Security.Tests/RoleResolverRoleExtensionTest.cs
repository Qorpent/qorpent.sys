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
// Original file : RoleResolverRoleExtensionTest.cs
// Project: Qorpent.Security.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Linq;
using System.Security.Principal;
using NUnit.Framework;
using Qorpent.Dsl.LogicalExpressions;
using Qorpent.Security;
using Qorpent.Utils.LogicalExpressions;

namespace Qorpent.Core.Tests.Security {
	[TestFixture]
	public class RoleResolverRoleExtensionTest {
		[TestCase("TEMP & exact___ZET3", new[] {"ADMIN", "ZET3"}, true)]
		[TestCase("TEMP & exact___ZET2", new[] {"ADMIN"}, false)]
		[TestCase("TEMP & ZET2", new[] {"ADMIN"}, true)]
		[TestCase("TEMP & ZET2", new[] {"ZET2", "TEMP"}, true)]
		[TestCase("TEMP & ZET2", new[] {"ZET2"}, false)]
		[TestCase("TEMP | ZET2", new[] {"ZET2"}, true)]
		[TestCase("TEMP,ZET2", new[] {"ZET3"}, false)]
		[TestCase("TEMP,ZET2", new[] {"ZET2"}, true)]
		[TestCase("TEMP", new[] {"TEMP"}, true)]
		[TestCase("ADMIN", new[] {""}, false)]
		[TestCase("TEMP", new[] {"ADMIN"}, true)]
		[TestCase("exact___TEMP", new[] {"ADMIN"}, false)]
		[TestCase("exact___TEMP", new[] {"TEMP"}, true)]
		public void CanTestRoleTest(string testrole, string[] roles, bool result) {
			roles = roles.Select(x => "test\\test_" + x).ToArray();
			var roleresolver = new StubRoleResolver(roles);
			var principal = new GenericPrincipal(new GenericIdentity("test\\test"), null);
			var obj = new StubIWithRole {Role = testrole};
			var formulaevaluator = new LogicalExpressionEvaluator();
			var extension = new RoleBasedAccessProviderForIWithRole {FormulaEvaluator = formulaevaluator};

			Assert.IsTrue(extension.IsSupported(obj));
			var execution = extension.IsAccessible(obj, AccessRole.Access, principal, roleresolver);
			Console.WriteLine(execution.ResultType);
			bool boolresult = execution;
			Console.WriteLine(boolresult);
			Assert.AreEqual(result, (bool) execution);
		}
	}
}