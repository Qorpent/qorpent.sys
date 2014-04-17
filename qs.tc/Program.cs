using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;

namespace qs.tc
{
	static class Program
	{
		static int Main(string[] args){
			var dict = new ConsoleArgumentHelper().ParseDictionary(args);

			var outstring = dict.SafeGet("out", "hello");
			var errorstring = dict.SafeGet("error", "");
			var state = dict.SafeGet("state", 0);
			var throwex = dict.SafeGet("throwex", false);
			var doreadln = dict.SafeGet("doreadln", false);
			var timeout = dict.SafeGet("timeout", 0);
			var interactive = dict.SafeGet("interactive", false);
			var passread = dict.SafeGet("passread", false);
			var debug = dict.SafeGet("debug", false);
			if (debug){
				Debugger.Launch();
			}

			if (!string.IsNullOrWhiteSpace(outstring)){
				Console.Out.WriteLine(outstring);
			}
			if (doreadln){
				Console.WriteLine(Console.ReadLine());
			}

			if (interactive){
				Console.WriteLine("a");
				var fst = Console.ReadLine();
				Console.WriteLine("b");
				var sec = Console.ReadLine();
				Console.WriteLine(fst.ToInt()+sec.ToInt());
			}

			if (passread){
				Console.Write("Enter login: ");
				var user = Console.ReadLine();
				var pass = new ConsoleArgumentHelper().ReadLineSafety("Password: ");
				Console.WriteLine("/{0}:{1}/",user,pass);

			}

			if (!string.IsNullOrWhiteSpace(errorstring)){
				Console.Error.WriteLine(errorstring);	
			}
			if (timeout!=0){
				Thread.Sleep(timeout*1000);
			}
			if (throwex){
				throw new Exception("some error");
			}

			return state;
		}
	}
}
