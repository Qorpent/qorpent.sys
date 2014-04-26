namespace Qorpent.Host
{
	/// <summary>
	/// Константы настройки хоста
	/// </summary>
	public static class HostConstants
	{
		/// <summary>
		/// Папка с конфигурацией по умолчанию
		/// </summary>
		public const string DefaultConfigFolder = ".config";
		/// <summary>
		/// Папка с исполнимым кодом по умолчанию
		/// </summary>
		public const string DefaultDllFolder = "bin";
		/// <summary>
		/// Папка для временных файлов по умолчанию
		/// </summary>
		public const string DefaultTmpFolder = ".tmp";
		/// <summary>
		/// Папка для журналов по умолчанию
		/// </summary>
		public const string DefaultLogFolder = ".log";
		/// <summary>
		/// Интерфейс для привязки по умолчанию
		/// </summary>
		public const string DefaultBindingInterface = "*";
		/// <summary>
		/// Порт для привязки по умолчанию
		/// </summary>
		public const int DefaultBindingPort = 8091;
		/// <summary>
		/// Имя параметра корневой папки в схеме XML
		/// </summary>
		public const string RootFolderXmlName = "RootFolder";
		/// <summary>
		/// Имя параметра папки конфига в схеме XML
		/// </summary>
		public const string ConfigFolderXmlName = "ConfigFolder";
		/// <summary>
		/// Имя параметра папки DLL в схеме XML
		/// </summary>
		public const string DllFolderXmlName = "DllFolder";
		/// <summary>
		/// Имя параметра папки временных файлов в схеме XML
		/// </summary>
		public const string TmpFolderXmlName = "TmpFolder";
		/// <summary>
		/// Имя параметра папки журналов в схеме XML
		/// </summary>
		public const string LogFolderXmlName = "LogFolder";
		/// <summary>
		/// Имя для элемента биндинга в схеме XML
		/// </summary>
		public const string BindingXmlName = "Binding";
		/// <summary>
		/// 
		/// </summary>
		public const string IncludeConfigXmlName = "IncludeConfig";
		/// <summary>
		/// 
		/// </summary>
		public const string ExcludeConfigXmlName = "ExcludeConfig";
		/// <summary>
		/// Имя для атрибута порта биндинга в схеме XML
		/// </summary>
		public const string PortXmlName = "port";
		/// <summary>
		/// Имя для атрибута интерфейса биндинга в схеме XML
		/// </summary>
		public const string InterfaceXmlName = "interface";
		/// <summary>
		/// Имя для атрибута схемы биндинга в схеме XML
		/// </summary>
		public const string SchemaXmlName = "schema";
		/// <summary>
		///Значение для атрибута схемы HTTPS в схеме XML
		/// </summary>
		public const string HttpsXmlValue = "https";
		/// <summary>
		/// Путь к сертификату SSL в схеме HTTPS
		/// </summary>
		public const string CertifityPathXmlName = "certifity";

		/// <summary>
		/// Количество потоков прослушивания запросов Web по умолчанию
		/// </summary>
		public const int DefaultThreadCount = 3;

		/// <summary>
		/// 
		/// </summary>
		public const string LogLevelXmlName = "LogLevel";
		/// <summary>
		/// Использовать имя приложения
		/// </summary>
		public const string UseApplicationName = "UseAppName";
	}
}