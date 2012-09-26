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
// Original file : XsltBasedDslProvider.cs
// Project: Qorpent.Dsl
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Dsl.SmartXslt;
using Qorpent.IO;
using Qorpent.IoC;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;

namespace Qorpent.Dsl {
	/// <summary>
	/// 	Main Qorpent DSL implementation - source files treated as BXL/XML set to be processed with SmartXslt
	/// 	with some productions of *.cs files, then C# compiler creates DLL with this files
	/// </summary>
	/// <remarks>
	/// </remarks>
	[ContainerComponent(Lifestyle = Lifestyle.Transient, ServiceType = typeof (IDslProvider))]
	public class XsltBasedDslProvider : CompilerProviderBase {
		/// <summary>
		/// 	Initializes the specified project.
		/// </summary>
		/// <param name="project"> The project. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public override DslContext Initialize(DslProject project) {
			var result = base.Initialize(project);
			var def = new XsltDslDefinition();
			def.Load(project.LangName, project.DslDirectory, FileNameResolver,
			         (from p in result.Parameters
			          where p.Value == null || p.Value is string
			          let pval = (p.Value as string) ?? ""
			          let fixedval = pval == "" ? "1" : pval
			          let isxpath = pval.StartsWith("xpath:")
			          select
				          isxpath
					          ? XsltExtensionDefinition.ParameterSelect(p.Key, fixedval.Substring(6))
					          : XsltExtensionDefinition.Parameter(p.Key, fixedval)
			         ).Union(
				         from e in project.Extensions
				         let prefix = e.Key.Split('~')[0]
				         let ns = e.Key.Split('~')[1]
				         let obj = e.Value
				         select XsltExtensionDefinition.Extension(obj, prefix, ns)
				         ));

			result.Set("xslt.dsl.def", def); //optional
			result.Set("xslt.transform", def.CreateTransform(
				                             )); //must be in context

			LoadSourceXmlData(project, result);

			return result;
		}

		private static void LoadSourceXmlData(DslProject project, DslContext result) {
			var includeloader = new XmlIncludeProcessor(result.Resolver);

			if (result.Project.JoinSources) {
				var singleresult = new XElement("root");
				foreach (var file in project.EnumerateFiles(result.Resolver)) {
					singleresult.Add(includeloader.Load(file, true, project.BxlOptions));
				}
				foreach (var directSource in project.DirectSources) {
					singleresult.Add(includeloader.Include(XmlExtensions.GetXmlFromAny(directSource.Value, project.BxlOptions),
					                                       directSource.Key));
				}
				result.LoadedXmlSources["joinedsource"] = singleresult;
			}
			else {
				foreach (var file in project.EnumerateFiles(result.Resolver)) {
					result.LoadedXmlSources[file] = includeloader.Load(file, true, project.BxlOptions);
				}
				foreach (var directSource in project.DirectSources) {
					result.LoadedXmlSources[directSource.Key] =
						includeloader.Include(XmlExtensions.GetXmlFromAny(directSource.Value, project.BxlOptions),
						                      directSource.Key);
				}
			}
		}

		/// <summary>
		/// 	Preprocesses the specified context.
		/// </summary>
		/// <param name="context"> The context. </param>
		/// <remarks>
		/// </remarks>
		public override void Preprocess(DslContext context) {
			var ext = ".cs";
			if (context.Project.ProjectType == DslProjectType.SqlScript) {
				ext = ".sql";
			}
			var smartxslt = context.Get<SmartXsltTransform>(); // must be setted in context
			var root = context.Resolver.Root.NormalizePath();
			var splitFileWriter = new FileSplitWriter();
			foreach (var xmlsource in context.LoadedXmlSources) {
				var resultitempath = xmlsource.Key.NormalizePath().Replace(root, "").Replace("/", "__") + ext;
				resultitempath = Path.Combine(context.NativeCodeDirectory, resultitempath);
				using (var wr = new StringWriter()) {
					smartxslt.Process(xmlsource.Value, wr,
					                  new Dictionary<string, object>
						                  {
							                  {"_name", Path.GetFileNameWithoutExtension(resultitempath)},
							                  {"_path", xmlsource.Key},
							                  {"_root", FileNameResolver.Root},
						                  }
						);
					var result = splitFileWriter.WriteContent(resultitempath, wr.ToString(),
					                                          context.Project.IncludeEmptyFilesInProduction,
					                                          context.Project.CustomSplitFileStartMask,
					                                          context.Project.CustomSplitFileEndMask
						);
					foreach (var wrotefile in result) {
						context.GeneratedNativeCode[wrotefile.Key] = wrotefile.Value;
					}
				}
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="fileNameResolver"> </param>
		public void SetFileNameResolver(IFileNameResolver fileNameResolver) {
			FileNameResolver = fileNameResolver;
		}
	}
}