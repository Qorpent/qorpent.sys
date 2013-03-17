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
// PROJECT ORIGIN: Qorpent.Data/MetaStorageException.cs
#endregion
using System;

namespace Qorpent.Data.MetaStorage {
	/// <summary>
	/// Общее исключение репозитория Zeta
	/// </summary>
	[Serializable]
	public class MetaStorageException : QorpentException {


		/// <summary>
		/// 
		/// </summary>
		public MetaStorageException() {}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public MetaStorageException(string message) : base(message) {}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public MetaStorageException(string message, Exception inner) : base(message, inner) {}

	}
}