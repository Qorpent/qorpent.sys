using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qorpent.Log;

namespace Qorpent.Data.DataDiff
{
	/// <summary>
	/// 
	/// </summary>
	public class DiffExecutor
	{
		private TableDiffGeneratorContext _context;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		public DiffExecutor(TableDiffGeneratorContext context){
			_context = context;
		}
		/// <summary>
		/// Выполняет весь цикл, включая обращение к БД
		/// </summary>
		public void Execute(){
			_context.Log = ConsoleLogWriter.CreateLog("main",customFormat:"${Message}");
			if (_context.DiffPairs == null){
				_context.Log.Trace("require diff checking");
				if (!string.IsNullOrWhiteSpace(_context.SqlConnectionString) && string.IsNullOrWhiteSpace(_context.GitBaseRevision)){
					_context.Log.Trace("start revision determining");
					new SqlSourceRevisionRetriever(_context).Determine();
					_context.Log.Trace("base revision detected as " + _context.GitBaseRevision);
				}
				else{
					_context.Log.Trace("sql server not set up - work for script gen only");
				}
				new DiffPairGenerator(_context).Generate();
				_context.Log.Trace("difference pairs prepared evaluated");
			}
			new DataTableDiffGenerator(_context).Generate();
			_context.Log.Trace("difference tables generated: "+_context.Tables.Count()+" tables");
			if (_context.Tables.Any()){
				new SqlDiffGenerator(_context).Generate();
				_context.Log.Trace("sql script prepared");
				if (!string.IsNullOrWhiteSpace(_context.SqlConnectionString)){
					new SqlUpdateApplyer(_context).Update();
					_context.Log.Trace("sql database updated");
				}
				else{
					_context.Log.Trace("sql not set up - no real apply to server");	
				}
			}
			else{
				_context.Log.Trace("Нет значимых изменений");
			}
		}
	}
}
