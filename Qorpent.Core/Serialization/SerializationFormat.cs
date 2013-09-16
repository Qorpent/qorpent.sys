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
// PROJECT ORIGIN: Qorpent.Core/SerializationFormat.cs
#endregion

using System;

namespace Qorpent.Serialization {
	/// <summary>
	/// Стандартные форматы сериализации объектов
	/// </summary>
    public enum SerializationFormat {
		/// <summary>
		/// 	Custom format name will be used
		/// </summary>
		Custom =1 ,

		/// <summary>
		/// 	Html
		/// </summary>
		Html =1<<1,

		/// <summary>
		/// 	Plain text
		/// </summary>
		Text =1<<2,

		/// <summary>
		/// 	XML
		/// </summary>
		Xml =1<<3,

		/// <summary>
		/// 	BXL
		/// </summary>
		Bxl =1<<4,

		/// <summary>
		/// 	JSON
		/// </summary>
		Json =1<<5,

		/// <summary>
		/// 	JavaScript
		/// </summary>
		Js =1<<6,

		/// <summary>
		/// 	Md5 hash
		/// </summary>
		Md5 =1<<7,
        /// <summary>
        /// Скрипт на языке Dot
        /// </summary>
        Dot = 1<<8,

        /// <summary>
        /// SVG
        /// </summary>
        Svg= 1<<9,

        /// <summary>
        /// 	По умолчанию XML
        /// </summary>
        Default = Json,
	}
}