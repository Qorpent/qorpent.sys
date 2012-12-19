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
// Original file : FileSplitWriter.cs
// Project: Qorpent.Dsl
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Qorpent.Utils.Extensions;

namespace Qorpent.Dsl {
	/// <summary>
	/// 	Process given content saving with splitting into several output files
	/// </summary>
	public class FileSplitWriter {
		/// <summary>
		/// 	Default mask used to define start of output to target file
		/// </summary>
		public const string DefaultStartFileMask = @"@@@@/(?<filename>[\s\S]+?)/@@@@";

		/// <summary>
		/// 	Default mask used to define end of output to custom file
		/// </summary>
		public const string DefaultEndFileMask = "@@@@@@@@";

		/// <summary>
		/// 	Write content to given file with splitting and saving to additional
		/// 	files using default or given start-file and end-file markers
		/// 	first occurance of file treated as REWRITE and following TREATED as APPEND
		/// </summary>
		/// <param name="mainpath"> default target path - not splitted context will be writed here </param>
		/// <param name="content"> source content to be splitted and writeout </param>
		/// <param name="writeEmptyFiles"> true if empty or ws files have to be still saved </param>
		/// <param name="customStartFileRegex"> a regex that contains mask of file start (must contain 'filename' named group) default- <see
		/// 	 cref="FileSplitWriter.DefaultStartFileMask" /> </param>
		/// <param name="customEndFileRegex"> a regex that contains mask of end of file output <see
		/// 	 cref="FileSplitWriter.DefaultEndFileMask" /> </param>
		public Dictionary<string, string> WriteContent(string mainpath, string content, bool writeEmptyFiles = false,
		                                               string customStartFileRegex = null, string customEndFileRegex = null) {
			var startregex = customStartFileRegex.IsEmpty() ? DefaultStartFileMask : customEndFileRegex;
			var endregex = customEndFileRegex.IsEmpty() ? DefaultEndFileMask : customEndFileRegex;
			var fullregex = startregex + @"(?<content>[\s\S]*?)" + endregex;
			var dir = Path.GetDirectoryName(mainpath);
			var fileContent = new Dictionary<string, string>();
			fileContent[mainpath] = "";
			var maincontent = Regex.Replace(content, fullregex,
			                                m =>
				                                {
					                                var filename = m.Groups["filename"].Value;
					                                var subcontent = m.Groups["content"].Value;
					                                var fullname = filename;
			                                    try {
			                                        if (!Path.IsPathRooted(fullname)) {
			                                            fullname = Path.GetFullPath(Path.Combine(dir, filename));
			                                        }
			                                    }
			                                    catch (Exception e) {
			                                         throw new QorpentException("File: " + fullname, e);
			                                    }
					                                if (!fileContent.ContainsKey(fullname)) {
						                                fileContent[fullname] = "";
					                                }

					                                if (fileContent[fullname] != "") {
						                                fileContent[fullname] += Environment.NewLine;
					                                }

					                                fileContent[fullname] += subcontent;

					                                return "";
				                                }, RegexOptions.Compiled);
			fileContent[mainpath] = maincontent;
			foreach (var filepair in fileContent.ToArray()) {
				if (writeEmptyFiles || filepair.Value.IsNotEmpty()) {
				    try {
					    Directory.CreateDirectory(Path.GetDirectoryName(filepair.Key));
				        using (var sw = new StreamWriter(filepair.Key, false, Encoding.UTF8)) {
				            sw.Write(filepair.Value);
				            sw.Flush();
				        }
				    }
				    catch (Exception e) {
				        throw new QorpentException("File: " + filepair.Key, e);
				    }
				}
				else {
					fileContent.Remove(filepair.Key);
				}
			}
			return fileContent;
		}
	}
}