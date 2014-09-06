using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Qorpent.Log;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils{
	/// <summary>
	/// Утилита для детекции и перезапуска приложения из защищенной директории
	/// </summary>
	public class ProcessSafeStart{
		private IUserLog _log;

		/// <summary>
		/// 
		/// </summary>
		public IUserLog Log{
			get { return _log ??(_log = StubUserLog.Default); }
			set { _log = value; }
		}

		/// <summary>
		/// Проверка, что  процесс - тень, по умолчанию - текущий процесс
		/// </summary>
		/// <returns></returns>
		public  bool IsShadow(Process process = null){
			process = process ?? Process.GetCurrentProcess();
			var root = EnvironmentInfo.GetShadowDirecroty();
			return process.MainModule.FileName.NormalizePath().StartsWith(root);
		}
		/// <summary>
		/// Осуществляет поиск копий процесса кроме него
		/// </summary>
		/// <returns></returns>
		public  Process[] FindCopies(){
			return
				Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName)
				       .Where(_ => _.Id != Process.GetCurrentProcess().Id).ToArray();
		}

		/// <summary>
		/// На данный момент без параметров - если это Shadow, то ничего не делает кроме убийства копий
		/// если же это не Shadow - то убивает копии, копирует себя в Shadow и перезапускается оттуда с ShadowEvidence и с теми же параметрами, что и были
		/// </summary>
		/// <returns></returns>
		public  bool EnsureShadow(){
			var copies = FindCopies();
			foreach (var process in copies){
				Log.Info("kill copy: "+process.Id);
				process.Kill();
				process.WaitForExit();
				Log.Info("killed");
			}
			if (IsShadow()){
				Log.Info("Is shadow run, proceed");
				return true;
			}
			Log.Warn("It's not shadow copy, require upgrade and restart");

			Log.Trace("upgrade start");
			var targetDirectory = EnvironmentInfo.GetShadowDirecroty();
			Directory.CreateDirectory(targetDirectory);
			foreach (var file in Directory.GetFiles(targetDirectory,"*.*",SearchOption.AllDirectories)){
				Log.Debug("delete "+file);
				File.Delete(file);
				Log.Debug("ok");
			}
			Thread.Sleep(30);
			var binDir = EnvironmentInfo.BinDirectory.NormalizePath();
			foreach (var file in Directory.GetFiles(binDir,"*.*",SearchOption.AllDirectories)){
				var f = file.NormalizePath().Replace(binDir,"");
				var trg = Path.Combine(targetDirectory, f);
				Log.Debug("copy " + file);
				File.Copy(file,trg,true);
				Log.Debug("ok");
			}
			Log.Trace("upgrade complete");
			var args = Environment.GetCommandLineArgs();
			var safedArgs = string.Join(" ", args.Skip(1).Select(_ => "\"" + _+ "\""));
			safedArgs += " --shadowevidence \"" + binDir + "\"";
			Log.Debug("adapted args "+safedArgs);
			var exeName = Path.Combine(targetDirectory, Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName));
			Log.Trace("start "+exeName);
			Process.Start(exeName, safedArgs);
			Log.Debug("started");
			return false;
		}
	}
}