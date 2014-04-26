using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.Bxl;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;

namespace Qorpent.Host
{
	internal static class Program
	{
		public static void Main(string[] args)
		{
			var cfg = PrepareHostConfig(args);
			var srv = new HostServer(cfg);
			Console.WriteLine("Конфигурация");
			Console.WriteLine(cfg);
			
			srv.Start();
			var cmd = "";
			while ((cmd = Console.ReadLine()) != "exit")
			{
				if (cmd == "stat")
				{
					Console.WriteLine(srv.GetStatisticsString());
				}
			}
			srv.Stop();

		}

		private static HostConfig PrepareHostConfig(string[] args)
		{
			var cfg = new HostConfig();
			cfg.ApplicationMode = HostApplicationMode.Standalone;
			var argdicts = new ConsoleArgumentHelper().ParseDictionary(args);
			if (argdicts.ContainsKey("config"))
			{
				var configFile = argdicts["config"];
				XElement configXml = null;
				if (configFile.EndsWith(".xml"))
				{
					configXml = XElement.Load(configFile);
				}
				else
				{
					configXml = new BxlParser().Parse(File.ReadAllText(configFile), configFile);
				}
				cfg.LoadXmlConfig(configXml);
			}
			if (argdicts.ContainsKey("root"))
			{
				cfg.RootFolder = Path.GetFullPath(argdicts["root"]);
			}
			if (cfg.Bindings.Count == 0)
			{
				cfg.AddDefaultBinding();
			}

			if (argdicts.ContainsKey("port")){
				cfg.Bindings[0].Port = argdicts["port"].ToInt();

			}

			if (argdicts.ContainsKey("content")){
				var folders = argdicts["content"].SmartSplit(false, true, ';');
				foreach (var folder in folders){
					cfg.ContentFolders.Add(folder);	
				}

				
			}

			if (argdicts.ContainsKey("appname"))
			{
				foreach (var hostBinding in cfg.Bindings)
				{
					if (hostBinding.AppName == "/")
					{
						hostBinding.AppName = argdicts["appname"];
					}
				}
			}
			return cfg;
		}
	}
}
