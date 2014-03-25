using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.Data.MetaDataBase
{
	/// <summary>
	/// Регистрирует файлы в банке метаданных
	/// </summary>
	public interface IMetaFileRegistry{
		/// <summary>
		/// Дописывает (при отсутствии файл в список ревийзий)
		/// </summary>
		/// <param name="descriptor"></param>
		MetaFileDescriptor Register(MetaFileDescriptor descriptor);

		/// <summary>
		/// Устанавливает указанную ревизию в качестве текущей (не требует контента)
		/// </summary>
		/// <param name="descriptor"></param>
		MetaFileDescriptor Checkout(MetaFileDescriptor descriptor);
		/// <summary>
		/// Получить ревизии файла по коду
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		IEnumerable<RevisionDescriptor> GetRevisions(string code);
		/// <summary>
		/// Возвращает текущий файл по коду
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		MetaFileDescriptor GetCurrent(string code);

		/// <summary>
		/// Считывает указанную ревизию
		/// </summary>
		/// <param name="descriptor"></param>
		/// <returns></returns>
		MetaFileDescriptor GetByRevision(MetaFileDescriptor descriptor);

		/// <summary>
		/// Прочитать набор кодов
		/// </summary>
		/// <returns></returns>
		IEnumerable<string> GetCodes(string prefix =null);
		/// <summary>
		/// Проверяет наличие файла с кодом
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		bool Exists(string code);

		/// <summary>
		/// Производит некую перезагрузку, очистку, по умолчанию не делает ничего
		/// </summary>
		void Refresh();
	}
}
