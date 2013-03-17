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
// PROJECT ORIGIN: Qorpent.Core/IRender.cs
#endregion
using System;

namespace Qorpent.Mvc {
	/// <summary>
	/// 	Abstract render info
	/// </summary>
	public interface IRender {
		/// <summary>
		/// 	False if it's self-made render which not require call to action to get user response
		/// </summary>
		bool NeedResult { get; }

		/// <summary>
		/// 	Renders given context
		/// </summary>
		/// <param name="context"> </param>
		void Render(IMvcContext context);

		/// <summary>
		/// 	Renders error, occured in given context
		/// </summary>
		/// <param name="error"> </param>
		/// <param name="context"> </param>
		void RenderError(Exception error, IMvcContext context);
	}
}