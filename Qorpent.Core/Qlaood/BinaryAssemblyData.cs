#region LICENSE

// Copyright 2007-2013 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
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
// Original file : BinaryAssemblyData.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;

namespace Qorpent.Qlaood {
	/// <summary>
	/// 	Helper structure to hold assembly data as binary - helpfull for working with assemblies whithout loading in current domain
	/// </summary>
	[Serializable]
	public struct BinaryAssemblyData {
		/// <summary>
		/// 	Assembly name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 	Assembly data
		/// </summary>
		public byte[] Assembly { get; set; }

		/// <summary>
		/// 	Content of pdb/mdb file
		/// </summary>
		public byte[] SymbolInfo { get; set; }
	}
}