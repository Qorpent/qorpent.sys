using System;
using System.Threading;
using Qorpent.Model;

namespace Qorpent.Data.MetaStorage {
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class MetaSynchronizerBase<T> :ServiceBase, IMetaSynchronizer<T> where T : class, IWithId, IWithCode, new() {
		/// <summary>
		/// ������� ���������
		/// </summary>
		public IMetaStorage<T> Storage { get; set; }

		/// <summary>
		/// ��������� ��������
		/// </summary>
		public abstract void Load();

		/// <summary>
		/// ��������� ������� ����������
		/// </summary>
		/// <returns></returns>
		public abstract DateTime GetLastVersion();

		/// <summary>
		/// ��������� ��������� �� �� � ���
		/// </summary>
		public abstract void Update();
		/// <summary>
		/// ��������������� ����� ���������� �� ����������
		/// </summary>
		/// <param name="seconds"></param>
		protected void DoAutoUpdate(int seconds) {
			Thread.Sleep(TimeSpan.FromSeconds(seconds));
			if (autoupdate)
			{
				if(Storage.LastSyncTime< GetLastVersion()) {
					Update();
				}
				StartAutoUpdate(seconds);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		protected bool autoupdate = false;

		/// <summary>
		/// ��������� ������� ��������������� ����������� � ��������� ��������������
		/// </summary>
		/// <param name="seconds"></param>
		public void StartAutoUpdate(int seconds = 60) {
			lock (this) {
				autoupdate = true;
				ThreadPool.QueueUserWorkItem(t => DoAutoUpdate((int) t), seconds);
			}
			
		}

		/// <summary>
		/// ��������� ������� ��������������� �����������
		/// </summary>
		public void StopAutoUpdate() {
			lock(this) {
				autoupdate = false;
			}
		}
	}
}