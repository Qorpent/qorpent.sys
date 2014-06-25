using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qorpent.BSharp;

namespace bstimetest
{
	class Program
	{
		static void Main(string[] args){
			var times = 1000;
			if (args.Length != 0){
				times = Convert.ToInt32(args[0]);
				
			}
			for (var i = 0; i < times; i++){
				Console.Write(i);
				BSharpCompiler.CompileDirectory(@"C:\z3projects\assoi\local\Draft\report\content\charts");
				Console.Write(",");
			}
			Console.WriteLine();
		}
	}
}
