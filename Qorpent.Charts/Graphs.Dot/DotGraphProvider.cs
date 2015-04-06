using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Qorpent.IoC;

namespace Qorpent.Graphs.Dot
{
	/// <summary>
	/// Поставщик сервиса генерации графов
	/// </summary>
	[ContainerComponent(ServiceType = typeof(IGraphProvider),Name="dot.graph.provider")]
	public class DotGraphProvider:IGraphProvider
	{
		class DotClient: IDisposable {
			private const string ENDOFSVGMARKER = "</svg>";
			private const string PNGENDMARKER = "IEND";
			private const int INITIAL_PNG_BUFFER_SIZE = 8; //size of initial title and start of chank length;
			private const int CHUNK_LENGTH_BLOCK_SIZE = 4;
			private const int CHUNK_TYPE_BLOCK_SIZE = 4;
			private const int CHUNK_SRC_BLOCK_SIZE = 4;
			private Process _p;
			private GraphOptions _options;

			public DotClient(string script, GraphOptions options) {
				_p = EnvironmentInfo.GetConsoleProcess(options.Algorithm, "-T" + options.Format);
				_p.Start();
				_p.StandardInput.WriteLine(script);
				_options = options;
			}
			public void Dispose() {
				if (null != _p) {
					_p.StandardInput.Close();
				}
			}


			public object Generate() {
				if (_options.Format == GraphOptions.SVGFORMAT)
				{
					return Svg();
				}
				return Png();
			}

			private object Png() {
				var buffer = new byte[INITIAL_PNG_BUFFER_SIZE];
				Stream s = _p.StandardOutput.BaseStream;
				int idx = 0;
				idx += s.Read(buffer, 0, INITIAL_PNG_BUFFER_SIZE);
				while (true)
				{
					int al = 0;
					bool endchunk = false;
					var subbuf = new byte[CHUNK_LENGTH_BLOCK_SIZE];
					s.Read(subbuf, 0, CHUNK_LENGTH_BLOCK_SIZE);
					uint length = BitConverter.ToUInt32(subbuf.Reverse().ToArray(), 0);
					int expand = CHUNK_LENGTH_BLOCK_SIZE + CHUNK_TYPE_BLOCK_SIZE + (int)length + CHUNK_SRC_BLOCK_SIZE;
					Array.Resize(ref buffer, buffer.Length + expand);
					Array.Copy(subbuf, 0, buffer, idx, subbuf.Length);
					idx += subbuf.Length;
					s.Read(subbuf, 0, CHUNK_TYPE_BLOCK_SIZE);
					string str = Encoding.ASCII.GetString(subbuf);
					if (str == PNGENDMARKER)
					{
						endchunk = true;
					}
					Array.Copy(subbuf, 0, buffer, idx, subbuf.Length);
					idx += subbuf.Length;

					idx += al = s.Read(buffer, idx, (int)length);
					if (al < (int)length)
					{
						idx += s.Read(buffer, idx, (int)length - al);
					}
					idx += s.Read(buffer, idx, CHUNK_SRC_BLOCK_SIZE);
					if (endchunk)
					{
						break;
					}
				}
				return buffer;
			}

			private object Svg() {
				string result = "";
				string line = "";
				Stream s = _p.StandardOutput.BaseStream;
				var sr = new StreamReader(s, Encoding.UTF8);
				while (true)
				{
					var linereader = sr.ReadLineAsync();
				    var read = linereader.Wait(5000);
                    if (!read) {
                        throw new Exception("some errors in dot");
                    }
				    line = linereader.Result;
					result += line;
					if (line.Contains(ENDOFSVGMARKER)) break;
				}
				return result;
			}
		}
		/// <summary>
		/// Формирует контент графа
		/// </summary>
		/// <param name="script"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public object Generate(string script, GraphOptions options) {
			using (var dc = new DotClient(script, options)) {
				return dc.Generate();
			}
		}
	}
}
