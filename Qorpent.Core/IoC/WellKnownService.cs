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
// PROJECT ORIGIN: Qorpent.Core/WellKnownService.cs
#endregion
using System;

namespace Qorpent.IoC {
	/// <summary>
	/// </summary>
	public class WellKnownService {
		/// <summary>
		/// 	Initializes a new instance of the <see cref="WellKnownService&lt;T&gt;" /> class.
		/// </summary>
		/// <param name="serviceType"> </param>
		/// <param name="wellKnownTypeName"> Name of the well known type. </param>
		/// <param name="defaultType"> The default type. </param>
		/// <param name="lifestyle"> The lifestyle. </param>
		/// <param name="name"> Asserted name of component </param>
		/// <remarks>
		/// </remarks>
		public WellKnownService(Type serviceType, string wellKnownTypeName, Type defaultType, Lifestyle lifestyle,
		                        string name = null) {
			ServiceType = serviceType;
			WellKnownTypeName = wellKnownTypeName;
			DefaultType = defaultType;
			if (string.IsNullOrEmpty(wellKnownTypeName)) {
				ResolvedWellKnownType = DefaultType;
			}
			else {
				ResolvedWellKnownType = Type.GetType(WellKnownTypeName, false) ?? DefaultType;
			}
			Lifestyle = lifestyle;
			Name = name;
		}

		/// <summary>
		/// 	Gets or sets the lifestyle.
		/// </summary>
		/// <value> The lifestyle. </value>
		/// <remarks>
		/// </remarks>
		public Lifestyle Lifestyle { get; set; }

		/// <summary>
		/// 	Gets or sets the priority.
		/// </summary>
		/// <value> The priority. </value>
		/// <remarks>
		/// </remarks>
		public int Priority { get; set; }

		/// <summary>
		/// 	Name of component
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// </summary>
		public Type DefaultType;

		/// <summary>
		/// </summary>
		public Type ResolvedWellKnownType;

		/// <summary>
		/// </summary>
		public Type ServiceType;

		/// <summary>
		/// </summary>
		public string WellKnownTypeName;
	}

	/// <summary>
	/// 	Description for well known types in Qorpent
	/// </summary>
	/// <typeparam name="T"> </typeparam>
	/// <remarks>
	/// </remarks>
	public class WellKnownService<T> : WellKnownService {
		/// <summary>
		/// 	Initializes a new instance of the <see cref="WellKnownService&lt;T&gt;" /> class.
		/// </summary>
		/// <param name="wellKnownTypeName"> Name of the well known type. </param>
		/// <param name="defaultType"> The default type. </param>
		/// <param name="lifestyle"> The lifestyle. </param>
		/// <param name="name"> Asserted component name </param>
		/// <remarks>
		/// </remarks>
		public WellKnownService(string wellKnownTypeName, Type defaultType = null, Lifestyle lifestyle = Lifestyle.Transient,
		                        string name = null) : base(typeof (T), wellKnownTypeName, defaultType, lifestyle, name) {}
	}
}