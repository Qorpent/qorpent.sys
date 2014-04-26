using System.Diagnostics;
using System.IO;
using System.Linq;
using Qorpent.BSharp;
using Qorpent.BSharp.Builder;
using Qorpent.Log;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Qorpent.Integration.BSharp.Builder.Exe {
    class Program {
        static int Main(string[] args) {
	        try{
		        if (null != args && 0 != args.Length){
			        var wdir = Array.IndexOf(args, "--workdir");
			        if (-1 != wdir){
				        Environment.CurrentDirectory = Path.GetFullPath(args[wdir + 1]);
			        }
		        }

		        var builder = new BSharpBuilder();
		        var adict = new ConsoleArgumentHelper().ParseDictionary(args);
		        if (adict.ContainsKey("debug")){
			        Debugger.Launch();
		        }
		        var log = SetupLog(adict);
		        var project = SetupProject(adict, log, builder);
		        builder.Log = log;
		        builder.Initialize(project);
		        var resultContext = builder.Build();
		        WriteOutErrors(resultContext, log);
		        return 0;
	        }
	        catch (Exception ex){
		        Console.Error.WriteLine(ex.ToString());
		        return -1;
	        }
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
			var log = ConsoleLogWriter.CreateLog("main", customFormat: "${Message}",level:level);
			log.Info("log level "+level);
		    
		    return log;
	    }

	    private static IBSharpProject SetupProject(IDictionary<string, string> adict, IUserLog log, BSharpBuilder builder) {
		    var project = new BSharpProject {IsFullyQualifiedProject = true};
		    project.IsFullyQualifiedProject = true;
            
            if (adict.ContainsKey("project")) {
	            project.ProjectName = adict["project"];
	            project.IsFullyQualifiedProject = false;
            }else if (adict.ContainsKey("arg1")) {
				project.ProjectName = adict["arg1"];
				project.IsFullyQualifiedProject = false;
            }


            if (!project.OutputAttributes.HasFlag(BSharpBuilderOutputAttributes.IncludeWork)) {
                if (!adict.ContainsKey("noIncludeWork")) {
                    project.OutputAttributes |= BSharpBuilderOutputAttributes.IncludeWork;
                }
            }

            if (adict.ContainsKey("out-layout")) {
                project.OutputAttributes = adict["out-layout"].To<BSharpBuilderOutputAttributes>();
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

	    private static void WriteOutErrors(IBSharpContext resultContext, IUserLog log) {
		    foreach (var e in resultContext.GetErrors()) {
			    var el = LogLevel.Error;
			    if (e.Level == ErrorLevel.Warning) {
				    el = LogLevel.Warning;
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