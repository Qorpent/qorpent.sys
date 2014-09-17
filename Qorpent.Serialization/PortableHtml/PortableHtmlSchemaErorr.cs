using System;

namespace Qorpent.PortableHtml{
	/// <summary>
	/// Ошибки в схеме PortableHtml
	/// </summary>
	[Flags]
	public enum PortableHtmlSchemaErorr:ulong{
		/// <summary>
		/// Ошибки отсутствуют
		/// </summary>
		None = 0,
		/// <summary>
		/// Представленный текст не является XML
		/// </summary>
		NonXml = 1<<0,
		/// <summary>
		/// Представленный фрагмент не имеет единичного корневого элемента
		/// </summary>
		NoRootTag = 1<<1,
		/// <summary>
		/// Неправильный корневой элемент (должен быть div)
		/// </summary>
		InvalidRootTag = 1<<2,
		/// <summary>
		/// Пустой или пробельный код на входе проверки
		/// </summary>
		EmptyInput = 1<<3,
		/// <summary>
		/// Комментарии запрещены
		/// </summary>
		CommentsDetected = 1 << 4,
		/// <summary>
		/// Обнаружены инструкции процессинга
		/// </summary>
		ProcessingInstructionsDetected = 1 << 5,
		/// <summary>
		/// Обнаружены определения пространст имен
		/// </summary>
		NamespaceDeclarationDetected = 1<<6,
		/// <summary>
		/// Обнаружен тег Script
		/// </summary>
		ScriptDetected = 1<<7,
		/// <summary>
		/// Обнаружен тег OBJECT
		/// </summary>
		ObjectDetected = 1<<8,
		/// <summary>
		/// Обнаружен тег EMBED
		/// </summary>
		EmbedDetected = 1<<9,

		/// <summary>
		/// Обнаружен тег FORM
		/// </summary>
		FormDetected = 1 << 10,

		/// <summary>
		/// Обнаружен тег INPUT
		/// </summary>
		InputDetected = 1 << 11,

		/// <summary>
		/// Обнаружен тег BUTTON
		/// </summary>
		ButtonDetected = 1 << 12,

		/// <summary>
		/// Обнаружен тег SELECT
		/// </summary>
		SelectDetected = 1 << 13,

		/// <summary>
		/// Обнаружен тег TEXTAREA
		/// </summary>
		TextareaDetected = 1 << 14,

		/// <summary>
		/// Обнаружен тег IFRAME
		/// </summary>
		IframeDetected = 1 << 15,

		/// <summary>
		/// Обнаружен тег STYLE
		/// </summary>
		StyleDetected = 1 << 16,

		/// <summary>
		/// Обнаружена CDATA
		/// </summary>
		CdataDetected = 1 << 17,
		/// <summary>
		/// Обнаружена неэкранированная закрывающая скобка	
		/// </summary>
		NonEscapedGt = 1<<18,
		/// <summary>
		/// Обнаружен атрибут-обработчик
		/// </summary>
		EventAttributeDetected = 1<<19,
		/// <summary>
		/// Обнаружен атрибут, значимый для CSS
		/// </summary>
		CssAttributeDetected = 1<<20,
		/// <summary>
		/// Обнаружен атрибут, связанный с AngularJS
		/// </summary>
		AngularAttributeDetected = 1<<21,
		/// <summary>
		/// Обнаружен иной запрещенный атрибут
		/// </summary>
		DeprecatedAttributeDetected = 1<<22,
	}
}