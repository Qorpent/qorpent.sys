using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.Bxl;
using Qorpent.Utils;

namespace bxl
{
	class Program{
		private static string filename = "";
		private static BxlParserOptions opts   = BxlParserOptions.None;
		private static bool useoutfile = false;
		static int Main(string[] args){
			var argsdict = new ConsoleArgumentHelper().ParseDictionary(args);
			if (argsdict.ContainsKey("arg1")){
				filename = argsdict["arg1"];
			}
			if (argsdict.ContainsKey("nolex")){
				opts|=BxlParserOptions.NoLexData;
			}
			if (argsdict.ContainsKey("tofile")){
				useoutfile = true;
			}
			if (string.IsNullOrWhiteSpace(filename)){
				useoutfile = true;
			}
			string[] files;
			if (!string.IsNullOrWhiteSpace(filename)){
				files = new[]{filename};
			}
			else{
				files = Directory.GetFiles(Environment.CurrentDirectory, "*.bxl", SearchOption.AllDirectories);
			}
			foreach (var file in files){
				Execute(file);
			}
			return 0;
		}

		private static void Execute(string file){
			var parser = new BxlParser();
			try{
				var xml = parser.Parse(File.ReadAllText(file), file, opts);
				if (useoutfile)
				{
					Console.WriteLine("File: "+file+" processed");
					File.WriteAllText(file + ".xml", xml.ToString());
				}
				else{
					Console.WriteLine("<!-- FILE : "+file+"-->");
					Console.WriteLine(xml.ToString());
				}
			}
			catch (Exception ex){
				var er = new XElement("error", ex.ToString());
				if (useoutfile)
				{
				
					File.WriteAllText(file + ".error.xml", er.ToString());
				}
				
					Console.ForegroundColor = ConsoleColor.Red;
					
					Console.WriteLine("<!-- FILE : " + file + "-->");
					Console.WriteLine(er.ToString());
					Console.ResetColor();

				
			}
		}
	}
}
