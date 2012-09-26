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
// Original file : CSharpDslCompiler.cs
// Project: Qorpent.Dsl
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using Microsoft.CSharp;
using Qorpent.Utils.Extensions;

namespace Qorpent.Dsl {
	/// <summary>
	/// 	compile given context as CSharp library
	/// </summary>
	public class CSharpDslCompiler {
		/// <summary>
		/// 	compiles DLL from source
		/// </summary>
		/// <param name="context"> </param>
		public void Compile(DslContext context) {
			var codeprovider = new CSharpCodeProvider();
			var parameters = new CompilerParameters
				{
					IncludeDebugInformation = true,
					GenerateInMemory = false,
					TreatWarningsAsErrors = false,
					OutputAssembly = context.GetProductionFileName(),
					CompilerOptions = "/define:DSLCOMPILATION;" +
					                  (
						                  from p in context.Parameters
						                  where "".Equals(p.Value) || null == p.Value
						                  select p.Key
					                  ).ConcatString(";")
				};
			parameters.ReferencedAssemblies.Add("mscorlib.dll");
			parameters.ReferencedAssemblies.Add("System.dll");
			parameters.ReferencedAssemblies.Add("System.Core.dll");
			parameters.ReferencedAssemblies.Add("System.Xml.dll");
			parameters.ReferencedAssemblies.Add("System.Xml.Linq.dll");
			//parameters.ReferencedAssemblies.Add("Qorpent.Core.dll");

			// codeprovider.


			if (context.Project.References.ToBool()) {
				foreach (var reference in context.Project.References) {
					parameters.ReferencedAssemblies.Add(reference);
				}
			}
			else {
				foreach (var dllfile in Directory.GetFiles(context.Project.ReferenceDirectory, "*.dll")) {
					var name = Path.GetFileName(dllfile);
					if (!parameters.ReferencedAssemblies.Contains(name)) {
						parameters.ReferencedAssemblies.Add(dllfile);
					}
				}
			}

			foreach (var resource in context.ResourceList) {
				parameters.EmbeddedResources.Add(resource);
			}
			var result = codeprovider.CompileAssemblyFromFile(parameters, context.GeneratedNativeCode.Keys.ToArray());
			if (result.Errors.Count > 0) {
				foreach (CompilerError error in result.Errors) {
					var m = new DslMessage
						{
							ErrorCode = error.ErrorNumber,
							LexInfo = new LexInfo(error.FileName, error.Line, error.Column),
							Message = error.ErrorText
						};
					m.ErrorLevel = ErrorLevel.Error;
					if (error.IsWarning) {
						m.ErrorLevel = ErrorLevel.Warning;
					}
					context.Messages.Add(m);
				}
			}
		}
	}
}