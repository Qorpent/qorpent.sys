using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;
using Qorpent.Bxl;
using Qorpent.Utils.Extensions;

namespace Qorpent.Integration.SqlExtensions
{
    /// <summary>
    /// Обновленные расширения для работы с тегами
    /// </summary>
    public static class TagFunctions
    {
        /// <summary>
        /// 	Converts given BXL code to XML
        /// </summary>
        [SqlFunction(
            IsDeterministic = true,
            SystemDataAccess = SystemDataAccessKind.None,
            DataAccess = DataAccessKind.None
            )]
        public static SqlString tagSet(SqlString tags,SqlString name,SqlString value) {
            return TagHelper.SetValue(tags.Value, name.Value, value.Value);
        }

        /// <summary>
        /// 	Converts given BXL code to XML
        /// </summary>
        [SqlFunction(
            IsDeterministic = true,
            SystemDataAccess = SystemDataAccessKind.None,
            DataAccess = DataAccessKind.None
            )]
        public static SqlString tagGet(SqlString tags, SqlString name)
        {
            return TagHelper.Value(tags.Value, name.Value);
        }

        /// <summary>
        /// 	Converts given BXL code to XML
        /// </summary>
        [SqlFunction(
            IsDeterministic = true,
            SystemDataAccess = SystemDataAccessKind.None,
            DataAccess = DataAccessKind.None
            )]
        public static SqlBoolean tagMatch(SqlString tags, SqlString name)
        {
            return TagHelper.Match(tags.Value, name.Value);
        }

        /// <summary>
        /// Табличный варинат парсинга тегов
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        [SqlFunction(
            FillRowMethodName = "FillTags",
            TableDefinition = "code nvarchar(255), value nvarchar(255)"
            )]
        public static IEnumerable tagRead(SqlString tag)
        {
            return TagHelper.Parse(tag.Value).OrderBy(_=>_.Key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="code"></param>
        /// <param name="value"></param>
        public static void FillTags(Object obj, out SqlString code, out SqlString value) {
            var p = (KeyValuePair<string, string>)obj;
            code = p.Key;
            value = p.Value;
        }
    }
}
