using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.Log;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.Git;

namespace Qorpent.Data.DataDiff
{
	/// <summary>
	/// Основанный на GIT+BSHARP класс для подготовки пар сравниваемых файлов
	/// </summary>
	public class DiffPairGenerator
	{
		private readonly TableDiffGeneratorContext _context	;
		private GitHelper _githelper;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		public DiffPairGenerator(TableDiffGeneratorContext context){
			this._context = context;
			if (null == _context.Log){
				_context.Log = ConsoleLogWriter.CreateLog("main", customFormat: "${Message}", level: LogLevel.Trace);
			}
			if (string.IsNullOrWhiteSpace(_context.ProjectDirectory)){
				throw new Exception("ProjectDirectory is required");
			}
			if (string.IsNullOrWhiteSpace(_context.GitBranch)){
				_context.GitBranch = "master";
			}

			if (string.IsNullOrWhiteSpace(_context.BSharpPrototype)){
				_context.BSharpPrototype = "db-meta";
			}

			if (string.IsNullOrWhiteSpace(_context.RootDirectory)){
				_context.RootDirectory = Path.GetTempFileName();
			}
			if (string.IsNullOrWhiteSpace(_context.GitUrl)){
				if (!Directory.Exists(Path.Combine(_context.RootDirectory, ".git"))){
					throw new Exception("no valid repo directory, nor git URL was given");
				}
			}
			if (string.IsNullOrWhiteSpace(_context.GitUpdateRevision)){
				_context.GitUpdateRevision = "HEAD";
			}
		}
		/// <summary>
		/// Компилирует исходую и целевую версию проекта, сравнивает классы и формирует пары для сравнений
		/// </summary>
		public void Generate(){
			Directory.CreateDirectory(_context.RootDirectory);
			_context.Log.Trace("start initialize git");
			PrepareGitRepository();
			_context.Log.Info("git initialized");
			_context.Log.Trace("start base proj reading");
			IDictionary<string, XElement> sourceClasses = GetBSharpClasses(_context.GitBaseRevision);
			_context.Log.Trace("end base proj reading");
			_context.Log.Trace("start update proj reading");
			IDictionary<string, XElement> updatedClasses = GetBSharpClasses(_context.GitUpdateRevision);
			_context.Log.Trace("end base proj reading");
			IDictionary<string,DiffPair> result = new Dictionary<string, DiffPair>();
			foreach (var updatedClass in updatedClasses){
				var name = updatedClass.Key;
				var diff = new DiffPair{Updated = updatedClass.Value,Base = new XElement("stub"), FileName = name};
				if (sourceClasses.ContainsKey(name)){
					diff.Base = sourceClasses[name];
				}
				if(diff.Base.ToString()==diff.Updated.ToString())continue;
				result[name] = diff;
			}
			_context.DiffPairs = result.Values.ToArray();
		}

		private IDictionary<string, XElement> GetBSharpClasses(string rev){

			var result = new Dictionary<string, XElement>();
			if (rev == "HEAD"){
				rev = _context.GitBranch;
			}
			if (!string.IsNullOrWhiteSpace(rev)){
				_context.Log.Trace("begin checkout "+rev);
				_githelper.Checkout(rev);
				_context.ResolvedUpdateRevision = _githelper.GetCommitId();
				_context.Log.Trace("end checkout " + rev);
				var bscStarter = new ConsoleApplicationHandler();
				bscStarter.ExePath = "bsc";
				bscStarter.WorkingDirectory = Path.Combine(_context.RootDirectory, _context.ProjectDirectory);
				if (!string.IsNullOrWhiteSpace(_context.ProjectName)){
					bscStarter.Arguments = _context.ProjectName;
				}
				_context.Log.Trace("start bsc");
				var bscResult = bscStarter.RunSync();
				_context.Log.Trace("finish bsc");
				if (bscResult.IsOK){
					_context.Log.Trace("ok bsc");
					var clsProvider = new BSharp.Runtime.BSharpFileBasedClassProvider{
						RootDirectory = Path.Combine(_context.RootDirectory, _context.OutputDirectory)
					};
					var dbclasses = clsProvider.FindClasses(prototype: _context.BSharpPrototype).ToArray();
					foreach (var cls in dbclasses){
						_context.Log.Trace("cls "+cls.Fullname+" detected");
						var name = cls.Name;
						var xml = cls.GetClassElement();
						result[name] = xml;
					}
					var maps = clsProvider.FindClasses(prototype: _context.BSharpMapPrototype).ToArray();
					foreach (var map in maps){
						foreach (var r in map.Definition.Descendants("ref")){
							var fromtable = r.ChooseAttr("table","code");
							var fromfield = r.ChooseAttr("name","code");
							var totable = r.Value;
							if (string.IsNullOrWhiteSpace(totable)){
								var cls = dbclasses.FirstOrDefault(_ => _.Fullname.EndsWith("." + fromfield));
								if (null != cls){
									totable = cls.Definition.Elements().First().Attr("table");
								}
							}
							_context.Mappings.Add(new TableMap(fromtable, fromfield, totable));
						}
					}
				}
				else{
					_context.Log.Error("error in bsc "+bscResult.Error,bscResult.Exception);
					throw new Exception("bsharp compile exception "+bscResult.Error,bscResult.Exception);
				}
			}
			return result;
		}

		private void PrepareGitRepository(){
			this._githelper = new GitHelper{RemoteUrl = _context.GitUrl, DirectoryName = _context.RootDirectory, Branch = _context.GitBranch,NoAutoPush = true}.Connect();
			try{
				_githelper.FixBranchState();
			}
			catch{
				
			}

		}
	}
}
