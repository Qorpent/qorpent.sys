﻿#region LICENSE
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
// PROJECT ORIGIN: Qorpent.Core/RenderBase.cs
#endregion
using System;
using Qorpent.Model;

namespace Qorpent.Mvc {
	/// <summary>
	/// 	Base render class
	/// </summary>
	public abstract class RenderBase : ServiceBase, IWithRole, IContextualRender {
		/// <summary>
		/// </summary>
		protected RenderDescriptor Descriptor { get; set; }


		/// <summary>
		/// 	False if it's self-made render which not require call to action to get user response
		/// </summary>
		public virtual bool NeedResult {
			get { return true; }
		}


		/// <summary>
		/// 	Renders given context
		/// </summary>
		/// <param name="context"> </param>
		public virtual void Render(IMvcContext context) {
			SetContext(context);
		}

		/// <summary>
		/// 	Renders error, occured in given context
		/// </summary>
		/// <param name="error"> </param>
		/// <param name="context"> </param>
		public abstract void RenderError(Exception error, IMvcContext context);

		/// <summary>
		/// 	Executes before all other calls to Action
		/// </summary>
		/// <param name="context"> </param>
		public virtual void SetContext(IMvcContext context) {}

		/// <summary>
		/// 	Executes on creation with setting action descriptor
		/// </summary>
		/// <param name="descriptor"> </param>
		public void SetDescriptor(RenderDescriptor descriptor) {
			Descriptor = descriptor;
		}


		/// <summary>
		/// 	Роль (список ролей), которые необходимы для доступа (на данный момент для рендеров используется через дескриптор)
		/// </summary>
		public string Role { get; set; }
	}
}