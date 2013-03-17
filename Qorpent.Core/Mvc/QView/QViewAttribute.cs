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
// PROJECT ORIGIN: Qorpent.Core/QViewAttribute.cs
#endregion
using System;
using System.Linq;

namespace Qorpent.Mvc.QView {
	/// <summary>
	/// 	can be attached to incode IQView to provide custom name
	/// </summary>
	public class QViewAttribute : Attribute {
		/// <summary>
		/// </summary>
		/// <param name="viewname"> </param>
		public QViewAttribute(string viewname) {
			Name = viewname;
		}

		/// <summary>
		/// </summary>
		/// <param name="viewname"> </param>
		/// <param name="level"> </param>
		public QViewAttribute(string viewname, QViewLevel level) {
			Name = viewname;
			Level = level;
		}

		/// <summary>
		/// 	Name of view
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// 	Explicitly seted level of qview class
		/// </summary>
		public QViewLevel Level { get; private set; }


		/// <summary>
		/// 	Имя файла на диске, с которого ведется компиляция
		/// </summary>
		public string Filename { get; set; }

		/// <summary>
		/// 	Calculates view name
		/// </summary>
		/// <param name="view"> </param>
		/// <returns> </returns>
		public static string GetName(IQView view) {
			var type = view.GetType();
			return GetName(type);
		}

		/// <summary>
		/// 	Calculates viewtype name
		/// </summary>
		/// <param name="type"> </param>
		/// <returns> </returns>
		public static string GetName(Type type) {
			var attr =
				type.GetCustomAttributes(typeof (QViewAttribute), true).OfType<QViewAttribute>().FirstOrDefault();
			if (null != attr) {
				return attr.Name;
			}
			return type.Name.Replace("_", "/"); //way to provide path to view
		}


		/// <summary>
		/// 	Calculates view level
		/// </summary>
		/// <param name="view"> </param>
		/// <returns> </returns>
		public static QViewLevel GetLevel(IQView view) {
			var type = view.GetType();
			return GetLevel(type);
		}

		/// <summary>
		/// 	Calculates viewtype level
		/// </summary>
		/// <param name="type"> </param>
		/// <returns> </returns>
		public static QViewLevel GetLevel(Type type) {
			var attr =
				type.GetCustomAttributes(typeof (QViewAttribute), true).OfType<QViewAttribute>().FirstOrDefault();
			if (null != attr) {
				return attr.Level;
			}
			return QViewLevel.Code;
		}
	}
}