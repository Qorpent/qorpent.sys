// Copyright 2007-2014  Qorpent Team - http://github.com/Qorpent
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
// Created : 2014-09-02

using System;

namespace Qorpent.IO.Net{
	/// <summary>
	///     Ошибки, связанные с разбором HTTP
	/// </summary>
	[Serializable]
	public class HttpReaderException : IOException{
		/// <summary>
		/// </summary>
		public HttpReaderException(){
		}

		/// <summary>
		/// </summary>
		/// <param name="message"></param>
		public HttpReaderException(string message) : base(message){
		}

		/// <summary>
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public HttpReaderException(string message, Exception inner) : base(message, inner){
		}
	}
}