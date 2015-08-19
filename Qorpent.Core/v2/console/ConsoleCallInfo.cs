using System;
using System.IO;
using Qorpent;
using Qorpent.IoC;
using Qorpent.IO;

namespace qorpent.v2.console {
    public class ConsoleCallInfo {
        public string[] Arguments { get; set; }
        public string CurrentDirectory { get; set; }
        public string BinDirectory { get; set; }
        public string RepositoryPath { get; set; }
        public FileSet ConfigSet { get; set; }
        public IContainer Container { get; set; }
        public string[] Libraries { get; set; }

        public void Normalize() {
            Arguments = Arguments ?? Environment.GetCommandLineArgs();
            if (string.IsNullOrWhiteSpace(CurrentDirectory)) {
                CurrentDirectory = EnvironmentInfo.RootDirectory;
            }
            if (string.IsNullOrWhiteSpace(BinDirectory)) {
                BinDirectory = EnvironmentInfo.BinDirectory;
            }
            if (string.IsNullOrWhiteSpace(RepositoryPath)) {
                RepositoryPath = EnvironmentInfo.GetRepositoryRoot();
            }
            if (null==ConfigSet) {
                ConfigSet = new FileSet(Path.Combine(RepositoryPath,".www"));
            }
        }
    }
}