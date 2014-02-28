using System.IO;
using System.Linq;
using Qorpent.BSharp;
using Qorpent.BSharp.Builder;
using Qorpent.Integration.BSharp.Builder.Tasks;

namespace Qorpent.Data.BSharpDDL{
	/// <summary>
	/// 
	/// </summary>
	public class GenerateTsqlScriptsTask : BSharpBuilderTaskBase
	{
		private IBSharpContext _context;

		/// <summary>
		/// Индекс
		/// </summary>
		public const int INDEX = TaskConstants.WriteWorkingOutputTaskIndex + 10;
		/// <summary>
		/// Формирует задачу посткомиляции для построения ZETA INDEX
		/// </summary>
		public GenerateTsqlScriptsTask()
		{
			Phase = BSharpBuilderPhase.PostProcess;
			Index =  INDEX;
	 

		}

		/// <summary>
		/// Трансформирует классы прототипа BIZINDEX в полноценные карты соотношения тем, блоков, подсистем
		/// </summary>
		/// <param name="context"></param>
		public override void Execute(IBSharpContext context)
		{
			_context = context;
			Project.Log.Info("GenerateTSQLScriptsTask called");
			var dbobjectclasses = _context.ResolveAll("attr:isdataobject").ToArray();
			var dbobjects = dbobjectclasses.SelectMany(_ => DbObject.Create(_, _context)).ToArray();
			var outdir = GetOutDir();
			var safetsqlf = Path.Combine(outdir, Project.ProjectName + ".MSSQL.safe.sql");
			var nsafetsqlf = Path.Combine(outdir, Project.ProjectName + ".MSSQL.sql");
			var dropsqlf = Path.Combine(outdir, Project.ProjectName + ".MSSQL.drop.sql");
			File.WriteAllText(safetsqlf,DbObject.GetSql(dbobjects,DbGenerationMode.Script|DbGenerationMode.Safe,DbDialect.TSQL,Project));
			File.WriteAllText(nsafetsqlf,DbObject.GetSql(dbobjects,DbGenerationMode.Script,DbDialect.TSQL,Project));
			File.WriteAllText(dropsqlf, DbObject.GetSql(dbobjects, DbGenerationMode.Script | DbGenerationMode.Drop, DbDialect.TSQL, Project));
		}

		private string GetOutDir(){
			var basedir = Project.Get("SqlDir", "Sql");
			if (string.IsNullOrWhiteSpace(basedir)){
				basedir = "Sql";
			}
			if (!Path.IsPathRooted(basedir)){
				basedir = Path.Combine(Project.GetOutputDirectory(), basedir);
			}
			Directory.CreateDirectory(basedir);
			return basedir;
		}
	}
}