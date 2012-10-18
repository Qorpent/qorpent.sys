using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Qorpent.Events;
using Qorpent.IoC;

namespace Qorpent.Data.Connections {
	/// <summary>
	/// �������� ���������� ���������� ����� ����������
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
		/// �������� ���������� ����� �����������
		/// </summary>
		[Inject] public IDatabaseConnectionProviderExtension[] SubProviders { get; set; }
		/// <summary>
		/// ������ ���������� ��� ������������ ���������� ���������
		/// </summary>
		[Inject] public IDatabaseConnectionRegistryExtension[] Persisters { get; set; }

		/// <summary>
		/// ����������� ��������� ���������� � �������� ����������
		/// </summary>
		/// <param name="nameOrConnection"></param>
		/// <returns></returns>
		public static string Resolve(string nameOrConnection) {
			if(nameOrConnection.Contains(";")) return nameOrConnection;
			return Applications.Application.Current.DatabaseConnections.GetConnectionString(nameOrConnection);
		}
		/// <summary>
		/// 
		/// </summary>
		public DatabaseConnectionProvider() {
			Registry = new Dictionary<string, ConnectionDescriptor>();
		}
		/// <summary>
		/// ��� ������ �����������
		/// </summary>
		protected IDictionary<string, ConnectionDescriptor> Registry { get; private set; }

		private bool _loaded = false;
		/// <summary>
		/// ������ � ���� ���������� ����������� � ������������
		/// </summary>
		/// <returns></returns>
		public override object GetPreResetInfo() {
			return Registry;
		}
		/// <summary>
		/// ����������� ������ ����� ����������� ��� ������� �������
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
		/// ����������� ������ ����������� �� ���� ������������������ ����������
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
		/// �������� ���������� �� �����
		/// </summary>
		/// <param name="name">��� ����������</param>
		/// <returns>����������</returns>
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
		/// �������� ������ ����������� �� �����
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
		/// ��������� ������� ����������� � ����������
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
		/// ���������� ������ ���� ��������� ������������ �����������
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ConnectionDescriptor> Enlist() {
			lock(this) {
				Reload();
				return Registry.Values.ToArray();
			}
		}


		/// <summary>
		/// ���������������� ����� ����������
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
		/// ������� ������ �� ������������
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