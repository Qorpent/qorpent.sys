using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Qorpent.IO.VcsStorage;
using Qorpent.IO.VcsStorage.Engines;

namespace Qorpent.IO.Tests {
    public class VcsStorageTestsBase {
        /// <summary>
        ///     Перзистер
        /// </summary>
        protected VcsStoragePersister Persister;
        [TearDown]
        public void TearDown() {
            Persister.Dispose();
        }
        /// <summary>
        /// 
        /// </summary>
        [SetUp]
        public void SetUp() {
            Persister = new VcsStoragePersister(new FileStorageFs(new DirectoryInfo(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()))));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="names"></param>
        protected void WriteStubElements(IEnumerable<string> names) {
            foreach (var name in names) {

                Persister.Commit(
                    new VcsCommit{File = new FileEntity {Path = name}} ,
                    GenerateStreamFromString(
                        Guid.NewGuid().ToString()
                        )
                    );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        protected void WriteStubElements(int count) {
            for (var i = 0; i < count; i++) {
                Persister.Commit(
                    new VcsCommit {File = new FileEntity {
                        Path = Guid.NewGuid().ToString()
                    }},
                    GenerateStreamFromString(
                        Guid.NewGuid().ToString()
                    )
                );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        protected Stream GenerateStreamFromString(string s) {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}