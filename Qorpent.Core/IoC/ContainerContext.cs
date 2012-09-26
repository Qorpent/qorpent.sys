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
// Original file : ContainerContext.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;

namespace Qorpent.IoC {
	/// <summary>
	/// 	Context of container operation execution to provide data for extensions
	/// </summary>
	public class ContainerContext {
		/// <summary>
		/// 	current processing operation
		/// </summary>
		public ContainerOperation Operation { get; set; }

		/// <summary>
		/// 	Type that was requested (in Get, All operations)
		/// </summary>
		public Type RequestedType { get; set; }

		/// <summary>
		/// 	Name that was requested (in Get, All operations)
		/// </summary>
		public string RequestedName { get; set; }

		/// <summary>
		/// 	Component for
		/// 	in for:
		/// 	AfterCreate, BeforeActivate,AfterActivate,Release, UnregisterComponent,
		/// 	AfterGet,AfterAll
		/// 	in/out for 
		/// 	RegisterComponent, BeforeGet
		/// 	not served for BeforeAll,AfterAll
		/// </summary>
		public IComponentDefinition Component { get; set; }

		/// <summary>
		/// 	out: BeforeGet, BeforeAll, BeforeCreate
		/// 	in - other cases
		/// </summary>
		public object Object { get; set; }

		/// <summary>
		/// 	Resolved type in BeforeGet
		/// </summary>
		public Type ResolvedType { get; set; }
	}
}