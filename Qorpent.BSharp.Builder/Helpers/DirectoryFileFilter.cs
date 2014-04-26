using System.IO;
using System.Linq;
using System.Collections.Generic;
using Qorpent.BSharp.Builder;

namespace Qorpent.Integration.BSharp.Builder.Helpers {
	/// <summary>
    ///     Класс ля резольвинга инклудов и эксклудов
    /// </summary>
    public class DirectoryFileFilter {
		/// <summary>
		/// Процедура инициализации из проекта
		/// </summary>
		/// <param name="project"></param>
		public static DirectoryFileFilter Create(IBSharpProject project) {
			return new DirectoryFileFilter {
				Root = project.GetRootDirectory(),
				SearchMasks =project.InputExtensions.Split(';').Select(_=>"*."+_).ToArray(),
				Includes = project.Targets.Paths.Where(_ => _.Value == BSharpBuilderTargetType.Include).Select(_ => _.Key).ToArray(),
				Excludes = project.Targets.Paths.Where(_ => _.Value == BSharpBuilderTargetType.Exclude).Select(_ => _.Key).ToArray()
			};
		}
		/// <summary>
		/// Маска поиска файлов
		/// </summary>
	    public string[] SearchMasks { get; set; }

	    /// <summary>
		/// Корневая директория
		/// </summary>
	    public string Root { get; set; }

	    /// <summary>
		///     Поиск всех вхождений по указанным инклукдам
		/// </summary>
		/// <returns>
		///     Перечисление путей файлов, подходящих по маске
		///     к списку инклудов
		/// </returns>
		public IEnumerable<string> Collect()
		{
			foreach (var mask in SearchMasks)
			{
				foreach (var f in
						Directory.GetFiles(Root, mask, SearchOption.AllDirectories)
					)
				{
					var normalized = f.Replace(Root, "").Replace("\\", "/");
					if (Includes.Any())
					{
						if (!Includes.Any(normalized.Contains)) continue;
					}
					if (Excludes.Any())
					{
						if (Excludes.Any(normalized.Contains)) continue;
					}
					yield return f;
				} 
			}
		}
		/// <summary>
		/// Фильтры на включение
		/// </summary>
	    public string[] Includes { get; set; }
		/// <summary>
		/// Фильтры на исключение
		/// </summary>
		public string[] Excludes { get; set; }
    }
}
