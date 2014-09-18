using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Qorpent.Dsl;
using Qorpent.Log;
using Qorpent.Utils.Extensions;

namespace Qorpent.PortableHtml{

	/// <summary>
	/// Стратегия разбора схемы
	/// </summary>
	public enum PortableHtmlVerificationStrategy{
		/// <summary>
		/// Любой не None результат завершает проверку
		/// </summary>
		ForcedResult=1,
		/// <summary>
		/// Первая ошибка на конкретном элементе прекращает его работу
		/// </summary>
		ForcedElementResult=2,
		/// <summary>
		/// Обрабатываются и записываются все ошибки
		/// </summary>
		Full =4,
		
	}

	/// <summary>
	/// Контекст обработки PHTML
	/// </summary>
	public class PortableHtmlContext{
		/// <summary>
		/// 
		/// </summary>
		public PortableHtmlContext(){
			TrustedHosts = new List<string>();
			Errors= new List<PortableHtmlSchemaErorrDescription>();
			Strategy = PortableHtmlVerificationStrategy.ForcedResult;
			Log = StubUserLog.Default;
		}
		/// <summary>
		/// Uri происхождения документа
		/// </summary>
		public Uri BaseUri { get; set; }
		/// <summary>
		/// Разрешает использовать любые внешние адреса для рисунков
		/// </summary>
		public bool TrustAllImages { get; set; }
		/// <summary>
		/// Стратегия обработки ошибок
		/// </summary>
		public PortableHtmlVerificationStrategy Strategy { get; set; }

		/// <summary>
		/// Разрешает использовать A - ссылки на любые сайты
		/// </summary>
		public bool TrustAllSites { get; set; }

		/// <summary>
		/// Обрабатываемый материал в формате
		/// </summary>
		public XElement SourceXml { get; set; }
		/// <summary>
		/// Перечень доверенных хостов
		/// </summary>
		public IList<string> TrustedHosts { get; private set; }
		/// <summary>
		/// Аннотация ранее проверенных (напрример запрещенных) элементов - не требуют перепроверки на прочие условия
		/// </summary>
		private class SkipInElementChecking { public static readonly SkipInElementChecking Default = new SkipInElementChecking(); }
		/// <summary>
		/// 
		/// </summary>
		public IUserLog Log { get; set; }

		/// <summary>
		/// Возвращает статус доверия к ссылке
		/// </summary>
		/// <param name="url">URL ресурса для проверки</param>
		/// <param name="isImg">Признак того, что это ссылка на рисунок</param>
		/// <returns></returns>
		public PortableHtmlSchemaErorr GetUriTrustState(string url, bool isImg = false){
			if (string.IsNullOrWhiteSpace(url) || url.StartsWith("#")){
				return PortableHtmlSchemaErorr.EmptyOrHashedLink;
			}
				var uri = new Uri(url, UriKind.RelativeOrAbsolute);
				if (!uri.IsAbsoluteUri) return PortableHtmlSchemaErorr.None;
				if (uri.Scheme.ToLowerInvariant() == "data"){
					if (isImg) return PortableHtmlSchemaErorr.None;
					return PortableHtmlSchemaErorr.DataLink;
				}
				if (uri.Scheme.ToLowerInvariant() == "javascript"){
					return PortableHtmlSchemaErorr.JavascriptLink;
				}
				if (uri.Scheme.ToLowerInvariant() == "file"){
					return PortableHtmlSchemaErorr.FileLink;
				}
				if (isImg && TrustAllImages) return PortableHtmlSchemaErorr.None;
				if (!isImg && TrustAllSites) return PortableHtmlSchemaErorr.None;
				if (null != BaseUri && BaseUri.Host == uri.Host){
					return PortableHtmlSchemaErorr.None;
				}
				if (TrustedHosts.Contains(uri.Host)) return PortableHtmlSchemaErorr.None;
				return PortableHtmlSchemaErorr.NonTrustedLink;
		
			
		}
		/// <summary>
		/// Определение активности проверки, исходя из стратегии
		/// </summary>
		public bool IsActive{
			get{
				if (Ok) return true;
				if (Strategy == PortableHtmlVerificationStrategy.ForcedResult) return false;
				return true;
			}
		}

		

		/// <summary>
		/// Признак валидного результата
		/// </summary>
		public bool Ok
		{
			get { return PortableHtmlSchemaErorr.None == SchemaError; }
		}
		/// <summary>
		/// Код ошибки в схеме PHTML
		/// </summary>
		public PortableHtmlSchemaErorr SchemaError { get; set; }
		/// <summary>
		/// Реестр ошибок
		/// </summary>
		public IList<PortableHtmlSchemaErorrDescription> Errors { get; private set; }

		/// <summary>
		/// Системное исключение
		/// </summary>
		public Exception Exception { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="el"></param>
		/// <returns></returns>
		public bool InChecking(XElement el){
			if (Strategy == PortableHtmlVerificationStrategy.Full) return true;
			return null == el.Annotation<SkipInElementChecking>();
		}

			/// <summary>
		/// Fluent-метод применения ошибки
		/// </summary>
		/// <param name="error"></param>
		/// <param name="el"></param>
		/// <param name="a"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		public PortableHtmlContext SetError(PortableHtmlSchemaErorr error,XElement  el=null, XAttribute a=null,Exception e=null){
			var desc = new PortableHtmlSchemaErorrDescription(error, el, a, e);
			return SetError(desc);
		}
		/// <summary>
		/// Установить заранее подготовленный дескриптор ошибки
		/// </summary>
		/// <param name="error"></param>
		/// <returns></returns>
		public PortableHtmlContext SetError(PortableHtmlSchemaErorrDescription error){
			if (null != error.Element && this.Strategy != PortableHtmlVerificationStrategy.Full){
				error.Element.AddAnnotation(SkipInElementChecking.Default);
			}
			this.SchemaError |= error.Error;
			this.Errors.Add(error);
			this.Exception = Exception ?? error.Exception;
			return this;
		}

		
	}
	/// <summary>
	/// Описатель отдельной ошибки схемы
	/// </summary>
	public class PortableHtmlSchemaErorrDescription{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="error"></param>
		/// <param name="el"></param>
		/// <param name="a"></param>
		/// <param name="e"></param>
		public PortableHtmlSchemaErorrDescription(PortableHtmlSchemaErorr error, XElement el, XAttribute a, Exception e){
			this.Error = error;
			if (null != el){
				Element = el;
				var li = new LexInfo();
				var eli = el as IXmlLineInfo;
				if (eli.HasLineInfo()){
					li.Line = eli.LineNumber;
					li.Column = eli.LinePosition;
				}
				li.Context = el.GetXPath();
				Info = li;
			}else if (null != a){
				Attribute = a;
				Element = a.Parent;
				var li = new LexInfo();
				var eli = a.Parent as IXmlLineInfo;
				if (eli.HasLineInfo()){
					li.Line = eli.LineNumber;
					li.Column = eli.LinePosition;
				}
				li.Context = a.GetXPath();
				Info = li;
			}
			this.Exception = e;


		}

		/// <summary>
		/// Ссылка на элемент
		/// </summary>
		public XElement Element { get; set; }
		/// <summary>
		/// Ссылка на атрибут
		/// </summary>
		public XAttribute Attribute { get; set; }
		/// <summary>
		/// Тип ошибки
		/// </summary>
		public PortableHtmlSchemaErorr Error { get; set; }
		/// <summary>
		/// Место возникновения
		/// </summary>
		public LexInfo Info { get; set; }
		/// <summary>
		/// Связаннвя системная ошибка
		/// </summary>
		public Exception Exception { get; set; }
	}
}