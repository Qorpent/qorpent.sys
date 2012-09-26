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
// Original file : QViewContext.cs
// Project: Qorpent.Mvc
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc.QView {
	/// <summary>
	/// 	Default QViewContext implementation
	/// </summary>
	[ContainerComponent(Lifestyle.Transient)]
	public class QViewContext : IQViewContext {
		private string Root { get; set; }
		private IDictionary<string, object> SharedData { get; set; }


		/// <summary>
		/// 	Name of view
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 	Maste name for view
		/// </summary>
		public string Master { get; set; }

		/// <summary>
		/// 	A data source from calling context
		/// </summary>
		public object ViewData { get; set; }

		/// <summary>
		/// 	Current MVC context
		/// </summary>
		public IMvcContext Context { get; set; }

		/// <summary>
		/// 	Back ref to factory
		/// </summary>
		public IMvcFactory Factory { get; set; }

		/// <summary>
		/// 	Output to send
		/// </summary>
		public TextWriter Output { get; set; }

		/// <summary>
		/// 	true to send errors to output stream insted of throwing
		/// </summary>
		public bool OutputErrors { get; set; }

		/// <summary>
		/// 	Master-only child context for main view
		/// </summary>
		public IQViewContext ChildContext { get; private set; }

		/// <summary>
		/// 	some advamced data provided with subview call
		/// </summary>
		public object AdvancedData { get; private set; }

		/// <summary>
		/// 	Родительский вид
		/// </summary>
		public IQView ParentView { get; set; }

		/// <summary>
		/// 	Access to all-view shared data in master-view-subview stack
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <param name="name"> </param>
		/// <returns> </returns>
		public T GetShared<T>(string name) {
			if (null == SharedData) {
				return default(T);
			}
			if (!SharedData.ContainsKey(name)) {
				return default(T);
			}
			return SharedData[name].To<T>();
		}

		/// <summary>
		/// 	Access to write all-view shared data in master-view-subview stack
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="value"> </param>
		public void SetShared(string name, object value) {
			if (null == SharedData) {
				SharedData = new Dictionary<string, object>();
			}
			SharedData[name] = value;
		}

		/// <summary>
		/// 	Generatres subview context
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="advanceddata"> </param>
		/// <returns> </returns>
		public IQViewContext CreateSubviewContext(string name, object advanceddata) {
			var result = MemberwiseClone() as QViewContext;
			result.Root = Name;
			result.Name = name;
			result.AdvancedData = advanceddata;
			result.Master = null;
			result.IsLayout = false;
			if (IsLayout) {
				result.ParentContext = this;
			}
			result.NormalizeName();
			return result;
		}

		/// <summary>
		/// 	Generates fully-prepared context with master usage and so on
		/// </summary>
		/// <returns> </returns>
		public IQViewContext GetNormalizedContext() {
			if (Master.IsNotEmpty()) {
				return GetLayoutedContext();
			}
			if (Root.IsNotEmpty() || !Name.StartsWith("/")) {
				var result = MemberwiseClone() as QViewContext;
				Debug.Assert(result != null, "result != null");
				result.NormalizeName();
				return result;
			}

			return this;
		}

		/// <summary>
		/// 	Регистриует потребность дочернего вида в ресурсе
		/// </summary>
		/// <param name="resourceName"> </param>
		public void Require(string resourceName) {
			if (IsLayout) {
				requirements = Requirements ?? new List<string>();
				if (Requirements.Contains(resourceName)) {
					return;
				}
				Requirements.Add(resourceName);
			}
			else if (null != ParentContext) {
				ParentContext.Require(resourceName);
			}
		}


		/// <summary>
		/// 	Ссылка на родительский контекст для дочерних по отношению к Layout видов
		/// </summary>
		public IQViewContext ParentContext { get; set; }

		/// <summary>
		/// 	Для лейаутов - реальный исходящий контекст
		/// </summary>
		public TextWriter RealOutPut { get; set; }


		/// <summary>
		/// 	Признк нахождения в контексте Layout
		/// </summary>
		public bool IsLayout { get; set; }

		/// <summary>
		/// 	Список потребностей видов в ресурсах на уровне Layout
		/// </summary>
		public IList<string> Requirements {
			get { return requirements; }
		}

		private QViewContext GetLayoutedContext() {
			var mastercontext = MemberwiseClone() as QViewContext;
			mastercontext.RealOutPut = mastercontext.Output;
			mastercontext.Output = new StringWriter();
			mastercontext.IsLayout = true;
			var childcontext = MemberwiseClone() as QViewContext;
			childcontext.Output = mastercontext.Output;
			Debug.Assert(childcontext != null, "childcontext != null");
			childcontext.Master = null;
			childcontext.ParentContext = mastercontext;
			Debug.Assert(mastercontext != null, "mastercontext != null");
			mastercontext.Name = "/layouts/" + Master;
			mastercontext.Root = mastercontext.Name;
			mastercontext.ChildContext = childcontext;
			mastercontext.Master = null;
			return mastercontext;
		}

		private void NormalizeName() {
			if (Name.StartsWith("/")) {
				return;
			}
			var dir = "/";
			if (Root.IsNotEmpty()) {
				dir = Root.SmartSplit(false, true, '/')[0];
			}
			var result = "/" + dir + "/" + Name;
			result = result.Replace("//", "/");
			result = result.Replace("//", "/"); // /// tripple can be created
			if (result.Contains("..")) {
				result = Regex.Replace(result, @"[^/]*/\.\.", "", RegexOptions.Compiled);
			}
			result = result.Replace("//", "/");
			Name = result;
		}

		private IList<string> requirements;
	}
}