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
		DangerousElement = 1<<7,
		/// <summary>
		/// Текст непосредственно в корневом элементе
		/// </summary>
		RootText = 1<<8,

		/// <summary>
		/// Текст непосредственно в корневом элементе
		/// </summary>
		RootInline = 1 << 9,
		/// <summary>
		/// Неизвестный атрибут
		/// </summary>
		UnknownAttribute =1<<10,
		/// <summary>
		/// Вложенные параграфы
		/// </summary>
		NestedParaElements = 1 << 11,
		/// <summary>
		/// Неверная позиция для BR
		/// </summary>
		InvalidBrPosition = 1<<12,
		/// <summary>
		/// Текст в элементе, запрещающем его наличие
		/// </summary>
		TextInNonTextElement =1<<14,
		/// <summary>
		/// Инлайновый элемент в нетекстовом элементе
		/// </summary>
		InlineInNonTextElement =1<<15,
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
		DangerousAttribute = 1<<22,
		/// <summary>
		/// Пустой элемент
		/// </summary>
		EmptyElement = 1<<23,
		/// <summary>
		/// Не пустой элемент IMG
		/// </summary>
		NonEmptyNonContentTag = 1<<24,
		/// <summary>
		/// Обнаружены имена тегов с верхним регистром
		/// </summary>
		UpperCaseDetected = 1<<25,
		/// <summary>
		/// Признак наличия ссылки на javascript	
		/// </summary>
		DangerousLink =1<<26,
		/// <summary>
		/// Признак недоверенной ссылки
		/// </summary>
		NonTrustedLink = 1<<27,
		/// <summary>
		/// Пустая или чистая хэш ссылка
		/// </summary>
		EmptyOrHashedLink = (ulong)1 << 28,
		/// <summary>
		/// Обший описатель проблем с ссылками
		/// </summary>
		InvalidLink = DangerousLink|NonTrustedLink|EmptyOrHashedLink,
		/// <summary>
		/// У IMG не указан src
		/// </summary>
		NoRequiredSrcAttributeInImg = (ulong)1<<30,
		/// <summary>
		/// У A не указан HREF
		/// </summary>
		NoRequiredHrefAttributeInA = (ulong)1<<31,
		/// <summary>
		/// Обнаружен тег EMBED
		/// </summary>
		AppletDetected = (ulong)1 << 32,

		/// <summary>
		/// Элемент, не разрешенный по схеме
		/// </summary>
		UnknownElement = (ulong)1<<33,

		/// <summary>
		/// Ошибки в структуре списка
		/// </summary>
		InvalidList = (ulong)1<<34,
		/// <summary>
		/// Ошибка в структуре таблицы
		/// </summary>
		InvalidTable = (ulong)1<<35,
		/// <summary>
		/// Неверный URI ссылки
		/// </summary>
		InvalidUri = (ulong)1<<36,
	}
}