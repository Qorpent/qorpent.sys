using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.Git{
	/// <summary>
	/// 
	/// </summary>
	public static class GitUtils{
		private static IDictionary<int,int> haskeliocharset = new Dictionary<int, int>();

		private const string haskel =
			@"\320\220\320\221\320\222\320\223\320\224\320\225\320\226\320\227\320\230\320\231\320\232\320\233\320\234\320\235\320\236\320\237\320\240\320\241\320\242\320\243\320\244\320\245\320\246\320\247\320\250\320\251\320\252\320\253\320\254\320\255\320\256\320\257\320\260\320\261\320\262\320\263\320\264\320\265\320\266\320\267\320\270\320\271\320\272\320\273\320\274\320\275\320\276\320\277\321\200\321\201\321\202\321\203\321\204\321\205\321\206\321\207\321\210\321\211\321\212\321\213\321\214\321\215\321\216\321\217";

		/// <summary>
		/// Конвертируем имя файла в нормальный вид с кирилицей и удалением трейловых кавычек
		/// </summary>
		/// <param name="_"></param>
		/// <returns></returns>
		public static string ConvertToValidFileName(string _){
			var result = _;
			result = Regex.Replace(result, @"\\(\d+)\\(\d+)", m =>{
				string r = "";
				r = ((char) haskeliocharset[m.Groups[1].Value.ToInt()*1000 + m.Groups[2].Value.ToInt()]).ToString();

				return r;
			});
			if (result.StartsWith("\"") && _.EndsWith("\"")){
				return result.Substring(1, result.Length - 2);
			}
			return result;
		}
		/// <summary>
		/// Парсит информацию о коммите
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static GitCommitInfo ParseGitCommitInfo(string data){
			var parts = data.Split('|');
			if (parts.Length != 7){
				throw new Exception("cannot parse data as valid commit info '"+data+"'");
			}
			var result = new GitCommitInfo{
				Hash = parts[0],
				ShortHash = parts[0].Substring(0, 7),
				Commiter = parts[2],
				CommiterEmail = parts[3],
				Author = parts[4],
				AuthorEmail = parts[5],
				
				GlobalRevisionTime = new DateTime(1970, 1, 1).AddSeconds(Convert.ToInt32(parts[1]))
			};
			var comment = parts[6];
			if (!String.IsNullOrWhiteSpace(comment)){
				comment = Encoding.UTF8.GetString(Encoding.GetEncoding(1251).GetBytes(comment));
				result.Comment = comment;
			}

			result.LocalRevisionTime = result.GlobalRevisionTime.ToLocalTime();
			return result;
		}

		static GitUtils(){
			var ints = haskel.SmartSplit(false, true, '\\').Select(_=>CoreExtensions.ToInt(_)).ToArray();
			var idx = 0;
			int i = 0;

			for (i = (int) 'А'; i <= (int) 'Я'; i++){
					
				var fst = ints[idx++];
				var sec = ints[idx++];
				haskeliocharset[fst*1000+ sec] = i;

			}
			for (i = (int) 'а'; i <= (int) 'я'; i++){
			
				var fst = ints[idx++];
				var sec = ints[idx++];
				haskeliocharset[fst * 1000 + sec] = i;
			}
			haskeliocharset[320201] = 'Ё';
			haskeliocharset[321221] = 'ё';

		}

		/// <summary>
		/// Конвертирует двусимвольную запись в статус
		/// </summary>
		/// <param name="rawState"></param>
		/// <returns></returns>
		public static FileState ConvertToValidState(char rawState){
			switch (rawState){
				case ' ':
				case '\t': return FileState.NotModified;
				case 'M': return FileState.Modified;
				case 'D': return FileState.Deleted;
				case 'A': return FileState.Added;
				case 'U': return FileState.Updated;
				case 'R': return FileState.Renamed;
				case 'C': return FileState.Copied;
				case '?': return FileState.Untracked;
				case '!': return FileState.Ignored;
				default:return FileState.Undefined;
			}
		}

		/// <summary>
		/// Конвертирует двусимвольную запись в статус
		/// </summary>
		/// <returns></returns>
		public static char ConvertToChar(FileState state)
		{
			switch (state)
			{
				case FileState.NotModified: return ' ';
				case FileState.Modified : return  'M';
				case FileState.Deleted : return  'D';
				case FileState.Added :return 'A';
				case FileState.Updated:  return 'U';
				case FileState.Renamed : return 'R';
				case FileState.Copied: return 'C';
				case FileState.Untracked: return '?';
				case FileState.Ignored: return '!';
				default: return 'X';
			}
		}

	}
}