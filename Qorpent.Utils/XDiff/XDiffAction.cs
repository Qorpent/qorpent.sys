using System;

namespace Qorpent.Utils.XDiff{
	/// <summary>
	/// Действия, типы разностных элементов
	/// </summary>
	[Flags]
	public enum XDiffAction{
		/// <summary>
		/// Новый элемент
		/// </summary>
		CreateElement = 1,

		/// <summary>
		/// Изменение значения элемента
		/// </summary>
		ChangeElement = 2,

		/// <summary>
		/// Добавленный атрибут
		/// </summary>
		CreateAttribute = 4,

		/// <summary>
		/// Изменение значения атрибута
		/// </summary>
		ChangeAttribute = 8,

		/// <summary>
		/// У элемента изменилось имя (при смешанных списках как у строк)
		/// </summary>
		RenameElement = 16,

		/// <summary>
		/// Удаленный элемент
		/// </summary>
		DeleteElement = 32,

		/// <summary>
		/// Удаленный атрибут
		/// </summary>
		DeleteAttribute = 64,

		/// <summary>
		/// Изменение положения в иерерхии
		/// </summary>
		ChangeHierarchyPosition = 128,

		/// <summary>
		/// Типовые операции по добавлению и обновлению данных
		/// </summary>
		MainCreateOrUpdate = CreateElement | ChangeElement | CreateAttribute | ChangeAttribute | ChangeHierarchyPosition,
		/// <summary>
		/// Обновления по атрибутам
		/// </summary>
		AttributeCreateOrUpdate = CreateAttribute | ChangeAttribute 
	}
}