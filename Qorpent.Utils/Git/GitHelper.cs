using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.Git{

	

	/// <summary>
	/// 
	/// </summary>
	public class GitHelper
	{
		private bool _connected;

		/// <summary>
		/// Режим отладки
		/// </summary>
		public bool DebugMode { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public GitHelper(){
			RemoteName = "origin";
			Branch = "master";
		}
		/// <summary>
		/// Директория для расположения файлов
		/// </summary>
		public string DirectoryName { get; set; }
		/// <summary>
		/// Внешний репозиторий в формате name[~url] если укзан URL, то переинициплизирует директорию
		/// </summary>
		public string RemoteName { get; set; }
		/// <summary>
		/// Уделенный URL
		/// </summary>
		public string RemoteUrl { get; set; }
		/// <summary>
		/// Требуемый бранч
		/// </summary>
		public string Branch { get; set; }
		/// <summary>
		/// Бранч от которого следует откалываться по умолчанию
		/// </summary>
		public string BaseBranch { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string AuthorName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Password { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string AuthorEmail { get; set; }

		/// <summary>
		/// 
		/// </summary>
		HashSet<string> MergeCache = new HashSet<string>();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="usecache"></param>
		/// <returns></returns>
		public bool IsMerged(string from = null, string to = null, bool usecache = false){
			if (string.IsNullOrWhiteSpace(from)) from = "HEAD";
			if (string.IsNullOrWhiteSpace(to)) to = "origin/" + Branch;
			var key = from + ":" + to;
			if (usecache){
				if (MergeCache.Contains(key)) return true;
			}
			var dist = GetDistance(from, to);
			if (dist.IsForwardable||dist.IsZero){
				if (usecache){
					MergeCache.Add(key);
				}
				return true;
			}
			return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public GitCommitInfo[] GetTags(){
			var data = ExecuteCommand("show-ref", "--tags",allowinvstate:true);
			var lines = data.SmartSplit(false, true, '\r', '\n');
			return lines.Select(_ =>{
				var pair = _.SmartSplit(false, true, ' ');
				var hash = pair[0];
				var name = pair[1].Substring(10);
				var commit = new GitCommitInfo{Hash = hash, ShortHash = hash.Substring(0, 7), Name = name};
				return commit;
			}).ToArray();
		}

		/// <summary>
		/// 
		/// </summary>
		public string GetAuthorName(){
			var a = AuthorName??"";
			if (a.Contains("\\")){
				a = a.Split('\\')[1];
			}
			if (string.IsNullOrWhiteSpace(a) && string.IsNullOrWhiteSpace(AuthorEmail)) return null;
			if (!string.IsNullOrWhiteSpace(a) && !string.IsNullOrWhiteSpace(AuthorEmail)){
				return string.Format("{0} <{1}>", a, AuthorEmail);
			}
			return a??"" + AuthorEmail??"";
		}
		/// <summary>
		/// 
		/// </summary>
		public bool NoAutoPush { get; set; }
		/// <summary>
		/// Инициализирует и настраиват директорию
		/// </summary>
		public GitHelper Connect(){

			if (_connected) return this;

			Directory.CreateDirectory(DirectoryName);
			var gitpath = Path.Combine(DirectoryName, ".git");
			if (!Directory.Exists(gitpath)){
				InitializeRepository();
			}
			else{
				if (!string.IsNullOrWhiteSpace(RemoteUrl)){
					if (RemoteUrl.Contains("https")){
						ExecuteCommand("config", "--global http.sslVerify false");
					}
					RemoteSet(RemoteName, RemoteUrl);
					if (!IsWaitMergeCommit() && 0 == GetChangedFilesList().Length && IsRemoteAccessible()){
						EnsureBranch();
					}
				}
			}
			if (!string.IsNullOrWhiteSpace(RemoteUrl)){
				if (!IsWaitMergeCommit() && IsRemoteAccessible()){
					Fetch();
				}
			}
			_connected = true;
			return this;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="command"></param>
		/// <param name="parameters"></param>
		public void Tortoise(string command, object parameters = null){
			var args = "/command:" + command;
			if (null != parameters){
				var dict = parameters.ToDict();
				args += " " + string.Join( " ",dict.Select(_ => "/" + _.Key + ":\"" + _.Value + "\""));
			}
			var startInfo = new ProcessStartInfo
			{
				FileName = "tortoisegitproc",
				Arguments = args,
				UseShellExecute = false,
				WorkingDirectory = DirectoryName ?? Environment.CurrentDirectory,				
			};
			Process.Start(startInfo).EnsureForeground();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="command"></param>
		/// <param name="args"></param>"
		/// <param name="timeout"></param>
		/// <param name="allowinvstate"></param>
		/// <returns></returns>
		public string ExecuteCommand(string command, string args = "", int timeout = 0, bool allowinvstate = false){
			var result = ExecuteCommandDetailed(command, args, timeout);
			return PrepareResult(allowinvstate, result);
		}

		private static string PrepareResult(bool allowinvstate, ConsoleApplicationResult result){
			if (null != result.Exception) throw result.Exception;
			if (!allowinvstate && 0 != result.State){
				throw new Exception("Invalid State " + result.State + "\r\n" + result.Output + result.Error);
			}
			return (result.Output??"").Trim(new[]{'\r', '\n', ' '});
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="command"></param>
		/// <param name="args"></param>
		/// <param name="timeout"></param>
		/// <returns></returns>
		public ConsoleApplicationResult ExecuteCommandDetailed(string command, string args, int timeout){

			return PrepareCall(command, args, timeout).Run();
		}
		/// <summary>
		/// 
		/// </summary>
		public bool NoErrorOnNotConnected { get; set;}
		/// <summary>
		/// 
		/// </summary>
		public event Action OnNotConnected;

		private ConsoleApplicationHandler PrepareCall(string command, string args, int timeout){
			if ((command == "fetch" || command=="push"||command=="clone") && !string.IsNullOrWhiteSpace(RemoteUrl) && RemoteUrl.StartsWith("http")){
				if (!IsRemoteAccessible()){
					if(null!=OnNotConnected)OnNotConnected.Invoke();
					if(!NoErrorOnNotConnected)throw new Exception("Cannot perform "+command+" because remote url "+RemoteUrl+" is not accessible");
					return ConsoleApplicationHandler.Null;
				}
			}
			return new ConsoleApplicationHandler{
				ExePath = "git", 
				WorkingDirectory = DirectoryName ?? Environment.CurrentDirectory,
				Arguments = command + " " + args,
				NoWindow = true,
				Timeout = timeout
			};
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool IsRemoteAccessible(){
			if (string.IsNullOrWhiteSpace(RemoteUrl)){
				return true;
			}
			if (RemoteUrl.StartsWith("http")){
				var uri = new Uri(RemoteUrl);
				var host = uri.Host;
				var port = uri.Port;
				var cli = new System.Net.Sockets.TcpClient();
				try{
					var t = cli.ConnectAsync(host, port);
					return t.Wait(500);
				}
				catch{
					return false;
				}
			}
			return Directory.Exists(RemoteUrl);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="command"></param>
		/// <param name="args"></param>
		/// <param name="timeout"></param>
		/// <param name="allowinvstate"></param>
		/// <returns></returns>
		public async Task<string> ExecuteCommandAsync(string command, string args = "", int timeout = 0, bool allowinvstate = false)
		{
			var result = await ExecuteCommandDetailedAsync(command, args, timeout);
			return PrepareResult(allowinvstate, result);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="command"></param>
		/// <param name="args"></param>
		/// <param name="timeout"></param>
		/// <returns></returns>
		public async Task<ConsoleApplicationResult> ExecuteCommandDetailedAsync(string command, string args, int timeout){
			return await PrepareCall(command,args,timeout).RunAsync();
		}

		/// <summary>
		/// Восстанавливает единичный файл в рабочей директории
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public string ResetSingleFile(string file){
			try{
				ExecuteCommand("reset", " HEAD \"" + file + "\"");
				return ExecuteCommand("checkout", " -- \"" + file + "\"");
			}
			catch (Exception ex){
				if (ex.Message.Contains("did not match any file")){
					File.Delete(Path.Combine(DirectoryName,file));
					return "";
				}
				throw;
			}
		}

		/// <summary>
		/// Init command
		/// </summary>
		public string Init(){
			return ExecuteCommand("init");
		}

		/// <summary>
		/// Получить упрощенный список измененных файлов
		/// </summary>
		/// <param name="fromref"></param>
		/// <param name="toref"></param>
		/// <param name="level"></param>
		/// <returns></returns>
		public FileRecord[] GetChangedFilesList(string fromref = "HEAD", string toref="WORKING_TREE", string level = ""){
			var result =InternalGetFileChanges(fromref, toref);
			foreach (var fileRecord in result){
				var fullname = Path.Combine(DirectoryName, fileRecord.FileName);
				if (File.Exists(fullname)){
					fileRecord.LastWriteTime = File.GetLastWriteTime(fullname);
				}
			}
			if (!string.IsNullOrWhiteSpace(level)){
				foreach (var fileRecord in result){
					fileRecord.Level = level;
				}
			}
			return result;
		}

		private FileRecord[] InternalGetFileChanges(string fromref, string toref){
			if (string.IsNullOrWhiteSpace(toref)){
				toref = "WORKING_TREE";
			}
			if (string.IsNullOrWhiteSpace(fromref)){
				if (toref == "WORKING_TREE"){
					fromref = "HEAD";
				}
				else{
					var refs = ExecuteCommand("log", "-n2 --format=%h~ " + toref).SmartSplit(false, true, '~');
					if (refs.Count >= 2){
						fromref = refs[1];
					}
				}
			}

			if (toref == "WORKING_TREE"){
				var rawchanged = ExecuteCommand("status", "-s").SmartSplit(false, false, '\r', '\n');
				return rawchanged.Select(_ => (FileRecord) _).ToArray();
			}

			if (string.IsNullOrWhiteSpace(fromref)){
				var raw = ExecuteCommand("log", "-n1 --name-status --format=\"%h`\" " + toref);
				var files = raw.SmartSplit(false, true, '`').Last().SmartSplit(false, false, '\r', '\n');
				return files.Select(_ => (FileRecord) _).ToArray();
			}
			string[] rawchanges = null;
			if (fromref == toref){
				rawchanges =
					ExecuteCommand("log", "-n1 --name-status --format=\"`%h`\" " + toref).SmartSplit(false, true, '`').ToArray();
			}
			else{
				rawchanges =
					ExecuteCommand("log", " --name-status --format=\"`%h`\" " + fromref + ".." + toref)
						.SmartSplit(false, true, '`')
						.ToArray();
			}

			var result = new List<FileRecord>();
			for (var i = 1; i < rawchanges.Length; i += 2){
				var files = rawchanges[i].SmartSplit(false, false, '\r', '\n').Select(_ => (FileRecord) _).ToArray();
				foreach (var file in files){
					if (!result.Contains(file)){
						result.Add(file);
					}
				}
			}
			return result.ToArray();
		}


		/// <summary>
		/// Add or replace remote to url
		/// </summary>
		/// <param name="name"></param>
		/// <param name="url"></param>
		public string RemoteSet(string name="", string url=""){
			if (string.IsNullOrWhiteSpace(name)){
				name = RemoteName;
			}
			if (string.IsNullOrWhiteSpace(url)){
				url = RemoteUrl;
			}
			if (string.IsNullOrWhiteSpace(url)) return "false";
			var _u = url;
			if (!string.IsNullOrWhiteSpace(Password) && !_u.Contains("@")){
				_u = _u.Replace("://", "://" + AuthorName + ":" + Password + "@");
			}
			try{
				return ExecuteCommand("remote", "set-url " + name + " \"" + _u + "\"");
			}
			catch{
				try{
					ExecuteCommand("remote", "rm " + name);
				}
				catch{

				}
				return ExecuteCommand("remote", "add " + name + " \"" + _u + "\"");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="remoteName"></param>
		/// <param name="branch"></param>
		/// <param name="tags"></param>
		public string Fetch(string remoteName = "", string branch = "", bool tags=true){
			remoteName =string.IsNullOrWhiteSpace( remoteName) ? RemoteName :remoteName;
			branch = branch ?? "";
			if (tags){
				ExecuteCommand("fetch", "--tags " + remoteName);	
			}
			return ExecuteCommand("fetch", remoteName + " " + branch);
			
		}
		/// <summary>
		/// Добавление файлов к выборке
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public string Add(string path = null){
			var args = "";
			if (string.IsNullOrWhiteSpace(path)){
				args = "--a";
			}
			else{
				args = "\"" + path + "\"";
			}
			return ExecuteCommand("add", args);
		}




		/// <summary>
		/// Сформировать коммит
		/// </summary>
		/// <param name="message"></param>
		/// <param name="amend"></param>
		/// <param name="autoemail"></param>
		/// <returns></returns>
		public string Commit(string message="", bool amend=false, bool autoemail = true){
			var args = "";
			if (amend){
				args += "--amend ";
			}
			if (string.IsNullOrWhiteSpace(message)){
				message = "no comment";
			}
			args += "-m \"" + message + "\"";
			var author = GetAuthorName();
			if (null != author){
				if (amend){
					args += " --reset-author ";
					
				}
				args += " --author \"" + author + "\"";
			}
			string result = "";
			try{
				result = ExecuteCommand("commit", args);
				
			}
			catch(Exception ex) {
				if (ex.Message.Contains("fatal: No existing author")){
					if (autoemail){
						args = args.Substring(0, args.Length - 1);
						args += " <" + GetAuthorName() + "@auto." + Environment.MachineName + ".com>\"";
						result = ExecuteCommand("commit", args);
					}
					else{
						throw new Exception("invalid author");
					}
				}
			}

			return result;
		}
		/// <summary>
		///
		/// </summary>
		/// <param name="message"></param>
		/// <param name="amend"></param>
		public string  CommitAllChanges(string message = "", bool amend = false){
			Add();
			Commit(message, amend);
			return GetCommitId();
		}
		/// <summary>
		/// Записать отдельный файл с созданием версии
		/// </summary>
		/// <param name="path"></param>
		/// <param name="content"></param>
		/// <param name="message"></param>
		/// <param name="amend"></param>
		public string WriteAndCommit(string path, string content, string message = "", bool amend = false){
			WriteFile(path,content);
			Add(path);
			Commit(message, amend);
			return GetCommitId();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hard"></param>
		/// <returns></returns>
		public string Reset(bool hard = false){
			var args = "";
			if (hard) args += " --hard";
			return ExecuteCommand("reset", args);
		}

		/// <summary>
		/// Специальная команда показа контента файла
		/// </summary>
		/// <param name="path"></param>
		/// <param name="commit"></param>
		/// <param name="returnNullOnNotExisted"></param>
		/// <returns></returns>
		public string GetContent(string path, string commit=null, bool returnNullOnNotExisted = true){
			try{
				if (string.IsNullOrWhiteSpace(commit)){
					commit = "HEAD";
				}
				var args = commit + ":" + path;
				var result = ExecuteCommand("show", args);
				if (result.Contains("fatal: Path '" + path)){
					return null;
				}
				return result;
			}
			catch (Exception ex){
				if (ex.Message.Contains("does not exist")){
					if (returnNullOnNotExisted) return null;
				}
				throw;
			}
		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rev"></param>
        /// <returns></returns>
        public string GetParentCommit(string rev) {
            return ExecuteCommand("log", "-2 " + rev + " --format=%h").Split('\r', '\n').Last().Trim();
        }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <param name="since"></param>
		/// <param name="all"></param>
		/// <param name="nomerges"></param>
		/// <returns></returns>
		public GitCommitInfo[] GetHistory(string path="",string since = null , bool all = false, bool nomerges = false){
			if (!string.IsNullOrWhiteSpace(path)){
				path = "\"" + path + "\"";
			}
			var args = "--format=\"%H|%ct|%cN|%cE|%aN|%aE|%s`\" " + path;
			if (!string.IsNullOrWhiteSpace(since)){
				args = "--since \"" + since + "\" " + args;
			}

			if (all){
				args = " --all " + args;
			}
			if (nomerges){
				args = " --no-merges " + args;
			}

			var result = ExecuteCommand("log", args);
			var hists = result.SmartSplit(false, true, '`');
			return hists.Select(GitUtils.ParseGitCommitInfo).ToArray();
		}
		/// <summary>
		/// Проверяет состояние незавершенного мержа
		/// </summary>
		/// <returns></returns>
		public bool IsWaitMergeCommit(){
			var raw = ExecuteCommand("status");
			return raw.Contains("fixed but you are still merging");
		}

		/// <summary>
		/// Производит Checkout на бранч
		/// </summary>
		/// <param name="branch"></param>
		/// <param name="createIfNotExists"></param>
		public string Checkout(string branch, bool createIfNotExists = true){
			try
			{
				return ExecuteCommand("checkout", " " + branch);
			}
			catch(Exception e){
				if (e.Message.Contains("did not match")){
					if (createIfNotExists){
						return ExecuteCommand("checkout", "-b " + branch);
					}
				}
				if (e.Message.Contains("need to resolve")){
					throw new Exception("has active conflicted merge");
				}
				throw;
			}
		}
		/// <summary>
		/// Выполняет команду Merge
		/// </summary>
		/// <param name="firstPart"></param>
		/// <param name="secondPart"></param>
		/// <param name="options"></param>
		public string Merge(string firstPart, string secondPart = "", MergeStrategyOption options = MergeStrategyOption.None){
			var args = firstPart;
			if (!string.IsNullOrWhiteSpace(secondPart)){
				args+="/"+secondPart;
			}
			if (options != MergeStrategyOption.None){
				args = "-X" + options.ToString().ToLower() + " " + args;
			}
			return ExecuteCommand("merge", args);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public string GetFullPath(string code){
			return Path.GetFullPath(Path.Combine(DirectoryName, code));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fromcommit"></param>
		/// <param name="tocommit"></param>
		/// <param name="nullOnUnknown"></param>
		/// <returns></returns>
		public RevisionDistance GetDistance(string fromcommit="", string tocommit="", bool nullOnUnknown = true){
			try{
				if (string.IsNullOrWhiteSpace(fromcommit)){
					fromcommit = Branch;
				}
				if (string.IsNullOrWhiteSpace(tocommit)){
					tocommit = RemoteName + "/" + Branch;
				}
				var result = new RevisionDistance();
				var ft = Task.Run(() =>GetCommitList(fromcommit, tocommit));
				var bt = Task.Run(() =>GetCommitList(tocommit,fromcommit));
				Task.WaitAll(ft, bt);
				result.Forward = ft.Result.Length;
				result.Behind = bt.Result.Length;
				return result;
			}
			catch (Exception ex){
				if (ex.Message.Contains("unknown revision")){
					if (nullOnUnknown) return null;
				}
				throw;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="fromcommit"></param>
		/// <param name="tocommit"></param>
		/// <returns></returns>
		public string[] GetCommitList(string fromcommit, string tocommit){
			return ExecuteCommand("rev-list", fromcommit + ".." + tocommit).SmartSplit(false, true, '\r', '\n').ToArray();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="refer"></param>
		public void SetTag(string tag, string refer=null){
			if (string.IsNullOrWhiteSpace(refer)) refer = "HEAD";
			try{
				ExecuteCommand("tag", "-d " + tag);
			}
			catch{
				
			}
			finally {
				ExecuteCommand("tag", "-f " +tag+" "+refer);
			}
		}

		

		/// <summary>
		/// 
		/// </summary>
		/// <param name="code"></param>
		/// <param name="content"></param>
		public void WriteFile(string code, string content){
			File.WriteAllText(GetFullPath(code),content);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public string ReadFile(string code){
			return File.ReadAllText(GetFullPath(code));
		}
		/// <summary>
		/// 
		/// </summary>
		public void InitializeRepository(bool throwErrroOnBranchFix = false)
		{
			Init();
			if (!string.IsNullOrWhiteSpace(RemoteUrl)){
				RemoteSet(RemoteName, RemoteUrl);
				try{
					FixBranchState();
				}
				catch{
					if (throwErrroOnBranchFix) throw;
					Checkout(Branch);
				}


			}
			else{
				if (!File.Exists(Path.Combine(DirectoryName, ".gitignore"))){
					WriteFile(".gitignore", "*.tmp\r\n*~.*");
					Add();
					Commit("init");

					if (!string.IsNullOrWhiteSpace(RemoteUrl)){
						Push();
					}
				}

			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public string Push(bool tags = false, bool force = false, string remote=null){
			if (string.IsNullOrWhiteSpace(remote)) remote = RemoteName;
			var args = "";
			if (tags){
				args += " --tags";
			}
			if (force){
				args += " --force";
			}
			args += " " + remote + " " + Branch;
			return ExecuteCommand("push",args);
		}

		/// <summary>
		/// Переключается и синхронихзируется с удаленным бранчем
		/// </summary>
		public void FixBranchState(bool resetOnMergeProblems = true){
			if (string.IsNullOrWhiteSpace(RemoteUrl)) return;
			if (!string.IsNullOrWhiteSpace(RemoteName)){
				Fetch(RemoteName);
			}
			EnsureBranch();
			if (!string.IsNullOrWhiteSpace(RemoteName)){
				try{
					Merge(RemoteName, Branch);
				}
				catch{
					if (resetOnMergeProblems){
						Reset(true);
						Merge(RemoteName, Branch,MergeStrategyOption.Theirs);
					}
				}
			}
			
			Add();
			Commit("after merge");
			Push();
		}

		private void EnsureBranch(){
			var basebranch = BaseBranch;
			if (string.IsNullOrWhiteSpace(basebranch)){
				basebranch = "master";
			}
			if (null == GetCommitInfo(Branch)){
				Checkout(basebranch);
				if (!NoAutoPush){
					if (null == GetCommitInfo("origin/" + basebranch)){

						ExecuteCommand("push", RemoteName + " " + basebranch);
					}
				}
			}
			Checkout(Branch);
			if (!NoAutoPush){
				if (null == GetCommitInfo("origin/" + Branch)){
					Push();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="refcode"></param>
		public string GetCommitId(string refcode=""){
			if (string.IsNullOrWhiteSpace(refcode)){
				refcode = "HEAD";
			}
			var commit = ResolveRef(refcode);
			if (string.IsNullOrWhiteSpace(commit)) {
				throw new Exception("Cannot get commti code");
			}
			return commit;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="refcode"></param>
		/// <returns></returns>
		public string ResolveRef(string refcode ){
			return (ExecuteCommand("rev-parse", refcode) ?? "").Trim(new[]{'\r','\n',' '});

		}

		

		/// <summary>
		/// 
		/// </summary>
		/// <param name="refcode"></param>
		/// <returns></returns>
		public string[] GetFileList(string refcode = ""){
			if (string.IsNullOrWhiteSpace(refcode)){
				refcode = "HEAD";
			}
			var list = ExecuteCommand("ls-tree", "--name-only --full-name  -r " + refcode);
			return list.SmartSplit(false, true, '\r', '\n').Select(GitUtils.ConvertToValidFileName).ToArray();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <param name="commit"></param>
		public void RestoreSingleFile(string path, string commit){
			var current = GetCommitId("HEAD");
			var commitinfo = GetCommitInfo(commit);
			if (current != commitinfo.Hash){
				var currentContent = GetContent(path);
				var commitContent = GetContent(path,commit);
				if (commitContent != currentContent){
					var realpath = Path.Combine(DirectoryName, path);
					File.WriteAllText(realpath,commitContent);
					Add(path);
					Commit("restore " + commit.Substring(0,7) + " for " + path);
				}
				File.SetLastWriteTime(GetFullPath(path),commitinfo.LocalRevisionTime);
			}
		}

		/// <summary>
		/// Возвращает информацию о коммите
		/// </summary>
		/// <param name="refcode"></param>
		/// <param name="returnNullIfNotExisted"></param>
		/// <returns></returns>
		public GitCommitInfo GetCommitInfo(string refcode=null, bool returnNullIfNotExisted = true){
			try{
				if (string.IsNullOrWhiteSpace(refcode)){
					refcode = "HEAD";
				}
				var data = ExecuteCommand("show", "--format=\"%H|%ct|%cN|%cE|%aN|%aE|%s\"  --quiet \"" + refcode + "\"");
				return GitUtils.ParseGitCommitInfo(data);
			}
			catch(Exception ex){
				if (ex.Message.Contains("unknown")){
					if (returnNullIfNotExisted) return null;
				}
				throw;
			}

		}
		/// <summary>
		/// 
		/// </summary>
		public void DropAllTags(){
			var tags = GetTags();
			if (0 != tags.Length){
				ExecuteCommand("tag", "-d " + string.Join(" ", tags.Select(_ => _.Name)));
			}
		}
	}
}
