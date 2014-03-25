﻿namespace Qorpent.Utils.XDiff{
	/// <summary>
	/// 
	/// </summary>
	public class XDiffOptions{
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
	}
}