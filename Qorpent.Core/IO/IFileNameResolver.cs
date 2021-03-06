﻿#region LICENSE
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
// PROJECT ORIGIN: Qorpent.Core/IFileNameResolver.cs
#endregion
namespace Qorpent.IO {
	/// <summary>
	/// 	Describes abstract filename resolver
	/// </summary>
	public interface IFileNameResolver {
		/// <summary>
		/// 	Own root of fileresolver
		/// </summary>
		string Root { get; set; }

		/// <summary>
		/// 	Resolves first file that match query
		/// </summary>
		/// <param name="query"> search query </param>
		/// <returns> </returns>
		string Resolve(FileSearchQuery query);

		/// <summary>
		/// 	Resolves all files match query
		/// </summary>
		/// <param name="query"> search query </param>
		/// <returns> </returns>
		string[] ResolveAll(FileSearchQuery query);

		/// <summary>
		/// 	Clears file resolution cache
		/// </summary>
		void ClearCache();
	}
}