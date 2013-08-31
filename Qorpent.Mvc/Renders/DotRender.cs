using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Qorpent.Mvc.Renders {
	/// <summary>
	/// Render для отрисовки графиков DOT
	/// </summary>
	[Render("dot")]
	public class DotRender : RenderBase {
		private string _dotpath;

		/// <summary>
		/// 	Renders given context
		/// </summary>
		/// <param name="context"> </param>
		public override void Render(IMvcContext context)
		{
			var dotscript = ExtractDotScript(context);
			var format = context.Get("__format", "svg");
			context.ContentType = ReolveContentType(format);
			var p = GetProcess(format);
			p.Start();
			try {
				p.StandardInput.WriteLine(dotscript);
				if (format == "svg") {
					WriteOutSvg(context, p);
				}
				else {
					WriteOutPng(context, p);
				}
			}
			finally {
				p.StandardInput.Close();
			}
		}

		private static void WriteOutPng(IMvcContext context, Process p) {
			const int INITIAL_PNG_BUFFER_SIZE = 8; //size of initial title and start of chank length;
			const int CHUNK_LENGTH_BLOCK_SIZE = 4;
			const int CHUNK_TYPE_BLOCK_SIZE = 4;
			const int CHUNK_SRC_BLOCK_SIZE = 4;
			var buffer = new byte[INITIAL_PNG_BUFFER_SIZE];
			var s = p.StandardOutput.BaseStream;
			var idx = 0;
			idx += s.Read(buffer, 0, INITIAL_PNG_BUFFER_SIZE);		
			while (true) {
				int al = 0;
				bool endchunk = false;
				var subbuf = new byte[CHUNK_LENGTH_BLOCK_SIZE];
				s.Read(subbuf, 0, CHUNK_LENGTH_BLOCK_SIZE);
				var length = BitConverter.ToUInt32(subbuf.Reverse().ToArray(),0);
				var expand = CHUNK_LENGTH_BLOCK_SIZE + CHUNK_TYPE_BLOCK_SIZE + (int)length + CHUNK_SRC_BLOCK_SIZE;
				Array.Resize(ref buffer, buffer.Length + expand);
				Array.Copy(subbuf, 0, buffer, idx, subbuf.Length);
				idx += subbuf.Length;
				s.Read(subbuf, 0, CHUNK_TYPE_BLOCK_SIZE);
				var str = Encoding.ASCII.GetString(subbuf);
				if (str == "IEND")
				{
					endchunk = true;
				}
				Array.Copy(subbuf, 0, buffer, idx, subbuf.Length);
				idx += subbuf.Length;

				idx += al = s.Read(buffer, idx, (int)length);
				if (al < (int) length) {
					idx += s.Read(buffer, idx, (int)length-al);	
				}
				idx += s.Read(buffer, idx, CHUNK_SRC_BLOCK_SIZE);
				if (endchunk) {
					break;
				}
			}
			context.WriteOutBytes(buffer);
		}

		private static void WriteOutSvg(IMvcContext context, Process p) {
			var result = "";
			var line = "";
			while (true) {
				line = p.StandardOutput.ReadLine();
				result += line;
				if (line.Contains("</svg>")) break;
			}
			context.Output.Write(result);
		}

		private Process GetProcess(string format) {
			var callargs = "-T" + format;
			var process_start = new ProcessStartInfo(ResolveDotPath(), callargs) {
				UseShellExecute = false,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardInput = true
			};
			var p = new Process {StartInfo = process_start};
			return p;
		}

		private string ReolveContentType(string format) {
			if ("svg" == format) return "image/svg+xml";
			if ("png" == format) return "image/png";
			throw new Exception("unknown format " + format);
		}

		/// <summary>
		/// 	Renders error, occured in given context
		/// </summary>
		/// <param name="error"> </param>
		/// <param name="context"> </param>
		public override void RenderError(Exception error, IMvcContext context) {

			context.ContentType = "text/plain";
			context.Output.Write(error.ToString());

		}

		private string ResolveDotPath() {
			if (null != _dotpath) {
				return _dotpath;
			}
			var paths = Environment.GetEnvironmentVariable("PATH").Split(';');
			foreach (var p in paths) {
				var realp = Environment.ExpandEnvironmentVariables(p.Trim());
				var checkpath = Path.Combine(realp, "dot.exe");
				if (File.Exists(checkpath)) {
					return _dotpath = checkpath;
				}
			}
			throw new Exception("cannot find dot.exe");
		}

		private string ExtractDotScript(IMvcContext context) {
			var result = context.ActionResult;
			var dotscript = "";
			if (null == result) {
				dotscript = "digraph N{ null }";
			}
			else if (result is string) {
				dotscript = (string) result;
			}
			else {
				dotscript = ConvertToDotScript(result);
			}
			return dotscript;
		}

		private string ConvertToDotScript(object result) {
			throw new NotImplementedException("for now any-to-dot mode is not realized");
		}
	}
}