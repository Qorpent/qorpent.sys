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
// PROJECT ORIGIN: Qorpent.Core/BxlGeneratorOptions.cs
#endregion
namespace Qorpent.Bxl {
	/// <summary>
	/// 	ќпци€ дл€ создани€ BXL из XML
	/// </summary>
	public class BxlGeneratorOptions {
		/// <summary>
		/// 	Ќабор высокоприоритетных атрибутов, пор€док которых будет примен€тьс€ перед алфавитым.
		/// </summary>
		public string[] FirstPlaceAttributes = new[] {"id", "code", "name"};

		/// <summary>
		/// 	Ќабор атрибутов которые всегда должны быть представлены в той-же строке, что и элемент начала
		/// </summary>
		public string[] InlineAlwaysAttributes = new[] {"id", "code", "name", "_file", "_line", "idx"};

		/// <summary>
        /// 	≈сли возможно - представл€ет атрибуты в той-же строке, что и элемент начала
		/// </summary>
		public bool InlineAttributesByDefault;

		/// <summary>
		/// 	Ќабор атрибутов которые всегда вынос€тс€ в отдельную строку
		/// </summary>
		public string[] NewlineAlwaysAttributes = new string[] {};

		/// <summary>
		/// 	ѕредотвращает создание корневых элементов (“олько первоуровневые дочерние элементы будут представлены)
		/// </summary>
		public bool NoRootElement;

		/// <summary>
		/// 	Ќабор атрибутов, которые должны быть пропущены во врем€ создани€
		/// </summary>
		public string[] SkipAttributes = new string[] {};

		/// <summary>
		/// 	ѕринудительно безопасное использование тройных кавычек в строке при представлении значени€
		/// </summary>
		public bool UseTrippleQuotOnValues;
	}
}