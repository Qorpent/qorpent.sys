using System;
using System.Data.SqlTypes;
using System.Linq;
using Microsoft.SqlServer.Server;
using Microsoft.SqlServer.Types;

namespace Qorpent.Integration.SqlExtensions {
    /// <summary>
    /// </summary>
    public static class SpatialFunctions {
        const double Pole = 20037508.34;

        /// <summary>
        /// 	evaluates usual .net regex match
        /// </summary>
        /// <param name="lon"></param>
        /// <returns> </returns>
        [SqlFunction(IsDeterministic = true, SystemDataAccess = SystemDataAccessKind.None, DataAccess = DataAccessKind.None)]
        public static SqlDouble GetX(SqlDouble lon) {
            
            return lon * Pole / 180;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lat"></param>
        /// <returns></returns>
        [SqlFunction(IsDeterministic = true, SystemDataAccess = SystemDataAccessKind.None, DataAccess = DataAccessKind.None)]
        public static SqlDouble GetY(SqlDouble lat) {
            return Math.Log(Math.Tan((double) ((90 + lat) * Math.PI / 360))) / Math.PI * Pole;
        }
        
        /// <summary>
        /// 	evaluates usual .net regex match
        /// </summary>
        /// <param name="x"></param>
        /// <returns> </returns>
        [SqlFunction(IsDeterministic = true, SystemDataAccess = SystemDataAccessKind.None, DataAccess = DataAccessKind.None)]
        public static SqlDouble GetLon(SqlDouble x) {
            return  x*180/Pole;
        }

       /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        [SqlFunction(IsDeterministic = true, SystemDataAccess = SystemDataAccessKind.None, DataAccess = DataAccessKind.None)]
        public static SqlDouble GetLat(SqlDouble y) {
            return  180 / Math.PI * (2 * Math.Atan(Math.Exp((double) ((y / Pole) * Math.PI))) - Math.PI / 2);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="bx"></param>
        /// <param name="by"></param>
        /// <param name="tx"></param>
        /// <param name="ty"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        [SqlFunction(IsDeterministic = true, SystemDataAccess = SystemDataAccessKind.None,
            DataAccess = DataAccessKind.None)]
        public static SqlBoolean InBounds(SqlDouble x, SqlDouble y, SqlDouble bx, SqlDouble by, SqlDouble tx,
            SqlDouble ty, SqlDouble distance) {
            if (x.IsNull ||x.Value==0) return SqlBoolean.False;
            if (y.IsNull || y.Value == 0) return SqlBoolean.False;
            if (bx.IsNull || bx.Value == 0) return SqlBoolean.False;
            if (by.IsNull || bx.Value == 0) return SqlBoolean.False;

            if (tx.IsNull || tx.Value == 0)
            {
                if (distance.IsNull || distance.Value == 0) {
                    return SqlBoolean.False;
                }
                return Math.Sqrt(Math.Pow(bx.Value - x.Value, 2) + Math.Pow(@by.Value - y.Value, 2)) <=
                       distance.Value;
            }
            if (ty.IsNull || ty.Value == 0) return SqlBoolean.False;
            var minx = Math.Min(bx.Value, tx.Value);
            var miny = Math.Min(@by.Value, ty.Value);
            var maxx = Math.Max(bx.Value, tx.Value);
            var maxy = Math.Max(@by.Value, ty.Value);
            return x.Value >= minx && x.Value <= maxx && y.Value >= miny && y.Value <= maxy;
        }
        /// <summary>
        /// Формирует точку в проекции 4236 с резолюцией X,Y или Lon , Lat на входе
        /// </summary>
        /// <param name="xOrLon"></param>
        /// <param name="yOrLat"></param>
        /// <returns></returns>
        [SqlFunction(IsDeterministic = true, SystemDataAccess = SystemDataAccessKind.None,
            DataAccess = DataAccessKind.None)]
        public static SqlGeography GetPoint(SqlDouble xOrLon, SqlDouble yOrLat) {
            if (xOrLon.IsNull || yOrLat.IsNull) return SqlGeography.Null;
            var builder = new SqlGeographyBuilder();
            builder.SetSrid(4236);
            builder.BeginGeography(OpenGisGeographyType.Point);
            

            if (Math.Abs(xOrLon.Value) <= 180 && Math.Abs(yOrLat.Value) <= 180) {
                builder.BeginFigure(yOrLat.Value, xOrLon.Value);

            }
            else {
                builder.BeginFigure(GetLat(yOrLat).Value,GetLon(xOrLon).Value);
            }
            builder.EndFigure();
            builder.EndGeography();
            return builder.ConstructedGeography;
        }
    }
}