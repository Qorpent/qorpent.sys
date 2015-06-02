using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Data;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Model.SqlObjects{
	/// <summary>
	///     Описатель для скриптов
	/// </summary>
	public class SqlScript:SqlObject{
		/// <summary>
		/// </summary>
		public SqlScript(){
			SubScripts = new List<SqlScript>();
			Position = ScriptPosition.After;
			DbDialect = DbDialect.Ansi;
		}



		/// <summary>
		///     Текст скрипта
		/// </summary>
		public string Text { get; set; }


		/// <summary>
		///     Под-скрипты
		/// </summary>
		public IList<SqlScript> SubScripts { get; private set; }

		/// <summary>
		///     Позиция выполнения скрипта
		/// </summary>
		public ScriptPosition Position { get; set; }

		/// <summary>
		///     Режим применения скрипта (при создании или при удалении)
		/// </summary>
		public ScriptMode Mode { get; set; }

		/// <summary>
		///     Диалект, для которого применяется
		/// </summary>
		public DbDialect DbDialect { get; set; }

		/// <summary>
		///     Родительский скрипт
		/// </summary>
		public SqlScript Parent { get; set; }

		/// <summary>
		///     Директория скрипта
		/// </summary>
		public string Directory { get; set; }

		/// <summary>
		/// </summary>
		protected IBSharpClass MyClass { get; set; }


		/// <summary>
		///     Возвращает реальные скрипты н
		/// </summary>
		/// <param name="dialect"></param>
		/// <param name="position"></param>
		/// <param name="mode"></param>
		/// <returns></returns>
		public IEnumerable<SqlScript> GetRealScripts(DbDialect dialect, ScriptPosition position, ScriptMode mode){
			if (0 == SubScripts.Count){
				if (DbDialect == DbDialect.Ansi || dialect == DbDialect){
					if (Mode == mode){
						if (Position == position){
							yield return this;
						}
					}
				}
			}
			else{
				foreach (SqlScript subScript in SubScripts){
					foreach (SqlScript rs in subScript.GetRealScripts(dialect, position, mode)){
						yield return rs;
					}
				}
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="model"></param>
		/// <param name="cls"></param>
		/// <param name="definition"></param>
		/// <returns></returns>
		public SqlScript Setup(PersistentModel model, IBSharpClass cls, XElement definition){
			Model = model;
			MyClass = cls;
			Definition = definition;
			Name = definition.Attr("code");
			Comment = definition.Attr("name");
			External = definition.Attr("external");
			Position = definition.Attr("position", "After").To<ScriptPosition>();
			Mode = definition.Attr("mode", "Create").To<ScriptMode>();
			DbDialect = definition.Attr("dialect", "Ansi").To<DbDialect>();
			if (string.IsNullOrWhiteSpace(definition.Value) && string.IsNullOrWhiteSpace(External)){
				External = Name;
			}
			if (!string.IsNullOrWhiteSpace(External) && !External.EndsWith(".sql")){
				External += ".sql";
			}

			XElement[] subscripts = definition.Elements("script").ToArray();
			if (0 == subscripts.Length){
				
				if (string.IsNullOrWhiteSpace(External) ){
					Text = definition.Value;
				}
				else{
					Text = model.ResolveExternalContent(definition, External);
				}

			}
			else{
				foreach (XElement subscriptdef in definition.Elements("script")){
					var subscript = new SqlScript();
					subscript.Parent = this;
					subscript.Setup(Model, cls, subscriptdef);
					SubScripts.Add(subscript);
				}
			}
			return this;
		}
	}
}