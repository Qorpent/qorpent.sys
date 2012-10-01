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
// Original file : ActionFormMain.cs
// Project: Qorpent.Mvc
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.IoC;
using Qorpent.Mvc.Binding;
using Qorpent.Mvc.QView;

namespace Qorpent.Mvc.Renders.ActionFormViews {
	/// <summary>
	/// 	Главный вид HTML формы действий
	/// </summary>
	[QView("_sys/actionform")]
	[ContainerComponent(Name = "_sys/actionform.code.view", Lifestyle = Lifestyle.Transient, ServiceType = typeof (IQView))
	]
	public class ActionFormMain : QViewBase {
		private static bool _resource_loaded;
		private static readonly object _resource_lock = new object();
		private static readonly IDictionary<string, string> _static_resources = new Dictionary<string, string>();

		/// <summary>
		/// 	Отрисовывает форму с параметрами действия и результатом выполнения
		/// </summary>
		protected override void Render() {
			Require("res:Qorpent.Mvc/actionform.css");
			Require("res:Qorpent.Mvc/actionform.js");
			binders = data.GetBindings().ToArray();
			help = help ?? "";
			name = name ?? "";
			write(
				new XElement("header",
				             new XElement("h1",
				                          string.Format(GetResource("titlestart"), name)
					             )
					).ToString()
				);


			write(
				new XElement("section",
				             new XElement("h2", GetResource("helptitle")),
				             new XElement("p", help.Replace(";", "<BR/>")),
				             new XElement("form",
				                          new XAttribute("id", "formcall"),
				                          new XAttribute("actionname", name),
				                          new XAttribute("target", "formresult"),
										  new XAttribute("method", "POST"),
				                          new XElement("table",
				                                       binders.SelectMany(p => XhtmlSubview("actionformparameter", new {p})))
					             ),
				             new XElement("select", new XAttribute("id", "formrender"),
				                          from item in Container.All<IRender>().OrderBy(x => RenderAttribute.GetName(x))
				                          select
					                          new XElement("option",
					                                       new XAttribute("value",
					                                                      RenderAttribute.GetName(item).ToLower().Replace("render", "")),
					                                       new XText(RenderAttribute.GetName(item).ToUpper().Replace("RENDER", ""))
					                          )
					             ),
				             new XElement("input", new XAttribute("type", "button"), new XAttribute("value", GetResource("submit")),
				                          new XAttribute("onclick", "actionform.submit(document.querySelector('#formcall'))")
					             ),
				             new XElement("section",
				                          new XElement("iframe", new XAttribute("id", "formresult"),
				                                       new XAttribute("name", "formresult"))
					             )
					).ToString()
				);
		}

		/// <summary>
		/// 	INTERNAL USAGE: OVERRIDET INTERNALLY IN VBXL
		/// </summary>
		/// <returns> </returns>
		protected override IDictionary<string, string> _getResources() {
			return _static_resources;
		}

		/// <summary>
		/// 	INTERNAL USAGE: OVERRIDET INTERNALLY IN VBXL
		/// </summary>
		/// <returns> </returns>
		protected override bool _getResourceLoaded() {
			return _resource_loaded;
		}

		/// <summary>
		/// 	INTERNAL USAGE: OVERRIDET INTERNALLY IN VBXL
		/// </summary>
		/// <returns> </returns>
		protected override void _setResourceLoaded() {
			_resource_loaded = true;
		}

		/// <summary>
		/// 	INTERNAL USAGE: OVERRIDET INTERNALLY IN VBXL
		/// </summary>
		/// <returns> </returns>
		protected override object _getResourceLock() {
			return _resource_lock;
		}

		[QViewBind] private BindAttribute[] binders;

		/// <summary>
		/// 	Содержит данные по дескриптору в целом
		/// </summary>
		[ViewData] protected ActionDescriptor data;

		[QViewBind] private string help;
		[QViewBind] private string name;
	}
}