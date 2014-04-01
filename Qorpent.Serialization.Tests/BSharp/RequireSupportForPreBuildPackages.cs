using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.BSharp;
using Qorpent.Bxl;

namespace Qorpent.Serialization.Tests.BSharp
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class RequireSupportForPreBuildPackages
	{
		[Test]
		public void CanUseDataPackage(){
			var code = @"
require data
class mytable1
	import Qorpent.Db.TableBase
";

			var result = BSharpCompiler.CreateDefault().Compile(new[] { new BxlParser().Parse(code) }); 
			var cls = result.Get("mytable1");
			Assert.NotNull(cls);
			Assert.GreaterOrEqual(cls.Compiled.Elements("datatype").Count(),5);
			Console.WriteLine(cls.Compiled);

		}
	}
}
