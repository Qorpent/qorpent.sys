using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qorpent.Log;
using Qorpent.Utils;

namespace Qorpent.Host.Server
{
    /// <summary>
    /// Параметры консольного запуска сервера
    /// </summary>
    public class ServerParameters : ConsoleApplicationParameters
    {
        /// <summary>
        /// 
        /// </summary>
        public ServerParameters() {
            TreatAnonymousAsBSharpProjectReference = true;
            ConfigExtension = "hostconf";
            
        }
		/// <summary>
		///		Порт
		/// </summary>
	    public int Port {
		    get {
			    return Get<int>("port");
		    }
			set {
				Set("port", value);
			}
	    }

        /// <summary>
		///		Порт
		/// </summary>
	    public string Root
        {
            get
            {
                return Get<string>("root");
            }
            set
            {
                Set("root", value);
            }
        }
        /// <summary>
        /// Корень настройки веб-фермы
        /// </summary>
        public string WebFarmRoot {
            get {
                var result = this.ResolveBestString("farmroot", "~fr");
                if (string.IsNullOrWhiteSpace(result)) {
                    return EnvironmentInfo.ResolvePath("@repos@/.www");
                }
                return EnvironmentInfo.ResolvePath(result);
            }
            set {
                Set("farmroot",EnvironmentInfo.ResolvePath(value));
            }
        }
        /// <summary>
        /// Перекрывает корневой каталог для запуска приложений
        /// </summary>
        /// <returns></returns>
        protected override string GetBSharpRoot() {
            return WebFarmRoot;
        }
        /// <summary>
        /// Формирует конфигурацию для Host
        /// </summary>
        /// <returns></returns>
        public HostConfig BuildServerConfig() {
	        return new HostConfig(Definition, BSharpContext, log: Log){LogLevel = this.LogLevel, Port = Port, RootFolder = Root};
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected override void InternalInitialize(string[] args) {
            this.ShadowSuffix = this.Arg1;
        }
    }
}
