using System.Collections.Generic;
using Qorpent.Config;
using Qorpent.Log;
using Qorpent.Mvc;

namespace Qorpent.IntermediateFormat{
	/// <summary>
	/// ������ �� ������������� ������
	/// </summary>
	public  class IntermediateFormatQuery : ConfigBase,IConfigurableFromDictionary{
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
		/// ������� ���������������� ��������� � ������������
		/// </summary>
		/// <param name="args"></param>
		public virtual void Setup(IDictionary<string, string> args){
			foreach (var arg in args){
				var key = arg.Key.ToLowerInvariant();
				if (!Exists(key)){
					Set(key,arg.Value);
				}
			}
		}
	}
}