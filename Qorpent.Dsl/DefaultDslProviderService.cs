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
// Original file : DefaultDslProviderService.cs
// Project: Qorpent.Dsl
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using Qorpent.IoC;

namespace Qorpent.Dsl {
	/// <summary>
	/// 	default implementation of DslProviderService - supply XsltBasedDslProvider
	/// </summary>
	/// <remarks>
	/// </remarks>
	[ContainerComponent(Lifestyle.Singleton)]
	public class DefaultDslProviderService : ServiceBase, IDslProviderService {
		/// <summary>
		/// 	Gets the provider.
		/// </summary>
		/// <param name="project"> The project. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public IDslProvider GetProvider(DslProject project) {
			lock (this) {
				Log.Debug("requested project lang " + project.LangName, this);
				var configuredDsl = ResolveService<IDslProvider>(project.LangName + ".dsl.provider");

				if (null == configuredDsl) {
					configuredDsl = new XsltBasedDslProvider();
					((XsltBasedDslProvider) configuredDsl).SetContainerContext(Container, Component);
				}
				if (null == configuredDsl.ResourceProvider) {
					configuredDsl.ResourceProvider = ResolveService<IDslResourceProvider>(project.LangName + ".dsl.resource.provider");
				}
				if (null == configuredDsl.XmlPreprocessor) {
					configuredDsl.XmlPreprocessor = ResolveService<IDslXmlPreprocessor>(project.LangName + ".dsl.xml.preprocessor");
				}
				return configuredDsl;
			}
		}
	}
}