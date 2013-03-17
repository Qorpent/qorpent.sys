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
// PROJECT ORIGIN: Qorpent.Core/ApplicationBoundContainerExtension.cs
#endregion
using Qorpent.IoC;

namespace Qorpent.Applications {
	/// <summary>
	/// 	provide facility to embed application to IApplicationBound instances of IoC
	/// </summary>
	public class ApplicationBoundContainerExtension : IContainerExtension {
		/// <summary>
		/// 	creates new instance of application bound extension
		/// </summary>
		/// <param name="application"> </param>
		public ApplicationBoundContainerExtension(IApplication application) {
			Application = application;
		}

		/// <summary>
		/// 	Back reference to application
		/// </summary>
		public IApplication Application { get; set; }


		/// <summary>
		/// 	Back reference to containing container
		/// </summary>
		public IContainer Container { get; set; }

		/// <summary>
		/// 	Operations that served by this extensions (flags OR contruct)
		/// </summary>
		public ContainerOperation SupportedOperations {
			get { return ContainerOperation.AfterCreate; }
		}

		/// <summary>
		/// 	Execute extension's job in given context
		/// </summary>
		/// <param name="context"> </param>
		public void Process(ContainerContext context) {
			var appbound = context.Object as IApplicationBound;
			if (null == appbound) {
				return;
			}
			if (ContainerOperation.AfterCreate == context.Operation) {
				appbound.SetApplication(Application);
			}
		}

		/// <summary>
		/// 	Order for extension's collection iteration
		/// </summary>
		public int Order {
			get { return 10; }
		}
	}
}