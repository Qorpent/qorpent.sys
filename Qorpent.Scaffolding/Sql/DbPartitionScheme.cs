using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Sql{
	/// <summary>
	/// 
	/// </summary>
	public class DbPartitionScheme:DbObject{
		/// <summary>
		/// 
		/// </summary>
		public DbPartitionScheme()
		{
			this.ObjectType = DbObjectType.PartitionScheme;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		protected override IEnumerable<DbObject> Setup(XElement e)
		{
			var result =  base.Setup(e).ToArray();
			this.UsePartition = true;
			this.PartitionField = e.Attr("with");
			this.PartitionStep = e.Attr("step");
			this.PartitionStart = e.Attr("start");
			return result;
		}

		/// <summary>
		/// Изначальное разбиение
		/// </summary>
		public string PartitionStart { get; set; }

		/// <summary>
		/// Тип шага партиции
		/// </summary>
		public string PartitionStep { get; set; }

		/// <summary>
		/// Имя партицируемого поля
		/// </summary>
		public string PartitionField { get; set; }

		/// <summary>
		/// Признак партицируемой таблицы
		/// </summary>
		public bool UsePartition { get; set; }
	}
}