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
// Original file : IQViewContext.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;
using System.IO;

namespace Qorpent.Mvc.QView {
	/// <summary>
	/// 	Abstract view for QView subsystem
	/// </summary>
	public interface IQViewContext {
		/// <summary>
		/// 	Name of view
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// 	Maste name for view
		/// </summary>
		string Master { get; set; }

		/// <summary>
		/// 	A data source from calling context
		/// </summary>
		object ViewData { get; set; }

		/// <summary>
		/// 	Current MVC context
		/// </summary>
		IMvcContext Context { get; set; }

		/// <summary>
		/// 	Back ref to factory
		/// </summary>
		IMvcFactory Factory { get; set; }

		/// <summary>
		/// 	Output to send
		/// </summary>
		TextWriter Output { get; set; }

		/// <summary>
		/// 	true to send errors to output stream insted of throwing
		/// </summary>
		bool OutputErrors { get; set; }

		/// <summary>
		/// 	Master-only child context for main view
		/// </summary>
		IQViewContext ChildContext { get; }

		/// <summary>
		/// 	some advamced data provided with subview call
		/// </summary>
		object AdvancedData { get; }

		/// <summary>
		/// 	Родительский вид
		/// </summary>
		IQView ParentView { get; set; }

		/// <summary>
		/// 	Для лейаутов - реальный исходящий контекст
		/// </summary>
		TextWriter RealOutPut { get; set; }

		/// <summary>
		/// 	Признк нахождения в контексте Layout
		/// </summary>
		bool IsLayout { get; set; }

		/// <summary>
		/// 	Ссылка на родительский контекст для дочерних по отношению к Layout видов
		/// </summary>
		IQViewContext ParentContext { get; set; }

		/// <summary>
		/// 	Набор ресурсов, необходимых видам от Layout
		/// </summary>
		IList<string> Requirements { get; }

		/// <summary>
		/// 	Access to all-view shared data in master-view-subview stack
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <param name="name"> </param>
		/// <returns> </returns>
		T GetShared<T>(string name);

		/// <summary>
		/// 	Access to write all-view shared data in master-view-subview stack
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="value"> </param>
		void SetShared(string name, object value);

		/// <summary>
		/// 	Generatres subview context
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="advanceddata"> </param>
		/// <returns> </returns>
		IQViewContext CreateSubviewContext(string name, object advanceddata);

		/// <summary>
		/// 	Generates fully-prepared context with master usage and so on
		/// </summary>
		/// <returns> </returns>
		IQViewContext GetNormalizedContext();

		/// <summary>
		/// 	Регистриует потребность дочернего вида в ресурсе
		/// </summary>
		/// <param name="resourceName"> </param>
		void Require(string resourceName);
	}
}