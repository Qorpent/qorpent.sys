using System.Linq;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Model{
	/// <summary>
	/// Описывает поле в хранимом классе
	/// </summary>
	public class Field{
		private string _reverseCollectionName;
		/// <summary>
		/// Исходное определение
		/// </summary>
		public XElement Definition { get; set; }

		/// <summary>
		/// Класс (таблица) - контейнер
		/// </summary>
		public PersistentClass Table { get; set; }
		/// <summary>
		/// Имя поля
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Комментарий к полю
		/// </summary>
		public string Comment { get; set; }
		/// <summary>
		/// Тип данных
		/// </summary>
		public DataType DataType { get; set; }
		/// <summary>
		/// Признак уникального поля
		/// </summary>
		public bool IsUnique { get; set;}
		/// <summary>
		/// Значение по умолчанию для SQL
		/// </summary>
		public DefaultValue DefaultSqlValue { get; set; }
		/// <summary>
		/// Значение по умолчанию для POKO
		/// </summary>
		public DefaultValue DefaultObjectValue { get; set; }
		/// <summary>
		/// Определитель для целевой таблицы
		/// </summary>
		public string ReferenceTable { get; set; }
		/// <summary>
		/// Класс, на который ссылается ссылка
		/// </summary>
		public PersistentClass ReferenceClass { get; set; }
		/// <summary>
		/// Признак внешней ссылки
		/// </summary>
		public bool IsReference { get; set; }
		/// <summary>
		/// Поле, на которое ссылается ссылка
		/// </summary>
		public string ReferenceField { get; set; }
		/// <summary>
		/// Для ссылок - признак автоматической загрузки по умолчанию
		/// </summary>
		public bool IsAutoLoadByDefault { get; set; }
		/// <summary>
		/// Признак "ленивой" загрузки по умолчанию
		/// </summary>
		public bool IsLazyLoadByDefault { get; set; }
		/// <summary>
		/// Признак ссылки, порождающей на втором конце коллекцию
		/// </summary>
		public bool IsReverese { get; set; }
		/// <summary>
		/// Признак того, что коллекция на втором конце автоматически загружается
		/// </summary>
		public bool IsAutoLoadReverseByDefault { get; set; }
		/// <summary>
		/// Признак того, что коллекция на втором конце загружается в "ленивом" режиме по умолчанию
		/// </summary>
		public bool IsLazyLoadReverseByDefault { get; set; }
		/// <summary>
		/// Пользовательское имя для обратной коллекции
		/// </summary>
		public string CustomReverseName { get; set; }
		/// <summary>
		/// Признак первичного ключа
		/// </summary>
		public bool IsPrimaryKey { get; set; }
		/// <summary>
		/// Поле требующее автоматического приращения
		/// </summary>
		public bool IsAutoIncrement { get; set; }
		/// <summary>
		/// Порядок поля при генерации таблиц и классов
		/// </summary>
		public int Idx { get; set; }
		/// <summary>
		/// Перекрытие размерности типа данных для данного поля
		/// </summary>
		public int Size { get; set; }

		/// <summary>
		/// Признак того, что проход по данной ссылке может привести к циркулярным проходам
		/// </summary>
		public bool? IsCircular { get; set; }
		/// <summary>
		/// Резолюция полного типа данных для поля
		/// </summary>
		/// <param name="dialect"></param>
		/// <returns></returns>
		public SqlDataType GetSqlType(SqlDialect dialect = SqlDialect.Ansi){
			var result = DataType.ResolveSqlDataType(dialect);
			if (0 == Size) return result;
			return new SqlDataType{Name = result.Name, Size = Size, Precession = result.Precession};
		}

		/// <summary>
		/// Имя для целевой обратной коллекции
		/// </summary>
		/// <remarks>Приоритет - CustomReverseName, множественное имя класса при совпадении имени поля и целевого класса,
		/// множественное имя+суффикс By+имя поля</remarks>
		public string ReverseCollectionName{
			get{
				if (null == _reverseCollectionName){
					if (!string.IsNullOrWhiteSpace(CustomReverseName)){
						_reverseCollectionName = CustomReverseName;
					}
					else{
						var multName = Table.Name;
						if (multName.EndsWith("s")){
							multName += "es";
						}
						else{
							multName += "s";
						}
						_reverseCollectionName = multName;
						if (null == ReferenceClass || Name != ReferenceClass.Name){
							_reverseCollectionName += "By" + Name;
						}
					}
				}
				return _reverseCollectionName;
			}
		}

		/// <summary>
		/// Загружает поле из XML
		/// </summary>
		/// <param name="c"></param>
		/// <param name="e"></param>
		/// <param name="cls"></param>
		/// <returns></returns>
		public Field Setup(IBSharpClass c, XElement e, PersistentClass cls){
			this.Table = cls;
			this.Definition = e;
			SetupCommon(c, e);
			if (e.Name.LocalName == "ref"){
				SetupReference(c,e);
			}
			else{
				SetupUsualField(c,e);
			}
			SetupDefaultValue(c,e);
			return this;
		}

		private void SetupDefaultValue(IBSharpClass c, XElement e){
			DefaultSqlValue = DefaultValue.Create(this, e.Attr("default"), e);
			DefaultObjectValue = DefaultValue.Create(this, e.Attr("csharp-default"), e);
		}

		private void SetupCommon(IBSharpClass c, XElement e){
			Name = e.Attr("code");
			Comment = e.Attr("name");
			Idx = e.Attr("idx").ToInt();
			
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="suffix"></param>
		/// <returns></returns>
		public string GetConstraintName(string suffix){
			if (suffix == "FK"){
				return Table.FullSqlName.Replace(".", "_") + "_" + Name + "_" + ReferenceClass.Name+"_"+ReferenceField+"_" + suffix;
			}
			return Table.FullSqlName.Replace(".", "_") + "_" + Name + "_" + suffix;
		}


		private void SetupUsualField(IBSharpClass c, XElement e){
			DataType = Table.DataTypeMap[e.Name.LocalName];
			IsPrimaryKey = e.GetSmartValue("primarykey").ToBool();
			IsUnique = e.GetSmartValue("unique").ToBool();
			IsAutoIncrement = e.GetSmartValue("identity").ToBool();
		}

		private void SetupReference(IBSharpClass c, XElement e){
			IsReference = true;
			IsAutoLoadByDefault = e.GetSmartValue("auto").ToBool();
			IsLazyLoadByDefault = e.GetSmartValue("lazy").ToBool();
			IsReverese = e.GetSmartValue("reverse").ToBool();
			//проверяем, что реверс указан не как флаг, а как имя коллекции
			if (IsReverese && 0 == e.GetSmartValue("reverse").ToInt())
			{

				CustomReverseName = e.GetSmartValue("reverse");
			}
			IsAutoLoadReverseByDefault = e.GetSmartValue("reverse-auto").ToBool();
			IsLazyLoadReverseByDefault = e.GetSmartValue("reverse-lazy").ToBool();
			var refto = e.Attr("to", Name + ".PrimaryKey");
			if (!refto.Contains(".")){
				refto += ".PrimaryKey";
			}
			var refparts = refto.Split('.');
			ReferenceTable = refparts[refparts.Length - 2].ToLowerInvariant();
			ReferenceField = refparts[refparts.Length - 1].ToLowerInvariant();
			DataType = Table.DataTypeMap["int"];
			if (ReferenceField.ToLowerInvariant() == "code"){
				DataType = Table.DataTypeMap["string"];
			}
		}
		/// <summary>
		/// Проверяет ссылеи на циркулярность и выставляет соответствующее свойство
		/// </summary>
		public bool GetIsCircular(){
			if(IsCircular.HasValue)return IsCircular.Value;
			if (!IsReference || null==ReferenceClass || ReferenceClass==Table){
				IsCircular = false;
				return false;
			}
			IsCircular = (ReferenceClass.GetAccessibleClasses(onlyforeigns: true).Any(_ => _ == ReferenceClass)) ;
			return IsCircular.Value;
		}
	}
}