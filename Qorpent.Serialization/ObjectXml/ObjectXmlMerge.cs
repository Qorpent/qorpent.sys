namespace Qorpent.ObjectXml {
	/// <summary>
	/// </summary>
	public class ObjectXmlMerge {
		/// <summary>
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///     Имя цели мержинга (рут)
		/// </summary>
		public string TargetName { get; set; }

		/// <summary>
		///     Тип импорта
		/// </summary>
		public ObjectXmlMergeType Type { get; set; }
	}
}