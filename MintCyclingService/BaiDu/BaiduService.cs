using MintCyclingData;
using MintCyclingService.Breakdown;
using MintCyclingService.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static MintCyclingService.Cycling.BaiduModel;

namespace MintCyclingService.BaiDu
{
    public class BaiduService:IBaiduService
    {
        private static string ak = System.Web.Configuration.WebConfigurationManager.AppSettings["AK"];                      //AK

        #region 百度API接口



        public string GetAddress(string lng, string lat)
        {
            string Address = string.Empty;
            string strUrl = @"http://api.map.baidu.com/geocoder/v2/?ak=" + ak + "&callback=renderReverse&location=" + lat + "," + lng + @"&output=json&pois=0";
            //WebRequest request = WebRequest.Create(url);
            //request.Method = "POST";
            //XmlDocument xmlDoc = new XmlDocument();
            //string sendData = xmlDoc.InnerXml;
            //byte[] byteArray = Encoding.Default.GetBytes(sendData);

            //Stream dataStream = request.GetRequestStream();
            //dataStream.Write(byteArray, 0, byteArray.Length);
            //dataStream.Close();

            //WebResponse response = request.GetResponse();
            //dataStream = response.GetResponseStream();
            //StreamReader reader = new StreamReader(dataStream, System.Text.Encoding.GetEncoding("utf-8"));

            string strResult;
            try
            {
                //创建远程服务请求
                WebRequest request = WebRequest.Create(strUrl);
                request.Timeout = 12000;
                request.Method = "POST";

                HttpWebResponse HttpWResp = (HttpWebResponse)request.GetResponse();
                Stream myStream = HttpWResp.GetResponseStream();
                StreamReader reader = new StreamReader(myStream, System.Text.Encoding.GetEncoding("utf-8"));
                strResult = reader.ReadToEnd();

                //StringBuilder strBuilder = new StringBuilder();
                //while (-1 != reader.Peek())
                //{
                //    strBuilder.Append(reader.ReadLine());
                //}
                //strResult = strBuilder.ToString();

                #region 解析Json

                strResult = strResult.Remove(strResult.Length - 1).Remove(0, 29);
                JObject jo = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(strResult);
                string Status = jo["status"].ToString();
                if (Status == "0")
                {
                    //地址拼接
                    strResult = jo["result"]["formatted_address"].ToString() + jo["result"]["sematic_description"].ToString();
                }
                else if (Status == "1")
                {
                    strResult = "服务器内部错误";
                }
                else if (Status == "2")
                {
                    strResult = "请求参数非法";
                }
                else if (Status == "101")
                {
                    strResult = "服务禁用";
                }

                #endregion 解析Json
            }
            catch (Exception exp)
            {
                strResult = "错误：" + exp.Message;
            }
            return strResult;
        }



