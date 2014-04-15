using System.Collections.Generic;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Bxl;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Sql
{
	/// <summary>
	/// Поставщик исходни	ков для расширений по генерации данных
	/// </summary>
	[ContainerComponent(Lifestyle.Transient, Name = "preprocessor.bssrcpkg", ServiceType = typeof(IBSharpSourceCodeProvider))]
	public class PreprocessorSourcePackageForBSharp : ServiceBase, IBSharpSourceCodeProvider{
		[Inject] private IBxlParser parser { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="compiler"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public IEnumerable<XElement> GetSources(IBSharpCompiler compiler, IBSharpContext context){
			foreach (var rname in typeof(PreprocessorSourcePackageForBSharp).Assembly.FindAllResourceNames("preprocessor")){
				var resource = typeof (PreprocessorSourcePackageForBSharp).Assembly.ReadManifestResource(rname);
				yield return parser.Parse(resource, rname);
			}
		}
	}
}
