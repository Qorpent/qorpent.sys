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
// PROJECT ORIGIN: Qorpent.Core/ConsoleLogWriter.cs
#endregion

using System;
using Microsoft.SqlServer.Server;

namespace Qorpent.Log.NewLog {
    /// <summary>
	/// 	ћенеджер записи в консоль, используемый по умолчанию
	/// </summary>
	public class ConsoleAppender : AppenderBase {
        
		/// <summary>
		///     ¬нутренн€€ операци€ записи в лог
		/// </summary>
		/// <param name="message"> </param>
		public override void Write(LoggyMessage message) {
		    
		    
			try {
				switch (message.Level) {
					case LogLevel.Debug:
						Console.ForegroundColor = ConsoleColor.DarkGray;
						break;
					case LogLevel.Trace:
						Console.ForegroundColor = ConsoleColor.Gray;
						break;
					case LogLevel.Info:
						Console.ForegroundColor = ConsoleColor.White;
						break;
					case LogLevel.Warning:
						Console.ForegroundColor = ConsoleColor.Yellow;
						break;
					case LogLevel.Error:
						Console.ForegroundColor = ConsoleColor.Red;
						break;
					case LogLevel.Fatal:
						Console.ForegroundColor = ConsoleColor.Magenta;
						break;
				}
				Console.WriteLine(GetText(message));
			}
			finally {
				Console.ResetColor();
			}
		}

	}
}