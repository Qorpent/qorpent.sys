using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Bxl;
using Qorpent.Utils;

namespace Qorpent.Config
{
    public class ConfigurationLoader:IConfigurationLoader
    {
        private ConfigurationOptions opts;

   
        public ConfigurationLoader(ConfigurationOptions opts = null)
        {
            this.opts = opts;
        }

        public IConfigProvider Load(ConfigurationOptions options = null) {
            options = options ?? opts ?? new ConfigurationOptions();
#if BRIDGE
		    var compiler = new BSharpCompiler();
		    var bxl = new BxlParser();
#else
            var compiler = WellKnownHelper.Create<IBSharpCompiler>();           
            var bxl = new BxlParser();
#endif
            SetupGlobals(options, compiler);
            var context = compiler.Compile(options.FileSet.CollectBxl());
            return new GenericConfiguration(GetConfig(options, context),context);
        }

        private static XElement GetConfig(ConfigurationOptions options, IBSharpContext context) {
            XElement config = null;
            if (!string.IsNullOrWhiteSpace(options.Name)) {
                var cls = context.Get(options.Name);
                if (null == cls) {
                    cls = context.ResolveAll(options.Name).FirstOrDefault();
                }
                if (null == cls) {
                    throw new Exception("cannot find given config name " + options.Name);
                }
                config = cls.Compiled;
            }
            return config;
        }

        private static void SetupGlobals(ConfigurationOptions options, IBSharpCompiler compiler) {
            if (options.AddNameToGlobalConditions && !string.IsNullOrWhiteSpace(options.Name)) {
                var cond = "APP_" + options.Name.ToUpperInvariant();
                if (!compiler.Global.ContainsKey(cond)) {
                    compiler.Global[cond] = true;
                }
                if (!compiler.Global.ContainsKey(options.ConfigNameBSharpPrefix)) {
                    compiler.Global[options.ConfigNameBSharpPrefix] = options.Name;
                }
            }
        }
    }
}
