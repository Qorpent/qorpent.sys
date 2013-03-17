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
// PROJECT ORIGIN: Qorpent.IoC/ComponentDefinition.cs
#endregion
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace Qorpent.IoC {
	/// <summary>
	/// 	Container component definition for container (generic)
	/// </summary>
	public class ComponentDefinition<TService, TImplementation> : IComponentDefinition where TService : class
	                                                                                   where TImplementation : class,
		                                                                                   TService {
		/// <summary>
		/// </summary>
		public ComponentDefinition() {
			ServiceType = typeof (TService);
			ImplementationType = typeof (TImplementation);
			Lifestyle = Lifestyle.Default;
			Priority = 1000;
			Name = "";
		}

		/// <summary>
		/// 	creates new component in generic style
		/// </summary>
		/// <param name="lifestyle"> </param>
		/// <param name="priority"> </param>
		/// <param name="implementation"> </param>
		/// <param name="name"> </param>
		public ComponentDefinition(Lifestyle lifestyle = Lifestyle.Default,
		                           string name = "", int priority = 1000, TImplementation implementation = null) {
			ServiceType = typeof (TService);
			ImplementationType = typeof (TImplementation);
			Lifestyle = lifestyle;
			//implementation must be assigned only if it's singleton
			if (null != implementation && (Lifestyle.Singleton == lifestyle || Lifestyle.Extension == lifestyle)) {
				Implementation = implementation;
			}
			name = name ?? "";
			Name = name;
			Priority = priority;
		}


		/// <summary>
		/// 	Some custom parameters for component
		/// </summary>
		public IDictionary<string, object> Parameters {
			get { return _parameters; }
		}

		/// <summary>
		/// 	Container-wide Id (must be supplied by container)
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
		/// 	При первом создании и при наличии стиля жизни Extension - объекты будут сохраняться в <see cref="Implementation" />
		/// </summary>
		public bool CacheInstanceOfExtension { get; set; }

		/// <summary>
		/// 	Priority hint for Best component resolution
		/// </summary>
		public int Priority { get; set; }

		/// <summary>
		/// 	(reserved) - defined to setup role security on access container
		/// </summary>
		public string Role { get; set; }

		/// <summary>
		/// 	Help string on component - to provide addition information about configured component
		/// </summary>
		public string Help { get; set; }

		/// <summary>
		/// 	Опицональный элемент XML из которого произведена загрузка компонента (при манифестах)
		/// </summary>
		public XElement Source { get; set; }


		/// <summary>
		/// 	Flow set property method
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="value"> </param>
		/// <returns> </returns>
		public ComponentDefinition<TService, TImplementation> Set(string name, object value) {
			Parameters[name] = value;
			return this;
		}

		/// <summary>
		/// 	Serves as a hash function for a particular type.
		/// </summary>
		/// <returns> A hash code for the current <see cref="T:System.Object" /> . </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode() {
			return string.Format("{0}{1}{2}{3}{4}{5}",
			                     ServiceType.Name,
			                     ImplementationType.Name,
			                     Name,
			                     Priority,
			                     Lifestyle,
			                     Implementation == null
				                     ? "NULL"
				                     : Implementation.GetHashCode().ToString(CultureInfo.InvariantCulture)
				).GetHashCode();
		}

		/// <summary>
		/// 	Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.
		/// </summary>
		/// <returns> true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" /> ; otherwise, false. </returns>
		/// <param name="obj"> The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" /> . </param>
		/// <filterpriority>2</filterpriority>
		public override bool Equals(object obj) {
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			var cd = obj as IComponentDefinition;
			if (null == cd) {
				return false;
			}
			if (ServiceType != cd.ServiceType) {
				return false;
			}
			if (ImplementationType != cd.ImplementationType) {
				return false;
			}
			if (Lifestyle != cd.Lifestyle) {
				return false;
			}
			if (Name != cd.Name) {
				return false;
			}
			if (Priority != cd.Priority) {
				return false;
			}
			if (Implementation != cd.Implementation) {
				return false;
			}
			if (Parameters.Count != cd.Parameters.Count) {
				return false;
			}
			foreach (var parameter in Parameters) {
				if (!cd.Parameters.ContainsKey(parameter.Key)) {
					return false;
				}
				if (parameter.Value != cd.Parameters[parameter.Key]) {
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// 	Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
		/// </summary>
		/// <returns> A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" /> . </returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString() {
			return string.Format("{0} {1} {2} ({3}) {4} {5} {6} ", Lifestyle, Name, ServiceType, ImplementationType, Priority,
			                     null == Implementation ? "NULL" : "WITH IMPLEMENTATION",
			                     0 == Parameters.Count ? "NO PARAMETERS" : "WITH PARAMETERS");
		}

		private readonly IDictionary<string, object> _parameters = new Dictionary<string, object>();
		                                                                                   }


	/// <summary>
	/// 	Container component definition for container (native)
	/// </summary>
	public class ComponentDefinition : ComponentDefinition<object, object> {
		/// <summary>
		/// </summary>
		public ComponentDefinition() {}

		/// <summary>
		/// 	creates new component for given types
		/// </summary>
		/// <param name="tService"> </param>
		/// <param name="tImplementation"> </param>
		/// <param name="lifestyle"> </param>
		/// <param name="name"> </param>
		/// <param name="priority"> </param>
		/// <param name="implementation"> </param>
		public ComponentDefinition(Type tService, Type tImplementation, Lifestyle lifestyle = Lifestyle.Default,
		                           string name = "", int priority = 1000, object implementation = null) {
			if (null == tService) {
				throw new ArgumentNullException("tService");
			}
			if (null == tImplementation) {
				throw new ArgumentNullException("tImplementation");
			}
			if (tService.IsValueType) {
				throw new ArgumentException("tService must be interface or reference class");
			}
			if (!tService.IsAssignableFrom(tImplementation)) {
				throw new ArgumentException("tImplementation must be descendant of tService");
			}
			if (tImplementation.IsAbstract) {
				throw new ArgumentException("tImplementation cannot be abstract");
			}
			if (null != implementation) {
				if (!tImplementation.IsInstanceOfType(implementation)) {
					throw new ArgumentException("implementation must be if tImplementation type");
				}
			}
			name = name ?? "";
			Name = name;
			Priority = priority;
			ServiceType = tService;
			ImplementationType = tImplementation;
			Lifestyle = lifestyle;
			if (null != implementation && (Lifestyle.Singleton == lifestyle || Lifestyle.Extension == lifestyle)) {
				Implementation = implementation;
			}
		}
	}
}