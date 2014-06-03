using Qorpent.Scaffolding.Model.SqlObjects;

namespace Qorpent.Scaffolding.Model.SqlWriters{
	/// <summary>
	/// 
	/// </summary>
	public class ScriptWriter : SqlCommandWriter{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="script"></param>
		public ScriptWriter(SqlScript script){
			this.Script = script;
			this.Parameters = script;
		}
		/// <summary>
		/// 
		/// </summary>
		public SqlScript Script { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override string GetText(){
			return "${Text}";
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override string GetDigestFinisher(){
			return "Script " + Script.Name;
		}
	}
}