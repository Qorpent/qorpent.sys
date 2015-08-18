using Qorpent.IO;

namespace Qorpent.Config {
    public class ConfigurationOptions {
        public string Name { get; set; }
        public FileSet FileSet { get; set; }
        public string ConfigNameBSharpPrefix { get; set; } = "APP";
        public bool AddNameToGlobalConditions { get; set; } = true;
    }
}