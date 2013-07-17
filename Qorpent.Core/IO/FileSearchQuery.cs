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
// PROJECT ORIGIN: Qorpent.Core/FileSearchQuery.cs
#endregion
using System.Text;
using Qorpent.Log;

namespace Qorpent.IO {
	/// <summary>
	/// 	Query for IFileNameResolver
	/// </summary>
	public class FileSearchQuery {
		/// <summary>
		/// 
		/// </summary>
		public FileSearchQuery()
		{
			UseCache = true;
		}
		/// <summary>
		/// 	indicatest that all matched files must be returned
		/// </summary>
		public bool All { get; set; }

		/// <summary>
		/// 	Requested return file path type
		/// </summary>
		public FileSearchResultType PathType { get; set; }

		/// <summary>
		/// 	Return paths only for existed files
		/// </summary>
		public bool ExistedOnly { get; set; }

		/// <summary>
		/// 	Files/masks to search for
		/// </summary>
		public string[] ProbeFiles { get; set; }

		/// <summary>
		/// 	Paths to search in
		/// </summary>
		public string[] ProbePaths { get; set; }

		/// <summary>
		/// 	Custom UserLog listener for current request
		/// </summary>
		public IUserLog UserLog { get; set; }

		/// <summary>
		/// Признак использования кэша
		/// </summary>
		public bool UseCache { get; set; }

		/// <summary>
		/// 	Запрос на поиск одного конкретного файла с разрешением относительно типовых уровней
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="existed"> </param>
		/// <param name="pathType"> </param>
		/// <returns> </returns>
		public static FileSearchQuery Leveled(string name, bool existed = false,
		                                      FileSearchResultType pathType = FileSearchResultType.FullPath) {
			var result = new FileSearchQuery
				{
					ProbeFiles = new[] {name},
					ProbePaths = new[] {"~/usr", "~/mod", "~/sys", "~/"},
					ExistedOnly = existed,
					PathType = pathType
				};
			return result;
		}

		/// <summary>
		/// 	Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
		/// </summary>
		/// <returns> A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" /> . </returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString() {
			var result = new StringBuilder();
			result.Append("All:" + All);
			result.Append(",");
			result.Append(PathType);
			result.Append(",");
			result.Append(ExistedOnly);
			result.Append(":");
			if (null != ProbeFiles) {
				foreach (var probeFile in ProbeFiles) {
					result.Append(probeFile);
					result.Append(";");
				}
			}
			result.Append(" in ");
			if (null != ProbePaths) {
				foreach (var probePath in ProbePaths) {
					result.Append(probePath);
					result.Append(";");
				}
			}
			return result.ToString();
		}
	}
}