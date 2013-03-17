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
// PROJECT ORIGIN: Qorpent.IoC/ContainerLoader.cs
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
				var result = LoadManifest(fullmanifest, allowErrors).ToArray();
				return result;
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
			PrepareServices();
			var result = new XElement("root");	
			if (null == _resolver) {
				return result;
			}
			var manifestfiles = GetManifestFileList();
			
			foreach (var manifestxml in manifestfiles.Select(LoadManifesFile)) {
				result.Add(manifestxml.Elements());
			}
			return result;
		}

		private XElement LoadManifesFile(string manifestfile) {
			XElement manifestxml;
			if (manifestfile.EndsWith(".xml")) {
				manifestxml = XElement.Load(manifestfile);
			}
			else {
				if (null == _bxl) {
					throw new Exception("cannot ProcessExtensions bxl manifests, because Bxl not configured");
				}
				manifestxml = _bxl.Parse(File.ReadAllText(manifestfile), manifestfile);
			}
			if (null != _includer) {
				_includer.Include(manifestxml, manifestfile);
			}
			return manifestxml;
		}

		private string[] GetManifestFileList() {
			var manifestfiles = _resolver.ResolveAll(
				new FileSearchQuery
					{
						ExistedOnly = true,
						PathType = FileSearchResultType.FullPath,
						ProbeFiles = new[] {"*.ioc-manifest.xml", "*.ioc-manifest.bxl"},
						ProbePaths = new[] {"~/", "~/.config", "~/bin", "~/sys", "~/usr"}
					});
			return manifestfiles;
		}

		private void PrepareServices() {
			_resolver = _container.Get<IFileNameResolver>();
			_includer = _container.Get<IXmlIncludeProcessor>();
			_bxl = _container.Get<IBxlParser>();
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
			PrepareAliases(manifest);
			foreach (var containerExtension in LoadContainerExtensions(manifest, allowErrors)) {
				yield return containerExtension;
			}
			foreach (var component in RegisterCommonComponents(manifest, allowErrors)) {
				yield return component;
			}
			//result components  for MVC are hidden by API for now //TODO: bad design
			RegisterMvcLibraries();
		}

		private void RegisterMvcLibraries() {
//now we can load mvc-friendly assemblies
			if (_mvcassemblies.Count != 0) {
				var mvcfactory = _container.Get<IMvcFactory>();
				if (null != mvcfactory) {
					foreach (var mvca in _mvcassemblies) {
						var assembly = Assembly.Load(mvca);
						if (null != assembly) {
							mvcfactory.Register(assembly);
						}
					}
				}
			}
		}

		private IEnumerable<IComponentDefinition> RegisterCommonComponents(XElement manifest, bool allowErrors) {
			var components = (
				                from componentxml in manifest.Elements()
				                where componentxml.Name.LocalName != "ref"
				                where componentxml.Name.LocalName != "using"
				                where componentxml.Name.LocalName != "containerextension"
				                where componentxml.Name.LocalName != "mvc"
				                select new ManifestComponentDefinition(componentxml, allowErrors, _dlls, _namespaces)).ToArray();
			//usual components must be load after extensions
			foreach (var component in components) {
				_container.Register(component);
				yield return component;
			}
		}

		private IEnumerable<IComponentDefinition> LoadContainerExtensions(XElement manifest, bool allowErrors) {
			return
				manifest.Elements("containerextension").Select(
					extensionxml => new ManifestComponentDefinition(extensionxml, allowErrors, _dlls, _namespaces));
		}

		private void PrepareAliases(XElement manifest) {
			_dlls = manifest.Elements("ref").Select(element => element.Attr("code")).Distinct().ToList();


			_namespaces = manifest.Elements("using").Select(element => element.Attr("code")).Distinct().ToList();

			_mvcassemblies = manifest.Elements("mvc").Select(element => element.Attr("code")).Distinct().ToList();
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
		private IFileNameResolver _resolver;
		private IXmlIncludeProcessor _includer;
		private IBxlParser _bxl;
		private List<string> _dlls;
		private List<string> _namespaces;
		private List<string> _mvcassemblies;
	}
}