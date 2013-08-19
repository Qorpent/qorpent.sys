using System.Xml.Linq;
using NUnit.Framework;
using Qorpent.BSharp.Runtime;
using Qorpent.IoC;

namespace Qorpent.Core.Tests.BSharp.Runtime {
	[TestFixture]
	public class BSharpRuntimeClassTest : BSharpRuntimeTestBase {


		[Test]
		public void CanSetupRuntimeClass() {
			var cls = CreateCls<A>();
			Assert.AreEqual(RuntimeClassResolutionType.Resolved, cls.RuntimeDescriptor.ResolutionType);
			Assert.AreEqual(typeof(A), cls.RuntimeDescriptor.ResolvedType);
			Assert.IsInstanceOf<A>(cls.Create());
		}

		[Test]
		public void CanSetupRuntimeClassWithContainerService() {
			var c = new Container();
			c.Register(new ComponentDefinition<I,A>());
			var cls = CreateCls<I>(c);
			Assert.IsInstanceOf<A>(cls.Create());
			c.Register(new ComponentDefinition<I, B>());
			Assert.IsInstanceOf<B>(cls.Create());
			var b = (B) cls.Create();
			Assert.AreEqual(cls,b.Cls);
		}

		[Test]
		public void CanSetupRuntimeClassWithContainerName()
		{
			var c = new Container();
			c.Register(new ComponentDefinition<I, A>(name:"my.c"));
			var cls = CreateCls("my.c",c);
			Assert.IsInstanceOf<A>(cls.Create());
			c.Register(new ComponentDefinition<I, B>(name: "my.c"));
			Assert.IsInstanceOf<B>(cls.Create());
			var b = (B)cls.Create();
			Assert.AreEqual(cls, b.Cls);
		}

		[Test]
		public void CanSetupRuntimeClassWithBound() {
			var cls = CreateCls<B>();
			Assert.AreEqual(RuntimeClassResolutionType.Resolved, cls.RuntimeDescriptor.ResolutionType);
			Assert.AreEqual(typeof(B), cls.RuntimeDescriptor.ResolvedType);
			Assert.IsInstanceOf<B>(cls.Create());
			var b = (B) cls.Create();
			Assert.AreEqual(cls,b.Cls);
		}

		[Test]
		public void CanSetupSimplyNoHeader() {
			var cls = new BSharpRuntimeClass{Definition = XElement.Parse("<a code='X' fullcode='N.S.X'/>")};
			Assert.AreEqual("X",cls.Name);
			Assert.AreEqual("N.S",cls.Namespace);
			Assert.AreEqual("N.S.X",cls.Fullname);
			Assert.NotNull(cls.RuntimeDescriptor);
			Assert.AreEqual(RuntimeClassResolutionType.NotDefined, cls.RuntimeDescriptor.ResolutionType);
		}

		[Test]
		public void CanSetupSimplyHeader()
		{
			var cls = new BSharpRuntimeClass { Definition =new XElement(BSharpRuntimeDefaults.BSHARP_CLASS_HEADER,  XElement.Parse("<a code='X' fullcode='N.S.X'/>") )};
			Assert.AreEqual("X", cls.Name);
			Assert.AreEqual("N.S", cls.Namespace);
			Assert.AreEqual("N.S.X", cls.Fullname);
			Assert.NotNull(cls.RuntimeDescriptor);
			Assert.AreEqual(RuntimeClassResolutionType.NotDefined, cls.RuntimeDescriptor.ResolutionType);
		}
	}
}