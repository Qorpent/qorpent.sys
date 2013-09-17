using System;
using System.IO;
using System.IO.Compression;

namespace Qorpent.IO.DirtyVersion.Storage {
	/// <summary>
	/// Поток, который при чтении пишет в другой файл
	/// </summary>
	public class CopyOnReadStream : Stream {
		private Stream _source;
		private Stream _target;
		/// <summary>
		/// Создает прокси-поток в привязке к файлу
		/// </summary>
		public CopyOnReadStream(Stream source, string filename, bool compress=false) {
			_source = source;
			Directory.CreateDirectory(Path.GetDirectoryName(filename));
			_target = new FileStream(filename, FileMode.Create);
			if (compress) {
				_target = new GZipStream(_target, CompressionMode.Compress, false);
			}
		}

		/// <summary>
		/// Создает поток нацеленный на один входящий и один исходящий поток
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		public CopyOnReadStream(Stream source, Stream target) {
			_source = source;
			_target = target;
		}
		/// <summary>
		/// flushes target stream
		/// </summary>
		public override void Flush() {
			_target.Flush();
		}
		/// <summary>
		/// not supported
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="origin"></param>
		/// <returns></returns>
		public override long Seek(long offset, SeekOrigin origin) {
			throw new NotSupportedException();
		}
		/// <summary>
		/// not supported
		/// </summary>
		/// <param name="value"></param>
		public override void SetLength(long value) {
			throw new NotSupportedException();
		}
		/// <summary>
		/// read from source and side-effec - it's wrote to target
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public override int Read(byte[] buffer, int offset, int count) {
			var actual = _source.Read(buffer, offset, count);
			_target.Write(buffer, offset, actual);
			_target.Flush();
			return actual;
		}

		/// <summary>
		/// Cannot write
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		public override void Write(byte[] buffer, int offset, int count) {
			throw new NotSupportedException();
		}
		/// <summary>
		/// It can read
		/// </summary>
		public override bool CanRead {
			get { return true; }
		}
		/// <summary>
		/// It cannot seek
		/// </summary>
		public override bool CanSeek {
			get { return false; }
		}
		/// <summary>
		/// It cannot write
		/// </summary>
		public override bool CanWrite {
			get { return false; }
		}
		/// <summary>
		/// source stream length
		/// </summary>
		public override long Length {
			get { return _source.Length; }
		}
		/// <summary>
		/// source position,not seter!
		/// </summary>
		public override long Position {
			get { return _source.Position; }
			set { throw new NotSupportedException(); }
		}
		/// <summary>
		/// closes both
		/// </summary>
		public override void Close() {
			_source.Close();
			_target.Close();
		}
	}
}