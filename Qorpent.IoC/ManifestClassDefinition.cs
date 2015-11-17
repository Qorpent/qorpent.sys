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
// PROJECT ORIGIN: Qorpent.IoC/ManifestClassDefinition.cs
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
		public ManifestClassDefinition(Type type, object[] predesc = null) {
            
            // we use indirect attribute usage to avoid msbuild context problems - we have to make compoents even in different versions of system
            Type = type;
            predesc = predesc ??
                type.GetCustomAttributes(true).Where(x =>
                {
                    var baseType = x.GetType().BaseType;
                    return baseType != null && (x.GetType().Name == typeof(ContainerComponentAttribute).Name
                                                                                      ||
                                                                                      baseType.Name == typeof(ContainerComponentAttribute).Name);
                }).ToArray();

            foreach (var p in predesc)
            {
                var d = new ContainerComponentAttribute
                {
                    Lifestyle = p.GetValue<Lifestyle>("Lifestyle"),
                    Name = p.GetValue<string>("Name"),
                    Help = p.GetValue<string>("Help"),
                    Role = p.GetValue<string>("Role"),
                    ServiceType = p.GetValue<Type>("ServiceType"),
                    Priority = p.GetValue<int>("Priority"),
                    Tag = p.GetValue<string>("Tag"),
                    Names = p.GetValue<string[]>("Names"),
                    ServiceTypes = p.GetValue<Type[]>("ServiceTypes")
                };
                if (null == d.ServiceType && null == d.ServiceTypes)
                {
                    d.ServiceType =
                        type.GetInterfaces()
                            .Except(type.BaseType.GetInterfaces())
                            .FirstOrDefault(x => x != typeof(IContainerBound)) ??
                        type;
                }
                Descriptors.Add(d);
            }
        }


		/// <summary>
		/// 	Создает манифест для указанного типа и атрибута
		/// </summary>
		/// <param name="type"> </param>
		/// <param name="predesc"> </param>
		public ManifestClassDefinition(Type type, Attribute[] predesc) :this(type,predesc?.OfType<object>().ToArray()){
			// we use indirect attribute usage to avoid msbuild context problems - we have to make compoents even in different versions of system
			

			
		}


		/// <summary>
		/// 	Containing export assembly
		/// </summary>
		public AssemblyManifestDefinition AssemblyManifest { get; set; }

		/// <summary>
		/// 	Attribute with component settings
		/// </summary>
		public IList<ContainerComponentAttribute> Descriptors { get; private set; } = new List<ContainerComponentAttribute>();

		/// <summary>
		/// 	Target type
		/// </summary>
		public Type Type { get; private set; }

		/// <summary>
		/// 	Auto detected service type from definition
		/// </summary>
		public Type AutoDetectedServiceType { get; private set; }

		/// <summary>
		/// 	Retrieves all components for all attributes
		/// </summary>
		/// <param name="type"> </param>
		/// <returns> </returns>
		public static IEnumerable<ManifestClassDefinition> GetAllClassManifests(Type type) {
			var attributes =
				type.GetCustomAttributes(true).Where(x => x.GetType().Name == typeof (ContainerComponentAttribute).Name
				                                          ||
// ReSharper disable PossibleNullReferenceException
				                                          x.GetType().BaseType.Name == typeof (ContainerComponentAttribute).Name).
// ReSharper restore PossibleNullReferenceException
					OfType<Attribute>().ToArray();
			return attributes.Select(attribute => new ManifestClassDefinition(type,new [] { attribute}));
		}

		/// <summary>
		/// 	generates ioc component definition
		/// </summary>
		/// <returns> </returns>
		public IEnumerable<IComponentDefinition> GetComponents() {
            
            foreach (var d in Descriptors) {
                IList<ComponentDefinition> components = new List<ComponentDefinition>();
                var stype = d.ServiceType ?? AutoDetectedServiceType;
                var impltype = Type;
                var lifestyle = (d.Lifestyle == Lifestyle.Default && null != AssemblyManifest)
                                    ? AssemblyManifest.Descriptor.Lifestyle
                                    : d.Lifestyle;
                var priority = (d.Priority == -1 && null != AssemblyManifest)
                                   ? AssemblyManifest.Descriptor.Priority
                                   : d.Priority;
                var names = d.Names ?? new[] { d.Name };
                var services = d.ServiceTypes ?? new[] { stype };

                foreach (var name in names)
                {
                    foreach (var service in services)
                    {
                        var subresult = new ComponentDefinition(service, impltype, lifestyle, name, priority)
                        { Role = d.Role, Help = d.Help, Tag = d.Tag };
                        components.Add(subresult);
                    }
                }
                if (lifestyle == Lifestyle.Singleton && components.Count != 1)
                {
                    foreach (var c in components)
                    {
                        c.LinkedSingletons = components.Where(_ => !Equals(_, c)).ToArray();
                    }
                }

                foreach (var componentDefinition in components) {
                    yield return componentDefinition;
                }
            }

			
            
		}
	}
}