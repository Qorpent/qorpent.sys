using System.IO;
using System.Linq;

namespace Qorpent.Scaffolding.Model.CodeWriters{
	/// <summary>
	///     Generates extended model
	/// </summary>
	public class ExtendedModelWriter : CodeWriterBase{
		/// <summary>
		/// </summary>
		/// <param name="model"></param>
		/// <param name="output"></param>
		public ExtendedModelWriter(PersistentModel model, TextWriter output = null) : base(model, output){
		}

		/// <summary>
		/// </summary>
		protected override void InternalRun(){
			WriteHeader();
			o.WriteLine("using System;");
			o.WriteLine("using Qorpent.Data;");
			o.WriteLine("using Qorpent.Log;");
			o.WriteLine("using Qorpent.IoC;");
			o.WriteLine("using Qorpent.Data.Connections;");
			o.WriteLine("using Qorpent.Data.DataCache;");
			o.WriteLine("using System.Linq;");
			o.WriteLine("using System.Collections.Generic;");
			o.Write("namespace {0}.Adapters {{\r\n", DefaultNamespce);
			o.WriteLine("\t///<summary>Model for " + DefaultNamespce + " definition</summary>");
			o.WriteLine("\tpublic partial class Model {");
			GenerateSupportProperties();
			GenerateCacheInstances();
			GenerateSetupLinksMethod();
			o.WriteLine("\t}");
			o.WriteLine("}");
		}

		private void GenerateSetupLinksMethod(){
			o.WriteLine("\t\t///<summary>Setup auto load behavior for linked classes</summary>");
			o.WriteLine("\t\tprotected virtual void SetupLoadBehavior<T>(ObjectDataCache<T> cache)where T:class,new(){");
			o.WriteLine("\t\t\tswitch(typeof(T).Name){");
			foreach (PersistentClass t in Tables){
				o.WriteLine("\t\t\t\tcase \"" + t.Name + "\" : Setup" + t.Name + "LoadBehavior(cache as ObjectDataCache<" + t.Name +
				            ">);break;");
			}
			o.WriteLine("\t\t\t\tdefault: break;");
			o.WriteLine("\t\t\t}");
			o.WriteLine("\t\t}");

			foreach (PersistentClass t in Tables){
				Field[] ownrefs = t.GetOrderedFields().Where(_ => _.IsReference && !_.NoCode && !_.NoSql).ToArray();
				Field[] incomes =
					t.GetOrderedReverse()
					 .Where(_ => _.IsReverese && !_.NoCode && !_.NoSql)
					 .OrderBy(_ => _.Idx)
					 .ThenBy(_ => _.Name)
					 .ToArray();
				o.WriteLine("\t\t///<summary></summary>");
				o.WriteLine("\t\tprotected void Setup" + t.Name + "LoadBehavior(ObjectDataCache<" + t.Name + "> cache){");
				o.WriteLine("\t\t\tcache.OnAfterUpdateCache+= (s,ids,c,ctx) => {");
				o.WriteLine("\t\t\t\tvar mycache = s as ObjectDataCache<" + t.Name + ">;");
				o.WriteLine("\t\t\t\tvar targets = ids.Select(_=>mycache.Get(_,c)).ToArray();");
				o.WriteLine("\t\t\t\tforeach(var t in targets){");
				o.WriteLine("\t\t\t\t\tif(t.Id == -1||t.Id==0)continue;");
				foreach (Field element in ownrefs){
					GenerateSetupOwnRef(element);
				}
				o.WriteLine("\t\t\t\t}");
				if (incomes.Length != 0){
					o.WriteLine("\t\t\t\tif (ids.Count > 0 && !(ids.Count == 1 && (ids[0] == 0 || ids[0]==-1))){");
					o.WriteLine("\t\t\t\t\tvar inids = string.Join(\",\",ids.Where(_=>_!=0 && _!=-1));");
					foreach (Field source in incomes.OrderBy(_ => _.Name).ToArray()){
						GenerateSetupCollection(source);
					}
					o.WriteLine("\t\t\t\t}");
				}
				o.WriteLine("\t\t\t};");
				o.WriteLine("\t\t}");
			}
		}

