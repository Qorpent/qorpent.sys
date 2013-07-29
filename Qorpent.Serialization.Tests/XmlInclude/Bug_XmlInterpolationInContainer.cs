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
// Original file : Bug_XmlInterpolationInContainer.cs
// Project: Qorpent.Dsl.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using NUnit.Framework;
using Qorpent.Dsl;
using Qorpent.IoC;

namespace Qorpent.Serialization.Tests.XmlInclude {
	[TestFixture]
	public class Bug_XmlInterpolationInContainer {
		[Test]
		public void Can_Find_Interpolator_In_Container_After_Register() {
			var c = ContainerFactory.CreateEmpty();
			c.RegisterAssembly(typeof (XmlInterpolationPreprocessor).Assembly);
			Assert.NotNull(c.Get<IXmlInterpolationPreprocessor>());
		}
	}
}