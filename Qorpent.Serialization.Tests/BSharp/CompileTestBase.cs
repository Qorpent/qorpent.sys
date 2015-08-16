using System.Collections.Generic;
using System.Linq;
using Qorpent.BSharp;
using Qorpent.Bxl;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Tests.BSharp {
	public class CompileTestBase {
		protected BSharpContext Compile(string code, object globals = null, IDictionary<string, string> conditions = null, BSharpConfig _cfg = null) {
			var cfg = new BSharpConfig();
			cfg.Conditions = conditions;
			if (null != globals){
				cfg.Global = new Scope(globals.ToDict()){UseInheritance = false};
			}
		    if (null != _cfg) {
		        _cfg.SetParent(cfg);
		        cfg = _cfg;
		    }
			return (BSharpContext) BSharpCompiler.Compile(code, cfg);
		}

		protected BSharpContext CompileAll(bool single,params string[] code) {
			var parser = new BxlParser();
			var idx = 0;
			var xmls = code.Select(_ => parser.Parse(_, (idx++) + ".bxl")).ToArray();
			var cfg = new BSharpConfig{SingleSource = single,KeepLexInfo = false};
			var result = BSharpCompiler.Compile(xmls,cfg);
			return (BSharpContext)result;

		}
	}
}