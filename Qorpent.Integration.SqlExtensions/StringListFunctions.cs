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
    public static class StringListFunctions
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

        /// <summary>
        /// Табличный варинат парсинга тегов
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        [SqlFunction(
            FillRowMethodName = "FillDict",
            TableDefinition = "code nvarchar(255), value nvarchar(255)"
            )]
        public static IEnumerable dictRead(SqlString dict)
        {
            return dict.Value.ReadAsDictionary().OrderBy(_ => _.Key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="code"></param>
        /// <param name="value"></param>
        public static void FillDict(Object obj, out SqlString code, out SqlString value)
        {
            var p = (KeyValuePair<string, string>)obj;
            code = p.Key;
            value = p.Value;
        }



        /// <summary>
        /// Табличный варинат парсинга списков
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        [SqlFunction(
            FillRowMethodName = "FillList",
            TableDefinition = "value nvarchar(255)"
            )]
        public static IEnumerable listRead(SqlString tag) {
            return tag.Value.SmartSplit();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void FillList(Object obj, out SqlString value)
        {
            value = (string) obj;
        }

        /// <summary>
        /// Табличный варинат парсинга списков
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        [SqlFunction(
            FillRowMethodName = "FillIList",
            TableDefinition =  "value int"
            )]
        public static IEnumerable listIRead(SqlString tag)
        {
            return tag.Value.SmartSplit();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void FillIList(Object obj, out SqlInt32 value) {
            value = obj.ToInt();
        }
    }
}
