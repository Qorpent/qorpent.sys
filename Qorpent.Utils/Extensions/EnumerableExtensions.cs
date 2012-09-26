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
// Original file : EnumerableExtensions.cs
// Project: Qorpent.Utils
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections;
using System.Linq;

namespace Qorpent.Utils.Extensions {
	/// <summary>
	/// 	Some methods for Enumerables
	/// </summary>
	public static class EnumerableExtensions {
		/// <summary>
		/// 	test if enumerable is null or no elements or all elements are nulls
		/// </summary>
		/// <param name="e"> </param>
		/// <returns> </returns>
		public static bool IsEmpty(this IEnumerable e) {
			return null == e || !e.OfType<object>().Any();
		}

		/// <summary>
		/// 	test if enumerable is not null and contains not null elements
		/// </summary>
		/// <param name="e"> </param>
		/// <returns> </returns>
		public static bool IsNotEmpty(this IEnumerable e) {
			return !IsEmpty(e);
		}
	}
}