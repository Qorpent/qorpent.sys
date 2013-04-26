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
// PROJECT ORIGIN: Qorpent.Mvc/MvcHandlerFactory.cs
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Web;
using Qorpent.Applications;

namespace Qorpent.Mvc.HttpHandler {
	/// <summary>
	/// 	Main factory, that manages handlers and perform applicaition startup
	/// </summary>
	public class MvcHandlerFactory : IHttpHandlerFactory {
		private static readonly object Sync = new object();
		private static IApplication Application { get; set; }
        private readonly Stack<IMvcHandler> _handlers = new Stack<IMvcHandler>();

#if PARANOID
		static MvcHandlerFactory() {
			if(!Qorpent.Security.Watchdog.Paranoid.Provider.OK) throw new  Qorpent.Security.Watchdog.ParanoidException(Qorpent.Security.Watchdog.ParanoidState.GeneralError);
		}
#endif

		/// <summary>
		/// 	���������� ��������� ������, ������� ��������� ��������� <see cref="T:System.Web.IHttpHandler" />.
		/// </summary>
		/// <returns> ����� ������ <see cref="T:System.Web.IHttpHandler" /> , �������������� ������. </returns>
		/// <param name="context"> ��������� ������ <see cref="T:System.Web.HttpContext" /> , ������� ������������� ������ �� ���������� ������� ������� (��������, Request, Response, Session � Server), ������������ ��� ������������ HTTP-��������. </param>
		/// <param name="requestType"> ����� HTTP �������� ������ (GET ��� POST), ������� ������������ ��������. </param>
		/// <param name="url"> �������� <see cref="P:System.Web.HttpRequest.RawUrl" /> ������������ �������. </param>
		/// <param name="pathTranslated"> �������� <see cref="P:System.Web.HttpRequest.PhysicalApplicationPath" /> , �������� ���� � ������������ �������. </param>
		public IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated) {
            ServiceState.TotalQueriesHandledIncrease();
			lock (this) {
				//Debugger.Break();
				var httpHandler = TryGetFromPool();
				if (httpHandler != null) {
					return httpHandler;
				}

				CheckInitializeApplication(context);
                
				IHttpHandler waitApplicationStartupHandler;
				if (ApplicationNotStarted(out waitApplicationStartupHandler)) {
					return waitApplicationStartupHandler;
				}

			    var handler = InitMVCHandlerInstance();

				return handler.AsNative<IHttpHandler>();
			}
		}

	    /// <summary>
		/// 	��������� ������� ��������� ������������� ������������� ���������� �����������.
		/// </summary>
		/// <param name="handler"> ������ <see cref="T:System.Web.IHttpHandler" /> ��� ���������� �������������. </param>
		public void ReleaseHandler(IHttpHandler handler) {
			lock (this) {
				var mvcHandler = handler as IMvcHandler;
				if (mvcHandler != null) {
					_handlers.Push(mvcHandler);
				}
			}
		}


		private static bool ApplicationNotStarted(out IHttpHandler resultHandler) {
			resultHandler = null;
			if (!Application.StartupProcessed) {
				if (null == Application.StartupError) {
					if (!Application.IsInStartup) {
						Application.PerformAsynchronousStartup();
						Thread.Sleep(50); //hacked style to synchronize with application
						if (Application.IsInStartup) {
							Thread.Sleep(1000); //hacked style to synchronize with application	
						}
					}
				}

				if (Application.IsInStartup) {
					{
						resultHandler = new WaitApplicationStartupHandler {Application = Application};
						return true;
					}
				}

				if (Application.StartupError != null) {
					{
						resultHandler = new ErrorApplicationStartupHandler {Application = Application};
						return true;
					}
				}
			}
			return false;
		}

		private IHttpHandler TryGetFromPool() {
			if (_handlers.Count != 0) {
				return _handlers.Pop().AsNative<IHttpHandler>();
			}
			return null;
		}

		private static void CheckInitializeApplication(HttpContext context) {
			lock (Sync) {
				if (null == Application) {
					Application = Applications.Application.Current;
					if (context.Request.ApplicationPath != null) {
						Application.ApplicationName = context.Request.ApplicationPath.Replace("/", "");
					}
				}
			}
		}

                /// <summary>
        ///     Initialize a MVCHandler instance
        /// </summary>
        /// <returns></returns>
        private IMvcHandler InitMVCHandlerInstance() {
            return Application.Container.Get<IMvcHandler>() ?? CreateMVCHandlerManually();
        }

	    /// <summary>
        ///     Returns a MVCHandler instance
        /// </summary>
        /// <returns></returns>
	    private MvcHandler CreateMVCHandlerManually() {
            var handler = new MvcHandler();
            handler.SetApplication(Application);

	        return handler;
	    }
	}
}