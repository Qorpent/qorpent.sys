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
// PROJECT ORIGIN: Qorpent.Core/LogicalExpressionNode.cs
#endregion

using System;
using System.Collections.Generic;
using Qorpent.LogicalExpressions;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.LogicalExpressions;

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

	    public bool IsNumber { get; set; }
	    public LETokenType Operation { get; set; }

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

	    protected bool AreMatched(string fst, string sec) {
	      
	        switch (Operation) {
	            case LETokenType.Eq:
	                goto case LETokenType.Neq;
	            case LETokenType.Neq:
	                return IsNumber ? fst.ToDecimal() == sec.ToDecimal() : fst == sec; //explicit negation
	            case LETokenType.Greater:
	                return fst.ToDecimal() > sec.ToDecimal();
	            case LETokenType.GreaterOrEq:
	                return fst.ToDecimal() >= sec.ToDecimal();
	            case LETokenType.Lower:
	                return fst.ToDecimal() < sec.ToDecimal();
	            case LETokenType.LowerOrEq:
	                return fst.ToDecimal() <= sec.ToDecimal();
	            default:
	                throw new Exception("invalid op "+Operation);
                     
	        }
	    }
	}
}