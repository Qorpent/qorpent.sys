using System;
using System.IO;
using System.Linq;

namespace Qorpent.Data.DataDiff{
	/// <summary>
	/// Формаир
	/// </summary>
	public class SqlDiffGenerator{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		public SqlDiffGenerator(TableDiffGeneratorContext context){
			this._context = context;
			if (null == _context.Tables){
				throw new Exception("SqlDiffGenerator: tables are not defined");
			}
			if (null == _context.SqlOutput){
				_context.SqlOutput = Console.Out;
			}
			this._tables = _context.Tables.ToArray();
			this._output = _context.SqlOutput;
			
		}
		private DataDiffTable[] _tables;
		private TextWriter _output;
		private TableDiffGeneratorContext _context;

		/// <summary>
		/// Записать скрипт в переданный поток
		/// </summary>
		public void GenerateScript(){
			
			WriteStart();
			WriteBody();
			WriteFinish();
		}

		private void WriteFinish(){
			throw new NotImplementedException();
		}

		private void WriteBody(){
			throw new NotImplementedException();
		}

		private void WriteStart(){
			throw new NotImplementedException();
		}
	}
}