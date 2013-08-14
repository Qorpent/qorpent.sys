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
			return new BSharpError {Level = ErrorLevel.Error, Class = doubleClass, Phase = BSharpCompilePhase.SourceIndexing};
		}
	}
}