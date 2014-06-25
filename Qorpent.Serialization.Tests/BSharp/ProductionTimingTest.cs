using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.BSharp;

namespace Qorpent.Serialization.Tests.BSharp
{
	[TestFixture]
	[Explicit]
	public class ProductionTimingTest
	{
		/// <summary>
		/// 
		/// </summary>
		[TestCase(10)]
		[TestCase(20)]
		[TestCase(30)]
		[TestCase(40)]
		[TestCase(50)]
		//[TestCase(60)]
		//[TestCase(70)]
		//[TestCase(80)]
		//[TestCase(90)]
		//[TestCase(100)]
		public void ChartsCompileMultTimesTestForPerformanceControl(int times){
			Console.WriteLine(Process.GetCurrentProcess().Threads.Count);
			for (var i = 0; i < times; i++){
				Console.Write(i);
				BSharpCompiler.CompileDirectory(@"C:\z3projects\assoi\local\Draft\report\content\charts");
				Console.Write(":");
				Console.Write(Process.GetCurrentProcess().Threads.Count);
				Console.Write(", ");
			}
			
		
		}
	}
}
