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
// Original file : AssemblyManifestDefinition.cs
// Project: Qorpent.IoC
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Qorpent.Utils.Extensions;

namespace Qorpent.IoC {
	/// <summary>
	/// 	prepares asssembly container descriptor
	/// </summary>
	public class AssemblyManifestDefinition {
		/// <summary>
		/// 	creates  assembly manifest definition
		/// </summary>
		public AssemblyManifestDefinition(Assembly assembly, bool needExportAttribute) {
			if (assembly == null) {
				throw new ArgumentNullException("assembly");
			}
			// we use indirect attribute usage to avoid msbuild context problems - we have to make compoents even in different versions of system
			ComponentDefinitions = new List<ManifestClassDefinition>();

			AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

			try {
				var predesc =
					assembly.GetCustomAttributes(true).FirstOrDefault(
						x => x.GetType().Name == typeof (ContainerExportAttribute).Name ||
						     x.GetType().BaseType.Name == typeof (ContainerExportAttribute).Name);
				if (null != predesc) {
					Descriptor = new ContainerExportAttribute
						{
							Priority = predesc.GetValue<int>("Priority"),
							Lifestyle = predesc.GetValue<Lifestyle>("Lifestyle")
						};
				}
			}
			finally {
				AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
			}
			if (null == Descriptor && !needExportAttribute) {
				Descriptor = new ContainerExportAttribute(-1, Lifestyle.Transient);
			}
			if (null != Descriptor) {
				foreach (var type in assembly.GetTypes()) {
					if (type.IsAbstract) {
						continue; //cannot expose abstracts
					}
					var clsdef = new ManifestClassDefinition(type);
					if (null != clsdef.Descriptor) {
						ComponentDefinitions.Add(clsdef);
						clsdef.AssemblyManifest = this;
					}
				}
			}
		}

		/// <summary>
		/// 	Attribute with assembly defaults
		/// </summary>
		public ContainerExportAttribute Descriptor { get; set; }

		/// <summary>
		/// 	Exported classes in assembly
		/// </summary>
		public IList<ManifestClassDefinition> ComponentDefinitions { get; private set; }

		private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args) {
			var dir = Path.GetDirectoryName(args.RequestingAssembly.CodeBase.Replace("file:///", ""));
			var filename = Path.Combine(dir, args.Name.Split(',')[0] + ".dll");
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(filename);
			if (File.Exists(filename)) {
				return Assembly.LoadFile(filename);
			}
			return null;
		}
	}
}