﻿namespace Qorpent.BSharp.Builder{
	/// <summary>
	/// </summary>
	public static class BSharpBuilderDefaults{
		/// <summary>
		///     Директория для логов по умолчанию
		/// </summary>
		public const string DefaultLogDirectory = DefaultOutputDirectory;

		/// <summary>
		///     Директория для логов по умолчанию
		/// </summary>
		public const string DefaultOutputDirectory = ".output";

        /// <summary>
        ///     Директория для логов по умолчанию
        /// </summary>
        public const string DefaultCompileDirectory = ".compile";

        /// <summary>
        ///     Директория для логов по умолчанию
        /// </summary>
        public const string DefaultDebugDirectory = ".debug";

		/// <summary>
		/// </summary>
		public const string IncludeFileElementName = "add";

		/// <summary>
		/// </summary>
		public const string IncludeNsElementName = "ns";

		/// <summary>
		/// </summary>
		public const string IncludeClassElementName = "class";

		/// <summary>
		/// </summary>
		public const string DefaultOutputExtension = "bs.xml";

		/// <summary>
		///     Имя файла-лога с ошибками
		/// </summary>
		public const string ErrorsFilename = ".errors.xml";

		/// <summary>
		///     Имя файла с выдачей по умолчанию
		/// </summary>
		public const string SingleModeFilename = "output";

		/// <summary>
		/// </summary>
		public const string OrphansOutputDirectory = ".orphans";

		/// <summary>
		///     Имя классета внутри документа
		/// </summary>
		public const string BSharpClassContainerName = "bsharpclass";

		/// <summary>
		///     Имя контейнера для классетов внутри документа
		/// </summary>
		public const string BSharpClassetName = "bsharpclassset";

		/// <summary>
		///     Рсширение для файлов по умолчанию
		/// </summary>
		public const string DefaultInputExtension = "bxls;bxjs";

		/// <summary>
		///     BSharp project
		/// </summary>
		public const string DefaultBSharpProjectExtension = "bsproj";

		/// <summary>
		///     Имя контейнера для индекса классов
		/// </summary>
		public const string DefaultClassIndexContainerName = "bsharpclassindex";

		/// <summary>
		///     Расширение для файлов индекса по молчанию
		/// </summary>
		public const string IndexFileExtension = "ibsclass";

		/// <summary>
		///     Имя файла с индексом по умолчанию
		/// </summary>
		public const string IndexFileName = "index";
	}
}