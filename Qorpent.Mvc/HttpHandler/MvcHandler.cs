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
// PROJECT ORIGIN: Qorpent.Mvc/MvcHandler.cs
#endregion
using System;
using System.Diagnostics;
using System.Web;
using Qorpent.IoC;
using Qorpent.Mvc.Security;

namespace Qorpent.Mvc.HttpHandler {
	/// <summary>
	/// 	Main handler for QorpentMvc
	/// </summary>
	[ContainerComponent(Lifestyle.Transient)]
	public class MvcHandler : ServiceBase, IMvcHandler,IHttpHandler {

        /// <summary>
        ///     Default constructor
        /// </summary>
		public MvcHandler() {
            #if PARANOID
			if(!Qorpent.Security.Watchdog.Paranoid.Provider.OK) throw new  Qorpent.Security.Watchdog.ParanoidException(Qorpent.Security.Watchdog.ParanoidState.GeneralError);
            #endif

            HandlerState = new MvcHandlerState();
        }

        /// <summary>
        ///     Default destructor
        /// </summary>
        ~MvcHandler() {
            HandlerState.Checkout();
        }

		/// <summary>
		/// 	Authorizer service
		/// </summary>
		[Inject] public IActionAuthorizer Authorizer {
			get { return _authorizer ?? (ResolveService<IActionAuthorizer>()); }
			set { _authorizer = value; }
		}

        /// <summary>
        ///     Work state of this MvcHandler
        /// </summary>
	    public MvcHandlerState HandlerState {
	        get;
            private set;
	    }

		/// <summary>
		/// 	Extensions to be executed as hooks of handler
		/// </summary>
		[Inject] public IMvcHandlerExtension[] Extensions { get; set; }


		/// <summary>
		/// 	Возвращает значение, позволяющее определить, может ли другой запрос использовать экземпляр класса <see
		/// 	 cref="T:System.Web.IHttpHandler" />.
		/// </summary>
		/// <returns> Значение true, если экземпляр <see cref="T:System.Web.IHttpHandler" /> доступен для повторного использования; в противном случае — значение false. </returns>
		public bool IsReusable {
			get { return true; }
		}

		/// <summary>
		/// 	Разрешает обработку веб-запросов НТТР для пользовательского элемента HttpHandler, который реализует интерфейс <see
		/// 	 cref="T:System.Web.IHttpHandler" />.
		/// </summary>
		/// <param name="context"> Объект <see cref="T:System.Web.HttpContext" /> , предоставляющий ссылки на внутренние серверные объекты (например, Request, Response, Session и Server), используемые для обслуживания HTTP-запросов. </param>
		public void ProcessRequest(HttpContext context) {
			_isDefaultHandler = true;

			if (null != context.Request.UrlReferrer) {
				if (context.Request.Url.Host != context.Request.UrlReferrer.Host) {
					var origindomainparts = context.Request.UrlReferrer.Host.Split('.');
					var currentdomainparts = context.Request.Url.Host.Split('.');
					if (origindomainparts.Length == 3 && currentdomainparts.Length == 3) {
						if (origindomainparts[1] == currentdomainparts[1] && origindomainparts[2] == currentdomainparts[2]) {
							context.Response.Headers.Add("Access-Control-Allow-Origin","https://"+context.Request.UrlReferrer.Host);
						}
					}
				}
			}
			var ctx = ResolveService<IMvcContext>();
			ctx.SetNativeContext(new System.Web.HttpContextWrapper(context));
			ProcessRequest(ctx);
		}

		/// <summary>
		/// 	Executes given mvc context
		/// </summary>
		/// <param name="context"> </param>
		/// <returns> </returns>
		public IMvcContext ProcessRequest(IMvcContext context) {
            HandlerState.RequestsCountIncrease();

			Application.CurrentMvcContext = context;
			Application.Principal.SetCurrentUser(
				Application.Impersonation.GetImpersonation(context.LogonUser)
				);
			if (_isDefaultHandler) {
				MvcContextBase.Current = context;
			}
			context.NotModified = false;
			
			OnStart(context);
			try {
				BeforeAuthorize(context);
				context.AuthrizeResult = Authorizer.Authorize(context);
				if (context.IsRedirected) {
					return context;
				}
				AfterAuthorize(context);

				if (context.AuthrizeResult.Authorized) {
					BindContext(context);
					if (!context.IgnoreActionResult) {
						EvaluateActionResult(context);
					}
					else {
						ExecuteWithoutResult(context);
					}

					if (context.NotModified) {
						ProcessNotModified(context);
					}
					else {
						RenderResult(context);
						SetModifiedHeader(context);
					}
				}
				else {
					ProcessNotAuthorized(context);
				}
			}
			catch (Exception ex) {
                HandlerState.FailedRequestsCountIncrease();
				ProcessError(context, ex);
			}
			finally {
				context.Release();
			}
			OnFinish(context);
			return context;
		}

		private void BindContext(IMvcContext context) {
			context.Bind();
		}

		/// <summary>
		/// return native http handler with given interface
		/// </summary>
		/// <typeparam name="TIhandler"></typeparam>
		/// <returns></returns>
		public TIhandler AsNative<TIhandler>() {
			if(typeof(IHttpHandler)==typeof(TIhandler)) {
				return (TIhandler) (object) this;
			}
			return default(TIhandler);
		}


		private void ProcessError(IMvcContext context, Exception ex) {
			context.StatusCode = 403;
			context.Error = ex;
			context.RenderError(ex);
			OnError(context, ex);
		}

