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
// PROJECT ORIGIN: Qorpent.Mvc/WidgetType.cs
#endregion
namespace Qorpent.Mvc.UI {
	/// <summary>
	/// 	Типы виджетов
	/// </summary>
	public enum WidgetType {
		/// <summary>
		/// 	Неопределенный тип
		/// </summary>
		None,

		/// <summary>
		/// 	Контент прямо указан
		/// </summary>
		Static,

		/// <summary>
		/// 	Указано имя вида для виджета
		/// </summary>
		View,

		/// <summary>
		/// 	указан запрос и параметры, который выполняется в Ajax режиме после загрузки страницы
		/// </summary>
		Ajax,

		/// <summary>
		/// 	Кнопка
		/// </summary>
		Button,

		/// <summary>
		/// 	Раздел тулбара (закладка)
		/// </summary>
		ToolbarTab,

		/// <summary>
		/// 	Раздел тулбара (секция)
		/// </summary>
		ToolbarSection,

		/// <summary>
		/// 	Раздел тулбара (секция)
		/// </summary>
		ToolbarButton,

		/// <summary>
		/// 	Элемент статусной строки
		/// </summary>
		StatusItem,

		/// <summary>
		/// 	указан класс со специальными интерфейсом, ему передается обратная ссылка на вид и на контроллер
		/// </summary>
		Servlet,
	}
}