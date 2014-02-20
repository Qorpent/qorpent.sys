﻿using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qorpent.Data.BSharpDDL{
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
			return objects.OrderBy(_ => _, new DBObjectComparer()).ToArray();
		}

		class DBObjectComparer : IComparer<DbObject>{
			public int Compare(DbObject x, DbObject y){
				if (x.InDependency.Contains(y)) return 1;
				if (y.InDependency.Contains(x)) return -1;
				return x.ObjectType.CompareTo(y.ObjectType);
			}
		}

		protected abstract void CheckScriptDelimiter(DbGenerationMode mode, StringBuilder sb);
		protected abstract string GetEnsureSchema(string schema,DbGenerationMode mode);

		public virtual string GetSql(IEnumerable<DbObject> objects, DbGenerationMode mode, object hintObject){
			var schemas = objects.Select(_ => _.Schema).Distinct();
			var ordered = GetOrderedObjects(objects);
			var sb = new StringBuilder();
			GenerateTemporalUtils(sb, mode, hintObject);
			sb.AppendLine(string.Join("\r\n", schemas.Select(_=>GetEnsureSchema(_,mode))));
			CheckScriptDelimiter(mode, sb);
			foreach (var dbObject in ordered){
				sb.AppendLine(GetSql(dbObject, mode, hintObject));
				CheckScriptDelimiter(mode,sb);
			}
			GenerateConstraints(ordered,sb, mode, hintObject);
			DropTemporalUtils(sb, mode, hintObject);
			return sb.ToString();
		}

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

		protected abstract string GetSql(DbDataType dataType);
	}
}