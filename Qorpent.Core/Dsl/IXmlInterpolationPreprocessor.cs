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
// PROJECT ORIGIN: Qorpent.Core/IXmlInterpolationPreprocessor.cs
#endregion
using System.Xml.Linq;

namespace Qorpent.Dsl {
	/// <summary>
	/// 	Вспомогательный интерфейс, позволяющий нормализовать специальные конструкции ${...} в атрибутах и текстовых блоках XML
	/// </summary>
	/// <qorpentimplemented ref="Qorpent.Dsl~Qorpent.Dsl.XmlInterpolationPreprocessor">XmlInterpolationPreprocessor</qorpentimplemented>
	/// <remarks>
	/// 	<invariant>Блоки ${ЛЮБОЙ ТЕКСТ} замещаются уникальными номерными {N} конструкциями, при этом внутренности ${} блока
	/// 		переносятся в подэдементы со специальными именами, позволяет разделить строки на статическую и динамическую
	/// 		часть и свести на основе штатного
	/// 		<see cref="string.Format(string,object[])" />
	/// 	</invariant>
	/// </remarks>
	public interface IXmlInterpolationPreprocessor {
		/// <summary>
		/// 	Выполняет замены в указанном элементе
		/// </summary>
		/// <param name="xml"> Исходный XML документ </param>
		void Execute(XElement xml);
	}
}