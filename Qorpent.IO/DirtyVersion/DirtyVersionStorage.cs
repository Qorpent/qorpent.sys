using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qorpent.IO.DirtyVersion.Mapping;
using Qorpent.IO.DirtyVersion.Storage;

namespace Qorpent.IO.DirtyVersion
{
	/// <summary>
	/// Версионированное хранилище
	/// </summary>
	public class DirtyVersionStorage:HashedDirectoryBase, IDirtyVersionStorage {
		private Mapper _mapper;
		private HashedDirectory _hashed;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetDirectoryName"></param>
		/// <param name="compress"></param>
		/// <param name="hashsize"></param>
		public DirtyVersionStorage(string targetDirectoryName, bool compress = true, int hashsize = Const.MinHashSize)
			: base(targetDirectoryName, compress, hashsize) {
			_rootDirectory = Path.Combine(_rootDirectory, ".gvcs");
			_mapper = new Mapper(Path.Combine(_rootDirectory, ".maps"), false, hashsize);
			_hashed = new HashedDirectory(Path.Combine(_rootDirectory, ".objects"), compress, hashsize);
		}
		/// <summary>
		/// Прямой доступ к мэперу
		/// </summary>
		/// <returns></returns>
		public IMapper GetMapper() {
			return _mapper;
		}
		/// <summary>
		/// Прямой доступ к хранилищу
		/// </summary>
		/// <returns></returns>
		public IHashedDirectory GetStorage() {
			return _hashed;
		}
		/// <summary>
		/// Сохраняет строчные значения
		/// </summary>
		/// <param name="name"></param>
		/// <param name="data"></param>
		/// <param name="basecommit"></param>
		/// <returns></returns>
		public Commit Save(string name, string data, string basecommit = null) {
			var binobj = _hashed.Write(name, data);
			var commit = MakeCommit(name, basecommit, binobj);
			return commit;
		}

		private Commit MakeCommit(string name, string basecommit, HashedDirectoryRecord saved) {
			Commit result;
			using (var o = _mapper.Open(name)) {
				if (string.IsNullOrWhiteSpace(basecommit)) {
					result =o.Commit(saved.DataHash, CommitHeadBehavior.Override);
				}
				else {
					result =o.Commit(saved.DataHash, CommitHeadBehavior.Auto, basecommit);
				}
				o.Commit();
			}
			return result;
		}

		/// <summary>
		/// Сохраняет массив байтов
		/// </summary>
		/// <param name="name"></param>
		/// <param name="data"></param>
		/// <param name="basecommit"></param>
		/// <returns></returns>
		public Commit Save(string name, byte[] data, string basecommit = null)
		{
			return Save(name, new MemoryStream(data));
		}

		/// <summary>
		/// Сохраняет данные из потока
		/// </summary>
		/// <param name="name"></param>
		/// <param name="stream"></param>
		/// <param name="basecommit"></param>
		/// <returns></returns>
		public Commit Save(string name, Stream stream, string basecommit = null)
		{
			var binobj = _hashed.Write(name, stream);
			var commit = MakeCommit(name, basecommit, binobj);
			return commit;
		}

		/// <summary>
		/// Открывет на чтение файловый поток
		/// </summary>
		/// <param name="name"></param>
		/// <param name="versionHash"></param>
		/// <returns></returns>
		public Stream Open(string name, string versionHash = null) {
			string datahash = null;
			string namehash = null;
			using (var o = _mapper.Open(name)) {
				if (string.IsNullOrWhiteSpace(versionHash)) {
					versionHash = o.GetHeadHash();
					if (versionHash == Const.DETACHEDHEAD) {
						throw new Exception("head is detached");
					}
				}
				var c = o.Resolve(versionHash);
				if (null == c) {
					throw new Exception("commit " + versionHash + " not resolved");
				}
				datahash = c.Hash;
				namehash = o.MappingInfo.NameHash;
			}
			var binrecord = new HashedDirectoryRecord {DataHash = datahash, NameHash = namehash};
			return _hashed.Open(binrecord);
		}
		/// <summary>
		/// Возвращает полную историю
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public MappingInfo Explain(string name) {
			using (var o = _mapper.Open(name)) {
				return o.MappingInfo;
			}
		}
		/// <summary>
		/// Возвращает информацию о последнем или указанном коммите
		/// </summary>
		/// <param name="name"></param>
		/// <param name="hash"></param>
		/// <returns></returns>
		public Commit ExplainVersion(string name, string hash = null) {
			using (var o = _mapper.Open(name)) {
				if (string.IsNullOrWhiteSpace(hash)) {
					return o.MappingInfo.GetHead();
				}
				return o.Resolve(hash);
			}
		}
	}
}
