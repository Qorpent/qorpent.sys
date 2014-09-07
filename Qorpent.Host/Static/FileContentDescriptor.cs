using System;
using System.IO;
using Qorpent.IO;
using Qorpent.Utils.Extensions;

namespace Qorpent.Host.Static {
    /// <summary>
    /// Статический источник для файлов
    /// </summary>
    public class FileContentDescriptor : StaticContentDescriptor {
        private string _path;
        private DateTime _cachedDate;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="root"></param>
        public FileContentDescriptor(string path, string root = null) {
            _path = path.NormalizePath();
            MimeType = MimeHelper.GetMimeByExtension(Path.GetExtension(_path));
            if (string.IsNullOrWhiteSpace(root)) {
                FullName = _path;
            } else {
                var p1 = _path.ToLower();
                var p2 = root.NormalizePath().ToLower();
                if (p1.StartsWith(p2)) {
                    p1 = _path.Substring(p2.Length);
                    if (!p1.StartsWith("/")) {
                        p1 = "/" + p1;
                    }
                    FullName = p1;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool NoCheckVersion { get; set; }

	    /// <summary>
        ///		
        /// </summary>
        public override DateTime GetLastVersion() {
            if (NoCheckVersion) { 
                if (_cachedDate.Year <= 1900) {
                    _cachedDate = File.GetLastWriteTime(_path);
                }
                return _cachedDate;
            }
            return File.GetLastWriteTime(_path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Stream Open() {
	        lock (this){
		        if (UseMemoryCache){
			        if (null == FixedData){
				        this.FixedData = File.ReadAllBytes(_path);
				        IsFixedContent = true;
			        }
			        return new MemoryStream(FixedData);
		        }
		        return File.OpenRead(_path);
	        }
        }
    }
}