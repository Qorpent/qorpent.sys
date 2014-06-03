using System.IO;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Model{
	/// <summary>
	/// 
	/// </summary>
	public class SqlCommandWriter{
		/// <summary>
		/// Диалект
		/// </summary>
		public SqlDialect Dialect { get; set; }
		/// <summary>
		/// Комментарий
		/// </summary>
		public string Comment { get; set; }
		/// <summary>
		/// Сам текст команды
		/// </summary>
		public string Text { get; set; }
		/// <summary>
		/// Источник параметров для интерполяцииы
		/// </summary>
		public object Parameters { get; set; }
		/// <summary>
		/// Опциональная команда, форсирует выполнение в режиме try/catch
		/// </summary>
		public bool Optional { get; set; }

		/// <summary>
		/// Режим команды (для некоторых команд)
		/// </summary>
		public ScriptMode Mode { get; set; }
		/// <summary>
		/// Для ряда команд - пердварительная подготовка, например предварительный дроп процедуры перед созданием
		/// </summary>
		public bool WithPrepare { get; set; }
		/// <summary>
		/// Признак необходимости команды подчистки по итогам основной команды
		/// </summary>
		public bool WithCleanup { get; set; }
		/// <summary>
		/// Записывает команду в исходящий поток
		/// </summary>
		/// <param name="output"></param>
		public void Write(TextWriter output){
			Output = output;
			WriteComment();
			
			if (WithPrepare){
				WriteCommand(GetPrepareText());
			}
			
			WriteCommand(GetText(),Optional);
			
			if (WithCleanup){
				WriteCommand(GetCleanupText());
			}
			
		}
		/// <summary>
		/// Метод получения основного текста команды
		/// </summary>
		/// <returns></returns>
		protected virtual string GetText(){
			return Text;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected virtual string GetPrepareText(){
			return "";
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected virtual string GetCleanupText()
		{
			return "";
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="command"></param>
		/// <param name="optional"></param>
		protected virtual void WriteCommand(string command, bool optional=false){
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
			}else if (Dialect == SqlDialect.PostGres){
				Output.WriteLine("EXCEPTION WHEN OTHERS THEN raise notice '% %', SQLERRM, SQLSTATE; END;");
			}
		}

		private void WriteStartOptionalBlock(){
			if (Dialect == SqlDialect.SqlServer){
				Output.WriteLine("begin try");
			}else if (Dialect == SqlDialect.PostGres){
				Output.WriteLine("BEGIN");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void WriteComment(){
			if (NoComment || string.IsNullOrWhiteSpace(Comment)) return;
			if (string.IsNullOrWhiteSpace(Comment)){
				Comment = "begin command " + this.GetType().Name;
			}
			var lines = Comment.SmartSplit(false, true, '\r', '\n');
			foreach (var line in lines){
				Output.WriteLine("-- "+line);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		protected void WriteDelimiter(){
			if (NoDelimiter) return;
			if (Dialect == SqlDialect.SqlServer){
				Output.WriteLine("GO");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString(){
			var sw = new StringWriter();
			Write(sw);
			return sw.ToString();
		}
		/// <summary>
		/// Исходящий поток
		/// </summary>
		protected TextWriter Output { get; set; }

		/// <summary>
		/// Отключение комментария
		/// </summary>
		public bool NoComment { get; set; }

		/// <summary>
		/// Отлючение разделителей
		/// </summary>
		public bool NoDelimiter { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public PersistentModel Model { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public string GetDigest(){
			var info = GetDigestFinisher();
			if (!string.IsNullOrWhiteSpace(info)){
				var comment = Comment;
				if (!string.IsNullOrWhiteSpace(comment)){
					comment = ", " + comment;
				}
				return info + " (" + Mode.ToString()[0] + "," + Dialect.ToString()[0] + (Optional ? ",O" : ",R") + comment+")";
			}
			return "";
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected virtual string GetDigestFinisher(){
			return this.GetType().Name;
		}

		
	}
}