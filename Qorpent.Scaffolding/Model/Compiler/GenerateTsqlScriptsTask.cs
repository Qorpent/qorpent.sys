using System.Collections.Generic;
using Qorpent.BSharp;
using Qorpent.Data;

namespace Qorpent.Scaffolding.Model.Compiler{
	/// <summary>
	/// </summary>
	public class GenerateTsqlScriptsTask : CodeGeneratorTaskBase{
		/// <summary>
		/// </summary>
		public GenerateTsqlScriptsTask(){
			ClassSearchCriteria = "attr:isdataobject";
			DefaultOutputName = "Sql";
		}

		/// <summary>
		/// </summary>
		/// <param name="dbobjectclasses"></param>
		/// <returns></returns>
		protected override IEnumerable<Production> InternalGenerate(IBSharpClass[] dbobjectclasses){
			var model = (PersistentModel) _context.ExtendedData[PrepareModelTask.DefaultModelName];
            var prefix = Project.ProjectName;
            if (model.GenerationOptions.IncludeSqlObjectTypes != SqlObjectType.All || model.GenerationOptions.ExcludeSqlObjectTypes != SqlObjectType.None) {
                prefix += "."+string.Format("IN_{0}_EX_{1}", model.GenerationOptions.IncludeSqlObjectTypes,
                    model.GenerationOptions.ExcludeSqlObjectTypes).Replace(", ","_");

            }

            string nsafetsqlf = prefix + ".MSSQL.sql";
            string dropsqlf = prefix + ".MSSQL.drop.sql";
            string nsafetsqlfpg = prefix + ".PG.sql";
            string dropsqlfpg = prefix + ".PG.drop.sql";

		    

			yield return
				new Production{
					FileName = nsafetsqlf,
					GetContent = () => model.GetScript(SqlDialect.SqlServer, ScriptMode.Create)
				};

			yield return
				new Production{
					FileName = dropsqlf,
					GetContent = () => model.GetScript(SqlDialect.SqlServer, ScriptMode.Drop)
				};
			yield return
				new Production{
					FileName = nsafetsqlfpg,
					GetContent = () => model.GetScript(SqlDialect.PostGres, ScriptMode.Create)
				};

			yield return
				new Production{
					FileName = dropsqlfpg,
					GetContent = () => model.GetScript(SqlDialect.PostGres, ScriptMode.Drop)
				};
		}
	}
}