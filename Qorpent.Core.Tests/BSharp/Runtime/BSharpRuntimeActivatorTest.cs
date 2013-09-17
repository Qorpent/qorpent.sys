using NUnit.Framework;
using Qorpent.BSharp.Runtime;
using Qorpent.IoC;

namespace Qorpent.Core.Tests.BSharp.Runtime {
	[TestFixture]
	public class BSharpRuntimeActivatorTest : BSharpRuntimeTestBase
	{
		[Test]
		public void CanActivateWithDirectTypeNoBound() {
			var cls = CreateCls<A>();
			var act = new BSharpRuntimeActivatorService();
			Assert.True(act.CanActivate<A>(cls,BSharpActivationType.Configured));
			Assert.True(act.CanActivate<A>(cls));
			//A не поддерживает биндинга
			Assert.False(act.CanActivate<A>(cls,BSharpActivationType.Client));
		}
		[Test]
		public void CanActivateConfiguredWithDirectTypeBound()
		{
			var cls = CreateCls<B>();
			var act = new BSharpRuntimeActivatorService();
			Assert.True(act.CanActivate<B>(cls, BSharpActivationType.Configured));
			Assert.True(act.CanActivate<B>(cls));
			Assert.True(act.CanActivate<B>(cls, BSharpActivationType.Client));
		}

		[Test]
		public void CanActivateConfiguredWithInterfaceNoBound()
		{
			var cls = CreateCls<A>();
			var act = new BSharpRuntimeActivatorService();
			Assert.True(act.CanActivate<I>(cls, BSharpActivationType.Configured));
			Assert.True(act.CanActivate<I>(cls));
			//клиентские интерфейсы могут разрешаться только через контейнер
			Assert.False(act.CanActivate<I>(cls, BSharpActivationType.Client));
		}

		[Test]
		public void CanActivateWithInterfaceClientIfContainer() {
			var c = new Container();
			c.Register(new ComponentDefinition<I,B>());
			var cls = CreateCls<A>();
			var act = new BSharpRuntimeActivatorService();
			act.SetContainerContext(c,null);
			Assert.True(act.CanActivate<I>(cls, BSharpActivationType.Client));
			Assert.IsInstanceOf<A>(act.Activate<I>(cls,BSharpActivationType.Configured));
			Assert.IsInstanceOf<B>(act.Activate<I>(cls,BSharpActivationType.Client));
			var b = (B)act.Activate<I>(cls, BSharpActivationType.Client);
			Assert.AreEqual(cls,b.Cls);
		}

		[Test]
		public void CanActivateWithClassClient()
		{
			var cls = CreateCls<A>();
			var act = new BSharpRuntimeActivatorService();
			Assert.False(act.CanActivate<A>(cls, BSharpActivationType.Client));
			Assert.True(act.CanActivate<A>(cls, BSharpActivationType.Configured));
			Assert.True(act.CanActivate<B>(cls, BSharpActivationType.Client));
			Assert.False(act.CanActivate<B>(cls, BSharpActivationType.Configured));
			Assert.IsInstanceOf<A>(act.Activate<object>(cls));
			Assert.IsInstanceOf<A>(act.Activate<A>(cls));
			Assert.Null(act.Activate<B>(cls,BSharpActivationType.Configured));
			Assert.NotNull(act.Activate<B>(cls,BSharpActivationType.Client));
			//automode
			Assert.NotNull(act.Activate<B>(cls));
			var b = act.Activate<B>(cls);
			Assert.AreEqual(cls,b.Cls);

		}

		[Test]
		public void CanActivateConfiguredWithInterfaceBound()
		{
			var cls = CreateCls<B>();
			var act = new BSharpRuntimeActivatorService();
			Assert.True(act.CanActivate<I>(cls, BSharpActivationType.Configured));
			Assert.True(act.CanActivate<I>(cls));
			//клиентские интерфейсы могут разрешаться только через контейнер
			Assert.False(act.CanActivate<I>(cls, BSharpActivationType.Client));
		}

	}
}