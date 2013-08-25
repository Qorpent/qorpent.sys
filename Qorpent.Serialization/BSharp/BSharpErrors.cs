using System;
using System.Xml.Linq;

namespace Qorpent.BSharp {
	/// <summary>
	/// Фабрика для конструирования типовых сообщений об ошибках
	/// </summary>
	public static class BSharpErrors {
		/// <summary>
		/// Создает типовую ошибку о дублировании имени класса
		/// </summary>
		/// <returns></returns>
		public static BSharpError DuplicateClassNames(IBSharpClass cls1,IBSharpClass cls2) {
			return new BSharpError {
				Level = ErrorLevel.Error, 
				Class = cls1, 
				AltClass = cls2, 
				Phase = BSharpCompilePhase.SourceIndexing,
				Type = BSharpErrorType.DuplicateClassNames,
				Message = "В коде обнаружено два класса с одинаковыми (полными) именами."
			};
		}
		/// <summary>
		/// Создает ошибку импорта класса-"сироты"
		/// </summary>
		/// <param name="cls"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		public static BSharpError OrphanImport(IBSharpClass cls, IBSharpImport i) {
			return new BSharpError
			{
				Level = ErrorLevel.Error,
				Phase = BSharpCompilePhase.ImportResolution,
				Type = BSharpErrorType.OrphanImport,
				Data = i,
				ClassName = cls.FullName,
				Message = "Импортируемый класс является сиротским и соответственно импорт реально не производится"
			};
		}

