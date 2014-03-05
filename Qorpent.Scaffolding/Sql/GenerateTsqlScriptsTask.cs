using System.Collections.Generic;
using System.Linq;
using Qorpent.BSharp;
using Qorpent.Scaffolding.SqlGeneration;

namespace Qorpent.Scaffolding.Sql{
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
		protected override IEnumerable<Production> InternalGenerate(IBSharpClass[] dbobjectclasses)
		{
			var dbobjects = dbobjectclasses.SelectMany(_ => DbObject.Create(_, _context)).ToArray();
			var safetsqlf =  Project.ProjectName + ".MSSQL.safe.sql";
			var nsafetsqlf = Project.ProjectName + ".MSSQL.sql";
			var dropsqlf = Project.ProjectName + ".MSSQL.drop.sql";


			yield return
				new Production{
					FileName = safetsqlf,
					Content = DbObject.GetSql(dbobjects, DbGenerationMode.Script | DbGenerationMode.Safe, DbDialect.TSQL, Project)
				};

			yield return
				new Production
				{
					FileName = nsafetsqlf,
					Content = DbObject.GetSql(dbobjects, DbGenerationMode.Script, DbDialect.TSQL, Project)
				};

			yield return
				new Production
				{
					FileName = dropsqlf,
					Content = DbObject.GetSql(dbobjects, DbGenerationMode.Script | DbGenerationMode.Drop, DbDialect.TSQL, Project)
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