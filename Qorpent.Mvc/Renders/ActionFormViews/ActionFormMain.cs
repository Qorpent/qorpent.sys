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
// PROJECT ORIGIN: Qorpent.Mvc/ActionFormMain.cs
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
		private static bool _resourceLoaded;
		private static readonly object ResourceLock = new object();
		private static readonly IDictionary<string, string> StaticResources = new Dictionary<string, string>();

		/// <summary>
		/// 	Отрисовывает форму с параметрами действия и результатом выполнения
		/// </summary>
		protected override void Render() {
			Require("res:Qorpent.Mvc/actionform.css");
			Require("res:Qorpent.Mvc/actionform.js");
			Require("actionform-ex.css");
			Require("actionform-ex.js");
			_binders = Data.GetBindings().ToArray();
			Help = Help ?? "";
			Name = Name ?? "";
			write(
				new XElement("header",
				             new XElement("h1",
				                          string.Format(GetResource("titlestart"), Name)
					             )
					).ToString()
				);


			write(
				new XElement("section",
				             new XElement("h2", GetResource("helptitle")),
				             new XElement("p", Help.Replace(";", "<BR/>")),
				             new XElement("form",
				                          new XAttribute("id", "formcall"),
				                          new XAttribute("actionname", Name),
				                          new XAttribute("target", "formresult"),
				                          new XAttribute("method", "POST"),
				                          new XElement("table",
				                                       _binders.SelectMany(p => XhtmlSubview("actionformparameter", new {p})))
					             ),
				             new XElement("select", new XAttribute("id", "formrender"),
											
				                          from item in Container.All<IRender>().OrderBy(RenderAttribute.GetName)
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
				                                       new XAttribute("name", "formresult"),
													   new XComment("<!-- fill -->"))
					             ),
							new XElement("script",new XAttribute("type","text/javascript"),
								new XComment("\r\ndocument.getElementById('formrender').value='"+GetDefaultRender()+"';\r\n"))
					).ToString()
				);
		}

		private object GetDefaultRender() {
			var render = ViewContext.Context.Get("render");
			if (string.IsNullOrWhiteSpace(render)) {
				return "xml";
			}
			return render;
		}

		/// <summary>
		/// 	INTERNAL USAGE: OVERRIDET INTERNALLY IN VBXL
		/// </summary>
		/// <returns> </returns>
		protected override IDictionary<string, string> _getResources() {
			return StaticResources;
		}

		/// <summary>
		/// 	INTERNAL USAGE: OVERRIDET INTERNALLY IN VBXL
		/// </summary>
		/// <returns> </returns>
		protected override bool _getResourceLoaded() {
			return _resourceLoaded;
		}

		/// <summary>
		/// 	INTERNAL USAGE: OVERRIDET INTERNALLY IN VBXL
		/// </summary>
		/// <returns> </returns>
		protected override void _setResourceLoaded() {
			_resourceLoaded = true;
		}

		/// <summary>
		/// 	INTERNAL USAGE: OVERRIDET INTERNALLY IN VBXL
		/// </summary>
		/// <returns> </returns>
		protected override object _getResourceLock() {
			return ResourceLock;
		}

		[QViewBind] private BindAttribute[] _binders;

		/// <summary>
		/// 	Содержит данные по дескриптору в целом
		/// </summary>
		[ViewData] protected ActionDescriptor Data;
		/// <summary>
		/// Строка помощи действия
		/// </summary>
		[QViewBind] protected string Help;
		/// <summary>
		/// Имя действия
		/// </summary>
		[QViewBind] protected string Name;
	}
}