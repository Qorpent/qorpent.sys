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
// PROJECT ORIGIN: Qorpent.Core/MimeHelper.cs
#endregion
namespace Qorpent.IO {
	/// <summary>
	/// Helper class to convert mime-extension and extension-mime
	/// </summary>
	public static class MimeHelper
	{
		/// <summary>
		/// Стандартный MIME для HTML
		/// </summary>
		public const string HTML = "text/html";
		/// <summary>
		/// 
		/// </summary>
		public const string JSON = "application/json";
        /// <summary>
		/// 
		/// </summary>
		public const string TEXT = "text/plain";

        /// <summary>
        /// Converts extension to mime type
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static string GetMimeByExtension(string extension)
		{
			
			var ext = extension.Replace(".", "").ToLower();
			switch (ext)
			{
				case "txt":
					return "text/plain";
				case "xls":
					return "application/vnd.ms-excel";
				case "doc":
					return "application/vnd.ms-word";
				case "docx":
					return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
				case "xlsx":
					return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
				case "css":
					return "text/css";
				case "js":
					return "text/javascript";
				case "png":
					return "image/png";
                case "svg":
                    return "image/svg+xml";
				case "jpg":
					return "image/jpeg";
                case "bmp":
                    return "image/bmp";
				case "jpeg":
					return "image/jpeg";
                case "tif":
                    return "image/tiff";
                case "tiff":
                    return "image/tiff";
				case "gif":
					return "image/gif";
				case "pdf":
					return "application/pdf";
				case "html":
					return HTML;
				case "htm":
					return "text/html";
				case "zip":
					return "multipart/x-zip";
				case "rar":
					return "application/x-rar-compressed";
				case "xml":
					return "text/xml";
				case "wiki":
					return "text/plain";
				case "woff":
					return "application/x-font-woff";
				case "json":
					return "application/json";
                case "mp4":
			        return "video/mp4";
                case "avi":
			        return "video/x-msvideo";
			}

			return "bin/unknown";
		}

		/// <summary>
		/// Converts mime to extension
		/// </summary>
		/// <param name="mime"> </param>
		/// <returns></returns>
		public static string GetExtensionByMime(string mime) {

			var normalizedmime = mime.ToLower();
			switch (normalizedmime)
			{
				case "text/plain":
					return ".txt";
				case "application/vnd.ms-excel":
					return ".xls";
				case "application/vnd.ms-word":
					return ".doc";
                case "application/msword":
			        return ".doc";
                case "application/msexcel":
                    return ".xls";
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
					return ".docx";
				case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
					return ".xlsx";
				case "text/css":
					return ".css";
				case "text/javascript":
					return ".js";
				case "image/png":
					return ".png";
				case "image/jpeg":
					return ".jpeg";
				case "image/gif":
					return ".gif";
				case "application/pdf":
					return ".pdf";
				case "text/html":
					return ".html";
				case "multipart/x-zip":
					return ".zip";
				case  "application/x-rar-compressed":
					return ".rar";
				case "text/xml":
					return ".xml";
			}

			return "";
		}
	}
}