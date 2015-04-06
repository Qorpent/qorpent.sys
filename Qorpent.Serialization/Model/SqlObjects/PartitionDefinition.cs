using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Data;

namespace Qorpent.Scaffolding.Model.SqlObjects{
	/// <summary>
	///     Определение схемы и функции партицирования
	/// </summary>
	public class PartitionDefinition : SqlObject{
		/// <summary>
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// </summary>
		public string Start { get; set; }

		/// <summary>
		/// </summary>
		public string FileGroup { get; set; }

		/// <summary>
		/// </summary>
		protected AllocationInfo Allocation { get; set; }

		/// <summary>
		/// </summary>
		/// <param name="model"></param>
		/// <param name="cls"></param>
		/// <param name="bscls"></param>
		/// <param name="xml"></param>
		public override SqlObject Setup(PersistentModel model, PersistentClass cls, IBSharpClass bscls, XElement xml){
			base.Setup(model, cls, bscls, xml);
			Allocation = cls.AllocationInfo;
			FileGroup = Allocation.FileGroup.Name;
			Name = cls.FullSqlName.Replace(".", "_").Replace("\"","") + "_PARTITION";
			Start = Allocation.PartitioningStart;
			if (null != Allocation.PartitionField){
				Type = Allocation.PartitionField.DataType.ResolveSqlDataType(SqlDialect.SqlServer);
			}
			else{
				Type = "int";
			}
			return this;
		}
	}
}