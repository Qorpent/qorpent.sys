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
// Original file : MvcQuery.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;

namespace Qorpent.Mvc.Remoting {
	/// <summary>
	/// 	Query for Qorpent.MVC remoting
	/// </summary>
	public class MvcQuery {
		/// <summary>
		/// </summary>
		public MvcQuery() {
			Render = MvcStandardRender.Xml;
			Headers = new Dictionary<string, string>();
		}

		/// <summary>
		/// 	Action name
		/// </summary>
		public string Action { get; set; }

		/// <summary>
		/// 	Standard render (to simple acess)
		/// </summary>
		public MvcStandardRender Render {
			get { return _render; }
			set {
				_render = value;
				RenderName = _render.ToString().ToLower();
			}
		}

		/// <summary>
		/// 	Custom HTTP headers set
		/// </summary>
		public IDictionary<string, string> Headers { get; set; }

		/// <summary>
		/// 	Name of custom render (if used)
		/// </summary>
		public string RenderName {
			get { return _renderName; }
			set {
				_renderName = value;
				MvcStandardRender r;
				var st = Enum.TryParse(_renderName, true, out r);
				_render = st ? r : MvcStandardRender.Custom;
			}
		}

		/// <summary>
		/// 	An parameter source (dictionary, anonymous)
		/// </summary>
		public object Parameters { get; set; }


		/// <summary>
		/// 	Prepares given mvc context
		/// </summary>
		/// <param name="urlbase"> </param>
		/// <param name="context"> </param>
		public void Setup(string urlbase, IMvcContext context) {
			context.Uri = new Uri(urlbase + Action.Replace(".", "/") + "." + RenderName.ToLower() + ".qweb");
			if (null != Parameters) {
				if (Parameters is IDictionary<string, object>) {
					foreach (var o in ((IDictionary<string, Object>) Parameters)) {
						context.Set(o.Key, o.Value);
					}
				}
				else if (Parameters is IDictionary<string, string>) {
					foreach (var o in ((IDictionary<string, string>) Parameters)) {
						context.Set(o.Key, o.Value);
					}
				}
				else {
					var props = Parameters.GetType().GetProperties();
					foreach (var p in props) {
						context.Set(p.Name, p.GetValue(Parameters, null));
					}
				}
			}
		}

		private MvcStandardRender _render;
		private string _renderName;
	}
}