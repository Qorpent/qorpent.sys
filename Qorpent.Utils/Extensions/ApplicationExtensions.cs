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
// Original file : ApplicationExtensions.cs
// Project: Qorpent.Utils
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using Qorpent.Applications;
using Qorpent.Events;

namespace Qorpent.Utils.Extensions {
	/// <summary>
	/// 	shortcuts for using with application contexts
	/// </summary>
	public static class ApplicationExtensions {
		/// <summary>
		/// 	call reset using set of 'reset codes' known by accepted classes
		/// </summary>
		/// <param name="application"> </param>
		/// <param name="options"> </param>
		/// <returns> </returns>
		public static ResetEventResult Reset(this IApplication application, params string[] options) {
			lock (application) {
				if (null != application.Events) {
					return application.Events.Call<ResetEventResult>(new ResetEventData(options), null);
				}
				return null;
			}
		}

		/// <summary>
		/// 	call all reseters with All flag - total application cleanup
		/// </summary>
		/// <returns> </returns>
		public static ResetEventResult ResetAll(this IApplication application) {
			lock (application) {
				//return application.Events.Prepare<ResetEvent>()
				//    .Invoke(x =>
				//                {
				//                    x.Data.All = true;
				//                }).Result;
				return null;
			}
		}
	}
}