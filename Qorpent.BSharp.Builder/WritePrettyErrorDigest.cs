using System.IO;
using System.Linq;
using System.Text;
using Qorpent.BSharp;
using Qorpent.BSharp.Builder;
using Qorpent.Integration.BSharp.Builder.Tasks;

namespace Qorpent.Integration.BSharp.Builder{
	/// <summary>
	/// 
	/// </summary>
	public class WritePrettyErrorDigest : BSharpBuilderTaskBase
	{
		/// <summary>
		/// 
		/// </summary>
		public WritePrettyErrorDigest(){
			Index = TaskConstants.WriteErrorInfoTaskIndex + 10;
			Phase =BSharpBuilderPhase.PostProcess;
			Async = true;
		}
		/// <summary>
		/// Формирует простой для чтений реестр ошибок
		/// </summary>
		/// <param name="context"></param>
		public override void Execute(IBSharpContext context){
			var sb = new StringBuilder();
			sb.AppendLine("<html>");
			sb.AppendLine("\t<head>");
			sb.AppendLine("\t\t<title>Ошибки по проекту " + Project.ProjectName + "</title>");
			sb.AppendLine("\t\t<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
			sb.AppendLine("\t<style type='text/css'>");
			sb.AppendLine(
				"\t\ttable {border-collapse:collapse;} td, th { padding : 5px; border:solid 1px gray; } th {background-color:black;color:white;} ");
			sb.AppendLine("\t\t.level-error {background-color : #ffdddd;} ");
			sb.AppendLine("\t</style>");
			sb.AppendLine("\t</head>");
			sb.AppendLine("\t<body>");
			sb.AppendLine("\t\t<h1>Ошибки по проекту " + Project.ProjectName + "</h1>");
			sb.AppendLine("\t\t<table>");
			sb.AppendLine("\t\t\t<tr>");
			sb.AppendLine("\t\t\t\t<th>Номер</th><th>Уровень</th><th>Тип</th><th>Класс</th><th>Сообщение</th><th>Файл</th><th>Строка</th><th>Колонка</th><th>Фаза</th>");
			sb.AppendLine("\t\t\t</tr>");
			var id = 1;
			foreach (var d in context.GetErrors(ErrorLevel.Warning).Select(_ => _.GetDigest()).OrderByDescending(_ => _.ErrorLevel).ThenBy(_=>_.ClassName).ThenBy(_=>_.FileName)){
				sb.AppendLine("\t\t\t<tr class='level-"+d.ErrorLevel+"'>");
				sb.AppendFormat("\t\t\t\t<td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td><td>{7}</td><td>{8}</td>\r\n",
				                id++,d.ErrorLevel,d.Type,d.ClassName,d.Message.Replace("\r\n","<br/>"),d.FileName,d.Line,d.Column,d.Phase
					);
				sb.AppendLine("\t\t\t</tr>");	
			}
			sb.AppendLine("\t\t</table>");
			sb.AppendLine("\t</body>");
			sb.AppendLine("</html>");

			var filename = Path.Combine(Project.GetOutputDirectory(), Project.ProjectName + ".formatted.errors.html");
			Directory.CreateDirectory(Path.GetDirectoryName(filename));
			File.WriteAllText(filename,sb.ToString());
		}
	}
}