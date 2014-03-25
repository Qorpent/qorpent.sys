using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils{
	/// <summary>
	/// 
	/// </summary>
	public class GitHelper
	{
		/// <summary>
		/// ����� �������
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
		/// ���������� ��� ������������ ������
		/// </summary>
		public string DirectoryName { get; set; }
		/// <summary>
		/// ������� ����������� � ������� name[~url] ���� ����� URL, �� ������������������ ����������
		/// </summary>
		public string RemoteName { get; set; }
		/// <summary>
		/// ��������� URL
		/// </summary>
		public string RemoteUrl { get; set; }
		/// <summary>
		/// ��������� �����
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
		/// �������������� � ���������� ����������
		/// </summary>
		public GitHelper Connect()
		{
			Directory.CreateDirectory(DirectoryName);
			var gitpath = Path.Combine(DirectoryName, "\\.git");
			if (!Directory.Exists(gitpath))
			{
				InitializeRepository();
			}
			return this;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="command"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public string ExecuteCommand(string command, string args = "")
		{
			var startInfo = new ProcessStartInfo
			{
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				FileName = "git",
				Arguments = command+" "+ args,
				UseShellExecute = false,
				WorkingDirectory = DirectoryName,
				CreateNoWindow = true,
				
			};
			if (DebugMode){
				Console.WriteLine(command + " " + args);
			}

			Process process = null;
			try
			{
				process = Process.Start(startInfo);
				process.WaitForExit(10000);
			}
			catch (Exception ex)
			{
				var message = "";
				if (null != process)
				{
					message += process.StandardOutput.ReadToEnd();
					message += "\r\n--------------------------------\r\n";
					message += process.StandardError.ReadToEnd();
				}
				Console.WriteLine(message);
				throw new Exception(message, ex);
			}
			var result = process.StandardOutput.ReadToEnd();
			var error = process.StandardError.ReadToEnd();

			var msg = result;
			if(!string.IsNullOrWhiteSpace(error))msg+="\r\n--------------------------------\r\n" + error;
			if (DebugMode){
				Console.WriteLine(msg);
			}
			return msg.Trim();

		}
		/// <summary>
		/// Init command
		/// </summary>
		public string Init(){
			return ExecuteCommand("init");
		}
		/// <summary>
		/// Add or replace remote to url
		/// </summary>
		/// <param name="name"></param>
		/// <param name="url"></param>
		public string RemoteSet(string name, string url){
			try{
				ExecuteCommand("remote", "rm " + name);
			}
			catch{
				
			}
			return ExecuteCommand("remote", "add " + name + " \"" + url + "\"");
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="remoteName"></param>
		/// <param name="branch"></param>
		public string Fetch(string remoteName = "", string branch = ""){
			remoteName = remoteName ?? "";
			branch = branch ?? "";
			return ExecuteCommand("fetch", remoteName + " " + branch);
		}
		/// <summary>
		/// ���������� ������ � �������
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
		/// ������������ ������
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
	

			var result = ExecuteCommand("commit", args);
			if (result.Contains("fatal: No existing author")){
				if (autoemail){
					args = args.Substring(0, args.Length - 1);
					args += " <" + GetAuthorName() + "@auto." + Environment.MachineName + ".com>\"";
					result = ExecuteCommand("commit", args);
				}
				else{
					throw new Exception("invalid author");
				}
			}

			return result;
		}
		/// <summary>
		///
		/// </summary>
		/// <param name="message"></param>
		/// <param name="amend"></param>
		public void CommitAllChanges(string message = "", bool amend = false){
			Add();
			Commit(message, amend);
		}
		/// <summary>
		/// �������� ��������� ���� � ��������� ������
		/// </summary>
		/// <param name="path"></param>
		/// <param name="content"></param>
		/// <param name="message"></param>
		/// <param name="amend"></param>
		public void WriteAndCommit(string path, string content, string message = "", bool amend = false){
			WriteFile(path,content);
			Add(path);
			Commit(message, amend);
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
		/// ����������� ������� ������ �������� �����
		/// </summary>
		/// <param name="path"></param>
		/// <param name="commit"></param>
		/// <returns></returns>
		public string GetContent(string path, string commit=null){
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public GitCommitInfo[] GetHistory(string path){
			if (!string.IsNullOrWhiteSpace(path)){
				path = "\"" + path + "\"";
			}
			var result = ExecuteCommand("log", "--format=\"%H|%ct|%cN|%cE|%aN|%aE|%s`\" " + path);
			var hists = result.SmartSplit(false, true, '`');
			return hists.Select(ParseGitCommitInfo).ToArray();
		}

		/// <summary>
		/// ���������� Checkout �� �����
		/// </summary>
		/// <param name="branch"></param>
		/// <param name="createIfNotExists"></param>
		public string Checkout(string branch, bool createIfNotExists = true){
			try
			{
				return ExecuteCommand("checkout", " " + branch);
			}
			catch{
				if (createIfNotExists){
					return ExecuteCommand("checkout", "-b " + branch);
				}
				throw;
			}
		}
		/// <summary>
		/// ��������� ������� Merge
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
		public void InitializeRepository()
		{
			Init();
			if (!string.IsNullOrWhiteSpace(RemoteUrl)){
				RemoteSet(RemoteName, RemoteUrl);
				FixBranchState();
			}
			else{
				WriteFile(".gitignore","*.tmp\r\n*~.*");
				Add();
				
				Commit("init");
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public string Push(){
			return ExecuteCommand("push", RemoteName+" "+Branch);
		}

		/// <summary>
		/// ������������� � ����������������� � ��������� �������
		/// </summary>
		public void FixBranchState(bool resetOnMergeProblems = true){
			if (!string.IsNullOrWhiteSpace(RemoteName)){
				Fetch(RemoteName);
			}
			Checkout(Branch);
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
		/// <summary>
		/// 
		/// </summary>
		/// <param name="refcode"></param>
		public string GetCommitId(string refcode=""){
			if (string.IsNullOrWhiteSpace(refcode)){
				refcode = "HEAD";
			}
			return ExecuteCommand("rev-parse", refcode);
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
			return list.SmartSplit(false, true, '\r', '\n').Select(_ =>{
				if (_.StartsWith("\"") && _.EndsWith("\"")){
					return _.Substring(1, _.Length - 2);
				}
				return _;
			}).ToArray();
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
		/// ���������� ���������� � �������
		/// </summary>
		/// <param name="refcode"></param>
		/// <returns></returns>
		public GitCommitInfo GetCommitInfo(string refcode=null){
			if (string.IsNullOrWhiteSpace(refcode)){
				refcode = "HEAD";
			}
			var data = ExecuteCommand("show", "--format=\"%H|%ct|%cN|%cE|%aN|%aE|%s\"  --quiet \"" + refcode + "\"");
			return ParseGitCommitInfo(data);

		}

		private static GitCommitInfo ParseGitCommitInfo(string data){
			var parts = data.Split('|');
			var result = new GitCommitInfo{
				Hash = parts[0],
				ShortHash = parts[0].Substring(0, 7),
				Commiter = parts[2],
				CommiterEmail = parts[3],
				Author = parts[4],
				AuthorEmail = parts[5],
				Comment = parts[6],
				GlobalRevisionTime = new DateTime(1970, 1, 1).AddSeconds(Convert.ToInt32(parts[1]))
			};
			result.LocalRevisionTime = result.GlobalRevisionTime.ToLocalTime();
			return result;
		}
	}
	/// <summary>
	/// 
	/// </summary>
	public class GitCommitInfo{
		/// <summary>
		/// 
		/// </summary>
		public string Hash { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string ShortHash { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Author { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string AuthorEmail { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public DateTime LocalRevisionTime { get; set; }
		/// <summary>
		/// ���������� �����
		/// </summary>
		public DateTime GlobalRevisionTime { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Commiter { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string CommiterEmail { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Comment { get; set; }
	}
}
