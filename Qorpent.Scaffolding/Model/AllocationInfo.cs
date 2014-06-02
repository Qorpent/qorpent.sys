using Qorpent.BSharp;
using Qorpent.Scaffolding.Model.SqlObjects;

namespace Qorpent.Scaffolding.Model{
	/// <summary>
	/// 
	/// </summary>
	public class AllocationInfo{
		/// <summary>
		/// 
		/// </summary>
		public AllocationInfo(){
			FileGroupName = "SECONDARY";
		}
		/// <summary>
		/// 
		/// </summary>
		public PersistentClass MyClass { get; set; }
		/// <summary>
		/// Файловая группа
		/// </summary>
		public string FileGroupName { get; set; }
		/// <summary>
		/// Признак партицирования
		/// </summary>
		public bool Partitioned { get; set; }
		/// <summary>
		/// Имя поля для партицирования
		/// </summary>
		public string PartitionFieldName { get; set; }
		/// <summary>
		/// Связанное поле партицирования
		/// </summary>
		public Field PartitionField { get; set; }
		/// <summary>
		/// Первый разделитель партицирования
		/// </summary>
		public int PartitioningStart { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public FileGroup FileGroup { get; set; }
	}
}