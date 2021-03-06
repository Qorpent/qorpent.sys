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
// PROJECT ORIGIN: Qorpent.Core/IMvcFactory.cs
#endregion

using System;
using System.Reflection;
using Qorpent.Mvc.Binding;

namespace Qorpent.Mvc {
	/// <summary>
	/// 	Abstract factory of actions, renders and binders - core parts of MVC
	/// </summary>
	public interface IMvcFactory {
		/// <summary>
		/// 	Creates action descriptor for context
		/// </summary>
		/// <param name="context"> </param>
		/// <returns> </returns>
		ActionDescriptor GetAction(IMvcContext context);

		/// <summary>
		/// 	Releases action after context used
		/// </summary>
		/// <param name="context"> </param>
		void ReleaseAction(IMvcContext context);

		/// <summary>
		/// 	Creates render for context
		/// </summary>
		/// <param name="context"> </param>
		/// <returns> </returns>
		RenderDescriptor GetRender(IMvcContext context);

		/// <summary>
		/// 	Release render  after usage
		/// </summary>
		/// <param name="context"> </param>
		void ReleaseRender(IMvcContext context);

		/// <summary>
		/// 	Return action binder (ioc based service)
		/// </summary>
		/// <returns> </returns>
		IActionBinder GetBinder();


		/// <summary>
		/// 	clears all resolution caches
		/// </summary>
		void ClearCaches(MvcObjectType type = MvcObjectType.All, string name = null);

		/// <summary>
		/// 	Wrapper for container setup for Actions, Renders, Views
		/// </summary>
		/// <param name="type"> </param>
		/// <returns> </returns>
		IMvcFactory Register(Type type);

		/// <summary>
		/// 	Registers all mvcfactory-managed items from given assembly
		/// </summary>
		/// <param name="assembly"> </param>
		/// <returns> </returns>
		IMvcFactory Register(Assembly assembly);

		/// <summary>
		/// 	Создает новый экземпляр обработчика
		/// </summary>
		/// <returns> </returns>
		IMvcHandler CreateHandler();

		/// <summary>
		/// 	Создает контекст относительно URL
		/// </summary>
		/// <param name="url"> </param>
		/// <returns> </returns>
		IMvcContext CreateContext(string url = null);

	}
}