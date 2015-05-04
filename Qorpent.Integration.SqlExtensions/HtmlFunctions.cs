using System;
using System.Collections;
using System.Data.SqlTypes;
using System.Xml.Linq;
using Microsoft.SqlServer.Server;
using Qorpent.PortableHtml;

namespace Qorpent.Integration.SqlExtensions{
	/// <summary>
	/// </summary>
	public static class HtmlFunctions{
		private static readonly PortableHtmlConverter phtml = new PortableHtmlConverter();


		
			/// <summary>
			/// 
			/// </summary>
			/// <param name="src"></param>
			/// <param name="size"></param>
			/// <returns></returns>
		[SqlFunction(IsDeterministic = true, SystemDataAccess = SystemDataAccessKind.None, DataAccess = DataAccessKind.None)]
		public static SqlString PhtmlGetDigest(SqlString src, SqlInt32 size){
				try{
					return phtml.GetDigest(XElement.Parse(src.Value), size.IsNull ? 400 : size.Value);
				}
				catch(Exception ex){
					return "[DIGEST-ERROR:" + ex.Message + "]";
				}
		}


	}
}