using System.Collections.Generic;
using System.Linq;
using System.Text;

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
			return result;
		}

		class DBObjectComparer : IComparer<DbObject>{
			public int Compare(DbObject x, DbObject y){
				if (x.InDependency.Contains(y)) return 1;
				if (y.InDependency.Contains(x)) return -1;
				if (x.ObjectType != y.ObjectType){
					return x.ObjectType.CompareTo(y.ObjectType);	
				}
				return x.InDependency.Count.CompareTo(y.InDependency.Count);
			}
		}

		protected abstract void CheckScriptDelimiter(DbGenerationMode mode, StringBuilder sb);
		protected abstract string GetEnsureSchema(string schema,DbGenerationMode mode);
		

		public virtual string GetSql(IEnumerable<DbObject> objects, DbGenerationMode mode, object hintObject){
			var schemas = objects.Select(_ => _.Schema).Distinct();
			var sequencefields =
				objects.OfType<DbTable>()
				       .Where(_ => _.Fields.Values.Any(__ => __.IsIdentity))
				       .Select(_ => _.Fields.Values.First(__ => __.IsIdentity)).ToArray();
			var ordered = GetOrderedObjects(objects);
			var sb = new StringBuilder();

			if (mode.HasFlag(DbGenerationMode.Drop)){
				ordered = ordered.Reverse().ToArray();
				foreach (var dbObject in ordered)
				{
					sb.AppendLine(GetSql(dbObject, mode, hintObject));
					CheckScriptDelimiter(mode, sb);
				}
				foreach (var fld in sequencefields){
					sb.AppendLine(GetDropSequence(fld));
				}
				foreach (var schema in schemas){
					sb.AppendLine(this.GetDropSchema(schema));
					CheckScriptDelimiter(mode,sb);
				}
			}
			else{
				GenerateTemporalUtils(sb, mode, hintObject);
				sb.AppendLine(string.Join("\r\nGO\r\n", schemas.Select(_ => GetEnsureSchema(_, mode))));
				sb.AppendLine(string.Join("\r\nGO\r\n", sequencefields.Select(GetEnsureSequence)));
				CheckScriptDelimiter(mode, sb);

				foreach (var dbObject in ordered){
					sb.AppendLine(GetSql(dbObject, mode, hintObject));
					CheckScriptDelimiter(mode, sb);
				}
				GenerateConstraints(ordered, sb, mode, hintObject);
				DropTemporalUtils(sb, mode, hintObject);
			}
			return sb.ToString();
		}
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