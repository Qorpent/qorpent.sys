namespace Qorpent.ObjectXml {
	/// <summary>
	/// </summary>
	public class ObjectXmlImport {
		/// <summary>
		/// </summary>
		public ObjectXmlClass Target { get; set; }

		/// <summary>
		///     Тип импорта
		/// </summary>
		public string Condition { get; set; }

		/// <summary>
		///     Код цели
		/// </summary>
		public string TargetCode { get; set; }

		/// <summary>
		///     Признак неразрешенного импорта
		/// </summary>
		public bool Orphaned { get; set; }
	}
}