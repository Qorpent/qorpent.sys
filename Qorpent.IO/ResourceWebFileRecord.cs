using System;
using System.IO;
using System.Reflection;

namespace Qorpent.IO{
	/// <summary>
	/// ��������� ����� �� �������� �������
	/// </summary>
	public class ResourceWebFileRecord : WebFileRecord
	{
		/// <summary>
		/// ��� ����� � �������� �������
		/// </summary>
		public string ResourceName { get; set; }
		/// <summary>
		/// ������� ������, �������� �������
		/// </summary>
		public Assembly Assembly { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override DateTime GetVersion(){
			return File.GetCreationTime(Assembly.Location);
		}

		/// <summary>
		/// ���������� ���� � ������� �����
		/// </summary>
		/// <param name="output"></param>
		public override long Write(Stream output)
		{
			using (var fs = Assembly.GetManifestResourceStream(ResourceName))
			{
				fs.CopyTo(output);
				return fs.Length;
			}

		}
	}
}