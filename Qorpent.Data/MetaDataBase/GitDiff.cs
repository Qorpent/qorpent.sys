using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.Utils.Git;

namespace Qorpent.Data.MetaDataBase
{
	/// <summary>
	/// Опции DIFF на основые GIT
	/// </summary>
	public class GitDiffOptions{
		/// <summary>
		/// Оператор директории GIT
		/// </summary>
		public GitHelper Git { get; set; }
		/// <summary>
		/// Исходная ревизия
		/// </summary>
		public string Source { get; set; }
		/// <summary>
		/// Целевая ревизия
		/// </summary>
		public string Target { get; set; }
		/// <summary>
		/// Маска файлов
		/// </summary>
		public string PathMask { get; set; }
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public string GetTargetRef(){
			return Git.ResolveRef(Target);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public string GetSourceRef()
		{
			return Git.ResolveRef(Source);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public RevisionDistance GetDistance(){
			return Git.GetDistance(GetSourceRef(), GetTargetRef());
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class GitXDiff
	{
		
		private bool? _hasbranch;
		private bool? _hastag;

		/// <summary>
		/// 
		/// </summary>

		public GitXDiff(GitDiffOptions config){
			Config = config;
		}

		bool HasBranch{
			get{
				if (null == _hasbranch){
					_hasbranch = !string.IsNullOrWhiteSpace(Config.GetTargetRef());
				}
				return _hasbranch.Value;
			}
		}

		bool HasTag
		{
			get
			{
				if (null == _hastag)
				{
					_hastag = !string.IsNullOrWhiteSpace(Config.GetSourceRef());
				}
				return _hastag.Value;
			}
		}

		private RevisionDistance _distance = null;
		private GitDiffOptions _config;

		RevisionDistance Distance{
			get{
				if (null == _distance){
					_distance = new RevisionDistance();
					if (HasTag && HasBranch){
						_distance = Config.GetDistance();
					}
				}
				return _distance;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public bool IsConflict{
			get { return Distance.IsConflict; }
		}
		/// <summary>
		/// 
		/// </summary>
		public bool IsUpdateable{
			get { return Distance.IsForwardable; }
		}
		/// <summary>
		/// Настройки дифа
		/// </summary>
		public GitDiffOptions Config{
			get { return _config; }
			private set { _config = value; }
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="fromRevision"></param>
		/// <param name="toRevision"></param>
		/// <param name="phase"></param>
		/// <returns></returns>
		public IEnumerable<XDiffPair> GetAtomicDiffSet(string fromRevision = null, string toRevision = null, int phase = 0){
			fromRevision = fromRevision ?? Config.Source;
			toRevision = toRevision ?? Config.Target;
			var from = Config.Git.GetCommitInfo(fromRevision);
			var to = Config.Git.GetCommitInfo(toRevision);
			var changes = Config.Git.GetChangedFilesList(fromRevision, toRevision, "main")
				.Where(_=>string.IsNullOrWhiteSpace(Config.PathMask)||Regex.IsMatch(_,Config.PathMask)).ToArray();
			foreach (var fileRecord in changes){
				var src = Config.Git.GetContent(fileRecord.FileName, fromRevision);
				var trg = Config.Git.GetContent(fileRecord.FileName, toRevision);
				if (null != trg){
					XElement xsrc;
					XElement xtrg;
					if (null == src){
						xsrc = new XElement("stub");
					}
					else{
						xsrc = XElement.Parse(src);
					}
					xtrg = XElement.Parse(trg);
					yield return new XDiffPair{Phase = phase,BaseRevision = from, PatchRevision = to,BaseContent = xsrc,PatchContent = xtrg};
				}
			}
		}
		/// <summary>
		/// Собирает последоватльный набор обновлений по файлам XDiff
		/// </summary>
		/// <param name="fromRevision"></param>
		/// <param name="toRevision"></param>
		/// <param name="phase"></param>
		/// <returns></returns>
		public IEnumerable<XDiffPair> GetDiffSet(string fromRevision = null, string toRevision = null, int phase = 0){
			fromRevision = fromRevision ?? Config.Source;
			toRevision = toRevision ?? Config.Target;
			var log = Config.Git.GetCommitList(fromRevision, toRevision);
			var from = log[0];
			var ph = phase;
			for (var i = 1; i < log.Length; i++){
				var to = log[i];
				foreach (var diff in GetAtomicDiffSet(from,to,ph++)){
					yield return diff;
				}
				from = log[i];
				phase++;
			}
		}
	}
	/// <summary>
	/// Представляет собой пару файлов, которые требуют изменения между версиями
	/// </summary>
	public class XDiffPair{
		/// <summary>
		/// Фаза выполнения диффа
		/// </summary>
		public int Phase { get; set; }
		/// <summary>
		/// Базовая ревизия
		/// </summary>
		public GitCommitInfo BaseRevision { get; set; }
		/// <summary>
		/// Патч-ревизия
		/// </summary>
		public GitCommitInfo PatchRevision { get; set; }
		/// <summary>
		/// Базовый контент
		/// </summary>
		public XElement BaseContent { get; set; }
		/// <summary>
		/// Целевой контент
		/// </summary>
		public XElement PatchContent { get; set; }
	}
}
