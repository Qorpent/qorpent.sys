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
// PROJECT ORIGIN: Qorpent.Core/LogHelper.cs
#endregion
using System;
using Qorpent.Dsl;
using Qorpent.Mvc;

namespace Qorpent.Log {
	/// <summary>
	/// 	Helper class, providing mechanism for encapsulating objects into LogMessages
	/// </summary>
	public class LogHelper {
		/// <summary>
		/// 	Converts given user-friendly UserLog context to full UserLog message
		/// </summary>
		/// <param name="level"> </param>
		/// <param name="message"> </param>
		/// <param name="context"> </param>
		/// <returns> </returns>
		/// <exception cref="NotImplementedException"></exception>
		public LogMessage CreateMessage(LogLevel level, string message, object context) {
			var logMessage = context as LogMessage;
			if (logMessage != null) {
				var lm = logMessage;
				lm.Message += message;
				if (lm.Level < level) {
					lm.Level = level;
				}
				return lm;
			}
			var result = new LogMessage {Level = level, Message = message};
			if (context is Exception) {
				result.Error = (Exception) context;
			}
			else if (context is LexInfo) {
				result.LexInfo = ((LexInfo) context);
			}
			else if (context is IMvcContext) {
				result.MvcContext = ((IMvcContext) context);
				if (null == result.Error) {
					result.Error = ((IMvcContext) context).Error;
				}
			}
			return result;
		}
	}
}