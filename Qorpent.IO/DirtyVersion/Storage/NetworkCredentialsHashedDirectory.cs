using System;
using System.IO;
using Qorpent.IoC;

namespace Qorpent.IO.DirtyVersion.Storage {
    /// <summary>
    /// Специализированное хранилище пользовтельских креденций
    /// </summary>
    [ContainerComponent(ServiceType = typeof(IHashedDirectory), Name = "nc.hasheddirectory")]
    public class NetworkCredentialsHashedDirectory : HashedDirectory {
        /// <summary>
        /// Создает хэшированную директорию, нацеленную на специальную папку
        /// </summary>
        public NetworkCredentialsHashedDirectory() : base(Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\.qc"),true,Const.MaxHashSize) {
            if (!Directory.Exists(_rootDirectory))
            {
                Directory.CreateDirectory(_rootDirectory);
            }
            var attr = File.GetAttributes(_rootDirectory);
            if (0 == (attr & FileAttributes.Encrypted)) {
                File.Encrypt(_rootDirectory);
            }
            if (0 == (attr & FileAttributes.Hidden))
            {
                File.SetAttributes(_rootDirectory,attr|FileAttributes.Hidden);
            }
        }
    }
}