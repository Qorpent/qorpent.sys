using System;
using System.Data.SqlTypes;
using System.Linq;
using Microsoft.SqlServer.Server;

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


    }
}