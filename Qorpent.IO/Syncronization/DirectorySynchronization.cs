using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Qorpent.Log;

namespace Qorpent.IO.Syncronization
{


	/// <summary>
	/// 
	/// </summary>
	public class DirectorySynchronization
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sourceDir"></param>
		/// <param name="targetDir"></param>
		public DirectorySynchronization(string sourceDir, string targetDir){
			this.Source = sourceDir;
			this.Target = targetDir;
			Log = StubUserLog.Default;
		}
		/// <summary>
		/// 
		/// </summary>
		public string Target { get; private set; }
		/// <summary>
		/// 
		/// </summary>
		public string Source { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public IUserLog Log { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public void Synchronize(){
			Directory.CreateDirectory(Source);
			Directory.CreateDirectory(Target);
			var src = new DirectoryStructure(Source);
			var trg = new DirectoryStructure(Target);
			bool waserror = false;
			foreach (var fileItem in src.GetDiff(trg)){
				try{
					Log.Trace("start "+fileItem);
					Apply(fileItem);
					Log.Info("complete "+fileItem);
				}
				catch (Exception ex){
					Log.Error("error "+fileItem,ex);
					waserror = true;
				}
			}
			if (!waserror){
				var hash = src.GetStampFile().ToString();
				File.WriteAllText(Path.Combine(Target, DirectoryStructure.SyncFileName), hash);
				File.WriteAllText(Path.Combine(Source, DirectoryStructure.SyncFileName), hash);
				Log.Info("stamp file wrote");
			}
			else{
				Log.Error("stamp not written due to some exceptions");
			}
		}

		private void Apply(FileItem item){
			switch (item.Operation){
				case FileOperation.Create:
					goto case FileOperation.Update;
				case FileOperation.Update:
					UpdateFile(item.Name);
					break;
				case FileOperation.Delete:
					DeleteFile(item.Name);
					break;
			}
		}

		private const int TRYS = 5;

		private void DeleteFile(string name){
			name = Path.Combine(Target, name);
			if (File.Exists(name)){
				Execute(() => File.Delete(name));
			}
		}

		private static void Execute(Action exec){
			Exception e = null;
			for (var i = 0; i < TRYS; i++){
				try{
					exec();
					break;
				}
				catch (Exception ex){
					e = ex;
					Thread.Sleep(20);
				}
			}
			if (null != e){
				throw e;
			}
		}

		private void UpdateFile(string name){
			var src = Path.Combine(Source, name);
			var trg = Path.Combine(Target, name);
			Execute(()=>File.Copy(src,trg,true));
		}


		/// <summary>
		/// Получить разницу между исходной и целевой директорией
		/// </summary>
		/// <returns></returns>
		public IEnumerable<FileItem> GetDifference(){
			var src = new DirectoryStructure(Source);
			var trg = new DirectoryStructure(Target);
			return src.GetDiff(trg);
		} 
	}
}
