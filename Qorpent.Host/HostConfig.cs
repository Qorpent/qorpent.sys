using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Qorpent.Config;
using Qorpent.Log;
using Qorpent.Serialization;
using Qorpent.Uson;
using Qorpent.Utils.Extensions;

namespace Qorpent.Host
{
	/// <summary>
	/// Конфигурация хоста
	/// </summary>
	[Serialize]
	public class HostConfig
	{
		

		/// <summary>
		/// Формирует конфиг из XML
		/// </summary>
		/// <param name="xml"></param>
		public HostConfig(XElement xml):this()
		{
			if (null != xml)
			{
				LoadXmlConfig(xml);
			}
			
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.ToUson().ToXElement().ToString();
		}

		/// <summary>
		/// Загружает конфигурационный файл из XML
		/// </summary>
		/// <param name="xml"></param>
		public void LoadXmlConfig(XElement xml)
		{
			RootFolder = ResolveConfigWithXml(xml, RootFolder, HostConstants.RootFolderXmlName);
			ConfigFolder = ResolveConfigWithXml(xml, ConfigFolder, HostConstants.ConfigFolderXmlName);
			DllFolder = ResolveConfigWithXml(xml, DllFolder, HostConstants.DllFolderXmlName);
			LogFolder = ResolveConfigWithXml(xml, LogFolder, HostConstants.LogFolderXmlName);
			TmpFolder = ResolveConfigWithXml(xml, TmpFolder, HostConstants.TmpFolderXmlName);
			LogLevel = ResolveConfigWithXml(xml, "Info", HostConstants.LogLevelXmlName).To<LogLevel>();
			UseApplicationName = ResolveConfigWithXml(xml, "false", HostConstants.UseApplicationName).To<bool>();
			foreach (var bind in xml.Elements(HostConstants.BindingXmlName))
			{
				var hostbind = new HostBinding();
				hostbind.Port = bind.Attr(HostConstants.PortXmlName).ToInt();
				hostbind.Interface = bind.Attr(HostConstants.InterfaceXmlName);
				var schema = bind.Attr(HostConstants.SchemaXmlName);
				if (!string.IsNullOrWhiteSpace(schema))
				{
					if (schema == HostConstants.HttpsXmlValue)
					{
						hostbind.Schema = HostSchema.Https;
					}
				}
				if (hostbind.Port == 0)
				{
					hostbind.Port = HostConstants.DefaultBindingPort;
				}
				if (string.IsNullOrWhiteSpace(hostbind.Interface))
				{
					hostbind.Interface = HostConstants.DefaultBindingInterface;
				}
			}
			foreach (var e in xml.Elements(HostConstants.IncludeConfigXmlName))
			{
				IncludeConfigMasks.Add(e.Describe().GetEfficienValue());
			}
			foreach (var e in xml.Elements(HostConstants.ExcludeConfigXmlName))
			{
				ExcludeConfigMasks.Add(e.Describe().GetEfficienValue());
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public bool UseApplicationName
		{
			get { return _useApplicationName || Bindings.Any(_=>_.AppName!="/"); }
			set { _useApplicationName = value; }
		}

		/// <summary>
		/// Список путей для поиска статического контента
		/// </summary>
		public IList<string> ContentFolders { get; private set; }

		/// <summary>
		/// Список сборок для автоматической конфигурации
		/// </summary>
		public IList<string> AutoconfigureAssemblies { get; private set; }


		/// <summary>
		/// 
		/// </summary>
		[Serialize]
		public IList<string> IncludeConfigMasks { get; private set; }
		/// <summary>
		/// 
		/// </summary>
		[Serialize]
		public IList<string> ExcludeConfigMasks { get; private set; } 

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public bool IsConfigFileMatch(string filename)
		{
			if( 0 != IncludeConfigMasks.Count)
			{
				if (IncludeConfigMasks.All(_ => !Regex.IsMatch(filename, _))) return false;
			}
			if (0 != ExcludeConfigMasks.Count)
			{
				if (ExcludeConfigMasks.Any(_ => Regex.IsMatch(filename, _))) return false;
			}
			return true;
		}

		private string ResolveConfigWithXml(XElement xml, string current, string attributeOrElementName)
		{
			if (null == xml) return current;
			if (null != xml.Attribute(attributeOrElementName))
			{
				return xml.Attribute(attributeOrElementName).Value;
			}
			if (null != xml.Element(attributeOrElementName))
			{
				return xml.Element(attributeOrElementName).Value;
			}
			return current;
		}

		/// <summary>
		/// Формирует конфиг по умолчанию
		/// </summary>
		public HostConfig()
		{
			RootFolder = Environment.CurrentDirectory;
			_bindings = new List<HostBinding>();
			IncludeConfigMasks = new List<string>();
			ExcludeConfigMasks = new List<string>();
			AutoconfigureAssemblies = new List<string>();
			LogLevel = LogLevel.Info;
			ContentFolders = new List<string>();
			ExtendedContentFolders = new List<string>();
		}

		private string _configFolder;
		private string _dllFolder;
		private string _tmpFolder;
		private string _logFolder;
		private IList<HostBinding> _bindings;
		private string _rootFolder;
		private ConfigBase _parameters;
		private int _threadCount;
		private bool _useApplicationName;

		/// <summary>
		/// Корневая папка
		/// </summary>
		[Serialize]
		public string RootFolder
		{
			get { return _rootFolder; }
			set { _rootFolder = Path.GetFullPath(value); }
		}

		/// <summary>
		/// Папка с конфигами
		/// </summary>
		[Serialize]
		public string ConfigFolder
		{
			get
			{
				return _configFolder = NormalizeFolder(_configFolder, HostConstants.DefaultConfigFolder);
			}
			set { _configFolder = value; }
		}

		/// <summary>
		/// Папка с DLL
		/// </summary>
		[Serialize]
		public string DllFolder
		{
			get{
				return _dllFolder = NormalizeFolder(_dllFolder, HostConstants.DefaultDllFolder);
			}
			set { _dllFolder = value; }
		}
		private string NormalizeFolder(string current, string def)
		{

			if (!string.IsNullOrWhiteSpace(current) && Path.IsPathRooted(current)) return current;
			return Path.Combine(RootFolder, def);
		}

		/// <summary>
		/// Директория для временных файлов
		/// </summary>
		[Serialize]
		public string TmpFolder
		{
			get
			{
				return _tmpFolder = NormalizeFolder(_tmpFolder, HostConstants.DefaultTmpFolder);
			}
			set { _tmpFolder = value; }
		}

		/// <summary>
		/// Папка для файлов журналов
		/// </summary>
		[Serialize]
		public string LogFolder
		{
			get
			{
				return _logFolder = NormalizeFolder(_logFolder, HostConstants.DefaultLogFolder);
			}
			set { _logFolder = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public void AddDefaultBinding()
		{
			_bindings.Add(

							new HostBinding { Interface = HostConstants.DefaultBindingInterface, Port = HostConstants.DefaultBindingPort, Schema = HostSchema.Http }
						);
			//_bindings.Add(

				//			new HostBinding { Interface = HostConstants.DefaultBindingInterface, Port = HostConstants.DefaultBindingPort+1, Schema = HostSchema.Https }
					//	);
		}

		/// <summary>
		/// Коллекция привязок
		/// </summary>
		public IList<HostBinding> Bindings
		{
			get
			{
				return _bindings;
			}
		}

		/// <summary>
		/// Дополнительные параметры
		/// </summary>
		[Serialize]
		public ConfigBase Parameters
		{
			get { return _parameters ?? (_parameters = new ConfigBase()); }
			set { _parameters = value; }
		}

		/// <summary>
		/// Количество тредов
		/// </summary>
		[Serialize]
		public int ThreadCount
		{
			get
			{
				if (0 >= _threadCount)
				{
					_threadCount = HostConstants.DefaultThreadCount;
				}
				return _threadCount;
			}
			set { _threadCount = value; }
		}
		/// <summary>
		/// режим приложения
		/// </summary>
		public HostApplicationMode ApplicationMode { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public LogLevel LogLevel { get; set; }

	    /// <summary>
	    /// Список путей для поиска статического контента
	    /// </summary>
	    public IList<string> ExtendedContentFolders { get; private set; }
	}
}