		private static void ProcessNotAuthorized(IMvcContext context) {
			context.StatusCode = 403;
			context.Error = context.AuthrizeResult.AuthorizeError;
			context.RenderError(context.AuthrizeResult.AuthorizeError);
		}

		private void RenderResult(IMvcContext context) {
			BeforeRender(context);
			var s = Stopwatch.StartNew();
			context.Render();
			s.Stop();
			AfterRender(context);
			context.Output.Flush();
			context.Output.Close();
		}

		private void ExecuteWithoutResult(IMvcContext context) {
			CheckNotModified(context);
			BeforeAuthorize(context);
			context.AuthrizeResult = Authorizer.Authorize(context);
			AfterAuthorize(context);
		}

		private void EvaluateActionResult(IMvcContext context) {
			var s = Stopwatch.StartNew();
			try {

				CheckNotModified(context);
				if (!context.NotModified) {
					BeforeAction(context);
					context.Process();
					AfterAction(context);
				}
			}
			finally {
				s.Stop();
			}
		}

		private void CheckNotModified(IMvcContext context) {

			context.NotModified = CanReturnNotModifiedStatus(context);
		}


		private static void SetModifiedHeader(IMvcContext context) {
			INotModifiedStateProvider cp = null;
			if (null != context.RenderDescriptor && context.RenderDescriptor.SupportNotModifiedState) {
				cp = context.RenderDescriptor;
			}
			else if (null == context.ActionDescriptor) {
				return;
			}
			cp = cp ?? context.ActionDescriptor;
			if (!cp.SupportNotModifiedState) {
				return;
			}
			context.LastModified = cp.LastModified;
			context.Etag = cp.ETag;
		}

		private static void ProcessNotModified(IMvcContext context) {
			context.StatusCode = 304;
		}

		private static bool CanReturnNotModifiedStatus(IMvcContext context) {
			if (context.IfModifiedSince.Year <= 1900) {
				return false;
			}
			if (null != context.RenderDescriptor && context.RenderDescriptor.SupportNotModifiedState) {
				var rcp = (INotModifiedStateProvider) context.RenderDescriptor;
				return CheckbyLastModAndEtag(context, rcp.LastModified, rcp.ETag);
			}
			if (null == context.ActionDescriptor) {
				return false;
			}
			var cp = (INotModifiedStateProvider) context.ActionDescriptor;
			if (!cp.SupportNotModifiedState) {
				return false;
			}
			return CheckbyLastModAndEtag(context, cp.LastModified, cp.ETag);
		}

		private static bool CheckbyLastModAndEtag(IMvcContext context, DateTime lastmod, string etag) {
			if (context.IfModifiedSince < lastmod) {
				return false;
			}
			if (etag != context.IfNoneMatch) {
				return false;
			}
			return true;
		}

		private void ExecuteExtensions(IMvcContext context, MvcHandlerPhase phase) {
			if (null == Extensions) {
				return;
			}
			foreach (var extension in Extensions) {
				extension.Run(context, phase);
			}
		}

// ReSharper disable UnusedParameter.Global
		private void OnError(IMvcContext context, Exception exception) {
			if (null == context.Error) {
				context.Error = exception;
			}
			Log.Error("error occured", context);
			try {
				ExecuteExtensions(context, MvcHandlerPhase.OnError);
			}
			catch (Exception ex) {
				Log.Error("error on OnError extensions", ex);
				throw;
			}
		}

		private void AfterRender(IMvcContext context) {
			Log.Trace("after render", context);
			ExecuteExtensions(context, MvcHandlerPhase.AfterRender);
		}

		private void BeforeRender(IMvcContext context) {
			Log.Debug("before render", context);
			ExecuteExtensions(context, MvcHandlerPhase.BeforeRender);
		}

		private void AfterAction(IMvcContext context) {
			Log.Trace("after action", context);
			ExecuteExtensions(context, MvcHandlerPhase.AfterAction);
		}

		/// <summary>
		/// </summary>
		/// <param name="context"> </param>
		private void BeforeAction(IMvcContext context) {
			Log.Debug("before action", context);
			ExecuteExtensions(context, MvcHandlerPhase.BeforeAction);
		}

		/// <summary>
		/// </summary>
		/// <param name="context"> </param>
		private void OnFinish(IMvcContext context) {
			Log.Trace("finish", context);
			ExecuteExtensions(context, MvcHandlerPhase.OnFinish);
		}

		/// <summary>
		/// </summary>
		/// <param name="context"> </param>
		private void OnStart(IMvcContext context) {
			Log.Trace("start", context);
			ExecuteExtensions(context, MvcHandlerPhase.OnStart);
		}

		/// <summary>
		/// </summary>
		/// <param name="context"> </param>
		private void BeforeAuthorize(IMvcContext context) {
			Log.Debug("before autorize", context);
			ExecuteExtensions(context, MvcHandlerPhase.BeforeAuthorize);
		}

		/// <summary>
		/// </summary>
		/// <param name="context"> </param>
		private void AfterAuthorize(IMvcContext context) {
			if (context.AuthrizeResult.Authorized) {
				Log.Trace("authorized", context);
			}
			else {
				Log.Warn("not authorized", context);
			}
			ExecuteExtensions(context, MvcHandlerPhase.AfterAuthorize);
		}

		private IActionAuthorizer _authorizer;


		private bool _isDefaultHandler;
	}
}