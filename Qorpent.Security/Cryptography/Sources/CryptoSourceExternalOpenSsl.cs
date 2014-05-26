using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;

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
            var tempfileSigner = Path.GetTempFileName();
            File.WriteAllText(tempfileSource, action.Entity.EntityBody);
            Environment.SetEnvironmentVariable("OPENSSL_CONF", "c:\\OpenSSL-Win32\\bin\\openssl.cfg");
            Directory.SetCurrentDirectory("C:\\OpenSSL-Win32\\bin");
            var process = System.Diagnostics.Process.Start(
                "openssl.exe",
                "smime -verify -in " + tempfileSource + " -CAfile C:\\OpenSSL-Win32\\bin\\demoCA\\cacert.pem -out " + tempfileTarget + " -signer " + tempfileSigner
            );

            if (process == null) {
                throw new Exception("Can not start the OpenSSL");
            }

            /*
             * openssl smime -verify -in dec.msg -noverify -signer ./demoCA/cacert.pem5 -out textd
             * 
             * openssl x509 -text -in ./demoCA/cacert.pem4
             * */

            process.WaitForExit();
            process.Dispose();

            /*process = System.Diagnostics.Process.Start(
                "openssl.exe",
                "x509 -text -in " + tempfileCert
            );*/

            process = new Process();
            process.StartInfo = new ProcessStartInfo();
            process.StartInfo.FileName = "openssl.exe";
            process.StartInfo.Arguments = "x509 -text -in " + tempfileSigner;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            if (process == null) {
                throw new Exception("Can not start the OpenSSL");
            }

            var sr = process.StandardOutput;
            var cert = sr.ReadToEnd().ToString();

            process.WaitForExit();
            process.Dispose();



            var result = File.ReadAllText(tempfileTarget);
            var entity = new CryptoProviderEntity("", action.Entity.FileType);


            var sz = cert.Substring(cert.IndexOf("Subject: C=RU, ST=moscow, O=Aktiv, OU=IT, CN=", System.StringComparison.Ordinal), cert.Length - cert.IndexOf("Subject: C=RU, ST=moscow, O=Aktiv, OU=IT, CN=", System.StringComparison.Ordinal));
            var t = sz.Substring(sz.IndexOf("CN=", System.StringComparison.Ordinal) + 3,
                                 sz.IndexOf('/') - sz.IndexOf("CN=", System.StringComparison.Ordinal) -3);


            entity.EntityMetadata["Login"] = t;
            File.Delete(tempfileSource);
            File.Delete(tempfileTarget);
            File.Delete(tempfileSigner);
            
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
