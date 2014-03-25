using System;
using System.Collections.Generic;

namespace Qorpent.Data.MetaDataBase{
	/// <summary>
	/// 
	/// </summary>
	public abstract class MetaFileRegistryBase:ServiceBase,IMetaFileRegistry{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="descriptor"></param>
		/// <returns></returns>
		protected static MetaFileDescriptor CheckRegstrable(MetaFileDescriptor descriptor){
			if (!descriptor.IsFullyDefined()){
				throw new Exception("cannot register not fully qualified descriptor");
			}
			var savedescriptor = descriptor.Copy();
			if (string.IsNullOrWhiteSpace(savedescriptor.Revision)){
				savedescriptor.Revision = Guid.NewGuid().ToString().Replace("-", "");
			}
			if (savedescriptor.RevisionTime.Year <= 1900){
				savedescriptor.RevisionTime = DateTime.Now;
			}
			return savedescriptor;
		}
		/// <summary>
		/// 
		/// </summary>
		public bool AutoRevision { get; set; }
		/// <summary>
		/// ���������� (��� ���������� ���� � ������ ��������)
		/// </summary>
		/// <param name="descriptor"></param>
		public MetaFileDescriptor Register(MetaFileDescriptor descriptor){

			MetaFileDescriptor savedescriptor = null;
			if (AutoRevision){
				savedescriptor = descriptor.Copy();
			}
			else{
				savedescriptor = CheckRegstrable(descriptor);
			}
			InternalRegister(savedescriptor);
			return savedescriptor.Copy();
		}

		/// <summary>
		/// ������������� ��������� ������� � �������� ������� (�� ������� ��������)
		/// </summary>
		/// <param name="descriptor"></param>
		public abstract MetaFileDescriptor Checkout(MetaFileDescriptor descriptor);

		/// <summary>
		/// �������� ������� ����� �� ����
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public abstract IEnumerable<RevisionDescriptor> GetRevisions(string code);

		/// <summary>
		/// ���������� ������� ���� �� ����
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public abstract MetaFileDescriptor GetCurrent(string code);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="descriptor"></param>
		/// <returns></returns>
		public abstract MetaFileDescriptor GetByRevision(MetaFileDescriptor descriptor);

		/// <summary>
		/// ��������� ����� �����
		/// </summary>
		/// <returns></returns>
		public abstract IEnumerable<string> GetCodes(string prefix = null);

		/// <summary>
		/// ��������� ������� ����� � �����
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public abstract bool Exists(string code);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="savedescriptor"></param>
		protected abstract void InternalRegister(MetaFileDescriptor savedescriptor);
	}
}