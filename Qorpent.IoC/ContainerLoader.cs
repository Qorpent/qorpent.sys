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
using Qorpent.Utils.Extensions;

namespace Qorpent.IoC {
	/// <summary>
	/// 	Applys manifest to Givent container
	/// </summary>
	public class ContainerLoader : IContainerLoader
	{

#if PARANOID
		static ContainerLoader() {
			if(!Qorpent.Security.Paranoid.Paranoid.Provider.OK) throw new  Qorpent.Security.Paranoid.ParanoidException(Qorpent.Security.Paranoid.ParanoidState.GeneralError);
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
		/// Читает манифетсы приложения и конструирует единый 
		/// </summary>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public XElement ReadDefaultManifest() {
			var _resolver = _container.Get<IFileNameResolver>();
			if (null == _resolver) {
				return new XElement("root");
			}
			var includer = _container.Get<IXmlIncludeProcessor>();
			var manifestfiles = _resolver.ResolveAll(
				new FileSearchQuery
					{
						ExistedOnly = true,
						PathType = FileSearchResultType.FullPath,
						ProbeFiles = new[] {"*.ioc-manifest.xml", "*.ioc-manifest.bxl"},
						ProbePaths = new[] {"~/", "~/.config", "~/bin", "~/sys", "~/usr" }
					});

			var fullmanifest = new XElement("root");
			foreach (var manifestfile in manifestfiles) {
				XElement manifestxml = null;
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
			var result = new List<IComponentDefinition>();
			IList<string> dlls = new List<string>();
			IList<string> ns = new List<string>();


			foreach (var element in manifest.Elements("ref")) {
				dlls.Add(element.Attr("code"));
			}
			foreach (var element in manifest.Elements("using")) {
				ns.Add(element.Attr("code"));
			}

			dlls = dlls.Distinct().ToList();
			ns = ns.Distinct().ToList();
			//extensions must be load first
			foreach (var extensionxml in manifest.Elements("containerextension")) {
				var component = new ManifestComponentDefinition(extensionxml, allowErrors, dlls, ns);
				result.Add(component);
			}
			//usual components must be load after extensions
			foreach (var componentxml in manifest.Elements()) {
				if (componentxml.Name.LocalName == "ref") {
					continue;
				}
				if (componentxml.Name.LocalName == "using") {
					continue;
				}
				if (componentxml.Name.LocalName == "containerextension") {
					continue;
				}
				var component = new ManifestComponentDefinition(componentxml, allowErrors, dlls, ns);
				result.Add(component);
			}
			foreach (var component in result) {
				_container.Register(component);
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
		/// Loads all components defined on type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
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
		/// Loads all components defined on type
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IComponentDefinition> Load<T>() {
			return LoadType(typeof (T));
		}

		private readonly IContainer _container;
	}
}