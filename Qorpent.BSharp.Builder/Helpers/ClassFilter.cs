using System;
using System.Linq;
using Qorpent.BSharp.Builder;

namespace Qorpent.Integration.BSharp.Builder.Helpers {
	/// <summary>
	/// Фильтр для пространств имен, работает с полными именами и aaa.bbb.*
	/// </summary>
	public class ClassFilter {
		/// <summary>
		/// Процедура фильтрации входных файлов на уровне классов и пространств имен
		/// </summary>
		/// <param name="project"></param>
		public static ClassFilter Create(IBSharpProject project)
		{
			return new ClassFilter
			{
				NamespaceFullIncludes = project.Targets.Namespaces.Where(_ =>
					!_.Key.EndsWith(".*") && 
					_.Value == BSharpBuilderTargetType.Include)
				.Select(_ => _.Key).ToArray(),
				NamespaceStartIncludes = project.Targets.Namespaces.Where(_ =>
					_.Key.EndsWith(".*") &&
					_.Value == BSharpBuilderTargetType.Include)
					.Select(_ => _.Key.Replace(".*","")).ToArray(),
				NamespaceFullExcludes = project.Targets.Namespaces.Where(_ => 
					!_.Key.EndsWith(".*") &&
					_.Value == BSharpBuilderTargetType.Exclude).Select(_ => _.Key).ToArray(),
				NamespaceStartExcludes = project.Targets.Namespaces.Where(_ =>
					_.Key.EndsWith(".*") &&
					_.Value == BSharpBuilderTargetType.Exclude).Select(_ => 
						_.Key.Replace(".*", "")).ToArray(),
				ClassExcludes =  project.Targets.Classes.Where(_ =>
					_.Value == BSharpBuilderTargetType.Exclude).Select(_ => _.Key).ToArray(),
				ClassIncludes = project.Targets.Classes.Where(_ =>
					_.Value == BSharpBuilderTargetType.Include).Select(_ => _.Key).ToArray(),
				
			};
		}

		/// <summary>
		/// Признак необходимости применения фильтра
		/// </summary>
		public bool HasConditions {
			get {
				return
					NamespaceStartExcludes.Length != 0
					||
					NamespaceStartIncludes.Length != 0
					||
					ClassExcludes.Length != 0
					||
					ClassIncludes.Length != 0
					||
					NamespaceFullExcludes.Length != 0
					||
					NamespaceFullIncludes.Length != 0;
			}
		}

		/// <summary>
		/// Включения по классу
		/// </summary>
		public string[] ClassIncludes { get; set; }

		/// <summary>
		/// Исключения по классу
		/// </summary>
		public string[] ClassExcludes { get; set; }

		/// <summary>
		/// Исключения по началу Namespace
		/// </summary>
		public string[] NamespaceStartExcludes { get; set; }
		/// <summary>
		/// Исключения по полному Namespace
		/// </summary>
		public string[] NamespaceFullExcludes { get; set; }

		/// <summary>
		/// Включения по началу пространств имен
		/// </summary>
		public string[] NamespaceStartIncludes { get; set; }
		/// <summary>
		/// Включения по полному простнаству имени
		/// </summary>
		public string[] NamespaceFullIncludes { get; set; }


		/// <summary>
		/// Проверяет доступность имени пространства имен
		/// </summary>
		/// <param name="ns"></param>
		/// <returns></returns>
		public bool IsAvailableNamespace(string ns) {
			if (0 != NamespaceFullExcludes.Length) {
				if (NamespaceFullExcludes.Contains(ns)) return false;
			}
			if (0 != NamespaceFullIncludes.Length) {
				if (!NamespaceFullIncludes.Contains(ns)) return false;
			}
			if (0 != NamespaceStartExcludes.Length) {
				if (NamespaceStartExcludes.Any(ns.StartsWith)) return false;
			}
			if (0 != NamespaceStartIncludes.Length) {
				if (!NamespaceStartIncludes.Any(ns.StartsWith)) return false;
			}
			return true;
		}
		/// <summary>
		/// Проверяет доступность имени класса
		/// </summary>
		/// <param name="classname"></param>
		/// <returns></returns>
		public bool IsAvailableClassname(string classname) {
			if (classname.Contains(".")) {
				var lastdot = classname.LastIndexOf('.');
				var ns = classname.Substring(0, lastdot - 1);
				var cls = classname.Substring(lastdot);
				return IsAvailableNamespace(ns) && IsAvailableClassname(cls);
			}
			if (ClassExcludes.Length != 0) {
				if (ClassExcludes.Contains(classname)) return false;
			}
			if (ClassIncludes.Length!=0) {
				if (!ClassIncludes.Contains(classname)) return false;
			}
			return true;
		}
		
	}
}