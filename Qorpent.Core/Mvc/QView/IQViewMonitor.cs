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
// PROJECT ORIGIN: Qorpent.Core/IQViewMonitor.cs
#endregion
namespace Qorpent.Mvc.QView {
	/// <summary>
	/// 	Factory-attached QView monitor - it's goal is to  somehow monitor QView
	/// 	actuality and refresh container/factory to match actual
	/// </summary>
	public interface IQViewMonitor {
		/// <summary>
		/// 	Initial QView loading - synchronous
		/// </summary>
		void Startup();

		/// <summary>
		/// 	Asynchronous update QView monitoring and factory cleanup - 
		/// 	if qview updates occured - load them and cleanup factory as needed
		/// </summary>
		void StartMonitoring();

		/// <summary>
		/// 	Pause minitoring process
		/// </summary>
		void EndMonitoring();

		/// <summary>
		/// 	Set factory to be cleaned
		/// </summary>
		/// <param name="factory"> </param>
		void SetFactory(IMvcFactory factory);
	}
}