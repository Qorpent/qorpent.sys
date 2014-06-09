using System;
using System.Linq;
using NUnit.Framework;
using Qorpent.BSharp;

namespace Qorpent.Serialization.Tests.BSharp.Preprocessor{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class PreprocessorLoadingAndUsingTest{
		[Test]
		public void BugPreprocessorsAreNotDetectedAsClasses(){
			var code = @"
require preprocessor
using 
	preprocess=Qorpent.BSharp.PreprocessScript
preprocess a b";
			var ctx = BSharpCompiler.Compile(code);
			foreach (var error in ctx.GetErrors(ErrorLevel.Error))
			{
				Console.WriteLine(error.ToLogString());
			}
			Assert.AreEqual(0,ctx.GetErrors(ErrorLevel.Error).Count());
		}
	}
}