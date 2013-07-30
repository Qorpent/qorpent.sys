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
// Original file : ExtensionTestService.cs
// Project: Qorpent.IoC.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using Qorpent.Applications;
using Qorpent.Utils.Extensions;

namespace Qorpent.IoC.Tests {
	public class ExtensionTestService : IExtensionTestService, IContainerBound, IApplicationBound {
		public string AppName { get; set; }


		public void SetApplication(IApplication app) {
			AppName = app.ApplicationName;
		}


		public void SetContainerContext(IContainer container, IComponentDefinition component) {
			ContainerBoundTest = component.Parameters.GetValue<int>("cbt", -1);
			ContainerBoundCalled = true;
		}

		public void OnContainerRelease() {
			ContainerBoundRelease = true;
		}

		public void OnContainerCreateInstanceFinished() {
			
		}


		public bool ContainerBoundCalled { get; set; }

		public int ContainerBoundTest { get; set; }
		public int ExtensionBoundTest { get; set; }
		public bool ContainerBoundRelease { get; set; }
		public bool ExtensionRelease { get; set; }
	}
}