using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Qorpent.Utils;

namespace Qorpent.Data.MetaDataBase{
	/// <summary>
	/// 
	/// </summary>
	public class GitBasedMetaFileRegistry:MetaFileRegistryBase
	{
		/// <summary>
		/// ���������� ��� ������������ ������
		/// </summary>
		public string Directory { get; set; }
		/// <summary>
		/// ������� �����������
		/// </summary>
		public string RemoteName { get; set; }

		/// <summary>
		/// ������� �����������
		/// </summary>
		public string RemoteUrl { get; set; }
		/// <summary>
		/// ��������� �����
		/// </summary>
		public string Branch { get; set; }


		

		 /// <summary>
		 /// 
		 /// </summary>
		public GitBasedMetaFileRegistry(){
			
		}

		private bool _initialized = false;
		private GitHelper _git;
		/// <summary>
		/// 
		/// </summary>
		public void Initialize(){
			if (!_initialized){
				_git = new GitHelper{DirectoryName = Directory,Branch = Branch,RemoteName = RemoteName,RemoteUrl = RemoteUrl, AuthorName = Applications.Application.Current.Principal.CurrentUser.Identity.Name};
				_git.Connect();
				_git.FixBranchState();
			}
		}


		/// <summary>
		/// ������������� ��������� ������� � �������� ������� (�� ������� ��������)
		/// </summary>
		/// <param name="descriptor"></param>
		public override MetaFileDescriptor Checkout(MetaFileDescriptor descriptor){
			_git.RestoreSingleFile(descriptor.Code,descriptor.Revision);
			return ReadFile(descriptor);
		}

		private MetaFileDescriptor ReadFile(MetaFileDescriptor descriptor){
			var content = _git.GetContent(descriptor.Code, descriptor.Revision);
			var info = _git.GetCommitInfo(descriptor.Revision);
			var result = new MetaFileDescriptor{
				Code = descriptor.Code,
				Comment = info.Comment,
				Content = content,
				Revision = info.ShortHash,
				RevisionTime = info.LocalRevisionTime
			};
			result.CheckHash();
			return result;
		}

		/// <summary>
		/// �������� ������� ����� �� ����
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public override IEnumerable<RevisionDescriptor> GetRevisions(string code){
			return
				_git.GetHistory(code)
				    .Select(_ => new RevisionDescriptor{Revision = _.ShortHash, RevisionTime = _.LocalRevisionTime});
		}

		/// <summary>
		/// ���������� ������� ���� �� ����
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public override MetaFileDescriptor GetCurrent(string code){
			return ReadFile(new MetaFileDescriptor{Code = code});
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="descriptor"></param>
		/// <returns></returns>
		public override MetaFileDescriptor GetByRevision(MetaFileDescriptor descriptor){
			return ReadFile(descriptor);
		}

		/// <summary>
		/// ��������� ����� �����
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<string> GetCodes(string prefix = null){
			return _git.GetFileList().Where(_ => _.StartsWith(prefix) || ("/" + _).StartsWith(prefix));
		}

		/// <summary>
		/// ��������� ������� ����� � �����
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public override bool Exists(string code){
			return File.Exists(_git.GetFullPath(code));
		}

		protected override void InternalRegister(MetaFileDescriptor savedescriptor){
			_git.WriteAndCommit(savedescriptor.Code,savedescriptor.Comment);
			var info = _git.GetCommitInfo();
			savedescriptor.Revision = info.ShortHash;
			savedescriptor.RevisionTime = info.LocalRevisionTime;
		}
	}
}