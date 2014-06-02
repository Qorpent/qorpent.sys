using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Model.SqlObjects{
	/// <summary>
	/// Схема
	/// </summary>
	public class Sequence : SqlObject
	{
		/// <summary>
		/// 
		/// </summary>
		public Sequence()
		{
			ObjectType = SqlObjectType.Sequence;
			PreTable = true;
			Step = 10;
			Start = 10;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="model"></param>
		/// <param name="cls"></param>
		/// <param name="bscls"></param>
		/// <param name="xml"></param>
		public override void Setup(PersistentModel model, PersistentClass cls, BSharp.IBSharpClass bscls, System.Xml.Linq.XElement xml)
		{
			base.Setup(model, cls, bscls, xml);
			if (null != xml){
				this.Start = xml.Attr("start", "10").ToInt();
				this.Step = xml.Attr("step", "10").ToInt();
			}
			this.Name = cls.Name + "_SEQ";
			this.Schema = cls.Schema;
			if (null != cls.PrimaryKey){
				this.DataType = cls.PrimaryKey.DataType;
			}
			else{
				this.DataType = cls.DataTypeMap["int"];
			}

		}
		/// <summary>
		/// 
		/// </summary>
		public DataType DataType { get; set; }

		/// <summary>
		/// Шаг приращения
		/// </summary>
		public int Step { get; set; }
		/// <summary>
		/// начальный индекс
		/// </summary>
		public int Start { get; set; }
	}
}