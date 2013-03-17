#region LICENSE
// Copyright 2007-2013 Qorpent Team - http://github.com/Qorpent
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// PROJECT ORIGIN: Qorpent.Mvc/QViewCompilerHelper.cs
#endregion
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc.QView {
	/// <summary>
	/// 	Утилитный класс для компилятора QView, обеспечивает целостность имен и т.п.
	/// </summary>
	public static class QViewCompilerHelper {
		/// <summary>
		/// 	Определяет уровень расположения View
		/// </summary>
		/// <param name="path"> путь к локальному файлу </param>
		/// <param name="root"> корневая папка </param>
		/// <returns> уровень указанного файла </returns>
		public static QViewLevel GetLevel(string path, string root) {
			path = path.Replace("\\", "/");
			path = path.NormalizePath();
			root = root.NormalizePath();
			var local = path.Replace(root, "");
			if (local.StartsWith("/")) {
				local = local.Substring(1);
			}
			local = local.Replace("/qviews/", "/");
			local = local.Replace("/views/", "/");
			local = local.Replace("/qview/", "/");
			local = local.Replace("~qviews/", "~");
			local = local.Replace("~views/", "~");
			local = local.Replace("~qview/", "~");
			if (local.StartsWith("usr/")) {
				return QViewLevel.Usr;
			}
			if (local.StartsWith("mod/")) {
				return QViewLevel.Mod;
			}
			if (local.StartsWith("sys/")) {
				return QViewLevel.Sys;
			}
			return QViewLevel.Root;
		}

		/// <summary>
		/// 	Определяет  локальное имя вьюхи из пути (включая путь)
		/// </summary>
		/// <param name="path"> </param>
		/// <param name="root"> </param>
		/// <returns> </returns>
		public static string GetViewName(string path, string root) {
			path = path.Replace("\\", "/");
			path = path.NormalizePath();
			root = root.NormalizePath();
			var local = path.Replace(root, "");
			if (local.StartsWith("/")) {
				local = local.Substring(1);
			}
			local = "~" + local;
			local = local.Replace("/qviews/", "/");
			local = local.Replace("/views/", "/");
			local = local.Replace("/qview/", "/");


			local = local.Replace("~qviews/", "~");
			local = local.Replace("~views/", "~");
			local = local.Replace("~qview/", "~");
			local = local.Replace("~usr/", "");
			local = local.Replace("~mod/", "");
			local = local.Replace("~sys/", "");
			local = local.Replace(".vbxl", "");
			//local = local.Replace("/", ".");
			local = local.Replace("~", "");
			return local;
		}

		/// <summary>
		/// 	Определяет  локальное имя класса
		/// </summary>
		/// <param name="path"> </param>
		/// <param name="root"> </param>
		/// <returns> </returns>
		public static string GetClsName(string path, string root) {
			return GetViewName(path, root).Replace("/", "_0_");
		}
	}
}