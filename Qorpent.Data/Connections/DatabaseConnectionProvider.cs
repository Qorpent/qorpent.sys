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
// PROJECT ORIGIN: Qorpent.Data/DatabaseConnectionProvider.cs
#endregion
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
using Qorpent.Utils.Extensions;

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
	    /// �������� ���������� �� �����
	    /// </summary>
	    /// <param name="name">��� ����������</param>
	    /// <param name="defaultConnectionString"></param>
	    /// <returns>����������</returns>
	    public IDbConnection GetConnection(string name, string defaultConnectionString = null) {
			lock(Sync) {
				Reload();
				if(Registry.ContainsKey(name)) {
					var component = Registry[name];
					if(component.InstantiateWithContainer) {
						return component.Container.Get<IDbConnection>(component.ContainerName);
					}else {
						return Activator.CreateInstance(component.ConnectionType, component.ConnectionString) as IDbConnection;
					}
				}else if(name.Contains(";")){
					//full connection string
					return DatabaseExtensions.CreateDatabaseConnectionFromString(name);
				}
				if (defaultConnectionString.IsNotEmpty())
			    {
			        return new SqlConnection(defaultConnectionString);
			    }
				return null;
			}
		}

		/// <summary>
	    /// �������� ������ ����������� �� �����
	    /// </summary>
	    /// <param name="name"></param>
	    /// <param name="defaultConnectionString"></param>
	    /// <returns></returns>
	    public string GetConnectionString(string name, string defaultConnectionString = null) {
			lock(this) {
				Reload();
				if(Registry.ContainsKey(name)) {
					return Registry[name].ConnectionString;
				}
				return defaultConnectionString;
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