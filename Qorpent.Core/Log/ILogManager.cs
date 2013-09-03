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
// PROJECT ORIGIN: Qorpent.Core/ILogManager.cs
#endregion
namespace Qorpent.Log {
	/// <summary>
	/// 	Application LOG gate
	/// </summary>
	public interface ILogManager {
		/// <summary>
		/// 	Default error behavior
		/// </summary>
		InternalLoggerErrorBehavior ErrorBehavior { get; set; }

		/// <summary>
		/// 	returns UserLog listener, applyable for given context
		/// </summary>
		/// <param name="contextDescriptor"> описатель, который позволяет отобрать по маске логгеры, используемые в данном журнале </param>
		/// <param name="hostObject"> хост-объект по умолчанию для <see cref="IUserLog.HostObject" /> </param>
		/// <returns> </returns>
		IUserLog GetLog(string contextDescriptor, object hostObject);

		/// <summary>
		/// 	Fail safe UserLog gate to write internal exception without main logging context, but with max guarantee of regestering
		/// 	synchronous
		/// </summary>
		void LogFailSafe(LogMessage message);

		/// <summary>
		/// 	Synchronizes all internal loggers - calling code assumed here that all files/streams are now free
		/// 	use it for direct access for logging files and so on
		/// </summary>
		void Join();
	}
}