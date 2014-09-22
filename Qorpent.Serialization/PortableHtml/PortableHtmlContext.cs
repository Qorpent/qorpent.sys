using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Qorpent.PortableHtml{
	/// <summary>
	///     Контекст обработки PHTML
	/// </summary>
	public class PortableHtmlContext{
		/// <summary>
		/// </summary>
		public PortableHtmlContext(){
			TrustedHosts = new List<string>();
			Errors = new List<PortableHtmlSchemaErorrDescription>();
			Strategy = PortableHtmlVerificationStrategy.ForcedResult;
		}

		/// <summary>
		///     Uri происхождения документа
		/// </summary>
		public Uri BaseUri { get; set; }


		/// <summary>
		/// Уровень разрешений
		/// </summary>
		public PortableHtmlStrictLevel Level { get; set; }
		

		/// <summary>
		///     Стратегия обработки ошибок
		/// </summary>
		public PortableHtmlVerificationStrategy Strategy { get; set; }

		



		/// <summary>
		///     Обрабатываемый материал в формате
		/// </summary>
		public XElement SourceXml { get; set; }

		/// <summary>
		///     Перечень доверенных хостов
		/// </summary>
		public IList<string> TrustedHosts { get; private set; }

		/// <summary>
		///     Определение активности проверки, исходя из стратегии
		/// </summary>
		public bool IsActive{
			get{
				if (Ok) return true;
				if (Strategy == PortableHtmlVerificationStrategy.ForcedResult) return false;
				return true;
			}
		}


		/// <summary>
		///     Признак валидного результата
		/// </summary>
		public bool Ok{
			get { return PortableHtmlSchemaErorr.None == SchemaError; }
		}

		/// <summary>
		///     Код ошибки в схеме PHTML
		/// </summary>
		public PortableHtmlSchemaErorr SchemaError { get; set; }

		/// <summary>
		///     Реестр ошибок
		/// </summary>
		public IList<PortableHtmlSchemaErorrDescription> Errors { get; private set; }

		/// <summary>
		///     Системное исключение
		/// </summary>
		public Exception Exception { get; set; }

		/// <summary>
		///     Возвращает статус доверия к ссылке
		/// </summary>
		/// <param name="url">URL ресурса для проверки</param>
		/// <param name="isImg">Признак того, что это ссылка на рисунок</param>
		/// <returns></returns>
		public PortableHtmlSchemaErorr GetUriTrustState(string url, bool isImg = false){
			try{
				if (string.IsNullOrWhiteSpace(url) || url.StartsWith("#")){
					return PortableHtmlSchemaErorr.EmptyOrHashedLink;
				}
				var uri = new Uri(url, UriKind.RelativeOrAbsolute);
				if (!uri.IsAbsoluteUri) return PortableHtmlSchemaErorr.None;
				if (uri.Scheme.ToLowerInvariant() == "data"){
					if (isImg) return PortableHtmlSchemaErorr.None;
					return PortableHtmlSchemaErorr.DangerousLink;
				}
				if (uri.Scheme.ToLowerInvariant() == "javascript"){
					return PortableHtmlSchemaErorr.DangerousLink;
				}
				if (uri.Scheme.ToLowerInvariant() == "file"){
					return PortableHtmlSchemaErorr.DangerousLink;
				}
				if (isImg && Level.HasFlag(PortableHtmlStrictLevel.TrustAllImages)) return PortableHtmlSchemaErorr.None;
				if (!isImg && Level.HasFlag(PortableHtmlStrictLevel.TrustAllLinks)) return PortableHtmlSchemaErorr.None;
				if (null != BaseUri && BaseUri.Host == uri.Host){
					return PortableHtmlSchemaErorr.None;
				}
				if (TrustedHosts.Contains(uri.Host)) return PortableHtmlSchemaErorr.None;
				return PortableHtmlSchemaErorr.NonTrustedLink;
			}
			catch (UriFormatException){
				return PortableHtmlSchemaErorr.InvalidUri;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="el"></param>
		/// <returns></returns>
		public bool InChecking(XElement el){
			if (Strategy == PortableHtmlVerificationStrategy.Full) return true;
			return null == el.Annotation<SkipInElementChecking>();
		}

		/// <summary>
		///     Fluent-метод применения ошибки
		/// </summary>
		/// <param name="error"></param>
		/// <param name="el"></param>
		/// <param name="a"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		public PortableHtmlContext SetError(PortableHtmlSchemaErorr error, XElement el = null, XAttribute a = null,
		                                    Exception e = null){
			var desc = new PortableHtmlSchemaErorrDescription(error, el, a, e);
			return SetError(desc);
		}

		/// <summary>
		///     Установить заранее подготовленный дескриптор ошибки
		/// </summary>
		/// <param name="error"></param>
		/// <returns></returns>
		public PortableHtmlContext SetError(PortableHtmlSchemaErorrDescription error){
			if (null != error.Element && Strategy != PortableHtmlVerificationStrategy.Full){
				error.Element.AddAnnotation(SkipInElementChecking.Default);
			}
			SchemaError |= error.Error;
			Errors.Add(error);
			Exception = Exception ?? error.Exception;
			return this;
		}

		/// <summary>
		///     Аннотация ранее проверенных (напрример запрещенных) элементов - не требуют перепроверки на прочие условия
		/// </summary>
		private class SkipInElementChecking{
			public static readonly SkipInElementChecking Default = new SkipInElementChecking();
		}

		/// <summary>
		/// Возвращает статус элемента в данном контексе
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public PortableHtmlSchemaErorr GetTagState(string name){
			name = name.ToLowerInvariant();
			if (-1 != Array.IndexOf(PortableHtmlSchema.StrictElements, name)) return PortableHtmlSchemaErorr.None;
			if(-1!=Array.IndexOf(PortableHtmlSchema.DangerousElements,name))return PortableHtmlSchemaErorr.DangerousElement;
			if (Level.HasFlag(PortableHtmlStrictLevel.AllowBr))
			{
				if (PortableHtmlSchema.BrTag==name) return PortableHtmlSchemaErorr.None;
			}
			if (Level.HasFlag(PortableHtmlStrictLevel.AllowTables)){
				if (-1 != Array.IndexOf(PortableHtmlSchema.TableElements, name)) return PortableHtmlSchemaErorr.None;	
			}
			if (Level.HasFlag(PortableHtmlStrictLevel.AllowLists))
			{
				if (-1 != Array.IndexOf(PortableHtmlSchema.ListElements, name)) return PortableHtmlSchemaErorr.None;
			}
			return PortableHtmlSchemaErorr.UnknownElement;
		}


		/// <summary>
		/// Метод проверки разрешенных атрибутов
		/// </summary>
		/// <param name="attributename"></param>
		/// <param name="parentElementName">Имя родительского тега</param>
		/// <returns></returns>
		public  PortableHtmlSchemaErorr GetAttributeErrorState(string attributename, string parentElementName)
		{
			var aname = attributename.ToLowerInvariant();
			var tname = parentElementName.ToLowerInvariant();
			if(aname=="href" && tname=="a")return PortableHtmlSchemaErorr.None;
			if(aname=="src" && tname=="img")return PortableHtmlSchemaErorr.None;
			if (aname.StartsWith("phtml_")) return PortableHtmlSchemaErorr.None;
			if (aname == "target" && tname == "a") return PortableHtmlSchemaErorr.DangerousAttribute;


			if (-1 != Array.IndexOf(PortableHtmlSchema.DeprecatedAttributes, aname))
			{
				if (aname == "id" || aname == "style" || aname == "class")
				{
					return PortableHtmlSchemaErorr.CssAttributeDetected;
				}
				return PortableHtmlSchemaErorr.DangerousAttribute;
			}

			if (PortableHtmlSchema.DeprecatedAttributePrefixes.Any(aname.StartsWith))
			{
				if (aname.StartsWith("on")) return PortableHtmlSchemaErorr.EventAttributeDetected;
				return PortableHtmlSchemaErorr.AngularAttributeDetected;
			}

			if(Level.HasFlag(PortableHtmlStrictLevel.AllowUnknownAttributes))return PortableHtmlSchemaErorr.None;
			return PortableHtmlSchemaErorr.UnknownAttribute;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString(){
			var sb = new StringBuilder();
			sb.AppendLine(string.Format("Level : {0}", Level));
			sb.AppendLine(string.Format("Strategy : {0}", Strategy));
			sb.AppendLine(string.Format("IsOk : {0}", Ok));
			sb.AppendLine(string.Format("SchemaError : {0}", SchemaError));
			sb.AppendLine(string.Format("BaseUri : '{0}'", BaseUri == null ? "NULL" : BaseUri.ToString()));
			foreach (var trustedHost in TrustedHosts){
				sb.AppendLine("Trust : '" + trustedHost+"'");
			}
			if (0 != Errors.Count){
				foreach (var desc in Errors){
					sb.AppendLine(desc.ToString());
				}
			}
			return sb.ToString();
		}
	}
}