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
// PROJECT ORIGIN: Qorpent.Dsl/EqualNode.cs
#endregion

using Qorpent.Dsl.LogicalExpressions;
using Qorpent.LogicalExpressions;

namespace Qorpent.Utils.LogicalExpressions {
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
		    var resolvefst = FirstLiteral.StartsWith("$") ?  FirstLiteral.Substring(1) : FirstLiteral;
		    var resolvesec = SecondLiteral.StartsWith("$") ? SecondLiteral.Substring(1) : SecondLiteral;
		    var fst = source.Value(resolvefst);
            var sec = source.Value(resolvesec);
		    return AreMatched(fst, sec);
		}
	}
}