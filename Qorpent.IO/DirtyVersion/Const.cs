using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.IO.DirtyVersion
{
	/// <summary>
	/// Содержит константы DirtyVersion
	/// </summary>
	public static class Const
	{
		/// <summary>
		/// Размерность хэша Md5 в байтах
		/// </summary>
		internal const int Md5HashSizeInBytes = 128/8;

		/// <summary>
		/// Размерность хэша
		/// </summary>
		public const int HashSize = Md5HashSizeInBytes * 2;
		
	}
}
