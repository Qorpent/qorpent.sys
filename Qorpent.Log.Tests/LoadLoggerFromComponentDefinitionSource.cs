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
using Qorpent.Dsl;
using Qorpent.Dsl.XmlInclude;
using Qorpent.IoC;

namespace Qorpent.Log.Tests {
	[TestFixture]
	public class LoadLoggerFromComponentDefinitionSource {
		[Test]
		public void Basic_Test() {
			var c = ContainerFactory.CreateEmpty();
			c.Register(c.NewComponent<ILogManager, DefaultLogManager>(Lifestyle.Singleton));
			var sw = new StringWriter();
			var writeComponent = c.NewComponent<ILogWriter, BaseTextWriterLogWriter>(Lifestyle.Transient,
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


		[Test]
		public void Load_Log_Config_From_Templated_Manifest() {
			var c = ContainerFactory.CreateEmpty();
			c.Register(c.NewComponent<ILogManager, DefaultLogManager>(Lifestyle.Singleton));
			c.Register(c.NewComponent<IXmlIncludeProcessor, XmlIncludeProcessor>());
			c.Register(c.NewComponent<IBxlParser, BxlParser>());
			var tmpfile = Path.GetTempFileName();
			var bxl = string.Format(@"
qxi::template logfile
	transient name='Qorpent.Log.TextFileWriter, Qorpent.Log' : 'Qorpent.Log.ILogWriter, Qorpent.Core'
qxi::template logger
	transient name='Qorpent.Log.Logger, Qorpent.Log' : 'Qorpent.Log.ILogger, Qorpent.Core'
logfile def.writer, filename = '{0}'
logger main
	writer def.writer
", tmpfile.Replace("\\", "\\\\"));
			var bxlp = c.Get<IBxlParser>();
			var includer = c.Get<IXmlIncludeProcessor>();
			var xml = bxlp.Parse(bxl);
			xml = includer.Include(xml, "test", true, BxlParserOptions.NoLexData);
			c.GetLoader().LoadManifest(xml, false);


			var logger = c.Get<ILogManager>().GetLog("any", this);
			logger.Warn("x", new LogMessage {User = "test/test", Time = DateTime.MinValue, Server = "test"});
			c.Get<ILogManager>().Join();
			Console.WriteLine(File.ReadAllText(tmpfile).Trim());
			Assert.AreEqual(@"logitem level=Warning, time='01.01.0001 0:00:00', user='test/test', server=test ,logger='any' 
	host='''Qorpent.Log.Tests.LoadLoggerFromComponentDefinitionSource'''
	message='''
x
	'''", File.ReadAllText(tmpfile).Trim());
		}
	}
}