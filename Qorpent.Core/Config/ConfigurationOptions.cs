using Qorpent.IO;

namespace Qorpent.Config {
    public class ConfigurationOptions {
        public ConfigurationOptions() {
            ConfigNameBSharpPrefix = "APP";
            AddNameToGlobalConditions = true;
        }
        public string Name { get; set; }
        public FileSet FileSet { get; set; }
        public string ConfigNameBSharpPrefix { get; set; }
        public bool AddNameToGlobalConditions { get; set; }
    }
}