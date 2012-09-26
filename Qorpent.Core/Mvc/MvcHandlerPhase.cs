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
// Original file : MvcHandlerPhase.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

namespace Qorpent.Mvc {
	/// <summary>
	/// 	Phase of MvcHandler execution
	/// </summary>
	public enum MvcHandlerPhase {
		/// <summary>
		/// 	call initially after context created
		/// </summary>
		OnStart,

		/// <summary>
		/// 	called before context is authorized
		/// </summary>
		BeforeAuthorize,

		/// <summary>
		/// 	called after context is authorized
		/// </summary>
		AfterAuthorize,

		/// <summary>
		/// 	called before action is run
		/// </summary>
		BeforeAction,

		/// <summary>
		/// 	called after action is run
		/// </summary>
		AfterAction,

		/// <summary>
		/// 	called before render is run
		/// </summary>
		BeforeRender,

		/// <summary>
		/// 	called after render is run
		/// </summary>
		AfterRender,

		/// <summary>
		/// 	called if an error occured
		/// </summary>
		OnError,

		/// <summary>
		/// 	called when request is finished
		/// </summary>
		OnFinish,
	}
}