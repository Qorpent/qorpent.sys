using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qorpent.Utils;
using Qorpent.Utils.Git;

namespace updateRemotes
{
	class Program
	{
		static void Main(string[] args){
			var ah = new ConsoleArgumentHelper();
			var user = ah.ReadLineSafety("User:");
			var pass = ah.ReadLineSafety("Pass:");
			foreach (var repo in new[]{
				"qorpent.kernel","qorpent.sys","qorpent.integration"
		
			}){
				var helper = new GitHelper{
					DirectoryName = "g:/repos/" + repo,
					RemoteName = "gp",
					AuthorName = user,
					Password = pass,
					RemoteUrl = "https://gitpool.ru:8180/qorpent/"+repo.Replace(".","-")+".git"
				}.Connect();
				helper.RemoteSet();
			}
		}
	}
}
