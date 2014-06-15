using System;
using NUnit.Framework;
using Qorpent.Scaffolding.Model;
using Qorpent.Scaffolding.Model.CodeWriters;

namespace Qorpent.Scaffolding.Tests.SqlAndOrm{
	[TestFixture]
	public class PokoClassWriterTests{
		[Test]
		public void SimplestTable()
		{
			var model = PersistentModel.Compile(@"
class a prototype=dbtable
");
			var code = new PokoClassWriter(model["a"]) { WithHeader = false }.ToString().Replace("\"", "\"\"");
			Console.WriteLine(code);
			Assert.AreEqual(@"
using System;
using System.Collections.Generic;
#if !NOQORPENT
using Qorpent.Serialization;
using Qorpent.Model;
#endif
namespace  {
	///<summary>
	///
	///</summary>
#if !NOQORPENT
	[Serialize]
#endif
	public partial class a  {
		///<summary>Lazy load nest type</summary>
		public class Lazy:a{
			///<summary>Function to get lazy</summary>
			public Func<a,a> GetLazy;
		}
		///<summary>
		/// 
		///</summary>
		public virtual Int32 Id {get {return NativeId;} set{NativeId=value;}}

		///<summary>Direct access to Id</summary>
		protected Int32 NativeId;


	}
}".Trim(), code.Trim());

		}

		[Test]
		public void SerializationSupport()
		{
			var model = PersistentModel.Compile(@"
class b prototype=dbtable
class a prototype=dbtable
	string X serialize
	string Y serialize=notnull
	string Z serialize=ignore
	ref b serialize=notnull
	ref b2 to=b serialize
");
			var code = new PokoClassWriter(model["a"]) { WithHeader = false }.ToString().Replace("\"", "\"\"");
			Console.WriteLine(code);
			Assert.AreEqual(@"
using System;
using System.Collections.Generic;
#if !NOQORPENT
using Qorpent.Serialization;
using Qorpent.Model;
#endif
namespace  {
	///<summary>
	///
	///</summary>
#if !NOQORPENT
	[Serialize]
#endif
	public partial class a  {
		///<summary>Lazy load nest type</summary>
		public class Lazy:a{
			///<summary>Function to get lazy</summary>
			public Func<a,a> GetLazy;
		}
		///<summary>
		/// 
		///</summary>
		public virtual Int32 Id {get {return NativeId;} set{NativeId=value;}}

		///<summary>Direct access to Id</summary>
		protected Int32 NativeId;


		///<summary>
		/// (Идентификатор)
		///</summary>
#if !NOQORPENT
		[SerializeNotNullOnly]
#endif
		public virtual Int32 bId {get {return NativebId;} set{NativebId=value;}}

		///<summary>Direct access to bId</summary>
		protected Int32 NativebId;


		///<summary>
		/// 
		///</summary>
#if !NOQORPENT
		[SerializeNotNullOnly]
#endif
		public virtual b b {get {return ((null!=Nativeb as b.Lazy )?( Nativeb = ((b.Lazy) Nativeb ).GetLazy(Nativeb) ): Nativeb );} set{Nativeb=value;}}

		///<summary>Direct access to b</summary>
		protected b Nativeb;


		///<summary>
		/// (Идентификатор)
		///</summary>
#if !NOQORPENT
		[Serialize]
#endif
		public virtual Int32 b2Id {get {return Nativeb2Id;} set{Nativeb2Id=value;}}

		///<summary>Direct access to b2Id</summary>
		protected Int32 Nativeb2Id;


		///<summary>
		/// 
		///</summary>
#if !NOQORPENT
		[Serialize]
#endif
		public virtual b b2 {get {return ((null!=Nativeb2 as b.Lazy )?( Nativeb2 = ((b.Lazy) Nativeb2 ).GetLazy(Nativeb2) ): Nativeb2 );} set{Nativeb2=value;}}

		///<summary>Direct access to b2</summary>
		protected b Nativeb2;


		///<summary>
		///serialize 
		///</summary>
		public virtual String X {get {return NativeX;} set{NativeX=value;}}

		///<summary>Direct access to X</summary>
		protected String NativeX;


		///<summary>
		/// 
		///</summary>
#if !NOQORPENT
		[SerializeNotNullOnly]
#endif
		public virtual String Y {get {return NativeY;} set{NativeY=value;}}

		///<summary>Direct access to Y</summary>
		protected String NativeY;


		///<summary>
		/// 
		///</summary>
#if !NOQORPENT
		[IgnoreSerialize]
#endif
		public virtual String Z {get {return NativeZ;} set{NativeZ=value;}}

		///<summary>Direct access to Z</summary>
		protected String NativeZ;


	}
}".Trim(), code.Trim());

		}


		[Test]
		public void ImplementsSupport()
		{
			var model = PersistentModel.Compile(@"
class a prototype=dbtable
	implements IMyInterface
	string Code Код
");
			var code = new PokoClassWriter(model["a"]) { WithHeader = false }.ToString().Replace("\"", "\"\"");
			Console.WriteLine(code);
			Assert.AreEqual(@"
using System;
using System.Collections.Generic;
#if !NOQORPENT
using Qorpent.Serialization;
using Qorpent.Model;
#endif
namespace  {
	///<summary>
	///
	///</summary>
#if !NOQORPENT
	[Serialize]
#endif
	public partial class a : IMyInterface {
		///<summary>Lazy load nest type</summary>
		public class Lazy:a{
			///<summary>Function to get lazy</summary>
			public Func<a,a> GetLazy;
		}
		///<summary>
		/// 
		///</summary>
		public virtual Int32 Id {get {return NativeId;} set{NativeId=value;}}

		///<summary>Direct access to Id</summary>
		protected Int32 NativeId;


		///<summary>
		///Код 
		///</summary>
		public virtual String Code {get {return NativeCode;} set{NativeCode=value;}}

		///<summary>Direct access to Code</summary>
		protected String NativeCode;


	}
}
".Trim(), code.Trim());

		}

		[Test]
		public void ImplementsAndOverrideSupport()
		{
			var model = PersistentModel.Compile(@"
class a prototype=dbtable
	implements IMyInterface
	string Code Код override=IMyInterface
");
			var code = new PokoClassWriter(model["a"]) { WithHeader = false }.ToString().Replace("\"", "\"\"");
			Console.WriteLine(code);
			Assert.AreEqual(@"
using System;
using System.Collections.Generic;
#if !NOQORPENT
using Qorpent.Serialization;
using Qorpent.Model;
#endif
namespace  {
	///<summary>
	///
	///</summary>
#if !NOQORPENT
	[Serialize]
#endif
	public partial class a : IMyInterface {
		///<summary>Lazy load nest type</summary>
		public class Lazy:a{
			///<summary>Function to get lazy</summary>
			public Func<a,a> GetLazy;
		}
		///<summary>
		/// 
		///</summary>
		public virtual Int32 Id {get {return NativeId;} set{NativeId=value;}}

		///<summary>Direct access to Id</summary>
		protected Int32 NativeId;


		///<summary>
		///Код 
		///</summary>
		public override String Code {get {return NativeCode;} set{NativeCode=value;}}

		///<summary>Direct access to Code</summary>
		protected String NativeCode;


	}
}
".Trim(), code.Trim());

		}

		[Test]
		public void ImplementsAndHideSupport()
		{
			var model = PersistentModel.Compile(@"
class a prototype=dbtable
	implements IMyInterface
	string Code Код hide=IMyInterface
");
			var code = new PokoClassWriter(model["a"]) { WithHeader = false }.ToString().Replace("\"", "\"\"");
			Console.WriteLine(code);
			Assert.AreEqual(@"
using System;
using System.Collections.Generic;
#if !NOQORPENT
using Qorpent.Serialization;
using Qorpent.Model;
#endif
namespace  {
	///<summary>
	///
	///</summary>
#if !NOQORPENT
	[Serialize]
#endif
	public partial class a : IMyInterface {
		///<summary>Lazy load nest type</summary>
		public class Lazy:a{
			///<summary>Function to get lazy</summary>
			public Func<a,a> GetLazy;
		}
		///<summary>
		/// 
		///</summary>
		public virtual Int32 Id {get {return NativeId;} set{NativeId=value;}}

		///<summary>Direct access to Id</summary>
		protected Int32 NativeId;


	}
}
".Trim(), code.Trim());

		}

		[Test]
		public void ReferenceSupport()
		{
			var model = PersistentModel.Compile(@"
class a prototype=dbtable
	ref b
class b prototype=dbtable
");
			var code = new PokoClassWriter(model["a"]) { WithHeader = false }.ToString().Replace("\"", "\"\"");
			Console.WriteLine(code);
			Assert.AreEqual(@"
using System;
using System.Collections.Generic;
#if !NOQORPENT
using Qorpent.Serialization;
using Qorpent.Model;
#endif
namespace  {
	///<summary>
	///
	///</summary>
#if !NOQORPENT
	[Serialize]
#endif
	public partial class a  {
		///<summary>Lazy load nest type</summary>
		public class Lazy:a{
			///<summary>Function to get lazy</summary>
			public Func<a,a> GetLazy;
		}
		///<summary>
		/// 
		///</summary>
		public virtual Int32 Id {get {return NativeId;} set{NativeId=value;}}

		///<summary>Direct access to Id</summary>
		protected Int32 NativeId;


		///<summary>
		/// (Идентификатор)
		///</summary>
		public virtual Int32 bId {get {return NativebId;} set{NativebId=value;}}

		///<summary>Direct access to bId</summary>
		protected Int32 NativebId;


		///<summary>
		/// 
		///</summary>
		public virtual b b {get {return ((null!=Nativeb as b.Lazy )?( Nativeb = ((b.Lazy) Nativeb ).GetLazy(Nativeb) ): Nativeb );} set{Nativeb=value;}}

		///<summary>Direct access to b</summary>
		protected b Nativeb;


	}
}
".Trim(), code.Trim());

		}

		[Test]
		public void BackReferenceSupport()
		{
			var model = PersistentModel.Compile(@"
class a prototype=dbtable
class b prototype=dbtable
	ref a reverse
");
			var code = new PokoClassWriter(model["a"]) { WithHeader = false }.ToString().Replace("\"", "\"\"");
			Console.WriteLine(code);
			Assert.AreEqual(@"
using System;
using System.Collections.Generic;
#if !NOQORPENT
using Qorpent.Serialization;
using Qorpent.Model;
#endif
namespace  {
	///<summary>
	///
	///</summary>
#if !NOQORPENT
	[Serialize]
#endif
	public partial class a  {
		///<summary>Lazy load nest type</summary>
		public class Lazy:a{
			///<summary>Function to get lazy</summary>
			public Func<a,a> GetLazy;
		}
		///<summary>
		/// 
		///</summary>
		public virtual Int32 Id {get {return NativeId;} set{NativeId=value;}}

		///<summary>Direct access to Id</summary>
		protected Int32 NativeId;


		///<summary>
		///reverse (a  привязка )
		///</summary>
		public virtual ICollection<b> bs {get {return Nativebs?? (Nativebs = new List<b>());} set{Nativebs=value;}}

		///<summary>Direct access to bs</summary>
		protected ICollection<b> Nativebs;


	}
}
".Trim(), code.Trim());

		}


		[Test]
		public void NoCodeSupport()
		{
			var model = PersistentModel.Compile(@"
class a prototype=dbtable
	string x
	string y nosql
	string z nocode
");
			var code = new PokoClassWriter(model["a"]) { WithHeader = false }.ToString().Replace("\"", "\"\"");
			Console.WriteLine(code);
			Assert.AreEqual(@"
using System;
using System.Collections.Generic;
#if !NOQORPENT
using Qorpent.Serialization;
using Qorpent.Model;
#endif
namespace  {
	///<summary>
	///
	///</summary>
#if !NOQORPENT
	[Serialize]
#endif
	public partial class a  {
		///<summary>Lazy load nest type</summary>
		public class Lazy:a{
			///<summary>Function to get lazy</summary>
			public Func<a,a> GetLazy;
		}
		///<summary>
		/// 
		///</summary>
		public virtual Int32 Id {get {return NativeId;} set{NativeId=value;}}

		///<summary>Direct access to Id</summary>
		protected Int32 NativeId;


		///<summary>
		/// 
		///</summary>
		public virtual String x {get {return Nativex;} set{Nativex=value;}}

		///<summary>Direct access to x</summary>
		protected String Nativex;


		///<summary>
		///nosql 
		///</summary>
		public virtual String y {get {return Nativey;} set{Nativey=value;}}

		///<summary>Direct access to y</summary>
		protected String Nativey;


	}
}
".Trim(), code.Trim());

		}
	}
}