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
// PROJECT ORIGIN: Qorpent.Core/ILogWriter.cs
#endregion
namespace Qorpent.Log {
	/// <summary>
	/// 	Base asynchronous logger with error popup logic - reacts on Join
	/// </summary>
	public interface ILogWriter {
		/// <summary>
		/// 	Minimal log level of writer
		/// </summary>
		LogLevel Level { get; set; }
        /// <summary>
        /// Common marker of writer activity
        /// </summary>
	    bool Active { get; set; }

	    /// <summary>
		/// 	writes message synchronously on down level
		/// </summary>
		/// <param name="message"> </param>
		void Write(LogMessage message);
	}
}