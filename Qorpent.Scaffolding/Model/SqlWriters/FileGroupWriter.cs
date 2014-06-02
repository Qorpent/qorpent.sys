using Qorpent.Scaffolding.Model.SqlObjects;

namespace Qorpent.Scaffolding.Model.SqlWriters{
	/// <summary>
	/// Форматирование схемы
	/// </summary>
	public class FileGroupWriter : SqlCommandWriter
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="schema"></param>
		public FileGroupWriter(FileGroup schema)
		{
			FileGroup = schema;
			Parameters = FileGroup;
		}
		/// <summary>
		/// 
		/// </summary>
		public FileGroup FileGroup { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override string GetText()
		{
			if (Mode == ScriptMode.Create)
			{
				if (Dialect == SqlDialect.SqlServer)
				{
					return "exec __ensurefg @n='${Name}', @filecount=${FileCount}, @filesize=${FileSize}, @withidx=${WithIndexAsBit}, @isdefault=${IsDefaultAsBit}";
				}
				
				return "-- SCHEMAERROR: FileGroups are not implemented for "+Dialect;
			}
			return ""; //no drop behavior for FileGroups are applyble
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override string GetDigestFinisher(){
			return "FileGroup " + FileGroup.Name;
		}
	}
}