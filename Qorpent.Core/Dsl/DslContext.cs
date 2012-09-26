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
// Original file : DslContext.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Qorpent.IO;

namespace Qorpent.Dsl {
	/// <summary>
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class DslContext {
		/// <summary>
		/// 	Initializes a new instance of the <see cref="DslContext" /> class.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public DslContext() {
			GeneratedNativeCode = new Dictionary<string, string>();
			Messages = new List<DslMessage>();
		}

		/// <summary>
		/// 	Имена файлов ресурсов (для компилятора)
		/// </summary>
		public IList<string> ResourceList {
			get { return _resourcelist ?? (_resourcelist = new List<string>()); }
		}

		/// <summary>
		/// 	Gets or sets the project.
		/// </summary>
		/// <value> The project. </value>
		/// <remarks>
		/// </remarks>
		public DslProject Project { get; set; }

		/// <summary>
		/// 	Gets the generated S charp code.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IDictionary<string, string> GeneratedNativeCode { get; private set; }


		/// <summary>
		/// 	Gets the messages.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IList<DslMessage> Messages { get; private set; }

		/// <summary>
		/// 	Gets the max level.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public ErrorLevel MaxLevel {
			get { return Messages.Any() ? Messages.Select(x => x.ErrorLevel).Max() : ErrorLevel.None; }
		}

		/// <summary>
		/// 	Gets or sets the provider.
		/// </summary>
		/// <value> The provider. </value>
		/// <remarks>
		/// </remarks>
		public IDslProvider Provider { get; set; }

		/// <summary>
		/// </summary>
		public IDictionary<string, object> Parameters {
			get { return _parameters; }
			private set { _parameters = value; }
		}

		/// <summary>
		/// 	sources combined from files and direct sources
		/// </summary>
		public IDictionary<string, XElement> LoadedXmlSources {
			get { return _loadedXmlSources; }
		}

		/// <summary>
		/// </summary>
		public string NativeCodeDirectory { get; set; }

		/// <summary>
		/// </summary>
		public IFileNameResolver Resolver { get; set; }

		/// <summary>
		/// </summary>
		public string OutputDirectory { get; set; }

		/// <summary>
		/// </summary>
		public string OutputProductionName { get; set; }

		/// <summary>
		/// </summary>
		/// <returns> </returns>
		public Assembly LoadAssembly() {
			if (null == _assembly) {
				_assembly = Assembly.Load(File.ReadAllBytes(GetProductionFileName()));
			}
			return _assembly;
		}

		/// <summary>
		/// 	Sets the specified name.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <param name="def"> The def. </param>
		/// <remarks>
		/// </remarks>
		public void Set(string name, object def) {
			_parameters[name] = def;
		}

		/// <summary>
		/// 	Gets the specified name.
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <param name="name"> The name. </param>
		/// <param name="def"> The def. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public T Get<T>(string name = null, T def = default(T)) {
			if (string.IsNullOrEmpty(name)) {
				return _parameters.Values.OfType<T>().FirstOrDefault();
			}
			if (!_parameters.ContainsKey(name)) {
				return def;
			}
			return (T) _parameters[name];
		}

		/// <summary>
		/// </summary>
		/// <returns> </returns>
		public string GetProductionFileName(string extension = null) {
			if (string.IsNullOrEmpty(extension)) {
				extension = GetResultExtension();
			}
			return Path.Combine(OutputDirectory, OutputProductionName + extension);
		}

		/// <summary>
		/// 	retrieves default file extension for compile result
		/// </summary>
		/// <returns> </returns>
		public string GetResultExtension() {
			switch (Project.ProjectType) {
				case DslProjectType.SqlScript:
					return ".sql";
				default:
					return ".dll";
			}
		}

		/// <summary>
		/// 	reads string content of result
		/// </summary>
		/// <returns> </returns>
		public string GetResult() {
			return File.ReadAllText(GetProductionFileName());
		}

		private readonly IDictionary<string, XElement> _loadedXmlSources = new Dictionary<string, XElement>();
		private Assembly _assembly;

		/// <summary>
		/// </summary>
		private IDictionary<string, object> _parameters = new Dictionary<string, object>();

		private IList<string> _resourcelist;
	}
}