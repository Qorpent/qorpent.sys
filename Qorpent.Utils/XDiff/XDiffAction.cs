namespace Qorpent.Utils.XDiff{
	/// <summary>
	/// Действия, типы разностных элементов
	/// </summary>
	public enum XDiffAction{
		/// <summary>
		/// Новый элемент
		/// </summary>
		CreateElement,
		/// <summary>
		/// Изменение значения элемента
		/// </summary>
		ChangeElement,
		/// <summary>
		/// Добавленный атрибут
		/// </summary>
		CreateAttribute,
		/// <summary>
		/// Изменение значения атрибута
		/// </summary>
		ChangeAttribute,
		/// <summary>
		/// У элемента изменилось имя (при смешанных списках как у строк)
		/// </summary>
		RenameElement,
		/// <summary>
		/// Удаленный элемент
		/// </summary>
		DeleteElement,
		/// <summary>
		/// Удаленный атрибут
		/// </summary>
		DeleteAttribute,
		/// <summary>
		/// Изменение положения в иерерхии
		/// </summary>
		ChangeHierarchyPosition,
	}
}