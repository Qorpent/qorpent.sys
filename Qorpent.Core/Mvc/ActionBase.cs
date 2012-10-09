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
// Original file : ActionBase.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Security.Principal;
using System.Xml.Linq;
using Qorpent.Events;
using Qorpent.IO;
using Qorpent.IoC;
using Qorpent.Model;
using Qorpent.Security;
using Qorpent.Serialization;

namespace Qorpent.Mvc {
	/// <summary>
	/// 	Base for custom actions - wraps most of subsystems and context to simplify acess from user code + templated lifecycle to implement
	/// </summary>
	public abstract class ActionBase : ServiceBase, IContextualAction, IWithRole, INotModifiedStateProvider
	{

#if PARANOID
		static ActionBase() {
			if(!Qorpent.Security.Watchdog.Paranoid.Provider.OK) throw new  Qorpent.Security.Watchdog.ParanoidException(Qorpent.Security.Watchdog.ParanoidState.GeneralError);
		}
#endif


		/// <summary>
		/// 	Back reference to Descriptor
		/// </summary>
		public ActionDescriptor Descriptor { get; set; }

		/// <summary>
		/// 	Wrapper fo Descritor.Name
		/// </summary>
		public string Name {
			get { return Descriptor.Name; }
		}

		/// <summary>
		/// 	Wrapper fo Descritor.Help
		/// </summary>
		public string Help {
			get { return Descriptor.Help; }
		}

		/// <summary>
		/// 	Access to file system
		/// </summary>
		[Inject] public IFileNameResolver FileNameResolver {
			get {
				if (null != _fileNameResolver) {
					return _fileNameResolver;
				}
				if (null != Context) {
					return Context.Application.Files;
				}
				return Applications.Application.Current.Files;
			}
			set { _fileNameResolver = value; }
		}

		/// <summary>
		/// 	Access to role system
		/// </summary>
		[Inject] public IRoleResolver Roles {
			get {
				if (null != _roles) {
					return _roles;
				}
				if (null != Context) {
					return Context.Application.Roles;
				}
				return Applications.Application.Current.Roles;
			}
			set { _roles = value; }
		}

		/// <summary>
		/// 	Access to event system
		/// </summary>
		[Inject] public IEventManager Events {
			get {
				if (null != _events) {
					return _events;
				}
				if (null != Context) {
					return Context.Application.Events;
				}
				return Applications.Application.Current.Events;
			}
			set { _events = value; }
		}


		/// <summary>
		/// 	Access to current user
		/// </summary>
		public IPrincipal User {
			get {
				if (null != Context) {
					return Context.User;
				}
				return Applications.Application.Current.Principal.CurrentUser;
			}
		}

		/// <summary>
		/// </summary>
		protected IMvcFactory Factory {
			get { return Descriptor.Factory; }
		}


		///<summary>
		///	Executes itself in given context and return some action result
		///	0. SetContext called
		///	1. Initialize called - setup context-bound features here - start action state must be completed here
		///	2. Validate called - this phase have to validate request by internal logic (parameters complexity checking)
		///	3. Prepare called - Prepare is second-level preparation - some db and sys properties can be prepared here
		///	4. Authorize called - here U can authorize action on very specific logic kind
		///</summary>
		///<param name="context"> </param>
		///<returns> </returns>
		public object Process(IMvcContext context) {
			Log.Trace("start", this);
			try {
				SetContext(context);
				Log.Debug("context seted", this);
				Initialize();
				Log.Debug("initialized", this);
				Validate();
				Log.Debug("validated", this);
				Prepare();
				Log.Debug("prepared", this);
				Authorize();
				Log.Debug("authorized", this);
				var result = MainProcess();
				Log.Debug("finish", this);
				return result;
			}
			catch (Exception ex) {
				Log.Error("error", ex);
				throw;
			}
		}


		/// <summary>
		/// 	Executes before all other calls to Action
		/// </summary>
		/// <param name="context"> </param>
		public virtual void SetContext(IMvcContext context) {
			Context = context;
			_lastmodified = new DateTime();
			_etag = null;
			_supportnmd = null;
		}

		/// <summary>
		/// 	Executes on creation with setting action descriptor
		/// </summary>
		/// <param name="descriptor"> </param>
		public void SetDescriptor(ActionDescriptor descriptor) {
			Descriptor = descriptor;
		}


		/// <summary>
		/// 	Last modified header wrapper
		/// </summary>
		[IgnoreSerialize] public DateTime LastModified {
			get {
				if (_lastmodified == DateTime.MinValue) {
					_lastmodified = new DateTime(1900, 1, 1);
					if (SupportNotModifiedState) {
						_lastmodified = EvalLastModified();
						_lastmodified = new DateTime(_lastmodified.Year, _lastmodified.Month, _lastmodified.Day, _lastmodified.Hour,
						                             _lastmodified.Minute, _lastmodified.Second);
					}
				}
				return _lastmodified;
			}
			set { _lastmodified = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second); }
		}

		/// <summary>
		/// 	Etag header wrapper
		/// </summary>
		[IgnoreSerialize] public string ETag {
			get {
				if (_etag == null) {
					_etag = "";
					if (SupportNotModifiedState) {
						_etag = EvalEtag();
					}
				}
				return _etag;
			}
			set { _etag = value; }
		}

