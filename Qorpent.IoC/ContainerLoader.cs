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
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Qorpent.BSharp.Runtime;
using Qorpent.Bxl;
using Qorpent.IoC.BSharp;
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
			return manifestxml;
		}

		private string[] GetManifestFileList() {
			IList<string> _result = new List<string>();
			if (null != _resolver){
				foreach (var f in _resolver.ResolveAll(
				new FileSearchQuery
				{
					ExistedOnly = true,
					PathType = FileSearchResultType.FullPath,
					ProbeFiles = new[] { "*.ioc-manifest.xml", "*.ioc-manifest.bxl" },
					ProbePaths = new[] { "~/", "~/.config", "~/bin", "~/sys", "~/usr" }
				}))
				{
					_result.Add(f);
				}
			}
			if (!string.IsNullOrWhiteSpace(EnvironmentInfo.ManifestPath) && File.Exists(EnvironmentInfo.ManifestPath)) {
				_result.Add(EnvironmentInfo.ManifestPath);
			} else if (Directory.Exists(EnvironmentInfo.ConfigDirectory)){
				foreach (var f in Directory.GetFiles(EnvironmentInfo.ConfigDirectory, "*.ioc-manifest.*"))
				{
					_result.Add(f);
				}
			}

			return _result.Select(_=>_.NormalizePath()).Distinct().ToArray();
		}

		private void PrepareServices() {
			_resolver = _container.Get<IFileNameResolver>();
			_bxl = _container.Get<IBxlParser>();
		}

		/// <summary>
		/// </summary>
		/// <param name="manifest"> </param>
		/// <param name="allowErrors"> </param>
		/// <returns> </returns>
		public IEnumerable<IComponentDefinition> LoadManifest(XElement manifest, bool allowErrors) {
			var result = new List<IComponentDefinition>();
			//throw new Exception("im here");
			if (manifest == null)
			{
				throw new ArgumentNullException("manifest");
			}
			ResolveDefines(manifest);
			PrepareAliases(manifest);
			foreach (var containerExtension in LoadContainerExtensions(manifest, allowErrors))
			{
				result.Add(containerExtension);
			}
			foreach (var component in RegisterCommonComponents(manifest, allowErrors))
			{
				result.Add(component);
			}

			RegisterMvcLibraries();
			RegisterSubResolvers();

			if (null != _container.FindComponent(typeof (IBSharpRuntimeService),null)) {
				_container.RegisterSubResolver(new BSharpTypeResolver(_container));
			}

			return result.ToArray();

		}

		private void RegisterSubResolvers() {
			var subcontainers =_container.All<ITypeResolver>();
			foreach (var c in subcontainers) {
				//force valid order
				c.Idx = _container.Idx + c.Idx;
				_container.RegisterSubResolver(c);
			}
		}

		/// <summary>
		/// Регекс глобальных констант
		/// </summary>
		public const string DEFINE_REGEX = @"\{?@([\p{Lu}_\d]+)\}?";
		private void ResolveDefines(XElement manifest) {
			
			IDictionary<string,string> defines  = new Dictionary<string, string>();
			foreach (var e in manifest.Elements("define").OrderBy(_=>_.Attr("idx").ToInt()).ToArray()) {
				var desc = e.Describe();
				var code = desc.Code;
				var value = desc.Name;
				if (string.IsNullOrWhiteSpace(value)) {
					value = desc.Value;
				}
				defines[code] = value;
				e.Remove();
			}
			if (0 == defines.Count) return;
			var r = new Regex(DEFINE_REGEX, RegexOptions.Compiled);
			foreach (var define in defines.ToArray()) {
				if (define.Value.Contains("@")) {
					var current = ReplaceDefines(define.Value, r, defines);
					defines[define.Key] = current;
				}
			}
			
			foreach (var descendant in manifest.Descendants().ToArray())
			{
				foreach (var attr in descendant.Attributes())
				{
					if (attr.Value.Contains("@")) {
						attr.Value = ReplaceDefines(attr.Value, r, defines);
					}
				}
			}
			foreach (var text in manifest.DescendantNodes().OfType<XText>().ToArray())
			{
				if (text.Value.Contains("@")) {
					text.Value = ReplaceDefines(text.Value, r, defines);
				}
			}
		}

		private static string ReplaceDefines(string currentValue, Regex r, IDictionary<string, string> defines) {
			currentValue = r.Replace(currentValue, m =>
				{
					var code = m.Groups[1].Value;
					if (defines.ContainsKey(code)) {
						return defines[code];
					}
					else {
						return m.Value;
					}
				});
			return currentValue;
		}

		private void RegisterMvcLibraries() {
//now we can load mvc-friendly assemblies
			if (_mvcassemblies.Count != 0) {
				var mvcfactory = _container.Get<IMvcFactory>();
				if (null != mvcfactory) {
					foreach (var mvca in _mvcassemblies)
					{
						var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(_ => _.GetName().Name == mvca);
						if(null==assembly) assembly = AppDomain.CurrentDomain.Load(mvca);
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
		/// <param name="context"></param>
		/// <returns> </returns>
		public IEnumerable<IComponentDefinition> LoadAssembly(Assembly assembly, bool requireManifest = false, object context=null, bool autoonly=false) {
			if (assembly == null) {
				throw new ArgumentNullException("assembly");
			}
		
		    var am = new AssemblyManifestDefinition(assembly, needExportAttribute: false);
			var result = new List<IComponentDefinition>();
			if (null != am.Descriptor || !requireManifest){
				foreach (var definition in am.ComponentDefinitions){
				    foreach (var componentDefinition in definition.GetComponents()) {
                        _container.Register(componentDefinition);
                        result.Add(componentDefinition);
                    }
					
				}
			}
            //if assembly setupers exists - they will be called instead of auto-configuration
            var initializers =
                assembly.GetTypes().Where(_ => null != _.GetCustomAttribute<ContainerInitializerAttribute>()).ToArray();
            if (0 != initializers.Length)
            {
                result = result.Union( initializers.Select(_ => _.Create<IContainerInitializer>())
                    .SelectMany(_ => _.Setup(_container, context))).ToList();

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
			    foreach (var component in classDefinition.GetComponents()) {
                    _container.Register(component);
                    components.Add(component);
                }
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
		private IBxlParser _bxl;
		private List<string> _dlls;
		private List<string> _namespaces;
		private List<string> _mvcassemblies;
	}
}