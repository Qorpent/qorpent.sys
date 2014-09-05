using System;
using NUnit.Framework;
using Qorpent.Scaffolding.Model;
using Qorpent.Scaffolding.Model.CodeWriters;

namespace Qorpent.Scaffolding.Tests.SqlAndOrm{
	[TestFixture]
	public class ResolveTagFacilityWriterTests
	{
		[Test]
		public void EmptyResolveTagOptionsWriter()
		{
			var model = PersistentModel.Compile(@"
class a prototype=dbtable
");
			var code = new ResolveTagOptionsWriter(model) { WithHeader = false }.ToString().Replace("\"", "\"\"");
			Console.WriteLine(code);
			Assert.AreEqual(@"
using System;
namespace Orm{
	///<summary>Options for tag resolution in model</summary>
	public partial class ResolveTagOptions {
		///<summary>Defult instance for full resolution</summary>
		public static readonly ResolveTagOptions Default  = new ResolveTagOptions();
		///<summary>Default instance for resolution on only one level</summary>
		public static readonly ResolveTagOptions SelfOnly = new ResolveTagOptions{
		};
	}
}
".Trim(), code.Trim());

		}

	

		[Test]
		public void EmptyResolveTagFacilityWriter()
		{
			var model = PersistentModel.Compile(@"
class a prototype=dbtable
");
			var code = new ResolveTagFacilityWriter(model) { WithHeader = false }.ToString().Replace("\"", "\"\"");
			Console.WriteLine(code);
			Assert.AreEqual(@"
using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Utils.Extensions;
namespace Orm{
	///<summary>Extensions for resolvetag model objects</summary>
	public static partial class ResolveTagFacility {
	}
}

".Trim(), code.Trim());

		}

		[Test]
		public void BasicSelfTagResolutionOptions()
		{
			var model = PersistentModel.Compile(@"
class a prototype=dbtable resolve
	string Tag resolve
");
			var code = new ResolveTagOptionsWriter(model) { WithHeader = false }.ToString().Replace("\"", "\"\"");
			Console.WriteLine(code);
			Assert.AreEqual(@"
using System;
namespace Orm{
	///<summary>Options for tag resolution in model</summary>
	public partial class ResolveTagOptions {
		///<summary>Defult instance for full resolution</summary>
		public static readonly ResolveTagOptions Default  = new ResolveTagOptions();
		///<summary>Default instance for resolution on only one level</summary>
		public static readonly ResolveTagOptions SelfOnly = new ResolveTagOptions{
		};
		///<summary>aTag can be used in resolution</summary>
		public bool aTag = true;
	}
}
".Trim(), code.Trim());

		}

		[Test]
		public void BasicSelfTagResolutionFacility()
		{
			var model = PersistentModel.Compile(@"
class a prototype=dbtable resolve
	string Tag resolve
");
			var code = new ResolveTagFacilityWriter(model) { WithHeader = false }.ToString();
			Console.WriteLine(code.Replace("\"", "\"\""));
			Assert.AreEqual(@"
using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Utils.Extensions;
namespace Orm{
	///<summary>Extensions for resolvetag model objects</summary>
	public static partial class ResolveTagFacility {
		///<summary>Resolves all tags for a</summary>
		public static string ResolveTag(this a target, string name, ResolveTagOptions options = null) {
			if(null==target)return """";
			if(string.IsNullOrWhiteSpace(name))return """";
			options = options ?? ResolveTagOptions.Default;
			var result = string.Empty;
			if (options.aTag){
				result = TagHelper.Value ( target.Tag, name);
				if (!string.IsNullOrWhiteSpace(result))return result;
			}
			return result??"""";
		}
	}
}
".Trim(), code.Trim());

		}


		[Test]
		public void ReferencedTableResolution()
		{
			var model = PersistentModel.Compile(@"
class a prototype=dbtable resolve
	string Tag resolve=10
a b prototype=dbtable resolve
	ref a resolve
	string Marks resolve
b c prototype=dbtable resolve
	ref b resolve
");
			Assert.AreEqual(99999,model["b"]["a"].ResolvePriority);
			Assert.True(model["c"]["b"].Resolve);
			Assert.True(model["c"]["Tag"].Resolve);
			Assert.True(model["c"]["Marks"].Resolve);
			Assert.True(model["c"]["a"].Resolve);
			var code = new ResolveTagFacilityWriter(model) { WithHeader = false }.ToString();
			Console.WriteLine(code.Replace("\"", "\"\""));
			Assert.AreEqual(@"
using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Utils.Extensions;
namespace Orm{
	///<summary>Extensions for resolvetag model objects</summary>
	public static partial class ResolveTagFacility {
		///<summary>Resolves all tags for a</summary>
		public static string ResolveTag(this a target, string name, ResolveTagOptions options = null) {
			if(null==target)return """";
			if(string.IsNullOrWhiteSpace(name))return """";
			options = options ?? ResolveTagOptions.Default;
			var result = string.Empty;
			if (options.aTag){
				result = TagHelper.Value ( target.Tag, name);
				if (!string.IsNullOrWhiteSpace(result))return result;
			}
			return result??"""";
		}
		///<summary>Resolves all tags for b</summary>
		public static string ResolveTag(this b target, string name, ResolveTagOptions options = null) {
			if(null==target)return """";
			if(string.IsNullOrWhiteSpace(name))return """";
			options = options ?? ResolveTagOptions.Default;
			var result = string.Empty;
			if (options.bTag){
				result = TagHelper.Value ( target.Tag, name);
				if (!string.IsNullOrWhiteSpace(result))return result;
			}
			if (options.bMarks){
				result = target.Marks.SmartSplit().Contains(name)?name:"""";
				if (!string.IsNullOrWhiteSpace(result))return result;
			}
			if (options.ba){
				result = target.a.ResolveTag(name);
				if (!string.IsNullOrWhiteSpace(result))return result;
			}
			return result??"""";
		}
		///<summary>Resolves all tags for c</summary>
		public static string ResolveTag(this c target, string name, ResolveTagOptions options = null) {
			if(null==target)return """";
			if(string.IsNullOrWhiteSpace(name))return """";
			options = options ?? ResolveTagOptions.Default;
			var result = string.Empty;
			if (options.cTag){
				result = TagHelper.Value ( target.Tag, name);
				if (!string.IsNullOrWhiteSpace(result))return result;
			}
			if (options.cMarks){
				result = target.Marks.SmartSplit().Contains(name)?name:"""";
				if (!string.IsNullOrWhiteSpace(result))return result;
			}
			if (options.ca){
				result = target.a.ResolveTag(name);
				if (!string.IsNullOrWhiteSpace(result))return result;
			}
			if (options.cb){
				result = target.b.ResolveTag(name);
				if (!string.IsNullOrWhiteSpace(result))return result;
			}
			return result??"""";
		}
	}
}
".Trim(), code.Trim());

		}

		[Test]
		public void ResolveOrderUsed()
		{
			var model = PersistentModel.Compile(@"
class a prototype=dbtable resolve
	string Tag resolve=20
	string Tag2 resolve=10
	string Tag3 resolve=30
");
			var code = new ResolveTagFacilityWriter(model) { WithHeader = false }.ToString();
			Console.WriteLine(code.Replace("\"", "\"\""));
			Assert.AreEqual(@"
using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Utils.Extensions;
namespace Orm{
	///<summary>Extensions for resolvetag model objects</summary>
	public static partial class ResolveTagFacility {
		///<summary>Resolves all tags for a</summary>
		public static string ResolveTag(this a target, string name, ResolveTagOptions options = null) {
			if(null==target)return """";
			if(string.IsNullOrWhiteSpace(name))return """";
			options = options ?? ResolveTagOptions.Default;
			var result = string.Empty;
			if (options.aTag2){
				result = target.Tag2.SmartSplit().Contains(name)?name:"""";
				if (!string.IsNullOrWhiteSpace(result))return result;
			}
			if (options.aTag){
				result = TagHelper.Value ( target.Tag, name);
				if (!string.IsNullOrWhiteSpace(result))return result;
			}
			if (options.aTag3){
				result = target.Tag3.SmartSplit().Contains(name)?name:"""";
				if (!string.IsNullOrWhiteSpace(result))return result;
			}
			return result??"""";
		}
	}
}
".Trim(), code.Trim());

		}
	
	}
}