		/// <summary>
		/// Создает ошибку импорта класса-"сироты"
		/// </summary>
		/// <param name="cls"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		public static BSharpError NotResolvedImport(IBSharpClass cls, IBSharpImport i)
		{
			return new BSharpError
			{
				Level = ErrorLevel.Error,
				Phase = BSharpCompilePhase.ImportResolution,
				Type = BSharpErrorType.NotResolvedImport,
				Data = i,
				ClassName = cls.FullName,
				Message = "В качестве источника для импорта указан несуществующий класс"
			};
		}
		/// <summary>
		/// Создает ошибку циклического импорта
		/// </summary>
		/// <returns></returns>
		public static BSharpError RecycleImport(IBSharpClass cls, string root, IBSharpImport i) {
			return new BSharpError
			{
				Level = ErrorLevel.Error,
				Phase = BSharpCompilePhase.ImportResolution,
				Type = BSharpErrorType.RecycleImport,
				Data = i,
				ClassName = cls.FullName + " [ from root : " +root+" ]",
				Message = "Циклические ссылки означают ошибки в построении иерархии, при импорте такие ссылки игнорируются"
			};
		}
		/// <summary>
		/// Создает предупреждение о создании класса из расширения
		/// </summary>
		/// <param name="source"></param>
		/// <param name="targetClassName"></param>
		/// <returns></returns>
		public static BSharpError ClassCreatedFormExtension(XElement source, string targetClassName) {
			return new BSharpError
			{
				Level = ErrorLevel.Warning,
				Phase = BSharpCompilePhase.SourceIndexing,
				Type = BSharpErrorType.ClassCreatedFormExtension,
				ClassName = targetClassName,
				Xml = source,
				Message = "Создание класса через расширение может быть сигналом того, что реальный класс не включен в сборку или переименован"
			};
		}
		/// <summary>
		/// Создает предупреждение о создании класса из перекрытия
		/// </summary>
		/// <param name="source"></param>
		/// <param name="targetClassName"></param>
		/// <returns></returns>
		public static BSharpError ClassCreatedFormOverride(XElement source, string targetClassName) {
			return new BSharpError
			{
				Level = ErrorLevel.Warning,
				Phase = BSharpCompilePhase.SourceIndexing,
				Type = BSharpErrorType.ClassCreatedFormOverride,
				ClassName = targetClassName,
				Xml = source,
				Message = "Создание класса через перекрытие может быть сигналом того, что реальный класс не включен в сборку или переименован"
			};
		}
		/// <summary>
		/// Ошибка фейкового инклуда
		/// </summary>
		/// <param name="cls"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		public static BSharpError FakeInclude(IBSharpClass cls, XElement e) {
			return new BSharpError
			{
				Level = ErrorLevel.Warning,
				Phase = BSharpCompilePhase.IncludeProcessing,
				Type = BSharpErrorType.FakeInclude,
				ClassName = cls.FullName,
				Xml = e,
				Message = "В инклуде не указан код - это может быть как результат сознательного условного пустого инклуда или ошибка в коде"
			};
		}
		/// <summary>
		/// Ошибка включения несуществующего класса
		/// </summary>
		/// <param name="cls"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		public static BSharpError NotResolvedInclude(IBSharpClass cls, XElement e) {
			return new BSharpError
			{
				Level = ErrorLevel.Error,
				Phase = BSharpCompilePhase.IncludeProcessing,
				Type = BSharpErrorType.NotResolvedInclude,
				ClassName = cls.FullName,
				Xml = e,
				Message = "Попытка включить несуществующий класс"
			};
		}

		/// <summary>
		/// Ошибка включения несуществующего класса
		/// </summary>
		/// <param name="cls"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		public static BSharpError OrphanInclude(IBSharpClass cls, XElement e)
		{
			return new BSharpError
			{
				Level = ErrorLevel.Error,
				Phase = BSharpCompilePhase.IncludeProcessing,
				Type = BSharpErrorType.OrphanInclude,
				ClassName = cls.FullName,
				Xml = e,
				Message = "Попытка включить класс-сироту"
			};
		}
		/// <summary>
		/// Пустой инклуд
		/// </summary>
		/// <param name="cls"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		public static BSharpError EmptyInclude(IBSharpClass cls, XElement e) {
			return new BSharpError
			{
				Level = ErrorLevel.Warning,
				Phase = BSharpCompilePhase.IncludeProcessing,
				Type = BSharpErrorType.EmptyInclude,
				ClassName = cls.FullName,
				Xml = e,
				Message = "Инклуд, включаемый в режиме body не имеет контента, возможно ошибка в коде"
			};
		}
		/// <summary>
		/// Ошибка - класс "сирота"
		/// </summary>
		/// <param name="cls"></param>
		/// <returns></returns>
		public static BSharpError OrphanClass(IBSharpClass cls) {
			return new BSharpError
			{
				Level = ErrorLevel.Error,
				Class = cls,
				Phase = BSharpCompilePhase.SourceIndexing,
				Type = BSharpErrorType.OrphanClass,
				Message = "В коде обнаружен участок, похожий на класс, но который нельзя связать ни с одной из имеющихся базовых классов или ключевым словом class"
			};
		}

		/// <summary>
		/// Создает общую ошибку
		/// </summary>
		/// <param name="exception"></param>
		/// <param name="level"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public static BSharpError Generic(Exception exception, ErrorLevel level =ErrorLevel.Fatal , object data = null) {
			return new BSharpError
			{
				Level = level,
				Data = data,
				Phase = BSharpCompilePhase.Common,
				Type = BSharpErrorType.GenericError,
				Error = exception,
				Message = "Общая ошибка выполнения"
			};
		}
		/// <summary>
		/// Ошибка импорта игнорируемого класса
		/// </summary>
		/// <returns></returns>
		public static BSharpError IgnoredImport(BSharpClass cls, IBSharpImport i) {
			return new BSharpError
			{
				Level = ErrorLevel.Error,
				Phase = BSharpCompilePhase.ImportResolution,
				Type = BSharpErrorType.IgnoredImport,
				Data = i,
				ClassName = cls.FullName,
				Message = "Импортируемый класс является гнорируемым по признаку if"
			};
		}
		
		/// <summary>
		/// Подсказка косвенной ссылки на класс
		/// </summary>
		/// <returns></returns>
		public static BSharpError NotDirectClassReference(IBSharpClass cls, XElement parent, string clsname)
		{
			return new BSharpError
			{
				Level = ErrorLevel.Warning,
				Phase = BSharpCompilePhase.ReferenceResolution,
				Type = BSharpErrorType.NotDirectClassReference,
				ClassName = cls.FullName,
				Xml = parent,
				Data = clsname,
				Message = "Ссылка на класс разрешена косвенно по имени, а не по стеку пространств имен"
			};
		}
		/// <summary>
		/// Неоднозначная косвенная ссылка на класс
		/// </summary>
		/// <returns></returns>
		public static BSharpError AmbigousClassReference(IBSharpClass cls, XElement parent, string clsname)
		{
			return new BSharpError
			{
				Level = ErrorLevel.Error,
				Phase = BSharpCompilePhase.ReferenceResolution,
				Type = BSharpErrorType.AmbigousClassReference,
				ClassName = cls.FullName,
				Xml = parent,
				Data = clsname,
				Message = "Двойственная ссылка на класс - было найдено несколько кандидатов"
			};
		}
		/// <summary>
		/// Ошибка неразрешенной ссылки на класс
		/// </summary>
		/// <param name="cls"></param>
		/// <param name="parent"></param>
		/// <param name="clsname"></param>
		/// <returns></returns>
		public static BSharpError NotResolvedClassReference(IBSharpClass cls, XElement parent, string clsname)
		{
			return new BSharpError
			{
				Level = ErrorLevel.Error,
				Phase = BSharpCompilePhase.ReferenceResolution,
				Type = BSharpErrorType.NotResolvedClassReference,
				ClassName = cls.FullName,
				Xml = parent,
				Data = clsname,
				Message = "Не найден класс, на который указывает ссылка"
			};
		}
        /// <summary>
        /// Ошибка - не обнаружен словарь
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="parent"></param>
        /// <param name="code"></param>
        /// <returns></returns>
	    public static BSharpError NotResolvedDictionary(string fullName, XElement parent, string code) {
            return new BSharpError
            {
                Level = ErrorLevel.Warning,
                Phase = BSharpCompilePhase.ReferenceResolution,
                Type = BSharpErrorType.NotResolvedDictionary,
                ClassName = fullName,
                Xml = parent,
                Data = code,
                Message = "Не найден словарь, на который указывает ссылка"
            };
	    }
        /// <summary>
        /// Ошибка - не обнаружено значение словаря
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="parent"></param>
        /// <param name="val"></param>
        /// <returns></returns>
	    public static BSharpError NotResolvedDictionaryElement(string fullName, XElement parent, string val) {
            return new BSharpError
            {
                Level = ErrorLevel.Error,
                Phase = BSharpCompilePhase.ReferenceResolution,
                Type = BSharpErrorType.NotResolvedDictionaryElement,
                ClassName = fullName,
                Xml = parent,
                Data = val,
                Message = "Не найдено указанное значение словаря"
            };
	    }
	}
}