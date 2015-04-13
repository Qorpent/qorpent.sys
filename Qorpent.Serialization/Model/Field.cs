using System.Linq;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Data;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Model{
	/// <summary>
	///     Описывает поле в хранимом классе
	/// </summary>
	public class Field{
		private string _reverseCollectionName;

		/// <summary>
		///     Исходное определение
		/// </summary>
		public XElement Definition { get; set; }

		/// <summary>
		///     Класс (таблица) - контейнер
		/// </summary>
		public PersistentClass Table { get; set; }

		/// <summary>
		///     Признак свойства не для схемы SQL
		/// </summary>
		public bool NoSql { get; set; }

		/// <summary>
		///     Признак свойства не для кода
		/// </summary>
		public bool NoCode { get; set; }

		/// <summary>
		///     Имя поля
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///     Комментарий к полю
		/// </summary>
		public string Comment { get; set; }

		/// <summary>
		///     Тип данных
		/// </summary>
		public DataType DataType { get; set; }

		/// <summary>
		///     Признак уникального поля
		/// </summary>
		public bool IsUnique { get; set; }

		/// <summary>
		///     Значение по умолчанию для SQL
		/// </summary>
		public DefaultValue DefaultSqlValue { get; set; }

		/// <summary>
		///     Значение по умолчанию для POKO
		/// </summary>
		public DefaultValue DefaultObjectValue { get; set; }

		/// <summary>
		///     Определитель для целевой таблицы
		/// </summary>
		public string ReferenceTable { get; set; }

		/// <summary>
		///     Класс, на который ссылается ссылка
		/// </summary>
		public PersistentClass ReferenceClass { get; set; }

		/// <summary>
		///     Признак внешней ссылки
		/// </summary>
		public bool IsReference { get; set; }

		/// <summary>
		///     Поле, на которое ссылается ссылка
		/// </summary>
		public string ReferenceField { get; set; }

		/// <summary>
		///     Для ссылок - признак автоматической загрузки по умолчанию
		/// </summary>
		public bool IsAutoLoadByDefault { get; set; }

		/// <summary>
		///     Признак "ленивой" загрузки по умолчанию
		/// </summary>
		public bool IsLazyLoadByDefault { get; set; }

		/// <summary>
		///     Признак ссылки, порождающей на втором конце коллекцию
		/// </summary>
		public bool IsReverese { get; set; }

		/// <summary>
		///     Признак того, что коллекция на втором конце автоматически загружается
		/// </summary>
		public bool IsAutoLoadReverseByDefault { get; set; }

		/// <summary>
		///     Признак того, что коллекция на втором конце загружается в "ленивом" режиме по умолчанию
		/// </summary>
		public bool IsLazyLoadReverseByDefault { get; set; }

		/// <summary>
		///     Признак того, что целевой объект должен клонироваться по умолчанию при вызове Clone
		/// </summary>
		public bool IsCloneByDefault { get; set; }

		/// <summary>
		///     Признак того, что целевая коллекция должна клонироваться по умолчанию при вызове Clone
		/// </summary>
		public bool IsReverseCloneByDefault { get; set; }

		/// <summary>
		///     Пользовательское имя для обратной коллекции
		/// </summary>
		public string CustomReverseName { get; set; }

		/// <summary>
		///     Признак первичного ключа
		/// </summary>
		public bool IsPrimaryKey { get; set; }

		/// <summary>
		///     Поле требующее автоматического приращения
		/// </summary>
		public bool IsAutoIncrement { get; set; }
		/// <summary>
		///     Порядок поля при генерации таблиц и классов
		/// </summary>
		public int Idx { get; set; }

		/// <summary>
		///     Перекрытие размерности типа данных для данного поля
		/// </summary>
		public int Size { get; set; }

		/// <summary>
		/// </summary>
		public bool Resolve { get; set; }

		/// <summary>
		///     Приоритет при резолюции
		/// </summary>
		public int ResolvePriority { get; set; }
        /// <summary>
        /// Ссылка на внешнюю таблицу корпент
        /// </summary>
	    public bool IsExternal {
	        get { return Definition.Attr("external").ToBool(); }
	    }
        /// <summary>
        /// Признак NullAble поля
        /// </summary>
	    public bool IsNullable {
	        get {
	            if (Definition.Attr("not-null").ToBool()) return true;
	            if (IsReference) {
	                if (null == ReferenceClass || null == ReferenceClass.TargetClass) {
	                    return !IsExternal;
	                }
	            }
	            return false;
	        }
	    }

		/// <summary>
		/// </summary>
		public ResolveType ResolveType{
			get{
				if (!Resolve) return ResolveType.None;
				if (!Table.ResolveAble) return ResolveType.None;
				if (Name == "Tag") return ResolveType.Tag;
				if (IsReference) return ResolveType.Delegate;
				if (Name.Contains("Properites") || DataType.CSharpDataType.Contains("Dictionary")) return ResolveType.Dictionary;
				return ResolveType.List;
			}
		}

		/// <summary>
		///     Признак того, что проход по данной ссылке может привести к циркулярным проходам
		/// </summary>
		public bool? IsCircular { get; set; }

		/// <summary>
		///     Имя для целевой обратной коллекции
		/// </summary>
		/// <remarks>
		///     Приоритет - CustomReverseName, множественное имя класса при совпадении имени поля и целевого класса,
		///     множественное имя+суффикс By+имя поля
		/// </remarks>
		public string ReverseCollectionName{
			get{
				if (null == _reverseCollectionName){
					if (!string.IsNullOrWhiteSpace(CustomReverseName)){
						_reverseCollectionName = CustomReverseName;
					}
					else{
						string multName = Table.Name;
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
		/// Формула для автовычисляемого поля
		/// </summary>
		public string ComputeAs { get; set; }

		/// <summary>
		///     Резолюция полного типа данных для поля
		/// </summary>
		/// <param name="dialect"></param>
		/// <returns></returns>
		public SqlDataType GetSqlType(SqlDialect dialect = SqlDialect.Ansi){
			SqlDataType result = DataType.ResolveSqlDataType(dialect);
			if (0 == Size) return result;
			return new SqlDataType{Name = result.Name, Size = Size, Precession = result.Precession};
		}

		/// <summary>
		///     Загружает поле из XML
		/// </summary>
		/// <param name="c"></param>
		/// <param name="e"></param>
		/// <param name="cls"></param>
		/// <returns></returns>
		public Field Setup(IBSharpClass c, XElement e, PersistentClass cls){
			Table = cls;
			Definition = e;
			SetupCommon(c, e);
			if (ImplicitRef || e.Name.LocalName == "ref"){
				SetupReference(c, e);
			}
			else{
				SetupUsualField(c, e);
			}
			SetupDefaultValue(c, e);

			return this;
		}

		private void SetupDefaultValue(IBSharpClass c, XElement e){
			DefaultSqlValue = DefaultValue.Create(this, e.Attr("default"), e);
			DefaultObjectValue = DefaultValue.Create(this, e.Attr("csharp-default"), e);
		}

		private void SetupCommon(IBSharpClass c, XElement e){
			Name = e.Attr("code");
			Comment = e.Attr("name");
			IsHash = e.Attr("hash").ToBool();
			Idx = e.Attr("idx").ToInt();
			if (0 == Idx) Idx = 99999;
			Resolve = e.GetSmartValue("resolve").ToBool();
			ResolvePriority = e.GetSmartValue("resolve").ToInt();
			if (1>=ResolvePriority){
				ResolvePriority = 55555;
			}
			NoSql = e.GetSmartValue("nosql").ToBool();
			ComputeAs = e.GetSmartValue("as").ToString();
			NoCode = e.GetSmartValue("nocode").ToBool();
			IsCloneByDefault = e.GetSmartValue("clone").ToBool();
			//Name = PersistentClass.EscapeSqlName(Name);
		}

		/// <summary>
		/// </summary>
		/// <param name="suffix"></param>
		/// <returns></returns>
		public string GetConstraintName(string suffix) {
		    
			if (suffix == "FK"){
                var refclassName = ReferenceTable.Replace(".", "_");
                if (null != ReferenceClass)
                {
                    refclassName = ReferenceClass.Name;
                }
				return
					(Table.FullSqlName.Replace(".", "_").Replace("\"", "") + "_" + Name + "_" + refclassName + "_" +
					 ReferenceField + "_" + suffix).ToLowerInvariant();
			}
			return (Table.FullSqlName.Replace(".", "_").Replace("\"", "") + "_" + Name + "_" + suffix).ToLowerInvariant();
		}


		private void SetupUsualField(IBSharpClass c, XElement e){
			DataType = Table.DataTypeMap[e.Name.LocalName];
			IsPrimaryKey = e.GetSmartValue("primarykey").ToBool();
			if (!IsPrimaryKey) {
				IsUnique = e.GetSmartValue("unique").ToBool();
			} else {
				IsUnique = true;
			}
			IsAutoIncrement = e.GetSmartValue("identity").ToBool();
			string csharptype = e.GetSmartValue("csharp-type");
			if (!string.IsNullOrWhiteSpace(csharptype)){
				DataType realtype = DataType.Copy();
				realtype.CSharpDataType = csharptype;
				DataType = realtype;
			}
		}

		private void SetupReference(IBSharpClass c, XElement e){
			IsReference = true;
			IsAutoLoadByDefault = e.GetSmartValue("auto").ToBool();
			IsLazyLoadByDefault = e.GetSmartValue("lazy").ToBool();
			IsReverese = e.GetSmartValue("reverse").ToBool();
			//проверяем, что реверс указан не как флаг, а как имя коллекции
			if (IsReverese && 0 == e.GetSmartValue("reverse").ToInt()){
				CustomReverseName = e.GetSmartValue("reverse");
			}
			IsAutoLoadReverseByDefault = e.GetSmartValue("reverse-auto").ToBool();
			IsLazyLoadReverseByDefault = e.GetSmartValue("reverse-lazy").ToBool();

			IsReverseCloneByDefault = e.GetSmartValue("reverse-clone").ToBool();
			string refto = "";
			if (ImplicitRef){
				refto = e.Name.LocalName + ".PrimaryKey";
			}
			else{
				refto = e.Attr("to", Name + ".PrimaryKey");
				if (!refto.Contains("."))
				{
					refto += ".PrimaryKey";
				}
			}
			string[] refparts = refto.Split('.');
			ReferenceTable = refparts[refparts.Length - 2].ToLowerInvariant();
			ReferenceField = refparts[refparts.Length - 1].ToLowerInvariant();
			if (refparts.Length > 2){
				ReferenceSchema = refparts[refparts.Length - 3].ToLowerInvariant();
			}
			DataType = Table.DataTypeMap["int"];
			if (ReferenceField.ToLowerInvariant() == "code"){
				DataType = Table.DataTypeMap["string"];
			}
			if (ResolvePriority==55555)
			{
				ResolvePriority = 99999;
			}
		}
		/// <summary>
		/// Схема таблицы на ссылке
		/// </summary>
		public string ReferenceSchema { get; set; }
		/// <summary>
		/// Признак неявной ссылки
		/// </summary>
		public bool ImplicitRef { get; set; }
		/// <summary>
		///		Признак использования в хэше
		/// </summary>
		public bool IsHash { get; set; }

		/// <summary>
		///     Проверяет ссылеи на циркулярность и выставляет соответствующее свойство
		/// </summary>
		public bool GetIsCircular(){
			if (IsCircular.HasValue) return IsCircular.Value;
			if (!IsReference || null == ReferenceClass || ReferenceClass == Table){
				IsCircular = false;
				return false;
			}
			IsCircular = (ReferenceClass.GetAccessibleClasses(onlyforeigns: true).Any(_ => _ == ReferenceClass));
			return IsCircular.Value;
		}
	}
}