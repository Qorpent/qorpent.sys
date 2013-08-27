using System.IO;
namespace Qorpent.BSharp.Builder {
    /// <summary>
    /// 
    /// </summary>
    public static class BSharpBuilderDefaults {
		/// <summary>
		/// Директория для логов по умолчанию
		/// </summary>
	    public const string DefaultLogDirectory = BSharpBuilderDefaults.DefaultOutputDirectory;
		
		/// <summary>
		/// Директория для логов по умолчанию
		/// </summary>
	    public const string DefaultOutputDirectory = ".output";

		/// <summary>
		/// Директория для логов по умолчанию
		/// </summary>
		public const string DefaultDebugDirectory = ".debug";
	    /// <summary>
        /// 
        /// </summary>
        public const string IncludeFileElementName = "add";
        /// <summary>
        /// 
        /// </summary>
        public const string IncludeNsElementName = "ns";
        /// <summary>
        /// 
        /// </summary>
        public const string IncludeClassElementName = "class";

		/// <summary>
		/// 
		/// </summary>
		public const string DefaultOutputExtension = ".bs.xml";
        /// <summary>
        ///     Имя файла-лога с ошибками
        /// </summary>
        public const string ErrorsFilename = ".errors.xml";
        /// <summary>
        ///     Имя файла с выдачей по умолчанию
        /// </summary>
        public const string SingleModeFilename = "output";
        /// <summary>
        /// 
        /// </summary>
        public const string OrphansOutputDirectory = BSharpBuilderDefaults.DefaultOutputDirectory + "\\.orphans";
        /// <summary>
        ///     Имя классета внутри документа
        /// </summary>
        public const string BSharpClassName = "bsharpclass";
        /// <summary>
        ///     Имя контейнера для классетов внутри документа
        /// </summary>
        public const string BSharpClassetName = "bsharpclassset";
        /// <summary>
        ///     Рсширение для файлов по умолчанию
        /// </summary>
        public const string DefaultInputExtension = "bxls";
        /// <summary>
        ///     BSharp project
        /// </summary>
        public const string DefaultBSharpProjectExtension = "bsproj";

    }
}
