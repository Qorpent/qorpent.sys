using System.Text;
using System.Text.RegularExpressions;
using Qorpent.Utils.Extensions;

namespace Qorpent.Host.Utils
{
	/// <summary>
	///Вспомогоательный класс обхода MultipartForm данных
	/// </summary>
	internal class MiltipartReadContext
	{
		private byte[] _buffer;
		private byte[] _boundaryBytes;
		private int _start;
		private int _idx;
		private int _end;
		private string _currentName;
		private string _currentFileName;
		private string _currentType;
		private RequestParameterSet _result;
		private Encoding _encoding;

		public MiltipartReadContext(byte[] buffer, string contentType, Encoding contentEncoding, RequestParameterSet result)
		{
			_buffer = buffer;
			_encoding = contentEncoding;
			_currentType = "";
			_boundaryBytes = _encoding.GetBytes("--" + contentType.Split(';')[1].Split('=')[1]);
			_result = result;
			_start = 0;
			_idx = 0;
			_end = 0;
			_currentName = "";
			_currentFileName = "";
				
		}
		/// <summary>
		/// 
		/// </summary>
		public void Emit()
		{
			var data = new byte[_end - _idx];
			var delt = _idx;
			for (var i = _idx; i < _end; i++)
			{
				data[i - delt] = _buffer[i];
			}
			if (_currentType == "")
			{
				_result.Form[_currentName] = _encoding.GetString(data, 0, (int)data.Length);
			}
			else
			{
				var postFile = new PostFile
					{
						Content = data,
						ContentType = _currentType,
						FileName = _currentFileName,
						Name = _currentName
					};
				_result.Files[_currentName] = postFile;
			}
			_currentName = "";
			_currentType = "";
			_currentFileName = "";
		}

		public byte[] Buffer
		{
			get { return _buffer; }
		}

		public byte[] BoundaryBytes
		{
			get { return _boundaryBytes; }
		}

		public int Start
		{
			get { return _start; }
			set { _start = value; }
		}

		public int Idx
		{
			get { return _idx; }
			set { _idx = value; }
		}

		public int End
		{
			get { return _end; }
			set { _end = value; }
		}

		public string CurrentName
		{
			get { return _currentName; }
			set { _currentName = value; }
		}

		public string CurrentFileName
		{
			get { return _currentFileName; }
			set { _currentFileName = value; }
		}

		public string CurrentType
		{
			get { return _currentType; }
			set { _currentType = value; }
		}

		public void Read()
		{
			while (ReadNext()){}
		}

		private bool ReadNext()
		{
			if (0 == Start)
			{
				Start = Buffer.IndexOf(Idx, BoundaryBytes);
			}
			if (Start == -1)
			{
				return false;
			}
			Idx = Start + BoundaryBytes.Length;
			var next = Buffer.IndexOf(Idx, BoundaryBytes);


			if (next == -1)
			{
				return false;
			}

			End = next - 1;
			if (Buffer[End] == '\n' && Buffer[End - 1] == '\r') End--;

			while (Buffer[Idx] == '\r' || Buffer[Idx] == '\n')
			{
				Idx++;
			}

			var cdb = new byte[256];
			var delt = Idx;
			while (Buffer[Idx] != '\r' && Buffer[Idx] != '\n')
			{
				cdb[Idx - delt] = Buffer[Idx];
				Idx++;
			}
			var disposstring = _encoding.GetString(cdb, 0, Idx - delt);
			var dispmatch = Regex.Match(disposstring,
			                            "Content-Disposition:[^;]+;\\s*name\\s*=\\s*\"(?<n>[^\"]+)\"(;\\s*filename\\s*=\\s*\"(?<fn>[^\"]+)\")?");
			CurrentName = dispmatch.Groups["n"].Value;
			CurrentFileName = dispmatch.Groups["fn"].Value;
			var cnt = 0;
			bool wasn = false;
			while (Buffer[Idx] == '\r' || Buffer[Idx] == '\n')
			{
				if (Buffer[Idx] == '\n')
				{
					wasn = true;
				}
				if (Buffer[Idx] == '\n' || !wasn)
				{
					cnt++;
				}
				Idx++;
			}
			if (cnt == 1)
			{
				var ctb = new byte[100];
				delt = Idx;
				while (Buffer[Idx] != '\r' && Buffer[Idx] != '\n')
				{
					ctb[Idx - delt] = Buffer[Idx];
					Idx++;
				}
				var ctypestring = _encoding.GetString(ctb, 0, Idx - delt);
				CurrentType = ctypestring.Split(':')[1].Trim();
			}
			while (Buffer[Idx] == '\r' || Buffer[Idx] == '\n') Idx++;


			Emit();

			Start = next;
			return true;
		}
	}
}