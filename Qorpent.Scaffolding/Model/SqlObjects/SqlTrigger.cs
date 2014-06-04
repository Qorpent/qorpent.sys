using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Model.SqlObjects{
	/// <summary>
	/// Обертка над SQL Триггером
	/// </summary>
	public class SqlTrigger : SqlObject{
		/// <summary>
		/// Формирует SQL-объект
		/// </summary>
		/// <param name="model"></param>
		/// <param name="cls"></param>
		/// <param name="bscls"></param>
		/// <param name="xml"></param>
		/// <returns></returns>
		public override SqlObject Setup(PersistentModel model, PersistentClass cls, BSharp.IBSharpClass bscls, System.Xml.Linq.XElement xml)
		{
			base.Setup(model, cls, bscls, xml);
			this.TableName = cls.FullSqlName;
			this.Name = xml.Attr("code");
			this.Schema = cls.Schema;
			this.Insert = xml.GetSmartValue("insert").ToBool();
			this.Update = xml.GetSmartValue("update").ToBool();
			this.Delete = xml.GetSmartValue("delete").ToBool();
			this.Before = xml.GetSmartValue("before").ToBool();
			this.External = xml.GetSmartValue("external");
			this.ExternalBody = xml.GetSmartValue("externalbody");
			this.Dialect = SqlDialect.Ansi;
			var dialect = xml.GetSmartValue("dialect");
			if (!string.IsNullOrWhiteSpace(dialect)){
				this.Dialect = dialect.To<SqlDialect>();
			}
			this.Body = xml.Value;
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		public bool Before { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public bool Delete { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public bool Update { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public bool Insert { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string TableName { get; set; }
	}
}