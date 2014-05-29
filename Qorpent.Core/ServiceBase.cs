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
	///	������� ����� �������� - ������������ ����� � ����������� � �����������.
	///</summary>
	///<remarks>
	///	������������� ������� ����� ��� ���� �������� �������� � ����������,
	///	����� ��������� �������������� � ��� ����� ������ ���������������� �������.
	///	������� ������ ������� ������������ ������  - ����������� ����� ���������� � 
	///	���������� ��� �������� - � ����������� � ����������� (���� ������������). ���
	///	����� � ServiceBase ����������� ������� ���������� <see cref="IContainerBound" />,<see cref="IApplicationBound" />.
	///	������ ���������� ServiceBase ������� ��-���� <see cref="Container" /> � <see cref="Log" /> ���
	///	��������������� � ������ ������ � � ��������. 
	///	��� ����������� ������ �������� ������� ������������ ������� <see cref="ResolveService{T}" />, �������
	///	����� ������������ ��� ����� ������ ����������� ����������� ���������.
	///	������ ��������� <see cref="SetContainerContext" /> � <see cref="SetApplication" /> ���
	///	���������� �������������� �������� �� ������ �������� � ������������� ���������.
	///</remarks>
	///<source>Qorpent/Qorpent.Core/ServiceBase.cs</source>
	public abstract class ServiceBase : MarshalByRefObject, IContainerBound, IApplicationBound, IDisposable, IResetable,
	                                    ILogBound {
		/// <summary>
		/// 	�������� ������ �� ����������, � ������� �������� ������ ������ ��������� (����� ���� NULL, ���� 
		/// 	������ ��� ������ �� ���������� ��� ����� � �����������)
		/// </summary>
		protected IApplication Application { get; set; }
#if PARANOID
		static ServiceBase() {
			if(!Qorpent.Security.Watchdog.Paranoid.Provider.OK) throw new  Qorpent.Security.Watchdog.ParanoidException(Qorpent.Security.Watchdog.ParanoidState.GeneralError);
		}
#endif
		/// <summary>
		/// ������� ��������������� ������������� �������� ����������
		/// </summary>
		public bool IgnoreDefaultApplication{ get; set; }

		/// <summary>
		/// 	����������� ����������� ��� ���������� ���������, �� NULL
		/// </summary>
		/// <remarks>
		/// 	������������� ���������� ������� � �������: <see cref="SourceContainer" />,<see cref="Application" />.Container,<see
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
		/// 	���������� � ������ �������� � ������, ���� ��������� ������ � �����������
		/// </summary>
		/// <param name="app"> ����������, � ��������� �������� ��������� ������ </param>
		/// <remarks>
		/// 	����� ���� �������� ��� ���������� �������������� ��������, �� ��������� ������������� �������� <see cref="Application" />
		/// </remarks>
		public virtual void SetApplication(IApplication app) {
			Application = app;
			SetupResetReaction();
		}


		/// <summary>
		/// 	���������� � ������ �������� ���������� ����� ���������� ����������� ��������.
		/// </summary>
		/// <param name="container"> ���������, ��������� ������ </param>
		/// <param name="component"> ���������, ����������� ������ </param>
		/// <remarks>
		/// 	����������� ���������� ���������� <see href="Qorpent.IoC~Qorpent.IoC.Container.html" /> �������������
		/// 	��������� �������� ������������� �� ������ ���������� <see cref="IComponentDefinition.Parameters" /> � �� ������ <see
		/// 	 cref="InjectAttribute" />,
		/// 	������ ����� � ������� ���������� ������������� ��������  <see cref="SourceContainer" /> � <see cref="Component" />,
		/// 	��� ���������� �� ������ ��������� ������ ����������� ����� ���������� ���������� � ��������� ��� ����������� � �����������
		/// </remarks>
		public virtual void SetContainerContext(IContainer container, IComponentDefinition component) {
			SourceContainer = container;
			Component = component;
		}

		/// <summary>
		/// 	����������� ���� ������ ������������ � ��������� ������� <see cref="ITypeResolver.Release" />.
		/// </summary>
		/// <remarks>
		/// 	����� <see cref="ITypeResolver.Release" /> �� ���� ������ � �������� �� ��������������
		/// 	��������� <see cref="Lifestyle.Pooled" />,<see cref="Lifestyle.PerThread" /> � ����������� ��
		/// 	������������ �� ��������. ����� <see cref="ITypeResolver.Release" /> ���������� ��� ��������
		/// 	��������� �����������. �� ���� �� ���� ������ ����� ������� ����������� � �������������
		/// 	������ � ������ ������ �������������, ��� ��� �������� ��� ������ � ������ �����������
		/// 	������� ��� � �������������� ����������� ���� ������ � ������ ��������
		/// 	����� ������������ ����������
		/// </remarks>
		public virtual void OnContainerRelease() {}

		/// <summary>
		/// �������, ���������� ����� ���������� ������������� ��� ������ ����������
		/// </summary>
		public virtual void OnContainerCreateInstanceFinished() {
			
		}


		/// <summary>
		/// 	��������� ��������� <see cref="IDisposable" />, �� ��������� �������� <see cref="ITypeResolver.Release" />, ������������
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
		/// 	������ ��� ������� �������, ������������ ����� ����������� ����������� <see cref="ILogManager" /> � �������������� 
		/// 	����� ������, ������ � ����������������� �������� <see cref="GetLoggerNameSuffix" />
		/// 	������ ��-<c>null</c>, ����� ��������� �������������� � ����
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
		/// 	���������� ��� ������ Reset
		/// </summary>
		/// <param name="data"> </param>
		/// <returns> ����� ������ - ����� ������� � ������ ����������� <see cref="ResetEventResult" /> </returns>
		/// <remarks>
		/// 	��� ������������� ����������� ��������� �� <see cref="ServiceBase" /> �� ������� ���������� �����,
		/// 	������������� �� ������ �������� <see cref="RequireResetAttribute" />
		/// </remarks>
		public virtual object Reset(ResetEventData data) {
			return null;
		}


		/// <summary>
		/// 	��������� ������, ����������� ��������� �� �������
		/// </summary>
		/// <returns> </returns>
		public virtual object GetPreResetInfo() {
			return null;
		}


		/// <summary>
		/// 	����-������ ��� �������� � ������� ������� � <see cref="ILogManager" />
		/// 	�� ��������� - ������ ������. ��������� ���� ��������� �������
		/// 	������ ��������������� ������������ ����� ��� ������, �� ���������� � 
		/// 	��������� �������
		/// </summary>
		/// <returns> </returns>
		protected virtual string GetLoggerNameSuffix() {
			return "";
		}

		/// <summary>
		/// 	��������������� ������ � ���������� ��������,
		/// 	����� ���� ��������, �� ��������� �������� ������� �� 
		/// 	����������� ������������ ���������� <see cref="Container" />
		/// </summary>
		/// <typeparam name="T"> ��� ���������� ������� </typeparam>
		/// <returns> ��������� ������� ��� Null </returns>
		/// <remarks>
		/// 	<note>������������ ����������� ������������ ������
		/// 		���� ����� ��� ����������� � ���������� �������,
		/// 		��������
		/// 		<see cref="Container" />
		/// 		,
		/// 		<see cref="SourceContainer" />
		/// 		�
		/// 		<see cref="Application" />
		/// 		������
		/// 		�������������� ������ ��� ���������� �������������</note>
		/// </remarks>
		protected virtual T ResolveService<T>(string name = null, params object[] ctorArgs) where T : class {
			return Container.Get<T>(name, ctorArgs);
		}

		/// <summary>
		/// 	������ ����� ���������� � <see cref="SetApplication" /> � ������� ���������� ���������� � <see cref="ResetEvent" />
		/// </summary>
		/// <remarks>
		/// 	��������� �� ��������� ������������ ������� �������� <see cref="RequireResetAttribute" />
		/// 	� ��������� ������������ �������� <see cref="ResetEvent" />
		/// 	<note>�������������� ��������� ������� Reset �������������� ������ ��� �������� �� ���������� � ������
		/// 		<see cref="Lifestyle.Singleton" />
		/// 	</note>
		/// </remarks>
		protected virtual void SetupResetReaction() {
			if (null == Application.Events) {
				return; //���� ��� ��������� - �� �� ��� ������������
			}
			if (null != Component && Component.Lifestyle == Lifestyle.Singleton) {
				var reseton =
					GetType().GetCustomAttributes(typeof (RequireResetAttribute), true).OfType<RequireResetAttribute>().FirstOrDefault();
				if (null != reseton) {
					//������ �� �� ���� ������������ ����� ���������
					var handler = new StandardResetHandler(this, reseton);
					Application.Events.Add(handler); //���, ��������� ���������
				}
			}
		}

		/// <summary>
		/// 	���������, �� ������ �������� ������ ������ ������
		/// </summary>
		protected IComponentDefinition Component;

		/// <summary>
		/// 	���������, �� �������� ������ ������ ������ (� ������� ������� ������� ������������ �� ���, �
		/// 	����� �������� Container, � ������� ���������� ���������)
		/// </summary>
		protected IContainer SourceContainer;

		private IUserLog _log;

		///<summary>
		///	������ ��� �������������
		///</summary>
		protected object Sync = new object();
	                                    }
}