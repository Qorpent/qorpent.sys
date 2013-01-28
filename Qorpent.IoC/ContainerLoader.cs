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
// Original file : ContainerLoader.cs
// Project: Qorpent.IoC
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Qorpent.Bxl;
using Qorpent.Dsl.XmlInclude;
using Qorpent.IO;
using Qorpent.Mvc;
using Qorpent.Utils.Extensions;

namespace Qorpent.IoC {
	/// <summary>
	/// 	Applys manifest to Givent container
	/// </summary>
	public class ContainerLoader : IContainerLoader {
#if PARANOID
		static ContainerLoader() {
			if(!Qorpent.Security.Watchdog.Paranoid.Provider.OK) throw new  Qorpent.Security.Watchdog.ParanoidException(Qorpent.Security.Watchdog.ParanoidState.GeneralError);
		}
#endif

		/// <summary>
		/// 	Creates loader, that targets specified container
		/// </summary>
		/// <param name="container"> </param>
		public ContainerLoader(IContainer container) {
			_container = container;
		}

		/// <summary>
		/// </summary>
		/// <returns> </returns>
		/// <exception cref="NotImplementedException"></exception>
		public IEnumerable<IComponentDefinition> LoadDefaultManifest(bool allowErrors) {
			try {
				var fullmanifest = ReadDefaultManifest();
				return LoadManifest(fullmanifest, allowErrors);
			}
			catch (NotImplementedException) {
				return new IComponentDefinition[] {};
			}
		}

		/// <summary>
		/// 	Читает манифетсы приложения и конструирует единый
		/// </summary>
		/// <returns> </returns>
		/// <exception cref="Exception"></exception>
		public XElement ReadDefaultManifest() {
			var resolver = _container.Get<IFileNameResolver>();
			if (null == resolver) {
				return new XElement("root");
			}
			var includer = _container.Get<IXmlIncludeProcessor>();
			var manifestfiles = resolver.ResolveAll(
				new FileSearchQuery
					{
						ExistedOnly = true,
						PathType = FileSearchResultType.FullPath,
						ProbeFiles = new[] {"*.ioc-manifest.xml", "*.ioc-manifest.bxl"},
						ProbePaths = new[] {"~/", "~/.config", "~/bin", "~/sys", "~/usr"}
					});

			var fullmanifest = new XElement("root");
			foreach (var manifestfile in manifestfiles) {
				XElement manifestxml;
				if (manifestfile.EndsWith(".xml")) {
					manifestxml = XElement.Load(manifestfile);
				}
				else {
					var bxl = _container.Get<IBxlParser>();
					if (null == bxl) {
						throw new Exception("cannot ProcessExtensions bxl manifests, because Bxl not configured");
					}
					manifestxml = bxl.Parse(File.ReadAllText(manifestfile), manifestfile);
				}
				if (null != includer) {
					includer.Include(manifestxml, manifestfile);
				}
				fullmanifest.Add(manifestxml.Elements());
			}
			return fullmanifest;
		}

		/// <summary>
		/// </summary>
		/// <param name="manifest"> </param>
		/// <param name="allowErrors"> </param>
		/// <returns> </returns>
		public IEnumerable<IComponentDefinition> LoadManifest(XElement manifest, bool allowErrors) {
			if (manifest == null) {
				throw new ArgumentNullException("manifest");
			}
			IList<string> dlls = manifest.Elements("ref").Select(element => element.Attr("code")).Distinct().ToList();


			IList<string> ns = manifest.Elements("using").Select(element => element.Attr("code")).Distinct().ToList();

			IList<string> mvcs = manifest.Elements("mvc").Select(element => element.Attr("code")).Distinct().ToList();


			
			//extensions must be load first
			var result = manifest.Elements("containerextension").Select(extensionxml => new ManifestComponentDefinition(extensionxml, allowErrors, dlls, ns)).Cast<IComponentDefinition>().ToList();

			result.AddRange((
				from componentxml in manifest.Elements() 
				where componentxml.Name.LocalName != "ref" 
				where componentxml.Name.LocalName != "using" 
				where componentxml.Name.LocalName != "containerextension" 
				where componentxml.Name.LocalName != "mvc"
				select new ManifestComponentDefinition(componentxml, allowErrors, dlls, ns)));
			//usual components must be load after extensions
			foreach (var component in result) {
				_container.Register(component);
			}

			//now we can load mvc-friendly assemblies
			if(mvcs.Count!=0) {
				var _mvcfactory = _container.Get<IMvcFactory>();
				if (null != _mvcfactory) {
					foreach (var mvca in mvcs) {
						var assembly = Assembly.Load(mvca);
						if(null!=assembly) {
							_mvcfactory.Register(assembly);
						}
					}
				}
			}
			return result;
		}

		/// <summary>
		/// 	Configures assembly to container if ContainerExport attribute defined
		/// </summary>
		/// <param name="assembly"> </param>
		/// <param name="requireManifest"> </param>
		/// <returns> </returns>
		public IEnumerable<IComponentDefinition> LoadAssembly(Assembly assembly, bool requireManifest = false) {
			if (assembly == null) {
				throw new ArgumentNullException("assembly");
			}
			var am = new AssemblyManifestDefinition(assembly, needExportAttribute: false);
			var result = new List<IComponentDefinition>();
			if (null != am.Descriptor || !requireManifest) {
				foreach (var definition in am.ComponentDefinitions) {
					var component = definition.GetComponent();
					_container.Register(component);
					result.Add(component);
				}
			}
			return result.ToArray();
		}

		/// <summary>
		/// 	Loads all components defined on type
		/// </summary>
		/// <param name="type"> </param>
		/// <returns> </returns>
		public IEnumerable<IComponentDefinition> LoadType(Type type) {
			var mcd = ManifestClassDefinition.GetAllClassManifests(type).ToArray();
			IList<IComponentDefinition> components = new List<IComponentDefinition>();
			foreach (var classDefinition in mcd) {
				var component = classDefinition.GetComponent();

				_container.Register(component);
				components.Add(component);
			}

			return components.ToArray();
		}

		/// <summary>
		/// 	Loads all components defined on type
		/// </summary>
		/// <returns> </returns>
		public IEnumerable<IComponentDefinition> Load<T>() {
			return LoadType(typeof (T));
		}

		private readonly IContainer _container;
	}
}