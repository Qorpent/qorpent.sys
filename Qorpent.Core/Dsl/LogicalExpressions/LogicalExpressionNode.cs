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
// Original file : LogicalExpressionNode.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;

namespace Qorpent.Dsl.LogicalExpressions {
	/// <summary>
	/// 	abstract node of logical expression tree
	/// </summary>
	public abstract class LogicalExpressionNode {
		/// <summary>
		/// </summary>
		protected LogicalExpressionNode() {
			Children = new List<LogicalExpressionNode>();
		}

		/// <summary>
		/// 	marks that ! operator must be applyed on this node
		/// </summary>
		public bool Negative { get; set; }

		/// <summary>
		/// 	children nodes
		/// </summary>
		public IList<LogicalExpressionNode> Children { get; private set; }

		/// <summary>
		/// 	evaluates bool value of node
		/// </summary>
		/// <param name="source"> </param>
		/// <returns> </returns>
		public bool Eval(ILogicTermSource source) {
			var result = InternalEval(source);
			if (Negative) {
				result = !result;
			}
			return result;
		}

		/// <summary>
		/// 	must override - real implementation of Bool value retrive
		/// </summary>
		/// <param name="source"> </param>
		/// <returns> </returns>
		protected abstract bool InternalEval(ILogicTermSource source);
	}
}