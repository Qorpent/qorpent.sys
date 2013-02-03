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
// Original file : IAssemblyLoader.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Reflection;

namespace Qorpent.Qlaood {
	/// <summary>
	/// 	Service interface, serves as AppDomain-wide assembly loader for not found references
	/// </summary>
	public interface IAssemblyLoader {
		/// <summary>
		/// 	Retrieves assembly as it's usual for ResolveAssembly event of AppDomain
		/// </summary>
		/// <param name="name"> full or short name of assembly to resolve </param>
		/// <param name="behavior"> loader behavior options </param>
		/// <returns> </returns>
		Assembly GetAssembly(string name, AssemblyLoaderBehavior behavior = AssemblyLoaderBehavior.Default);

		/// <summary>
		/// 	Retrieves binary data of assembly and symbol file (optional) without loading of assembly
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="behavior"> </param>
		/// <returns> </returns>
		BinaryAssemblyData GetAssemblyData(string name, AssemblyLoaderBehavior behavior = AssemblyLoaderBehavior.Default);
	}
}