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
// Original file : EqualNode.cs
// Project: Qorpent.Dsl
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

namespace Qorpent.Dsl.LogicalExpressions {
	/// <summary>
	/// 	node that describes logical == operator
	/// </summary>
	public class EqualNode : LogicalExpressionNode {
		/// <summary>
		/// 	first operand to check
		/// </summary>
		public string FirstLiteral { get; set; }

		/// <summary>
		/// 	second operand to check
		/// </summary>
		public string SecondLiteral { get; set; }

		/// <summary>
		/// </summary>
		/// <param name="source"> </param>
		/// <returns> </returns>
		protected override bool InternalEval(ILogicTermSource source) {
			if (FirstLiteral.StartsWith("$") && SecondLiteral.StartsWith("$")) {
				return source.Value(FirstLiteral.Substring(1)) == source.Value(SecondLiteral.Substring(1));
			}
			if (FirstLiteral.StartsWith("$") && !SecondLiteral.StartsWith("$")) {
				return source.Value(FirstLiteral.Substring(1)) == source.Value(SecondLiteral);
			}
			if (!FirstLiteral.StartsWith("$") && SecondLiteral.StartsWith("$")) {
				return source.Value(FirstLiteral) == source.Value(SecondLiteral.Substring(1));
			}
			return source.Get(FirstLiteral) == source.Get(SecondLiteral);
		}
	}
}