using System.Linq;
using NUnit.Framework;
using Qorpent.BSharp.Runtime;
using Qorpent.IoC;

namespace Qorpent.Core.Tests.BSharp.Runtime {
	[TestFixture]
	public class BSharpRuntimeServiceTest : BSharpRuntimeTestBase {
		[Test]
		public void CanInstantiate() {
			var a = new BSharpRuntimeActivatorService();
			var p = new BSharpGenericClassProvider();
			p.Set(CreateCls<A>());
			p.Set(CreateCls<B>());
			var rs = new BSharpRuntimeService();
			rs.Activators = new[] {a};
			rs.Providers = new[] {p};

			Assert.NotNull(rs.Activate<A>("my.test.A"));
			Assert.NotNull(rs.Activate<A>(rs.GetClassNames("*.A").First()));
			Assert.NotNull(rs.Activate<B>(rs.GetClassNames("*.B").First()));
			Assert.NotNull(rs.Activate<B>("my.test.A"));
			Assert.NotNull(rs.Activate<I>("my.test.A"));
			Assert.NotNull(rs.Activate<B>("my.test.A",BSharpActivationType.Client));
			
		}

		[Test]
		public void WorksWithIoc() {
			var c = new Container();
			var p = new BSharpGenericClassProvider();
			p.Set(CreateCls<A>());
			p.Set(CreateCls<B>());
			c.Register(
				new ComponentDefinition<
					IBSharpRuntimeProvider,BSharpGenericClassProvider>(
					Lifestyle.Extension, "sample.bs.p",implementation:p
					));
			c.Register(new ComponentDefinition<IBSharpRuntimeService, BSharpRuntimeService>());
			c.Register(new ComponentDefinition<IBSharpRuntimeActivatorService, BSharpRuntimeActivatorService>());
			c.RegisterSubResolver(new IoC.BSharp.BSharpTypeResolver(c));

			Assert.NotNull(c.Get<A>("bsharp://my.test.A"));
			Assert.NotNull(c.Get<B>("bsharp://my.test.B"));
			Assert.NotNull(c.Get<I>("bsharp://my.test.A"));
			Assert.NotNull(c.Get<I>("bsharp://my.test.B"));
			Assert.NotNull(c.Get<B>("bsharp://my.test.B"));
		}
	}
}