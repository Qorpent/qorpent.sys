using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Xml.Linq;

namespace Qorpent.Security.Watchdog
{
	/// <summary>
	/// Загружает систему в режиме параноидального использования
	/// </summary>
	public  static partial class Paranoid {
		private static readonly IParanoidProvider _provider;
		/// <summary>
		/// Provider of paranoid mode
		/// </summary>
		public static IParanoidProvider Provider {get { return _provider; }}
		private static object sync = new object();
		
		private static IParanoidProvider GetProvider() {
			lock(sync) {
				var qorpent_security_dll_filename = Path.Combine(EnvironmentInfo.BinDirectory, "Qorpent.Security.dll");
				var qorpent_security_dll_syg_filename = qorpent_security_dll_filename + ".sygnature";
				var passwd_filename = Path.Combine(EnvironmentInfo.RootDirectory, ".passwd");
				var passwd_syg_filename = passwd_filename + ".sygnature";
				___checkout_files_existence(qorpent_security_dll_filename, qorpent_security_dll_syg_filename, passwd_filename, passwd_syg_filename);
				var dll_bytes = __verify_sign(qorpent_security_dll_filename, qorpent_security_dll_syg_filename, developer_key, ParanoidState.InvalidAssemblySygFormat, ParanoidState.InvalidAssemblySyg);
				var passwd_bytes = __verify_sign(passwd_filename, passwd_syg_filename, user_key, ParanoidState.InvalidPasswdSygFormat, ParanoidState.InvalidPasswdSyg);
				var provider_type = ___get_provider_type(dll_bytes);
				var provider = __load_provider(passwd_bytes, provider_type);
				__check_environment(provider);
				return provider;
			}
		}

		private static void __check_environment(IParanoidProvider provider) {
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

		private static IParanoidProvider __load_provider(byte[] passwd_bytes, Type provider_type) {
			IParanoidProvider provider = null;
			XElement xpasswd = null;
			try {
				xpasswd = XElement.Parse(Encoding.UTF8.GetString(passwd_bytes));
			}
			catch {
				throw new ParanoidException(ParanoidState.InvalidPasswdFormat);
			}
			try {
				provider = (IParanoidProvider) Activator.CreateInstance(provider_type, xpasswd);
			}
			catch {
				throw new ParanoidException(ParanoidState.CannotCreateProvider);
			}
			if (provider.State != ParanoidState.Verified) {
				throw new ParanoidException(provider.State);
			}
			return provider;
		}

		static Paranoid() {
			_provider = GetProvider();
		}

		private static Type ___get_provider_type(byte[] dll) {
			Assembly qorpent_security_dll = null;
			try {
				qorpent_security_dll = Assembly.Load(dll);
			}
			catch {
				throw new ParanoidException(ParanoidState.InvalidDLL);
			}

			Type provider_type = null;
			try {
				provider_type = qorpent_security_dll.GetType("Qorpent.Security.Watchdog.ParanoidProvider", true, false);
			}
			catch {
				throw new ParanoidException(ParanoidState.InvalidDLLContent);
			}

			if (!typeof (IParanoidProvider).IsAssignableFrom(provider_type)) {
				throw new ParanoidException(ParanoidState.InvalidProviderType);
			}
			return provider_type;
		}

		private static byte[] __verify_sign(string secure_file, string sygnature_file, string key, ParanoidState invalidFormat, ParanoidState invalidSyg) {
			var content = File.ReadAllBytes(secure_file);
			var file_hash = SHA1.Create().ComputeHash(content);
			var sygnature = Convert.FromBase64String(File.ReadAllText(sygnature_file));
			var sygnature_checker = DSA.Create();
			sygnature_checker.FromXmlString(key);
			bool verified;
			try {
				verified = sygnature_checker.VerifySignature(file_hash,sygnature);
			}
			catch {
				throw new ParanoidException(invalidFormat);
			}
			if (!verified) {
				throw new ParanoidException(invalidSyg);
			}
			return content;
		}

		private static void ___checkout_files_existence(string qorpent_security_dll_filename,
		                                                string qorpent_security_dll_syg_filename, string passwd_filename, string passwd_syg_filename) {
			
			if (!File.Exists(qorpent_security_dll_filename)) {
				throw new ParanoidException(ParanoidState.NoAssembly);
			}
			if (!File.Exists(qorpent_security_dll_syg_filename)) {
				throw new ParanoidException(ParanoidState.NoAssemblySyg);
			}
			if (!File.Exists(passwd_filename)) {
				throw new ParanoidException(ParanoidState.NoPasswd);
			}
			if (!File.Exists(passwd_syg_filename)) {
				throw new ParanoidException(ParanoidState.NoPasswdSyg);
			}
		}
	}
}
