﻿using System.Collections.Generic;
using Qorpent.Scaffolding.Model.SqlObjects;
using Qorpent.Scaffolding.Model.SqlWriters;

namespace Qorpent.Scaffolding.Model{
	/// <summary>
	/// 
	/// </summary>
	public class SqlCommandWriterFactory{
		/// <summary>
		/// Целевая модель
		/// </summary>
		public PersistentModel Model { get; set; }
		/// <summary>
		/// Целевой диалект
		/// </summary>
		public SqlDialect Dialect { get; set; }
		/// <summary>
		/// Режим
		/// </summary>
		public ScriptMode Mode { get; set; }

		/// <summary>
		/// Конвертирует набор объектов в набор типизированных SqlWriter
		/// </summary>
		/// <param name="sources"></param>
		/// <returns></returns>
		public IEnumerable<SqlCommandWriter> Get(IEnumerable<object> sources){
			foreach (var source in sources){
				if (source is SqlScript){
					yield return new ScriptWriter(source as SqlScript){Dialect = Dialect, Mode = Mode, Model = Model};
				}else if (source is FileGroup){
					if(!Model.GenerationOptions.Supports(SqlObjectType.FileGroup))continue;
					if (Dialect != SqlDialect.SqlServer) continue;
					if(Mode!=ScriptMode.Create)continue;
					yield return new FileGroupWriter(source as FileGroup) { Dialect = Dialect, Mode = Mode, Model = Model};
				}else if (source is Schema){
					if (!Model.GenerationOptions.Supports(SqlObjectType.Schema)) continue;
					yield return new SchemaWriter(source as Schema) { Dialect = Dialect, Mode = Mode, Model = Model, Optional = true };
				}else if (source is Sequence){
					if (!Model.GenerationOptions.Supports(SqlObjectType.Sequence)) continue;
					yield return new SequenceWriter(source as Sequence) { Dialect = Dialect, Mode = Mode, Model = Model, Optional = true };
				}else if (source is PartitionDefinition){
					if (!Model.GenerationOptions.Supports(SqlObjectType.PartitionScheme)) continue;
					if (!Model.IsSupportPartitioning(Dialect)) continue;
					yield return new PartitionDefinitionWriter(source as PartitionDefinition) { Dialect = Dialect, Mode = Mode, Model = Model, Optional = true };
				}else if (source is PersistentClass){
					if (!Model.GenerationOptions.Supports(SqlObjectType.Table)) continue;
					yield return new TableWriter(source as PersistentClass) { Dialect = Dialect, Mode = Mode, Model = Model };
				}else if (source is Field){
					if (!Model.GenerationOptions.Supports(SqlObjectType.Table)) continue;
					yield return new LateForeignKeyWriter(source as Field) { Dialect = Dialect, Mode = Mode, Model = Model };
				}
			}
		}

	}
}