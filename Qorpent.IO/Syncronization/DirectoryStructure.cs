using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO.Syncronization{
	/// <summary>
	/// 
	/// </summary>
	internal class DirectoryStructure{
		internal const string SyncFileName = ".qpt.sync";
		/// <summary>
		/// 
		/// </summary>
		/// <param name="directory"></param>
		public DirectoryStructure(string directory){
			this.Folder = directory;
			
		}

		protected string Folder { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public XElement GetStampFile(){
			Directory.CreateDirectory(Folder);
			var infos = Directory.GetFiles(Folder).OrderBy(_ => _).Select(_ => new FileInfo(_)).Where(_=>_.Name!=SyncFileName).ToArray();
			var lastver = infos.AsParallel().Max(_ => _.LastWriteTime);
			var dirver = Directory.GetLastWriteTime(Folder);
			if (dirver > lastver) lastver = dirver;
			var syncfile = Path.Combine(Folder, SyncFileName);
			XElement result = null;
			if (!File.Exists(syncfile) || File.GetLastWriteTime(syncfile) < lastver){
				
				if (!File.Exists(syncfile)){
					result = GetFullStampFile(infos);
				}
				else{
					result = GetUpdatedFile(XElement.Load(syncfile), infos,File.GetLastWriteTime(syncfile));
				}
				File.WriteAllText(syncfile,result.ToString());
				return result;
			}
			var exresult = XElement.Load(syncfile);
			if (!exresult.Elements().All(_ => infos.Any(__ => __.Name == _.Attr("n")))){
				result = GetUpdatedFile(XElement.Load(syncfile), infos, File.GetLastWriteTime(syncfile));
				File.WriteAllText(syncfile, result.ToString());
				return result;
			}
			return XElement.Load(syncfile);
		}

		private XElement GetUpdatedFile(XElement current, FileInfo[] infos,DateTime timeToCheck){
			var cache = new List<FileInfo>(infos);
			foreach (var e in current.Elements().ToArray()){
				var n = e.Attr("n");
				var ex = cache.FirstOrDefault(_ => _.Name == n);
				cache.Remove(ex);
				if (null == ex){
					e.Remove();
				}
				else if(ex.LastWriteTime> timeToCheck){
					var data = File.ReadAllBytes(Path.Combine(ex.DirectoryName, ex.Name));
					var hash = CoreExtensions.Md5BasedDigitHash(data);
					e.SetAttributeValue("h",hash);
				}

			}
			foreach (var c in cache){
				var data = File.ReadAllBytes(Path.Combine(c.DirectoryName, c.Name));
				var hash = CoreExtensions.Md5BasedDigitHash(data);
				current.Add(new XElement("f", new XAttribute("n", c.Name), new XAttribute("h", hash)));
			}
			var result = new XElement("sync");
			result.Add(current.Elements().OrderBy(_=>_.Attr("name")));
			return result;
		}

		private static XElement GetFullStampFile(FileInfo[] infos){
			var result = new XElement("sync");
			foreach (var c in infos){
				var data = File.ReadAllBytes(Path.Combine(c.DirectoryName, c.Name));
				var hash = CoreExtensions.Md5BasedDigitHash(data);
				result.Add(new XElement("f", new XAttribute("n", c.Name), new XAttribute("h", hash)));
			}
			return result;
		}

		/// <summary>
		/// Получить разницу между директориями
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public IEnumerable<FileItem> GetDiff(DirectoryStructure other){
			var src = this.GetStampFile();
			var trg = other.GetStampFile();
			if (src.ToString() == trg.ToString()){
				yield break;
			}
			var srce = src.Elements().Select(_=>new FileItem(_,FileOperation.None)).ToArray();
			var trge = trg.Elements().Select(_ => new FileItem(_, FileOperation.None)).ToArray();
			var leftOuterJoin = from s in srce
			                    join t in trge
				                    on s.Name equals t.Name
				                    into temp
			                    from t in temp.DefaultIfEmpty(new FileItem())
			                    select new FileItem{Hash = s.Hash, Name = s.Name, OtherHash = t.Hash};
			var rightOuterJoin = from t in trge
								 join s in srce
									 on t.Name equals s.Name
									 into temp
								 from s in temp.DefaultIfEmpty(new FileItem())
								 select new FileItem { Hash = s.Hash, Name = t.Name, OtherHash = t.Hash };
			var fullOuterJoin = leftOuterJoin.Union(rightOuterJoin.Where(_=>string.IsNullOrWhiteSpace(_.Hash))).ToArray();
			foreach (FileItem i in fullOuterJoin){
				if (i.Hash == i.OtherHash) continue;
				if (string.IsNullOrWhiteSpace(i.Hash)){
					i.Operation = FileOperation.Delete;
					yield return i;
				}else if (string.IsNullOrWhiteSpace(i.OtherHash)){
					i.Operation = FileOperation.Create;
					yield return i;
				}
				else{
					i.Operation = FileOperation.Update;
					yield return i;
				}
			}

		}

	}
}