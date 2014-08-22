using System;
using System.IO;

namespace Qorpent.BSharp
{
	/// <summary>
	/// Структура, предназначенная для передачи данных о проекте
	/// </summary>
	public class BSharpProjectDescriptor
	{
		/// <summary>
		/// 
		/// </summary>
		public BSharpProjectDescriptor(){
			Branch = "master";
			Name = "default";
		}
		/// <summary>
		/// Адрес директории с локальным репозиторием
		/// </summary>
		public string RepositoryDirectory { get; set; }
		/// <summary>
		/// Адрес внешнего репозитория для клонирования
		/// </summary>
		public string RemoteRepositoryUrl { get; set; }
		/// <summary>
		/// Используемый бранч
		/// </summary>
		public string Branch { get; set; }
		/// <summary>
		/// Имя проекта
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Имя проекта BSPROJ (при работе с BSC)
		/// </summary>
		public string BSProjectName { get; set; }
		/// <summary>
		/// Относительная папка проекта в RepositoryDirectory
		/// </summary>
		public string ProjectDirectory { get; set; }
		/// <summary>
		/// Автоматически настраивает адреса проекта
		/// </summary>
		/// <param name="existedDirectory"></param>
		public void AutoSetup(string existedDirectory = null){
			existedDirectory = existedDirectory ?? Environment.CurrentDirectory;
			int levels = 0;
			var path = existedDirectory;
			while (!Directory.Exists(Path.Combine(path, ".git")))
			{
				levels++;
				path = Path.GetDirectoryName(path);
			}
			RepositoryDirectory = path;	
			var dir = "";
			path = existedDirectory;
			for (var i = 0; i < levels; i++)
			{
				dir = Path.GetFileName(path) + "/" + dir;
				path = Path.GetDirectoryName(path);
			}
			dir = "./" + dir;
			ProjectDirectory = dir;
			
		}
	}
}
