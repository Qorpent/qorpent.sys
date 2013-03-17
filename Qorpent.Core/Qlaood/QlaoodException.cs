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
// PROJECT ORIGIN: Qorpent.Core/QlaoodException.cs
#endregion
using System;
using System.Runtime.Serialization;

namespace Qorpent.Qlaood {
	/// <summary>
	/// 	General qlaood exception
	/// </summary>
	[Serializable]
	public class QlaoodException : Exception {
		//
		// For guidelines regarding the creation of new exception types, see
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
		// and
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
		//

		/// <summary>
		/// </summary>
		public QlaoodException() {}

		/// <summary>
		/// </summary>
		/// <param name="message"> </param>
		public QlaoodException(string message) : base(message) {}

		/// <summary>
		/// </summary>
		/// <param name="message"> </param>
		/// <param name="inner"> </param>
		public QlaoodException(string message, Exception inner) : base(message, inner) {}

		/// <summary>
		/// </summary>
		/// <param name="info"> </param>
		/// <param name="context"> </param>
		protected QlaoodException(
			SerializationInfo info,
			StreamingContext context) : base(info, context) {}
	}
}