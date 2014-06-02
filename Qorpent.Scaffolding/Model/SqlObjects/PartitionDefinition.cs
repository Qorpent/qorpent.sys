using System.Xml.Linq;
using Qorpent.BSharp;

namespace Qorpent.Scaffolding.Model.SqlObjects{
	/// <summary>
	/// Определение схемы и функции партицирования
	/// </summary>
	public class PartitionDefinition : SqlObject{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="model"></param>
		/// <param name="cls"></param>
		/// <param name="bscls"></param>
		/// <param name="xml"></param>
		public override void Setup(PersistentModel model, PersistentClass cls, IBSharpClass bscls, XElement xml)
		{
			base.Setup(model, cls, bscls, xml);
			this.Allocation = cls.AllocationInfo;
			this.FileGroup = Allocation.FileGroup.Name;
			this.Name = cls.FullSqlName.Replace(".","_")+"_PARTITION";
			this.Start = Allocation.PartitioningStart;
			if (null != Allocation.PartitionField){
				this.Type = Allocation.PartitionField.DataType.ResolveSqlDataType(SqlDialect.SqlServer);
			}
			else{
				this.Type = "int";
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Type { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Start { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string FileGroup { get; set; }
		/// <summary>
		/// 
		/// </summary>
		protected AllocationInfo Allocation { get; set; }
	}
}