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
// PROJECT ORIGIN: Qorpent.Core/LicenseAttribute.cs
#endregion
using System;

namespace Qorpent {
	/// <summary>
	/// 	Маркер лицензии сборки
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = true)]
	public class LicenseAttribute : Attribute {
		/// <summary>
		/// 	Главный конструктор, совместимый с MSBUILD
		/// </summary>
		/// <param name="licenseType"> </param>
		/// <param name="licenseUrl"> </param>
		/// <param name="licenseDescription"> </param>
		public LicenseAttribute(string licenseType, string licenseUrl, string licenseDescription) {
			LicenseType = licenseType;
			LicenseUrl = licenseUrl;
			LicenseDescription = licenseDescription;
		}

		/// <summary>
		/// 	Тип лицензии
		/// </summary>
		public string LicenseType { get; private set; }

		/// <summary>
		/// 	Ссылка на файл лицензии
		/// </summary>
		public string LicenseUrl { get; private set; }

		/// <summary>
		/// 	Краткое описание лицензии
		/// </summary>
		public string LicenseDescription { get; private set; }
	}
}