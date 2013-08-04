using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.IO
{
	/// <summary>
	/// Источник файлов
	/// </summary>
	public interface IFileSource {
		/// <summary>
		/// Перечисляет имена файлов
		/// </summary>
		/// <returns></returns>
		IEnumerable<string> GetFileNames();
		/// <summary>
		/// Открывает поток на чтение
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		Stream Open(string name);
	}
}
