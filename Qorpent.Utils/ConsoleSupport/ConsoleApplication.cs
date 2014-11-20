using System;
using System.Diagnostics;
using System.Linq;
using Qorpent.Log;

namespace Qorpent.Utils{
	/// <summary>
	/// Оболочка для запуска консольных приложений приложений
	/// </summary>
	public static class ConsoleApplication{
		/// <summary>
		/// Вариант вызова консоли с штатными, а не типизированными параметрами
		/// </summary>
		/// <param name="args"></param>
		/// <param name="executor"></param>
		/// <param name="shadowByDefault"></param>
		/// <returns></returns>
		public static int Execute(string[] args, Func<ConsoleApplicationParameters, int> executor, bool shadowByDefault = false){
		    if (args.Contains("--debug")) {
		        Debugger.Launch();
		    }
			return Execute<ConsoleApplicationParameters>(args, executor,shadowByDefault);
		}

		/// <summary>
		/// Стандартный шаблон выполнения консольных приложений в Qorpent
		/// </summary>
		/// <typeparam name="TArgs"></typeparam>
		/// <param name="args"></param>
		/// <param name="executor"></param>
		/// <param name="shadowByDefault">Признак использования ShadowRun по умолчанию</param>
		/// <returns></returns>
		public static int Execute<TArgs>(string[] args, Func<TArgs, int> executor, bool shadowByDefault = false) where TArgs:ConsoleApplicationParameters,new(){
			var log = ConsoleLogWriter.CreateLog();
			try{
				var parameters = new TArgs();
			    parameters.ShadowByDefault = shadowByDefault;
				parameters.Initialize(args);
				log = parameters.Log;
				if ((shadowByDefault || parameters.Shadow) && !parameters.NoShadow){
					var shadower = new ShadowRun {
                        Parameters = parameters,
					    Log = log
					};
					if (!shadower.EnsureShadow()){
						return -3; //shadow restart
					}
				}
				return executor(parameters);
			}
			catch (Exception ex){
				log.Fatal(ex.ToString(), ex);
				return -1;
			}
			finally{
				if (null != log){
					log.Synchronize();
				}
			}
		}
	}
}