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
// PROJECT ORIGIN: Qorpent.Core/LogLevel.cs
#endregion
using System;

namespace Qorpent.Log {
	/// <summary>
	/// 	Describes level of UserLog messages
	/// </summary>
	[Flags]
	public enum LogLevel {
		/// <summary>
		/// 	All levels (lowest, stub)
		/// </summary>
		All = 0,

		/// <summary>
		/// 	Debug information (1 level)
		/// </summary>
		Debug = 1,

		/// <summary>
		/// 	Trace info (2 level)
		/// </summary>
		Trace = 2,

		/// <summary>
		/// 	User information (3 level)
		/// </summary>
		Info = 4,

		/// <summary>
		/// 	Warnings (4 level)
		/// </summary>
		Warn = 8,
		/// <summary>
		/// 
		/// </summary>
		Warning = Warn,
		/// <summary>
		/// 	Errors (5 level)
		/// </summary>
		Error = 16,

		/// <summary>
		/// 	Fatal errors (6 level)
		/// </summary>
		Fatal = 32,

		/// <summary>
		/// 	Non logging level (total switch off)
		/// </summary>
		None = 64,
	}
}