        public List<AddressModel> GetAddressInfo(string lng, string lat)
        {
            List<AddressModel> list = new List<AddressModel>();

            string Address = string.Empty;
            string strUrl = @"http://api.map.baidu.com/geocoder/v2/?ak=" + ak + "&callback=renderReverse&location=" + lat + "," + lng + @"&output=json&pois=0";
            //WebRequest request = WebRequest.Create(url);
            //request.Method = "POST";
            //XmlDocument xmlDoc = new XmlDocument();
            //string sendData = xmlDoc.InnerXml;
            //byte[] byteArray = Encoding.Default.GetBytes(sendData);

            //Stream dataStream = request.GetRequestStream();
            //dataStream.Write(byteArray, 0, byteArray.Length);
            //dataStream.Close();

            //WebResponse response = request.GetResponse();
            //dataStream = response.GetResponseStream();
            //StreamReader reader = new StreamReader(dataStream, System.Text.Encoding.GetEncoding("utf-8"));

            string strResult;
            try
            {
                //创建远程服务请求
                WebRequest request = WebRequest.Create(strUrl);
                request.Timeout = 12000;
                request.Method = "POST";

                HttpWebResponse HttpWResp = (HttpWebResponse)request.GetResponse();
                Stream myStream = HttpWResp.GetResponseStream();
                StreamReader reader = new StreamReader(myStream, System.Text.Encoding.GetEncoding("utf-8"));
                strResult = reader.ReadToEnd();

                //StringBuilder strBuilder = new StringBuilder();
                //while (-1 != reader.Peek())
                //{
                //    strBuilder.Append(reader.ReadLine());
                //}
                //strResult = strBuilder.ToString();

                #region 解析Json

                strResult = strResult.Remove(strResult.Length - 1).Remove(0, 29);
                JObject jo = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(strResult);
                string Status = jo["status"].ToString();
                if (Status == "0")
                {
                    AddressModel model = new AddressModel();

                    model.provinceName = jo["result"]["addressComponent"]["province"].ToString();     //省
                    model.city = jo["result"]["addressComponent"]["city"].ToString();      //市
                    model.district = jo["result"]["addressComponent"]["district"].ToString();         //区
                    model.Address = jo["result"]["formatted_address"].ToString() + jo["result"]["sematic_description"].ToString(); //组合的详细地址
                    if (string.IsNullOrEmpty(model.provinceName) || string.IsNullOrEmpty(model.provinceName) || string.IsNullOrEmpty(model.provinceName))
                    {
                        return list;
                    }
                    else
                    {
                        list.Add(model);
                    }
                }
                else if (Status == "1")
                {
                    strResult = "服务器内部错误";
                }
                else if (Status == "2")
                {
                    strResult = "请求参数非法";
                }
                else if (Status == "101")
                {
                    strResult = "服务禁用";
                }

                #endregion 解析Json
            }
            catch (Exception exp)
            {
                strResult = "错误：" + exp.Message;
            }
            return list;
        }


        public   Latlng GetLJByAdress(string Address)
        {
            var res = new Latlng();
            string strUrl = @"http://api.map.baidu.com/geocoder/v2/?callback=renderOption&output=json&address=" + Address + "&ak=" + ak;
            //WebRequest request = WebRequest.Create(url);
            //request.Method = "POST";
            //XmlDocument xmlDoc = new XmlDocument();
            //string sendData = xmlDoc.InnerXml;
            //byte[] byteArray = Encoding.Default.GetBytes(sendData);

            //Stream dataStream = request.GetRequestStream();
            //dataStream.Write(byteArray, 0, byteArray.Length);
            //dataStream.Close();

            //WebResponse response = request.GetResponse();
            //dataStream = response.GetResponseStream();
            //StreamReader reader = new StreamReader(dataStream, System.Text.Encoding.GetEncoding("utf-8"));

            string strResult;
            try
            {
                //创建远程服务请求
                WebRequest request = WebRequest.Create(strUrl);
                request.Timeout = 12000;
                request.Method = "POST";

                HttpWebResponse HttpWResp = (HttpWebResponse)request.GetResponse();
                Stream myStream = HttpWResp.GetResponseStream();
                StreamReader reader = new StreamReader(myStream, System.Text.Encoding.GetEncoding("utf-8"));
                strResult = reader.ReadToEnd();

                //StringBuilder strBuilder = new StringBuilder();
                //while (-1 != reader.Peek())
                //{
                //    strBuilder.Append(reader.ReadLine());
                //}
                //strResult = strBuilder.ToString();

                #region 解析Json

                strResult = strResult.Remove(strResult.Length - 1).Remove(0, 29);
                JObject jo = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(strResult);
                string Status = jo["status"].ToString();

                if (Status == "0")
                {
                    res.IsSuccess = true;
                    res.Lat = jo["result"]["location"]["lat"].ToString();
                    res.Lng = jo["result"]["location"]["lng"].ToString();
                    return res;
                }
                else if (Status == "1")
                {
                    res.IsSuccess = false;
                    res.message = "服务器内部错误";
                }
                else if (Status == "2")
                {
                    res.IsSuccess = false;
                    res.message = "请求参数非法";
                }
                else if (Status == "101")
                {
                    res.IsSuccess = false;
                    res.message = "服务禁用";
                }

                #endregion 解析Json
            }
            catch (Exception exp)
            {
                res.IsSuccess = false;
                res.message = "错误：" + exp.Message;
            }
            return res;
        }



