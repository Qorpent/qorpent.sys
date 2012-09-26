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
// Original file : IContainerExtension.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

namespace Qorpent.IoC {
	/// <summary>
	/// 	Container extension interface - extensions are used with Container
	/// 	in SimpleContainer extensions can totally override behavior of controller
	/// 	in working Container extensions can just override componentResolution and 
	/// 	make some operations on components and objects during activations, but cannot replace or override
	/// 	object or objectself ant those creation totally
	/// </summary>
	public interface IContainerExtension {
		/// <summary>
		/// 	Back reference to containing container
		/// </summary>
		IContainer Container { get; set; }

		/// <summary>
		/// 	Operations that served by this extensions (flags OR contruct)
		/// </summary>
		ContainerOperation SupportedOperations { get; }

		/// <summary>
		/// 	Order for extension's collection iteration
		/// </summary>
		int Order { get; }

		/// <summary>
		/// 	Execute extension's job in given context
		/// </summary>
		/// <param name="context"> </param>
		void Process(ContainerContext context);
	}
}