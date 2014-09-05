using System.Text;

namespace Qorpent.Scaffolding.Model{
	/// <summary>
	///     Описатель типа данных SQL
	/// </summary>
	public class SqlDataType{
		/// <summary>
		/// </summary>
		public static readonly SqlDataType Default = new SqlDataType{Name = "varchar", Size = 255};

		/// <summary>
		///     Название
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///     Размер
		/// </summary>
		public int Size { get; set; }

		/// <summary>
		///     Точность
		/// </summary>
		public int Precession { get; set; }
		/// <summary>
		/// Диалект
		/// </summary>
		public string Dialect { get; set; }

		/// <summary>
		///     Прямое приведение к строке для упрощенного составления строк - выражений
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static implicit operator string(SqlDataType type){
			return type.ToString();
		}

		/// <summary>
		///     Converts to valis sql string
		/// </summary>
		/// <returns></returns>
		public override string ToString(){
			var result = new StringBuilder();
			result.Append(Name);
			if (Size != 0){
				if (!(Size < 0 && Dialect != "ansi" && Dialect!="sqlserver")){
					result.Append('(');
					if (Size <= 0){
							result.Append("max");
						
					}
					else{
						result.Append(Size);
					}
					if (Precession != 0){
						result.Append(',');
						result.Append(Precession);
					}
					result.Append(")");
				}
			}
			return result.ToString();
		}
	}
}