		private void GenerateSetupCollection(Field f){
			string alprop = "AutoLoad" + f.ReferenceClass.Name + f.ReverseCollectionName;

			o.WriteLine("\t\t\t\t\tif(" + alprop + "){");

			o.WriteLine("\t\t\t\t\t\tvar q = string.Format(\"(" + f.Name + " in ({0}))\",inids);");
			string cache =
				"this." + f.Table.TargetClass.Name;
			if (f.Name == "Parent"){
				cache = "cache";
			}
			o.WriteLine("\t\t\t\t\t\tif(Lazy" + f.ReferenceClass.Name + f.ReverseCollectionName + "){");
			o.WriteLine("\t\t\t\t\t\t\tforeach(var t in targets){");
			o.WriteLine("\t\t\t\t\t\t\t\tif(t.Id == -1||t.Id==0)continue;");
			o.WriteLine("\t\t\t\t\t\t\t\tt." + f.ReverseCollectionName + "= new ObjectDataCacheBindLazyList<" +
			            f.Table.FullCodeName + ">{Query=q,Cache=" + cache + "};");
			o.WriteLine("\t\t\t\t\t\t\t}");
			o.WriteLine("\t\t\t\t\t\t}else{");
			o.WriteLine("\t\t\t\t\t\t\tvar nestIds = " + cache + ".UpdateSingleQuery(q, ctx,c, null, true);");
			o.WriteLine("\t\t\t\t\t\t\tforeach(var nid in nestIds){");
			o.WriteLine("\t\t\t\t\t\t\t\tvar n=" + cache + ".Get(nid);");
			o.WriteLine("\t\t\t\t\t\t\t\tvar t=cache.Get(n." + f.Name + "Id);");
			o.WriteLine("\t\t\t\t\t\t\t\tt." + f.ReverseCollectionName + ".Add(n);");
			o.WriteLine("\t\t\t\t\t\t\t}");
			o.WriteLine("\t\t\t\t\t\t}");
			o.WriteLine("\t\t\t\t\t}");
		}

		private void GenerateSetupOwnRef(Field f){
			o.WriteLine("\t\t\t\t\tif(AutoLoad" + f.Table.TargetClass.Name + f.Name + " && null==t." + f.Name +
			            (f.Name.ToLowerInvariant() == "id" ? (" && -1 != t." + f.Name + f.ReferenceField) : "") + "){");
			o.WriteLine("\t\t\t\t\t\tt." + f.Name + "= (!Lazy" + f.Table.TargetClass.Name + f.Name + "?(this." +
			            f.ReferenceClass.Name + ".Get(t." + f.Name + f.ReferenceField + ",c)): new " + f.ReferenceClass.Name +
			            ".Lazy{GetLazy=_=>this." + f.ReferenceClass.Name + ".Get(t." + f.Name + f.ReferenceField + ")});");
			o.WriteLine("\t\t\t\t\t}");
		}

		private void GenerateSupportProperties(){
			o.WriteLine(@"
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
		}");
		}

		private void GenerateCacheInstances(){
			foreach (PersistentClass table in Tables){
				string n = table.Name;
				if (DefaultNamespce != table.Namespace && !string.IsNullOrWhiteSpace(table.Namespace)){
					n = table.Namespace + "." + n;
				}
				o.WriteLine("\t\tprivate ObjectDataCache<" + n + "> _" + table.Name + "Cache;");
				o.WriteLine("\t\t///<summary>Cache of " + table.Name + "</summary>");
				o.WriteLine(
					string.Format(
						"\t\tpublic ObjectDataCache<{0}> {1} {{get {{ return _{1}Cache ?? (_{1}Cache = InitCache<{0}>());}}}}", n,
						table.Name));
			}
		}
	}
}