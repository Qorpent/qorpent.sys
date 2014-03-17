using System;

namespace Qorpent.Scaffolding.Sql{
	/// <summary>
	/// Фабричный класс для языковой поддержки
	/// </summary>
	public static class SqlProvider {
		private static ISqlProvider tsql = new TSQLProvider();
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dialect"></param>
		/// <returns></returns>
		public static ISqlProvider Get(DbDialect dialect){
			if (dialect == DbDialect.TSQL){
				return tsql;
			}
			throw new Exception("cannot find support class for this language");
		}
	}
}