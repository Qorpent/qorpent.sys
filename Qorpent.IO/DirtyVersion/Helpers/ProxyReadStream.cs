using System;
using System.IO;

namespace Qorpent.IO.DirtyVersion.Helpers {
	/// <summary>
	/// Поток, который при чтении пишет в другой файл
	/// </summary>
	public class ProxyReadStream : Stream {
		private Stream _source;
		private Stream _target;
		/// <summary>
		/// Создает прокси-поток в привязке к файлу
		/// </summary>
		/// <param name="source"></param>
		/// <param name="filename"></param>
		public ProxyReadStream(Stream source, string filename) {
			_source = source;
			Directory.CreateDirectory(Path.GetDirectoryName(filename));
			_target = new FileStream(filename, FileMode.Create);
		}

		/// <summary>
		/// Создает поток нацеленный на один входящий и один исходящий поток
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		public ProxyReadStream(Stream source, Stream target) {
			_source = source;
			_target = target;
		}

		public override void Flush() {
			_target.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin) {
			throw new NotSupportedException();
		}

		public override void SetLength(long value) {
			throw new NotSupportedException();
		}

		public override int Read(byte[] buffer, int offset, int count) {
			var actual = _source.Read(buffer, offset, count);
			_target.Write(buffer, offset, actual);
			_target.Flush();
			return actual;
		}

		public override void Write(byte[] buffer, int offset, int count) {
			throw new NotSupportedException();
		}

		public override bool CanRead {
			get { return true; }
		}

		public override bool CanSeek {
			get { return false; }
		}

		public override bool CanWrite {
			get { return false; }
		}

		public override long Length {
			get { return _source.Length; }
		}

		public override long Position {
			get { return _source.Position; }
			set { throw new NotSupportedException(); }
		}

		public override void Close() {
			_source.Close();
			_target.Close();
		}
	}
}