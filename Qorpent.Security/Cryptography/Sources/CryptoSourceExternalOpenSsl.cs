using System;
using System.IO;

namespace Qorpent.Security.Cryptography.Sources {
    /// <summary>
    ///     A cryptography source via binary OpenSSL
    /// </summary>
    public class CryptoSourceExternalOpenSsl : ICryptoSource {
        /// <summary>
        /// 
        /// </summary>
        public CryptoSourceSupportActions SupportActions { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public CryptoSourceSupportTypes SupportTypes { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public CryptoSourceExternalOpenSsl() {
            SupportActions = CryptoSourceSupportActions.Full;
            SupportTypes = CryptoSourceSupportTypes.AllWellKnown;
        }

        /// <summary>
        ///     Execute
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public ICryptoProviderResult Execute(ICryptoProviderAction action) {
            if (!CorrectConfiguration(action)) {
                throw new Exception("Incorrect configuration");
            }

            switch (action.ActionType) {
                case CryptoProviderActionType.Verify: return Verify(action);
                case CryptoProviderActionType.Sign: return Sign(action);
                case CryptoProviderActionType.GenerateCertificate: throw new NotImplementedException();
                case CryptoProviderActionType.GenerateRequest: throw new NotImplementedException();
                case CryptoProviderActionType.None: return null;
                default: return null;
            }
        }

        /// <summary>
        ///     Verify the certificate
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private ICryptoProviderResult Verify(ICryptoProviderAction action) {
            var tempfileSource = Path.GetTempFileName();
            var tempfileTarget = Path.GetTempFileName();
            File.WriteAllText(tempfileSource, action.Entity.EntityBody);
            Environment.SetEnvironmentVariable("OPENSSL_CONF", "c:\\OpenSSL-Win32\\bin\\openssl.cfg");
            Directory.SetCurrentDirectory("C:\\OpenSSL-Win32\\bin");
            var process = System.Diagnostics.Process.Start(
                "openssl.exe",
                "smime -verify -in " + tempfileSource + " -CAfile C:\\OpenSSL-Win32\\bin\\demoCA\\cacert.pem -out " + tempfileTarget
            );

            if (process == null) {
                throw new Exception("Can not start the OpenSSL");
            }

            process.WaitForExit();
            process.Dispose();
            var result = File.ReadAllText(tempfileTarget);
            File.Delete(tempfileSource);
            File.Delete(tempfileTarget);

            var entity = new CryptoProviderEntity("", action.Entity.FileType);
            var first = result.IndexOf(':') - 2;
            entity.EntityMetadata["Hash"] = result.Substring(first, result.Length - first);

            return new CryptoProviderResult(
                entity,
                action,
                result.Contains(action.Config["Salt"])
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private ICryptoProviderResult Sign(ICryptoProviderAction action) {
            var tempfileTarget = Path.GetTempFileName();
            var tempfileSource = Path.GetTempFileName();

            File.WriteAllText(tempfileSource, action.Entity.EntityBody);
            Directory.SetCurrentDirectory("C:\\OpenSSL-Win32\\bin");
            var process = System.Diagnostics.Process.Start(
                "openssl.exe",
                "ca -out " + tempfileTarget + " -in " + tempfileSource + " -batch -passin pass:8139kroots912  -config openssl.cfg"
            );

            if (process == null) {
                throw new Exception("Can not start the OpenSSL");
            }

            process.WaitForExit();
            var newcert = File.ReadAllText(tempfileTarget);
            File.Delete(tempfileTarget);
            File.Delete(tempfileSource);
            var index = newcert.IndexOf("-----BEGIN CERTIFICATE-----", StringComparison.Ordinal);

            return new CryptoProviderResult(
                new CryptoProviderEntity(
                    newcert.Substring(index, newcert.Length - index),
                    action.Entity.FileType
                ),
                action,
                (index != 0)
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private bool CorrectConfiguration(ICryptoProviderAction action) {
            return true;
        }
    }
}
