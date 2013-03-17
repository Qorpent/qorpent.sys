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
// PROJECT ORIGIN: Qorpent.Core/InternalLoggerErrorBehavior.cs
#endregion
using System;

namespace Qorpent.Log {
	/// <summary>
	/// 	Behavior of logger on inner exceptions occured
	/// </summary>
	[Flags]
	public enum InternalLoggerErrorBehavior {
		/// <summary>
		/// 	No behavior - use manager's on logger, use Ignore on manager
		/// </summary>
		None = 0,

		/// <summary>
		/// 	All errors occured in loggers will be ignored
		/// </summary>
		Ignore = 1,

		/// <summary>
		/// 	All errors will be logged into special fail-safe UserLog of logmanager
		/// </summary>
		Log = 2,

		/// <summary>
		/// 	Errors are thrown
		/// </summary>
		Throw = 4,

		/// <summary>
		/// 	Logger will be disabled if error occured
		/// </summary>
		AutoDisable = 8,
	}
}