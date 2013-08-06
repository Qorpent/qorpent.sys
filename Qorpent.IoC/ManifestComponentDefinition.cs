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
// PROJECT ORIGIN: Qorpent.IoC/ManifestComponentDefinition.cs
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.IoC {
	internal class ManifestComponentDefinition : ComponentDefinition {
		private static readonly string[] Skipbxlattributes = new[]
			{"code", "name", "id", "__code", "__name", "__id", "_file", "_line","tag"};

		private static readonly Regex FullyQualifiedTypeName = new Regex(@"^[^\.]+\.[^,]+\s*,\s*\S+$");

		public ManifestComponentDefinition(XElement manifestElement, bool allowerrors, IEnumerable<string> references,
		                                   IEnumerable<string> namespaces) {
			Source = manifestElement;
			try {
				Prepare(references, namespaces);
			}
			catch (Exception ex) {
				if (allowerrors) {
					Name = "ioc.error";
					Lifestyle = Lifestyle.Extension;
					Exception = ex;
				}
				else {
					throw;
				}
			}
		}

		public string ImplementationTypeName { get; set; }

		public string SericeTypeName { get; set; }

		public Exception Exception { get; set; }

		private void Prepare(IEnumerable<string> references, IEnumerable<string> namespaces) {
			ReadFromXml();
			IEnumerable<string> correctedReferences = references as string[] ?? references.ToArray();
			IEnumerable<string> correctedNamespaces = namespaces as string[] ?? namespaces.ToArray();
			if (SericeTypeName.IsNotEmpty()) {
				ServiceType = ResolveType(SericeTypeName, correctedReferences, correctedNamespaces);
			}
			if (ImplementationTypeName.IsNotEmpty()) {
				ImplementationType = ResolveType(ImplementationTypeName, correctedReferences, correctedNamespaces);
			}
		}

		private Type ResolveType(string basename, IEnumerable<string> references, IEnumerable<string> namespaces) {
			
			if (FullyQualifiedTypeName.IsMatch(basename)) {
				return Type.GetType(basename, true);
			}
			IEnumerable<string> correctedNamespaces = namespaces as string[] ?? namespaces.ToArray();
			Type resolved = null;
			foreach (var reference in references) {
				var typename = basename + ", " + reference;
				if (basename.Contains(".")) {
					resolved = Type.GetType(typename, false);
					if (null != resolved) {
						break;
					}
				}
				else {
					
					foreach (var ns in correctedNamespaces) {
						typename = ns + "." + basename + ", " + reference;
						resolved = Type.GetType(typename, false);
						if (null != resolved) {
							break;
						}
					}
				}
				if (null != resolved) {
					break;
				}
			}
			if (null == resolved) {
				throw new ContainerException("cannot resolve type " + basename);
			}
			return resolved;
		}

		private void ReadFromXml() {
			if (null != Source.Attribute("nobxl")) {
				//marks auto generated xml
				Name = Source.Attr("componentname");
				SericeTypeName = Source.Attr("servicetype").Trim();
				ImplementationTypeName = Source.Attr("implementationtype").Trim();
				Lifestyle = Source.Attr("lifestyle", "Default").To<Lifestyle>();
				foreach (var p in Source.Elements("param")) {
					Parameters[p.Attr("code")] = p.Value;
				}
			}
			else {
				//bxl mode
				if (Source.Attr("name").IsEmpty()) {
					ImplementationTypeName = Source.Attr("code").Trim();
					Name = "";
				}
				else {
					Name = Source.Attr("code");
					ImplementationTypeName = Source.Attr("name").Trim();
				}
				Tag = Source.Attr("tag");
				SericeTypeName = Source.SelfValue();
				Lifestyle = Source.Name.LocalName.To<Lifestyle>();
				foreach (var attribute in Source.Attributes()) {
					if (-1 == Array.IndexOf(Skipbxlattributes, attribute.Name.LocalName)) {
						Parameters[attribute.Name.LocalName] = attribute.Value;
					}
				}
				//collection parameters support (Q-34,Q-35)
				foreach (var element in Source.Elements())
				{
					var itemelements = element.Elements("add");
					if (itemelements.Count()!=0)
					{
						if(string.IsNullOrWhiteSpace(itemelements.First().ChooseAttr("__name","name"))){
							PrepareArrayParameter(element,itemelements);
						}else{
							PrepareDictionaryParameter(element,itemelements);
						}
					}
				}
			}
			Priority = Source.Attr("priority", "1000").ToInt();
		}

		private void PrepareDictionaryParameter(XElement element, IEnumerable<XElement> itemelements)
		{
			Parameters[element.Name.LocalName] = itemelements.Select(_ => _.Describe()).ToDictionary(_ => _.Code, _ => _.Name);
			
		}

		private void PrepareArrayParameter(XElement element, IEnumerable<XElement> itemelements)
		{
			Parameters[element.Name.LocalName] = itemelements.Select(_ => _.ChooseAttr("__code", "code")).ToArray();
		}
	}
}