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
			MetadataTable = "qptmds.MDFile";
			BSharpPrototype = "db-meta";
			BSharpMapPrototype = "db-map";
			Mappings = new List<TableMap>();
			Indexes = new List<TableMap>();
		}
		/// <summary>
		/// Строка соединения с SQL
		/// </summary>
		public string SqlConnectionString { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string MetadataTable { get; set; }
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
		/// <summary>
		/// Кэшированный SQL скрипт
		/// </summary>
		public string SqlScript { get; set; }
		/// <summary>
		/// Результирующая ревизия обновления
		/// </summary>
		public string ResolvedUpdateRevision { get; set; }
		/// <summary>
		/// Прототип мапингов для диффа
		/// </summary>
		public string BSharpMapPrototype { get; set; }
		/// <summary>
		/// Определения схемы
		/// </summary>
		public IList<TableMap> Mappings { get; private set; }
		/// <summary>
		/// Мапинги отключаемых индексов
		/// </summary>
		public IList<TableMap> Indexes { get; private set; }
		/// <summary>
		/// Запрет на применение SQL к БД
		/// </summary>
		public bool NoApply { get; set; }
		/// <summary>
		/// Признак необходимости полного апдейта, независимо от наличия дельты с прошлым апдейтом (force-update)
		/// </summary>
		public bool FullUpdate { get; set; }
		/// <summary>
		/// Только регистрация версии
		/// </summary>
		public bool OnlyRegister { get; set; }
	}
}