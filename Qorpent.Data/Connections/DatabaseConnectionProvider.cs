using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Qorpent.Events;
using Qorpent.IoC;

namespace Qorpent.Data.Connections {
	/// <summary>
	/// Основная реализация резольвера строк соединений
	/// </summary>
	[ContainerComponent(Lifestyle.Singleton, ServiceType = typeof(IDatabaseConnectionProvider))]
	[RequireReset(All=false,Role = "DEVELOPER", Options = new[]{"db.connection"})]
	public class DatabaseConnectionProvider :ServiceBase, IDatabaseConnectionProvider
	{
#if PARANOID
		static DatabaseConnectionProvider() {
			if(!Qorpent.Security.Watchdog.Paranoid.Provider.OK) throw new  Qorpent.Security.Watchdog.ParanoidException(Qorpent.Security.Watchdog.ParanoidState.GeneralError);
		}
#endif
		/// <summary>
		/// Реальные поставщики строк подключений
		/// </summary>
		[Inject] public IDatabaseConnectionProviderExtension[] SubProviders { get; set; }
		/// <summary>
		/// Список расширений для статического сохранения состояния
		/// </summary>
		[Inject] public IDatabaseConnectionRegistryExtension[] Persisters { get; set; }

		/// <summary>
		/// Статическая резолюция соединений в масштабе приложения
		/// </summary>
		/// <param name="nameOrConnection"></param>
		/// <returns></returns>
		public static string Resolve(string nameOrConnection) {
		
			if(nameOrConnection.Contains(";")) {
				
				return nameOrConnection;
			}
			return Applications.Application.Current.DatabaseConnections.GetConnectionString(nameOrConnection);
		}
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
			Cleanup();
			if(SubProviders!=null) {
				foreach (var provider in SubProviders) {
					foreach (var connection in provider.GetConnections()) {
						Register(connection, false);
					}
				}
			}
			_loaded = true;
		}

		private void Cleanup() {
			foreach (var p in Registry.ToArray()) {
				if (!p.Value.PresereveCleanup) {
					Registry.Remove(p.Key);
				}
			}
		}

		/// <summary>
		/// Default connection provider instance
		/// </summary>
		public static IDatabaseConnectionProvider Default {
			get { return Applications.Application.Current.DatabaseConnections; }
		}

		/// <summary>
		/// Получить соединение по имени
		/// </summary>
		/// <param name="name">Имя соединения</param>
		/// <returns>Содениение</returns>
		public IDbConnection GetConnection(string name) {
			lock(Sync) {
				Reload();
				if(Registry.ContainsKey(name)) {
					var component = Registry[name];
					if(component.InstantiateWithContainer) {
						return component.Container.Get<IDbConnection>(component.ContainerName);
					}else {
						return Activator.CreateInstance(component.ConnectionType, component.ConnectionString) as IDbConnection;
					}
				}else if(name.Contains(";")) { //full connection string
					var connectionString = name;
					if (connectionString.StartsWith("ProviderName"))
					{
						var parsematch = Regex.Match(connectionString, @"^ProviderName=([^;]+);([\s\S]+)$");
						var providername = parsematch.Groups[1].Value;
						var connstring = parsematch.Groups[2].Value;
						if(providername.ToUpper()=="NPGSQL") {
							if(File.Exists(Path.Combine(EnvironmentInfo.BinDirectory,"Npgsql.dll"))) {
								return GetPostGresConnection(connstring);
							}else {
								throw new QorpentException("cannot connect to PostGres because Npgsql not exists in application");
							}
						}
						var provider = System.Data.Common.DbProviderFactories.GetFactory(providername);
						var result = provider.CreateConnection();
						result.ConnectionString = connstring;
						return result;
					}
					else {
						return new SqlConnection(connectionString);
					}
				}
				return null;
			}
		}

		private Assembly _npgsqlassembly = null;
		private Type _npgsqlconnectiontype;

		Assembly NpgSQLAssembly {
			get {
				if(null==_npgsqlassembly) {
					_npgsqlassembly =
						AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name.ToLower().StartsWith("npgsql"));
					if(null==_npgsqlassembly) {
						_npgsqlassembly = Assembly.LoadFrom("Npgsql.dll");
					}
				}
				return _npgsqlassembly;
			}
		}

		Type NpgSQLConnectionType {
			get {
				if(_npgsqlconnectiontype==null) {
					_npgsqlconnectiontype = NpgSQLAssembly.GetType("Npgsql.NpgsqlConnection");
				}
				return _npgsqlconnectiontype;
			}
		}

		private IDbConnection GetPostGresConnection(string connstring) {
			return (IDbConnection)Activator.CreateInstance(NpgSQLConnectionType, connstring);
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
		/// Возвращает список всех имеющихся конфигураций подключений
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ConnectionDescriptor> Enlist() {
			lock(this) {
				Reload();
				return Registry.Values.ToArray();
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