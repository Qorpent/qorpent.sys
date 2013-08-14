using System.Linq;
using Qorpent.BSharp;
using Qorpent.Bxl;

namespace Qorpent.Serialization.Tests.ObjectXml {
	public class CompileTestBase {
		protected BSharpContext Compile(string code) {
			var xml = new BxlParser().Parse(code, "c.bxl");
			var cfg = new BSharpConfig();
			cfg.UseInterpolation = true;

			var compiler = new BSharpCompiler();
			compiler.Initialize(cfg);
			return  compiler.Compile(new[] {xml});
		}

		protected BSharpContext CompileAll(bool single,params string[] code) {
			var parser = new BxlParser();
			var idx = 0;
			var xmls = code.Select(_ => parser.Parse(_, (idx++) + ".bxl")).ToArray();
			var cfg = new BSharpConfig();
			cfg.UseInterpolation = true;
			if (single) {
				cfg.SingleSource = true;
			}
			var compiler = new BSharpCompiler();
			compiler.Initialize(cfg);
			var result = compiler.Compile(xmls);
			return result;

		}
	}
}