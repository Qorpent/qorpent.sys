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
// Original file : IMvcHandlerExtension.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using Qorpent.Model;

namespace Qorpent.Mvc {
	/// <summary>
	/// 	Extension point to setup hooks for MvcHttpHandler
	/// </summary>
	public interface IMvcHandlerExtension : IWithIdx {
		/// <summary>
		/// 	Implement to execute some logic in MvcHttpHandler context
		/// </summary>
		/// <param name="context"> </param>
		/// <param name="phase"> </param>
		void Run(IMvcContext context, MvcHandlerPhase phase);
	}
}