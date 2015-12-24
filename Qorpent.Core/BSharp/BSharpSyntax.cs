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
		/// Отключает имплицитную настройку элементов
		/// </summary>
		public const string ExplicitClassMarker = "explicit";
		/// <summary>
		/// Специальное значение для explicit, указывающее распространение на все дочерние классы
		/// </summary>
		public const string ExplicitClassPropagateValue = "propagate";

		/// <summary>
		/// Помечает партицированные классы
		/// </summary>
		public const string PartialClass = "partial";
		/// <summary>
		/// Корневой элемент использования типов
		/// </summary>
		public const string AliasImport = "using";
		/// <summary>"nname"
		/// Ключевое слово,обозначающее заголовок класса
		/// </summary>
		public const string Class = "class";

		/// <summary>
		/// Ключевое слово,обозначающее источник данных для генерации классов
		/// </summary>
		public const string Dataset = "dataset";
		/// <summary>
		/// Ключевое слово шаблона
		/// </summary>
		public const string Template = "template";
		/// <summary>
		/// Директива для отключения обработки стандартных конструций (в основном INCLUDE)
		/// </summary>
		public const string NoProcessDirective = "__no_process";
		/// <summary>
		/// Ключевое слово,обозначающее источник данных для генерации классов
		/// </summary>
		public const string DatasetImport = "ref";
		/// <summary>
		/// Ключевое слово,обозначающее источник данных для генерации классов
		/// </summary>
		public const string DatasetItem = "item";

		/// <summary>
		/// Префикс кода класса набора данных
		/// </summary>
		public const string DatasetClassCodePrefix = "__ds_";

		/// <summary>
		/// Ключевое слово,обозначающее генератор классов
		/// </summary>
		public const string Generator = "generator";
		/// <summary>
		/// Подставной код класса для генератора
		/// </summary>
		public const string GeneratorClassCodeAttribute = "class.code";
		/// <summary>
		/// Подставное имя класса для генератора
		/// </summary>
		public const string GeneratorClassNameAttribute = "class.name";

		/// <summary>
		/// Ключевое слово для инициализации патча
		/// </summary>
		public const string PatchClassKeyword = "patch";
		/// <summary>
		/// 
		/// </summary>
		public const string PatchTargetAttribute = "for";
		/// <summary>
		/// 
		/// </summary>
		public const string PatchPlainAttribute = "plain";

		/// <summary>
		/// 
		/// </summary>
		public const string PatchBeforeAttribute = "before";

		/// <summary>
		/// 
		/// </summary>
		public const string PatchAfterBuildAttribute = "build";

		/// <summary>
		/// 
		/// </summary>
		public const string PatchAfterAttribute = "after";
		/// <summary>
		/// Описатель действия патча при создании элемента
		/// </summary>
		public const string PatchCreateBehavior = "new";

		/// <summary>
		/// Создание элементов игнорируется
		/// </summary>
		public const string PatchCreateBehaviorNone = "none";
		/// <summary>
		/// При необходимости создания элемента формируется ошибка (по умолчанию)
		/// </summary>
		public const string PatchCreateBehaviorError = "error";
		/// <summary>
		/// При создании элемента - он создается
		/// </summary>
		public const string PatchCreateBehaviorCreate = "create";

		/// <summary>
		/// Описатель поведения патча по именам элементов
		/// </summary>
		public const string PatchNameBehavior = "elementname";

		/// <summary>
		/// Описатель поведения патча по именам элементов - точное совпадение
		/// </summary>
		public const string PatchNameBehaviorMatch = "match";


		/// <summary>
		/// Описатель поведения патча по именам элементов - не точное совпадение
		/// </summary>
		public const string PatchNameBehaviorFree = "free";

		/// <summary>
		/// Соединение
		/// </summary>
		public const string Connection = "connection";
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
		public const string PriorityAttribute = "priority";


		/// <summary>
		/// Собственный код соединения
		/// </summary>
		public const string ConnectionCodeAttribute = "connection.code";

		/// <summary>
		/// Строка соединения
		/// </summary>
		public const string TemplateValueAttribute = "template.value";
		/// <summary>
		/// Строка соединения
		/// </summary>
		public const string ConnectionStringAttribute = "connection.string";
		/// <summary>
		/// Атрибут режима соединения
		/// </summary>
		public const string ConnectionModeAttribute = "mode";
		/// <summary>
		/// Атрибут приоритета перекрытия класса
		/// </summary>
		public const string ClassExtensionPriorityAttribute = PriorityAttribute;
		/// <summary>
		/// Признак абстрактного класса
		/// </summary>
		public const string ClassAbstractModifier = "abstract";

		/// <summary>
		/// Признак класса- генерика
		/// </summary>
		public const string ClassGenericModifier = "generic";
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
		/// Определение методов класса
		/// </summary>
		public const string ClassEvaluateDefinition = "eval";
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
		/// Общий атрибут условной компиляции для всей группы partial - объединяются в одно условие при мерже
		/// </summary>
		public const string AllConditionalAttribute = "all-if";
        /// <summary>
        /// Сгруппированный блок элементов
        /// </summary>
        public const string GroupedBlock = "set";
        /// <summary>
        /// Блок для применения атрибутов "вверх"   
        /// </summary>
	    public const string UpSetBlock = "up-set";

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
        /// Модификатор include, изменяющий имя целевого элемента
        /// </summary>
        public const string IncludeElementNameModifier = "element";

        /// <summary>
        /// Модификатор include, изменяющий имя целевого элемента
        /// </summary>
        public const string IncludeKeepCodeModifier = "keepcode";

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
		/// Директива постпроцессора на уделение части элементов сверху
		/// </summary>
        public static readonly string PostProcessRemoveBefore = "remove-before";
		/// <summary>
		/// Директива постпроцессора на выборку элементов
		/// </summary>
        public static readonly string PostProcessSelectElements = "select-elements";
        
		/// <summary>
		/// Префикс приватного атрибута
		/// </summary>
		public const char PrivateAttributePrefix = '_';
		/// <summary>
		/// Признак внедряемого класса (не может использоваться самостоятельно, не включается отдельно в библиотеки)
		/// </summary>
		public const string EmbedAttribute = "embed";
		/// <summary>
		/// Элемент ссылки на другой файл в качестве "требуемого"
		/// </summary>
		public const string Require = "require";
		/// <summary>
		/// Определение глобальой константы (констант)
		/// </summary>
		public const string ConstantDefinition = "const";

		/// <summary>
		/// Переопределение глобальой константы (констант)
		/// </summary>
		public static readonly string ConstantOverrideDefinition = "~".Escape(EscapingType.XmlName)+ConstantDefinition;
		/// <summary>
		/// Дефолт глобальной константы
		/// </summary>
		public static readonly string ConstantDefaultDefinition = "+".Escape(EscapingType.XmlName)+ConstantDefinition;
        /// <summary>
        /// Элемент макрозамены элемента
        /// </summary>
	    public const string PostProcessMacroReplace = "macro-replace";

	    /// <summary>
		/// Формирует имя класса соединения
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="code"></param>
		/// <returns></returns>
		public static string GenerateConnectionClassName(string mode, string code){
			return "connection_" + mode + "_" + code;
		}

		/// <summary>
		/// Формирует имя класса строкового шаблона
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public static string GenerateTemplateClassName( string code)
		{
			return "template_"  + code;
		}
	}
}