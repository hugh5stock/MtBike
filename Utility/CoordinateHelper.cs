using System;
using System.Collections.Generic;
using System.Linq;

namespace Utility
{
    public class CoordinateHelper
    {
        private const double EARTH_RADIUS = 6378137.0;

        /// <summary>
        /// 根据半径查找经纬度
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="raidus"></param>
        /// <returns></returns>
        public static List<Coordinates> GetDegreeCoordinates(decimal lat, decimal lon, decimal raidus)
        {
            raidus += 29.25m;
            double dlng0 = 2 * Math.Asin(Math.Sin(Convert.ToDouble(raidus) / (2 * EARTH_RADIUS)) / Math.Cos(Convert.ToDouble(lon)));
            dlng0 = degrees(dlng0);
            var dlng = Convert.ToDecimal(dlng0);
            double dlat0 = Convert.ToDouble(raidus) / EARTH_RADIUS;
            dlat0 = degrees(dlat0);
            var dlat = Convert.ToDecimal(dlat0);
            var qdata = new List<Coordinates> { new Coordinates(Math.Round(lon + dlat,6), Math.Round(lat - dlng,6)),
                                  new Coordinates(Math.Round(lon - dlat,6), Math.Round(lat - dlng,6)),
                                  new Coordinates(Math.Round(lon + dlat,6), Math.Round(lat + dlng,6)),
                                  new Coordinates(Math.Round(lon - dlat,6), Math.Round(lat + dlng,6))
            };
            qdata = qdata.OrderByDescending(s => s.Latitude).ThenByDescending(s => s.Longitude).ToList();
            return qdata;
        }

        /// <summary>
        /// 弧度转换为角度数公式
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static double degrees(double d)
        {
            return (d * (180 / Math.PI));
        }

        /**
      * 转化为弧度(rad)
       * */
        private static double rad(double d)
        {
            return d * Math.PI / 180.0;
        }

        /**
 * 基于余弦定理求两经纬度距离
 * @param lon1 第一点的精度
 * @param lat1 第一点的纬度
 * @param lon2 第二点的精度
 * @param lat3 第二点的纬度
 * @return 返回的距离，单位km
 * */
        public static double LantitudeLongitudeDist(double lon1, double lat1, double lon2, double lat2)
        {
            double radLat1 = rad(lat1);
            double radLat2 = rad(lat2);

            double radLon1 = rad(lon1);
            double radLon2 = rad(lon2);

            if (radLat1 < 0)
                radLat1 = Math.PI / 2 + Math.Abs(radLat1);// south
            if (radLat1 > 0)
                radLat1 = Math.PI / 2 - Math.Abs(radLat1);// north
            if (radLon1 < 0)
                radLon1 = Math.PI * 2 - Math.Abs(radLon1);// west
            if (radLat2 < 0)
                radLat2 = Math.PI / 2 + Math.Abs(radLat2);// south
            if (radLat2 > 0)
                radLat2 = Math.PI / 2 - Math.Abs(radLat2);// north
            if (radLon2 < 0)
                radLon2 = Math.PI * 2 - Math.Abs(radLon2);// west
            double x1 = EARTH_RADIUS * Math.Cos(radLon1) * Math.Sin(radLat1);
            double y1 = EARTH_RADIUS * Math.Sin(radLon1) * Math.Sin(radLat1);
            double z1 = EARTH_RADIUS * Math.Cos(radLat1);

            double x2 = EARTH_RADIUS * Math.Cos(radLon2) * Math.Sin(radLat2);
            double y2 = EARTH_RADIUS * Math.Sin(radLon2) * Math.Sin(radLat2);
            double z2 = EARTH_RADIUS * Math.Cos(radLat2);

            double d = Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2) + (z1 - z2) * (z1 - z2));
            //余弦定理求夹角
            double theta = Math.Acos((EARTH_RADIUS * EARTH_RADIUS + EARTH_RADIUS * EARTH_RADIUS - d * d) / (2 * EARTH_RADIUS * EARTH_RADIUS));
            double dist = theta * EARTH_RADIUS;
            return dist;
        }

        /// <summary>
        /// 计算直线距离
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static decimal QDistance(Coordinates start, Coordinates end)
        {
            double lat1 = (Math.PI / 180) * Convert.ToDouble(start.Latitude);
            double lat2 = (Math.PI / 180) * Convert.ToDouble(end.Latitude);
            double lon1 = (Math.PI / 180) * Convert.ToDouble(start.Longitude);
            double lon2 = (Math.PI / 180) * Convert.ToDouble(end.Longitude);
            //地球半径
            double R = 6371;
            //两点间距离 km，如果想要米的话，结果*1000就可以了
            double d = Math.Acos(Math.Sin(lat1) * Math.Sin(lat2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(lon2 - lon1)) * R;
            return Convert.ToDecimal(d * 1000);
        }
    }

    public class Coordinates
    {
        public Coordinates(decimal x, decimal y)
        {
            Longitude = x;
            Latitude = y;
        }

        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
    }
}