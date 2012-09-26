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
// Original file : SystemBasicConfiguration.cs
// Project: Qorpent.Mvc.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Linq;
using NUnit.Framework;
using Qorpent.Applications;
using Qorpent.IoC;
using Qorpent.Mvc.Actions;

namespace Qorpent.Mvc.Tests {
	[TestFixture]
	public class SystemBasicConfiguration {
		[SetUp]
		public void setup() {
			container = ContainerFactory.CreateDefault();
			app = new Application {Container = container};
		}


		private IContainer container;
		private Application app;

		[Test]
		public void ActionsArePrepared() {
			var types =
				typeof (EchoAction).Assembly.GetTypes().Where(x => typeof (IAction).IsAssignableFrom(x) && !x.IsAbstract);
			foreach (var type in types) {
				Assert.NotNull(container.Get<IAction>(MvcFactory.NormalizeActionName(ActionAttribute.GetName(type))), type.FullName);
			}
		}

		[Test]
		public void RendersArePrepared() {
			var types =
				typeof (EchoAction).Assembly.GetTypes().Where(x => typeof (IRender).IsAssignableFrom(x) && !x.IsAbstract);
			foreach (var type in types) {
				Assert.NotNull(container.Get<IRender>(RenderAttribute.GetName(type) + ".render"), type.FullName);
			}
		}
	}
}