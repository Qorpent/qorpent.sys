using System.Collections.Generic;

namespace Qorpent.Data.MetaDataBase{
	/// <summary>
	/// Выполняет обработку файлов
	/// </summary>
	public interface IMetaFileProcessor{
		/// <summary>
		/// Встраивает выявленное изменение в общий лог
		/// </summary>
		void Prepare(string sqlconnection, IEnumerable<DatabaseUpdateRecord> givendelta, LinkedList<DatabaseUpdateRecord> workinglist, LinkedList<DatabaseUpdateRecord> errorlist);
		/// <summary>
		/// Выполняет обновления
		/// </summary>
		/// <param name="sqlconnection"></param>
		/// <param name="records"></param>
		void Execute(string sqlconnection, IEnumerable<DatabaseUpdateRecord> records);
	}
}