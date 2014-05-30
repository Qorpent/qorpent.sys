using System.Diagnostics;
using Qorpent.BSharp.Builder;
using Qorpent.IoC;
using Qorpent.Scaffolding.Sql;

namespace Qorpent.Scaffolding.Orm{
	/// <summary>
	/// Расширения компилятора B# для работы с DDL B#
	/// </summary>
	[ContainerComponent(Lifestyle.Transient,"orm.bsbext",ServiceType=typeof(IBSharpBuilderExtension))]
	public class OrmCompilerExtensions : BSharpBuilderExtensionBase, IBSharpBuilderExtension
	{
		/// <summary>
		/// 
		/// </summary>
		protected override void PrepareTasks()
		{
			
			Tasks.Add(new GeneratePokoClasses());
			Tasks.Add(new GeneratePokoClassDataAdapter());
			Tasks.Add(new GenerateModel());
			Tasks.Add(new GenerateExtendedCachedModel());
			
		}
	}
}