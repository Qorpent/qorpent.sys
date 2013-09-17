using System.IO;
using System.Xml.Linq;

namespace Qorpent.IO.DirtyVersion.Mapping {
	/// <summary>
	/// Сесиия работы с мапером
	/// </summary>
	public class MapperSession: IMapperSession {
		private readonly IMapper _mapper;
		private readonly string _file;
		private readonly MappingInfoSerializer _serializer = new MappingInfoSerializer();
		private MappingInfo _info;
		private readonly string _fileName;
		private IMappingOperator _operator;
		private readonly string _hash;
		private bool _commited;

		/// <summary>
		/// Акцессор до исходного мапинга
		/// </summary>
		public MappingInfo GetMappingInfo() {
			
				if (null == _info) {
					if (!File.Exists(_fileName)) {
						_info = CreateNewInfo(_hash);
					}
					else {
						_info = LoadInfo(_fileName);
					}
					_info.Normalize();
					
				}
				return _info;
			
		}

		/// <summary>
		/// Отменяет изменения и сбрасывает MappingInfo
		/// </summary>
		public void Cancel() {
			_info = null;
			_commited = false;
		}

		/// <summary>
		/// Помечает сессию к сохранению
		/// </summary>
		public void Commit() {
			_commited = true;
		}

		private MappingInfo LoadInfo(string fileName) {
			var xml = XElement.Load(fileName);
			return _serializer.DeSerialize(xml);
		}

		private MappingInfo CreateNewInfo(string makeHash) {
			var result = new MappingInfo {Name = _file, NameHash = makeHash, Head = Const.DETACHEDHEAD};
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		public MapperSession(IMapper mapper, string file,string hash, string filename) {
			_mapper = mapper;
			_file = file;
			_hash = hash;
			_fileName = filename;
		}
		/// <summary>
		/// 
		/// </summary>
		public void Dispose() {
			if (null!=_info && _info.Changed && _commited) {
				_info.Normalize();
				Save();
			}
			_mapper.ReleaseLock(_file);
		}

		private void Save() {
			var xml = _serializer.Serialize(_info);
			xml.Save(_fileName);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IMappingOperator GetOperator() {
			return _operator ?? (_operator = new MappingOperator(this));
		}
	}
}