using System.Xml.Linq;

namespace Qorpent.BSharp {
	/// <summary>
	/// Фабрика для конструирования типовых сообщений об ошибках
	/// </summary>
	public static class BSharpErrors {
		/// <summary>
		/// Создает типовую ошибку о дублировании имени класса
		/// </summary>
		/// <param name="doubleClass"></param>
		/// <returns></returns>
		public static BSharpError DuplicateClassNames(IBSharpClass doubleClass) {
			return new BSharpError {
				Level = ErrorLevel.Error, 
				Class = doubleClass, 
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
				Phase = BSharpCompilePhase.SourceIndexing,
				Type = BSharpErrorType.ClassCreatedFormOverride,
				ClassName = cls.FullName,
				Xml = e,
				Message = "В инклуде не указан код - это может быть как результат сознательного условного пустого инклуда или ошибка в коде"
			};
		}

		public static BSharpError NotResolvedInclude(IBSharpClass cls, XElement e) {
			throw new System.NotImplementedException();
		}

		public static BSharpError EmptyInclude(IBSharpClass cls, XElement e) {
			throw new System.NotImplementedException();
		}
	}
}