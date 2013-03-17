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
// PROJECT ORIGIN: Qorpent.Log/Logger.cs
#endregion
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Log {
	/// <summary>
	/// 	Логер по умолчанию с поддержкой загрузки из XML
	/// </summary>
	public class Logger : BaseLogger {
		/// <summary>
		/// 	Извлекает объект аппендера из элемента
		/// </summary>
		/// <param name="element"> </param>
		/// <returns> </returns>
		protected override ILogWriter GetWriter(XElement element) {
			var desc = element.Describe();
			var writercode = desc.Code;
			var writer = ResolveService<ILogWriter>(writercode);
			if (null == writer) {
				return null;
			}
			var defaultLevel = writer.Level;
			element.Apply(writer); // накатываем перекрытие
			if (defaultLevel > writer.Level) {
				//нельзя понижать уровень Writer
				writer.Level = defaultLevel;
			}
			return writer;
		}
	}
}