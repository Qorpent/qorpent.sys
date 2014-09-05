using NUnit.Framework;

namespace Qorpent.IoC.Tests{
	/// <summary>
	/// Проверяем работоспособность SimpleContainer
	/// </summary>
	[TestFixture]
	public class SimpleContainerTest{
		interface IA{
			int Val { get; set; }
		}
		class A:IA{
			public A(){
				this.Val = 1;
			}
			public A(int val){
				this.Val = val;
			}
			public int Val { get; set; }
		}
		[Test]
		public void SupportCtors(){
			var c = new SimpleContainer();
			c.Register(new BasicComponentDefinition { ServiceType = typeof(IA), ImplementationType = typeof(A) });
			var a1 = c.Get<IA>();
			var a2 = c.Get<IA>(null, 23);
			Assert.AreEqual(1,a1.Val);
			Assert.AreEqual(23,a2.Val);
		}
	}
}