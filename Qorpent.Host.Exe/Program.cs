using System;
using System.Diagnostics;
using System.Linq;
using Qorpent.Host.Server;
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
	        EnsureRequiredApplications(config);
	        config.DllFolder = EnvironmentInfo.ResolvePath("@repos@/.build/bin/all");
	        var hostServer = new HostServer(config);

            LogHostInfo(arg, config);
	        hostServer.Start();
	        try {
	            Console.ReadLine();
	            return 0;
	        }
	        finally {
	            hostServer.Stop();
	        }
	    }

	    private static void EnsureRequiredApplications(HostConfig config) {
	        var requires = config.Definition.Elements("require");
	        foreach (var require in requires) {
	            var name = require.IdCodeOrValue();
	            var shadow = EnvironmentInfo.GetShadowDirectory(name);
	            var processes = Process.GetProcessesByName("qh");
                Console.WriteLine(string.Join("; ",processes.Select(_=>_.ProcessName)));
	            var required =
	                processes.FirstOrDefault(_ => _.MainModule.FileName.NormalizePath().StartsWith(shadow.NormalizePath()));
	            if (null != required) {
	                config.Log.Info("Required '" + name + "' found, PID: " + required.Id);
	            }
	            else {
	                required = Process.Start(EnvironmentInfo.ResolvePath("@repos@/.build/bin/all/qh.exe"), name);
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
	    }
	}
}
