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
// Original file : DslProject.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Qorpent.Bxl;
using Qorpent.IO;
using Qorpent.Log;

namespace Qorpent.Dsl {
	/// <summary>
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class DslProject {
		/// <summary>
		/// 	Initializes a new instance of the <see cref="DslProject" /> class.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public DslProject() {
			DirectSources = new Dictionary<string, string>();
			ErrorLevel = ErrorLevel.Warning;
			ReferenceDirectory = EnvironmentInfo.BinDirectory;
			FileSources = new List<string>();
		}

		/// <summary>
		/// 	folder where dll reference files placed (native bin by default)
		/// </summary>
		public string ReferenceDirectory { get; set; }


		/// <summary>
		/// </summary>
		public IDictionary<string, object> Extensions {
			get { return _extensions; }
		}


		/// <summary>
		/// 	Gets or sets the root directory.
		/// </summary>
		/// <value> The root directory. </value>
		/// <remarks>
		/// </remarks>
		public string RootDirectory { get; set; }

		/// <summary>
		/// 	Gets or sets the name of the lang.
		/// </summary>
		/// <value> The name of the lang. </value>
		/// <remarks>
		/// </remarks>
		public string LangName { get; set; }

		/// <summary>
		/// 	Gets or sets the file sources.
		/// </summary>
		/// <value> The file sources. </value>
		/// <remarks>
		/// </remarks>
		public IList<string> FileSources { get; private set; }

		/// <summary>
		/// 	constructor friendly property to setup sources
		/// </summary>
		public IEnumerable<string> SetupSources {
			set {
				if (null != value) {
					foreach (var x in value) {
						if (!FileSources.Contains(x)) {
							FileSources.Add(x);
						}
					}
				}
			}
		}

		/// <summary>
		/// 	Project result type
		/// </summary>
		public DslProjectType ProjectType { get; set; }

		/// <summary>
		/// 	Gets the direct sources.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IDictionary<string, string> DirectSources { get; private set; }

		/// <summary>
		/// 	Gets or sets the references.
		/// </summary>
		/// <value> The references. </value>
		/// <remarks>
		/// </remarks>
		public string[] References { get; set; }

		/// <summary>
		/// 	Gets or sets the name of the library.
		/// </summary>
		/// <value> The name of the library. </value>
		/// <remarks>
		/// </remarks>
		public string ResultName { get; set; }

		/// <summary>
		/// 	Gets or sets the C sharp directory.
		/// </summary>
		/// <value> The C sharp directory. </value>
		/// <remarks>
		/// </remarks>
		public string NativeCodeDirectory { get; set; }

		/// <summary>
		/// 	Gets or sets the extra data.
		/// </summary>
		/// <value> The extra data. </value>
		/// <remarks>
		/// </remarks>
		public XElement ExtraData { get; set; }

		/// <summary>
		/// 	Gets or sets the name of the generated library file.
		/// </summary>
		/// <value> The name of the generated library file. </value>
		/// <remarks>
		/// </remarks>
		public string OutputDirectory { get; set; }


		/// <summary>
		/// 	Gets or sets the error level.
		/// </summary>
		/// <value> The error level. </value>
		/// <remarks>
		/// </remarks>
		public ErrorLevel ErrorLevel { get; set; }

		/// <summary>
		/// 	Gets or sets the UserLog level.
		/// </summary>
		/// <value> The UserLog level. </value>
		/// <remarks>
		/// </remarks>
		public LogLevel LogLevel { get; set; }

		/// <summary>
		/// 	Complex string with compiler parameters
		/// </summary>
		public string CompilerOptions { get; set; }

		/// <summary>
		/// 	Force DSL to concatenate source file in single source document
		/// </summary>
		public bool JoinSources { get; set; }

		/// <summary>
		/// 	True if output file directory of preprocessing must include empty preprocessed files
		/// </summary>
		public bool IncludeEmptyFilesInProduction { get; set; }

		/// <summary>
		/// 	Custom start file mask to be used in FileSplitWriter
		/// </summary>
		public string CustomSplitFileStartMask { get; set; }

		/// <summary>
		/// 	Custom end file mask to be used in FileSplitWriter
		/// </summary>
		public string CustomSplitFileEndMask { get; set; }

		/// <summary>
		/// 	Directly defined root directory for dsl definitions
		/// </summary>
		public string DslDirectory { get; set; }

		/// <summary>
		/// 	Project used only for preprocessing, not for final compilation - only first phase will be executed
		/// </summary>
		public bool PreprocessOnly { get; set; }

		/// <summary>
		/// </summary>
		public BxlParserOptions BxlOptions { get; set; }

		/// <summary>
		/// </summary>
		/// <param name="resolver"> </param>
		/// <returns> </returns>
		/// <exception cref="DslException"></exception>
		public IEnumerable<string> EnumerateFiles(IFileNameResolver resolver) {
			foreach (var fileSource in FileSources) {
				if (fileSource.Contains("*")) {
					//wildcards
					var dir = Path.GetDirectoryName(fileSource);
					var mask = Path.GetFileName(fileSource);
					dir =
						resolver.Resolve(new FileSearchQuery
							{ExistedOnly = true, PathType = FileSearchResultType.FullPath, ProbeFiles = new[] {dir}});
					if (null != dir) {
						foreach (var file in Directory.GetFiles(dir, mask)) {
							yield return file;
						}
					}
				}
				else {
					var file =
						resolver.Resolve(new FileSearchQuery
							{ExistedOnly = true, ProbeFiles = new[] {fileSource}, PathType = FileSearchResultType.FullPath});
					if (null == file) {
						throw new DslException(new DslMessage
							{ErrorLevel = ErrorLevel.Error, Message = "cannot find source file " + fileSource});
					}
					yield return file;
				}
			}
		}

		private readonly IDictionary<string, object> _extensions = new Dictionary<string, object>();
	}
}