        public string GetDistanceTime(string origins, string destinations, out double distance, out double time)
        {
            distance = 0;
            time = 0;
            string Address = string.Empty;
            string strUrl = @"http://api.map.baidu.com/routematrix/v2/walking?output=json&origins=" + origins + "&destinations=" + destinations + "&ak=" + ak + "";

            string statusStr = string.Empty;
            string strResult;
            try
            {
                //创建远程服务请求
                WebRequest request = WebRequest.Create(strUrl);
                request.Timeout = 12000;
                request.Method = "GET";

                HttpWebResponse HttpWResp = (HttpWebResponse)request.GetResponse();
                Stream myStream = HttpWResp.GetResponseStream();
                StreamReader reader = new StreamReader(myStream, System.Text.Encoding.GetEncoding("utf-8"));
                strResult = reader.ReadToEnd();

                #region 解析Json

                //strResult = strResult.Remove(strResult.Length - 1).Remove(0, 29);
                JObject jo = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(strResult);
                string Status = jo["status"].ToString();
                if (Status == "0")
                {
                    JArray arry = (JArray)jo["result"]; //
                    foreach (var item in arry)
                    {
                        //distance = item["distance"]["text"].ToString();  //默认单位为米
                        //time = item["duration"]["text"].ToString();       //秒转换成分钟
                        distance = double.Parse(item["distance"]["value"].ToString());   //默认单位为米
                        time = Math.Round(double.Parse(item["duration"]["value"].ToString()) / 60, 0);       //秒转换成分钟
                    }
                    statusStr = "成功";
                }
                else if (Status == "1")
                {
                    statusStr = "服务器内部错误";
                }
                else if (Status == "2")
                {
                    statusStr = "请求参数非法";
                }

                #endregion 解析Json
            }
            catch (Exception exp)
            {
                statusStr = "错误：" + exp.Message;
            }
            return statusStr;
        }

        public string GetRidingDistance(string origins, string destinations, out double distance, out double time)
        {
            distance = 0;
            time = 0;
            string Address = string.Empty;
            string strUrl = @"http://api.map.baidu.com/routematrix/v2/riding?output=json&origins=" + origins + "&destinations=" + destinations + "&ak=" + ak + "";

            string statusStr = string.Empty;
            string strResult;
            try
            {
                //创建远程服务请求
                WebRequest request = WebRequest.Create(strUrl);
                request.Timeout = 12000;
                request.Method = "GET";
           

                HttpWebResponse HttpWResp = (HttpWebResponse)request.GetResponse();
                Stream myStream = HttpWResp.GetResponseStream();
                StreamReader reader = new StreamReader(myStream, System.Text.Encoding.GetEncoding("utf-8"));
                strResult = reader.ReadToEnd();

                #region 解析Json

                //strResult = strResult.Remove(strResult.Length - 1).Remove(0, 29);
                JObject jo = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(strResult);
                string Status = jo["status"].ToString();
                if (Status == "0")
                {
                    JArray arry = (JArray)jo["result"]; //
                    foreach (var item in arry)
                    {
                        distance = double.Parse(item["distance"]["value"].ToString());                        //默认单位为米
                        time = Math.Round(double.Parse(item["duration"]["value"].ToString()) / 60, 0);       //秒转换成分钟
                    }
                    statusStr = "成功";
                }
                else if (Status == "1")
                {
                    statusStr = "服务器内部错误";
                }
                else if (Status == "2")
                {
                    statusStr = "请求参数非法";
                }

                #endregion 解析Json
            }
            catch (Exception exp)
            {
                statusStr = "错误：" + exp.Message;
            }
            return statusStr;
        }

        /// <summary>
        /// 调用百度API--查询省市区ID编号
        /// </summary>
        /// <returns></returns>
        public List<AddressIDList> GetBaiDuProvinceID(BaiduApiModel model)
        {
            var result = new ResultModel();
            List<AddressIDList> List = new List<AddressIDList>();



            return List;
        }


        #endregion 百度API接口

    }
}
