using System.Collections.Generic;
using System.IO;
using Qorpent.Log;

namespace Qorpent.Data.DataDiff{
	/// <summary>
	/// Контекст выполнения генерации XDiffTable
	/// </summary>
	public class TableDiffGeneratorContext{
		/// <summary>
		/// 
		/// </summary>
		public TableDiffGeneratorContext(){
			InTransaction = true;
			IgnoreFields = new List<string>();
		}
		/// <summary>
		///Корневая директория репозитория
		/// </summary>
		public string RootDirectory { get; set; }

		/// <summary>
		/// Директория проекта
		/// </summary>
		public string ProjectDirectory { get; set; }

		/// <summary>
		/// Имя проекта
		/// </summary>
		public string ProjectName { get; set; }

		/// <summary>
		/// Директория с результирующими файлам
		/// </summary>
		public string OutputDirectory { get; set; }


		/// <summary>
		/// Прототип метаданных для прокачки
		/// </summary>
		public string BSharpPrototype { get; set; }

		/// <summary>
		/// Remote GIT Url
		/// </summary>
		public string GitUrl { get; set; }
		/// <summary>
		/// Базовая ревизия
		/// </summary>
		public string GitBaseRevision { get; set; }
		/// <summary>
		/// Ревизия с обновлением
		/// </summary>
		public string GitUpdateRevision { get; set; }

		/// <summary>
		/// Подготовленные пары XML для сравнения
		/// </summary>
		public IEnumerable<DiffPair> DiffPairs { get; set; } 
		/// <summary>
		/// Таблицы с измененными данными
		/// </summary>
		public IEnumerable<DataDiffTable> Tables { get; set; }
		/// <summary>
		/// Исходящий поток
		/// </summary>
		public TextWriter SqlOutput { get; set; }

		/// <summary>
		/// Признак необходимости использования транзакций
		/// </summary>
		public bool InTransaction { get; set; }
		/// <summary>
		/// Поля, которые игнорируются при формировании дифа
		/// </summary>
		public IList<string> IgnoreFields { get; private set; }
		/// <summary>
		/// Бранч, с которым работает сихронизатор
		/// </summary>
		public string GitBranch { get; set; }
		/// <summary>
		/// Журнал
		/// </summary>
		public IUserLog Log { get; set; }
	}
}