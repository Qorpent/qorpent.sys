#region LICENSE
// Copyright 2007-2013 Qorpent Team - http://github.com/Qorpent
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// PROJECT ORIGIN: Qorpent.Core/Paranoid.cs
#endregion
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Xml.Linq;

namespace Qorpent.Security.Watchdog {
	/// <summary>
	/// 	Загружает систему в режиме параноидального использования
	/// </summary>
	public static partial class Paranoid {
// ReSharper disable InconsistentNaming
		private static readonly IParanoidProvider _provider;
// ReSharper restore InconsistentNaming

		private static readonly object Sync = new object();

		static Paranoid() {
			_provider = GetProvider();
		}

		/// <summary>
		/// 	Provider of paranoid mode
		/// </summary>
		public static IParanoidProvider Provider {
			get { return _provider; }
		}

		private static IParanoidProvider GetProvider() {
			lock (Sync) {
				var qorpentSecurityDllFilename = Path.Combine(EnvironmentInfo.BinDirectory, "Qorpent.Security.dll");
				var qorpentSecurityDllSygFilename = qorpentSecurityDllFilename + ".sygnature";
				var passwdFilename = Path.Combine(EnvironmentInfo.RootDirectory, ".passwd");
				var passwdSygFilename = passwdFilename + ".sygnature";
				CheckoutFilesExistence(qorpentSecurityDllFilename, qorpentSecurityDllSygFilename, passwdFilename, passwdSygFilename);
				var dllBytes = VerifySign(qorpentSecurityDllFilename, qorpentSecurityDllSygFilename, DeveloperKey,
				                          ParanoidState.InvalidAssemblySygFormat, ParanoidState.InvalidAssemblySyg);
				var passwdBytes = VerifySign(passwdFilename, passwdSygFilename, UserKey, ParanoidState.InvalidPasswdSygFormat,
				                             ParanoidState.InvalidPasswdSyg);
				var providerType = GetProviderType(dllBytes);
				var provider = LoadProvider(passwdBytes, providerType);
				CheckEnvironment(provider);
				return provider;
			}
		}

		private static void CheckEnvironment(IParanoidProvider provider) {
			if (!EnvironmentInfo.IsWeb) {
				var domain = Environment.UserDomainName;
				if (domain.ToUpper() == "." || domain == "" || domain.ToUpper() == Environment.MachineName) {
					domain = "local";
				}
				var principal = new GenericPrincipal(new GenericIdentity(domain + "\\" + Environment.UserName), null);
				if (!provider.IsSpecialUser(principal) || !provider.IsInRole(principal, "ADMIN")) {
					if (Environment.UserInteractive) {
						Console.Write("Username: ");
						var username = Console.ReadLine();
						Debug.Assert(!string.IsNullOrWhiteSpace(username), "username != null");
						var password = "";
						Console.Write("Password:");
						while (true) {
							var c = Console.ReadKey();

							if (c.Key == ConsoleKey.Enter) {
								break;
							}
							password += c.KeyChar;
							Console.Write("\x8");
							Console.Write(" ");
							Console.Write("\x8");
						}


						principal = new GenericPrincipal(new GenericIdentity(username), null);
						if (!(provider.Authenticate(principal, password) && provider.IsInRole(principal, "ADMIN"))) {
							throw new ParanoidException(ParanoidState.NoSuUser);
						}
					}
					else {
						throw new ParanoidException(ParanoidState.CannotDoSuLoginInConsole);
					}
				}
			}
		}

		private static IParanoidProvider LoadProvider(byte[] passwdBytes, Type providerType) {
			IParanoidProvider provider;
			XElement xpasswd;
			try {
				xpasswd = XElement.Parse(Encoding.UTF8.GetString(passwdBytes));
			}
			catch {
				throw new ParanoidException(ParanoidState.InvalidPasswdFormat);
			}
			try {
				provider = (IParanoidProvider) Activator.CreateInstance(providerType, xpasswd);
			}
			catch {
				throw new ParanoidException(ParanoidState.CannotCreateProvider);
			}
			if (provider.State != ParanoidState.Verified) {
				throw new ParanoidException(provider.State);
			}
			return provider;
		}

		private static Type GetProviderType(byte[] dll) {
			Assembly qorpentSecurityDll;
			try {
				qorpentSecurityDll = Assembly.Load(dll);
			}
			catch {
				throw new ParanoidException(ParanoidState.InvalidDll);
			}

			Type providerType;
			try {
				providerType = qorpentSecurityDll.GetType("Qorpent.Security.Watchdog.ParanoidProvider", true, false);
			}
			catch {
				throw new ParanoidException(ParanoidState.InvalidDllContent);
			}

			if (!typeof (IParanoidProvider).IsAssignableFrom(providerType)) {
				throw new ParanoidException(ParanoidState.InvalidProviderType);
			}
			return providerType;
		}

		private static byte[] VerifySign(string secureFile, string sygnatureFile, string key, ParanoidState invalidFormat,
		                                 ParanoidState invalidSyg) {
			var content = File.ReadAllBytes(secureFile);
			var fileHash = SHA1.Create().ComputeHash(content);
			var sygnature = Convert.FromBase64String(File.ReadAllText(sygnatureFile));
			var sygnatureChecker = DSA.Create();
			sygnatureChecker.FromXmlString(key);
			bool verified;
			try {
				verified = sygnatureChecker.VerifySignature(fileHash, sygnature);
			}
			catch {
				throw new ParanoidException(invalidFormat);
			}
			if (!verified) {
				throw new ParanoidException(invalidSyg);
			}
			return content;
		}

		private static void CheckoutFilesExistence(string qorpentSecurityDllFilename,
		                                           string qorpentSecurityDllSygFilename, string passwdFilename,
		                                           string passwdSygFilename) {
			if (!File.Exists(qorpentSecurityDllFilename)) {
				throw new ParanoidException(ParanoidState.NoAssembly);
			}
			if (!File.Exists(qorpentSecurityDllSygFilename)) {
				throw new ParanoidException(ParanoidState.NoAssemblySyg);
			}
			if (!File.Exists(passwdFilename)) {
				throw new ParanoidException(ParanoidState.NoPasswd);
			}
			if (!File.Exists(passwdSygFilename)) {
				throw new ParanoidException(ParanoidState.NoPasswdSyg);
			}
		}
	}
}