using System;
using System.Collections.Generic;
using System.IO;
using Qorpent.Scaffolding.Model.SqlObjects;
using Qorpent.Scaffolding.Model.SqlWriters;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Model{
	/// <summary>
	/// </summary>
	public abstract class SqlCommandWriter{
		/// <summary>
		///     Диалект
		/// </summary>
		public SqlDialect Dialect { get; set; }

		/// <summary>
		///     Комментарий
		/// </summary>
		public string Comment { get; set; }

		/// <summary>
		///     Сам текст команды
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		///     Источник параметров для интерполяцииы
		/// </summary>
		public object Parameters { get; set; }

		/// <summary>
		///     Опциональная команда, форсирует выполнение в режиме try/catch
		/// </summary>
		public bool Optional { get; set; }

		/// <summary>
		///     Режим команды (для некоторых команд)
		/// </summary>
		public ScriptMode Mode { get; set; }

		/// <summary>
		///     Исходящий поток
		/// </summary>
		protected TextWriter Output { get; set; }

		/// <summary>
		///     Отключение комментария
		/// </summary>
		public bool NoComment { get; set; }

		/// <summary>
		///     Отлючение разделителей
		/// </summary>
		public bool NoDelimiter { get; set; }

		/// <summary>
		/// </summary>
		public PersistentModel Model { get; set; }

		/// <summary>
		///     Записывает команду в исходящий поток
		/// </summary>
		/// <param name="output"></param>
		public void Write(TextWriter output){
			Output = output;
			WriteComment();
			WriteCommand(GetText(), Optional);


		}

		/// <summary>
		///     Метод получения основного текста команды
		/// </summary>
		/// <returns></returns>
		protected abstract string GetText();

		/// <summary>
		/// Создает типизированный врайтер
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static SqlCommandWriter Create(SqlObject obj){
			if(obj is SqlFunction)return new SqlFunctionWriter(obj as SqlFunction);
			if(obj is SqlView)return new SqlFunctionWriter(obj as SqlFunction);
			if(obj is SqlTrigger)return new SqlFunctionWriter(obj as SqlFunction);
			throw new NotSupportedException("not supported type "+obj.GetType().FullName);
		}

		/// <summary>
		/// </summary>
		/// <param name="command"></param>
		/// <param name="optional"></param>
		protected virtual void WriteCommand(string command, bool optional = false){
			if (!string.IsNullOrWhiteSpace(command)){
				if (Optional){
					WriteStartOptionalBlock();
				}
				if (null == Parameters){
					Output.WriteLine(command);
				}
				else{
					var i = new StringInterpolation();
					Output.WriteLine(i.Interpolate(command, Parameters));
				}
				if (Optional){
					WriteEndOptionalBlock();
				}
				WriteDelimiter();
			}
		}

		private void WriteEndOptionalBlock(){
			if (Dialect == SqlDialect.SqlServer){
				Output.WriteLine("end try begin catch print ERROR_MESSAGE() end catch");
			}
			else if (Dialect == SqlDialect.PostGres){
				Output.WriteLine("EXCEPTION WHEN OTHERS THEN raise notice '% %', SQLERRM, SQLSTATE; END;");
			}
		}

		private void WriteStartOptionalBlock(){
			if (Dialect == SqlDialect.SqlServer){
				Output.WriteLine("begin try");
			}
			else if (Dialect == SqlDialect.PostGres){
				Output.WriteLine("BEGIN");
			}
		}

		/// <summary>
		/// </summary>
		protected virtual void WriteComment(){
			if (NoComment) return;
			var comment = "begin command " + GetType().Name;
			if (!string.IsNullOrWhiteSpace(comment)){
				comment += "\r\n" + Comment;
			}
			IList<string> lines = comment.SmartSplit(false, true, '\r', '\n');
			foreach (string line in lines){
				Output.WriteLine("-- " + line);
			}
		}

		/// <summary>
		/// </summary>
		protected void WriteDelimiter(){
			if (NoDelimiter) return;
			if (Dialect == SqlDialect.SqlServer){
				Output.WriteLine("GO");
			}
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override string ToString(){
			var sw = new StringWriter();
			Write(sw);
			return sw.ToString();
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public string GetDigest(){
			string info = GetDigestFinisher();
			if (!string.IsNullOrWhiteSpace(info)){
				string comment = Comment;
				if (!string.IsNullOrWhiteSpace(comment)){
					comment = ", " + comment.Replace("\r","").Replace("\n","; ");
				}
				return info + " (" + Mode.ToString()[0] + "," + Dialect.ToString()[0] + (Optional ? ",O" : ",R") + comment + ")";
			}
			return "";
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		protected virtual string GetDigestFinisher(){
			return GetType().Name;
		}
	}
}