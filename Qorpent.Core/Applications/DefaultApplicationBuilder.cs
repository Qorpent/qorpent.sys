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
// Original file : DefaultApplicationBuilder.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Configuration;
using System.IO;
using Qorpent.IO;
using Qorpent.IoC;

namespace Qorpent.Applications {
	/// <summary>
	/// 	generates default application instance for current process
	/// </summary>
	public class DefaultApplicationBuilder {
		/// <summary>
		/// 	Creates the default application.
		/// </summary>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public IApplication CreateDefaultApplication(Type applicationImplementationType = null) {
			var applicationtype = applicationImplementationType ?? ResolveApplicationType();
			var application = (IApplication) Activator.CreateInstance(applicationtype);
			application.Container = ContainerFactory.CreateDefault();
			ContainerFactory.DumpContainer(application.Container, application.Container.Get<IFileNameResolver>().Resolve(FileSearchQuery.Leveled("~/.tmp/container.dump")));
			return application;
		}

		

		/// <summary>
		/// 	Gets the type of the container.
		/// </summary>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		private Type ResolveApplicationType() {
			var configuredType = ConfigurationManager.AppSettings.Get(QorpentConst.Config.ApplicationTypeAppSetting);
#if !SQL2008
			if (!string.IsNullOrWhiteSpace(configuredType)) {
#else
			if (!string.IsNullOrEmpty(configuredType))
			{
#endif
				try {
					return Type.GetType(configuredType);
				}
				catch (Exception e) {
					throw new ContainerException("cannot load configured type :" + configuredType, e);
				}
			}
			return typeof (Application);
		}
	}
}