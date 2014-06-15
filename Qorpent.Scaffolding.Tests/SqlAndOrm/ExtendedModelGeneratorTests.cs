﻿using System;
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
namespace Orm.Adapters {
	///<summary>Model for Orm definition</summary>
	public partial class Model {

		private IUserLog _log;
		private IDatabaseConnectionProvider _connectionProvider;
		private IUserLog _sqlLog;
		///<summary>initiator for caches</summary>
		protected ObjectDataCache<T> InitCache<T>()where T:class,new(){
			var result = new ObjectDataCache<T>{ Adapter = GetAdapter<T>(), ConnectionProvider  = ConnectionProvider, Log= Log, SqlLog= SqlLog, ConnectionString = ConnectionString };
			SetupLoadBehavior(result);
			return result;
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
		private ObjectDataCache<a> _aCache;
		///<summary>Cache of a</summary>
		public ObjectDataCache<a> a {get { return _aCache ?? (_aCache = InitCache<a>());}}
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
using Qorpent.Data.Connections;
using Qorpent.Data.DataCache;
using System.Linq;
using System.Collections.Generic;
namespace Orm.Adapters {
	///<summary>Model for Orm definition</summary>
	public partial class Model {

		private IUserLog _log;
		private IDatabaseConnectionProvider _connectionProvider;
		private IUserLog _sqlLog;
		///<summary>initiator for caches</summary>
		protected ObjectDataCache<T> InitCache<T>()where T:class,new(){
			var result = new ObjectDataCache<T>{ Adapter = GetAdapter<T>(), ConnectionProvider  = ConnectionProvider, Log= Log, SqlLog= SqlLog, ConnectionString = ConnectionString };
			SetupLoadBehavior(result);
			return result;
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
		private ObjectDataCache<a> _aCache;
		///<summary>Cache of a</summary>
		public ObjectDataCache<a> a {get { return _aCache ?? (_aCache = InitCache<a>());}}
		private ObjectDataCache<b> _bCache;
		///<summary>Cache of b</summary>
		public ObjectDataCache<b> b {get { return _bCache ?? (_bCache = InitCache<b>());}}
		private ObjectDataCache<c> _cCache;
		///<summary>Cache of c</summary>
		public ObjectDataCache<c> c {get { return _cCache ?? (_cCache = InitCache<c>());}}
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