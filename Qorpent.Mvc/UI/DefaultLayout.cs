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
// PROJECT ORIGIN: Qorpent.Mvc/DefaultLayout.cs
#endregion
using System.IO;
using Qorpent.IoC;
using Qorpent.Mvc.QView;
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc.UI {
	/// <summary>
	/// 	Базовый оптимизированный лейаут с поддержкой расширения
	/// </summary>
	[QView("layouts/default", QViewLevel.Code, Filename = "CODE/views/layouts/base.vbxl")]
	[ContainerComponent(Name = "layouts/default.code.view", Lifestyle = Lifestyle.Transient, ServiceType = typeof (IQView))
	]
	public class DefaultLayout : QViewBase {
		/// <summary>
		/// 	Подключает прямой режим Layout
		/// </summary>
		public DefaultLayout() {
			UseDirectLayout = true;
		}


		private bool BasicHeaderExists {
			get { return Factory.ViewExists(DefaultHeaderName); }
		}

		private bool BasicBeforeContentExists {
			get { return Factory.ViewExists(DefaultBeforeContent); }
		}

		private bool BasicAfterContentExists {
			get { return Factory.ViewExists(DefaultAfterContent); }
		}

		private bool BasicHeaderEndExists {
			get { return Factory.ViewExists(DefaultHeaderEndName); }
		}

		/// <summary>
		/// 	u must implement it - main user render is executed here
		/// </summary>
		protected override void Render() {
			ViewContext.ChildContext.Output = new StringWriter();
			RenderChild();
			var content = ViewContext.ChildContext.Output.ToString();
			//so, we can get errors before layout rendering and got requirements and different
			//context

			write("<!DOCTYPE html><html><head>");
			if (PageTitle.IsNotEmpty()) {
				writef("<title>{0}</title>", PageTitle);
			}
			if (BasicHeaderExists) {
				Subview(DefaultHeaderName);
			}
			if (null != ViewContext.Requirements) {
				foreach (var requirement in ViewContext.Requirements) {
					RenderLink(requirement, requirement.StartsWith("res:"));
				}
			}
			if (BasicHeaderEndExists) {
				Subview(DefaultHeaderEndName);
			}
			write("</head>");
			write("<body class='qorpent" + (PageBodyClass.IsNotEmpty() ? (" " + PageBodyClass) : "") + "'>");
			if (BasicBeforeContentExists) {
				Subview(DefaultBeforeContent);
			}

			write(content);

			if (BasicAfterContentExists) {
				Subview(DefaultAfterContent);
			}
			write("</body></html>");
		}

		/// <summary>
		/// 	Имя вида, вставляемого после контента
		/// </summary>
		[QViewBind] public string DefaultAfterContent = "/layouts/defaultaftercontent";

		/// <summary>
		/// 	Имя вида, вставляемого перед контентом
		/// </summary>
		[QViewBind] public string DefaultBeforeContent = "/layouts/defaultbeforecontent";

		/// <summary>
		/// 	Имя вида для вставки в качестве нижнего хидера
		/// </summary>
		[QViewBind] public string DefaultHeaderEndName = "/layouts/defaultheaderend";

		/// <summary>
		/// 	Имя вида для вставки в качестве верхнего хидера
		/// </summary>
		[QViewBind] public string DefaultHeaderName = "/layouts/defaultheader";

		/// <summary>
		/// 	Расширительный класс элемента BODY
		/// </summary>
		[QViewBind] protected string PageBodyClass;

		/// <summary>
		/// 	Заголовок страницы
		/// </summary>
		[QViewBind] protected string PageTitle;
	}
}