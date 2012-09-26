#region LICENSE

// Copyright 2007-2012 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Solution: Qorpent
// Original file : ILogger.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

namespace Qorpent.Log {
	/// <summary>
	/// 	low level logger - executes UserLog writings itself
	/// 	internal structure for Qorpent log manager
	/// </summary>
	public interface ILogger {
		/// <summary>
		/// 	Level of logger
		/// </summary>
		LogLevel Level { get; set; }

		/// <summary>
		/// 	Marks logger to be used - false - disable logger
		/// </summary>
		bool Available { get; set; }


		/// <summary>
		/// 	user friendly name of logger
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// 	Behavior on error occured in logger - if None - Manager behavior will be used
		/// </summary>
		InternalLoggerErrorBehavior ErrorBehavior { get; set; }

		/// <summary>
		/// 	check's if this logger is applyable to given context
		/// </summary>
		/// <param name="context"> </param>
		/// <returns> </returns>
		bool IsApplyable(object context);

		/// <summary>
		/// 	Starts write UserLog message to it's target persistence  - asserted to be async
		/// </summary>
		/// <param name="message"> </param>
		void StartWrite(LogMessage message);

		/// <summary>
		/// 	Synchronizes calling context to logger
		/// </summary>
		void Join();
	}
}