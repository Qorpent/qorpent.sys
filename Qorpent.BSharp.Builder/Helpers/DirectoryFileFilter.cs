﻿using System.IO;
using System.Linq;
using System.Collections.Generic;
using Qorpent.BSharp.Builder;
using Qorpent.Utils.Extensions;

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
				Roots = project.GetSourceDirectories().ToArray(),
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
	    public string[] Roots { get; set; }

	    /// <summary>
		///     Поиск всех вхождений по указанным инклукдам
		/// </summary>
		/// <returns>
		///     Перечисление путей файлов, подходящих по маске
		///     к списку инклудов
		/// </returns>
		public IEnumerable<string> Collect() {
	        var directincludes = Includes.Where(File.Exists).Select(_=>_.NormalizePath()).ToArray();
	        foreach (var directinclude in directincludes) {
	            yield return directinclude;
	        }
		    foreach (var root in Roots){

			    foreach (var mask in SearchMasks){
				    foreach (var f in
					    Directory.GetFiles(root, mask, SearchOption.AllDirectories)
					    ){
					    var normalized = f.Replace(root, "").Replace("\\", "/");
					    if (Includes.Any()){
                            if(directincludes.Contains(normalized))continue;
					        
						    if (!Includes.Any(normalized.Contains)) continue;
					    }
					    if (Excludes.Any()){
						    if (Excludes.Any(normalized.Contains)) continue;
					    }
					    yield return f;
				    }
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
