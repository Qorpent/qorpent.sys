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
// Original file : iocconfiguationtest.cs
// Project: Qorpent.Serialization.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using NUnit.Framework;
using Qorpent.IoC;

namespace Qorpent.Serialization.Tests {
	[TestFixture]
	public class iocconfiguationtest {
		[SetUp]
		public void SetUp() {
			container = new Container();
			ContainerFactory.SetupWellKnownContainerServices(container);
		}


		private IContainer container;

		[TestCase(typeof (ISerializerFactory), null, typeof (DefaultSerializerFactory))]
		[TestCase(typeof (ISerializer), "bxl.serializer", typeof (BxlSerializer))]
		[TestCase(typeof (ISerializer), "xml.serializer", typeof (XmlSerializer))]
		[TestCase(typeof (ISerializer), "js.serializer", typeof (JsSerializer))]
		[TestCase(typeof (ISerializer), "json.serializer", typeof (JsonSerializer))]
		[TestCase(typeof (ISerializer), "md5.serializer", typeof (Md5Serializer))]
		public void ClassesAreWellConfigured(Type type, string name, Type resultType) {
			var result = container.Get(type, name);
			Assert.NotNull(result);
			Assert.IsInstanceOf(resultType, result);
		}
	}
}