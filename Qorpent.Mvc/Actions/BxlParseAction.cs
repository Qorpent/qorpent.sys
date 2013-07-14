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
// PROJECT ORIGIN: Qorpent.Mvc/BxlParseAction.cs
#endregion
using Qorpent.Bxl;
using Qorpent.Mvc.Binding;

namespace Qorpent.Mvc.Actions {
	/// <summary>
	/// 	Parses source text as BXL and return XElemnt
	/// </summary>
	[Action("_sys.bxlparse", Help = "Converts BXL to XML")]
	public class BxlParseAction : ActionBase {
		/// <summary>
		/// 	processing of execution - main method of action
		/// </summary>
		/// <returns> </returns>
		protected override object MainProcess() {
			var opts = BxlParserOptions.None;
			if (NoLexData) {
				opts = opts | BxlParserOptions.NoLexData;
			}
			if (SafeAttributes) {
				opts = opts | BxlParserOptions.SafeAttributeNames;
			}
			return Context.Application.Bxl.Parse(Text, "bxlparse.action", opts);
		}

		/// <summary>
		/// </summary>
		[Bind] protected bool NoLexData;

		/// <summary>
		/// </summary>
		[Bind] protected bool SafeAttributes;

		/// <summary>
		/// </summary>
		[Bind(IsLargeText = true)] protected string Text;
	}
}