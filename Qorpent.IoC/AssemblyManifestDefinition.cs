#region LICENSE
// Copyright 2007-2013 Qorpent Team - http://github.com/Qorpent
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// PROJECT ORIGIN: Qorpent.IoC/AssemblyManifestDefinition.cs
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
						x =>
							{
								var baseType = x.GetType().BaseType;
								return baseType != null && (x.GetType().Name == typeof (ContainerExportAttribute).Name ||
							                                 baseType.Name == typeof (ContainerExportAttribute).Name);
							});
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
				Descriptor = new ContainerExportAttribute(-1);
			}
			if (null != Descriptor) {
				foreach (var type in assembly.GetTypes()) {
					if (type.IsAbstract) {
						continue; //cannot expose abstracts
					}
				    var attributes = type.GetCustomAttributes(true).Where(x => {
				        var baseType = x.GetType().BaseType;
				        return baseType != null && (x.GetType().Name == typeof (ContainerComponentAttribute).Name
				                                    ||
				                                    baseType.Name == typeof (ContainerComponentAttribute).Name);
				    }).ToArray();
				    foreach (var attribute in attributes) {
                        var clsdef = new ManifestClassDefinition(type,new [] { attribute});
				        if (null != clsdef.Descriptors && 0 != clsdef.Descriptors.Count) {
                            ComponentDefinitions.Add(clsdef);
                            clsdef.AssemblyManifest = this;
                        }    
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
			var dir = Path.GetDirectoryName(args.RequestingAssembly.CodeBase.Replace(EnvironmentInfo.FULL_FILE_NAME_START, ""));
			if (dir != null) {
				var filename = Path.Combine(dir, args.Name.Split(',')[0] + ".dll");
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine(filename);
				if (File.Exists(filename)) {
					return Assembly.LoadFile(filename);
				}
			}
			return null;
		}
	}
}