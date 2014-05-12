using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qorpent.Data.Connections;
using Qorpent.Utils.Extensions;

namespace Qorpent.Data.DataDiff
{
	/// <summary>
	/// Определяет правильную исходную версию для сравнения по проекту
	/// </summary>
	public class SqlSourceRevisionRetriever
	{
		private TableDiffGeneratorContext _context;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		public SqlSourceRevisionRetriever(TableDiffGeneratorContext context){
			if (string.IsNullOrWhiteSpace(context.ProjectName)){
				throw new Exception("Require ProjectName to be set in context");
			}
			
			if (string.IsNullOrWhiteSpace(context.MetadataTable)){
				context.MetadataTable = "qptmds.MDFile";
			}
			if (string.IsNullOrWhiteSpace(context.SqlConnectionString)){
				context.SqlConnectionString =
					"Data Source=(local);Initial Catalog=z3;Integrated Security=True;Application Name=z3test";
			}
			
			this._context = context;
		}
		/// <summary>
		/// 
		/// </summary>
		public void Determine(){
			using (var connection = new DatabaseConnectionProvider().GetConnection(_context.SqlConnectionString)){
				connection.Open();
				var exproc = connection.ExecuteScalar<int>("select object_id('" + _context.MetadataTable + "')");
				if (0 == exproc){
					throw new Exception("В целевой БД отсутвует таблица "+_context.MetadataTable);
				}
				var currev =
					connection.ExecuteScalar<string>("select ActiveRevisionRevision from " + _context.MetadataTable +
					                                 "Full where Code='" + _context.ProjectName + ".project'");
				_context.GitBaseRevision = currev;
			}
		}
	}
}
