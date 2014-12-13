using System.Data.SqlTypes;
using System.Linq;
using System.Xml.Linq;
using Microsoft.SqlServer.Server;
using Qorpent.Bxl;
using Qorpent.Utils.Extensions;

namespace Qorpent.Integration.SqlExtensions
{
    /// <summary>
    /// Расширения XML
    /// </summary>
    public static class XmlExtensions
    {
        /// <summary>
        /// 	Converts given BXL code to XML
        /// </summary>
        [SqlFunction(
            IsDeterministic = true,
            SystemDataAccess = SystemDataAccessKind.None,
            DataAccess = DataAccessKind.None
            )]
        public static SqlString sxaString(SqlString xml,SqlString name) {
            return ConvertToXml(xml).Attr(name.Value);
        }
        /// <summary>
        /// 	Converts given BXL code to XML
        /// </summary>
        [SqlFunction(
            IsDeterministic = true,
            SystemDataAccess = SystemDataAccessKind.None,
            DataAccess = DataAccessKind.None
            )]
        public static SqlString xaString(SqlXml xml, SqlString name)
        {
            return ConvertToXml(xml).Attr(name.Value);
        }

        /// <summary>
        /// 	Converts given BXL code to XML
        /// </summary>
        [SqlFunction(
            IsDeterministic = true,
            SystemDataAccess = SystemDataAccessKind.None,
            DataAccess = DataAccessKind.None
            )]
        public static SqlXml xGet(SqlString xml)
        {
            return new SqlXml(ConvertToXml(xml).CreateReader());
        }
       
        private static XElement ConvertToXml(object xml) {
            if (null == xml) return XElement.Parse("<stub/>");
            if (xml is SqlXml) {
                
                return XElement.Load((xml as SqlXml).CreateReader());
            }
            if (xml is SqlString) {
                var str = (SqlString)xml;
                if (str.Value.StartsWith("<")) {
                    return XElement.Parse(str.Value);
                }
                var result = MyBxl.Parse(str.Value);
                if (result.Elements().Count() == 1) {
                    return result.Elements().First();
                }
                return result;

            }
            return XElement.Parse("<stub/>");
        }

        /// <summary>
        /// 	Converts given BXL code to XML
        /// </summary>
        [SqlFunction(
            IsDeterministic = true,
            SystemDataAccess = SystemDataAccessKind.None,
            DataAccess = DataAccessKind.None
            )]
        public static SqlInt32 sxaInt(SqlString xml, SqlString name)
        {
            return ConvertToXml(xml).Attr(name.Value).ToInt();
        }

        /// <summary>
        /// 	Converts given BXL code to XML
        /// </summary>
        [SqlFunction(
            IsDeterministic = true,
            SystemDataAccess = SystemDataAccessKind.None,
            DataAccess = DataAccessKind.None
            )]
        public static SqlInt32 xaInt(SqlXml xml, SqlString name)
        {
            return ConvertToXml(xml).Attr(name.Value).ToInt();
        }

        /// <summary>
        /// 	Converts given BXL code to XML
        /// </summary>
        [SqlFunction(
            IsDeterministic = true,
            SystemDataAccess = SystemDataAccessKind.None,
            DataAccess = DataAccessKind.None
            )]
        public static SqlBoolean sxaBool(SqlString xml, SqlString name)
        {
            return ConvertToXml(xml).Attr(name.Value).ToBool();
        }

        /// <summary>
        /// 	Converts given BXL code to XML
        /// </summary>
        [SqlFunction(
            IsDeterministic = true,
            SystemDataAccess = SystemDataAccessKind.None,
            DataAccess = DataAccessKind.None
            )]
        public static SqlBoolean xaBool(SqlXml xml, SqlString name)
        {
            return ConvertToXml(xml).Attr(name.Value).ToBool();
        }

        /// <summary>
        /// 	Converts given BXL code to XML
        /// </summary>
        [SqlFunction(
            IsDeterministic = true,
            SystemDataAccess = SystemDataAccessKind.None,
            DataAccess = DataAccessKind.None
            )]
        public static SqlDecimal sxaDecimal(SqlString xml, SqlString name)
        {
            return ConvertToXml(xml).Attr(name.Value).ToDecimal();
        }

        /// <summary>
        /// 	Converts given BXL code to XML
        /// </summary>
        [SqlFunction(
            IsDeterministic = true,
            SystemDataAccess = SystemDataAccessKind.None,
            DataAccess = DataAccessKind.None
            )]
        public static SqlDecimal xaDecimal(SqlXml xml, SqlString name)
        {
            return ConvertToXml(xml).Attr(name.Value).ToDecimal();
        }

        /// <summary>
        /// 	Converts given BXL code to XML
        /// </summary>
        [SqlFunction(
            IsDeterministic = true,
            SystemDataAccess = SystemDataAccessKind.None,
            DataAccess = DataAccessKind.None
            )]
        public static SqlDateTime sxaDate(SqlString xml, SqlString name)
        {
            return ConvertToXml(xml).Attr(name.Value).ToDate();
        }

        /// <summary>
        /// 	Converts given BXL code to XML
        /// </summary>
        [SqlFunction(
            IsDeterministic = true,
            SystemDataAccess = SystemDataAccessKind.None,
            DataAccess = DataAccessKind.None
            )]
        public static SqlDateTime xaDate(SqlXml xml, SqlString name)
        {
            return ConvertToXml(xml).Attr(name.Value).ToDate();
        }
    }
}
