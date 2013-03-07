namespace Qorpent.IO {
	/// <summary>
	/// Helper class to convert mime-extension and extension-mime
	/// </summary>
	public static class MimeHelper
	{
		/// <summary>
		/// Converts extension to mime type
		/// </summary>
		/// <param name="extension"></param>
		/// <returns></returns>
		public static string GetMimeByExtension(string extension)
		{
			
			var ext = extension.Replace(".", "");
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
				case "jpg":
					return "image/jpeg";
				case "jpeg":
					return "image/jpeg";
				case "gif":
					return "image/gif";
				case "pdf":
					return "application/pdf";
				case "html":
					return "text/html";
				case "htm":
					return "text/html";
				case "zip":
					return "multipart/x-zip";
				case "rar":
					return "application/x-rar-compressed";
				case "xml":
					return "text/xml";
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