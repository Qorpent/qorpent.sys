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
namespace Qorpent.Dsl.LogicalExpressions {
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
			return source.Equal(Literal, Value);
		}
	}
}