using System.Linq;
using Qorpent.Bxl;
using Qorpent.ObjectXml;

namespace Qorpent.Serialization.Tests.ObjectXml {
	public class CompileTestBase {
		protected ObjectXmlCompilerIndex Compile(string code) {
			var xml = new BxlParser().Parse(code, "c.bxl");
			var cfg = new ObjectXmlCompilerConfig();
			cfg.UseInterpolation = true;

			var compiler = new ObjectXmlCompiler();
			compiler.Initialize(cfg);
			return  compiler.Compile(new[] {xml});
		}

		protected ObjectXmlCompilerIndex CompileAll(bool single,params string[] code) {
			var parser = new BxlParser();
			var idx = 0;
			var xmls = code.Select(_ => parser.Parse(_, (idx++) + ".bxl")).ToArray();
			var cfg = new ObjectXmlCompilerConfig();
			cfg.UseInterpolation = true;
			if (single) {
				cfg.SingleSource = true;
			}
			var compiler = new ObjectXmlCompiler();
			compiler.Initialize(cfg);
			var result = compiler.Compile(xmls);
			return result;

		}
	}
}