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
// Original file : GetResourceUrlAction.cs
// Project: Qorpent.Mvc
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.IO;
using Qorpent.Mvc.Binding;
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc.Actions {
	/// <summary>
	/// 	Calculates local url of resource - to create HTML reference
	/// </summary>
	[Action("_sys.getresourceurl")]
	public class GetResourceUrlAction : ActionBase {
		/// <summary>
		/// 	processing of execution - main method of action
		/// </summary>
		/// <returns> </returns>
		protected override object MainProcess() {
			name = name.Replace("~", ""); //remove QJS path extras
			var probes = getProbes(name, type);
			var url = Context.Application.Files.Resolve(probes.ToArray(), true, pathtype: FileSearchResultType.LocalUrl);
			return url;
		}

		private IEnumerable<string> getProbes(string name, string type) {
			if (type == "js") {
				return getProbes(name, new[] {"js"}, true);
			}
			if (type == "css") {
				return getProbes(name, new[] {"css"}, true);
			}
			if (type == "img") {
				return getProbes(name, new[] {"png", "gif", "jpg", "jpeg"}, true);
			}
			throw new Exception("unknouwn resource type " + type);
		}

		private IEnumerable<string> getProbes(string name, string[] extensions, bool useculture) {
			var culture = "";
			if (useculture) {
				culture = Context.Language;

				if (string.IsNullOrWhiteSpace(culture)) {
					useculture = false;
				}
				else {
					culture = culture.Split(',')[0].Trim();
				}
			}
			foreach (var e in extensions) {
				string probe;
				if (useculture) {
					probe = name + "-" + culture + "." + e;
					yield return probe;
					probe = name + "-" + culture.Split('-')[0] + "." + e;
					yield return probe;
				}
				probe = name + "." + e;
				yield return probe;
			}
		}

		/// <summary>
		/// </summary>
		[Bind(Required = true)] protected string name;

		/// <summary>
		/// </summary>
		[Bind("js", Constraint = new object[] {"js", "css", "img"})] protected string type;
	}
}