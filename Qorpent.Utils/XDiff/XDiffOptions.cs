using System.Collections.Generic;
using System.Xml.Linq;

namespace Qorpent.Utils.XDiff{
	/// <summary>
	/// 
	/// </summary>
	public class XDiffOptions{
		/// <summary>
		/// 
		/// </summary>
		public XDiffOptions(){
			IncludeActions = XDiffAction.All;
			ErrorActions = XDiffAction.None;
		}
		/// <summary>
		/// Признак того, что набор элементов это древовидный список (требует предварительного "уплощения" для обработки)
		/// </summary>
		public bool IsHierarchy { get; set; }
		/// <summary>
		/// If false (default) elements with distinct local names treats as distinct,
		/// if true - only "code" and/or "id" attribute will be used to detect equality
		/// </summary>
		public bool IsNameIndepended { get; set; }
		/// <summary>
		/// If true  - only codes are used to detect  identity
		/// </summary>
		public bool ChangeIds { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public XElement SrcXml { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public IDictionary<string, string> RefMaps { get; set; }

		/// <summary>
		/// Маска операций, которые должны возвращаться диффом
		/// </summary>
		public XDiffAction IncludeActions { get; set; }
		/// <summary>
		/// Маска действий, которые при наличии должны вызывать ошибку
		/// </summary>
		public XDiffAction ErrorActions { get; set; }
		/// <summary>
		/// При включении данной опции объединяет все обновления по атрибутам одной цели в один Diff с промежуточным элементом- контенером
		/// </summary>
		public bool MergeAttributeChanges { get; set; }

		/// <summary>
		/// Передает все команды создания атрибутов как ИЗМЕНЕНИЯ атрибутов
		/// </summary>
		/// <remarks></remarks>
		public bool TreatNewAttributesAsChanges { get; set; }
	}
}