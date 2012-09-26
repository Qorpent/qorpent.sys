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
// Original file : ResetEventResult.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;
using Qorpent.Serialization;

namespace Qorpent.Events {
	/// <summary>
	/// 	Reset event result - contains references to reseted items
	/// </summary>
	[Serialize]
	public class ResetEventResult : IEventResult {
		/// <summary>
		/// 	creates new result
		/// </summary>
		public ResetEventResult() {
			_invokeList = new List<ResetResultInfo>();
		}

		/// <summary>
		/// 	List of items, invoked by reset event
		/// </summary>
		[Serialize] public IList<ResetResultInfo> InvokeList {
			get { return _invokeList; }
		}

		private readonly IList<ResetResultInfo> _invokeList;
	}
}