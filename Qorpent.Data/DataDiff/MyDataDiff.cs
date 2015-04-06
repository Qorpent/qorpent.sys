using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.IO;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Data.DataDiff
{
    /// <summary>
    /// Упрощает работу с SQL Diff
    /// </summary>
    public class MyDataDiff
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="update"></param>
        /// <param name="fields"></param>
        public static TableDiffGeneratorContext Update(IDbConnection connection, XElement update, params string[] fields)
        {
            if (update.Name.LocalName == "batch") {
                var updateList = new List<DiffPair>();
                var i = 0;
                foreach (var e in update.Elements()) {
                    var name = "dynamic" + i++;
                    var s = GetBaseXml(connection, e, fields);
                    var diff = new DiffPair {FileName = name, Base = s, Updated = e};
                    updateList.Add(diff);
                }
                return Update(connection, updateList.ToArray());
            }
            var baseXml = GetBaseXml(connection, update, fields);
            return Update(connection, baseXml, update);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="basis"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        public static TableDiffGeneratorContext Update(IDbConnection connection, XElement basis, XElement update)
        {
            return Update(connection, new[] {new DiffPair {FileName = "dynamic", Base = basis, Updated = update}});
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        public static TableDiffGeneratorContext Update(IDbConnection connection, DiffPair[] update)
        {
            var context = GenerateContext(update);
            foreach (var script in context.SqlScripts) {
                connection.WellOpen();
                connection.ExecuteNonQuery(script);
            }
            return context;
        }


        /// <summary>
        /// Формирует полный контекст сравнения обновления данных
        /// </summary>
        /// <returns></returns>
        public static TableDiffGeneratorContext GenerateContext(IDbConnection connection, XElement update, params string[] fields)
        {
            if (update.Name.LocalName == "batch")
            {
                var updateList = new List<DiffPair>();
                var i = 0;
                foreach (var e in update.Elements())
                {
                    var name = "dynamic" + i++;
                    var s = GetBaseXml(connection, e, fields);
                    var diff = new DiffPair { FileName = name, Base = s, Updated = e };
                    updateList.Add(diff);
                }
                return GenerateContext(updateList.ToArray());
            }
            var baseXml = GetBaseXml(connection, update, fields);
            return GenerateContext(baseXml, update);
        }

        /// <summary>
        /// Формирует полный контекст сравнения обновления данных
        /// </summary>
        /// <returns></returns>
        public static TableDiffGeneratorContext GenerateContext(XElement basis, XElement update)
        {
            return GenerateContext(new DiffPair[] {new DiffPair{FileName = "dynamic",Base = basis,Updated = update} });
        }

        /// <summary>
        /// Формирует полный контекст сравнения обновления данных
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        public static TableDiffGeneratorContext GenerateContext(DiffPair[] update)
        {
            var tdc = new TableDiffGeneratorContext();
            tdc.DiffPairs = update;
            var dtdg = new DataTableDiffGenerator(tdc);
            dtdg.Generate();
            var sqlg = new SqlDiffGenerator(tdc);
            sqlg.Generate();
            if (tdc.Tables.All(_ => _.Definitions.Count == 0))
            {
                tdc.SqlScripts.Clear();
            }
            return tdc;
        }

        private static XElement GetBaseXml(IDbConnection connection, XElement update, string[] fields) {
            if (!update.Elements().Any()) {
                return new XElement("root");
            }
            var itemname = update.Elements().First().Name.LocalName;
            var table = update.Attr("table");
            var ids = string.Join(",", update.Elements().Select(_ => _.Attr("id")));
            var codes = string.Join(",",update.Elements().Select(_ => "'"+_.Attr("code").ToSqlString()+"'"));
            if (fields.Length == 0) {
                fields =
                    update.Elements()
                        .SelectMany(_ => _.Attributes().Select(__ => __.Name.LocalName))
                        .Distinct()
                        .ToArray();
            }
            if (string.IsNullOrWhiteSpace(ids) && string.IsNullOrWhiteSpace(codes)) {
                return new XElement("root");
            }
            if (string.IsNullOrWhiteSpace(ids)) {
                ids = " 1=1 ";
            }
            else {
                ids = " id IN (" + ids + ")";
            }
            if (string.IsNullOrWhiteSpace(codes))
            {
                codes = " 1=1 ";
            }
            else
            {
                codes = " code IN (" + codes + ") ";
            }
            var flds = string.Join(",", fields.Select(_=>"\""+_+"\""));
            var q  = string.Format( 
@"declare @s nvarchar(max) set @s = (select {0} from {1} where {2} or {3} for xml raw ('{4}'))
declare @x xml set @x = '<root>'+@s+'</root>'
select @x",flds,table,ids,codes,itemname);
            return connection.ExecuteScalar<XElement>(q);

        }
    }
}
