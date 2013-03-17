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
// PROJECT ORIGIN: Qorpent.Dsl/SmartXsltTransform.cs
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Xsl;
using Qorpent.IO;
using Qorpent.IoC;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;

namespace Qorpent.Dsl.SmartXslt {
	/// <summary>
	/// 	Xslt transform with addition preprocessing ability
	/// </summary>
	/// <remarks>
	/// </remarks>
	[ContainerComponent(Lifestyle.Transient)]
	public class SmartXsltTransform : ServiceBase, ISmartXsltTransform {
		/// <summary>
		/// 	Initializes a new instance of the <see cref="SmartXsltTransform" /> class.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public SmartXsltTransform() {
			helper = new XsltHelper();
//	xmlurlresolver = new 
		}

		/// <summary>
		/// </summary>
		[Inject] public IFileNameResolver Resolver { get; set; }

		/// <summary>
		/// </summary>
		public string Codebase {
			get { return _codebase; }
		}


		/// <summary>
		/// 	Setups the specified xsltsource.
		/// </summary>
		/// <param name="xsltsource"> The xsltsource. </param>
		/// <param name="codebase"> logical/phisical uri for url resolver </param>
		/// <param name="extensions"> The extensions. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public ISmartXsltTransform Setup(object xsltsource, string codebase,
		                                 IEnumerable<XsltExtensionDefinition> extensions = null) {
			_extensions = extensions ?? new XsltExtensionDefinition[] {};
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (XsltExtensionDefinition e in extensions) {
				if ((e.Type == XsltExtenstionType.Import || e.Type == XsltExtenstionType.Include) && (e.Value != null && e.Value is string && ((string) e.Value).Contains("<xsl:"))) {
					dictionary.Add(e.Name, (string) e.Value);
				}
			}
			_directsrc = dictionary;


			_codebase = codebase;
			Resolver = Resolver ?? ResolveService<IFileNameResolver>();
			if (null == Resolver) {
				throw new QorpentException("resolver was not found in calling context");
			}
			_xsltsource = XmlExtensions.GetXmlFromAny(XmlExtensions.GetXmlFromAny(xsltsource));
			Uri baseuri;
			baseuri = Path.IsPathRooted(codebase) ? new Uri(codebase) : new Uri(Resolver.Resolve(codebase));
			xmlresolver = new FileNameResolverXmlUrlResolver(baseuri, Resolver, _directsrc);
			_xsltsource = helper.PrepareXsltStylesheet(_xsltsource, _extensions);
			_transform = new XslCompiledTransform();
			_transform.Load(_xsltsource.CreateReader(), XsltSettings.TrustedXslt, xmlresolver);
			return this;
		}


		/// <summary>
		/// 	Processes the specified contentsource into textwrite.
		/// </summary>
		/// <param name="contentsource"> The contentsource. </param>
		/// <param name="output"> The output. </param>
		/// <param name="additionParameters"> The addition parameters. </param>
		/// <remarks>
		/// </remarks>
		public void Process(object contentsource, TextWriter output, IDictionary<string, object> additionParameters = null) {
			var args = new XsltArgumentList();
			var xml = XmlExtensions.GetXmlFromAny(contentsource);
			helper.PrepareXsltArguments(args, _extensions);
			if (null != additionParameters) {
				foreach (var additionParameter in additionParameters) {
					args.RemoveParam(additionParameter.Key, "");
					args.AddParam(additionParameter.Key, "", additionParameter.Value);
				}
			}
			_transform.Transform(xml.CreateReader(), args, output);
		}

		/// <summary>
		/// 	Processes the specified contentsource and returns as str.
		/// </summary>
		/// <param name="contentsource"> The contentsource. </param>
		/// <param name="additionParameters"> The addition parameters. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public string Process(object contentsource, IDictionary<string, object> additionParameters = null) {
			var sw = new StringWriter();
			Process(contentsource, sw, additionParameters);
			return sw.ToString();
		}

		/// <summary>
		/// </summary>
		private readonly XsltHelper helper;

		/// <summary>
		/// </summary>
		private string _codebase;

		private Dictionary<string, string> _directsrc;

		/// <summary>
		/// </summary>
		private IEnumerable<XsltExtensionDefinition> _extensions;

		/// <summary>
		/// </summary>
		private XslCompiledTransform _transform;

		/// <summary>
		/// </summary>
		private XElement _xsltsource;

		/// <summary>
		/// </summary>
		private FileNameResolverXmlUrlResolver xmlresolver;
	}
}