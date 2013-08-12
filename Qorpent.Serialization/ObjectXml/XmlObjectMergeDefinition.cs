namespace Qorpent.ObjectXml {
	/// <summary>
	/// 
	/// </summary>
	public class XmlObjectMergeDefinition
	{
		/// <summary>
		/// 
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Имя цели мержинга (рут)
		/// </summary>
		public string TargetName { get; set; }
		/// <summary>
		/// Тип импорта 
		/// </summary>
		public XmlObjectMergeType Type { get; set; }

	}
}