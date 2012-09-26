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
// Original file : DictionaryTermSource.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;

namespace Qorpent.Dsl.LogicalExpressions {
	/// <summary>
	/// 	system provided simple dictionary based term source - all existed, non empty pairs treated as bool, other false
	/// 	(changed from Qweb where just ContainsKey was checked)
	/// </summary>
	public class DictionaryTermSource : LogicTermSource {
		/// <summary>
		/// 	creates new instance
		/// </summary>
		/// <param name="dict"> </param>
		public DictionaryTermSource(IDictionary<string, string> dict) {
			all = dict;
		}

		/// <summary>
		/// 	returns string Value of term
		/// </summary>
		/// <param name="name"> </param>
		/// <returns> </returns>
		public override string Value(string name) {
			return all.ContainsKey(name) ? all[name] : "";
		}

		/// <summary>
		/// 	return all handled terms
		/// </summary>
		/// <returns> </returns>
		public override IDictionary<string, string> GetAll() {
			return all;
		}

		/// <summary>
		/// 	returns bool equivalent of term
		/// </summary>
		/// <param name="name"> </param>
		/// <returns> </returns>
		public override bool Get(string name) {
			return all.ContainsKey(name) &&
#if !SQL2008
			       !string.IsNullOrWhiteSpace(all[name])
#else
						!string.IsNullOrEmpty(all[name])
#endif
			       && all[name] != "0" && all[name].ToUpperInvariant() != "FALSE";
			;
		}

		/// <summary>
		/// 	returns equality checking of term with Value
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="value"> </param>
		/// <returns> </returns>
		public override bool Equal(string name, string value) {
			return all.ContainsKey(name) && Equals(value, all[name]);
		}

		private readonly IDictionary<string, string> all;
	}
}