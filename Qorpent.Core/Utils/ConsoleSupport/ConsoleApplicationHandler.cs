using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qorpent.Utils
{
	/// <summary>
	/// Safe and friendly wrapper for console application
	/// </summary>
	public class ConsoleApplicationHandler{
		/// <summary>
		/// Счетчик вызовыов
		/// </summary>
		public static int Calls;
		/// <summary>
		/// 
		/// </summary>
		public IConsoleApplicationListener Listener { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public ConsoleApplicationHandler(){
			Encoding = Encoding.Default;
			WorkingDirectory = Environment.CurrentDirectory;
			EnvironmentVariables = new Dictionary<string, string>();
			StandardArguments = new Dictionary<string, string>();
			Redirect = true;
			NoWindow = true;
		}
		/// <summary>
		/// 
		/// </summary>
		public IDictionary<string,string> StandardArguments{
			get; private set; } 

		/// <summary>
		/// 
		/// </summary>
		public string ExePath { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string WorkingDirectory { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Arguments { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public bool Redirect { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int Timeout { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public Encoding Encoding { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public IDictionary<string, string> EnvironmentVariables { get; private set; }
		/// <summary>
		/// 
		/// </summary>
		public bool NoWindow { get; set; }
		/// <summary>
		/// 
		/// </summary>
		static ConsoleApplicationHandler _null = new ConsoleApplicationHandler{IsStub=true};
		/// <summary>
		/// Если включе стаб, то процесс в реальности не выполняется, а только шлется событие OnStub
		/// </summary>
		public bool IsStub { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public event Action<ConsoleApplicationHandler,ConsoleApplicationResult> OnStub;
		/// <summary>
		/// 
		/// </summary>
		public static ConsoleApplicationHandler Null{
			get { return _null; }
		}

	    public string[] SendLines { get; set; }

	    /// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public ConsoleApplicationResult Run(){
			var t = RunAsync();
			t.Wait();
			return t.Result;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public async Task<ConsoleApplicationResult> RunAsync(){
			return await Task.Run(() => RunSync());
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public ConsoleApplicationResult RunSync(){
			if (IsStub){
				var res = new ConsoleApplicationResult();
				if (null != OnStub) OnStub.Invoke(this, new ConsoleApplicationResult());
				return res;
			}
			Interlocked.Increment(ref Calls);
			var result = new ConsoleApplicationResult();
			var startinfo = PrepareStrartInfo();
			result.StartInfo = startinfo;

			if (ExePath == "del"){
				return DoDeleteCommand(startinfo, result);
			}

			var process = new Process{StartInfo = startinfo};
			try{
				var output = new StringBuilder();
				var error = new StringBuilder();

				process.OutputDataReceived += (s, e) =>{
					output.AppendLine(e.Data);
					if (null != Listener){
						Listener.EmitOutput(e.Data);
						CheckMessage(process);
					}
				};
				process.ErrorDataReceived += (s, e) =>{
					error.AppendLine(e.Data);
					if (null != Listener){
						Listener.EmitOutput(e.Data);
						CheckMessage(process);
					}
				};

				try{
					var wellFinish = false;
					process.Start();
					process.BeginOutputReadLine();
					process.BeginErrorReadLine();
				    if (null != SendLines && 0 != SendLines.Length) {
				        foreach (var sendLine in SendLines) {
				            process.StandardInput.WriteLine(sendLine);
				        }
				    }
					CheckMessage(process);
					if (0 != Timeout){
						wellFinish = process.WaitForExit(Timeout);
						result.Timeouted = !wellFinish;
					}
					else{
						process.WaitForExit();
						wellFinish = true;
					}
					if (!wellFinish){
						if (!process.HasExited){
							process.Kill();
						}
					}
					result.State = process.ExitCode;
					result.Output = output.ToString();
					result.Error = error.ToString();
				}
				catch (Exception ex){
					result.Exception = ex;
				}

				return result;
			}
			finally{
				if (!process.HasExited){
					process.Kill();
				}
			}
		}

	    private static ConsoleApplicationResult DoDeleteCommand(ProcessStartInfo startinfo, ConsoleApplicationResult result) {
	        var fullpath = Path.Combine(startinfo.WorkingDirectory, startinfo.Arguments.Replace("\"", "").Trim());
	        var mask = Path.GetFileName(fullpath);
	        var dir = Path.GetDirectoryName(fullpath);
	        try {
	            if (Directory.Exists(dir)) {
	                var files = Directory.GetFiles(dir, mask);
	                foreach (var file in files) {
	                    try {
	                        File.Delete(file);
	                    }
	                    catch {
	                        try {
	                            Thread.Sleep(10);
	                            File.Delete(file);
	                        }
	                        catch {
	                        }
	                    }
	                }
	            }
	            result.State = 0;
	        }
	        catch (Exception ex) {
	            result.State = -1;
	            result.Exception = ex;
	        }
	        return result;
	    }

	    private void CheckMessage(Process process){
			if (null != Listener){
				var message = Listener.GetMessage();
				if (null != message){
					var startwrite = process.StandardInput.WriteLineAsync(message);
					bool finished = startwrite.Wait(1000000);
					if (!finished){
						process.Kill();
						throw new Exception("cannot send message");
						
					}
				}
			}
		}


		private ProcessStartInfo PrepareStrartInfo(){
			var startinfo = new ProcessStartInfo{
				FileName = EnvironmentInfo.GetExecutablePath(ExePath),
				WorkingDirectory = WorkingDirectory,
				Arguments = (Arguments ?? " ")+string.Join(" ",StandardArguments.Select(
				_=>string.Format("--{0}{1}",_.Key,string.IsNullOrWhiteSpace(_.Value)?"":" \""+_.Value+"\""))),
				UseShellExecute = !Redirect,
				CreateNoWindow = NoWindow
			};
			if (Redirect){
				startinfo.RedirectStandardError = true;
				startinfo.RedirectStandardInput = true;
				startinfo.RedirectStandardOutput = true;
			}
			startinfo.StandardOutputEncoding = Encoding;
			startinfo.StandardErrorEncoding = Encoding;
			foreach (var e in EnvironmentVariables){
				startinfo.EnvironmentVariables[e.Key] = e.Value;
			}
			return startinfo;
		}
	}
}
