using System;
using System.Xml.Linq;
using Qorpent.Serialization;

namespace Qorpent.BSharp {
	/// <summary>
	/// Класс с определением и описанием ключевых и зарезервированных слов BSharp
	/// </summary>
	public static class BSharpSyntax
	{
		/// <summary>
		/// Ключевое слово, открывающее пространство имен
		/// </summary>
		public const string Namespace = "namespace";
		/// <summary>
		/// Ключевое слово,обозначающее заголовок класса
		/// </summary>
		public const string Class = "class";
		/// <summary>
		/// Разделитель полного имени класса (путь) 
		/// </summary>
		public const char ClassPathDelimiter = '.';
	    /// <summary>
	    /// Определение перекрывающего аспекта класса
	    /// </summary>
	    public static readonly string ClassOverrideKeyword = "~".Escape(EscapingType.XmlName) + "class";
	    /// <summary>
	    /// Определение дополняющего аспекта класса
	    /// </summary>
        public static readonly string ClassExtensionKeyword = "+".Escape(EscapingType.XmlName) + "class";
		/// <summary>
		/// Атрибут приоритета перекрытия класса
		/// </summary>
		public const string ClassOverridePriorityAttribute = "priority";
		/// <summary>
		/// Атрибут приоритета перекрытия класса
		/// </summary>
		public const string ClassExtensionPriorityAttribute = ClassOverridePriorityAttribute;
		/// <summary>
		/// Признак абстрактного класса
		/// </summary>
		public const string ClassAbstractModifier = "abstract";
		/// <summary>
		/// Признак статичского класса
		/// </summary>
		public const string ClassStaticModifier = "static";
		/// <summary>
		/// Признак класса - схемы (для других классов)
		/// </summary>
		public const string SchemaClassModifier = "schema";
		/// <summary>
		/// Единица импорта родительского класса
		/// </summary>
		public const string ClassImportDefinition = "import";
		/// <summary>
		/// Определение элемента класса
		/// </summary>
		public const string ClassElementDefinition= "element";
		/// <summary>
		/// Зарезервированный атрибут класса, указывющий его прототип
		/// </summary>
		public const string ClassPrototypeAttribute = "prototype";
		/// <summary>
		/// Зарезервированный атрибут класса, указывающий целевой рантайм класс для IoC
		/// </summary>
		public const string ClassRuntimeAttribute = "runtime";

		/// <summary>
		/// Зарезервированный атрибут класса, указывающий целевой рантайм класс для IoC
		/// </summary>
		public const string ClassNameAttribute = "code";

		/// <summary>
		/// Зарезервированный атрибут класса, указывающий целевой рантайм класс для IoC
		/// </summary>
		public const string ClassFullNameAttribute = "fullcode";
		/// <summary>
		/// Общий атрибут условной компиляции
		/// </summary>
		public const string ConditionalAttribute = "if";
		/// <summary>
		/// Сгруппированный блок элементов
		/// </summary>
		public const string GroupedBlock = "set";

		/// <summary>
		/// Определение экспортируемого словаря класса
		/// </summary>
		public const string ClassExportDefinition = "export";

		/// <summary>
		/// Блок включения одного класса в другой
		/// </summary>
		public const string IncludeBlock = "include";

		/// <summary>
		/// Модификатор блока Include, указывающий что импортировать надо именно содержимое класса
		/// </summary>
		public const string IncludeBodyModifier = "body";

		/// <summary>
		/// Модификатор включения по запросу а не отдельного класса
		/// </summary>
		public const string IncludeAllModifier = "all";

		/// <summary>
		/// Модификатор include, блокирующий включение дочерних элементов
		/// </summary>
		public const string IncludeNoChildModifier = "nochild";

		/// <summary>
		/// Описание условия для включения элементов в режиме body для Include
		/// </summary>
		public const char IncludeInterpolationAncor = '%';

		/// <summary>
		/// Описание условия для включения элементов в режиме body для Include
		/// </summary>
		public const string IncludeWhereClause = "where";

        /// <summary>
        /// Описание условия на включение атрибутов в операции include [all] ... body
        /// </summary>
        public const string IncludeSelectClause = "select";

        /// <summary>
        /// Описание условия на группировку элементов в операции include
        /// </summary>
        /// <remarks>
        /// </remarks>
        public const string IncludeGroupByClause = "groupby";

        /// <summary>
        /// Описание условия на операцию упорядочени я элементов в операции include [all] ... body
        /// </summary>
        /// <remarks>
        /// В отличие от SQL order применяется до группировки и селекта и соответственно может и должен быть выражен в исходных именах
        /// атрибутов
        /// </remarks>
        public const string IncludeOrderByClause = "orderby";
		/// <summary>
		/// Атрибут блокирования интерполяции
		/// </summary>
		public const string StopInterpolationAttribute = "stopinterpolate";
		/// <summary>
		/// Префикс свойства, импортирующего имя класса
		/// </summary>
		public const char ClassReferencePrefix = '^';
		/// <summary>
		/// Префикс свойства, импортирующего словарь
		/// </summary>
		public const char DictionaryReferencePrefix = '?';
		/// <summary>
		/// Модификатор запроса словаря только по значению
		/// </summary>
		public const char DictionaryReferenceValueOnlyModifier = '?';
		/// <summary>
		/// Модификатор опционально значения из словаря
		/// </summary>
		public const char DictionaryReferenceOptionalModifier = '~';
		/// <summary>
		/// Разделитель кода словаря и элемента в ссылке на словарь
		/// </summary>
		public const char DictionaryCodeElementDelimiter = '.';
		/// <summary>
		/// Префикс перекрытия элемента
		/// </summary>
        public static readonly string ElementOverridePrefix = "~".Escape(EscapingType.XmlName);
		/// <summary>
		/// Префикс расширения элемента
		/// </summary>
        public static readonly string ElementExtensionPrefix = "+".Escape(EscapingType.XmlName);
		/// <summary>
		/// Префикс приватного атрибута
		/// </summary>
		public const char PrivateAttributePrefix = '_';

	}
}