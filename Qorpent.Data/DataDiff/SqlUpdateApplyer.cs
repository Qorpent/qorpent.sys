using System;
using Qorpent.Data.Connections;
using Qorpent.Utils.Extensions;

namespace Qorpent.Data.DataDiff{
	/// <summary>
	/// Обновляет версию по проекту
	/// </summary>
	public class SqlUpdateApplyer{
		private TableDiffGeneratorContext _context;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		public SqlUpdateApplyer(TableDiffGeneratorContext context)
		{
			if (string.IsNullOrWhiteSpace(context.ProjectName)){
				throw new Exception("Require ProjectName to be set in context");
			}
			if (string.IsNullOrWhiteSpace(context.SqlScript)){
				throw new Exception("Require SqlScript to be set in context");
			}
			if (string.IsNullOrWhiteSpace(context.ResolvedUpdateRevision))
			{
				throw new Exception("Require ResolvedUpdateRevision to be set in context");
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
		public void Update()
		{
			using (var connection = new DatabaseConnectionProvider().GetConnection(_context.SqlConnectionString))
			{
				connection.Open();

				var exproc = connection.ExecuteScalar<int>("select object_id('" + _context.MetadataTable + "Register')");
				if (0 == exproc)
				{
					throw new Exception("В целевой БД отсутвует процедура " + _context.MetadataTable+"Register");
				}
				//сначала надо прорегистрировать скрипт, и только потом его выполнять, так как на него могут идти ссылки
				var register = "exec " + _context.MetadataTable +
							   "Register @code=@code,@name=@code,@content=@content, @hash=@hash,@revision=@revision,@filetime=@filetime,@comment=@comment";

				connection.ExecuteNonQuery(register, new { code = _context.ProjectName + ".project",comment=_context.FullUpdate||!_context.GitBaseRevision.ToBool()?"full":"diff", content = _context.SqlScript, filetime = DateTime.Now, hash = _context.ResolvedUpdateRevision, revision = _context.ResolvedUpdateRevision.Substring(0, 7) });
				if (_context.OnlyRegister){
					_context.Log.Trace("Включен режим 'только регистрация', сам скрипт обновления не выполняется");
				}
				else{
					connection.ExecuteNonQuery(_context.SqlScript, timeout: 30000);
				}

			}
		}
	}
}