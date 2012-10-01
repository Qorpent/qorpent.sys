using System;
using System.Collections.Generic;
using System.Data;
using Qorpent.Events;
using Qorpent.IoC;

namespace Qorpent.Data.Connections {
	/// <summary>
	/// Основная реализация резольвера строк соединений
	/// </summary>
	[ContainerComponent(Lifestyle.Singleton, ServiceType = typeof(IDatabaseConnectionProvider))]
	[RequireReset(All=false,Role = "DEVELOPER", Options = new[]{"db.connection"})]
	public class DatabaseConnectionProvider :ServiceBase, IDatabaseConnectionProvider {

		/// <summary>
		/// Реальные поставщики строк подключений
		/// </summary>
		[Inject] public IDatabaseConnectionProviderExtension[] SubProviders { get; set; }
		/// <summary>
		/// Список расширений для статического сохранения состояния
		/// </summary>
		[Inject] public IDatabaseConnectionRegistryExtension[] Persisters { get; set; }


		/// <summary>
		/// 
		/// </summary>
		public DatabaseConnectionProvider() {
			Registry = new Dictionary<string, ConnectionDescriptor>();
		}
		/// <summary>
		/// Сам реестр подключений
		/// </summary>
		protected IDictionary<string, ConnectionDescriptor> Registry { get; private set; }

		private bool _loaded = false;
		/// <summary>
		/// Реестр и есть собственно информацией о перезагрузке
		/// </summary>
		/// <returns></returns>
		public override object GetPreResetInfo() {
			return Registry;
		}
		/// <summary>
		/// Перегружает реестр строк подключений при запросе системы
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public override object Reset(ResetEventData data) {
			lock(this) {
				_loaded = false;
				Reload();
				return Registry;
			}
		}
		/// <summary>
		/// Перегружает реестр подключений из всех сконфигурированных источников
		/// </summary>
		protected void Reload() {
			if(_loaded) return;
			Registry.Clear();
			foreach (var provider in SubProviders) {
				foreach (var connection in provider.GetConnections()) {
					Register(connection,false);
				}
			}
			_loaded = true;
		}

		/// <summary>
		/// Получить соединение по имени
		/// </summary>
		/// <param name="name">Имя соединения</param>
		/// <returns>Содениение</returns>
		public IDbConnection GetConnection(string name) {
			lock(this) {
				Reload();
				if(Registry.ContainsKey(name)) {
					var component = Registry[name];
					if(component.InstantiateWithContainer) {
						return component.Container.Get<IDbConnection>(component.ContainerName);
					}else {
						return Activator.CreateInstance(component.ConnectionType, component.ConnectionString) as IDbConnection;
					}
				}
				return null;
			}
		}

		/// <summary>
		/// Получить строку подключения по имени
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public string GetConnectionString(string name) {
			lock(this) {
				Reload();
				if(Registry.ContainsKey(name)) {
					return Registry[name].ConnectionString;
				}
				return null;
			}
		}

		/// <summary>
		/// Проверяет наличие подключения у поставщика
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool Exists(string name) {
			lock(this) {
				Reload();
				return Registry.ContainsKey(name);
			}
		}
		

		/// <summary>
		/// Зарегистрировать новое соединение
		/// </summary>

		public void Register(ConnectionDescriptor connectionDescriptor, bool persistent) {
			lock(this) {
				Registry[connectionDescriptor.Name] = connectionDescriptor;
				if(persistent) {
					foreach (var persister in Persisters) {
						persister.Register(connectionDescriptor);
					}
				}
			}
		}

		/// <summary>
		/// Удалить строку из регистратора
		/// </summary>
		/// <param name="name"></param>
		/// <param name="persistent"></param>
		public void UnRegister(string name, bool persistent) {
			lock(this) {
				Registry.Remove(name);
				if(persistent) {
					foreach (var persister in Persisters)
					{
						persister.Unregister(name);
					}
				}
			}
		}
	}
}