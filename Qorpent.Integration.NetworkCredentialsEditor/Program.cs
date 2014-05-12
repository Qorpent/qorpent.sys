using System;
using Qorpent.Security;
using Qorpent.Utils;

namespace Qorpent.Integration.NetworkCredentialsEditor
{
    /// <summary>
    /// Программа NCE
    /// </summary>
    public static class Program
    {
        private static ICredentialStorage Storage;

        /// <summary>
        /// Выполняет установку новых креденций
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args) {
            Storage = Applications.Application.Current.Container.Get<ICredentialStorage>();
            if (null == Storage) {
                throw new Exception("no storage provided");
            }
            if (0 == args.Length) {
                ExecuteInteractive();
            }
            else {
                ExecuteNonInteractive(args);
            }
        }

        private static void ExecuteNonInteractive(string[] args) {
            var cah = new ConsoleArgumentHelper();
            var dict = cah.ParseDictionary(args);
            if(!dict.ContainsKey("host") || string.IsNullOrWhiteSpace(dict["host"]))throw new ArgumentException("no host provided");
            if(!dict.ContainsKey("app") || string.IsNullOrWhiteSpace(dict["app"]))throw new ArgumentException("no app provided");
            if(!dict.ContainsKey("user") || string.IsNullOrWhiteSpace(dict["user"]))throw new ArgumentException("no user provided");
            if(!dict.ContainsKey("password") || string.IsNullOrWhiteSpace(dict["password"]))throw new ArgumentException("no password provided");
            Execute(dict["host"],dict["app"],dict["user"],dict["password"]);
        }

        private static void Execute(string host, string app, string user, string password) {
            Storage.SetCredentials(host,app,user,password);
            Console.WriteLine("credentials set");
        }

        private static void ExecuteInteractive() {
            while (true) {
                Console.Write("Host: ");
                var host = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(host)) {
                    Console.WriteLine("хост указан неверно!");
                    continue;
                }
                Console.Write("App: ");
                var app = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(app))
                {
                    Console.WriteLine("приложение указано неверно!");
                    continue;
                }
                Console.Write("User: ");
                var user = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(user))
                {
                    Console.WriteLine("пользователь указан неверно!");
                    continue;
                }
                var password = new ConsoleArgumentHelper().ReadLineSafety("Password: ");
                if (string.IsNullOrWhiteSpace(password))
                {
                    Console.WriteLine("пароль указан неверно!");
                    continue;
                }
                Console.WriteLine();
                Execute(host, app, user, password);
            }
           
        }
    }
}
