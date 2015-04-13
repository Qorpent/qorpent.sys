using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Data;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Model.SqlObjects{
	/// <summary>
	///     Схема
	/// </summary>
	public class Sequence : SqlObject{
		/// <summary>
		/// </summary>
		public Sequence(){
			ObjectType = SqlObjectType.Sequence;
			PreTable = true;
			Step = 10;
			Start = 10;
		}
		/// <summary>
		///		Признак цикличности
		/// </summary>
		public bool IsCyclic { get; set; }
		/// <summary>
		/// </summary>
		public DataType DataType { get; set; }
		/// <summary>
		///		Строковое значение цикличности
		/// </summary>
		public string Cycle {
			get { return IsCyclic ? " CYCLE" : ""; }
		}
		/// <summary>
		///     Шаг приращения
		/// </summary>
		public int Step { get; set; }

		/// <summary>
		///     начальный индекс
		/// </summary>
		public int Start { get; set; }

		/// <summary>
		/// </summary>
		/// <param name="model"></param>
		/// <param name="cls"></param>
		/// <param name="bscls"></param>
		/// <param name="xml"></param>
		public override SqlObject Setup(PersistentModel model, PersistentClass cls, IBSharpClass bscls, XElement xml){
			base.Setup(model, cls, bscls, xml);
			if (null != xml){
				Start = xml.Attr("start", "10").ToInt();
				Step = xml.Attr("step", "10").ToInt();
			}
			Name = cls.Name + "_SEQ";
			Schema = cls.Schema;
			IsCyclic = cls.CyclicId;
			if (null != cls.PrimaryKey){
				DataType = cls.PrimaryKey.DataType;
			}
			else{
				DataType = cls.DataTypeMap["int"];
			}
			return this;
		}
	}
}