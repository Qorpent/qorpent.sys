using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.BSharp;
using Qorpent.Utils.Extensions;

namespace Qorpent.Core.Tests.BSharp
{
	[TestFixture]
	public class ProjectDescriptorTest
	{
		private string dir;

		[SetUp]
		public void Setup(){
			dir = Path.GetTempFileName();
			File.Delete(dir);
			Directory.CreateDirectory(dir);

		}
		[TearDown]
		public void TearDown(){
			try{
				Directory.Delete(dir, true);
			}
			catch{
				
			}
		}
		[Test]
		public void CanAutomaticallyDetectRepositoryAndDirectory(){
			var projdir = Path.Combine(dir, "root/rep/dir/proj").NormalizePath();
			var gitdir = Path.Combine(dir, "root/rep/.git").NormalizePath();
			var repodir = Path.Combine(dir, "root/rep").NormalizePath();
			Directory.CreateDirectory(projdir);
			Directory.CreateDirectory(gitdir);
			var projdesc = new BSharpProjectDescriptor();
			projdesc.AutoSetup(projdir);
			Assert.AreEqual(repodir, projdesc.RepositoryDirectory.NormalizePath());
			Assert.AreEqual("./dir/proj/", projdesc.ProjectDirectory.NormalizePath());
		}
	}
}
