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
// Original file : ILogicTermSource.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;

namespace Qorpent.Dsl.LogicalExpressions {
	/// <summary>
	/// 	describes abstract source of logical/valued terms - used to describe source data for logical expression parser
	/// </summary>
	public interface ILogicTermSource {
		/// <summary>
		/// 	returns bool equivalent of term
		/// </summary>
		/// <param name="name"> </param>
		/// <returns> </returns>
		bool Get(string name);

		/// <summary>
		/// 	returns equality checking of term with Value
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="value"> </param>
		/// <returns> </returns>
		bool Equal(string name, string value);

		/// <summary>
		/// 	returns string Value of term
		/// </summary>
		/// <param name="name"> </param>
		/// <returns> </returns>
		string Value(string name);

		/// <summary>
		/// 	return all handled terms
		/// </summary>
		/// <returns> </returns>
		IDictionary<string, string> GetAll();
	}
}