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
// PROJECT ORIGIN: Qorpent.Core/DslMessage.cs
#endregion
using System;
using System.Text;

namespace Qorpent.Dsl {
	/// <summary>
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class DslMessage {
		/// <summary>
		/// 	Gets or sets the error level.
		/// </summary>
		/// <value> The error level. </value>
		/// <remarks>
		/// </remarks>
		public ErrorLevel ErrorLevel { get; set; }

		/// <summary>
		/// 	Gets or sets the message.
		/// </summary>
		/// <value> The message. </value>
		/// <remarks>
		/// </remarks>
		public string Message { get; set; }

		/// <summary>
		/// 	Gets or sets the lex info.
		/// </summary>
		/// <value> The lex info. </value>
		/// <remarks>
		/// </remarks>
		public LexInfo LexInfo { get; set; }

		/// <summary>
		/// 	Gets or sets the error code.
		/// </summary>
		/// <value> The error code. </value>
		/// <remarks>
		/// </remarks>
		public string ErrorCode { get; set; }

		/// <summary>
		/// 	Gets or sets the exception.
		/// </summary>
		/// <value> The exception. </value>
		/// <remarks>
		/// </remarks>
		public Exception Exception { get; set; }

		/// <summary>
		/// 	Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
		/// </summary>
		/// <returns> A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" /> . </returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString() {
			var sb = new StringBuilder();
			sb.AppendFormat("{0} {1} {2} {3}", ErrorCode, ErrorLevel, Message, LexInfo);
			if (null != Exception) {
				sb.AppendLine();
				sb.Append(Exception);
			}
			return sb.ToString();
		}
	}
}