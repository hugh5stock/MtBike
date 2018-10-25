using MintCyclingService.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace MintCyclingService.JPush
{
    /// <summary>
    /// 极光推送实现类
    /// </summary>
    public class JPushService : IJPushService
    {
        /// <summary>
        /// 读取配置文件信息
        /// </summary>
        private readonly string ApiKey = ConfigurationManager.AppSettings["App_Key"].ToString();                    //Android ApiKey
        private readonly string APIMasterSecret = ConfigurationManager.AppSettings["Master_Secret"].ToString();    //Android密码
        private readonly string RequestUrl = "https://api.jpush.cn/v3/push";         //极光推送请求的url地址


        #region V3JPush

        /// <summary>
        /// 发送Post请求
        /// </summary>
        /// <param name="reqParams">请求的json参数，一般由Platform(平台)、Audience(设备对象标识)、Notification(通知)、Message(自定义消息)、Options(推送可选项)组成</param>
        /// <returns></returns>
        public string SendPostRequest(String content, List<string> registrationList)
        {
            string auth = GetBase64Auth();
            return SendRequest("POST", this.RequestUrl, auth, content, registrationList);
        }


        /// <summary>
        /// 发送推送请求到JPush，使用HttpWebRequest
        /// 自定义消息
        /// </summary>
        /// <param name="method">传入POST或GET</param>
        /// <param name="url">固定地址</param>
        /// <param name="auth">用户名AppKey和密码MasterSecret形成的Base64字符串</param>
        /// <param name="reqParams">请求的json参数，一般由Platform(平台)、Audience(设备对象标识)、Notification(通知)、Message(自定义消息)、Options(推送可选项)组成</param>
        /// <returns></returns>
        public string SendRequest(String method, String url, String auth, String reqParams, List<string> registrationList)
        {
            if (registrationList == null || !registrationList.Any()) //支持给多用户主动发送消息
                return string.Empty;
            string resultJson = "";
            HttpWebRequest myReq = null;
            HttpWebResponse response = null;
            string t = "all";

            string registration_id = "";
            registrationList.ForEach(s =>
            {
                registration_id += "\"" + s + "\",";
            });
            registration_id = registration_id.Substring(0, registration_id.Length - 1);
            //传参必必须是Json字符串格式的
            String jsonPost = "{\"platform\":\"" + t + "\",\"audience\" : {\"registration_id\" : [" + registration_id + "]},\"message\": {\"msg_content\": \"" + reqParams + "\",\"content_type\": \"text\", \"title\": \"msg\"}}";

            try
            {
                myReq = (HttpWebRequest)WebRequest.Create(url);
                myReq.Method = method;
                myReq.ContentType = "application/json";

                if (!String.IsNullOrEmpty(auth))
                {
                    myReq.Headers.Add("Authorization", "Basic " + auth);
                }
                if (method == "POST")
                {
                    byte[] bs = UTF8Encoding.UTF8.GetBytes(jsonPost);
                    myReq.ContentLength = bs.Length;
                    using (Stream reqStream = myReq.GetRequestStream())
                    {
                        reqStream.Write(bs, 0, bs.Length);
                        reqStream.Close();
                    }
                }

                response = (HttpWebResponse)myReq.GetResponse();
                HttpStatusCode statusCode = response.StatusCode;
                if (Equals(response.StatusCode, HttpStatusCode.OK))
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8))
                    {
                        resultJson = reader.ReadToEnd();
                        try
                        {
                            object json = Newtonsoft.Json.JsonConvert.DeserializeObject(resultJson);
                            resultJson = string.Format("{{\"error\": {{\"message\": \"{0}\", \"code\": 200}}}}", "自定义消息推送成功");
                        }
                        catch
                        {
                            resultJson = string.Format("{{\"error\": {{\"message\": \"{0}\", \"code\": 10086}}}}", "响应的结果不是正确的json格式");
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                #region 异常处理
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpStatusCode errorCode = ((HttpWebResponse)ex.Response).StatusCode;
                    string statusDescription = ((HttpWebResponse)ex.Response).StatusDescription;
                    using (StreamReader sr = new StreamReader(((HttpWebResponse)ex.Response).GetResponseStream(), System.Text.Encoding.UTF8))
                    {
                        resultJson = sr.ReadToEnd();

                        //{"errcode":404,"errmsg":"request api doesn't exist"}

                        Dictionary<string, object> dict = JsonToDictionary(resultJson);
                        string errCode = "10086";

                        string errMsg = "发送推送的请求地址不存在或无法连接";
                        if (dict.ContainsKey("errcode"))
                        {
                            errCode = dict["errcode"].ToString();
                        }
                        if (dict.ContainsKey("errmsg"))
                        {
                            errMsg = dict["errmsg"].ToString();
                        }
                        resultJson = string.Format("{{\"error\": {{\"message\": \"{0}\", \"code\": {1}}}}}", errMsg, errCode);
                    }
                }
                else
                {
                    //这里一定是error作为键名（自定义错误号10086），和极光推送失败时的json格式保持一致 如 {"error": {"message": "Missing parameter", "code": 1002}}
                    resultJson = string.Format("{{\"error\": {{\"message\": \"{0}\", \"code\": 10086}}}}", ex.Message.Replace("\"", " ").Replace("'", " "));
                }
                #endregion
            }
            catch (System.Exception ex)
            {
                resultJson = string.Format("{{\"error\": {{\"message\": \"{0}\", \"code\": 10086}}}}", ex.Message.Replace("\"", " ").Replace("'", " "));
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
                if (myReq != null)
                {
                    myReq.Abort();
                }
            }
            return resultJson;
        }

        /// <summary>
        /// 通过用户名AppKey和密码获取验证码
        /// HTTP Basic Authentication认证
        /// </summary>
        /// <returns></returns>
        private string GetBase64Auth()
        {
            string str = this.ApiKey + ":" + this.APIMasterSecret;
            byte[] bytes = Encoding.Default.GetBytes(str);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 发送推送请求到JPush
        /// </summary>
        /// <param name="method">POST或GET</param>
        /// <param name="content">请求的json参数，一般由Platform(平台)、Audience(设备对象标识)、Notification(通知)、Message(自定义消息)、Options(推送可选项)组成</param>
        /// <returns></returns>

        public string SendRequest(String method, String content, List<string> registrationList)
        {
            string auth = GetBase64Auth();
            return SendRequest(method, this.RequestUrl, auth, content, registrationList);
        }


        /// <summary>
        /// 发送Get请求
        /// </summary>
        /// <param name="content">请求的json参数，一般由Platform(平台)、Audience(设备对象标识)、Notification(通知)、Message(自定义消息)、Options(推送可选项)组成</param>
        /// <returns></returns>
        public string SendGetRequest(String content, List<string> registrationList)
        {
            string auth = GetBase64Auth();
            return SendRequest("GET", this.RequestUrl, auth, content, registrationList);
        }

        /*
         * 生成唯一的sendNo的方法： 取序列号
         * 查看返回结果的方法
        */
        ///// <summary>
        ///// 查询推送的结果
        ///// </summary>
        ///// <param name="msg_ids">生成的json信息唯一id</param>
        ///// <returns></returns>
        //public string GetReceivedResult(String msg_ids)
        //{
        //    string url = this.ReceivedUrl + "?msg_ids=" + msg_ids;
        //    String auth = GetBase64Auth();
        //    return SendRequest("GET", url, auth, null); ;
        //}

        /*
         * 1.正确时返回结果{"sendno":"123456","msg_id":"1799597405"}
         * 或者 {"sendno":"0","msg_id":"351403900"}
         * 2.入参json完全正确，但找不到要到达的设备。错误时：返回
         * {"msg_id": 3125719446, "error": {"message": "cannot find user by this audience", "code": 1011}}
         * 3.传入空字符串 或者 非json格式，或者没有必须的选项：{"error": {"message": "Missing parameter", "code": 1002}}
         * 传入的键（键区分大小写）、值不符合要求 {"error": {"message": "Audience value must be JSON Array format!", "code": 1003}}
        */

        /// <summary>
        /// 将返回的json转换为Hashtable对象
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>

        public Hashtable JsonToHashtable(string jsonString)
        {
            /*
             * 正确时返回结果{"sendno":"123456","msg_id":"1799597405"}
             * {"sendno":"0","msg_id":"351403900"}
             * 入参json完全正确，但找不到要到达的设备。错误时：返回 {"msg_id": 3125719446, "error": {"message": "cannot find user by this audience", "code": 1011}}
             * 传入空字符串 或者 非json格式，或者没有必须的选项：{"error": {"message": "Missing parameter", "code": 1002}}
             * 传入的键值不符合要求 {"error": {"message": "Audience value must be JSON Array format!", "code": 1003}}  键区分大小写
            */

            Hashtable ht = new Hashtable();
            object json = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString);

            //返回的结果一定是一个json对象
            Newtonsoft.Json.Linq.JObject jsonObject = json as Newtonsoft.Json.Linq.JObject;
            if (jsonObject == null)
            {
                return ht;
            }
            foreach (Newtonsoft.Json.Linq.JProperty jProperty in jsonObject.Properties())
            {
                Newtonsoft.Json.Linq.JToken jToken = jProperty.Value;
                string value = "";
                if (jToken != null)
                {
                    value = jToken.ToString();
                }
                ht.Add(jProperty.Name, value);
            }
            return ht;
        }

        /// <summary>
        /// 根据json返回的结果判断是否推送成功
        /// </summary>
        /// <param name="jsonString">响应的json</param>
        /// <param name="errorMessage">错误信息</param>
        /// <param name="errorCode">错误号</param>
        /// <returns></returns>

        public bool IsSuccess(string jsonString, out string errorMessage, out string errorCode)
        {
            Hashtable ht = JsonToHashtable(jsonString);
            errorMessage = "";
            errorCode = "";
            foreach (string key in ht.Keys)
            {
                //如果存在error键,说明推送出错
                if (key == "error")
                {
                    string errJson = ht[key].ToString();
                    Hashtable htError = JsonToHashtable(errJson);
                    errorMessage = htError["message"].ToString();
                    errorCode = htError["code"].ToString();
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 根据返回的响应json来判断推送是否成功，成功时记录sendno与msg_id。
        /// 失败时记录错误信息errorMessage、错误号errCode等

        /// </summary>
        /// <param name="jsonString">响应的json</param>
        /// <param name="errorMessage">错误信息</param>
        /// <param name="errorCode">错误号</param>
        /// <param name="sendno">用户自定义的推送编号（从序列号中获取），不设置则为0，成功后返回该编号</param>
        /// <param name="msg_id">极光服务器处理后返回的信息编号</param>
        /// <returns></returns>

        public bool IsSuccess(string jsonString, out string errorMessage, out string errorCode, out string sendno, out string msg_id)
        {
            bool result = IsSuccess(jsonString, out errorMessage, out errorCode);
            Hashtable ht = JsonToHashtable(jsonString);
            sendno = "";
            msg_id = "";

            if (result) //推送成功时，只有键sendno、msg_id
            {
                sendno = ht["sendno"].ToString();
                msg_id = ht["msg_id"].ToString();
            }
            else //如果失败时存在msg_id键，则记录msg_id的值
            {
                if (ht.ContainsKey("msg_id"))
                {
                    msg_id = ht["msg_id"].ToString();
                }
            }
            return result;
        }

        /// <summary>
        /// 将返回的json转换为字典Dictionary对象
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>

        public Dictionary<string, object> JsonToDictionary(string jsonString)
        {
            Dictionary<string, object> ht = new Dictionary<string, object>();
            object json = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString);

            //返回的结果一定是一个json对象
            Newtonsoft.Json.Linq.JObject jsonObject = json as Newtonsoft.Json.Linq.JObject;
            if (jsonObject == null)
            {
                return ht;
            }
            foreach (Newtonsoft.Json.Linq.JProperty jProperty in jsonObject.Properties())
            {
                Newtonsoft.Json.Linq.JToken jToken = jProperty.Value;
                string value = "";
                if (jToken != null)
                {
                    value = jToken.ToString();
                }
                ht.Add(jProperty.Name, value);
            }
            return ht;
        }

        #endregion V3JPush


        public ResultModel GetJPushValidate()
        {
            var result = new ResultModel();

            return result;
        }

    }
}