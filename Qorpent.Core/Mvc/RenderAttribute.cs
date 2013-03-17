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
// PROJECT ORIGIN: Qorpent.Core/RenderAttribute.cs
#endregion
using System;
using System.Linq;
using Qorpent.IoC;

namespace Qorpent.Mvc {
	/// <summary>
	/// 	Marks class as Render, can be atached to non-abstract IRender implementation type.
	/// 	Render name is an uniques string which will be used in pre .qweb part of Request URLs
	/// </summary>
	public class RenderAttribute : ContainerComponentAttribute {
		/// <summary>
		/// 	creates new attribute with given render name
		/// </summary>
		/// <param name="name"> render name </param>
		public RenderAttribute(string name) {
			Name = name.Replace(".render", "") + ".render"; //нормализуем имя
			Lifestyle = Lifestyle.Transient;
			ServiceType = typeof (IRender);
		}


		/// <summary>
		/// 	Retrieves short name of render name which is used in  mvc call
		/// </summary>
		/// <returns> </returns>
		public static string GetName(IRender render) {
			var type = render.GetType();
			return GetName(type);
		}


		/// <summary>
		/// 	Get role of given action (attribute-based)
		/// </summary>
		/// <param name="render"> </param>
		/// <returns> </returns>
		public static string GetRole(IRender render) {
			var type = render.GetType();
			var attr =
				type.GetCustomAttributes(typeof (RenderAttribute), true).OfType<RenderAttribute>().FirstOrDefault();
			if (null == attr) {
				return "";
			}
			return attr.Role;
		}

		/// <summary>
		/// </summary>
		/// <param name="type"> </param>
		/// <returns> </returns>
		public static string GetName(Type type) {
			var attr =
				type.GetCustomAttributes(typeof (RenderAttribute), true).OfType<RenderAttribute>().FirstOrDefault();
			return null == attr
				       ? type.Name.Replace("_", ".").Replace(".render", "").Replace("Render", "").ToLower()
				       : attr.Name.ToLower().Replace(".render", "");
		}
	}
}