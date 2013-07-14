using System;
using System.IO;
using System.Web;
using Qorpent.IO;
using Qorpent.Mvc.Binding;
using Qorpent.Wiki;

namespace Qorpent.Mvc.Actions {
	/// <summary>
	/// Сохраняет контент в Wiki
	/// </summary>
	[Action("wiki.savefile", Help = "Загрузить бинарный контент в Wiki", Role = "DOCWRITER", Arm="sys")]
	public class WikiSaveFileAction : WikiActionBase {
		private HttpPostedFileBase _file;

		/// <summary>
		/// Код файла
		/// </summary>
		[Bind(Required = true)]public string Code { get; set; }
		/// <summary>
		/// Имя файла
		/// </summary>
		[Bind(Required = false)]public string Title { get; set; }
		/// <summary>
		/// 	First phase of execution - override if need special input parameter's processing
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize();
			_file = (HttpPostedFileBase)Context.GetFile("datafile");
		}

		/// <summary>
		/// 	Second phase - validate INPUT/REQUEST parameters here - it called before PREPARE so do not try validate
		/// 	second-level internal state and authorization - only INPUT PARAMETERS must be validated
		/// </summary>
		protected override void Validate()
		{
			base.Validate();
			if (null == _file)
			{
				throw new Exception("not file provided");
			}
		}

		/// <summary>
		/// Сохраняет файл в Wiki
		/// </summary>
		/// <returns></returns>
		protected override object MainProcess() {
			var srcname = _file.FileName;
			var ext = Path.GetExtension(srcname);
			var mime = MimeHelper.GetMimeByExtension(ext);
			var binobj = new WikiBinary();
			binobj.Code = Code;
			binobj.Title = string.IsNullOrWhiteSpace(Title) ? Code : Title;
			binobj.MimeType = mime;
			var data = new byte[_file.ContentLength];
			using (var s = _file.InputStream) {
				s.Read(data, 0, (int)s.Length);
			}
			binobj.Data = data;
			WikiSource.SaveBinary(binobj);
			return true;
		}
	}
}