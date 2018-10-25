using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class CommonHelper
    {
        public static string PostWebRequest(string postUrl, string paramData)
        {
            string ret = string.Empty;
            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(paramData); //转化
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";

                webReq.ContentLength = byteArray.Length;
                Stream newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                newStream.Close();
                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.Default);
                ret = sr.ReadToEnd();
                sr.Close();
                response.Close();
                newStream.Close();
            }
            catch
            {

            }
            return ret;
        }

        /// <summary>
        /// GET请求与获取结果
        /// </summary>
        public static string HttpGet(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        public static async Task<string> APIPost(string url, string data)
        {
            string result = string.Empty;
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            using (var http = new HttpClient(handler))
            {
                var content = new FormUrlEncodedContent(new Dictionary<string, string>()
                {
                  {"", data}
                 });

                var response = await http.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                result = await response.Content.ReadAsStringAsync();
            }
            return result;
        }

        public static string DtToString(DateTime? dt, bool isShortType, string strformat = "")
        {
            if (!dt.HasValue) return string.Empty;
            var st = string.IsNullOrEmpty(strformat) ? (isShortType ? "yyyy-MM-dd" : "yyyy-MM-dd HH:mm:ss") : strformat;
            return dt.Value.ToString(st);
        }

        public static string CalcTimeSpan(DateTime? dt0, DateTime? dt1)
        {
            if (!dt0.HasValue || !dt1.HasValue) return string.Empty;
            var tk = dt0 > dt1 ? dt0.Value.Subtract(dt1.Value) : dt1.Value.Subtract(dt0.Value);
            var ts = tk.Hours;
            var str = string.Format("{0}{1}分钟", ts >= 1 ? ts + "小时" : "", tk.Minutes);
            return str;
        }

        /// <summary>
        /// 计算行程的骑行总时间
        /// </summary>
        /// <param name="TotalMinSpan"></param>
        /// <returns></returns>
        public static string CalcTimeSpanMyTravel(decimal TotalMinSpan)
        {
            //if (!dt0.HasValue || !dt1.HasValue) return string.Empty;
            //var tk = dt0 > dt1 ? dt0.Value.Subtract(dt1.Value) : dt1.Value.Subtract(dt0.Value);
            //var ts = tk.Hours;
            //var str = string.Format("{0}{1}分钟", ts >= 1 ? ts + "小时" : "", tk.Minutes);
            TimeSpan tmSpan = TimeSpan.FromMinutes(Convert.ToDouble(TotalMinSpan));
            var ts = tmSpan.Hours;
            var tmin = tmSpan.Minutes;
            var tsec=tmSpan.Seconds<1?1:tmSpan.Seconds; 
            var str = string.Format("{0}{1}{2}", ts >= 1 ? ts + "小时" : "", tmin >= 1 ? tmin + "分钟" : "",  tsec + "秒");
            return str;
        }



    }
}
