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
// Original file : LoadLoggerFromComponentDefinitionSource.cs
// Project: Qorpent.Log.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.IO;
using System.Xml.Linq;
using NUnit.Framework;
using Qorpent.Bxl;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace Qorpent.Log.Tests {
	[TestFixture]
	public class LoadLoggerFromComponentDefinitionSource {
		[Test]
		public void Basic_Test() {
			var c = ContainerFactory.CreateEmpty();
			c.Register(c.NewComponent<ILogManager, DefaultLogManager>(Lifestyle.Singleton));
			var sw = new StringWriter();
			var writeComponent = c.NewComponent<ILogWriter, TextWriterLogWriter>(Lifestyle.Transient,
			                                                                         "def.writer");
			writeComponent.Parameters["_writer"] = sw;
			writeComponent.Parameters["level"] = LogLevel.Error;
			writeComponent.Parameters["customformat"] = "${Message}";
			var loggerComponent = c.NewComponent<ILogger, Logger>(Lifestyle.Transient, "logger");
			loggerComponent.Source = XElement.Parse("<test><writer code='def.writer'/><writer code='empty'/></test>");
			loggerComponent.Parameters["mask"] = ".";
			c.Register(writeComponent);
			c.Register(loggerComponent);

			var logger = c.Get<ILogManager>().GetLog("any", this);
			logger.Warn("x");
			logger.Error("z");
			c.Get<ILogManager>().Join();
			Assert.AreEqual("z", sw.ToString().Trim());
		}


	}
}