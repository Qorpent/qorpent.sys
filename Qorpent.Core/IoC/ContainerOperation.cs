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
// Original file : ContainerOperation.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;

namespace Qorpent.IoC {
	/// <summary>
	/// 	object operation in container
	/// </summary>
	[Flags]
	public enum ContainerOperation {
		/// <summary>
		/// 	undefined, none
		/// </summary>
		None = 0,

		/// <summary>
		/// 	before new instance created
		/// </summary>
		BeforeCreate = 1,

		/// <summary>
		/// 	after new instance created
		/// </summary>
		AfterCreate = 2,

		/// <summary>
		/// 	before component activated
		/// </summary>
		BeforeActivate = 4,

		/// <summary>
		/// 	after component activated
		/// </summary>
		AfterActivate = 8,

		/// <summary>
		/// 	object released (after)
		/// </summary>
		Release = 16,

		/// <summary>
		/// 	Before Component registered (can modify component)
		/// </summary>
		BeforeRegisterComponent = 32,

		/// <summary>
		/// 	After Component unregisterd
		/// </summary>
		UnregisterComponent = 64,

		/// <summary>
		/// 	Before default Get work
		/// </summary>
		BeforeGet = 128,

		/// <summary>
		/// 	After default Get
		/// </summary>
		AfterGet = 256,

		/// <summary>
		/// 	Before default All work
		/// </summary>
		BeforeAll = 512,

		/// <summary>
		/// 	After default All
		/// </summary>
		AfterAll = 1024,

		/// <summary>
		/// 	Called after component is registered
		/// </summary>
		AfterRegisterComponent = 2048
	}
}