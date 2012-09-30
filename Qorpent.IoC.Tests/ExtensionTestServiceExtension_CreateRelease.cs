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
// Original file : ExtensionTestServiceExtension_CreateRelease.cs
// Project: Qorpent.IoC.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.IoC.Tests {
	public class ExtensionTestServiceExtension_CreateRelease : IContainerExtension {
		public IContainer Container { get; set; }

		public ContainerOperation SupportedOperations {
			get { return ContainerOperation.AfterCreate | ContainerOperation.Release; }
		}

		public void Process(ContainerContext context) {
			if (typeof (IExtensionTestService) == context.Component.ServiceType) {
				var obj = (IExtensionTestService) context.Object;
				if (context.Operation == ContainerOperation.AfterCreate) {
					Assert.True(obj.ContainerBoundCalled);
					obj.ExtensionBoundTest = context.Component.Parameters.GetValue<int>("ebt", -1);
				}
				else {
					obj.ExtensionRelease = true;
				}
			}
		}

		public int Order {
			get { return 10; }
		}
	}
}