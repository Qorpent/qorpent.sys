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
// Original file : ContainerComponentAttribute.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;

namespace Qorpent.IoC {
	/// <summary>
	/// 	Marks classes to be exposed as components for container throug mainfest
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class ContainerComponentAttribute : ContainerAttribute {
		/// <summary>
		/// </summary>
		public ContainerComponentAttribute() : this(null, Lifestyle.Default, "", -1) {}

		/// <summary>
		/// 	Configures component with default service type (first not-IConrainerBound interface) with no name and default Priority/Lifestyle
		/// </summary>
		public ContainerComponentAttribute(Lifestyle lifestyle = Lifestyle.Default, string name = "", int priority = -1)
			: this(null, lifestyle, name, priority) {}

		/// <summary>
		/// 	Configures component with default service type (first not-IConrainerBound interface) with no name and default Priority/Lifestyle
		/// </summary>
		public ContainerComponentAttribute(Type serviceType = null, Lifestyle lifestyle = Lifestyle.Default, string name = "",
		                                   int priority = -1) {
			Priority = priority; //default component level for manifest (assembly level will be used)
			Lifestyle = lifestyle;
			Name = name;
			ServiceType = serviceType; //will be resolved automatically
		}

		/// <summary>
		/// 	Exposed service type, null - for auto detection
		/// </summary>
		public Type ServiceType { get; set; }

		/// <summary>
		/// 	Component name for container (for name-style resolution)
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 	Help string for action
		/// </summary>
		public string Help { get; set; }

		/// <summary>
		/// 	Access role for action
		/// </summary>
		public string Role { get; set; }
	}
}