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
// Original file : DslProviderBase.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.IO;
using Qorpent.IO;

namespace Qorpent.Dsl {
	/// <summary>
	/// 	base dsl provider - preprocessing marked as abstract, but default compile logic implemented
	/// </summary>
	/// <remarks>
	/// </remarks>
	public abstract class DslProviderBase : ServiceBase, IDslProvider {
		/// <summary>
		/// 	Gets or sets the file name resolver.
		/// </summary>
		/// <value> The file name resolver. </value>
		/// <remarks>
		/// </remarks>
		protected IFileNameResolver FileNameResolver {
			get {
				return _fileNameResolver ?? (
					                            _fileNameResolver = ResolveService<IFileNameResolver>());
			}
			set { _fileNameResolver = value; }
		}


		/// <summary>
		/// 	Initializes the specified project.
		/// </summary>
		/// <param name="project"> The project. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public virtual DslContext Initialize(DslProject project) {
			var result = new DslContext {Project = project, Provider = this};

			SetupResolver(project, result);
			SetupCompilerOutputDirectory(project, result);
			SetupCSharpOutputDirectory(result);
			return result;
		}

		/// <summary>
		/// 	Preprocesses the specified context.
		/// </summary>
		/// <param name="context"> The context. </param>
		/// <remarks>
		/// </remarks>
		public abstract void Preprocess(DslContext context);

		/// <summary>
		/// 	Compiles the specified context.
		/// </summary>
		/// <param name="context"> The context. </param>
		/// <remarks>
		/// </remarks>
		public abstract void Compile(DslContext context);

		/// <summary>
		/// 	Runs the specified project.
		/// </summary>
		/// <param name="project"> The project. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public DslContext Run(DslProject project) {
			var context = Initialize(project);
			PreprocessXml(context);
			Preprocess(context);
			PrepareResourceFiles(context);
			if (!project.PreprocessOnly) {
				Compile(context);
			}
			return context;
		}

		/// <summary>
		/// 	Обеспечивает набор внедренных ресурсов для языка
		/// </summary>
		public IDslResourceProvider ResourceProvider { get; set; }

		/// <summary>
		/// 	Опциональный класс дополнительно обработки XML
		/// </summary>
		public IDslXmlPreprocessor XmlPreprocessor { get; set; }


		private void SetupResolver(DslProject project, DslContext result) {
			var resolver = FileNameResolver;
			if (!string.IsNullOrEmpty(project.RootDirectory)) {
				var rootdir =
					resolver.Resolve(new FileSearchQuery
						{ProbeFiles = new[] {project.RootDirectory}, PathType = FileSearchResultType.FullPath});
				resolver = (IFileNameResolver) Activator.CreateInstance(resolver.GetType());
				resolver.Root = rootdir;
			}
			result.Resolver = resolver;
		}

		/// <summary>
		/// 	Setups the compiler output directory.
		/// </summary>
		/// <param name="project"> The project. </param>
		/// <param name="result"> The result. </param>
		/// <remarks>
		/// </remarks>
		private void SetupCompilerOutputDirectory(DslProject project, DslContext result) {
			result.OutputDirectory = project.OutputDirectory;
			result.OutputProductionName = project.ResultName;
			if (string.IsNullOrEmpty(result.OutputDirectory)) {
				result.OutputDirectory = Path.GetTempFileName();
				File.Delete(result.OutputDirectory);
			}
			if (string.IsNullOrEmpty(project.ResultName)) {
				result.OutputProductionName = Path.GetFileNameWithoutExtension(Path.GetTempFileName());
			}
			result.OutputDirectory =
				FileNameResolver.Resolve(new FileSearchQuery
					{ProbeFiles = new[] {result.OutputDirectory}, ExistedOnly = false, PathType = FileSearchResultType.FullPath});
			Directory.CreateDirectory(result.OutputDirectory);
		}

		/// <summary>
		/// 	Setups the C sharp output directory.
		/// </summary>
		/// <param name="result"> The result. </param>
		/// <remarks>
		/// </remarks>
		private void SetupCSharpOutputDirectory(DslContext result) {
			var outdir = result.Project.NativeCodeDirectory;
			if (string.IsNullOrEmpty(outdir)) {
				outdir = Path.GetTempFileName();
				File.Delete(outdir);
			}
			else {
				outdir =
					result.Resolver.Resolve(new FileSearchQuery
						{ProbeFiles = new[] {outdir}, ExistedOnly = false, PathType = FileSearchResultType.FullPath});
			}
			Directory.CreateDirectory(outdir);
			foreach (var file in Directory.GetFiles(outdir, "*.cs")) {
				File.Delete(file);
			}
			result.NativeCodeDirectory = outdir;
		}

		/// <summary>
		/// 	Вызывается после препроцессинга и подготавливает файловые ресурсы для библиотеки
		/// </summary>
		/// <param name="context"> </param>
		protected void PrepareResourceFiles(DslContext context) {
			if (null != ResourceProvider) {
				ResourceProvider.PrepareResources(context);
			}
		}

		/// <summary>
		/// 	Метод, который можно вызывать в DSL, имеющих дело с входными файлами BXL/XML  для доп. обработки
		/// </summary>
		/// <param name="context"> </param>
		protected void PreprocessXml(DslContext context) {
			if (null != XmlPreprocessor) {
				XmlPreprocessor.PreprocessXml(context);
			}
		}

		/// <summary>
		/// </summary>
		private IFileNameResolver _fileNameResolver;
	}
}