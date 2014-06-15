using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Scaffolding.Model;
using Qorpent.Scaffolding.Model.CodeWriters;

namespace Qorpent.Scaffolding.Tests.SqlAndOrm
{
	[TestFixture]
	public class CloneFacilityWriterTests
	{
		[Test]
		public void EmptyCloneableOptionsWriter(){
			var model = PersistentModel.Compile(@"
class a prototype=dbtable
");
			var code = new CloneOptionsWriter(model){WithHeader = false}.ToString().Replace("\"","\"\"");
			Console.WriteLine(code);
			Assert.AreEqual(@"
using System;
namespace Orm{
	///<summary>Options for cloning in model</summary>
	public partial class CloneOptions {
		///<summary>Defult instance</summary>
		public static readonly CloneOptions Default = new CloneOptions();
	}
}
".Trim(),code.Trim());

		}

		[Test]
		public void EmptyCloneableFacilityWriter()
		{
			var model = PersistentModel.Compile(@"
class a prototype=dbtable
");
			var code = new CloneFacilityWriter(model) { WithHeader = false }.ToString().Replace("\"", "\"\"");
			Console.WriteLine(code);
			Assert.AreEqual(@"
using System;
using System.Collections.Generic;
using System.Linq;
namespace Orm{
	///<summary>Extensions for cloning model objects</summary>
	public static partial class CloneFacility {
	}
}
".Trim(), code.Trim());

		}


		[Test]
		public void CloneableOptionsWriterWithRefs()
		{
			var model = PersistentModel.Compile(@"
class a prototype=dbtable cloneable
class b prototype=dbtable cloneable
	ref a reverse
");
			var code = new CloneOptionsWriter(model) { WithHeader = false }.ToString().Replace("\"", "\"\"");
			Console.WriteLine(code);

			Assert.AreEqual(@"
using System;
namespace Orm{
	///<summary>Options for cloning in model</summary>
	public partial class CloneOptions {
		///<summary>Defult instance</summary>
		public static readonly CloneOptions Default = new CloneOptions();
		///<summary>abs must be cloned</summary>
		public bool abs = false;
		///<summary>ba must be cloned</summary>
		public bool ba = false;
	}
}
".Trim(), code.Trim());

		}

		[Test]
		public void CloneableFacilityWriterWithRefs()
		{
			var model = PersistentModel.Compile(@"
class a prototype=dbtable cloneable
class b prototype=dbtable cloneable
	ref a reverse
");
			var code = new CloneFacilityWriter(model) { WithHeader = false }.ToString().Replace("\"", "\"\"");
			Console.WriteLine(code);

			Assert.AreEqual(@"
using System;
using System.Collections.Generic;
using System.Linq;
namespace Orm{
	///<summary>Extensions for cloning model objects</summary>
	public static partial class CloneFacility {
				///<summary>Clones a</summary>
		public static a Clone(this a target, CloneOptions options = null) {
			if(null==target)return null;
			options = options ?? CloneOptions.Default;
			var result = new a();
			result.Id = target.Id;
			if(options.abs) {
				result.bs = target.bs.Select(_=>_.Clone(options)).ToList();
			}else{
				result.bs = target.bs;
			}
			return result;
		}
				///<summary>Clones b</summary>
		public static b Clone(this b target, CloneOptions options = null) {
			if(null==target)return null;
			options = options ?? CloneOptions.Default;
			var result = new b();
			result.Id = target.Id;
			if(options.ba)result.a = result.a.Clone();
			else result.a = target.a;
			return result;
		}
	}
}
".Trim(), code.Trim());

		}

		[Test]
		public void CloneableOptionsWriterWithRefsAndDefaults()
		{
			var model = PersistentModel.Compile(@"
class a prototype=dbtable cloneable
class b prototype=dbtable cloneable
	ref a reverse clone reverse-clone
");
			var code = new CloneOptionsWriter(model) { WithHeader = false }.ToString().Replace("\"", "\"\"");
			Console.WriteLine(code);

			Assert.AreEqual(@"
using System;
namespace Orm{
	///<summary>Options for cloning in model</summary>
	public partial class CloneOptions {
		///<summary>Defult instance</summary>
		public static readonly CloneOptions Default = new CloneOptions();
		///<summary>abs must be cloned</summary>
		public bool abs = true;
		///<summary>ba must be cloned</summary>
		public bool ba = true;
	}
}
".Trim(), code.Trim());

		}
	}
}
