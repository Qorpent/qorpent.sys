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
// PROJECT ORIGIN: Qorpent.Core/SerializeAttribute.cs
#endregion
using System;

namespace Qorpent.Serialization {
	/// <summary>
	/// 	Marks that given class is have to be serialized
	/// </summary>
	[AttributeUsage(
		AttributeTargets.Class | AttributeTargets.Struct| AttributeTargets.Interface | AttributeTargets.Property | AttributeTargets.Field,
		AllowMultiple = false, Inherited = true)]
	public class SerializeAttribute : Attribute {
		/// <summary>
		///		collection element name
		/// </summary>
		public string ItemName { get; set; }
		/// <summary>
		/// 	Forces renaming during serialization to camel-case usefull for Json/Js
		/// </summary>
		public bool CamelNames { get; set; }
	}
}