using System;
using System.Data.SqlTypes;

namespace Qorpent.Utils {
    /// <summary>
    /// </summary>
    public static class GeoProjectionHelper {
        const double Pole = 20037508.34;


        /// <summary>
        /// 	evaluates usual .net regex match
        /// </summary>
        /// <param name="lon"></param>
        /// <returns> </returns>
        public static double GetX(double lon) {
            
            return lon * Pole / 180;
        }
        /// <summary>
        /// 	evaluates usual .net regex match
        /// </summary>
        /// <param name="lon"></param>
        /// <returns> </returns>
        public static decimal GetX(decimal lon)
        {

            return Convert.ToDecimal(GetX(Convert.ToDouble(lon)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lat"></param>
        /// <returns></returns>
        public static double GetY(double lat)
        {
            return Math.Log(Math.Tan((double) ((90 + lat) * Math.PI / 360))) / Math.PI * Pole;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lat"></param>
        /// <returns></returns>
        public static decimal GetY(decimal lat) {
            return Convert.ToDecimal(GetY(Convert.ToDouble(lat)));
        }
        
        /// <summary>
        /// 	evaluates usual .net regex match
        /// </summary>
        /// <param name="x"></param>
        /// <returns> </returns>
        public static double GetLon(double x)
        {
            return  x*180/Pole;
        }

        /// <summary>
        /// 	evaluates usual .net regex match
        /// </summary>
        /// <param name="x"></param>
        /// <returns> </returns>
        public static decimal GetLon(decimal x)
        {
            return Convert.ToDecimal(GetLon(Convert.ToDouble(x)));
        }

       /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>

        public static double GetLat(double y)
        {
            return  180 / Math.PI * (2 * Math.Atan(Math.Exp((double) ((y / Pole) * Math.PI))) - Math.PI / 2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>

        public static decimal GetLat(decimal y)
        {
            return Convert.ToDecimal(GetLat(Convert.ToDouble(y)));
        }
    }
}