using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Model.SqlObjects{
	/// <summary>
	///     Обертка над SQL Триггером
	/// </summary>
	public class SqlTrigger : SqlObject{
		/// <summary>
		/// </summary>
		public SqlTrigger(){
			UseTablePrefixedName = true;
		}

		/// <summary>
		/// </summary>
		public bool Before { get; set; }

		/// <summary>
		/// </summary>
		public bool Delete { get; set; }

		/// <summary>
		/// </summary>
		public bool Update { get; set; }

		/// <summary>
		/// </summary>
		public bool Insert { get; set; }

		/// <summary>
		///     Формирует SQL-объект
		/// </summary>
		/// <param name="model"></param>
		/// <param name="cls"></param>
		/// <param name="bscls"></param>
		/// <param name="xml"></param>
		/// <returns></returns>
		public override SqlObject Setup(PersistentModel model, PersistentClass cls, IBSharpClass bscls, XElement xml){
			base.Setup(model, cls, bscls, xml);
			TableName = cls.FullSqlName;
			Name = xml.Attr("code");
			Schema = cls.Schema;
			Insert = xml.GetSmartValue("insert").ToBool();
			Update = xml.GetSmartValue("update").ToBool();
			Delete = xml.GetSmartValue("delete").ToBool();
			Before = xml.GetSmartValue("before").ToBool();
			return this;
		}
	}
}