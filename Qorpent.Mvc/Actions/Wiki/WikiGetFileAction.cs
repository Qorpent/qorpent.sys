using System;
using Qorpent.Mvc.Binding;

namespace Qorpent.Mvc.Actions {
	/// <summary>
	/// ��������, ������������ �������������� ���� WIKI
	/// </summary>
	[Action("wiki.getfile",Help="�������� �������� ������� �� Wiki")]
	public class WikiGetFileAction : WikiActionBase {
		/// <summary>
		/// ��� �����
		/// </summary>
		[Bind(Required = true)]
		public string Code;
		/// <summary>
		/// ������� ���� ��� ���� ������ �� ������ ������������, �� ������ �����������
		/// </summary>
		[Bind]
		public bool AsFile { get; set; }

		/// <summary>
		/// 	override if Yr action provides 304 state and return TRUE
		/// </summary>
		/// <returns> </returns>
		protected override bool GetSupportNotModified() {
			return true;
		}

		/// <summary>
		/// 	override if Yr action provides 304 state  and return Last-Modified-State header
		/// </summary>
		/// <returns> </returns>
		protected override System.DateTime EvalLastModified() {
			var file = WikiSource.LoadBinary(Code, false);
			if (null == file) return DateTime.MinValue;
			return file.LastWriteTime;
		}
		

		/// <summary>
		/// 	processing of execution - main method of action
		/// </summary>
		/// <returns> </returns>
		protected override object MainProcess() {
			var file = WikiSource.LoadBinary(Code, true);
			if (null == file) {
				throw new Exception("file not found");
			}
			var filedesc = new FileDescriptor();
			filedesc.NeedDisposition = AsFile;
			
			filedesc.MimeType = file.MimeType;
			filedesc.Data = file.Data;
			filedesc.Length = (int)file.Size;
			return filedesc;
		}
		
	}
}