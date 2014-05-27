using System;
using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Tests.BSharp{
	[TestFixture]
	public class GenericSupport : CompileTestBase
	{
		[Test]
		public void BaseGenericSupport()
		{
			var result = Compile(@"
class BASE t=1
class A test='`{code}${code}${t}' generic
	import BASE
class B 
	import A
class C t=2
	import B");

			var b = result.Get("B");
			var c = result.Get("C");
			Assert.AreEqual("AB1", b.Compiled.Attr("test"));
			Assert.AreEqual("AC2", c.Compiled.Attr("test"));
		}



		[Test]
		public void GenericAttributeNames()
		{
			var result = Compile(@"
class BASE t=1
class A test`{t}${t}='${t}' t=2 generic
	import BASE
class B t=3
	import A");

			var b = result.Get("B");
			Console.WriteLine(b.Compiled);
			Assert.AreEqual("3", b.Compiled.Attr("test23"));
		}

		[Test]
		public void RealWorldGenericCase()
		{
			var result = Compile(@"
class A abstract x`{i}=true
class G1 'g1' generic i=1	
	import A
class G2 'g2' generic i=2
	import A
class G3 'g3' generic i=3
	import A
class B
	import G1
	import G2
	import G3
");

			var b = result.Get("B");
			Console.WriteLine(b.Compiled.ToString().Replace("\"", "'"));
			Assert.AreEqual(@"<class code='B' fullcode='B' name='g3' x1='true' i='3' x2='true' x3='true' />".Length, b.Compiled.ToString().Replace("\"", "'").Length);
		}

		[Test]
		public void CanSetGenericInElements()
		{
			var result = Compile(@"
class A abstract test`{i}=false
	element x
	x a`{i}  active=${test`{i}} 
class G1 'g1' generic i=1	
	import A
class G2 'g2' generic i=2
	import A
class G3 'g3' generic i=3
	import A
class B
	test2=true
	import G1
	import G2
	import G3
");

			var b = result.Get("B");
			Console.WriteLine(b.Compiled.ToString().Replace("\"", "'"));
			Assert.AreEqual(@"<class code='B' test2='true' fullcode='B' name='g3' test1='false' i='3' test3='false'>
  <x code='a3' active='false' />
  <x code='a2' active='true' />
  <x code='a1' active='false' />
</class>".Trim().LfOnly().Length, b.Compiled.ToString().Replace("\"", "'").Trim().LfOnly().Length);

		}

		[Test]
		public void CanSetGenericInElementsWithGenericConditions()
		{
			var result = Compile(@"
class A abstract test`{i}=false
	element x
	x a`{i}  active=${test`{i}} if=test`{i}
class G1 'g1' generic i=1	
	import A
class G2 'g2' generic i=2
	import A
class G3 'g3' generic i=3
	import A
class B
	test2=true
	import G1
	import G2
	import G3
");

			var b = result.Get("B");
			Console.WriteLine(b.Compiled.ToString().Replace("\"", "'"));
			Assert.AreEqual(@"<class code='B' test2='true' fullcode='B' name='g3' test1='false' i='3' test3='false'>
  <x code='a2' active='true' />
</class>".Trim().LfOnly().Length, b.Compiled.ToString().Replace("\"", "'").Trim().LfOnly().Length);

		}


		[Test]
		public void GenericSupportWithInternals()
		{
			var result = Compile(@"
class BASE abstract
	element X
	X '`{index}`{index2}${key}' name='${_name`{index}}'
class A index=1 index2=2 generic
	import BASE
class B index=2 index2=3 generic
	import BASE
class C index=4 index2=5 generic
	import BASE
class Final key=x 
	_name1 = a
	_name2 = b
	_name4 = c
	import A
	import B
	import C
");

			var b = result.Get("Final");
			Console.WriteLine(b.Compiled.ToString().Replace("\"", "'"));
			Assert.AreEqual(@"<class code='Final' key='x' fullcode='Final' index='4' index2='5'>
  <X code='45x' name='c' />
  <X code='23x' name='b' />
  <X code='12x' name='a' />
</class>".Trim().LfOnly().Length, b.Compiled.ToString().Replace("\"", "'").Trim().LfOnly().Length);
		}
	}

	
}