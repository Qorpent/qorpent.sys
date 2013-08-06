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
// PROJECT ORIGIN: Qorpent.Core/WellKnownAttachedComponentDefinition.cs
#endregion
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Qorpent.IoC {
	/// <summary>
	/// 	Wellknownservice to container adapter
	/// </summary>
	internal class WellKnownAttachedComponentDefinition : IComponentDefinition {
		public WellKnownAttachedComponentDefinition(WellKnownService service) {
			Lifestyle = service.Lifestyle;
			ServiceType = service.ServiceType;
			ImplementationType = service.ResolvedWellKnownType;
			Priority = service.Priority;
			if (Priority <= 0) {
				Priority = 2000;
			}
			Parameters = new Dictionary<string, object>();
			Name = service.Name;
			Tag = service.Tag;
		}


		public object Implementation { get; set; }
		public Lifestyle Lifestyle { get; set; }
		public Type ImplementationType { get; set; }
		public Type ServiceType { get; set; }
		public string Name { get; set; }
		public int Priority { get; set; }
		public IDictionary<string, object> Parameters { get; private set; }
		public int ContainerId { get; set; }
		public int ActivationCount { get; set; }
		public int CreationCount { get; set; }
		public string Role { get; set; }
		public string Help { get; set; }
		public XElement Source { get; set; }
		public bool CacheInstanceOfExtension { get; set; }

		public string Tag { get; set; }
	}
}