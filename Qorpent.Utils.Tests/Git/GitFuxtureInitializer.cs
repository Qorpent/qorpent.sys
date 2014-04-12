using System;
using System.Diagnostics;
using System.Text;

namespace Qorpent.Utils.Tests {
    public static class GitFuxtureInitializer {
        /// <summary>
        ///     Установка фикстуры
        /// </summary>
        public static void SetUpFixture() {
            var startInfo = new ProcessStartInfo {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                FileName = "git",
                Arguments = "config --global user.email",
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.GetEncoding(1251),
            };
            var getCurrentEmailProcess = Process.Start(startInfo);
            if (getCurrentEmailProcess == null) {
                throw new Exception("Cannot get current email");
            }
            var output = getCurrentEmailProcess.StandardOutput.ReadToEnd();
            if (!string.IsNullOrWhiteSpace(output)) {
                return;
            }

            var setEmailProcess = Process.Start(new ProcessStartInfo {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                FileName = "git",
                Arguments = "config --global user.email \"you@example.com\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.GetEncoding(1251),
            });
            var setNameProcess = Process.Start(new ProcessStartInfo {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                FileName = "git",
                Arguments = "config --global user.name \"Some one\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.GetEncoding(1251),
            });
            if (setEmailProcess == null || setNameProcess == null) {
                throw new Exception("Cannot set user name or email");
            }
            setEmailProcess.Start();
            setNameProcess.Start();
            setEmailProcess.WaitForExit();
            setNameProcess.WaitForExit();

        }
    }
}