using System.Collections.Generic;
using Qorpent.Log;
using Qorpent.Mvc;

namespace Qorpent.IntermediateFormat{
	/// <summary>
	/// ������ �� ������������� ������
	/// </summary>
	public  class IntermediateFormatQuery : Scope{
		/// <summary>
		///		����������, �������� �� ������ ����������
		/// </summary>
		/// <returns>������� ������������ �������</returns>
		public virtual ValidationResult GetIsValid(){
			return ValidationResult.OK;
		}

		/// <summary>
		///		������
		/// </summary>
		public IntermediateFormatOutputType Format { 
			get { return Get<IntermediateFormatOutputType>("format"); }
			set { Set("format",value); }
		}

		/// <summary>
		/// ������� ������� ������� ��������� �������
		/// </summary>
		public LogLevel LogLevel {
			get { return Get<LogLevel>("loglevel"); }
			set { Set("loglevel", value); }
		}

		/// <summary>
		/// ������ ������� ������� ��������� �������
		/// </summary>
		public string LogFormat {
			get { return Get<string>("logformat"); }
			set { Set("logformat", value); }
		}

		/// <summary>
		/// ������ ������� ������� ��������� �������
		/// </summary>
		public IntermediateFormatLayer Layers
		{
			get { return Get<IntermediateFormatLayer>("layers"); }
			set { Set("layers", value); }
		}

		/// <summary>
		/// ���� XSLT ��� ���������� �������� ��� �������� � XML
		/// </summary>
		public string Xslt
		{
			get { return Get<string>("xslt"); }
			set { Set("xslt", value); }
		}

	}
}