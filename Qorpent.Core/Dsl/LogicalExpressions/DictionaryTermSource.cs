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
// PROJECT ORIGIN: Qorpent.Core/DictionaryTermSource.cs
#endregion

using System.Collections.Generic;
using System.Linq;
using Qorpent.Dsl.LogicalExpressions;
using Qorpent.Utils.Extensions;

namespace Qorpent.LogicalExpressions {
	/// <summary>
	/// 	system provided simple dictionary based term source - all existed, non empty pairs treated as bool, other false
	/// 	(changed from Qweb where just ContainsKey was checked)
	/// </summary>
	public class DictionaryTermSource<V> : LogicTermSource {
		/// <summary>
		/// 	creates new instance
		/// </summary>
		/// <param name="dict"> </param>
		public DictionaryTermSource(IDictionary<string, V> dict) {
			_all = dict;
		}
	
		/// <summary>
		/// 	returns string Value of term
		/// </summary>
		/// <param name="name"> </param>
		/// <returns> </returns>
		public override string Value(string name) {
			return _all.ContainsKey(name) ? _all[name].ToStr() : "";
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override IDictionary<string, string> GetAll(){
			if (typeof (V) == typeof(string)) return _all as IDictionary<string, string>;
			return _all.ToDictionary(_ => _.Key, _ => _.Value.ToStr());
		}


		/// <summary>
		/// 	returns bool equivalent of term
		/// </summary>
		/// <param name="name"> </param>
		/// <returns> </returns>
		public override bool Get(string name){
			var val = Value(name);
			return !string.IsNullOrWhiteSpace(val) && val != "0" && val.ToUpperInvariant() != "FALSE";
		}

	    /// <summary>
	    /// 	returns equality checking of term with Value
	    /// </summary>
	    /// <param name="name"> </param>
	    /// <param name="value"> </param>
	    /// <param name="isNumber"></param>
	    /// <returns> </returns>
	    public override bool Equal(string name, string value, bool isNumber) {
	        if (isNumber) {
                return _all.ContainsKey(name) && Equals(value.ToDecimal(), _all[name].ToDecimal(true));
	        }
			return _all.ContainsKey(name) && Equals(value, _all[name]);
		}

		private readonly IDictionary<string, V> _all;
	}
}