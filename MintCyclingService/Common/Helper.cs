using MintCyclingService.Common;
using MintCyclingService.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Web.Script.Serialization;

namespace Utility.Common
{
    public class Helper
    {
        /// <summary>
        /// 比对电话号码,如果比对功,则对电话号码进行Mask
        /// </summary>
        /// <param name="templatePhone">样板号码</param>
        /// <param name="maskPhone">需Mask的号码</param>
        /// <returns>处理后的号码</returns>
        public static string MaskPhoneNoStringToMask(string templatePhone, string maskPhone)
        {
            if (templatePhone == maskPhone)
            {
                maskPhone = maskPhone.Substring(0, 3) + "****" + maskPhone.Substring(7,4);
            }

            return maskPhone;
        }

        /// <summary>
        /// Transfer UNIX time stamp to .net datetime
        /// </summary>
        /// <param name="time">UNIX time stamp</param>
        /// <returns> .net datetime</returns>
        public static ResultModel UNIXTimeToDateTime(string strTime)
        {
            var result = new ResultModel();
            long time;

            if (strTime.Length != 10)
            {
                result.IsSuccess = false;
                result.MsgCode = ResPrompt.UnixTimeError;
                result.Message = ResPrompt.UnixTimeErrorMessage;
                return result;
            }

            if (!long.TryParse(strTime, out time))
            {
                result.IsSuccess = false;
                result.MsgCode = ResPrompt.UnixTimeError;
                result.Message = ResPrompt.UnixTimeErrorMessage;
                return result;
            }


            var timeStamp = new DateTime(1970, 1, 1); //得到1970年的时间戳
            var t = (time + 8 * 60 * 60) * 10000000 + timeStamp.Ticks;
            var dt = new DateTime(t);

            result.ResObject = dt;

            return result;
        }

