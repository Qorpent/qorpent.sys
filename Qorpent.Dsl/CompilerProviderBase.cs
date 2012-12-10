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
// Original file : CompilerProviderBase.cs
// Project: Qorpent.Dsl
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using Qorpent.Utils;

namespace Qorpent.Dsl {
	/// <summary>
	/// 	compiles DLL with CSharp
	/// </summary>
	/// <remarks>
	/// </remarks>
	public abstract class CompilerProviderBase : DslProviderBase {
		/// <summary>
		/// 	additionaly prepares xslt and compiler parameter options using complex string on project parameters
		/// </summary>
		/// <param name="project"> </param>
		/// <returns> </returns>
		public override DslContext Initialize(DslProject project) {
			var result = base.Initialize(project);
			var cmplx = ComplexStringExtension.Parse(project.CompilerOptions);
			foreach (var p in cmplx) {
				result.Parameters[p.Key] = p.Value;
			}
			return result;
		}

		/// <summary>
		/// 	Compiles the specified context.
		/// </summary>
		/// <param name="context"> The context. </param>
		/// <remarks>
		/// </remarks>
		public override void Compile(DslContext context) {
			if (context.Project.ProjectType == DslProjectType.Library || context.Project.ProjectType == DslProjectType.Default) {
				new CSharpDslCompiler().Compile(context);
			}
			else if (context.Project.ProjectType == DslProjectType.SqlScript) {
				new SqlScriptDslCompiler().Compile(context);
			}
		}
	}
}