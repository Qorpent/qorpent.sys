using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qorpent.IO.Net;

namespace qhcurl
{
    class Program
    {
        static void Main(string[] args) {
            var cli = new HttpClient();
            Console.WriteLine(cli.GetString(args[0]));
        }
    }
}
