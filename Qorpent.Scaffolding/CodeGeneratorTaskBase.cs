using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Qorpent.BSharp;
using Qorpent.BSharp.Builder;
using Qorpent.Integration.BSharp.Builder.Tasks;

namespace Qorpent.Scaffolding{

	/// <summary>
	/// 
	/// </summary>
	public class Production{
		/// <summary>
		/// Имя файла
		/// </summary>
		public string FileName { get; set; }
		/// <summary>
		/// Содержимое
		/// </summary>
		public string Content { get; set; }
	}

	/// <summary>
	/// Задача для кодогенерации
	/// </summary>
	public abstract class CodeGeneratorTaskBase : BSharpBuilderTaskBase{
		/// <summary>
		/// Индекс
		/// </summary>
		public const int INDEX = TaskConstants.WriteWorkingOutputTaskIndex + 10;
		/// <summary>
		/// 
		/// </summary>
		protected const string CommonHeader = @"
//////////////////////////////////////////////////////////////////////
////       AUTO-GENERATED WITH  GenerateDataTypesInCSharpTask     ////
//////////////////////////////////////////////////////////////////////
";

		/// <summary>
		/// 
		/// </summary>
		protected IBSharpContext _context;

		/// <summary>
		/// Формирует задачу посткомиляции для построения ZETA INDEX
		/// </summary>
		public CodeGeneratorTaskBase()
		{
			Phase = BSharpBuilderPhase.PostProcess;
			Index =  INDEX;
	 

		}
		/// <summary>
		/// Критерий отбора классов для генерации
		/// </summary>
		public string ClassSearchCriteria { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string DefaultOutputName { get; set; }

		/// <summary>
		/// Трансформирует классы прототипа BIZINDEX в полноценные карты соотношения тем, блоков, подсистем
		/// </summary>
		/// <param name="context"></param>
		public override void Execute(IBSharpContext context)
		{
			_context = context;
			Project.Log.Info(this.GetType().Name+" called");
			var targetclasses = _context.ResolveAll(ClassSearchCriteria).ToArray();
			var outdir = GetOutDir();
			foreach (var production in InternalGenerate(targetclasses)){
				var filename = production.FileName;
				if (!Path.IsPathRooted(filename)){
					filename = Path.Combine(outdir, filename);
				}
				Project.Log.Info("Write "+filename);
				File.WriteAllText(filename,production.Content);
			}
			
		}

		

		private string GetOutDir(){
			if (string.IsNullOrWhiteSpace(DefaultOutputName)) return Project.GetOutputDirectory();
			var basedir = Project.Get(DefaultOutputName+"Dir", DefaultOutputName);
			if (string.IsNullOrWhiteSpace(basedir)){
				basedir = DefaultOutputName;
			}
			if (!Path.IsPathRooted(basedir)){
				basedir = Path.Combine(Project.GetOutputDirectory(), basedir);
			}
			Directory.CreateDirectory(basedir);
			return basedir;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetclasses"></param>
		protected abstract IEnumerable<Production> InternalGenerate(IBSharpClass[] targetclasses);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sb"></param>
		/// <param name="summary"></param>
		/// <param name="lang"></param>
		protected static void WriteMemberSummary(StringBuilder sb, string summary,string lang = "cs"){
			if (lang == "cs"){
				sb.AppendLine("\t\t///<summary>\r\n\t\t///\t" + summary + "\r\n\t\t///</summary>");
			}
			if (lang == "ts"){
				sb.AppendLine("\t\t\t// " + summary);
			}
			if (lang == "js")
			{
				sb.AppendLine("\t\t// " + summary);
			}
		}
	}
}