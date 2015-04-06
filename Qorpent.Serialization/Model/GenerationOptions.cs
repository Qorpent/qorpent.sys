using Qorpent.Data;

namespace Qorpent.Scaffolding.Model{
	/// <summary>
	/// </summary>
	public class GenerationOptions{
		/// <summary>
		/// </summary>
		public GenerationOptions(){
			IncludeSqlObjectTypes = SqlObjectType.All;
			ExcludeSqlObjectTypes = SqlObjectType.None;
			IncludeDialect = SqlDialect.SqlServer | SqlDialect.PostGres;
			GenerateCreateScript = true;
			GenerateDropScript = true;
			GeneratePartitions = true;
		}

		/// <summary>
		///     Типы объектов, участвующие в генерации
		/// </summary>
		public SqlObjectType IncludeSqlObjectTypes { get; set; }

		/// <summary>
		///     Типы объектов, не участвующие в генерации
		/// </summary>
		public SqlObjectType ExcludeSqlObjectTypes { get; set; }

		/// <summary>
		///     Диалекты генерации
		/// </summary>
		public SqlDialect IncludeDialect { get; set; }

		/// <summary>
		///     Признак генерации скрипта создания БД
		/// </summary>
		public bool GenerateCreateScript { get; set; }

		/// <summary>
		///     Признак генерации скрипта удаления БД
		/// </summary>
		public bool GenerateDropScript { get; set; }

		/// <summary>
		///     Признак генерации C#
		/// </summary>
		public bool GenerateCSharpModel { get; set; }

		/// <summary>
		///     Формировать партицириование (SqlServer)
		/// </summary>
		public bool GeneratePartitions { get; set; }

		/// <summary>
		///     Проверяет учет объектов указанного типа в скрипте
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public bool Supports(SqlObjectType type){
			if (IncludeSqlObjectTypes.HasFlag(type)) return true;
			if (ExcludeSqlObjectTypes.HasFlag(type)) return false;
			if (IncludeSqlObjectTypes != SqlObjectType.All) return false;
			if (ExcludeSqlObjectTypes == SqlObjectType.All) return false;
			return true;
		}
	}
}