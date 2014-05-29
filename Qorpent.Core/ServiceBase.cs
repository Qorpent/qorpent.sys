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
// PROJECT ORIGIN: Qorpent.Core/ServiceBase.cs
#endregion
using System;
using System.Linq;
using Qorpent.Applications;
using Qorpent.Events;
using Qorpent.IoC;
using Qorpent.Log;

namespace Qorpent {
	///<summary>
	///	Базовый класс сервисов - обеспечивает связь с контейнером и приложением.
	///</summary>
	///<remarks>
	///	Рекомендуемый базовый класс для всех основных сервисов и расширений,
	///	может безопасно использоваться и для любых других пользовательских классов.
	///	Главная задача данного абстрактного класса  - обеспечение связи экземпляра с 
	///	контекстом его создания - с контейнером и приложением (если наличествует). Для
	///	этого в ServiceBase реализованы базовые интерфейсы <see cref="IContainerBound" />,<see cref="IApplicationBound" />.
	///	Любому экземпляру ServiceBase дотупны не-нулл <see cref="Container" /> и <see cref="Log" /> для
	///	взаимодействаия с средой вызова и с журналом. 
	///	Для определения других сервисов следует пользоваться методом <see cref="ResolveService{T}" />, который
	///	будет использовать для своей работы максимально релевантный контейнер.
	///	Можете перекрыть <see cref="SetContainerContext" /> и <see cref="SetApplication" /> для
	///	выполнения дополнительных действий на момент привязки к генерирующему контексту.
	///</remarks>
	///<source>Qorpent/Qorpent.Core/ServiceBase.cs</source>
	public abstract class ServiceBase : MarshalByRefObject, IContainerBound, IApplicationBound, IDisposable, IResetable,
	                                    ILogBound {
		/// <summary>
		/// 	Обратная ссылка на приложение, в составе которого создан данный экземпляр (может быть NULL, если 
		/// 	объект был создан из контейнера без связи с приложением)
		/// </summary>
		protected IApplication Application { get; set; }
#if PARANOID
		static ServiceBase() {
			if(!Qorpent.Security.Watchdog.Paranoid.Provider.OK) throw new  Qorpent.Security.Watchdog.ParanoidException(Qorpent.Security.Watchdog.ParanoidState.GeneralError);
		}
#endif
		/// <summary>
		/// Признак принудительного игнорирования текущего приложения
		/// </summary>
		public bool IgnoreDefaultApplication{ get; set; }

		/// <summary>
		/// 	Максимально релевантный для экземпляра контейнер, не NULL
		/// </summary>
		/// <remarks>
		/// 	Релевантность контейнера следует в порядке: <see cref="SourceContainer" />,<see cref="Application" />.Container,<see
		/// 	cref="Applications.Application.Current" />.Container
		/// </remarks>
		public IContainer Container {
			get {
				if (null != SourceContainer) {
					return SourceContainer;
				}
				if (null != Application) {
					return Application.Container;
				}
				if (!IgnoreDefaultApplication){
					return Applications.Application.Current.Container;
				}
				else{
					return null;
				}
			}
		}


		/// <summary>
		/// 	Вызывается в момент создания в случае, если контейнер связан с приложением
		/// </summary>
		/// <param name="app"> Приложение, в контексте которого создается объект </param>
		/// <remarks>
		/// 	Может быть перекрыт для выполнения дополнительных действий, по умолчанию устанавливает свойство <see cref="Application" />
		/// </remarks>
		public virtual void SetApplication(IApplication app) {
			Application = app;
			SetupResetReaction();
		}


		/// <summary>
		/// 	Вызывается в момент создания экземпляра после применения стандартных инъекций.
		/// </summary>
		/// <param name="container"> Контейнер, создающий объект </param>
		/// <param name="component"> Компонент, описывающий объект </param>
		/// <remarks>
		/// 	Стандартная реализация контейнера <see href="Qorpent.IoC~Qorpent.IoC.Container.html" /> автоматически
		/// 	выполняет инъекции зависимоостей на основе параметров <see cref="IComponentDefinition.Parameters" /> и на освное <see
		/// 	 cref="InjectAttribute" />,
		/// 	данный метод в базовой реализации устанавливает свойства  <see cref="SourceContainer" /> и <see cref="Component" />,
		/// 	при перекрытии вы можете расширить логику собственным кодом связывания экземпляра с создавшим его контейнером и компонентом
		/// </remarks>
		public virtual void SetContainerContext(IContainer container, IComponentDefinition component) {
			SourceContainer = container;
			Component = component;
		}

		/// <summary>
		/// 	Выполняется если объект возвращается в контейнер методом <see cref="ITypeResolver.Release" />.
		/// </summary>
		/// <remarks>
		/// 	Метод <see cref="ITypeResolver.Release" /> по сути сделан с резервом на использованние
		/// 	совместно <see cref="Lifestyle.Pooled" />,<see cref="Lifestyle.PerThread" /> и практически не
		/// 	используется на практике. Также <see cref="ITypeResolver.Release" /> вызывается при удалении
		/// 	имеющихся компонентов. То есть по сути данный метод следует перекрывать и реализовывать
		/// 	только в случае острой необходимости, так как гарантий его вызова в момент уничтожения
		/// 	объекта нет и соответственно перекрывать надо только в случае реальных
		/// 	задач относительно контейнера
		/// </remarks>
		public virtual void OnContainerRelease() {}

		/// <summary>
		/// Событие, вызываемое после выполнения инициализации при помощи контейнера
		/// </summary>
		public virtual void OnContainerCreateInstanceFinished() {
			
		}


		/// <summary>
		/// 	Реализует интерфейс <see cref="IDisposable" />, по умолчанию вызывает <see cref="ITypeResolver.Release" />, относительно
		/// 	<see cref="SourceContainer" />
		/// </summary>
		public void Dispose() {
			Dispose(true);
		}

		/// <summary>
		/// 	CA1063 recomendation match
		/// </summary>
		/// <param name="full"> </param>
		protected virtual void Dispose(bool full) {
			if (null != SourceContainer) {
				SourceContainer.Release(this);
			}
		}


		/// <summary>
		/// 	Журнал для данного объекта, выставляется через максимально релевантный <see cref="ILogManager" /> с использованием 
		/// 	имени класса, сборки и пользовательского суффикса <see cref="GetLoggerNameSuffix" />
		/// 	всегда не-<c>null</c>, может безопасно использоваться в коде
		/// </summary>
		public IUserLog Log {
			get {
				lock (Sync) {
					if (null == _log) {
						var name = GetType().FullName + ";" + GetType().Assembly.GetName().Name;
						var suffix = GetLoggerNameSuffix();
						if (string.IsNullOrEmpty(suffix)) {
							name += ";" + suffix;
						}
						var manager = ResolveService<ILogManager>();
						_log = (null == manager ? new StubUserLog() : manager.GetLog(name, this)) ?? new StubUserLog();
					}
					return _log;
				}
			}
			set { _log = value; }
		}


		/// <summary>
		/// 	Вызывается при вызове Reset
		/// </summary>
		/// <param name="data"> </param>
		/// <returns> любой объект - будет включен в состав результатов <see cref="ResetEventResult" /> </returns>
		/// <remarks>
		/// 	При использовании стандартной настройки из <see cref="ServiceBase" /> не требует фильтрации опций,
		/// 	настраивается на основе атрибута <see cref="RequireResetAttribute" />
		/// </remarks>
		public virtual object Reset(ResetEventData data) {
			return null;
		}


		/// <summary>
		/// 	Возващает объект, описывающий состояние до очистки
		/// </summary>
		/// <returns> </returns>
		public virtual object GetPreResetInfo() {
			return null;
		}


		/// <summary>
		/// 	Спец-строка для указания в составе запроса к <see cref="ILogManager" />
		/// 	по умолчанию - пустая строка. Требуется если несколько классов
		/// 	должны консолидировано использовать общее имя логера, не связанного с 
		/// 	иерархией классов
		/// </summary>
		/// <returns> </returns>
		protected virtual string GetLoggerNameSuffix() {
			return "";
		}

		/// <summary>
		/// 	Унифицированный доступ к разрешению сервисов,
		/// 	может быть перекрыт, по умолчанию получает сервисы из 
		/// 	максимально релевантного контейнера <see cref="Container" />
		/// </summary>
		/// <typeparam name="T"> тип требуемого сервиса </typeparam>
		/// <returns> экземпляр сервиса или Null </returns>
		/// <remarks>
		/// 	<note>Настоятельно рекомендуем использовать именно
		/// 		этот метод при потребности в разрешении сервиса,
		/// 		напрямую
		/// 		<see cref="Container" />
		/// 		,
		/// 		<see cref="SourceContainer" />
		/// 		и
		/// 		<see cref="Application" />
		/// 		должны
		/// 		использовтаься только при выраженной необходимости</note>
		/// </remarks>
		protected virtual T ResolveService<T>(string name = null, params object[] ctorArgs) where T : class {
			return Container.Get<T>(name, ctorArgs);
		}

		/// <summary>
		/// 	Данный метод вызывается в <see cref="SetApplication" /> и призван обеспечить интеграцию с <see cref="ResetEvent" />
		/// </summary>
		/// <remarks>
		/// 	Поведение по умолчанию предполагает наличие атрибута <see cref="RequireResetAttribute" />
		/// 	и настройку стандартного хэндлера <see cref="ResetEvent" />
		/// 	<note>Автоматическая настройка события Reset осуществляется только при загрузке из контейнера в режиме
		/// 		<see cref="Lifestyle.Singleton" />
		/// 	</note>
		/// </remarks>
		protected virtual void SetupResetReaction() {
			if (null == Application.Events) {
				return; //если нет менеджера - не на что настраиватся
			}
			if (null != Component && Component.Lifestyle == Lifestyle.Singleton) {
				var reseton =
					GetType().GetCustomAttributes(typeof (RequireResetAttribute), true).OfType<RequireResetAttribute>().FirstOrDefault();
				if (null != reseton) {
					//значит мы на деле поддерживаем сброс состояния
					var handler = new StandardResetHandler(this, reseton);
					Application.Events.Add(handler); //все, настройка завершена
				}
			}
		}

		/// <summary>
		/// 	Компонент, на основе которого создан данный объект
		/// </summary>
		protected IComponentDefinition Component;

		/// <summary>
		/// 	Контейнер, из которого создан данный объект (в обычных задачах следует использовать не его, а
		/// 	общее свойство Container, с логикой разрешения контейнра)
		/// </summary>
		protected IContainer SourceContainer;

		private IUserLog _log;

		///<summary>
		///	Объект для синхронизации
		///</summary>
		protected object Sync = new object();
	                                    }
}