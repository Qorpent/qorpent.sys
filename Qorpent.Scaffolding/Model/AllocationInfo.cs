using Qorpent.BSharp;

namespace Qorpent.Scaffolding.Model{
	/// <summary>
	/// 
	/// </summary>
	public class AllocationInfo{
		/// <summary>
		/// 
		/// </summary>
		public AllocationInfo(){
			FileGroup = "SECONDARY";
		}
		/// <summary>
		/// 
		/// </summary>
		public PersistentClass MyClass { get; set; }
		/// <summary>
		/// Файловая группа
		/// </summary>
		public string FileGroup { get; set; }
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
		public IBSharpClass FileGroupClass { get; set; }
	}
}