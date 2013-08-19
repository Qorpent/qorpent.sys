using NUnit.Framework;
using Qorpent.BSharp.Runtime;
using Qorpent.IoC;

namespace Qorpent.Core.Tests.BSharp.Runtime
{
	[TestFixture]
	public class RuntimeClassDescriptorTests
	{
		public class A:I {}
		public class B:I {}
		public interface I {}
		[Test]
		public void CanSetupWithUsualClass() {
			var desc = new RuntimeClassDescriptor(typeof (A).AssemblyQualifiedName, null);
			Assert.AreEqual(RuntimeClassResolutionType.Resolved,desc.ResolutionType);
			Assert.AreEqual(typeof(A),desc.ResolvedType);
		}
		[Test]
		public void CanSetupNotDefinedState()
		{
			var desc = new RuntimeClassDescriptor(null, null);
			Assert.AreEqual(RuntimeClassResolutionType.NotDefined, desc.ResolutionType);
		}

		[Test]
		public void CanSetupNotResolvedState()
		{
			var desc = new RuntimeClassDescriptor("NOT_"+typeof(A).AssemblyQualifiedName, null);
			Assert.AreEqual(RuntimeClassResolutionType.NotResolved, desc.ResolutionType);
		}

		[Test]
		public void CanSetupContainerService()
		{
			var desc = new RuntimeClassDescriptor(typeof(I).AssemblyQualifiedName, null);
			Assert.AreEqual(RuntimeClassResolutionType.ContainerService, desc.ResolutionType);
			Assert.AreEqual(typeof(I), desc.ResolvedType);
		}

		[Test]
		public void NoErrorIfContainerIsNull()
		{
			var desc = new RuntimeClassDescriptor(typeof(I).AssemblyQualifiedName, null);
			Assert.AreEqual(RuntimeClassResolutionType.ContainerService, desc.ResolutionType);
			Assert.Null(desc.GetActualType());
			var desc2 = new RuntimeClassDescriptor("my.component", null);
			Assert.AreEqual(RuntimeClassResolutionType.ContainerName, desc2.ResolutionType);
			Assert.Null(desc2.GetActualType());

		}
		
		[Test]
		public void CanCreateInstanceFromContainerWithService() {
			var c = new Container();
			c.Register(new ComponentDefinition<I,A>());
			var desc = new RuntimeClassDescriptor(typeof(I).AssemblyQualifiedName, c);
			Assert.AreEqual(typeof(A),desc.GetActualType());
			Assert.IsInstanceOf<A>(desc.Create());
		}

		[Test]
		public void ActualTypeWithServiceIsSynchronizedWithContainer()
		{
			var c = new Container();
			c.Register(new ComponentDefinition<I, A>());
			var desc = new RuntimeClassDescriptor(typeof(I).AssemblyQualifiedName, c);
			Assert.AreEqual(typeof(A), desc.GetActualType());
			Assert.IsInstanceOf<A>(desc.Create());
			c.Register(new ComponentDefinition<I, B>());
			Assert.AreEqual(typeof(B), desc.GetActualType());
			Assert.IsInstanceOf<B>(desc.Create());
		}

		[Test]
		public void CanCreateInstanceFromContainerWithName()
		{
			var c = new Container();
			c.Register(new ComponentDefinition<I, A>(name:"my.c"));
			var desc = new RuntimeClassDescriptor("my.c", c);
			Assert.AreEqual(typeof(A), desc.GetActualType());
			Assert.IsInstanceOf<A>(desc.Create());
		}

		[Test]
		public void ActualTypeWithNameIsSynchronizedWithContainer()
		{
			var c = new Container();
			c.Register(new ComponentDefinition<I, A>(name:"my.c"));
			var desc = new RuntimeClassDescriptor("my.c", c);
			Assert.AreEqual(typeof(A), desc.GetActualType());
			Assert.IsInstanceOf<A>(desc.Create());
			c.Register(new ComponentDefinition<I, B>(name:"my.c"));
			Assert.AreEqual(typeof(B), desc.GetActualType());
			Assert.IsInstanceOf<B>(desc.Create());
		}

		[Test]
		public void CanSetupContainerName()
		{
			var desc = new RuntimeClassDescriptor("my.component", null);
			Assert.AreEqual(RuntimeClassResolutionType.ContainerName, desc.ResolutionType);
			Assert.AreEqual("my.component", desc.SourceClassName);
		}

		[Test]
		public void CanActivateDirectType() {
			var desc = new RuntimeClassDescriptor(typeof(A).AssemblyQualifiedName, null);
			var result = desc.Create();
			Assert.IsInstanceOf<A>(result);
		}
	}
}
