using System;
using System.Diagnostics;
using Qorpent.Host.Server;
using Qorpent.Utils;

namespace Qorpent.Host.Exe
{
	internal static class Program
	{
		public static int Main(string[] args) {
		    return ConsoleApplication.Execute<ServerParameters>(args, Execute, true);
		}

	    private static int Execute(ServerParameters arg) {
	        var config = arg.BuildServerConfig();
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

	    private static void LogHostInfo(ServerParameters arg, HostConfig config) {
            Console.WriteLine("BinRoot: "+config.DllFolder);
	        foreach (var assembly in config.AutoconfigureAssemblies) {
	            Console.WriteLine("Lib: "+assembly);
	        }
	        foreach (var hostBinding in config.Bindings) {
	            Console.WriteLine("Binding: "+hostBinding);
	        }
	        arg.Log.Trace("RootFolder: " + config.RootFolder);
	        foreach (var contentFolder in config.ContentFolders) {
	            arg.Log.Trace("ContentFolder: " + contentFolder);
	        }
	        foreach (var map in config.StaticContentMap) {
	            arg.Log.Trace("Map: "+map.Key+" : "+map.Value);
	        }
	    }
	}
}
