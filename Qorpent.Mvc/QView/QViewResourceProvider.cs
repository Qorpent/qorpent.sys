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
// Original file : QViewResourceProvider.cs
// Project: Qorpent.Mvc
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.IO;
using Qorpent.Dsl;
using Qorpent.IoC;

namespace Qorpent.Mvc.QView {
	/// <summary>
	/// 	ќбеспечивает переименование и предоставление в QView DLL файлов resb, css, js
	/// </summary>
	[ContainerComponent(Lifestyle.Transient, "qview.dsl.resource.provider")]
	public class QViewResourceProvider : IDslResourceProvider {
		/// <summary>
		/// 	—оздает реестр ресурсов контекста
		/// </summary>
		/// <param name="context"> </param>
		public void PrepareResources(DslContext context) {
			foreach (var srcfile in context.LoadedXmlSources.Keys) {
				if (!Path.IsPathRooted(srcfile)) {
					continue; //признак динамического файла
				}
				var viewname = QViewCompilerHelper.GetClsName(srcfile, context.Project.RootDirectory);
				var level = QViewCompilerHelper.GetLevel(srcfile, context.Project.RootDirectory);
				var outdir = context.Project.NativeCodeDirectory;
				var localdir = Path.GetDirectoryName(srcfile);
				var localname = Path.GetFileNameWithoutExtension(srcfile);
				var localresources = Directory.GetFiles(localdir, localname + ".*");
				foreach (var localresource in localresources) {
					var ext = Path.GetExtension(localresource);
					if (".vbxl" == ext) {
						continue; //это собственно файлы вьюхи
					}
					var outfilename = Path.Combine(outdir, viewname + "_" + level + "_View" + Path.GetExtension(localresource));

					if (".cs" == ext) {
						outfilename = outfilename.Replace(".cs", ".partial.cs");
						File.Copy(localresource, outfilename);
						//CSharp код мы копируем дл€ компил€ции, а не дл€ внедрени€, соответственно их надо прописать в исходный код
						context.GeneratedNativeCode[outfilename] = File.ReadAllText(outfilename);
					}
					else {
						File.Copy(localresource, outfilename);
						//»наче это ресурс дл€ внедрени€
						context.ResourceList.Add(outfilename);
					}
				}
			}
		}
	}
}