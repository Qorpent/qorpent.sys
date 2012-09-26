#region LICENSE

// Copyright 2007-2012 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Solution: Qorpent
// Original file : TextFileWriter.cs
// Project: Qorpent.Log
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Qorpent.IO;
using Qorpent.Utils.Extensions;

namespace Qorpent.Log {
	/// <summary>
	/// 	Writeout log message to text file
	/// 	using base BXL style or custom format
	/// </summary>
	public class TextFileWriter : BaseLogWriter {
		/// <summary>
		/// </summary>
		public TextFileWriter() {
			CurrentNumber = -1;
		}

		/// <summary>
		/// 	Name of file (full or relative) to file where to write
		/// </summary>
		public string FileName { get; set; }

		/// <summary>
		/// 	Максимальный размер файла в мегабайтах (может быть дробным)
		/// </summary>
		public decimal MaxSize { get; set; }

		/// <summary>
		/// 	Текущий номер файла
		/// </summary>
		public int CurrentNumber { get; set; }


		/// <summary>
		/// 	True - если файл должен формироваться с инкрементным, а не с шаблонным именем
		/// </summary>
		public virtual bool IsIncrement {
			get { return MaxSize > 0; }
		}

		/// <summary>
		/// </summary>
		/// <param name="message"> </param>
		protected override void InternalWrite(LogMessage message) {
			var resolvedFileName = GetResolvedFileName(message);
			var text = "";
			text = GetText(message);
			using (var sw = new StreamWriter(resolvedFileName, true, Encoding.UTF8)) {
				sw.WriteLine(text);
				sw.Flush();
			}
		}

		/// <summary>
		/// 	Перекройте данный метод если вам необходима система ведения журнала в файл не по статическому имени файла
		/// </summary>
		protected virtual string GetResolvedFileName(LogMessage message) {
			if (!Path.IsPathRooted(FileName)) {
				FileName = ResolveService<IFileNameResolver>().Resolve(FileName, false, userLog: new StubUserLog());
			}

			if (FileName.Contains("${")) {
				if (IsIncrement) {
					throw new LogException(
						"Лог-аппендер с поддержкой инкрементного прибавления файлов не поддерживает шаблонные имена файлов с ${...}");
				}
				var filename = StandardFormatedFileName(message);
				Directory.CreateDirectory(Path.GetDirectoryName(filename));
				return filename;
			}

			var result = FileName;
			if (IsIncrement) {
				CurrentNumber = CheckCurrentNumber(result);
				result = GenerateNumberedName(result);
			}
			return result;
		}

		/// <summary>
		/// 	Поддержка подстановки стандартных атрибутов в имя файла
		/// 	Поддерживает обработку следующих атрибутов в формате ${code}
		/// 	<list type="dotted">
		/// 		<item>data - дата сообщения в формате yyyy-MM-dd</item>
		/// 		<item>year - год сообщения</item>
		/// 		<item>month - месяц сообщения</item>
		/// 		<item>day - день сообщения</item>
		/// 		<item>opdata - дата операции записи в формате yyyy-MM-dd</item>
		/// 		<item>opyear - год операции записи</item>
		/// 		<item>opmonth - месяц операции записи</item>
		/// 		<item>opday - день операции записи</item>
		/// 		<item>level - уровень сообщения</item>
		/// 		<item>logger - имя логгера сообщения</item>
		/// 		<item>host - имя хоста сообщения (позволяет распределить лог по классам)</item>
		/// 		<item>usr - имя пользователя</item>
		/// 	</list>
		/// </summary>
		/// <returns> </returns>
		protected string StandardFormatedFileName(LogMessage message) {
			var result = FileName;
			result = Regex.Replace(result, @"\$\{(\w+)\}", m =>
				{
					var code = m.Groups[1].Value.ToLower();
					switch (code) {
						case "date":
							return message.Time.ToString("yyyy-MM-dd");
						case "year":
							return message.Time.Year.ToString();
						case "month":
							return message.Time.Month.ToString();
						case "day":
							return message.Time.Day.ToString();
						case "opdate":
							return DateTime.Now.ToString("yyyy-MM-dd");
						case "opyear":
							return DateTime.Now.Year.ToString();
						case "opmonth":
							return DateTime.Now.Month.ToString();
						case "opday":
							return DateTime.Now.Day.ToString();
						case "level":
							return message.Level.ToString();
						case "usr":
							return message.User.Replace("\\", "/").Replace("/", "--");
						case "logger":
							return message.Name;
						case "app":
							return message.ApplicationName;
						case "host":
							return message.HostObject.ToStr();
						default:
							return code.ToUpper();
					}
				});
			return result;
		}

		/// <summary>
		/// 	Формирует имя целевого файла для режима <see cref="IsIncrement" />
		/// </summary>
		/// <param name="basefilename"> </param>
		/// <returns> </returns>
		protected string GenerateNumberedName(string basefilename) {
			var dir = Path.GetDirectoryName(basefilename);
			var name = Path.GetFileName(basefilename);
			var newname = string.Format("{0:0000}.{1}", CurrentNumber, name);
			var result = Path.Combine(dir, newname);
			return result;
		}

		/// <summary>
		/// 	Проверяет и выставляет новый номер файла (если необходима смена файла по внутреннмеу условиюю)
		/// 	<see cref="FileIsFull" />
		/// </summary>
		/// <param name="basefilename"> </param>
		/// <returns> </returns>
		protected int CheckCurrentNumber(string basefilename) {
			if (CurrentNumber == -1) {
				CurrentNumber = GetLastNumberOfFile(basefilename);
			}
			if (FileIsFull(GenerateNumberedName(basefilename))) {
				CurrentNumber += 1;
			}
			return CurrentNumber;
		}

		/// <summary>
		/// 	Выдает номер последнего записанного файла или 1 в случае его отсутствия
		/// </summary>
		/// <param name="basefilename"> </param>
		/// <returns> </returns>
		protected int GetLastNumberOfFile(string basefilename) {
			var dir = Path.GetDirectoryName(basefilename);
			var search = "*." + Path.GetFileName(basefilename);
			var lastfile =
				Directory.GetFiles(dir, search, SearchOption.TopDirectoryOnly).OrderByDescending(x => x).FirstOrDefault();
			if (null == lastfile) {
				return 1;
			}
			var lastnumber = lastfile.Substring(0, 4).ToInt();
			return lastnumber;
		}

		/// <summary>
		/// 	Главный метод для перекрытия - должен определять границы файла на заполнение
		/// 	руководствутеся правилами - встроенный - объем в мегабайтах
		/// </summary>
		/// <param name="generateNumberedName"> </param>
		/// <returns> </returns>
		protected virtual bool FileIsFull(string generateNumberedName) {
			if (!File.Exists(generateNumberedName)) {
				return false; // файл еще не заполнялся
			}
			if (MaxSize <= 0) {
				return false; //нет ограничений на объем
			}
			var finfo = new FileInfo(generateNumberedName);
			var size = finfo.Length/1024.0m;
			return size >= MaxSize;
		}
	}
}