using System;
using System.Xml.Linq;
using Qorpent.Dsl;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;
namespace Qorpent.BSharp {
	/// <summary>
	/// Структура, описывающая ошибки компиляции
	/// </summary>
	[Serialize]
	public class BSharpError {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		protected bool Equals(BSharpError other){
			return Equals(LexInfo, other.LexInfo) && Level == other.Level && Type == other.Type && Phase == other.Phase && string.Equals(Message, other.Message) && string.Equals(ClassName, other.ClassName);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public BSharpErrorDigest GetDigest(){
		
			return new BSharpErrorDigest{
				LogLevel=Level, 
				Phase=Phase,Message= Message,
				Type= Type,
				ClassName=ResolvedClassName(), 
				FileName=LexInfo.File,
				Line= LexInfo.Line, 
				Column= LexInfo.Column
			};
		
		}

		private string ResolvedClassName(){
			if (!string.IsNullOrWhiteSpace(ClassName)) return ClassName;
			if (null == Class && null == AltClass) return "без привязки";
			if (null != Class && null == AltClass) return Class.FullName;
			if (null != AltClass && null == Class) return ":"+AltClass.FullName;
			if (Class.FullName == AltClass.FullName) return Class.FullName;
			return Class.FullName + ":" + AltClass.FullName;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode(){
			unchecked{
				int hashCode = (_lexInfo != null ? _lexInfo.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ (int) Level;
				hashCode = (hashCode*397) ^ (int) Type;
				hashCode = (hashCode*397) ^ (int) Phase;
				hashCode = (hashCode*397) ^ (Message != null ? Message.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ (ClassName != null ? ClassName.GetHashCode() : 0);
				return hashCode;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((BSharpError) obj);
		}

		private LexInfo _lexInfo;

		/// <summary>
		/// Уровень опасности ошибки
		/// </summary>
		[SerializeNotNullOnly]
		public ErrorLevel Level { get; set; }
		/// <summary>
		/// Тип ошибки
		/// </summary>
		[SerializeNotNullOnly]
		public BSharpErrorType Type { get; set; }

		/// <summary>
		/// Фаза, на которой произошла ошибка
		/// </summary>
		[SerializeNotNullOnly]
		public BSharpCompilePhase Phase { get; set; }

		/// <summary>
		/// Может содержать пользовательское сообщение
		/// </summary>
		[SerializeNotNullOnly]
		public string Message { get; set; }

		/// <summary>
		/// Может содержать Xml с какими-то дополнительными данными
		/// </summary>
		[SerializeNotNullOnly]
		public XElement Xml { get; set; }
		/// <summary>
		/// Класс, которого касается ошибка
		/// </summary>
		[SerializeNotNullOnly]
		public IBSharpClass Class { get; set; }
		/// <summary>
		/// Системное исключение
		/// </summary>
		[SerializeNotNullOnly]
		public Exception Error { get; set; }
		/// <summary>
		///Дополнительные данные
		/// </summary>
		[SerializeNotNullOnly]
		public object Data { get; set; }
		/// <summary>
		/// Имя класса
		/// </summary>
		[SerializeNotNullOnly]
		public string ClassName { get; set; }
		/// <summary>
		/// Дополнительный класс для некоторых сообщений
		/// </summary>
		[SerializeNotNullOnly]
		public IBSharpClass AltClass { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <exception cref="NotImplementedException"></exception>
		public LexInfo LexInfo{
			get{
				if (null == _lexInfo){
					var lexinfo = new LexInfo();
					if (Xml != null)
					{
						var desc = Xml.Describe();
						lexinfo.File = desc.File;
						lexinfo.Line = desc.Line;
						lexinfo.Column = desc.Column;
					}
					if (string.IsNullOrWhiteSpace(lexinfo.File)){
						if (Class != null){
							var xml = Class.Source;
							var desc = xml.Describe();
							lexinfo.File = desc.File;
							lexinfo.Line = desc.Line;
							lexinfo.Column = desc.Column;
						}
					}
					if (string.IsNullOrWhiteSpace(lexinfo.File))
					{
						lexinfo.File = "Общая ошибка компиляции";
					}
					_lexInfo = lexinfo;
				}
				return _lexInfo;
			}
		    set { _lexInfo = value; }
		}

		/// <summary>
		/// Конвертирует строку для лога
		/// </summary>
		/// <returns></returns>
		public string ToLogString() {
			return String.Format(@"{0}:{1} {2} ({4},{5})", Type, Phase, Message, Xml, 
				null==Class?ClassName:Class.FullName,null==AltClass?"":AltClass.FullName);
		}

	}
}