		/// <summary>
		/// 	True if object supports 304 state
		/// </summary>
		public bool SupportNotModifiedState {
			get {
				if (_supportnmd == null) {
					_supportnmd = false;
					_supportnmd = GetSupportNotModified();
				}
				return _supportnmd.Value;
			}
		}


		/// <summary>
		/// 	Wrapper fo Descritor.Role
		/// </summary>
		public string Role {
			get { return Descriptor.Role; }
			set { throw new NotSupportedException(); }
		}


		/// <summary>
		/// 	Быстрый метод для доступа к системе <see cref="IAccessProvider" />
		/// </summary>
		/// <param name="obj"> </param>
		/// <param name="role"> </param>
		/// <returns> </returns>
		public AccessResult IsAccessible(object obj, AccessRole role = AccessRole.Access) {
			IAccessProvider _provider;
			IPrincipal principal;
			if (null != Context) {
				_provider = Context.Application.Access;
				principal = Context.User;
			}
			else {
				_provider = Application.Access;
				principal = Application.Principal.CurrentUser;
			}
			return _provider.IsAccessible(obj, role, principal, Roles);
		}

		/// <summary>
		/// 	В качестве суффикса возвращается имя действия
		/// </summary>
		/// <returns> </returns>
		protected override string GetLoggerNameSuffix() {
			return ActionAttribute.GetName(this);
		}


		/// <summary>
		/// 	Check role against current user
		/// </summary>
		/// <param name="role"> </param>
		/// <param name="exact"> </param>
		/// <param name="customContext"> </param>
		/// <returns> </returns>
		public bool IsInRole(string role, bool exact = false, object customContext = null) {
			return Roles.IsInRole(User, role, exact, Context, customContext);
		}


		/// <summary>
		/// 	Обертка над вызовом событий приложення
		/// </summary>
		/// <param name="eventData"> </param>
		/// <param name="user"> </param>
		/// <param name="syslock"> </param>
		/// <typeparam name="TResult"> </typeparam>
		/// <returns> </returns>
		public TResult Call<TResult>(IEventData eventData = null, IPrincipal user = null, bool syslock = true)
			where TResult : IEventResult, new() {
			var events = ResolveService<IEventManager>();
			if (null == events) {
				return default(TResult);
			}
			return events.Call<TResult>(eventData, user, syslock);
		}

		/// <summary>
		/// 	Resolved file name against current file system of context
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="existed"> </param>
		/// <param name="resolvetype"> </param>
		/// <returns> </returns>
		public string ResolveFileName(string name, bool existed = true,
		                              FileSearchResultType resolvetype = FileSearchResultType.FullPath) {
			return FileNameResolver.Resolve(FileSearchQuery.Leveled(name, existed, resolvetype));
		}

		/// <summary>
		/// 	Quick access to Context.Get[T]
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <param name="name"> </param>
		/// <param name="def"> </param>
		/// <returns> </returns>
		public T Get<T>(string name, T def = default(T)) {
			return Context.Get(name, def);
		}

		/// <summary>
		/// 	Quick access to  Context.Get[string]
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="def"> </param>
		/// <returns> </returns>
		public string Get(string name, string def = "") {
			return Context.Get(name, def);
		}

		/// <summary>
		/// 	Quick acess to Context.GetXml
		/// </summary>
		/// <param name="name"> </param>
		/// <returns> </returns>
		public XElement Getx(string name) {
			return Context.GetXml(name);
		}

		/// <summary>
		/// 	First phase of execution - override if need special input parameter's processing
		/// </summary>
		protected virtual void Initialize() {}

		/// <summary>
		/// 	Second phase - validate INPUT/REQUEST parameters here - it called before PREPARE so do not try validate
		/// 	second-level internal state and authorization - only INPUT PARAMETERS must be validated
		/// </summary>
		protected virtual void Validate() {}

		/// <summary>
		/// 	Third part of execution - setup system-bound internal state here (called after validate, but before authorize)
		/// </summary>
		protected virtual void Prepare() {}

		/// <summary>
		/// 	4 part of execution - all internal context is ready - authorize it with custom logic
		/// </summary>
		protected virtual void Authorize() {}


		/// <summary>
		/// 	processing of execution - main method of action
		/// </summary>
		/// <returns> </returns>
		protected virtual object MainProcess() {
			return null;
		}

		/// <summary>
		/// 	override if Yr action provides 304 state  and return Last-Modified-State header
		/// </summary>
		/// <returns> </returns>
		protected virtual DateTime EvalLastModified() {
			return new DateTime(1900, 1, 1);
		}

		/// <summary>
		/// 	override if Yr action provides 304 state and return ETag header
		/// </summary>
		/// <returns> </returns>
		protected virtual string EvalEtag() {
			return "";
		}

		/// <summary>
		/// 	override if Yr action provides 304 state and return TRUE
		/// </summary>
		/// <returns> </returns>
		protected virtual bool GetSupportNotModified() {
			return false;
		}


		/// <summary>
		/// 	Execute MainProcess on prepared context
		/// </summary>
		/// <returns> </returns>
		public object Process() {
			return Process(Context);
		}

		/// <summary>
		/// </summary>
		protected IMvcContext Context;

		private string _etag;
		private IEventManager _events;

		private IFileNameResolver _fileNameResolver;
		private DateTime _lastmodified;
		private IRoleResolver _roles;
		private bool? _supportnmd;
	}
}