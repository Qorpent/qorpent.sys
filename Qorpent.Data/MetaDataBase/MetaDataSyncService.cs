using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.Bxl;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.XDiff;

namespace Qorpent.Data.MetaDataBase
{

	


	/// <summary>
	/// Сервис по обcлуживанию и синхронизации локальной БД
	/// </summary>
	public class MetaDataSyncService:ServiceBase,ICustomMerger
	{
		private IMetaFileProcessor _metaFileProcessor;

		/// <summary>
		/// Обработчик файлов
		/// </summary>
		[Inject]
		public IMetaFileProcessor MetaFileProcessor{
			get { return _metaFileProcessor??(_metaFileProcessor = new DefaultMetaFileProcessor()); }
			set { _metaFileProcessor = value; }
		}

		/// <summary>
		/// Соедингение с целевой БД
		/// </summary>
		public string SqlConnection { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string LastSql{
			get { return MetaFileProcessor.LastSql; }
		}

		/// <summary>
		/// Целевое хранилище с текущими даннымми
		/// </summary>
		[Inject(Name = "target.metastorage")]
		public IMetaFileRegistry TargetStorage { get; set; }
		/// <summary>
		/// Исходное хранилище с новыми данными
		/// </summary>
		[Inject(Name = "source.metastorage")]
		public IMetaFileRegistry SourceStorage { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string IncludeRegex { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string ExcludeRegex { get; set; }
		/// <summary>
		/// Выполняет синхронизацию через БД
		/// </summary>
		public void Synchronize(){
			Prepare();
			var merger = new MetaFileRegistryMerger{CustomMerger = this, ExcludeRegex = ExcludeRegex,IncludeRegex = IncludeRegex};
			merger.Merge(TargetStorage,SourceStorage,MergeFlags.Snapshot);
		}

		private void Prepare(){
			if (null == MetaFileProcessor) throw new Exception("MetaFileProcessor not set");
			if (null == TargetStorage) throw new Exception("TargetStorage not set");
			if (null == SourceStorage) throw new Exception("LocalStorage not set");
			
			TargetStorage.Refresh();
			SourceStorage.Refresh();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public string GetUpdateScript()
		{
			Prepare();
			var merger = new MetaFileRegistryMerger{CustomMerger = this};
			var delta = merger.GetDelta(TargetStorage, SourceStorage);

			IList<DatabaseUpdateRecord> allrequiredchanges = new List<DatabaseUpdateRecord>();
			BuildChanges(delta, allrequiredchanges);
			var goodlist = new LinkedList<DatabaseUpdateRecord>();
			var badlist = new LinkedList<DatabaseUpdateRecord>();
			MetaFileProcessor.Prepare(SqlConnection, allrequiredchanges, goodlist, badlist);
			if (badlist.Count != 0)
			{
				throw new InvalidDataSyncException(badlist.ToArray());
			}
			return MetaFileProcessor.GetSql(goodlist.ToArray());
		}

		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="basedelta"></param>
		/// <returns></returns>
		public IEnumerable<MetaFileRegistryDelta> Merge(IEnumerable<MetaFileRegistryDelta> basedelta){
			IList<DatabaseUpdateRecord> allrequiredchanges = new List<DatabaseUpdateRecord>();
			BuildChanges(basedelta, allrequiredchanges);
			var goodlist = new LinkedList<DatabaseUpdateRecord>();
			var badlist = new LinkedList<DatabaseUpdateRecord>();
			MetaFileProcessor.Prepare(SqlConnection,allrequiredchanges,goodlist,badlist);
			if (badlist.Count != 0){
				throw new InvalidDataSyncException(badlist.ToArray());
			}

			MetaFileProcessor.Execute(SqlConnection,goodlist.ToArray());

			var deltastoupdate = goodlist.Select(_ => _.FileDelta).Distinct().ToArray();

			return deltastoupdate;
		}	

		

		private void BuildChanges(IEnumerable<MetaFileRegistryDelta> basedelta, IList<DatabaseUpdateRecord> allrequiredchanges){
			foreach (var md in basedelta){
				var basexml = GetXml(md.Target, md.Code, md.TargetHistory);
				var srcxml = GetXml(md.Source, md.Code, md.SourceHistory);
				var changeids = srcxml.Attr("change-ids").ToBool();
				var tree = srcxml.Attr("tree").ToBool();
				var ignorenames = srcxml.Attr("ignore-names").ToBool();
				var table = srcxml.Attr("table");
				var schema = srcxml.Attr("schema");
				var options = new XDiffOptions{
					ChangeIds = changeids,
					SrcXml = srcxml,
					IsHierarchy = tree,
					IsNameIndepended = ignorenames
				};
				foreach (var a in srcxml.Attributes().Where(_=>_.Name.LocalName.StartsWith("ref-"))){
					options.RefMaps = options.RefMaps ?? new Dictionary<string, string>();
					options.RefMaps[a.Name.LocalName] = a.Value;
				}
				var differ =
					new XDiffGenerator(options);
				var delta = differ.GetDiff(basexml, srcxml);
				foreach (var item in delta){
					var record = new DatabaseUpdateRecord();
					record.FileDelta = md;
					record.DiffItem = item;
					record.Schema = schema;
					if (string.IsNullOrWhiteSpace(table)){
						record.TargetTable = (item.BasisElement ?? item.NewestElement).Name.LocalName;
						if (record.TargetTable.Contains(".")){
							record.Schema = record.TargetTable.Split('.')[0];
							record.TargetTable = record.TargetTable.Split('.')[1];
						}
						if (ignorenames && item.Action == XDiffAction.RenameElement){
							record.TargetTable = item.NewValue;
						}
					}
					else{
						record.TargetTable = table;
					}
					record.TargetId = (item.BasisElement ?? item.NewestElement).Attr("id").ToInt();
					record.TargetCode = (item.BasisElement ?? item.NewestElement).Attr("code");
					allrequiredchanges.Add(record);
				}
			}
		}

		private XElement GetXml(IMetaFileRegistry repo,string code, RevisionDescriptor[] history){
			if(null==history||0==history.Length)return new XElement("stub");
			var last = history.OrderByDescending(_ => _.RevisionTime).First();
			var content = repo.GetByRevision(code, last.Revision).Content;
			if (content.StartsWith("<")) return XElement.Parse(content);
			var result = new BxlParser().Parse(content);
			if (result.Elements().Count() == 1 && result.Elements().First().Name.LocalName == "data"){
				result = result.Elements().First();
			}
			return result;
		}
	}
}
