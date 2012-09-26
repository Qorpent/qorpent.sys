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
// Original file : BxlParseAction.cs
// Project: Qorpent.Mvc
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using Qorpent.Bxl;
using Qorpent.Mvc.Binding;

namespace Qorpent.Mvc.Actions {
	/// <summary>
	/// 	Parses source text as BXL and return XElemnt
	/// </summary>
	[Action("_sys.bxlparse", Help = "Converts BXL to XML", Role = "DEFAULT")]
	public class BxlParseAction : ActionBase {
		/// <summary>
		/// 	processing of execution - main method of action
		/// </summary>
		/// <returns> </returns>
		protected override object MainProcess() {
			var opts = BxlParserOptions.None;
			if (nolexdata) {
				opts = opts | BxlParserOptions.NoLexData;
			}
			if (safeattributes) {
				opts = opts | BxlParserOptions.SafeAttributeNames;
			}
			return Context.Application.Bxl.Parse(text, "bxlparse.action", opts);
		}

		/// <summary>
		/// </summary>
		[Bind] protected bool nolexdata;

		/// <summary>
		/// </summary>
		[Bind] protected bool safeattributes;

		/// <summary>
		/// </summary>
		[Bind(IsLargeText = true)] protected string text;
	}
}