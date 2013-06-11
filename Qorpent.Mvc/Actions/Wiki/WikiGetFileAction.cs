using System;
using Qorpent.Mvc.Binding;
using Qorpent.Wiki;

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
		/// �������� ����������� ������������� ��������� ������
		/// </summary>
		[Bind(Default = true)]
		public bool WithData { get; set; }

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
		protected override DateTime EvalLastModified() {
			return WikiSource.GetVersion(Code, WikiObjectType.File);
		}
		

		/// <summary>
		/// 	processing of execution - main method of action
		/// </summary>
		/// <returns> </returns>
		protected override object MainProcess() {
			var file = WikiSource.LoadBinary(Code, WithData);
			if (null == file) {
				throw new Exception("file not found");
			}
			var filedesc = new FileDescriptor();
			filedesc.NeedDisposition = AsFile;
			filedesc.LastWriteTime = file.LastWriteTime;
			filedesc.MimeType = file.MimeType;
			filedesc.Data = file.Data;
			filedesc.Length = (int)file.Size;
			return filedesc;
		}
		
	}
}