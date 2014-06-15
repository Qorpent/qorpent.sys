using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Qorpent.Applications;
using Qorpent.Data.Connections;
using Qorpent.IoC;
using Qorpent.Log;
using Qorpent.Model;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Data.DataCache
{
	/// <summary>
	/// Класс кэша данных на основе адаптера и СУБД
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ObjectDataCache<T>:IObjectDataCache<T> where T:class,new(){
		

		/// <summary>
		/// Служба генерации соединений
		/// </summary>
		[Inject]
		public IDatabaseConnectionProvider ConnectionProvider{
			get { return _connectionProvider??(_connectionProvider =new DatabaseConnectionProvider{IgnoreDefaultApplication = true}); }
			set { _connectionProvider = value; }
		}
		/// <summary>
		/// Журнал общих событий
		/// </summary>
		public IUserLog Log{
			get { return _log ?? (_log= StubUserLog.Default); }
			set { _log = value; }
		}

		/// <summary>
		/// Журнал событий SQL
		/// </summary>
		public IUserLog SqlLog{
			get { return _sqlLog ?? (_sqlLog=StubUserLog.Default); }
			set{
				_sqlLog = value;
			}
		}

		private const int COMMONIDBASE = -100;
		private const int COMMONIDSTEP = -10;

		/// <summary>
		/// 
		/// </summary>
		protected int CURRENTID = COMMONIDBASE;
		/// <summary>
		/// 
		/// </summary>
		protected object IDSYNC = new object();
		
		/// <summary>
		/// Кэш на основе ID
		/// </summary>
		protected IDictionary<int, T> _nativeCache = new Dictionary<int, T>();
		/// <summary>
		/// Кэш на основе Code
		/// </summary>
		protected IDictionary<string, T> _nativeCodeCache = new Dictionary<string, T>();

		/// <summary>
		/// Адаптер данных
		/// </summary>
		public IObjectDataAdapter<T> Adapter { get; set; }
		/// <summary>
		/// Строка соединения  с внешней БД
		/// </summary>
		public string ConnectionString { get; set; }

		/// <summary>
		/// Возвращает следующий ID
		/// </summary>
		/// <returns></returns>
		public int GetNextId(){
			lock (IDSYNC){
				var result = CURRENTID;
				CURRENTID += COMMONIDSTEP;
				return result;
			}
			
		}


		/// <summary>
		/// Возвращает сущность по коду или ID
		/// </summary>
		/// <param name="key"></param>
		/// <param name="connection"></param>
		/// <returns></returns>
		public T Get(object key, IDbConnection connection = null){
			int id; string code;
			var isid = IsId(key, out id, out code);
			if (isid){
				if (!_nativeCache.ContainsKey(id)){
					UpdateCache("(Id = "+id+")",connection:connection);
				}
				if (!_nativeCache.ContainsKey(id)){
					_nativeCache[id] = null;
				}
				return _nativeCache[id];
			}
			else{
				if (!_nativeCodeCache.ContainsKey(code))
				{
					UpdateCache("(Code = '" + code.ToSqlString()+"')",connection:connection);
				}
				if (!_nativeCodeCache.ContainsKey(code))
				{
					_nativeCodeCache[code] = null;
				}
				return _nativeCodeCache[code];
			}
		}

		/// <summary>
		/// Проверяет что ключ - ID
		/// </summary>
		/// <param name="key"></param>
		/// <param name="id"></param>
		/// <param name="code"></param>
		/// <returns></returns>
		protected bool IsId(object key, out int id, out string code)
		{
			id = 0;
			code = "";
			if (null == key) return false;
			if (key is string){
				var skey = key as string;
				code = skey;
				if (skey == "0") return true;
				id = skey.ToInt();
				return 0 != id;
			}
			if (key is int){
				id = (int) key;
			}
			else{
				id = key.ToInt();	
			}
			return true;
		}

		/// <summary>
		/// Проверить наличие
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool Exists(object key){
			int id; string code;
			var isid = IsId(key, out id, out code);
			return isid ? _nativeCache.ContainsKey(id) : _nativeCodeCache.ContainsKey(code);
		}

		/// <summary>
		/// Установить значение напрямую
		/// </summary>
		/// <param name="value"></param>
		public void Set(T value){
			var id = 0;
			string code = "";
			var vid = value as IWithId;
			
			if (null==vid || ( vid.Id < 0 && vid.Id!=-1) ){
				id = GetNextId();
				if (null != vid){
					vid.Id = id;
				}
			}
			if (null != vid){
				id = vid.Id;
			}
			var vcode = value as IWithCode;
			if (null == vcode || string.IsNullOrWhiteSpace(vcode.Code)){
				code = "CODE"+id.ToString();
				if (null != vcode){
					vcode.Code = code;
				}
			}
			if (null != vcode){
				code = vcode.Code;
			}
			_nativeCache[id] = value;
			_nativeCodeCache[code] = value;
		}

		/// <summary>
		/// Очищает кэш
		/// </summary>
		public void Clear(){
			_nativeCache.Clear();
			_nativeCodeCache.Clear();
			_allLoadWasCalled = false;
		}
		/// <summary>
		/// Метка того что прокачка всех сущностей уже производилась
		/// </summary>
		protected bool _allLoadWasCalled;

		private IUserLog _log;
		private IDatabaseConnectionProvider _connectionProvider;
		private IUserLog _sqlLog;

		/// <summary>
		/// Возвращает все по запросу
		/// </summary>
		/// <param name="query"></param>
		/// <param name="options"></param>
		/// <param name="connection"></param>
		/// <returns></returns>
		public T[] GetAll(object query= null, object options = null, IDbConnection connection = null){
			if (null == query || (query is string && string.IsNullOrWhiteSpace((string) query))){
				if (_allLoadWasCalled){
					return _nativeCache.Values.ToArray();
				}
				UpdateCache("",options,connection);
				_allLoadWasCalled = true;
				return _nativeCache.Values.ToArray();
			}
			var nativequery = query as Func<T, bool>;
			if (null!=nativequery){
				return _nativeCache.Values.Where(nativequery).ToArray();
			}
			var ids = UpdateCache(query as string,options,connection);
			return ids.Select(_ => _nativeCache[_]).ToArray();
		}

		/// <summary>
		/// Обновляет кэш отсутвующих 
		/// </summary>
		/// <param name="query"></param>
		/// <param name="options"></param>
		/// <param name="connection"></param>
		protected int[] UpdateCache(string query, object options = null, IDbConnection connection = null){
			lock (_nativeCache){
				
				var allids = new List<int>();
				bool cascade = true;
				if (null == connection){
					//no self created
					using (var c = ConnectionProvider.GetConnection(ConnectionString)){
						if (string.IsNullOrWhiteSpace(c.ConnectionString)){
							throw new Exception("bad connection!");
						}
						UpdateSingleQuery(query, options, c, allids, cascade);
						c.Close();
					}
				}
				else{
					UpdateSingleQuery(query, options, connection, allids, cascade);
				}
				
				return allids.ToArray();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public IDictionary<int,T> NativeCache{
			get { return _nativeCache; }
		}

		/// <summary>
		/// Низкоуровневый метод обновления после запроса
		/// </summary>
		/// <param name="query"></param>
		/// <param name="options"></param>
		/// <param name="c"></param>
		/// <param name="allids"></param>
		/// <param name="cascade"></param>
		public List<int> UpdateSingleQuery(string query, object options, IDbConnection c, List<int> allids, bool cascade){
			allids = allids ?? new List<int>();
			var q = "select Id from " + Adapter.GetTableName();
			if (!string.IsNullOrWhiteSpace(query)){
				q += " where " + query;
			}
			if (string.IsNullOrWhiteSpace(c.ConnectionString)){
				throw new Exception("bad connection string!!!");
			}
			c.WellOpen();
			var cmd = c.CreateCommand(q);
			var ids = new List<int>();
			SqlLog.Trace(q);
			using (var idsReader = cmd.ExecuteReader()){
				while (idsReader.Read()){
					var id = idsReader.GetInt32(0);
					if (!_nativeCache.ContainsKey(id)){
						ids.Add(id);
					}
					if (!allids.Contains(id)){
						allids.Add(id);
					}
				}
			}
			if (ids.Count != 0){
				q = "(Id in (" + string.Join(",", ids) + "))";
				
				cmd = c.CreateCommand(Adapter.PrepareSelectQuery(q));
				SqlLog.Trace(cmd.CommandText);
				using (var reader = cmd.ExecuteReader()){
					var items = Adapter.ProcessRecordSet(reader,true).ToArray();
					foreach (var item in items){
						Set(item);
					}
				}
				if (cascade){
					AfterUpdateCache(ids, c, options);
				}
			}
			return allids;
		}

		/// <summary>
		/// Выполняет действия после обновления кэша с учетом текущего соединения для оптимизации,
		/// позволяет донастроить кастомные биндинги и увязки с моделью
		/// </summary>
		/// <param name="ids"></param>
		/// <param name="dbConnection"></param>
		/// <param name="context"></param>
		protected virtual void AfterUpdateCache(IList<int> ids, IDbConnection dbConnection, object context){
			if (null != OnAfterUpdateCache){
				OnAfterUpdateCache.Invoke(this,ids,dbConnection,context);
			}
		}

		/// <summary>
		/// Событие обработки кастомного обновления кэша после прокачки из БД
		/// </summary>
		public event Action<object, IList<int>, IDbConnection,object> OnAfterUpdateCache;
	}
}
