﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.BSharp.Builder;
using Qorpent.Experiments;
using Qorpent.Log;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;

namespace Qorpent.Integration.BSharp.Builder.Exe {
    class Program {
        private static bool ConsoleMode;
        static int Main(string[] args) {
            Console.OutputEncoding = Encoding.UTF8;
	        try{
		        if (null != args && 0 != args.Length){
			        var wdir = Array.IndexOf(args, "--workdir");
			        if (-1 != wdir){
				        Environment.CurrentDirectory = Path.GetFullPath(args[wdir + 1]);
			        }
		        }
	            ConsoleMode = args.Contains("--console-mode");
		        var builder = new BSharpBuilder();
		        var adict = new ConsoleArgumentHelper().ParseDictionary(args);
		        if (adict.ContainsKey("debug")){
			        Debugger.Launch();
		        }
		        var log = SetupLog(adict);
	            bool watch = args.Contains("--watch");

                var project = DoBuild(adict, log, builder,watch);
	            if (watch)
	            {
                    
	                var laststamp = GetStamp(project);
                    Console.WriteLine("----------------------------------------------");
                    while (true)
	                {
                        Thread.Sleep(2000);
	                    var newstamp = GetStamp(project);
	                    if (newstamp > laststamp)
	                    {
	                        project = DoBuild(adict, log, new BSharpBuilder(),true);
	                        laststamp = newstamp;
                            Console.WriteLine("----------------------------------------------");
	                    }
	                }
	            }
	            return 0;
	        }
	        catch (Exception ex){
	            if (ConsoleMode) {
                    Console.Error.WriteLine(new XElement("generic-error",ex.ToString()).ToString());
	            }
	            else {
	                Console.Error.WriteLine(ex.ToString());
	            }
	            return -1;
	        }
        }

        private static DateTime GetStamp(IBSharpProject project)
        {
            project = project.Get<IBSharpProject>("_real_project") ?? project;
            var dirs = project.GetSourceDirectories().ToArray();
            var stamp = dirs.SelectMany(_ => Directory.GetFiles(_, "*.bxls",SearchOption.AllDirectories)).Max(_ => File.GetLastWriteTime(_));
            var csharp =
                Directory.GetFiles(project.GetCompileDirectory(), "*.cs", SearchOption.AllDirectories)
                    .Where(_ => !_.Contains("\\obj\\")).ToArray();
            if (0 != csharp.Length) {
               var _s = csharp.Max(_ => File.GetLastWriteTime(_));
                if (_s > stamp) stamp = _s;
            }
            var xslt =
                Directory.GetFiles(project.GetCompileDirectory(), "*.xslt", SearchOption.AllDirectories)
                    .Where(_ => !_.Contains("\\obj\\")).ToArray();
            if (0 != xslt.Length)
            {
                var _s = xslt.Max(_ => File.GetLastWriteTime(_));
                if (_s > stamp) stamp = _s;
            }
                    

            return stamp;
        }

        private static IBSharpProject DoBuild(IDictionary<string, string> adict, IUserLog log, BSharpBuilder builder, bool errorsafe = false)
        {
            
            var project = SetupProject(adict, log, builder);
            project.NoOutput = ConsoleMode;
            try
            {
                builder.Log = log;
                builder.Initialize(project);
                var resultContext = builder.Build();

                if (ConsoleMode)
                {
                    WriteOutConsoleMode(resultContext);
                }
                else
                {
                    WriteOutErrors(resultContext, log);
                }
            }
            catch (Exception e )
            {
                if (errorsafe)
                {
                    project.Log.Error("Error in cycle "+e,e);
                    return project;
                }
                throw;
            }
            
            return project;
        }

        private static void WriteOutConsoleMode(IBSharpContext ctx) {
            var result = new XElement("result");
            foreach (var cls in ctx.Get(BSharpContextDataType.Working)) {
                var clsElement = new XElement("cls");
                clsElement.SetAttr("code", cls.FullName);
                clsElement.SetAttr("name", cls.Name);
                clsElement.SetAttr("ns", cls.Namespace);
                clsElement.SetAttr("prototype", cls.Prototype);
                clsElement.Add(cls.Compiled);
                result.Add(clsElement);
            }
            foreach (var error in ctx.GetErrors()) {
                    var errorElement = new XElement("error", new XAttribute("type",error.Type)) {Value = error.ToLogString()};
                    if (null != error.LexInfo) {
                        var lex = new XElement("lexinfo", new XAttribute("file", error.LexInfo.File),
                            new XAttribute("line", error.LexInfo.Line));
                        errorElement.Add(lex);
                    }
                    result.Add(errorElement);
                
            }

            Console.WriteLine(result.ToString());
        }

        private static IUserLog SetupLog(IDictionary<string, string> adict) {
			var level = LogLevel.Info;
		    if (adict.ContainsKey("loglevel")) {
			    level = (LogLevel) Enum.Parse(typeof (LogLevel), adict["loglevel"], true);
		    }
			if (adict.ContainsKey("log-level"))
			{
				level = (LogLevel)Enum.Parse(typeof(LogLevel), adict["log-level"], true);
			}
            //disable logging for console-mode
	        if (adict.ContainsKey("console-mode")) {
	            level = LogLevel.Fatal;
	        }
			var log = ConsoleLogWriter.CreateLog("main", customFormat: "${Message}",level:level);
			log.Info("log level "+level);
		    
		    return log;
	    }

