using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Qorpent.IO.DirtyVersion.Storage
{
	/// <summary>
	/// Хелпер формирования хэшей
	/// </summary>
	public class Hasher {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="length"></param>
		public Hasher(int length=Const.MaxHashSize) {
			_length = Math.Min(length, Const.MaxHashSize);
			_length = Math.Max(_length, Const.MinHashSize);
		}

		private int _length;
		
		private HashAlgorithm _internalhasher;

		private HashAlgorithm Internalhasher {
			get { return _internalhasher ?? (_internalhasher = MD5.Create()); }
		}

		/// <summary>
		/// Конвертирует вхдоящий поток в хэш размерностью MaxHashSize символов
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public string GetHash(Stream stream) {
			if (null == stream) throw new ArgumentNullException("stream");
			if (!stream.CanRead) throw new ArgumentException("stream is not readable","stream");
			var hash = Internalhasher.ComputeHash(stream);
			return ConvertHashToString(hash);
		}

		/// <summary>
		/// Возвращает хэш из строки
		/// </summary>
		/// <returns></returns>
		public string GetHash(string str) {
			if (string.IsNullOrWhiteSpace(str)) throw new ArgumentException("empty str given", "str");
			var bytes = Encoding.UTF8.GetBytes(str);
			return GetHash(bytes);
		}
		/// <summary>
		/// Возвращает хэш для массива
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns></returns>
		public string GetHash(byte[] bytes) {
			if (null == bytes) throw new ArgumentNullException("bytes");
			var hash = Internalhasher.ComputeHash(bytes);
			return ConvertHashToString(hash);
		}

		private  string ConvertHashToString(byte[] hash) {
			var sb = new StringBuilder();
			for (var i = 0; i < hash.Length; i++) {
				if (0 == i) {
					var val = hash[i].ToString("x2");
					if (char.IsDigit(val[0])) {

						val = ((char) (((int) val[0]) + 55)).ToString() + val[1].ToString();
					}
					sb.Append(val);
				}
				else {
					sb.Append(hash[i].ToString("x2"));
				}
			}
			return sb.ToString().Substring(0,_length);
		}
	}
}
