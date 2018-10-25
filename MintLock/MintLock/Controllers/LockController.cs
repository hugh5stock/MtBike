using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;

using MintLockService.Results;
using YouTingParking.Models;
using MintLock.WSServer;
using System.Web.Http.Description;
using MintLock.Log;
using System.Text;

namespace MintLockService.Controllers
{
    [RoutePrefix("api/Lock")]
    public class LockController : ApiController
    {


        [HttpGet]
        [Route("Open/{deviceId}")]
        [ResponseType(typeof(OpenLockResponse))]
        public OpenLockResponse Open(string deviceId)
        {
            return LockServer.Instance.OpenLock(deviceId);
        }
        [HttpGet]
        [Route("EncryptKey/{timestamp}")]
        public string GetEncryptKey(string timestamp)
        { 
            timestamp = timestamp.PadRight(16, '0').ToUpper();
            string key = "k846eudn4jshaw7e";

            string mac = AESEncrypt(timestamp, key);
            return mac;
        }


        public long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            var start = new DateTime(1970, 1, 1, 0, 0, 0, dateTime.Kind);
            return Convert.ToInt64((dateTime - start).TotalSeconds);
        }


        public string AESEncrypt(string encryptStr, string key)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(encryptStr);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

    }
}
