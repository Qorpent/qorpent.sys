using System;
using System.Xml;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.PortableHtml{
	/// <summary>
	///     Описатель отдельной ошибки схемы
	/// </summary>
	public class PortableHtmlSchemaErorrDescription{
		/// <summary>
		/// </summary>
		/// <param name="error"></param>
		/// <param name="el"></param>
		/// <param name="a"></param>
		/// <param name="e"></param>
		public PortableHtmlSchemaErorrDescription(PortableHtmlSchemaErorr error, XElement el, XAttribute a, Exception e){
			Error = error;
			if (null != el){
				Element = el;
				var eli = el as IXmlLineInfo;
				if (eli.HasLineInfo()){
					Line = eli.LineNumber;
					Column = eli.LinePosition;
				}
				XPath = el.GetXPath();
			}
			else if (null != a){
				Attribute = a;
				Element = a.Parent;
				var eli = a.Parent as IXmlLineInfo;
				if (null!=eli && eli.HasLineInfo()){
					Line = eli.LineNumber;
					Column = eli.LinePosition;
				}
				XPath = a.GetXPath();
			}
			Exception = e;
		}

		/// <summary>
		///     Ссылка на элемент
		/// </summary>
		public XElement Element { get; set; }

		/// <summary>
		///     Ссылка на атрибут
		/// </summary>
		public XAttribute Attribute { get; set; }

		/// <summary>
		///     Тип ошибки
		/// </summary>
		public PortableHtmlSchemaErorr Error { get; set; }

		/// <summary>
		///     Строка
		/// </summary>
		public int Line { get; set; }

		/// <summary>
		///     Колонка
		/// </summary>
		public int Column { get; set; }

		/// <summary>
		///     XPath по происхождению
		/// </summary>
		public string XPath { get; set; }

		/// <summary>
		///     Связаннвя системная ошибка
		/// </summary>
		public Exception Exception { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString(){
			return string.Format("{0} {1}:{2}/{3} {4}", Error, Line, Column, XPath, Exception == null ? "" : Exception.Message);
		}
	}
}