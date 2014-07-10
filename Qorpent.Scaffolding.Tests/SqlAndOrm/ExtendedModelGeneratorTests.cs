using System;
using NUnit.Framework;
using Qorpent.Scaffolding.Model;
using Qorpent.Scaffolding.Model.CodeWriters;

namespace Qorpent.Scaffolding.Tests.SqlAndOrm{
	[TestFixture]
	public class ExtendedModelGeneratorTests
	{
		[Test]
		public void SimplestModel()
		{
			var model = PersistentModel.Compile(@"
class a prototype=dbtable
");
			var code = new ExtendedModelWriter(model) { WithHeader = false }.ToString();
			Console.WriteLine(code.Replace("\"", "\"\""));
			Assert.AreEqual(@"
using System;
using Qorpent.Data;
using Qorpent.Log;
using Qorpent.IoC;
using Qorpent.Data.Connections;
using Qorpent.Data.DataCache;
using System.Linq;
using System.Collections.Generic;
using Orm.ObjectCaches;
namespace Orm.Adapters {
	///<summary>Model for Orm definition</summary>
	public partial class Model {

		private IUserLog _log;
		private IDatabaseConnectionProvider _connectionProvider;
		private IUserLog _sqlLog;
		///<summary>initiator for caches</summary>
		protected ObjectDataCache<T> InitCache<T>()where T:class,new(){
			var result = CreateCache<T>();
			result.Adapter = GetAdapter<T>();
			result.ConnectionProvider = ConnectionProvider; 
			result.Log = Log; 
			result.SqlLog = SqlLog;
			result.ConnectionString = ConnectionString;
			SetupLoadBehavior(result);
			return result;
		}
		///<summary>initiator for caches</summary>
		protected ObjectDataCache<T> CreateCache<T>() where T:class,new(){
			
if(typeof(T)==typeof(a))return (new aDataCache{Model=this}) as ObjectDataCache<T>;

			return null;
		}
		///<summary>
		///Sql connection descriptor
		///</summary>
		public string ConnectionString{get;set;}
		/// <summary>
		/// Служба генерации соединений
		/// </summary>
		[Inject]
		protected  IDatabaseConnectionProvider ConnectionProvider{
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
		private aDataCache _aCache;
		///<summary>Cache of a</summary>
		public aDataCache a {get { return _aCache ?? (_aCache = (aDataCache)InitCache<a>());}}
		///<summary>Setup auto load behavior for linked classes</summary>
		protected virtual void SetupLoadBehavior<T>(ObjectDataCache<T> cache)where T:class,new(){
			switch(typeof(T).Name){
				case ""a"" : SetupaLoadBehavior(cache as ObjectDataCache<a>);break;
				default: break;
			}
		}
		///<summary></summary>
		protected void SetupaLoadBehavior(ObjectDataCache<a> cache){
			cache.OnAfterUpdateCache+= (s,ids,c,ctx) => {
				var mycache = s as ObjectDataCache<a>;
				var targets = ids.Select(_=>mycache.Get(_,c)).ToArray();
				foreach(var t in targets){
					if(t.Id == -1||t.Id==0)continue;
				}
			};
		}
	}
}


".Trim(), code.Trim());
		}


		[Test]
		public void ModelWithReferencesAndAutoAndLazySetup()
		{
			var model = PersistentModel.Compile(@"
class a prototype=dbtable 
class b prototype=dbtable
	ref a reverse auto reverse-lazy
class c prototype=dbtable
	ref b reverse reverse-auto lazy
");
			var code = new ExtendedModelWriter(model) { WithHeader = false }.ToString();
			Console.WriteLine(code.Replace("\"", "\"\""));
			Assert.AreEqual(@"
using System;
using Qorpent.Data;
using Qorpent.Log;
using Qorpent.IoC;
using Qorpent.Data.Connections;C
using Qorpent.Data.DataCache;
using System.Linq;
using System.Collections.Generic;
using Orm.ObjectCaches;
namespace Orm.Adapters {
	///<summary>Model for Orm definition</summary>
	public partial class Model {

		private IUserLog _log;
		private IDatabaseConnectionProvider _connectionProvider;
		private IUserLog _sqlLog;
		///<summary>initiator for caches</summary>
		protected ObjectDataCache<T> InitCache<T>()where T:class,new(){
			var result = CreateCache<T>();
			result.Adapter = GetAdapter<T>();
			result.ConnectionProvider = ConnectionProvider; 
			result.Log = Log; 
			result.SqlLog = SqlLog;
			result.ConnectionString = ConnectionString;
			SetupLoadBehavior(result);
			return result;
		}
		///<summary>initiator for caches</summary>
		protected ObjectDataCache<T> CreateCache<T>() where T:class,new(){
			
if(typeof(T)==typeof(a))return (new aDataCache{Model=this}) as ObjectDataCache<T>;
if(typeof(T)==typeof(b))return (new bDataCache{Model=this}) as ObjectDataCache<T>;
if(typeof(T)==typeof(c))return (new cDataCache{Model=this}) as ObjectDataCache<T>;

			return null;
		}
		///<summary>
		///Sql connection descriptor
		///</summary>
		public string ConnectionString{get;set;}
		/// <summary>
		/// Служба генерации соединений
		/// </summary>
		[Inject]
		protected  IDatabaseConnectionProvider ConnectionProvider{
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
		private aDataCache _aCache;
		///<summary>Cache of a</summary>
		public aDataCache a {get { return _aCache ?? (_aCache = (aDataCache)InitCache<a>());}}
		private bDataCache _bCache;
		///<summary>Cache of b</summary>
		public bDataCache b {get { return _bCache ?? (_bCache = (bDataCache)InitCache<b>());}}
		private cDataCache _cCache;
		///<summary>Cache of c</summary>
		public cDataCache c {get { return _cCache ?? (_cCache = (cDataCache)InitCache<c>());}}
		///<summary>Setup auto load behavior for linked classes</summary>
		protected virtual void SetupLoadBehavior<T>(ObjectDataCache<T> cache)where T:class,new(){
			switch(typeof(T).Name){
				case ""a"" : SetupaLoadBehavior(cache as ObjectDataCache<a>);break;
				case ""b"" : SetupbLoadBehavior(cache as ObjectDataCache<b>);break;
				case ""c"" : SetupcLoadBehavior(cache as ObjectDataCache<c>);break;
				default: break;
			}
		}
		///<summary></summary>
		protected void SetupaLoadBehavior(ObjectDataCache<a> cache){
			cache.OnAfterUpdateCache+= (s,ids,c,ctx) => {
				var mycache = s as ObjectDataCache<a>;
				var targets = ids.Select(_=>mycache.Get(_,c)).ToArray();
				foreach(var t in targets){
					if(t.Id == -1||t.Id==0)continue;
				}
				if (ids.Count > 0 && !(ids.Count == 1 && (ids[0] == 0 || ids[0]==-1))){
					var inids = string.Join("","",ids.Where(_=>_!=0 && _!=-1));
					if(AutoLoadabs){
						var q = string.Format(""(a in ({0}))"",inids);
						if(Lazyabs){
							foreach(var t in targets){
								if(t.Id == -1||t.Id==0)continue;
								t.bs= new ObjectDataCacheBindLazyList<b>{Query=q,Cache=this.b};
							}
						}else{
							var nestIds = this.b.UpdateSingleQuery(q, ctx,c, null, true);
							foreach(var nid in nestIds){
								var n=this.b.Get(nid);
								var t=cache.Get(n.aId);
								t.bs.Add(n);
							}
						}
					}
				}
			};
		}
		///<summary></summary>
		protected void SetupbLoadBehavior(ObjectDataCache<b> cache){
			cache.OnAfterUpdateCache+= (s,ids,c,ctx) => {
				var mycache = s as ObjectDataCache<b>;
				var targets = ids.Select(_=>mycache.Get(_,c)).ToArray();
				foreach(var t in targets){
					if(t.Id == -1||t.Id==0)continue;
					if(AutoLoadba && null==t.a){
						t.a= (!Lazyba?(this.a.Get(t.aId,c)): new a.Lazy{GetLazy=_=>this.a.Get(t.aId)});
if (!Lazyabs && !Lazyba && t.aId != 0 && t.aId != -1)
			{
				if (!t.a.bs.Contains(t))
				{
					t.a.bs.Add(t);
				}
			}
					}
				}
				if (ids.Count > 0 && !(ids.Count == 1 && (ids[0] == 0 || ids[0]==-1))){
					var inids = string.Join("","",ids.Where(_=>_!=0 && _!=-1));
					if(AutoLoadbcs){
						var q = string.Format(""(b in ({0}))"",inids);
						if(Lazybcs){
							foreach(var t in targets){
								if(t.Id == -1||t.Id==0)continue;
								t.cs= new ObjectDataCacheBindLazyList<c>{Query=q,Cache=this.c};
							}
						}else{
							var nestIds = this.c.UpdateSingleQuery(q, ctx,c, null, true);
							foreach(var nid in nestIds){
								var n=this.c.Get(nid);
								var t=cache.Get(n.bId);
								t.cs.Add(n);
							}
						}
					}
				}
			};
		}
		///<summary></summary>
		protected void SetupcLoadBehavior(ObjectDataCache<c> cache){
			cache.OnAfterUpdateCache+= (s,ids,c,ctx) => {
				var mycache = s as ObjectDataCache<c>;
				var targets = ids.Select(_=>mycache.Get(_,c)).ToArray();
				foreach(var t in targets){
					if(t.Id == -1||t.Id==0)continue;
					if(AutoLoadcb && null==t.b){
						t.b= (!Lazycb?(this.b.Get(t.bId,c)): new b.Lazy{GetLazy=_=>this.b.Get(t.bId)});
if (!Lazybcs && !Lazycb && t.bId != 0 && t.bId != -1)
			{
				if (!t.b.cs.Contains(t))
				{
					t.b.cs.Add(t);
				}
			}
					}
				}
			};
		}
	}
}
".Trim(), code.Trim());
		}
	}
}