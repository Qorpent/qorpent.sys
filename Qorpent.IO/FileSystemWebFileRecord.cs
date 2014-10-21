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
	}
}