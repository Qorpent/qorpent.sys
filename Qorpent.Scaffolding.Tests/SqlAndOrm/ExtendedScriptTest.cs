using System.IO;
using System.Linq;
using NUnit.Framework;
using Qorpent.Scaffolding.Model;

namespace Qorpent.Scaffolding.Tests.SqlAndOrm{
	[TestFixture]
	public class ExtendedScriptTest{
		[Test]
		public void CanReadSimpleScript(){
			var model = PersistentModel.Compile(@"class X prototype=dbscript : --echo");
			var script = model.ExtendedScripts.First(_=>_.Name=="X");
			Assert.AreEqual("X",script.Name);
			Assert.AreEqual(SqlDialect.Ansi,script.SqlDialect);
			Assert.AreEqual(ScriptMode.Create,script.Mode);
			Assert.AreEqual(ScriptPosition.After,script.Position);
			Assert.AreEqual("--echo",script.Text);
		}

		[Test]
		public void CanReadScriptWithParameters()
		{
			var model = PersistentModel.Compile(@"class X prototype=dbscript position=before dialect=postgres mode=drop : '--echo 2'");
			var script = model.ExtendedScripts.First(_ => _.Name == "X");
			Assert.AreEqual("X", script.Name);
			Assert.AreEqual(SqlDialect.PostGres, script.SqlDialect);
			Assert.AreEqual(ScriptMode.Drop, script.Mode);
			Assert.AreEqual(ScriptPosition.Before, script.Position);
			Assert.AreEqual("--echo 2", script.Text);
		}

		[Test]
		public void CanReadScriptWithSubscripts()
		{
			var model = PersistentModel.Compile(@"
class X prototype=dbscript 
	script a position=before dialect=postgres mode=drop : '--d p'
	script b position=after dialect=sqlserver mode=create : '--c s'
");
			var a = model.GetScripts(SqlDialect.PostGres, ScriptMode.Drop, ScriptPosition.Before).Where(_=>!_.Name.StartsWith("sys:")).ToArray();
			Assert.AreEqual(1,a.Length);
			Assert.AreEqual("a",a[0].Name);
			var b = model.GetScripts(SqlDialect.SqlServer, ScriptMode.Create, ScriptPosition.After).Where(_=>!_.Name.StartsWith("sys:")).ToArray();
			Assert.AreEqual(1, b.Length);
			Assert.AreEqual("b", b[0].Name);
		}

		[Test]
		public void CanReadExternal()
		{
			File.WriteAllText("CanReadExternal.sql","--echo x");
			var model = PersistentModel.Compile(@"class X prototype=dbscript external=CanReadExternal.sql");
			var script = model.ExtendedScripts.First(_ => _.Name == "X");
			Assert.AreEqual("--echo x", script.Text);
		}
	}
}