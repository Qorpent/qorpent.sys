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
// PROJECT ORIGIN: Qorpent.Dsl/EqualValueNode.cs
#endregion

using System.Text.RegularExpressions;
using Qorpent.Dsl.LogicalExpressions;
using Qorpent.LogicalExpressions;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.LogicalExpressions {
	/// <summary>
	/// 	evaluates == operator with VALUE at right side
	/// </summary>
	public class EqualValueNode : LogicalExpressionNode {
		/// <summary>
		/// 	term to check
		/// </summary>
		public string Literal { get; set; }

		/// <summary>
		/// 	valute to compare
		/// </summary>
		public string Value { get; set; }

	    /// <summary>
		/// </summary>
		/// <param name="source"> </param>
		/// <returns> </returns>
		protected override bool InternalEval(ILogicTermSource source) {
	        return AreMatched(source.Value(Literal), Value);
	    }
	}

    public class RegexTestNode : LogicalExpressionNode {
        public string First { get; set; }
        public bool FirstIsLiteral { get; set; }
        public string Second { get; set; }
        public bool SecondIsLiteral { get; set; }
        protected override bool InternalEval(ILogicTermSource source) {
            var text = FirstIsLiteral ? source.Value(First) : First;
            var regex = SecondIsLiteral ? source.Value(Second) : Second;
            return Regex.IsMatch(text, regex);

        }
    }
}