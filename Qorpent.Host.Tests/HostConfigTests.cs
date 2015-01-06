using NUnit.Framework;
using Qorpent.BSharp;

namespace Qorpent.Host.Tests {
	[TestFixture]
	public class HostConfigTests {
		[Test]
		public void CanCheckoutConfigUsingMachineNameToResolve() {
			const string code = @"
class local
     connection a y

class remote
     machine t use=^local
     connection b z
";
			var ctx = BSharpCompiler.Compile(code);
			var local = ctx["local"].Compiled;
			var remote = ctx["remote"].Compiled;
			var config = new HostConfig(remote, ctx, "t");
			Assert.AreSame(local, config.Definition);
		}
		[Test]
		public void CanCheckoutConfigUsingMachineNameToResolveWithNotCondition() {
			const string code = @"
class local
     connection a y

class remote
     machine not t use=^local
     connection b z
";
			var ctx = BSharpCompiler.Compile(code);
			var local = ctx["local"].Compiled;
			var remote = ctx["remote"].Compiled;
			var config = new HostConfig(remote, ctx, "z");
			Assert.AreSame(local, config.Definition);
		}
	}
}
