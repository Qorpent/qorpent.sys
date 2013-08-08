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
// Original file : Program.cs
// Project: Qorpent.Sys.bxlparser
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Qorpent.Bxl;
using Qorpent.Dsl;
using Qorpent.IO;
using Qorpent.Qxi;
using Qorpent.Serialization;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;

namespace bxlparser {
	/// <summary>
	/// 	Консоль BXL-парсера
	/// </summary>
	public class Program {
		/// <summary>
		/// 	Запускает консоль на выполнение
		/// </summary>
		/// <param name="args"> </param>
		public static int Main(string[] args) {
			var render = new ConsoleRenderHelper();
			try {
				var consoleargs = new ConsoleArgumentHelper().Parse<BxlParserConsoleArgs>(args);
				if (consoleargs.Help) {
					OutputHelp();
					return 0;
				}
				var sources = consoleargs.GetSourceFiles().ToArray();
				if (!sources.Any()) {
					render.Render("WARN: No source files detected", ConsoleColor.Red);
					return -1;
				}
				else {
					EnlistSourcesIfNeeded(consoleargs, render, sources);
					ProcessConversion(consoleargs, sources, render);
				}
			}catch(Exception ex) {
				render.Render(ex.ToString(),ConsoleColor.Red);
				return -2;
			}
			return 0;
		}

		private static void ProcessConversion(BxlParserConsoleArgs consoleargs, IEnumerable<string> sources, ConsoleRenderHelper render) {
			var bxl = new BxlParser();
			var serializer = GetSerializer(consoleargs);
			var options = BxlParserOptions.None;
			var includer = new XmlIncludeProcessor(new FileNameResolver {Root = Environment.CurrentDirectory}) {Bxl = bxl};
			if (consoleargs.NoLexInfo) {
				options = options | BxlParserOptions.NoLexData;
			}
			foreach (var source in sources) {
				var serializedresult = ConvertFile(includer, source, options, serializer);
				if (consoleargs.ToConsole) {
					var comment = CreateComment(consoleargs.OutputFormat, "SOURCE: " + source);
					render.Render(comment, ConsoleColor.Green);
					render.Render(serializedresult, ConsoleColor.Yellow);
				}else {
					var outfilename = GetOutputFileName(source, consoleargs);
					if (outfilename.NormalizePath().ToLower() == source.NormalizePath().ToLower())
					{
						throw new Exception("cannot execute because result filename " + outfilename + " will overwrite source with same name");
					}
					File.WriteAllText(outfilename,serializedresult);
					render.Render("Write: "+outfilename,ConsoleColor.Yellow);
				}
			}
		}

		private static string GetOutputFileName(string source, BxlParserConsoleArgs consoleargs) {
			var sourcefilename = Path.GetFileName(source);
			var sourcedirname = Path.GetDirectoryName(source);
			var outputdir = sourcedirname;
			var outfilename = sourcefilename + ".bxl-parse." + consoleargs.OutputFormat.ToString().ToLower();
			if(!string.IsNullOrWhiteSpace(consoleargs.OutputDirectory)) {
				outputdir = consoleargs.OutputDirectory;
			}

			if(!string.IsNullOrWhiteSpace(consoleargs.OutputRename)) {
				var regex = consoleargs.OutputRename.Split(';')[0];
				var replace = consoleargs.OutputRename.Split(';')[1];
				outfilename = Regex.Replace(sourcefilename, regex, replace);
			}
			
			var resultfilename = Path.Combine(outputdir, outfilename);
			Directory.CreateDirectory(Path.GetDirectoryName(resultfilename));			
			return resultfilename;
		}

		private static string ConvertFile(XmlIncludeProcessor includer, string source, BxlParserOptions options,
		                                  ISerializer serializer) {
			var xmlresult = includer.Load(source, true, options);
			var sw = new StringWriter();
			serializer.Serialize("sourcefile", xmlresult, sw);
			var serializedresult = sw.ToString();
			return serializedresult;
		}

		private static string CreateComment(BxlParserOutputFormat outputFormat, string comment) {
			switch (outputFormat) {
					case BxlParserOutputFormat.Bxl:
						return "# " + comment;
					case BxlParserOutputFormat.Xml:
						return "<!-- " + comment + " -->";
					case BxlParserOutputFormat.Json:
						return "/* " + comment + "*/";
					default:
						return "// " + comment;
			}
		}

		private static ISerializer GetSerializer(BxlParserConsoleArgs consoleargs) {
			ISerializer serializer = null;
			switch (consoleargs.OutputFormat) {
				case BxlParserOutputFormat.Bxl:
					serializer = new BxlSerializer();
					break;
				case BxlParserOutputFormat.Xml:
					serializer = new XmlSerializer();
					break;
				case BxlParserOutputFormat.Json:
					serializer = new JsonSerializer();
					break;
			}
			return serializer;
		}

		private static void EnlistSourcesIfNeeded(BxlParserConsoleArgs consoleargs, ConsoleRenderHelper render, string[] sources) {
			if (consoleargs.EnlistSources) {
				render.Render("Source files used:", ConsoleColor.Cyan);
				foreach (var source in sources) {
					render.Render(source.NormalizePath().Replace(Environment.CurrentDirectory.NormalizePath(), "."), ConsoleColor.Yellow);
				}
			}
		}

		/// <summary>
		/// 	Выводит информацию о параметрах вызова в консоль
		/// </summary>
		/// <exception cref="NotImplementedException"></exception>
		public static void OutputHelp() {
			new ConsoleRenderHelper().Render(GetHelpText());
		}

		/// <summary>
		/// 	Возвращает информацию о параметрах вызова
		/// </summary>
		/// <returns> </returns>
		public static string GetHelpText() {
			string helptext;
			var assembly = typeof (Program).Assembly;
			var resourcename = assembly.GetManifestResourceNames().First(x => x.EndsWith(".help.txt"));

			using (var s = assembly.GetManifestResourceStream(resourcename)) {
				var sr = new StreamReader(s);
				helptext = sr.ReadToEnd();
			}
			return helptext;
		}
	}
}