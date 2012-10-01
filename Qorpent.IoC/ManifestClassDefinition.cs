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
// Original file : ManifestClassDefinition.cs
// Project: Qorpent.IoC
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.IoC {
	/// <summary>
	/// 	Type descriptor for exported component
	/// </summary>
	public class ManifestClassDefinition {
		/// <summary>
		/// 	generates container manifest of given type
		/// </summary>
		/// <param name="type"> </param>
		public ManifestClassDefinition(Type type) {
			// we use indirect attribute usage to avoid msbuild context problems - we have to make compoents even in different versions of system
			Type = type;
			var predesc =
				type.GetCustomAttributes(true).Where(x => x.GetType().Name == typeof (ContainerComponentAttribute).Name
				                                          ||
				                                          x.GetType().BaseType.Name == typeof (ContainerComponentAttribute).Name).
					FirstOrDefault();
			if (null != predesc) {
				Descriptor = new ContainerComponentAttribute();
				Descriptor.Lifestyle = predesc.GetValue<Lifestyle>("Lifestyle");
				Descriptor.Name = predesc.GetValue<string>("Name");
				Descriptor.Help = predesc.GetValue<string>("Help");
				Descriptor.Role = predesc.GetValue<string>("Role");
				Descriptor.ServiceType = predesc.GetValue<Type>("ServiceType");
				Descriptor.Priority = predesc.GetValue<int>("Priority");
			}

			if (null != Descriptor) {
				if (null == Descriptor.ServiceType) {
					AutoDetectedServiceType =
						type.GetInterfaces().Except(type.BaseType.GetInterfaces()).FirstOrDefault(x => x != typeof (IContainerBound));
					if (null == AutoDetectedServiceType) {
						AutoDetectedServiceType = type;
					}
				}
			}
		}



		/// <summary>
		/// Создает манифест для указанного типа и атрибута
		/// </summary>
		/// <param name="type"></param>
		/// <param name="predesc"></param>
		public ManifestClassDefinition(Type type, Attribute predesc)
		{
			// we use indirect attribute usage to avoid msbuild context problems - we have to make compoents even in different versions of system
			Type = type;
		
			if (null != predesc)
			{
				Descriptor = new ContainerComponentAttribute();
				Descriptor.Lifestyle = predesc.GetValue<Lifestyle>("Lifestyle");
				Descriptor.Name = predesc.GetValue<string>("Name");
				Descriptor.Help = predesc.GetValue<string>("Help");
				Descriptor.Role = predesc.GetValue<string>("Role");
				Descriptor.ServiceType = predesc.GetValue<Type>("ServiceType");
				Descriptor.Priority = predesc.GetValue<int>("Priority");
			}

			if (null != Descriptor)
			{
				if (null == Descriptor.ServiceType)
				{
					AutoDetectedServiceType =
						type.GetInterfaces().Except(type.BaseType.GetInterfaces()).FirstOrDefault(x => x != typeof(IContainerBound));
					if (null == AutoDetectedServiceType)
					{
						AutoDetectedServiceType = type;
					}
				}
			}
		}


		/// <summary>
		/// Retrieves all components for all attributes
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static IEnumerable<ManifestClassDefinition> GetAllClassManifests(Type type) {
			var attributes =
				type.GetCustomAttributes(true).Where(x => x.GetType().Name == typeof (ContainerComponentAttribute).Name
				                                          ||
				                                          x.GetType().BaseType.Name == typeof (ContainerComponentAttribute).Name).
					OfType<Attribute>().ToArray();
			foreach (var attribute in attributes) {
				yield return new ManifestClassDefinition(type, attribute);
			}

		}




		/// <summary>
		/// 	Containing export assembly
		/// </summary>
		public AssemblyManifestDefinition AssemblyManifest { get; set; }

		/// <summary>
		/// 	Attribute with component settings
		/// </summary>
		public ContainerComponentAttribute Descriptor { get; private set; }

		/// <summary>
		/// 	Target type
		/// </summary>
		public Type Type { get; private set; }

		/// <summary>
		/// 	Auto detected service type from definition
		/// </summary>
		public Type AutoDetectedServiceType { get; private set; }

		/// <summary>
		/// 	generates ioc component definition
		/// </summary>
		/// <returns> </returns>
		public IComponentDefinition GetComponent() {
			var stype = Descriptor.ServiceType ?? AutoDetectedServiceType;
			var impltype = Type;
			var lifestyle = (Descriptor.Lifestyle == Lifestyle.Default && null != AssemblyManifest)
				                ? AssemblyManifest.Descriptor.Lifestyle
				                : Descriptor.Lifestyle;
			var priority = (Descriptor.Priority == -1 && null != AssemblyManifest)
				               ? AssemblyManifest.Descriptor.Priority
				               : Descriptor.Priority;
			var result = new ComponentDefinition(stype, impltype, lifestyle, Descriptor.Name, priority, null);
			result.Role = Descriptor.Role;
			result.Help = Descriptor.Help;
			return result;
		}
	}
}