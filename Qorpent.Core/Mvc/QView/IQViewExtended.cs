#region LICENSE

// Copyright 2007-2012 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Solution: Qorpent
// Original file : IQViewExtended.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.IO;

namespace Qorpent.Mvc.QView {
	/// <summary>
	/// 	Расширенные возможности видов
	/// </summary>
	public interface IQViewExtended : IQView {
		/// <summary>
		/// 	Renders local url to named resource with file resolution
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="prepared"> Признак подготовленности ссылки </param>
		/// <exception cref="NullReferenceException"></exception>
		void RenderLink(string name, bool prepared = false);

		/// <summary>
		/// 	Retrieves resource string from special-formed resources
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="lang"> </param>
		/// <returns> </returns>
		string GetResource(string name, string lang = null);

		/// <summary>
		/// 	allows to catch content in temporal stream
		/// </summary>
		void EnterTemporaryOutput(TextWriter output = null);

		/// <summary>
		/// 	retrieves catched content
		/// </summary>
		/// <returns> </returns>
		string GetTemporaryOutput();

		/// <summary>
		/// 	восстанавливает стандартный оутпут
		/// </summary>
		/// <returns> </returns>
		void RestoreOutput();
	}
}