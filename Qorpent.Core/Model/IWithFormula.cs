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
// PROJECT ORIGIN: Qorpent.Core/IWithFormula.cs
#endregion
namespace Qorpent.Model {
	/// <summary>
	/// Describes formula of an entity
	/// </summary>
	public interface IWithFormula {
		/// <summary>
		/// Formula's definition
		/// </summary>
		string Formula { get; set; }
		/// <summary>
		/// Formula's type
		/// </summary>
		string FormulaType { get; set; }
		/// <summary>
		/// Formula's activity flag
		/// </summary>
		bool IsFormula { get; set; }
		/// <summary>
		/// Пользовательский тип расчета
		/// </summary>
		EvalType EvalType { get; set; }
		/// <summary>
		/// Приоритет расчета
		/// </summary>
		int EvalPriority { get; set; }
	}

	
}