	    private static IBSharpProject SetupProject(IDictionary<string, string> adict, IUserLog log, BSharpBuilder builder) {
		    var project = new BSharpProject {IsFullyQualifiedProject = true};
		    project.IsFullyQualifiedProject = true;
	        var filename = adict.resolvestr("arg1");
	        if (!string.IsNullOrWhiteSpace(filename) && filename.Contains(".")) {
	                //direct file
	                log.Info("Single file compiled");
	                project = (BSharpProject) SingleFileProject(EnvironmentInfo.ResolvePath(filename), adict, log, builder);
	        }
	        else {
	            if (adict.ContainsKey("project")) {
	                project.ProjectName = adict["project"];
	                project.IsFullyQualifiedProject = false;
	            }
	            else if (adict.ContainsKey("arg1")) {
	                project.ProjectName = adict["arg1"];
	                project.IsFullyQualifiedProject = false;
	            }
	        }


	        if (!project.OutputAttributes.HasFlag(BSharpBuilderOutputAttributes.IncludeWork)) {
                if (!adict.ContainsKey("noIncludeWork")) {
                    project.OutputAttributes |= BSharpBuilderOutputAttributes.IncludeWork;
                }
            }

            if (adict.ContainsKey("out-layout")) {
                project.OutputAttributes = adict["out-layout"].To<BSharpBuilderOutputAttributes>();
            }

            

            if (adict.ContainsKey("compile-extensions"))
            {
                project.DoCompileExtensions = adict["compile-extensions"].ToBool();
            }

            if (adict.ContainsKey("out")) {
			    project.MainOutputDirectory = adict["out"];
		    }
            
            if (adict.ContainsKey("log")) {
                project.LogOutputDirectory = adict["log"];
            }
		    
            if (adict.ContainsKey("extension")) {
			    project.OutputExtension = adict["extension"];
		    }

		    if (adict.ContainsKey("root")) {
			    project.RootDirectory = adict["root"];
		    }
            if (adict.ContainsKey("dot"))
            {
                project.GenerateGraph = adict["dot"].ToBool();
            }

            if (adict.ContainsKey("include")) {
                project.Targets.Paths.RemoveTarget("*");
                var parsed = ParseKeyValueEnumeration(adict["include"], ',', ':');
                foreach (var el in parsed) {
                    Console.WriteLine("Include: <" +el.Key+ "," + el.Value+">");
                    WriteTarget(project, el.Key, el.Value, BSharpBuilderTargetType.Include);
                }
            }

            if (adict.ContainsKey("exclude")) {
                var parsed = ParseKeyValueEnumeration(adict["exclude"], ',', ':');
                foreach (var el in parsed) {
                    Console.WriteLine("Exclude: <" + el.Key + "," + el.Value + ">");
                    WriteTarget(project, el.Key, el.Value, BSharpBuilderTargetType.Exclude);
                }
            }

		    project.Log = log;
			foreach (var c in adict) {
				if (c.Key.StartsWith("set-")) {
					project.Conditions[c.Key.Substring(4)] = c.Value;
					log.Info("set option " + c.Key.Substring(4) + " = " + c.Value);
				}
			}

			log.Info("root dir = " + project.GetRootDirectory());
			log.Info("out dir = " + project.GetOutputDirectory());
			log.Info("log dir = " + project.GetLogDirectory());

		    return project;
	    }

        private static IBSharpProject SingleFileProject(string resolvePath, IDictionary<string, string> adict, IUserLog log, BSharpBuilder builder) {
            var result = new BSharpProject {IsFullyQualifiedProject = true, Log = log};
            result.RootDirectory = Path.GetDirectoryName(resolvePath);
            result.Targets.Paths[resolvePath] = BSharpBuilderTargetType.Include;
            if (adict.ContainsKey("cls")) {
               result.Targets.Classes[adict["cls"]] = BSharpBuilderTargetType.Include;
            }
            return result;
        }

        private static void WriteOutErrors(IBSharpContext resultContext, IUserLog log) {
		    foreach (var e in resultContext.GetErrors()) {
			    var el = LogLevel.Error;
			    if (e.Level == ErrorLevel.Warning) {
				    el = LogLevel.Warn;
			    }
			    else if (e.Level == ErrorLevel.Fatal) {
				    el = LogLevel.Fatal;
			    }
			    else if (e.Level == ErrorLevel.Hint) {
				    el = LogLevel.Info;
			    }
			    var m = e.ToLogString();
			    log.Write(el, m, e, e);
		    }
		    Thread.Sleep(200);
	    }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <param name="modifier"></param>
        /// <param name="target"></param>
        /// <param name="type"></param>
        private static void WriteTarget(BSharpProject project, string modifier, string target, BSharpBuilderTargetType type) {
            switch (modifier) {
                case "n": project.Targets.Namespaces.AppendTarget(target, type); break;
                case "c": project.Targets.Classes.AppendTarget(target, type); break;
                case "p": project.Targets.Paths.AppendTarget(target, type); break;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="separator"></param>
        /// <param name="delimeter"></param>
        /// <returns></returns>
        private static IEnumerable<KeyValuePair<string, string>> ParseKeyValueEnumeration(string source, char separator, char delimeter) {
            return source.Split(
                new[] {separator}
            ).Select(
                item => item.Split(new[] {delimeter})
            ).Select(
                el => new KeyValuePair<string, string>(el[0], el[1])
            ).ToList();
        }
    }
}