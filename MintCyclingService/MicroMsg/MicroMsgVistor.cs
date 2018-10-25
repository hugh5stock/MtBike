using MintCyclingService.Utils;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Security.Cryptography;
using System.Text;

namespace MintCyclingService.MicroMsg
{
    public class MicroMsgVistor
    {
        private string appId = "wx11d939e2e3ccb5a3";
        private string secretId = "feb313b5b9ae88cf87b896997a1daa1d";

        /// <summary>
        /// 向微信服务器获取OpenID和sessionKey
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public ResultModel CheckMicroMsg(string code)
        {
            var result = new ResultModel();

            try
            {
                RestClient restClient = new RestClient("https://api.weixin.qq.com/sns/jscode2session");
                RestRequest restRequest = new RestRequest(Method.GET);
                restRequest.AddQueryParameter("appid", appId);
                restRequest.AddQueryParameter("secret", secretId);
                restRequest.AddQueryParameter("js_code", code);
                IRestResponse iRestResponse = restClient.Execute(restRequest);
                string jsonStr = iRestResponse.Content;
                JObject jsonObj = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(jsonStr);

                // openid string 用户唯一标识
                string openid = jsonObj["openid"]?.ToString();
                // session_key string 会话密钥
                string session_key = jsonObj["session_key"]?.ToString();

                if (openid == null || session_key == null)
                {
                    result.IsSuccess = false;
                }
                result.ResObject = jsonObj;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Message = e.Message;
            }
            return result;
        }

        /// <summary>
        /// 微信数据AES解密
        /// </summary>
        /// <param name="encryptedStr">密文字符串</param>
        /// <param name="AesKey">秘钥</param>
        /// <param name="stringAesIV">16位初始向量</param>
        /// <returns></returns>
        public static string Decrypt(string encryptedStr, string key, string iv)
        {
            try
            {
                byte[] encryptedData = Convert.FromBase64String(encryptedStr);  // strToToHexByte(text);
                RijndaelManaged rijndaelCipher = new RijndaelManaged();
                rijndaelCipher.Key = Convert.FromBase64String(key); // Encoding.UTF8.GetBytes(AesKey);
                rijndaelCipher.IV = Convert.FromBase64String(iv);   // Encoding.UTF8.GetBytes(AesIV);
                rijndaelCipher.Mode = CipherMode.CBC;
                rijndaelCipher.Padding = PaddingMode.PKCS7;
                ICryptoTransform transform = rijndaelCipher.CreateDecryptor();
                byte[] plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                string result = Encoding.Default.GetString(plainText);
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
