using System.IO;
using System.Threading;
using NUnit.Framework;
using Qorpent.Log;

namespace Qorpent.Utils.Tests.ConsoleApplicationSupport{
    [TestFixture]
	public class ConsoleDefaultApplicationBehavior{
		private string dir;

		[Test]
		public void CanSetupLogLevel(){
			var parameters = new ConsoleApplicationParameters();
			parameters.Initialize("--log-level","Error");
			Assert.AreEqual(LogLevel.Error,parameters.Log.Level);
		}
		[SetUp]
		public void Setup(){
			dir = Path.GetTempFileName(); 
			if(File.Exists(dir)){
				File.Delete(dir);
			}
			if (Directory.Exists(dir)){
				Directory.Delete(dir,true);
			}
			Directory.CreateDirectory(dir);
		}
		[TearDown]
		public void TearDown(){
			if (Directory.Exists(dir))
			{
				Thread.Sleep(200);
				try{
					Directory.Delete(dir, true);
				}
				catch{
					
				}
			}
		}
		[Test]
		public void CanSetupWithBSharp(){
			File.WriteAllText(Path.Combine(dir,"test.bsconf"),@"
class x
	logformat='@{Message}'
	loglevel=Error
	x 3");
			var parameters = new ConsoleApplicationParameters();
			parameters.TreatAnonymousAsBSharpProjectReference = true;
			parameters.Initialize("x","--working-directory",dir,"--y","1");
			Assert.AreEqual(LogLevel.Error,parameters.Log.Level);
			Assert.AreEqual("3", parameters.Get("x", ""));
			Assert.AreEqual("${Message}",parameters.LogFormat);
			Assert.AreEqual("1",parameters.Get("y",""));
			
		}

	    [Test]
	    public void CanLoadConfigByFileName() {
	        var filename = Path.Combine(dir, "test.myconf");
            File.WriteAllText(filename, @"
class test
	logformat='@{Message}'
	loglevel=Error
	x 3");
            var parameters = new ConsoleApplicationParameters();
            parameters.TreatAnonymousAsBSharpProjectReference = true;
            parameters.Initialize(filename, "--y", "1");
            Assert.AreEqual(LogLevel.Error, parameters.Log.Level);
            Assert.AreEqual("3", parameters.Get("x", ""));
            Assert.AreEqual("${Message}", parameters.LogFormat);
            Assert.AreEqual("1", parameters.Get("y", ""));
        }
	}
}