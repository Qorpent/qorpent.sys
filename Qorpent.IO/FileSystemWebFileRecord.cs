using System;
using System.IO;

namespace Qorpent.IO{
	/// <summary>
	/// ��������� ����� �� �������� �������
	/// </summary>
	public class FileSystemWebFileRecord:WebFileRecord{
		/// <summary>
		/// ��� ����� � �������� �������
		/// </summary>
		public string FileSystemName { get; set; }

	    public override long Length {
	        get { return new FileInfo(FileSystemName).Length; }
	        set { throw new NotImplementedException(); }
	    }

	    /// <summary>
		/// ��������� ������ �� �����
		/// </summary>
		/// <returns></returns>
		protected override DateTime GetVersion(){
			return File.GetLastWriteTime(FileSystemName);
		}
		
		/// <summary>
		/// ���������� ���� � ������� �����
		/// </summary>
		/// <param name="output"></param>
		public override long Write(Stream output){
			using (var fs = File.OpenRead(FileSystemName)){
				fs.CopyTo(output);
				return  fs.Length;
			}
			
		}

		/// <summary>
		/// �������� ������ �� ������
		/// </summary>
		/// <returns></returns>
		public override Stream Open(){
			return File.Open(FileSystemName,FileMode.Open,FileAccess.Read,FileShare.ReadWrite);
		}
	}
}