using System.Collections.Generic;
using Qorpent.BSharp;

namespace Qorpent.Scaffolding.Model.Compiler{
	/// <summary>
	/// 
	/// </summary>
	public class GenerateTsqlScriptsTask : CodeGeneratorTaskBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dbobjectclasses"></param>
		/// <returns></returns>
		protected override IEnumerable<Production> InternalGenerate(IBSharpClass[] dbobjectclasses){

			var model = (PersistentModel)_context.ExtendedData[PrepareModelTask.DefaultModelName];
			var nsafetsqlf = Project.ProjectName + ".MSSQL.sql";
			var dropsqlf = Project.ProjectName + ".MSSQL.drop.sql";
			var nsafetsqlfpg = Project.ProjectName + ".PG.sql";
			var dropsqlfpg = Project.ProjectName + ".PG.drop.sql";


		
			yield return
				new Production
				{
					FileName = nsafetsqlf,
					GetContent =()=>model.GetScript(SqlDialect.SqlServer, ScriptMode.Create)
				};

			yield return
				new Production
				{
					FileName = dropsqlf,
					GetContent = () => model.GetScript(SqlDialect.SqlServer, ScriptMode.Drop)
				};
			yield return
				new Production
				{
					FileName = nsafetsqlfpg,
					GetContent = () => model.GetScript(SqlDialect.PostGres, ScriptMode.Create)
				};

			yield return
				new Production
				{
					FileName = dropsqlfpg,
					GetContent = () => model.GetScript(SqlDialect.PostGres, ScriptMode.Drop)
				};

		}
		/// <summary>
		/// 
		/// </summary>
		public GenerateTsqlScriptsTask():base(){
			ClassSearchCriteria = "attr:isdataobject";
			DefaultOutputName = "Sql";
		}
		
	}
}