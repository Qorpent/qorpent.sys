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
		protected  MetaFileDescriptor CheckRegstrable(MetaFileDescriptor descriptor){
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
			if (string.IsNullOrWhiteSpace(savedescriptor.Name))
			{
				savedescriptor.Name = savedescriptor.Code;
			}

			if (string.IsNullOrWhiteSpace(savedescriptor.UserName)){
				savedescriptor.UserName = GetDefaultUserName();
				if (savedescriptor.UserName.Contains("\\")){
					savedescriptor.UserName = savedescriptor.UserName.Split('\\')[1];
				}
			}
			return savedescriptor;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected virtual string GetDefaultUserName(){
			return Applications.Application.Current.Principal.CurrentUser.Identity.Name;
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
			
				savedescriptor = CheckRegstrable(descriptor);
			
			InternalRegister(savedescriptor);
			return savedescriptor.Copy();
		}
		/// <summary>
		/// ���������� ����� ������������, �������, �� ��������� �� ������ ������
		/// </summary>
		public virtual void Refresh(){
			
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