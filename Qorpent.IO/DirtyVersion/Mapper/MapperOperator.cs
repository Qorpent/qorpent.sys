using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Applications;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO.DirtyVersion.Mapper
{
	/// <summary>
/// Оператор журнала
/// </summary>
	public class MapperOperator:HashedDirectoryBase {
		private const string NULLCOMMIT = "init";

		/// <summary>
		/// Основной конструктор
		/// </summary>

		public MapperOperator(string targetDirectoryName, bool compress = true, int hashsize = Const.HashSize) :
			base(targetDirectoryName, compress,hashsize) {}
		/// <summary>
		/// Создает запись о коммите
		/// </summary>
		/// <param name="record"></param>
		public MapRecord Write(MapRecord record) {
			SetupRecord(record);

			var filename = ConvertToFileName(_rootDirectory,record.NameHash);
			if (!File.Exists(filename)) {
				return WriteFirstRecord(filename,record);
			}
			return UpdateRecord(filename, record);
		}
		/// <summary>
		/// Передвигает головной коммит на указанный, должен присутствовать  в наборе
		/// </summary>
		public void MoveHead(MapRecord record)
		{
			SetupRecord(record);
			var filename = ConvertToFileName(_rootDirectory, record.NameHash);
			if (!File.Exists(filename)) {
				throw new Exception("file not found");
			}
			var xml = XElement.Load(filename);
			var e = xml.Element(record.NewDataHash);
			if (null == e) {
				throw new Exception("commit not found");
			}
			xml.SetAttributeValue("head",record.NewDataHash);
			xml.Save(filename);
		}

		/// <summary>
		/// Передвигает головной коммит на указанный, должен присутствовать  в наборе
		/// </summary>
		public void Delete(MapRecord record)
		{
			SetupRecord(record);
			var filename = ConvertToFileName(_rootDirectory, record.NameHash);
			if (!File.Exists(filename))
			{
				throw new Exception("file not found");
			}
			var xml = XElement.Load(filename);
			var e = xml.Element(record.NewDataHash);
			ValidateDeletion(record, e, xml);
			e.Remove();
			RemoveReferences(record, xml);
			xml.Save(filename);
		}
		/// <summary>
		/// Возращает головной хэш
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public string GetHead(string name) {
			var filename = ConvertToFileName(_rootDirectory, _hasher.GetHash(name));
			if (!File.Exists(filename)) {
				throw new Exception("file not found");
			}
			var x = XElement.Load(filename);
			return x.Attr("head");
		}
		/// <summary>
		/// Возращает головной хэш
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> GetNotMerged(string name, bool branchheadsonly  = false)
		{
			var filename = ConvertToFileName(_rootDirectory, _hasher.GetHash(name));
			if (!File.Exists(filename))
			{
				throw new Exception("file not found");
			}
			var x = XElement.Load(filename);
			var all = x.Elements().Select(_ => _.Name.LocalName).ToList();
			RemoveMerged(x, all, x.Attr("head"),true);
			if (branchheadsonly) {
				foreach (var c in all.ToArray()) {
					RemoveMerged(x, all, c, false);
				}
			}
			return all.ToArray();
		}

		private void RemoveMerged(XElement x, List<string> all, string code,bool removeself) {
			if (removeself) {
				all.Remove(code);
			}
			var e = x.Element(code);
			var src = e.Attr("src");
			if ("init" == src) {
				return;
			}
			else if ("merge" == src) {
				foreach (var c in e.Elements()) {
					if (c.Name.LocalName != "pre") {
						RemoveMerged(x, all, c.Name.LocalName, true);
					}
				}
			}
			else {
				RemoveMerged(x, all, src, true);
			}
		}

		private static void RemoveReferences(MapRecord record, XElement xml) {
			foreach (var all in xml.Elements()) {
				if (all.Attr("src") == record.NewDataHash) {
					all.SetAttributeValue("src", "init");
				}
				else if (all.Attr("src") == "merged") {
					var r = all.Element(record.NewDataHash);
					if (null != r) {
						r.Remove();
						var others = all.Elements().Where(_ => _.Name.LocalName != "pre").ToArray();
						if (1 == others.Length) {
							all.SetAttributeValue("src", others[0].Name.LocalName);
							others.Remove();
						}
					}
				}
			}
		}

		private static void ValidateDeletion(MapRecord record, XElement e, XElement xml) {
			if (null == e) {
				throw new Exception("commit not found");
			}
			if (xml.Attr("head") == record.NewDataHash) {
				throw new Exception("commit is head");
			}
		}

		private void SetupRecord(MapRecord record) {
			if (string.IsNullOrWhiteSpace(record.NewDataHash)) {
				throw new Exception("empty commit");
			}
			if (string.IsNullOrWhiteSpace(record.NameHash) && string.IsNullOrWhiteSpace(record.Name)) {
				throw new Exception("no name");
			}

			if (string.IsNullOrWhiteSpace(record.NameHash)) {
				record.NameHash = _hasher.GetHash(record.Name);
			}
			if (string.IsNullOrWhiteSpace(record.Commiter)) {
				record.Commiter = Application.Current.Principal.CurrentUser.Identity.Name;
			}
			if (1900 >= record.VersionTime.Year) {
				record.VersionTime = DateTime.Now;
			}
			if (null == record.SourceDataHashes || 0 == record.SourceDataHashes.Length) {
				record.SourceDataHashes = new[] {NULLCOMMIT};
			}
		}
		/// <summary>
		/// Возвращает полную информацию о хранимом файле
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public XElement GetFullInfo(string name) {
			var filename = ConvertToFileName(_rootDirectory, _hasher.GetHash(name));
			if (!File.Exists(filename)) throw new Exception("file not existed");
			return XElement.Load(filename);
		}

		private MapRecord WriteFirstRecord(string filename, MapRecord record) {
			Directory.CreateDirectory(Path.GetDirectoryName(filename));
			var xml = new XElement("map");
			xml.SetAttributeValue("name",record.Name);
			xml.SetAttributeValue("namehash",record.NameHash);
			AddNewCommit(record, xml);
			record.IsHead = true;
			xml.SetAttributeValue("head",record.NewDataHash);
			xml.Save(filename);
			return record;
		}

		private static void AddNewCommit(MapRecord record, XElement xml) {
			var commit = new XElement(record.NewDataHash);
			commit.SetAttributeValue("time", record.VersionTime);
			commit.SetAttributeValue("commiter", record.Commiter);
			if (1 == record.SourceDataHashes.Length) {
				commit.SetAttributeValue("src", record.SourceDataHashes[0]);
			}
			else {
				commit.SetAttributeValue("src", "merged");
				foreach (var src in record.SourceDataHashes) {
					commit.Add(new XElement(src));
				}
			}
			xml.Add(commit);
		}

		private MapRecord UpdateRecord(string filename, MapRecord record) {
			var xml = XElement.Load(filename);
			XElement existed = null;
			if (null ==( existed= xml.Element(record.NewDataHash))) {
				AddNewCommit(record, xml);
				
			}
			else {
				record.IsHead = record.NewDataHash == xml.Attr("head");
				MergeCommitHistory(record, existed);
				MergeCommitSources(record, existed);
				RefineRecord(record,existed);
			}
			if (record.SourceDataHashes.Any(_ => _ == xml.Attr("head")))
			{
				xml.SetAttributeValue("head", record.NewDataHash);
				record.IsHead = true;
			}
			xml.Save(filename);
			return record;
		}

		private void RefineRecord(MapRecord record, XElement existed) {
			record.VersionTime = Convert.ToDateTime(existed.Attr("time"));
			record.Commiter = existed.Attr("commiter");
			if (existed.Attr("src") == "merged") {
				record.SourceDataHashes = existed.Elements().Where(_ => _.Name.LocalName != "pre")
				                                 .Select(_ => _.Name.LocalName).ToArray();
			}
			else {
				record.SourceDataHashes = new[] {existed.Attr("src")};
			}
		}

		private void MergeCommitSources(MapRecord record, XElement existed) {
			if (record.SourceDataHashes.Length == 1 && record.SourceDataHashes[0] == existed.Attr("src")) {
				return; //нечего сливать
			}
			if (existed.Attr("src") != "merged") {
				existed.Add(new XElement(existed.Attr("src")));
				existed.SetAttributeValue("src", "merged");
			}
			foreach (var s in record.SourceDataHashes) {
				if (null == existed.Element(s)) {
					existed.Add(new XElement(s));
				}
			}
		}

		private static void MergeCommitHistory(MapRecord record, XElement existed) {
			var ver = Convert.ToDateTime(existed.Attr("time"));
			if (record.VersionTime >= ver) {
				existed.SetAttributeValue("time", record.VersionTime);
				existed.Add(
					new XElement("pre",
					             new XAttribute("commiter", existed.Attr("commiter")),
					             new XAttribute("time", existed.Attr("time"))
						)
					);
				existed.SetAttributeValue("commiter", record.Commiter);
			}
			else {
				existed.Add(
					new XElement("pre",
					             new XAttribute("commiter", record.Commiter),
					             new XAttribute("time", record.VersionTime)
						)
					);
			}
		}
	}
}
