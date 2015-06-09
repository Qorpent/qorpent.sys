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
// PROJECT ORIGIN: Qorpent.Core/IApplication.cs
#endregion
using System;
using qorpent.v2.security.authorization;
using Qorpent.Bxl;
using Qorpent.Data;
using Qorpent.Dsl;
using Qorpent.Events;
using Qorpent.IO;
using Qorpent.IO.Resources;
using Qorpent.IoC;
using Qorpent.Log;
using Qorpent.Mvc;
using Qorpent.Security;
using Qorpent.Serialization;

namespace Qorpent.Applications {
	/// <summary>
	/// 	Abstract application definition
	/// </summary>
	public interface IApplication {
		/// <summary>
		/// 	Current QWeb Context (based on thread static)
		/// </summary>
		/// <value> The current MVC context. </value>
		/// <remarks>
		/// </remarks>
		IMvcContext CurrentMvcContext { get; set; }

		/// <summary>
		/// Access to HTTP context wrapper
		/// </summary>
		IHttpContextWrapper HttpWrapper { get; }

		/// <summary>
		/// 	Access to Bxl service
		/// </summary>
		/// <remarks>
		/// </remarks>
		IBxlService Bxl { get; }

		/// <summary>
		/// 	Access to IoC container
		/// </summary>
		/// <value> The container. </value>
		/// <remarks>
		/// </remarks>
		IContainer Container { get; set; }

		/// <summary>
		/// 	Access to file system services
		/// </summary>
		/// <remarks>
		/// </remarks>
		IFileService Files { get; }

		/// <summary>
		/// 	Access to Application logger
		/// </summary>
		/// <remarks>
		/// </remarks>
		ILogManager LogManager { get; }

		/// <summary>
		/// 	Access to Application logger
		/// </summary>
		/// <remarks>
		/// </remarks>
		IEventManager Events { get; }

		/// <summary>
		/// 	Logical root directory for Application Environment.CurrentDir for console and ~/ for web
		/// </summary>
		/// <value> The root directory. </value>
		/// <remarks>
		/// </remarks>
		string RootDirectory { get; set; }

		/// <summary>
		/// 	Codebase directory for Application EntryAssembly.Codebase для console and ~/bin for web
		/// </summary>
		/// <value> The bin directory. </value>
		/// <remarks>
		/// </remarks>
		string BinDirectory { get; set; }


		/// <summary>
		/// 	Web Application root name
		/// </summary>
		/// <value> The name of the application. </value>
		/// <remarks>
		/// </remarks>
		string ApplicationName { get; set; }

		

		/// <summary>
		/// 	Access to principal source
		/// </summary>
		/// <remarks>
		/// </remarks>
		IPrincipalSource Principal { get; }

		/// <summary>
		/// 	Access to role resolver
		/// </summary>
		/// <remarks>
		/// </remarks>
        IRoleResolverService Roles { get; set; }

		/// <summary>
		/// 	Indicates that application is in startup mode
		/// </summary>
		bool IsInStartup { get; }

		/// <summary>
		/// 	Error, occured during startup
		/// </summary>
		Exception StartupError { get; }

		/// <summary>
		/// 	Indicates that current application was startupped
		/// </summary>
		bool StartupProcessed { get; }

		/// <summary>
		/// 	Access to mvc factory
		/// </summary>
		/// <remarks>
		/// </remarks>
		IMvcFactory MvcFactory { get; }

	

		/// <summary>
		/// 	Access to <see cref="IAccessProviderService" /> service
		/// </summary>
		/// <remarks>
		/// </remarks>
		IImpersonationProvider Impersonation { get; set; }

		/// <summary>
		/// 	Время реального старта приложения
		/// </summary>
		DateTime StartTime { get; set; }

		/// <summary>
		/// 	Access to <see cref="IDatabaseConnectionProvider" /> service
		/// </summary>
		/// <remarks>
		/// </remarks>
		IDatabaseConnectionProvider DatabaseConnections { get; set; }

	
		/// <summary>
		/// Служба доступа к ресурсам
		/// </summary>
		IResourceProvider Resources { get; set; }

	    /// <summary>
	    /// 	Access to file system services
	    /// </summary>
	    /// <remarks>
	    /// </remarks>
	    ISerializerFactory Serialization { get; }

	    /// <summary>
		/// 	simple synchronization method, waits wile Application lock released
		/// </summary>
		/// <remarks>
		/// </remarks>
		void Synchronize();


		/// <summary>
		/// 	returns Application wide lock object
		/// </summary>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		object GetApplicationLock();

		/// <summary>
		/// 	Called by web- infrastructure to execute statrtup - MUST BE ASYNC
		/// </summary>
		void PerformAsynchronousStartup();

		/// <summary>
		/// 	Called by console application - must execute some startup logic in SYNCHRONOUS
		/// </summary>
		void PerformSynchronousStartup();

		/// <summary>
		/// 	Creates new MVC context
		/// </summary>
		/// <param name="createContext"> </param>
		/// <returns> </returns>
		IMvcContext CreateContext(object createContext);

		/// <summary>
		/// 
		/// </summary>
		void WaitStartUp();
	}
}