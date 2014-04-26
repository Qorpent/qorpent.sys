using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using Qorpent.Applications;
using Qorpent.BSharp;
using Qorpent.BSharp.Builder;
using Qorpent.Serialization;

namespace Qorpent.Integration.BSharp.Builder.Tasks {
	/// <summary>
	/// Формирует файл JSON с массивом откомпилированных классов
	/// </summary>
	public class GenerateJsonModuleTask : BSharpBuilderTaskBase
	{

	
		/// <summary>
		/// 
		/// </summary>
		/// <param name="phase"></param>
		/// <param name="index"></param>
		public GenerateJsonModuleTask(BSharpBuilderPhase phase = BSharpBuilderPhase.PostProcess, int index =TaskConstants.GenerateSrcPackageTaskIndex) {
			Phase = phase;
			Index = index;
			Async = true;
		}

		/// <summary>
		/// Формирует один большой файл JSON  с рабочиим классами
		/// </summary>
		/// <param name="context"></param>
		public override void Execute(IBSharpContext context) {
			Project.Log.Info("start generate json module");
			using (var sw = new StreamWriter(OutFileName)) {
				sw.Write("[");
				var tojson = new XmlToJsonConverter();
				var notformatedBuffer = new StringWriter();
				var formattedBuffer = new StringWriter();
				foreach (var cls in context.Get(BSharpContextDataType.LibPkg)) {
					var converted = tojson.ConvertToJson(cls.Compiled,false);
					notformatedBuffer.Write(converted);
					converted = tojson.ConvertToJson(cls.Compiled,true);
					formattedBuffer.Write(",");
					formattedBuffer.Write(converted);
				}
				var hashbase = notformatedBuffer.ToString();
				var content = formattedBuffer.ToString();

				sw.Write(tojson.ConvertToJson(new XElement(BSharpSyntax.Class,
					new XAttribute("code","__module"),
					new XAttribute("fullcode","__module"),
					new XAttribute("prototype","__sys"),
					new XAttribute("hash",Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(hashbase)))),
					new XAttribute("timestamp",(DateTime.Now-new DateTime(2000,1,1)).TotalMilliseconds),
					new XAttribute("user",Application.Current.Principal.CurrentUser.Identity.Name),
					new XAttribute("host",Environment.MachineName)
					),true));
				sw.Write(content);
				sw.Write("]");
				sw.Flush();
			}
			Project.Log.Info("finish generate json module");
		}



		/// <summary>
		/// Установить целевой проект
		/// </summary>
		/// <param name="project"></param>
		public override void SetProject(IBSharpProject project)
		{
			base.SetProject(project);

			var _name = Project.JsonModuleName;
			if (string.IsNullOrWhiteSpace(_name)) {
				_name = Project.ProjectName + ".json";
			}
			if (!Path.IsPathRooted(_name))
			{
				_name = Path.Combine(project.GetOutputDirectory(), _name );
			}

			this.OutFileName = _name;
		}
		/// <summary>
		/// 
		/// </summary>
		protected string OutFileName { get; set; }
	}
}