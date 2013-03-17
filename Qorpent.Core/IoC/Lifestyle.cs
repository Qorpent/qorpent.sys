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
// PROJECT ORIGIN: Qorpent.Core/Lifestyle.cs
#endregion
using System;

namespace Qorpent.IoC {
	/// <summary>
	/// 	Lifestyle flag for IoC component behaviour
	/// </summary>
	[Flags]
	public enum Lifestyle {
		/// <summary>
		/// 	Not default behavior given, will be used default for Container (mostly transient)
		/// </summary>
		Default = 1,

		/// <summary>
		/// 	Service configured as singleton - not ne will created (no release needed)
		/// </summary>
		Singleton = 2,

		/// <summary>
		/// 	If objects will be returned to container, they will be added to pool, on retrieve - if pool is not empty
		/// 	they will be got from pool (release needed)
		/// </summary>
		Pooled = 4,

		/// <summary>
		/// 	Every thread will contains self copy of service, every double requests on get will return same (no release needed)
		/// </summary>
		PerThread = 8,

		/// <summary>
		/// 	Every return will construct new object
		/// </summary>
		Transient = 16,

		/// <summary>
		/// 	Used for objects that MUST instanced through All, can use implementation
		/// </summary>
		Extension = 32,

		/// <summary>
		/// 	It's special mark for container extensions, RegisterComponent have to redirect requests
		/// 	to RegisterExtension when matched
		/// </summary>
		ContainerExtension = 64
	}
}