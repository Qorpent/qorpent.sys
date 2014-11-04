using System;

namespace Qorpent {
	/// <summary>
	///		Атрибут описания сущности
	/// </summary>
	public class HelpAttribute : Attribute {
		/// <summary>
		///		Описание сущности
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		///		Значение по умолчанию
		/// </summary>
		public string Default { get; set; }
		/// <summary>
		///		Атрибут описания сущности
		/// </summary>
		public HelpAttribute() { }
		/// <summary>
		///		Атрибут описания сущности
		/// </summary>
		/// <param name="description">Описание сущности</param>
		public HelpAttribute(string description) {
			Description = description;
		}
	}
}
