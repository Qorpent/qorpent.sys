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
		/// Директория для расположения файлов
		/// </summary>
		public string DirectoryName { get; set; }
		/// <summary>
		/// Внешний репозиторий
		/// </summary>
		public string RemoteName { get; set; }

		/// <summary>
		/// Внешний репозиторий
		/// </summary>
		public string RemoteUrl { get; set; }
		/// <summary>
		/// Требуемый бранч
		/// </summary>
		public string Branch { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string AuthorName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string AuthorEmail { get; set; }
		

		 /// <summary>
		 /// 
		 /// </summary>
		public GitBasedMetaFileRegistry(){
			 AutoRevision = true;
		 }

		private bool _initialized = false;
		private GitHelper _git;
		/// <summary>
		/// 
		/// </summary>
		public void Initialize(){
			if (!_initialized){
				if (string.IsNullOrWhiteSpace(AuthorName)){
					AuthorName = Applications.Application.Current.Principal.CurrentUser.Identity.Name.Split('\\')[1];
				}
				_git = new GitHelper{DirectoryName = DirectoryName,Branch = Branch,RemoteName = RemoteName,RemoteUrl = RemoteUrl, AuthorName = AuthorName,AuthorEmail = AuthorEmail};
				_git.Connect();
				_git.FixBranchState();
			}
		}


		/// <summary>
		/// Устанавливает указанную ревизию в качестве текущей (не требует контента)
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
		/// Получить ревизии файла по коду
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public override IEnumerable<RevisionDescriptor> GetRevisions(string code){
			Initialize();
			return
				_git.GetHistory(code)
				    .Select(_ => new RevisionDescriptor{Revision = _.ShortHash, RevisionTime = _.LocalRevisionTime});
		}

		/// <summary>
		/// Возвращает текущий файл по коду
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public override MetaFileDescriptor GetCurrent(string code){
			Initialize();
			var lastrev = _git.GetHistory(code)[0];
			return ReadFile(new MetaFileDescriptor{Code = code,Revision = lastrev.ShortHash});
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="descriptor"></param>
		/// <returns></returns>
		public override MetaFileDescriptor GetByRevision(MetaFileDescriptor descriptor){
			Initialize();
			return ReadFile(descriptor);
		}

		/// <summary>
		/// Прочитать набор кодов
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<string> GetCodes(string prefix = null){
			Initialize();
			return _git.GetFileList().Where(_ => _.StartsWith(prefix) || ("/" + _).StartsWith(prefix));
		}

		/// <summary>
		/// Проверяет наличие файла с кодом
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public override bool Exists(string code){
			Initialize();
			return File.Exists(_git.GetFullPath(code));
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="savedescriptor"></param>
		protected override void InternalRegister(MetaFileDescriptor savedescriptor){
			Initialize();
			_git.WriteAndCommit(savedescriptor.Code,savedescriptor.Content,savedescriptor.Comment);
			var info = _git.GetCommitInfo();
			savedescriptor.Revision = info.ShortHash;
			savedescriptor.RevisionTime = info.LocalRevisionTime;
			savedescriptor.CheckHash();
		}
	}
}