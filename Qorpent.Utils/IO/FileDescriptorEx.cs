using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Qorpent.Bxl;
using Qorpent.Utils.Git;

namespace Qorpent.Utils.IO
{
    /// <summary>
    /// Описывает файловый или условно файловый ресурс с дополнительными возможностями
    /// </summary>
    public class FileDescriptorEx : IVersionedDescriptor {
        private GitCommitInfo _commitInfo;
        private DateTime _version;
        private string _hash;
        private bool _isGitBased;
        private XElement _attributes;


        /// <summary>
        /// Признак использования Git в качестве источника версий
        /// </summary>
        public bool IsGitBased {
            get {
                CheckoutVersions();
                return _isGitBased;
            }
            set { _isGitBased = value; }
        }

        /// <summary>
        /// Хэщ версии
        /// </summary>
        public string Hash {
            get {
                CheckoutVersions();
                return _hash;
            }
            set { _hash = value; }
        }

        /// <summary>
        /// Время формирования версии
        /// </summary>
        public DateTime Version {
            get {
                CheckoutVersions();
                return _version;
            }
            set { _version = value; }
        }

        /// <summary>
        /// Логическое или локальное имя
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Полное имя файла
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Информация о связанном коммите
        /// </summary>
        public GitCommitInfo CommitInfo {
            get {
                CheckoutVersions();
                return _commitInfo;
            }
            set { _commitInfo = value; }
        }

        /// <summary>
        /// Расширенные атрибуты текстового файла
        /// </summary>
        public XElement Header {
            get {
                if (null == _attributes) {
                    ReadAttributes();
                }
                return _attributes;
            }
            set { _attributes = value; }
        }

        private void ReadAttributes() {
            lock (this) {
                Header = FileSystemHelper.ReadXmlHeader(FullName);
            }
        }

        private void CheckoutVersions() {
            if (!string.IsNullOrWhiteSpace(_hash)) {
                return;
            }
            if (string.IsNullOrWhiteSpace(FullName)) {
                throw new Exception("FullName not setup");
            }
            if (!File.Exists(FullName)) {
                throw new Exception("file not exists "+FullName);
            }
            lock (this) {
                var gitCommit = GitHelper.GetLastCommit(FullName);
                if (null == gitCommit) {
                    using (var s = File.Open(FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                        var md5 = MD5.Create();
                        var hash = md5.ComputeHash(s);
                        Hash = Convert.ToBase64String(hash);
                        Version = File.GetLastWriteTimeUtc(FullName);
                    }
                }
                else {
                    IsGitBased = true;
                    CommitInfo = gitCommit;
                    Hash = gitCommit.Hash;
                    Version = gitCommit.GlobalRevisionTime;
                }
            }
        }
    }
}
