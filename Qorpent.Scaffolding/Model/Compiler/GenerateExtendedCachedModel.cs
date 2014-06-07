﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Model.Compiler{
	/// <summary>
	/// Формирует вспомогательный класс адаптера для DataReader
	/// </summary>
	public class GenerateExtendedCachedModel : CSharpModelGeneratorBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetclasses"></param>
		/// <returns></returns>
		protected override IEnumerable<Production> InternalGenerate(IBSharpClass[] targetclasses){

			var genfactory = new Production{
				FileName = "Adapters/Model_Caches.cs",
				GetContent = ()=>GenerateModelClass()
			};
			yield return genfactory;
		}

		private string GenerateModelClass(){
			o = new StringBuilder();
			o.AppendLine(Header);
			o.AppendLine("using System;");
			o.AppendLine("using Qorpent.Data;");
			o.AppendLine("using Qorpent.Log;");
			o.AppendLine("using Qorpent.IoC;");
			o.AppendLine("using Qorpent.Data.Connections;");
			o.AppendLine("using Qorpent.Data.DataCache;");
			o.AppendLine("using System.Linq;");
			o.AppendLine("using System.Collections.Generic;");
			o.AppendFormat("namespace {0}.Adapters {{\r\n", DefaultNamespce);
			o.AppendLine("\t///<summary>Model for " + DefaultNamespce + " definition</summary>");
			o.AppendLine("\tpublic partial class Model {");
			GenerateSupportProperties();
			GenerateCacheInstances();
			GenerateSetupLinksMethod();
			o.AppendLine("\t}");
			o.AppendLine("}");
			return o.ToString();
		}

		private void GenerateSetupLinksMethod(){
			o.AppendLine("\t\t///<summary>Setup auto load behavior for linked classes</summary>");
			o.AppendLine("\t\tprotected virtual void SetupLoadBehavior<T>(ObjectDataCache<T> cache)where T:class,new(){");
			o.AppendLine("\t\t\tswitch(typeof(T).Name){");
			foreach (var t in Tables){
				o.AppendLine("\t\t\t\tcase \"" + t.Name + "\" : Setup" + t.Name + "LoadBehavior(cache as ObjectDataCache<"+t.Name+">);break;");
			}
			o.AppendLine("\t\t\t\tdefault: break;");
			o.AppendLine("\t\t\t}");
			o.AppendLine("\t\t}");

			foreach (var t in Tables){
				var ownrefs = t.GetOrderedFields().Where(_ => _.IsReference && !_.NoCode && !_.NoSql).ToArray();
				var incomes = t.GetOrderedReverse().Where(_ => _.IsReverese && !_.NoCode && !_.NoSql).OrderBy(_ => _.Idx).ThenBy(_ => _.Name).ToArray();
				o.AppendLine("\t\t///<summary></summary>");
				o.AppendLine("\t\tprotected void Setup" + t.Name + "LoadBehavior(ObjectDataCache<"+t.Name+"> cache){");
				o.AppendLine("\t\t\tcache.OnAfterUpdateCache+= (s,ids,c,ctx) => {");
				o.AppendLine("\t\t\t\tvar mycache = s as ObjectDataCache<"+t.Name+">;");
				o.AppendLine("\t\t\t\tvar targets = ids.Select(_=>mycache.Get(_,c)).ToArray();");
				o.AppendLine("\t\t\t\tforeach(var t in targets){");
				o.AppendLine("\t\t\t\t\tif(t.Id == -9999||t.Id==0)continue;");
				foreach (var element in ownrefs){
					GenerateSetupOwnRef(element);
				}
				o.AppendLine("\t\t\t\t}");
				if (incomes.Length != 0)
				{
					o.AppendLine("\t\t\t\tif (ids.Count > 0 && !(ids.Count == 1 && (ids[0] == 0 || ids[0]==-9999))){");
					o.AppendLine("\t\t\t\t\tvar inids = string.Join(\",\",ids.Where(_=>_!=0 && _!=-9999));");
					foreach (var source in incomes.OrderBy(_=>_.Name).ToArray()){
						GenerateSetupCollection(source);
					}
					o.AppendLine("\t\t\t\t}");
				}
				o.AppendLine("\t\t\t};");
				o.AppendLine("\t\t}");
			}
		}

		private void GenerateSetupCollection(Field f){
			
			var alprop = "AutoLoad" + f.ReferenceClass.Name+ f.ReverseCollectionName;

			o.AppendLine("\t\t\t\t\tif(" + alprop + "){");

			o.AppendLine("\t\t\t\t\t\tvar q = string.Format(\"(" + f.Name + " in ({0}))\",inids);");
			var cache = 
				"this."+f.Table.TargetClass.Name;
			if (f.Name == "Parent"){
				cache = "cache";
			}
			o.AppendLine("\t\t\t\t\t\tif(Lazy" + f.ReferenceClass.Name+ f.ReverseCollectionName + "){");
			o.AppendLine("\t\t\t\t\t\t\tforeach(var t in targets){");
			o.AppendLine("\t\t\t\t\t\t\t\tif(t.Id == -9999||t.Id==0)continue;");
			o.AppendLine("\t\t\t\t\t\t\t\tt." + f.ReverseCollectionName + "= new ObjectDataCacheBindLazyList<"+f.Table.FullCodeName+">{Query=q,Cache="+cache+"};");
			o.AppendLine("\t\t\t\t\t\t\t}");
			o.AppendLine("\t\t\t\t\t\t}else{");
			o.AppendLine("\t\t\t\t\t\t\tvar nestIds = " + cache + ".UpdateSingleQuery(q, ctx,c, null, true);");
			o.AppendLine("\t\t\t\t\t\t\tforeach(var nid in nestIds){");
			o.AppendLine("\t\t\t\t\t\t\t\tvar n=" + cache + ".Get(nid);");
			o.AppendLine("\t\t\t\t\t\t\t\tvar t=cache.Get(n."+f.Name+"Id);");
			o.AppendLine("\t\t\t\t\t\t\t\tt."+f.ReverseCollectionName+".Add(n);");
			o.AppendLine("\t\t\t\t\t\t\t}");
			o.AppendLine("\t\t\t\t\t\t}");
			o.AppendLine("\t\t\t\t\t}");

		}

		private void GenerateSetupOwnRef(Field f){
			o.AppendLine("\t\t\t\t\tif(AutoLoad" + f.Table.TargetClass.Name + f.Name + " && null==t." + f.Name +(f.Name.ToLowerInvariant()=="id"? (" && -9999 != t."+f.Name+f.ReferenceField):"")+   "){");
			o.AppendLine("\t\t\t\t\t\tt." + f.Name + "= (!Lazy" + f.Table.TargetClass.Name+f.Name + "?(this." + f.ReferenceClass.Name + ".Get(t." + f.Name + f.ReferenceField + ",c)): new " + f.ReferenceClass.Name + ".Lazy{GetLazy=_=>this." + f.ReferenceClass.Name + ".Get(t." + f.Name + f.ReferenceField + ")});");
			o.AppendLine("\t\t\t\t\t}");
		}

		private void GenerateSupportProperties(){
			o.AppendLine(@"
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
			foreach (var table in Tables){
				var n = table.Name;
				if (DefaultNamespce != table.Namespace)
				{
					n = table.Namespace + "." + n;
				}
				o.AppendLine("\t\tprivate ObjectDataCache<" + n + "> _" + table.Name + "Cache;");
				o.AppendLine("\t\t///<summary>Cache of " + table.Name + "</summary>");
				o.AppendLine(
					string.Format(
						"\t\tpublic ObjectDataCache<{0}> {1} {{get {{ return _{1}Cache ?? (_{1}Cache = InitCache<{0}>());}}}}", n,
						table.Name));
			}
		}
	}
}