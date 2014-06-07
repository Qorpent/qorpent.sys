using System.Collections.Generic;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Bxl;
using Qorpent.IoC;
using Qorpent.Scaffolding.Model.Compiler;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Application
{
	/// <summary>
	/// Поставщик исходни	ков для расширений по генерации данных
	/// </summary>
	[ContainerComponent(Lifestyle.Transient, Name = "app.bssrcpkg", ServiceType = typeof(IBSharpSourceCodeProvider))]
	public class AppSpecificationBSharpSource : ResourceBasedSrcBSharpProvider{

		/// <summary>
		/// 
		/// </summary>
		public AppSpecificationBSharpSource()
		{
			ResourceMarker = "Application";
		}

		
	}
}
