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
// Original file : RenderDescriptor.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using Qorpent.Serialization;

namespace Qorpent.Mvc {
	/// <summary>
	/// 	Describes info about Render
	/// </summary>
	[Serialize]
	public class RenderDescriptor : MvcStepDescriptor {
		/// <summary>
		/// 	Creates new descriptor over given action
		/// </summary>
		/// <param name="render"> </param>
		public RenderDescriptor(IRender render) {
			Render = render;
			Name = RenderAttribute.GetName(render);
			DirectRole = RenderAttribute.GetRole(render);
			var contextualRender = render as IContextualRender;
			if (contextualRender != null) {
				contextualRender.SetDescriptor(this);
			}
		}

		/// <summary>
		/// 	Underlined Render
		/// </summary>
		[IgnoreSerialize] public IRender Render { get; set; }

		/// <summary>
		/// 	Last modified header wrapper
		/// </summary>
		[IgnoreSerialize] public override DateTime LastModified {
// ReSharper disable SuspiciousTypeConversion.Global
			get { return ((INotModifiedStateProvider) Render).LastModified; }
// ReSharper restore SuspiciousTypeConversion.Global
		}

		/// <summary>
		/// 	Etag header wrapper
		/// </summary>
		[IgnoreSerialize] public override string ETag {
// ReSharper disable SuspiciousTypeConversion.Global
			get { return ((INotModifiedStateProvider) Render).ETag; }
// ReSharper restore SuspiciousTypeConversion.Global
		}


		/// <summary>
		/// 	True if object supports 304 state
		/// </summary>
		public override bool SupportNotModifiedState {
			get {
				if (null == Render) {
					return false;
				}
				// ReSharper disable SuspiciousTypeConversion.Global
				// ReSharper disable ExpressionIsAlwaysNull	
				// ReSharper disable ConditionIsAlwaysTrueOrFalse
				var notModifiedStateProvider = Render as INotModifiedStateProvider;

				return notModifiedStateProvider != null && notModifiedStateProvider.SupportNotModifiedState;
			}
			// ReSharper restore ConditionIsAlwaysTrueOrFalse
			// ReSharper restore ExpressionIsAlwaysNull
			// ReSharper restore SuspiciousTypeConversion.Global
		}

		/// <summary>
		/// 	true if render proceed only if ayuthorization passed
		/// </summary>
		public bool UseAuthorization {
			get { return !string.IsNullOrEmpty(Role) && "DEFAULT" != Role; }
			set { throw new NotSupportedException(); }
		}

		/// <summary>
		/// 	overrides action authorization
		/// </summary>
		public bool OverrideActionAuthorization { get; set; }

		/// <summary>
		/// 	can be rendered without calling to action
		/// </summary>
		public bool IgnoreActionResult {
			get { return !Render.NeedResult; }
			set { throw new NotSupportedException(); }
		}

		/// <summary>
		/// 	Для рендеров используется обычная <see cref="MvcStepDescriptor.DirectRole" />
		/// </summary>
		/// <returns> </returns>
		protected override string PrepareRole() {
			return DirectRole;
		}
	}
}