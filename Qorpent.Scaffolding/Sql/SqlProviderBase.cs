using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Qorpent.BSharp.Builder;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Sql{
	internal abstract class SqlProviderBase : ISqlProvider{
		protected const int DEFAULTSTRINGSIZE =255;
		protected const int DEFAULTDECIMALSIZE = 18;
		protected const int DEFAULTDECIMALPRECESSION = 6;
		public abstract string GetSql(DbObject dbObject, DbGenerationMode mode, object hintObject);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="objects"></param>
		/// <returns></returns>
		protected DbObject[] GetOrderedObjects(IEnumerable<DbObject> objects){
			foreach (var o in objects){
				o.UpgadeRank();
			}
			var result = objects.OrderBy(_ => _.ObjectType).ThenBy(_=>_.Rank).ToArray();
			var tables = result.OfType<DbTable>().ToArray();
			foreach (var dbTable in tables){
				dbTable.CheckLateReferences(tables);
			}
			return result;
		}

		protected abstract void CheckScriptDelimiter(DbGenerationMode mode, StringBuilder sb);
		protected abstract string GetEnsureSchema(string schema,DbGenerationMode mode);
		
		bool CheckCond(IBSharpProject p, DbObjectType t){
			if (p.IsSet("DO_" + t.ToString().ToUpperInvariant())) return true;
			if (p.IsSet("NO_ANY")) return false;
			if (p.IsSet("NO_" + t.ToString().ToUpperInvariant())) return false;
			return true;
		}
		bool CheckCond(IBSharpProject p, DbObject o){
			var t = o.ObjectType;
			if (p.IsSet("DO_" + t.ToString().ToUpperInvariant())) return true;
			if (p.IsSet("NO_ANY")) return false;
			if (p.IsSet("NO_" + t.ToString().ToUpperInvariant())) return false;
			if (p.IsSet("NAMEREGEX")){
				if (!Regex.IsMatch(o.FullName,p.GetCondition("NAMEREGEX"))) return false;
			}
			return true;
		}
		public virtual string GetSql(IEnumerable<DbObject> objects, DbGenerationMode mode, object hintObject){
			var ibsp = hintObject as IBSharpProject;
			
			var schemas = objects.Select(_ => _.Schema).Distinct();
			var sequencefields =
				objects.OfType<DbTable>()
				       .Where(_ => _.Fields.Values.Any(__ => __.IsIdentity))
				       .Select(_ => _.Fields.Values.First(__ => __.IsIdentity)).ToArray();
			var ordered = GetOrderedObjects(objects);
			var sb = new StringBuilder();

			if (mode.HasFlag(DbGenerationMode.Drop)){

				if (CheckCond(ibsp,DbObjectType.Table)){
					var lates = ordered.OfType<DbTable>().SelectMany(_ => _.Fields.Values).Where(_ => _.IsLateRef).ToArray();
					DropLateRefs(sb, lates);
				}
				ordered = ordered.Reverse().ToArray();
				foreach (var dbObject in ordered){
					RenderObject(mode, hintObject, dbObject, ibsp, sb);
				}
				if (CheckCond(ibsp, DbObjectType.Sequence))
				{
					foreach (var fld in sequencefields){
						sb.AppendLine(GetDropSequence(fld));
					}
				}
				if (CheckCond(ibsp, DbObjectType.Schema))
				{
					foreach (var schema in schemas){
						sb.AppendLine(this.GetDropSchema(schema));
						CheckScriptDelimiter(mode, sb);
					}
				}
			}
			else{
				GenerateTemporalUtils(sb, mode, hintObject);
				if (CheckCond(ibsp, DbObjectType.Schema)){
					sb.AppendLine(string.Join("\r\nGO\r\n", schemas.Select(_ => GetEnsureSchema(_, mode))));
				}
				if (CheckCond(ibsp, DbObjectType.Sequence)){
					sb.AppendLine(string.Join("\r\nGO\r\n", sequencefields.Select(GetEnsureSequence)));
					CheckScriptDelimiter(mode, sb);
				}

				foreach (var dbObject in ordered){
					RenderObject(mode,hintObject,dbObject,ibsp,sb);
				}

				if (CheckCond(ibsp, DbObjectType.Table)){
					GenerateConstraints(ordered, sb, mode, hintObject);
				}


				DropTemporalUtils(sb, mode, hintObject);
			}
			return sb.ToString();
		}

		private void RenderObject(DbGenerationMode mode, object hintObject, DbObject dbObject, IBSharpProject ibsp,
		                          StringBuilder sb){

			if (!CheckCond(ibsp, dbObject)) return;
			var script = GetSql(dbObject, mode, hintObject);
			if (!string.IsNullOrWhiteSpace(script)){
				sb.AppendLine(script);
				CheckScriptDelimiter(mode, sb);
			}
		}

		/// <summary>
		/// Очистка от ранних референсов
		/// </summary>
		/// <param name="sb"></param>
		/// <param name="lates"></param>
		protected abstract void DropLateRefs(StringBuilder sb, DbField[] lates);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dbField"></param>
		/// <returns></returns>
		protected abstract string GetEnsureSequence(DbField dbField);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fld"></param>
		/// <returns></returns>
		protected abstract string GetDropSequence(DbField fld);


		/// <summary>
		/// 
		/// </summary>
		/// <param name="schema"></param>
		/// <returns></returns>
		protected abstract string GetDropSchema(string schema);
		


		/// <summary>
		/// 
		/// </summary>
		/// <param name="ordered"></param>
		/// <param name="sb"></param>
		/// <param name="mode"></param>
		/// <param name="hintObject"></param>
		protected abstract void GenerateConstraints(DbObject[] ordered, StringBuilder sb, DbGenerationMode mode, object hintObject);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sb"></param>
		/// <param name="mode"></param>
		/// <param name="hintObject"></param>
		protected abstract void DropTemporalUtils(StringBuilder sb, DbGenerationMode mode, object hintObject);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sb"></param>
		/// <param name="mode"></param>
		/// <param name="hintObject"></param>
		protected abstract void GenerateTemporalUtils(StringBuilder sb, DbGenerationMode mode, object hintObject);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dataType"></param>
		/// <returns></returns>
		public abstract string GetSql(DbDataType dataType);
	}
}