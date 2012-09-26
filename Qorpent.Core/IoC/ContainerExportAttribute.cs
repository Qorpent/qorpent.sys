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
// Original file : ContainerExportAttribute.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;

namespace Qorpent.IoC {
	/// <summary>
	/// 	Marks defaults for assembly manifest, marks assemblies to create export manifest
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
	public class ContainerExportAttribute : ContainerAttribute {
		/// <summary>
		/// 	MSBuild-propose string only notation for constructor
		/// </summary>
		/// <param name="defaultPriority"> </param>
		/// <param name="defaultLifestyle"> </param>
		public ContainerExportAttribute(string defaultPriority, string defaultLifestyle) :
			this(Convert.ToInt32(defaultPriority),
			     (Lifestyle) Enum.Parse(typeof (Lifestyle), defaultLifestyle, true)) {}

		/// <summary>
		/// 	Creates top-level assembly manifest attribute
		/// </summary>
		/// <param name="priority"> </param>
		/// <param name="lifestyle"> </param>
		public ContainerExportAttribute(int priority = 1000, Lifestyle lifestyle = Lifestyle.Transient) {
			Priority = priority;
			Lifestyle = lifestyle;
		}
	}
}