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
// PROJECT ORIGIN: Qorpent.Core/ActionBase.cs
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
	/// Базовый класс для Действий - обертывает большинство подсистем и контекстов для упрощения доступа из пользовательского кода + шаблон "жизненного цикла" для имплементирования 
	/// </summary>
	public abstract class ActionBase : ServiceBase, IContextualAction, IWithRole, INotModifiedStateProvider {
#if PARANOID
		static ActionBase() {
			if(!Qorpent.Security.Watchdog.Paranoid.Provider.OK) throw new  Qorpent.Security.Watchdog.ParanoidException(Qorpent.Security.Watchdog.ParanoidState.GeneralError);
		}
#endif


		/// <summary>
		/// 	Обратная ссылка на дескриптор
		/// </summary>
		public ActionDescriptor Descriptor { get; set; }

		/// <summary>
		/// 	Обертка над именем дескриптора
		/// </summary>
		public string Name {
			get { return Descriptor.Name; }
		}

		/// <summary>
		/// 	Обертка над справкой дескриптора
		/// </summary>
		public string Help {
			get { return Descriptor.Help; }
		}

		/// <summary>
		/// 	Доступ к файловой системе
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
		/// 	Доступ к системе ролей
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
		/// 	Доступ к системе событий
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
		/// 	Доступ к текущему пользователю
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
		/// Обертка над фабрикой дескриптора
		/// </summary>
		protected IMvcFactory Factory {
			get { return Descriptor.Factory; }
		}


		///<summary>
		/// Выполнение действия в заданном контексте и возвращение результата
		///</summary>
		///<param name="context">Контекст Действия</param>
		/// <remarks>
        /// 0. Вызывается SetContext - устанавливается текущий контекст
        /// 1. Вызывается Initialize - происходит работа с внешним контекстом
        /// 2. Вызывается Validate - фаза на которой происходит проверка параметров запроса
        /// 3. Вызывается Prepare - фаза подготовки
        /// 4. Вызывается Authorize - происходит дополнительная (кастомная) проверка безопасности
        /// 5. Вызывается MainProcess - собственная логика Action
        /// 
        ///	Executes itself in given context and return some action result
        ///	0. SetContext called
        ///	1. Initialize called - setup context-bound features here - start action state must be completed here
        ///	2. Validate called - this phase have to validate request by internal logic (parameters complexity checking)
        ///	3. Prepare called - Prepare is second-level preparation - some db and sys properties can be prepared here
        ///	4. Authorize called - here U can authorize action on very specific logic kind
		/// </remarks>
		///<returns>Результат выполнения Действия</returns>
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
		/// Устанавливается текущий контекст 
		/// </summary>
		/// <param name="context">контекст</param>
		public virtual void SetContext(IMvcContext context) {
			Context = context;
			_lastmodified = new DateTime();
			_etag = null;
			_supportnmd = null;
		}

		/// <summary>
		/// Устанавливает текущий дескриптор
		/// </summary>
		/// <param name="descriptor"> </param>
		public void SetDescriptor(ActionDescriptor descriptor) {
			Descriptor = descriptor;
		}


		/// <summary>
        /// Формирует заголовок ответа LastModified
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
		/// Обертка над функцией получения Eтага
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
		/// Обертка над функцией отпределяющей поддержку 304 статуса
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
		/// Обертка над функцией возращающей роль текущего пользователя
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
			IAccessProvider provider;
			IPrincipal principal;
			if (null != Context) {
				provider = Context.Application.Access;
				principal = Context.User;
			}
			else {
				provider = Application.Access;
				principal = Application.Principal.CurrentUser;
			}
			return provider.IsAccessible(obj, role, principal, Roles);
		}

		/// <summary>
		/// 	В качестве суффикса возвращается имя действия
		/// </summary>
		/// <returns> </returns>
		protected override string GetLoggerNameSuffix() {
			return ActionAttribute.GetName(this);
		}


		/// <summary>
		/// Проверяет наличие указанной роли у текущего пользователя
		/// </summary>
		/// <param name="role">Роль</param>
		/// <param name="exact">Точное указание роли</param>
		/// <param name="customContext">контекст</param>
		/// <returns>true или false в зависимости от наличие роли у пользователя</returns>
		public bool IsInRole(string role, bool exact = false, object customContext = null) {
			return Roles.IsInRole(User, role, exact, Context, customContext);
		}


		/// <summary>
		/// 	Обертка над вызовом событий приложення
		/// </summary>
		/// <param name="eventData">время события</param>
		/// <param name="user">пользователь</param>
		/// <param name="syslock">блокировка</param>
		/// <typeparam name="TResult">Тип возвращаемого содержимого</typeparam>
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
		/// Определяет имя файла в текущей файловой системе, текущем контексте
		/// </summary>
		/// <param name="name">Имя файла</param>
		/// <param name="existed"> </param>
		/// <param name="resolvetype">тип файла</param>
		/// <returns> </returns>
		public string ResolveFileName(string name, bool existed = true,
		                              FileSearchResultType resolvetype = FileSearchResultType.FullPath) {
			return FileNameResolver.Resolve(FileSearchQuery.Leveled(name, existed, resolvetype));
		}

		/// <summary>
		/// Доступ к функции возвращающей значение параметра заданного в текущем контексте
		/// </summary>
		/// <typeparam name="T">тип параметра</typeparam>
		/// <param name="name">имя</param>
		/// <param name="def">значение по-умолчанию</param>
		/// <returns> </returns>
		public T Get<T>(string name, T def = default(T)) {
			return Context.Get(name, def);
		}

		/// <summary>
        /// Доступ к функции возвращающей текстовое значение параметра заданного в текущем контексте
		/// </summary>
		/// <param name="name">Имя параметра</param>
		/// <param name="def">значение по умолчанию</param>
		/// <returns> </returns>
		public string Get(string name, string def = "") {
			return Context.Get(name, def);
		}

		/// <summary>
        /// Доступ к функции возвращающей значение параметра, заданного в текущем контексте, в виде XML
		/// </summary>
		/// <param name="name">имя параметра</param>
		/// <returns> </returns>
		public XElement Getx(string name) {
			return Context.GetXml(name);
		}

		/// <summary>
		/// Инициализация - первая фаза запуска Действия. Перекрывается при необходимости дополнительной обработки входных параметров. 
		/// </summary>
		protected virtual void Initialize() {}

		/// <summary>
		/// Вторая фаза - проверка входных параметров, параметров запроса (вызывается до стадии подготовки, так что не
		/// пытайтесь проверить авторизацию или что либо кроме входных параметров)
		/// </summary>
		protected virtual void Validate() {}

		/// <summary>
		/// Третья фаза выполнения Действия - Подготовка, настройка системных параметров (вызывается после стадии проверки и до стадии авторизации)
		/// </summary>
		protected virtual void Prepare() {}

		/// <summary>
        /// Четвертая фаза - контекст сформирован, проверяем возможность выполнения Действия в соответсвии с правами доступа
		/// </summary>
		protected virtual void Authorize() {}


		/// <summary>
		/// Основная фаза - тело действия
		/// </summary>
		/// <returns> </returns>
		protected virtual object MainProcess() {
			return null;
		}

		/// <summary>
		/// Перекрываем, если Yr action возвращает 304 статус и заголовок Last-Modified-Stateer
		/// </summary>
		/// <returns> </returns>
		protected virtual DateTime EvalLastModified() {
			return new DateTime(1901, 1, 1);
		}

		/// <summary>
        /// Перекрываем, если Yr action возвращает 304 статус и заголовок ETag
		/// </summary>
		/// <returns> </returns>
		protected virtual string EvalEtag() {
			return "";
		}

		/// <summary>
        /// Перекрываем, если Yr action возвращает 304 статус и True
		/// </summary>
		/// <returns> </returns>
		protected virtual bool GetSupportNotModified() {
			return false;
		}


		/// <summary>
		/// Выполняем запуск Действия
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

	/// <summary>
	/// Типизированый по результату контроллер 
	/// </summary>
	/// <typeparam name="TResult"></typeparam>
	public abstract class ActionBase<TResult>: ActionBase{
		/// <summary>
		/// Основная фаза - тело действия
		/// </summary>
		/// <returns> </returns>
		protected override object MainProcess(){
			return GetResult();
		}
		/// <summary>
		/// Требует перекрытия в дочерних классах
		/// </summary>
		/// <returns></returns>
		protected abstract TResult GetResult();
	}
}