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
// Original file : LogicalExpressionEvaluator.cs
// Project: Qorpent.Dsl
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using Qorpent.IoC;

namespace Qorpent.Dsl.LogicalExpressions {
	/// <summary>
	/// 	simple realization of logical expression evaluator
	/// </summary>
	[ContainerComponent(Lifestyle.Transient)]
	public class LogicalExpressionEvaluator : ILogicalExpressionEvaluator {
		/// <summary>
		/// 	creates new instance
		/// </summary>
		public LogicalExpressionEvaluator() {
			parser = new LogicalExpressionParser();
		}


		/// <summary>
		/// 	Evaluates given expression over given logical term source
		/// </summary>
		/// <param name="expression"> </param>
		/// <param name="source"> </param>
		/// <returns> </returns>
		public bool Eval(string expression, ILogicTermSource source) {
			var parsedexpression = parser.Parse(expression);
			return parsedexpression.Eval(source);
		}

		private readonly LogicalExpressionParser parser;
	}
}