using System;
using System.Collections.Generic;
using System.Threading;
using Qorpent.Model;

namespace Qorpent.Data.MetaStorage {
	/// <summary>
	/// ������� ����-���������
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class MetaStorageBase<T> : ServiceBase,IMetaStorage<T> where T : class, IWithId, IWithCode, new() {
		/// <summary>
		/// ������� ������� read- �����������
		/// </summary>
		protected int ReadCount;

		/// <summary>
		/// ���������� �� ������
		/// </summary>
		protected object ReadLock = new object();

		/// <summary>
		/// ���������� �� ������
		/// </summary>
		protected object WriteLock = new object();

		/// <summary>
		/// 
		/// </summary>
		public MetaStorageBase() {
			IdCache = new Dictionary<int, T>();
			CodeCache = new Dictionary<string, T>();
			QueryCache = new Dictionary<string, T[]>();
			Options = new Dictionary<string, string>();
		}

		

		/// <summary>
		/// ��������������� ����� ��� ������������� �� ������
		/// </summary>
		/// <typeparam name="TIt"></typeparam>
		protected class ReadHandler<TIt> :IDisposable where TIt : class, IWithId, IWithCode, new() {
			private MetaStorageBase<TIt> _parent;
			/// <summary>
			/// </summary>
			/// <param name="parent"></param>
			public ReadHandler(MetaStorageBase<TIt> parent ) {
				_parent = parent;
				
				lock (parent.WriteLock) {
					
					if(!Monitor.IsEntered(_parent.ReadLock)) {
						Monitor.Enter(_parent.ReadLock);
					}
					_parent.ReadCount++;
				}
			}

			/// <summary>
			/// ��������� ������������ ����������� ������, ��������� � �������������� ��� ������� ������������� ��������.
			/// </summary>
			/// <filterpriority>2</filterpriority>
			public void Dispose() {
				_parent.ReadCount--;
				if(0==_parent.ReadCount) {
					Monitor.Exit(_parent.ReadLock);
				}
			}
		}

		/// <summary>
		/// ��������������� ����� ��� ������������� �� ������
		/// </summary>
		/// <typeparam name="TIt"></typeparam>
		protected class WriteHandler<TIt> : IDisposable where TIt : class, IWithId, IWithCode, new()
		{
			private MetaStorageBase<TIt> _parent;
			/// <summary>
			/// /
			/// </summary>
			/// <param name="parent"></param>
			public WriteHandler(MetaStorageBase<TIt> parent)
			{
				_parent = parent;
				Monitor.Enter(_parent.WriteLock);
				Monitor.Enter(_parent.ReadLock);
				_parent.IsInWriteState = true;
			}

			/// <summary>
			/// ��������� ������������ ����������� ������, ��������� � �������������� ��� ������� ������������� ��������.
			/// </summary>
			/// <filterpriority>2</filterpriority>
			public void Dispose() {
				_parent.IsInWriteState = false;
				Monitor.Exit(_parent.ReadLock);
				Monitor.Exit(_parent.WriteLock);
			}
		}

		/// <summary>
		/// ��� ID - ��������
		/// </summary>
		public IDictionary<int, T> IdCache { get; private set; }

		/// <summary>
		/// ��� CODE - ��������
		/// </summary>
		public IDictionary<string, T> CodeCache { get; private set; }

		/// <summary>
		/// ��� ������ - ����� ���������
		/// </summary>
		public IDictionary<string, T[]> QueryCache { get; private set; }

		/// <summary>
		/// ����� ��c������ �������������
		/// </summary>
		public DateTime LastSyncTime { get; set; }

		/// <summary>
		/// True - ���� ������ ������������ �� ������
		/// </summary>
		public bool IsInWriteState { get; protected set; }

		/// <summary>
		/// True - ���� �� ������� ������ ����������� �������� ������
		/// </summary>
		public bool IsInReadState { get { return ReadCount != 0; } }

		/// <summary>
		/// ������� ���������������� �����
		/// </summary>
		public IDictionary<string, string> Options { get; private set; }

		/// <summary>
		/// �������� ������ ������������� �� ������
		/// </summary>
		/// <returns></returns>
		public IDisposable EnterWrite() {
			return new WriteHandler<T>(this);
		}

		/// <summary>
		/// ��������� ������ ������������� �� ������ (������ ���� � try/finally) - �����������
		/// </summary>
		/// <summary>
		/// �������� ������ ������������� �� ������ (������ ���� � try/finally) - ������������
		/// </summary>
		/// <returns></returns>
		public IDisposable EnterRead() {
			return new ReadHandler<T>(this);
		}

		/// <summary>
		/// ���������� ����� ��������� �������� ���� �� ��
		/// </summary>
		public void AfterInitialLoad() {
			
		}

		/// <summary>
		/// ���������� ����� ��������� �������� ���� �� ��
		/// </summary>
		/// <param name="updatedItems"></param>
		public void AfterUpdate(T[] updatedItems) {
			
		}

		/// <summary>
		/// ������� ���������
		/// </summary>
		public void Clear() {
			using(EnterWrite()) {
				IdCache.Clear();
				CodeCache.Clear();
				QueryCache.Clear();
			}
		}

		/// <summary>
		/// ������� ����� ��������� ������������ ������
		/// </summary>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public virtual IMetaFacade<T> CreateFacade() {
			throw new NotImplementedException();
		}

		/// <summary>
		/// ��
		/// </summary>
		/// <returns></returns>
		public virtual IMetaSynchronizer<T> GetSynchronizer() {
			throw new NotImplementedException();
		}
	}
}