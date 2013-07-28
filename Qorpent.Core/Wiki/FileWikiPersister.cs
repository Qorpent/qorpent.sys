using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Qorpent.Applications;
using Qorpent.IO;

namespace Qorpent.Wiki {
    /// <summary>
    /// 
    /// </summary>
    public class FileWikiPersister : Application, IWikiPersister {
        private IFileService _fileService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        public IEnumerable<WikiPage> Get(params string[] codes) {
            foreach (var code in codes) {
                if (CheckArticleExists(code)) {
                    var file = ReadWikiPage(GenerateWikiPagePath(code)).ToList();
                    var title = file[1];
                    var editor = file[0];
                    file.RemoveRange(0, 2);
                    var text = "";

                    foreach (var line in file) {
                        text += line;
                    }

                    if (null != file) {
                        yield return new WikiPage {
                            Title = title,
                            Code = code,
                            Editor = editor,
                            Owner = "local\\Qorpent",
                            Existed = true,
                            Text = text
                        };
                    }
                }
            }
                
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private IEnumerable<string> ReadWikiPage(string path) {
            if (File.Exists(path)) {
                return File.ReadAllLines(path);
            }

            return null;
        }

        /// <summary>
        ///     Проверяет, существует ли статья
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private bool CheckArticleExists(string code) {
            return File.Exists(GenerateWikiPagePath(code));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        public IEnumerable<WikiPage> Exists(params string[] codes) {
            foreach (var code in codes) {
                if (CheckArticleExists(code)) {
                    yield return new WikiPage {
                        Code = code,
                        Existed = true
                    };
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public WikiPage GetWikiPageByVersion(string code, string version) {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pages"></param>
        /// <returns></returns>
        public bool Save(params WikiPage[] pages) {
            TouchPersister();

            foreach (var wikiPage in pages) {
                WriteWikiPage(
                    GenerateWikiPagePath(wikiPage.Code),
                    WikiPageToFileString(wikiPage)
                );
            }
            
            return true;
        }

        private string GenerateWikiPagePath(string code) {
            return _fileService.Root + code;
        }

        /// <summary>
        ///     Перегоняет страницу вики в строку для записи в файл
        /// </summary>
        /// <param name="wikiPage"></param>
        /// <returns></returns>
        private string WikiPageToFileString(WikiPage wikiPage) {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(wikiPage.Editor);
            stringBuilder.AppendLine(wikiPage.Title);
            stringBuilder.AppendLine(wikiPage.Text);

            return stringBuilder.ToString();
        }

        /// <summary>
        ///     Записывает подготовленную страницу Wiki в файл
        /// </summary>
        /// <param name="path"></param>
        /// <param name="wikiPage"></param>
        private void WriteWikiPage(string path, string wikiPage) {
            CreatePathIfNotExists(path);
            File.WriteAllText(path, wikiPage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        private void CreatePathIfNotExists(string path) {
            var elements = path.Replace('\\', '/').Split(new[] { '/' }, StringSplitOptions.None).ToList();
            elements.RemoveAt(elements.Count - 1);
            var dir = String.Join("/", elements);
            
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binary"></param>
        public void SaveBinary(WikiBinary binary) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="withData"></param>
        /// <returns></returns>
        public WikiBinary LoadBinary(string code, bool withData = true) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public IEnumerable<WikiPage> FindPages(string search) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public IEnumerable<WikiBinary> FindBinaries(string search) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public DateTime GetBinaryVersion(string code) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public DateTime GetPageVersion(string code) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public object CreateVersion(string code, string comment) {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public object RestoreVersion(string code, string version) {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public IEnumerable<object> GetVersionsList(string code) {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool GetLock(string code) {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool ReleaseLock(string code) {
            throw new NotSupportedException();
        }

        private void TouchPersister() {
            if (_fileService != null) return;
            _fileService = Container.Get<IFileService>();

            if (null == _fileService) {
                throw new Exception("Need an IFileService instance!");
            }

            _fileService.Root = ".Wiki";
        }
    }
}
