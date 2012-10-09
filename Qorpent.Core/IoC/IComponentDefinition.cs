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
// Original file : IComponentDefinition.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Qorpent.Model;

namespace Qorpent.IoC {
	/// <summary>
	/// 	Component definition interface
	/// </summary>
	public interface IComponentDefinition : IWithRole {
		/// <summary>
		/// 	Singleton, manual made object
		/// </summary>
		Object Implementation { get; set; }


		/// <summary>
		/// 	Lifestyle of object
		/// </summary>
		Lifestyle Lifestyle { get; set; }

		/// <summary>
		/// 	Type that implements Service
		/// </summary>
		Type ImplementationType { get; set; }

		/// <summary>
		/// 	Type of Service
		/// </summary>
		Type ServiceType { get; set; }

		/// <summary>
		/// 	Individual Name of component
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// 	Priority hint for Best component resolution
		/// </summary>
		int Priority { get; set; }

		/// <summary>
		/// 	Some custom parameters for component
		/// </summary>
		IDictionary<string, object> Parameters { get; }

		/// <summary>
		/// 	Container-side Id (must be supplied by container)
		/// </summary>
		int ContainerId { get; set; }

		/// <summary>
		/// 	Container-side statistic of Activations (Gets) on component
		/// </summary>
		int ActivationCount { get; set; }

		/// <summary>
		/// 	Container-side statistic of Activations (Gets) on component which cause creation of object
		/// </summary>
		int CreationCount { get; set; }


		/// <summary>
		/// 	Help string on component - to provide addition information about configured component
		/// </summary>
		string Help { get; set; }

		/// <summary>
		/// 	������������ ������� XML �� �������� ����������� �������� ���������� (��� ����������)
		/// </summary>
		XElement Source { get; set; }

		/// <summary>
		/// ��� ������ �������� � ��� ������� ����� ����� Extension - ������� ����� ����������� � <see cref="Implementation"/>
		/// </summary>
		bool CacheInstanceOfExtension { get; set; }
	}

	/// <summary>
	/// </summary>
	public class BasicComponentDefinition : IComponentDefinition {
		/// <summary>
		/// </summary>
		public BasicComponentDefinition() {
			Parameters = new Dictionary<string, object>();
		}


		/// <summary>
		/// 	Singleton, manual made object
		/// </summary>
		public object Implementation { get; set; }

		/// <summary>
		/// 	Lifestyle of object
		/// </summary>
		public Lifestyle Lifestyle { get; set; }

		/// <summary>
		/// 	Type that implements Service
		/// </summary>
		public Type ImplementationType { get; set; }

		/// <summary>
		/// 	Type of Service
		/// </summary>
		public Type ServiceType { get; set; }

		/// <summary>
		/// 	Individual Name of component
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 	Priority hint for Best component resolution
		/// </summary>
		public int Priority { get; set; }

		/// <summary>
		/// 	Some custom parameters for component
		/// </summary>
		public IDictionary<string, object> Parameters { get; private set; }

		/// <summary>
		/// 	Container-side Id (must be supplied by container)
		/// </summary>
		public int ContainerId { get; set; }

		/// <summary>
		/// 	Container-side statistic of Activations (Gets) on component
		/// </summary>
		public int ActivationCount { get; set; }

		/// <summary>
		/// 	Container-side statistic of Activations (Gets) on component which cause creation of object
		/// </summary>
		public int CreationCount { get; set; }

		/// <summary>
		/// 	(reserved) - defined to setup role security on access container
		/// </summary>
		public string Role { get; set; }

		/// <summary>
		/// 	Help string on component - to provide addition information about configured component
		/// </summary>
		public string Help { get; set; }

		/// <summary>
		/// 	������������ ������� XML �� �������� ����������� �������� ���������� (��� ����������)
		/// </summary>
		public XElement Source { get; set; }

		/// <summary>
		/// ��� ������ �������� � ��� ������� ����� ����� Extension - ������� ����� ����������� � <see cref="IComponentDefinition.Implementation"/>
		/// </summary>
		public bool CacheInstanceOfExtension { get; set; }
	}
}