using System;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO.DirtyVersion.Mapping {
	/// <summary>
	/// Сериализатор информации о версии
	/// </summary>
	public class MappingInfoSerializer {
		private const string COMMITERATTRIBUTE = "a";
		private const string MAPPINGNAMEATTRIBUTE = "n";
		private const string MAPPINGHASHATTRIBUTE = "nh";
		private const string HEADATTRIBUTE = "h";
		private const string TIMEATTRIBUTE = "t";
		private const string SRCTYPEATTRIBUTE = "st";
		private const string SRCATTRIBUTE = "s";
		private const string COELEMENT = "co";
		private const string SOURCESELEMENT = "ss";
		private const string COMMITSELEMENT = "cs";
		private const string ALIASESELEMENT = "as";
		private const string MAPPINGELEMENT = "map";

		

		private static void ReadCommits(XElement xml, MappingInfo result) {
			var commits = xml.Element(COMMITSELEMENT);
			if (null != commits) {
				foreach (var e in commits.Elements()) {
					var commit = new Commit();
					commit.Hash = e.Name.LocalName;
					ReadCommitAuthorInfo(commit, e);
					ReadCommitSourceInfo(commit, e);
					result.Commits[commit.Hash] = commit;
				}
			}
		}

		private static void ReadCommitSourceInfo(Commit commit, XElement e) {
			commit.SourceType = e.Attr(SRCTYPEATTRIBUTE).To<CommitSourceType>();
			if (commit.SourceType == CommitSourceType.Single) {
				commit.Sources.Add(e.Attr(SRCATTRIBUTE));
			}
			else if (commit.SourceType == CommitSourceType.Merged) {
				var sources = e.Element(SOURCESELEMENT);
				if (null != sources) {
					foreach (var c in sources.Elements()) {
						commit.Sources.Add(c.Name.LocalName);
					}
				}
			}
		}

		private static void ReadCommitAuthorInfo(Commit commit, XElement e) {
			commit.Author = new CommitAuthorInfo {
				Commiter = e.Attr(COMMITERATTRIBUTE),
				Time = Convert.ToDateTime(e.Attr(TIMEATTRIBUTE))
			};
			foreach (var co in e.Elements(COELEMENT)) {
				commit.CoAuthors.Add(new CommitAuthorInfo {
					Commiter = co.Attr(COMMITERATTRIBUTE),
					Time = Convert.ToDateTime(co.Attr(TIMEATTRIBUTE))
				});
			}
		}



		private static void ReadAliases(XElement xml, MappingInfo result) {
			var aliases = xml.Element(ALIASESELEMENT);
			if (null != aliases) {
				foreach (var e in aliases.Elements()) {
					result.Aliases[e.Value] = e.Name.LocalName;
				}
			}
		}

		/// <summary>
		/// Выполняет десериализацию из XML
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public MappingInfo DeSerialize(XElement xml)
		{
			var result = new MappingInfo
			{
				Name = xml.Attr(MAPPINGNAMEATTRIBUTE),
				NameHash = xml.Attr(MAPPINGHASHATTRIBUTE),
				Head = xml.Attr(HEADATTRIBUTE)
			};
			ReadAliases(xml, result);
			ReadCommits(xml, result);
			return result;
		}

		/// <summary>
		///Выполняет сериализацию в XML
		/// </summary>
		/// <param name="info"></param>
		/// <returns></returns>
		public XElement Serialize(MappingInfo info) {
			var result = new XElement(MAPPINGELEMENT);
			result.SetAttributeValue(MAPPINGNAMEATTRIBUTE,info.Name);
			result.SetAttributeValue(MAPPINGHASHATTRIBUTE, info.NameHash);
			result.SetAttributeValue(HEADATTRIBUTE, info.Head);
			WriteAliases(info, result);
			WriteCommits(info, result);
			return result;
		}

		private static void WriteCommits(MappingInfo info, XElement result) {
			if (0 != info.Commits.Count) {
				var commits = new XElement(COMMITSELEMENT);
				foreach (var c in info.Commits.Values) {
					WriteCommit(c, commits);
				}
				result.Add(commits);
			}
		}

		private static void WriteCommit(Commit c, XElement commits) {
			var commit = new XElement(c.Hash);
			commit.SetAttributeValue(COMMITERATTRIBUTE, c.Author.Commiter);
			commit.SetAttributeValue(TIMEATTRIBUTE, c.Author.Time);
			commit.SetAttributeValue(SRCTYPEATTRIBUTE, c.SourceType);
			WriteCoAuthors(c, commit);
			WriteSources(c, commit);
			commits.Add(commit);
		}

		private static void WriteSources(Commit c, XElement commit) {
			if (c.HasSources()) {
				if (c.SourceType == CommitSourceType.Single) {
					commit.SetAttributeValue(SRCATTRIBUTE, c.Sources[0]);
				}
				else if (c.SourceType == CommitSourceType.Merged) {
					var sources = new XElement(SOURCESELEMENT);
					foreach (var s in c.Sources) {
						sources.Add(new XElement(s));
					}
					commit.Add(sources);
				}
			}
		}

		private static void WriteCoAuthors(Commit c, XElement commit) {
			if (c.HasCoAuthors()) {
				foreach (var co in c.CoAuthors) {
					var coel = new XElement(COELEMENT);
					coel.SetAttributeValue(COMMITERATTRIBUTE, co.Commiter);
					coel.SetAttributeValue(TIMEATTRIBUTE, co.Time);
					commit.Add(coel);
				}
			}
		}

		private static void WriteAliases(MappingInfo info, XElement result) {
			if (0 != info.Aliases.Count) {
				var aliases = new XElement(ALIASESELEMENT);
				foreach (var a in info.Aliases) {
					aliases.Add(new XElement(a.Value, a.Key));
				}
				result.Add(aliases);
			}
		}
	}
}