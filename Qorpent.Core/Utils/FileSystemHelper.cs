using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.Bxl;

namespace Qorpent.Utils
{
	/// <summary>
	/// 
	/// </summary>
	public static class FileSystemHelper
	{
	    /// <summary>
	    /// Создает и очищает временную папку
	    /// </summary>
	    /// <param name="name">Указать имя папки, по умолчанию - заточка для тестов КЛАСС_МЕТОД</param>
	    /// <param name="tmproot">Указать, если нужно использовать особенную корневую временную папку</param>
	    /// <returns></returns>
	    public static string ResetTemporaryDirectory(string name = null, string tmproot = null) {
            var root = Path.GetTempPath();
            if (!String.IsNullOrWhiteSpace(tmproot)) {
                Directory.CreateDirectory(tmproot);
                root = tmproot;
            }
	        if (String.IsNullOrWhiteSpace(name)) {
	            var method = new StackFrame(1, true).GetMethod();
	            name = method.DeclaringType.Name + "_" + method.Name;
	        }
            var tmpdir = Path.Combine(root,name);
            KillDirectory(tmpdir);
            Directory.CreateDirectory(tmpdir);
	        return tmpdir;
	    }

	    public static void DeleteIfExists(string name) {
	        if (File.Exists(name)) {
	            File.Delete(name);
	        }
	    }

		/// <summary>
		/// Avoids Directory.Delete problem with ReadOnly files and checks directory existence
		/// </summary>
		/// <param name="dirname"></param>
		public static void KillDirectory(string dirname)
		{
			if (Directory.Exists(dirname))
			{
				try
				{
					Directory.Delete(dirname, true);
				}
				catch
				{
                    Thread.Sleep(100);
					foreach (var file in Directory.GetFiles(dirname, "*.*", SearchOption.AllDirectories))
					{
						File.SetAttributes(file, FileAttributes.Normal);
					}
					Directory.Delete(dirname, true);
				}
			}


		}

	    /// <summary>
	    /// Считывет заголовок исходного файла в формате BXL/XML
	    /// </summary>
	    /// <param name="fullname"></param>
	    /// <returns></returns>
	    public static XElement ReadXmlHeader(string fullname) {
	        var result = new XElement("stub");
	        if (!String.IsNullOrWhiteSpace(fullname) && File.Exists(fullname)) {
	            var rawHeader = ReadRawHeader(fullname);
	            result = GetXml(rawHeader);
            }
            else if (Directory.Exists(fullname) && File.Exists(Path.Combine(fullname,".header"))) {
                result = GetXml(File.ReadAllText(Path.Combine(fullname, ".header")));
            }
	        return result;
	    }

	    private static XElement GetXml(string rawHeader) {
	        XElement result = null;
            if (!String.IsNullOrWhiteSpace(rawHeader)) {
	            if (rawHeader.StartsWith("<")) {
	                result = XElement.Parse(rawHeader);
	            }
	            else {
	                result = new BxlParser().Parse(rawHeader);
	                if (result.Elements().Count() == 1) {
	                    result = result.Elements().First();
	                }
	            }
	        }
	        return result;
	    }

	    /// <summary>
        /// Считывет заголовок исходного файла в формате BXL/XML
        /// </summary>
        /// <param name="fullname"></param>
        /// <returns></returns>
        public static XElement ReadXmlHeader(TextReader input) {
	        var raw = ReadRawHeader(input);
	        return GetXml(raw);
	    }

	    /// <summary>
	    /// Считывает спецальный хидер файла
	    /// </summary>
	    /// <param name="fileName"></param>
	    /// <returns></returns>
	    public static string ReadRawHeader(string fileName) {
	        if (String.IsNullOrWhiteSpace(fileName)) return "";
	        if (!File.Exists(fileName)) return "";	        
	        using (var fr = new StreamReader(File.OpenRead(fileName)))
            {
                return ReadRawHeader(fr);
	        }
	    }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
	    public static string ReadRawHeader(TextReader input) {
	        var header = "";
	        bool starterRead = false;
	        bool first = true;
	        bool multiline = false;
	        while (true) {
                var line = input.ReadLine();
                if(null==line) break;
                if(string.IsNullOrWhiteSpace(line))continue;
	            
	            if (first) {
                    if (line.Contains("#!") || line.Contains("--!") || line.Contains("//!") || line.Contains("<!--!") || line.Contains("/*!"))
                    {
	                    var value = Regex.Match(line, @"((\#)|(//)|(<!--)|(/\*)|(--))!(?<v>[\s\S]+)").Groups["v"].Value;
	                    if (line.Contains("/*!") || line.Contains("<!--!")) {
	                        multiline = true;
	                    }
	                    header += value;
	                }
	                else {
	                    break;
	                }
	                first = false;
	            }
	            else {
	                if (line.Contains("#!") || line.Contains("//!") || line.Contains("--!") ) {
	                    var value =
	                        Regex.Match(line, @"((\#)|(//)|(--))!(?<v>[\s\S]+)").Groups["v"].Value;
	                    header += Environment.NewLine + value;
	                }
	                else if (multiline) {
	                    header += Environment.NewLine + line;
	                }
	                else {
	                    break;
	                }
	            }

	            if (multiline && (header.Contains("*/")||header.Contains("-->"))) {
	                header = Regex.Match(header, @"^([\s\S]+?)((\*/)|(-->))").Groups[1].Value;
	                break;
	            }
	        }
	        return header;
	    }
	}
}
