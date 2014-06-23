using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.IO.Syncronization;
using Qorpent.Log;

namespace Qorpent.IO.Tests
{
	[TestFixture]
	[Explicit]
	public class SynchronizeWellKnownDirectory
	{
		[Explicit]
		[Test]
		public void SyncThemasDiff(){
			var sync = new DirectorySynchronization(@"C:\z3projects\assoi\local\Draft\themas\.old\thema", @"C:\mnt\testthemas");
			foreach (var fileItem in sync.GetDifference()){
				Console.WriteLine(fileItem);
			}
		}


		[Explicit]
		[Test]
		public void SyncThemas()
		{
			var sync = new DirectorySynchronization(@"C:\z3projects\assoi\local\Draft\themas\.old\thema", @"C:\mnt\testthemas"){Log=ConsoleLogWriter.CreateLog("main",customFormat:"${Message}")};
			sync.Synchronize();
		}
	}
}