        /// <summary>
        /// 获取10位UNIX时间戳
        /// </summary>
        /// <param name="dt">当前时间</param>
        /// <returns>返回10位UNIX时间戳</returns>
        public static string DateTimeToUNIXTime(DateTime dt)
        {
            // Default implementation of UNIX time of the current UTC time
            TimeSpan ts = dt.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        /// 检验时间与当前时间比较是否过期
        /// </summary>
        /// <param name="time">被检验时间</param>
        /// <param name="dt">当前时间</param>
        /// <param name="secScope">有效时间范围</param>
        /// <returns>时间与当前时间比较是否过期</returns>
        public static ResultModel ValidateUNIXTimeInTimeScope(DateTime time, DateTime dt, int secScope)
        {
            var result = new ResultModel();

            if (time > dt.AddSeconds(secScope) || time < dt.AddSeconds(-secScope))
            {
                result.IsSuccess = false;
                result.MsgCode = ResPrompt.UnixTimeExpired;
                result.Message = ResPrompt.UnixTimeExpiredMessage;
                return result;
            }

            return result;
        }

        /// <summary>
        /// 根据模板生成编码
        /// 模板占位符:{0},显示indexNo;\yy\,显示2位年;\yyyy\,显示4位年;\M\,显示月;\MM\,显示2位月;\d\,显示日;\dd\显示2位日;
        /// \H\,显示小时;\HH\,显示2位小时;\m\,显示分;\mm\,显示2位分;\s\,显示秒;\ss\,显示2位秒;
        /// </summary>
        /// <param name="template">编码模板</param>
        /// <param name="indexNo">序号</param>
        /// <param name="bitOfIndexNo">序号位数</param>
        /// <param name="dt">生成编码的日期,如无时间占位符,则填写null</param>
        /// <returns>根据模板生成编码</returns>
        public static string GenerateCodeByTemplate(string template, int indexNo, int bitOfIndexNo, DateTime? dt)
        {
            if (dt.HasValue)
            {
                var now = DateTime.Parse(dt.Value.ToString("yyyy-MM-dd HH:mm:ss"));

                // 将\yy\标记转换为2位年份
                template = template.Replace(@"\yy\", now.Year.ToString().Substring(2));

                // 将\yyyy\标记转换为4位年份
                template = template.Replace(@"\yyyy\", now.Year.ToString());

                // 将\M\标记转换为月份
                template = template.Replace(@"\M\", now.Month.ToString());

                // 将\MM\标记转换为至少2位月份
                template = template.Replace(@"\MM\", now.Month.ToString().PadLeft(2, '0'));

                // 将\d\标记转换为日
                template = template.Replace(@"\d\", now.Day.ToString());

                // 将\dd\标记转换为至少2位日
                template = template.Replace(@"\dd\", now.Day.ToString().PadLeft(2, '0'));

                // 将\H\标记转换为小时
                template = template.Replace(@"\H\", now.Hour.ToString());

                // 将\HH\标记转换为至少2位小时
                template = template.Replace(@"\HH\", now.Hour.ToString().PadLeft(2, '0'));

                // 将\m\标记转换为分钟
                template = template.Replace(@"\m\", now.Minute.ToString());

                // 将\mm\标记转换为2位分钟
                template = template.Replace(@"\mm\", now.Minute.ToString().PadLeft(2, '0'));

                // 将\s\标记转换为秒
                template = template.Replace(@"\s\", now.Minute.ToString());

                // 将\ss\标记转换为至少2位秒
                template = template.Replace(@"\ss\", now.Minute.ToString().PadLeft(2, '0'));
            }

            if (template.Contains("{0}"))
            {
                template = string.Format(template, indexNo.ToString().PadLeft(bitOfIndexNo, '0'));
            }
            else
            {
                template += indexNo.ToString().PadLeft(bitOfIndexNo, '0');
            }


            return template;
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>序列化后的字符串</returns>
        public static string SerializerObject(object obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(obj);

            return json;
        }

        /// <summary>
        /// 解析JSON对象
        /// </summary>
        /// <typeparam name="T">解析目标对象</typeparam>
        /// <param name="json">JSON数据</param>
        /// <returns>目标对象</returns>
        public static T DeserializeJsonToObject<T>(string json)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Deserialize<T>(json);
        }

        /// <summary>
        /// 创建验证码字符
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string CreateVerificationText(int length)
        {
            char[] _verification = new char[length];
            char[] _dictionary = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            Random _random = new Random();
            for (int i = 0; i < length; i++) { _verification[i] = _dictionary[_random.Next(_dictionary.Length - 1)]; }
            return new string(_verification);
        }

        /// <summary>
        /// 创建验证码图片
        /// </summary>
        /// <param name="verificationText">验证码字符串</param>
        /// <param name="width">图片宽度</param>
        /// <param name="height">图片长度</param>
        /// <returns>图片</returns>
        public static Bitmap CreateVerificationImage(string verificationText, int width, int height)
        {
            Pen _pen = new Pen(Color.Black);
            Font _font = new Font("Arial", 14, FontStyle.Bold);
            Brush _brush = null;
            Bitmap _bitmap = new Bitmap(width, height);
            Graphics _g = Graphics.FromImage(_bitmap);
            SizeF _totalSizeF = _g.MeasureString(verificationText, _font);
            SizeF _curCharSizeF;
            PointF _startPointF = new PointF((width - _totalSizeF.Width) / 2, (height - _totalSizeF.Height) / 2);
            //随机数产生器
            Random _random = new Random();
            _g.Clear(Color.White);
            for (int i = 0; i < verificationText.Length; i++)
            {
                _brush = new LinearGradientBrush(new Point(0, 0), new Point(1, 1), Color.FromArgb(_random.Next(255), _random.Next(255), _random.Next(255)), Color.FromArgb(_random.Next(255), _random.Next(255), _random.Next(255)));
                _g.DrawString(verificationText[i].ToString(), _font, _brush, _startPointF);
                _curCharSizeF = _g.MeasureString(verificationText[i].ToString(), _font);
                _startPointF.X += _curCharSizeF.Width;
            }
            _g.Dispose();
            return _bitmap;
        }

        /// <summary>
        /// 生成指定位数的随机数字字符串
        /// </summary>
        /// <param name="bitOfString">指定位数</param>
        /// <returns>指定位数的随机数字字符串</returns>
        public static string GetRandomNumString(int bitOfString)//生成数字随机数
        {
            string a = "0123456789";
            var sb = new StringBuilder();
            for (int i = 0; i < bitOfString; i++)
            {
                sb.Append(a[new Random(Guid.NewGuid().GetHashCode()).Next(0, a.Length - 1)]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 角度数转换为弧度公式
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static double radians(double d)
        {
            return d * Math.PI / 180.0;
        }

        private const double qtEARTH_RADIUS = 6378137.0;
        public static double GetDistanceGoogle(double lat1, double lng1, double lat2, double lng2)
        {
            double radLat1 = radians(lat1);
            double radLng1 = radians(lng1);
            double radLat2 = radians(lat2);
            double radLng2 = radians(lng2);

            double s = Math.Acos(Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Cos(radLng1 - radLng2) + Math.Sin(radLat1) * Math.Sin(radLat2));
            s = s * qtEARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000 / 1000;
            return double.IsNaN(s) ? 0 : s;
        }

        /// <summary>
        /// 弧度转换为角度数公式
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static double degrees(double d)
        {
            return d * (180 / Math.PI);
        }

        /// <summary>
        /// 根据半径查找经纬度
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="raidus"></param>
        /// <returns></returns>
        public static List<Coordinates> GetDegreeCoordinates(double lat, double lon, double raidus)
        {
            raidus += 29.25;
            double dlng = 2 * Math.Asin(Math.Sin(raidus / (2 * qtEARTH_RADIUS)) / Math.Cos(lon));
            dlng = degrees(dlng);

            double dlat = raidus / qtEARTH_RADIUS;
            dlat = degrees(dlat);

            return new List<Coordinates> { new Coordinates(Convert.ToDecimal(Math.Round(lon + dlat,6)), Convert.ToDecimal(Math.Round(lat - dlng,6))),
                                  new Coordinates(Convert.ToDecimal(Math.Round(lon - dlat,6)), Convert.ToDecimal(Math.Round(lat - dlng,6))),
                                  new Coordinates(Convert.ToDecimal(Math.Round(lon + dlat,6)), Convert.ToDecimal(Math.Round(lat + dlng,6))),
                                  new Coordinates(Convert.ToDecimal(Math.Round(lon - dlat,6)), Convert.ToDecimal(Math.Round(lat + dlng,6)))
            };
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
