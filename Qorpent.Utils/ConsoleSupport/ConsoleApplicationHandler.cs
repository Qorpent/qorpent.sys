using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.Utils
{
	/// <summary>
	/// Safe and friendly wrapper for console application
	/// </summary>
	public class ConsoleApplicationHandler
	{
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
			return await Task.Run(() =>{

				var result = new ConsoleApplicationResult();
				var startinfo = PrepareStrartInfo();
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
			});
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
