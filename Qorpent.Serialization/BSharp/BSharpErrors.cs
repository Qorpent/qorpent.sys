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
				Message = "Импортируемый класс является сиртским и соответственно импорт реально не производится"
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
	}
}