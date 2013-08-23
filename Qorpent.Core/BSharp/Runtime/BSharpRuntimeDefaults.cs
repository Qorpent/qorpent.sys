namespace Qorpent.BSharp.Runtime {
	/// <summary>
	/// Общие константы BSharpRT
	/// </summary>
	public static class BSharpRuntimeDefaults {
		/// <summary>
		/// Расширение для классов BSharp на диске
		/// </summary>
		public const string BSHARP_CLASS_FILE_EXTENSION = "bsclass"; 

		/// <summary>
		/// Bsharp class header name
		/// </summary>
		public const string BSHARP_CLASS_HEADER = "bsharpclass";
		/// <summary>
		/// Атрибут имени класса в XML
		/// </summary>
		public const string BSHARP_CLASS_NAME_ATTRIBUTE = "code";
		/// <summary>
		/// Атрибут полного имени класса в XML
		/// </summary>
		public const string BSHARP_CLASS_FULLNAME_ATTRIBUTE = "fullcode";
		/// <summary>
		/// Атрибут назначения runtime-имени в BSharp
		/// </summary>
		public const string BSHARP_RUNTIME_ATTRIBUTE = "runtime";

		/// <summary>
		/// Индекс активатора BSharp по умолчанию
		/// </summary>
		public const int DEFAULT_ACTIVATOR_INDEX = 100;


	}
}