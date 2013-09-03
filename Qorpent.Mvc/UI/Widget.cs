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
// PROJECT ORIGIN: Qorpent.Mvc/Widget.cs
#endregion
using Qorpent.Model;
using Qorpent.Mvc.QView;

namespace Qorpent.Mvc.UI {
	/// <summary>
	/// 	Визуальный виджет при использовании в составле Layout (система виджетов предназначена именно для отрисовки общих страниц)
	/// </summary>
	public class Widget : IWithRole, IWithIndex {
		/// <summary>
		/// 	Код виджета
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		/// 	Название виджета
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 	Зона, в которую должен быть загружен виджет
		/// </summary>
		public string Position { get; set; }

		/// <summary>
		/// 	Перечень необходимых ресурсов (будут использованы как Require)
		/// </summary>
		public string Resources { get; set; }

		//will be included as require
		/// <summary>
		/// 	Имя вида для отрисовки
		/// </summary>
		public string View { get; set; }

		/// <summary>
		/// 	Complex строка с дополнительными данными для View
		/// </summary>
		public string ViewData { get; set; }

		/// <summary>
		/// 	Разрешимая ссылка на картинку (для кнопок)
		/// </summary>
		public string Image { get; set; }

		/// <summary>
		/// 	JS реакция на нажатие (для кнопок)
		/// </summary>
		public string OnClick { get; set; }


		/// <summary>
		/// 	True - виджет независимо от типа сам обеспечивает свою отрисовку (если не указан вид, то через Render)
		/// </summary>
		public bool IsCustom { get; set; }


		/// <summary>
		/// 	Контейнер виджета (секция для <see cref="WidgetType.ToolbarButton" />, закладка для <see
		/// 	 cref="WidgetType.ToolbarSection" />)
		/// </summary>
		public string ToolbarContainer { get; set; }


		/// <summary>
		/// 	Стандартный размер кнопки
		/// </summary>
		public ButtonSize ButtonSize { get; set; }

		/// <summary>
		/// 	Тип виджета
		/// </summary>
		public WidgetType Type { get; set; }

		/// <summary>
		/// 	Индекс виджета
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		/// 	Роль доступа к виджету
		/// </summary>
		public string Role { get; set; }

		/// <summary>
		/// 	Может видоизменить или задать поведение виджета (для Servlet и IsCustom) (потоково защищен)
		/// </summary>
		/// <param name="context"> </param>
		/// <param name="advancedData"> </param>
		public void Render(IQViewContext context, object advancedData) {
			lock (this) {
				InternalRender(context, advancedData);
			}
		}

		/// <summary>
		/// 	При перекрытии задает пользовательский способ отрисовки виджета
		/// </summary>
		/// <param name="context"> </param>
		/// <param name="advancedData"> </param>
		protected void InternalRender(IQViewContext context, object advancedData) {}
	}
}