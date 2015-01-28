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
	public class ShadowRun{
		private IUserLog _log;

		/// <summary>
		/// 
		/// </summary>
		public IUserLog Log{
			get { return _log ??(_log = StubUserLog.Default); }
			set { _log = value; }
		}
        /// <summary>
        /// 
        /// </summary>
	    public ConsoleApplicationParameters Parameters { get; set; }

	    /// <summary>
		/// Проверка, что  процесс - тень, по умолчанию - текущий процесс
		/// </summary>
		/// <returns></returns>
		public  bool IsShadow(Process process = null){
			process = process ?? Process.GetCurrentProcess();
			var root = EnvironmentInfo.GetShadowDirectory(Parameters.ShadowSuffix);
			return process.MainModule.FileName.NormalizePath().StartsWith(root);
		}
		/// <summary>
		/// Осуществляет поиск копий процесса кроме него
		/// </summary>
		/// <returns></returns>
		public  Process[] FindCopies(){

			var result = 
				Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName)
				       .Where(_ => _.Id != Process.GetCurrentProcess().Id).ToArray();
		    if (null!=Parameters && !string.IsNullOrWhiteSpace(Parameters.ShadowSuffix)) {
		        result = result.Where(_ => _.MainModule.FileName.NormalizePath().StartsWith(
                    EnvironmentInfo.GetShadowDirectory(Parameters.ShadowSuffix).NormalizePath())).ToArray();
		    }
		    return result;
		}

		/// <summary>
		/// На данный момент без параметров - если это Shadow, то ничего не делает кроме убийства копий
		/// если же это не Shadow - то убивает копии, копирует себя в Shadow и перезапускается оттуда с ShadowEvidence и с теми же параметрами, что и были
		/// </summary>
		/// <returns></returns>
		public  bool EnsureShadow() {
		    var trys = 3;
		    while (trys>0) {
		        trys--;
		        try {
		            var copies = FindCopies();
		            if (copies.Length != 0) {
		                if (this.Parameters.EnsureShadow) {
		                    Log.Info("Already run");
		                    return false;
		                }
		            }
		            return RestartShadow(copies);
		        }
		        catch {
		            if (trys > 0) {
		                Thread.Sleep(500);
		            }
		            else {
		                throw;
		            }
		        }
		    }
		    throw new Exception("Cannot restart due to general issue");
		}

	    private bool RestartShadow(Process[] copies) {
	        foreach (var process in copies) {
	            Log.Info("kill copy: " + process.Id);
	            process.Kill();
	            process.WaitForExit();
	            Log.Info("killed");
	        }
	        if (IsShadow()) {
	            Log.Info("Is shadow run, proceed");
	            return true;
	        }
	        Log.Warn("It's not shadow copy, require upgrade and restart");

	        Log.Trace("upgrade start");
	        var targetDirectory = EnvironmentInfo.GetShadowDirectory(Parameters.ShadowSuffix);
	        Directory.CreateDirectory(targetDirectory);
	        foreach (var file in Directory.GetFiles(targetDirectory, "*.*", SearchOption.AllDirectories)) {
	            Log.Debug("delete " + file);
	            File.Delete(file);
	            Log.Debug("ok");
	        }
	        Thread.Sleep(30);
	        var binDir = EnvironmentInfo.BinDirectory.NormalizePath();
	        foreach (var file in Directory.GetFiles(binDir, "*.*", SearchOption.AllDirectories)) {
	            var f = file.NormalizePath().Replace(binDir, "");
	            var trg = Path.Combine(targetDirectory, f);
	            var trgDir = Path.GetDirectoryName(trg);
	            Directory.CreateDirectory(trgDir);
	            Log.Debug("copy " + file);
	            File.Copy(file, trg, true);
	            Log.Debug("ok");
	        }
	        Log.Trace("upgrade complete");
	        var args = Environment.GetCommandLineArgs();
			var safedArgs = string.Join(" ", args.Skip(1).Select(_ => "\"" + (_.StartsWith("--shadow") && !_.IsIn("--shadow", "--shadowsuffix") ? "--" + _.Substring(8) : _) + "\""));
	        safedArgs += " --shadowevidence \"" + binDir;
	        Log.Debug("adapted args " + safedArgs);
	        var exeName = Path.Combine(targetDirectory, Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName));
	        Log.Trace("start " + exeName);
	        var startInfo = new ProcessStartInfo(exeName, safedArgs);
	        if (null != Parameters && Parameters.Get("hidden", false)) {
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
	        }
	        Process.Start(startInfo);
	        Thread.Sleep(100);
	        Log.Debug("started");
	        return false;
	    }
	}
}