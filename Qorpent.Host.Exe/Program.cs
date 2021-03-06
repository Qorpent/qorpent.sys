﻿using System;
using System.Diagnostics;
using System.Linq;
using qorpent.v2.console;
using Qorpent.Host.Server;
using Qorpent.Log;
using Qorpent.Log.NewLog;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;

namespace Qorpent.Host.Exe
{
	internal static class Program
	{
		public static int Main(string[] args) {
		    if (args.Contains("--debug")) {
		        Debugger.Launch();
		    }
		    return ConsoleApplication.Execute<ServerParameters>(args, Execute, true);
		}

	    private static int Execute(ServerParameters arg) {
	        var config = arg.BuildServerConfig();
	        EnsureRequiredApplications(arg,config);
	        config.DllFolder = EnvironmentInfo.ResolvePath("@repos@/.build/bin/all");
            var command = arg.Get("command", "");
	        if (string.IsNullOrWhiteSpace(command)) {
	            Console.WriteLine("LOGLEVEL " + config.LogLevel);
	        }
	        var hostServer = new HostServer(config);
            hostServer.Initialize();
            var consoleContext = new ConsoleContext
            {
                Container = hostServer.Container,
                Parameters = arg
            };
            var listener = new ConsoleListener(consoleContext);
            
	        if (!string.IsNullOrWhiteSpace(command)) {
	            var task = listener.Call(command);
	            task.Wait();
	        }
	        else {

	            LogHostInfo(arg, config);
	            hostServer.Start();
	            Console.WriteLine("LOGLEVEL " + config.LogLevel);

	            listener.Log = hostServer.Container.Get<ILoggyManager>().Get("console");
	            listener.Run();
	            hostServer.Stop();
	        }
	        return 0;
	    }

	    private static void EnsureRequiredApplications(ServerParameters serverParameters, HostConfig config) {
	        var requires = config.Definition.Elements("require");
	        foreach (var require in requires) {
                if(!string.IsNullOrWhiteSpace(require.Attr("server")))continue; //external service
	            var name = require.IdCodeOrValue()+require.Attr("suffix");
	            var shadow = EnvironmentInfo.GetShadowDirectory(name);
	            var processes = Process.GetProcessesByName("qh");
                Console.WriteLine(string.Join("; ",processes.Select(_=>_.ProcessName)));
	            var required =
	                processes.FirstOrDefault(_ => _.MainModule.FileName.NormalizePath().StartsWith(shadow.NormalizePath()));
	            if (null != required) {
	                config.Log.Info("Required '" + name + "' found, PID: " + required.Id);
	            }
	            else {
	                var args = name;
	                if (serverParameters.Get("hidden", false)) {
	                    args += " --hidden";
	                }
	                required = Process.Start(EnvironmentInfo.ResolvePath("@repos@/.build/bin/all/qh.exe"), args);
                    config.Log.Info("Required '" + name + "' started, PID: " + required.Id);
	            }
	        }
	    }

	    private static void LogHostInfo(ServerParameters arg, HostConfig config) {
            Console.WriteLine("BinRoot: "+config.DllFolder);
	        foreach (var assembly in config.AutoconfigureAssemblies) {
                arg.Log.Trace("Lib: " + assembly);
	        }
	        foreach (var hostBinding in config.Bindings) {
                arg.Log.Info("Binding: " + hostBinding);
	        }
	        arg.Log.Trace("RootFolder: " + config.RootFolder);
	        foreach (var contentFolder in config.ContentFolders) {
	            arg.Log.Trace("ContentFolder: " + contentFolder);
	        }
	        foreach (var map in config.StaticContentMap) {
	            arg.Log.Trace("Map: "+map.Key+" : "+map.Value);
	        }
            foreach (var map in config.Proxize)
            {
                arg.Log.Trace("Proxize: " + map.Key + " : " + map.Value);
            }
	        foreach (var e in config.StaticContentCacheMap) {
	            arg.Log.Trace(e.Value.ToString());
	        }
	    }
	}
}
