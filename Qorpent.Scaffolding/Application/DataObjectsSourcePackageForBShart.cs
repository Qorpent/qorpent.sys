using System.Collections.Generic;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Bxl;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Application{
	/// <summary>
	/// Поставщик исходни	ков для расширений по генерации данных
	/// </summary>
	[ContainerComponent(Lifestyle.Transient, Name = "data.bssrcpkg", ServiceType = typeof(IBSharpSourceCodeProvider))]
	public class DataObjectsSourcePackageForBShart : ServiceBase, IBSharpSourceCodeProvider
	{
		[Inject]
		private IBxlParser parser { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="compiler"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public IEnumerable<XElement> GetSources(IBSharpCompiler compiler, IBSharpContext context)
		{
			foreach (var rname in typeof(DataObjectsSourcePackageForBShart).Assembly.FindAllResourceNames("Model.Compiler"))
			{
				var resource = typeof(DataObjectsSourcePackageForBShart).Assembly.ReadManifestResource(rname);
				yield return parser.Parse(resource, rname);
			}
		}
	}
}